// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.EVoting.Kewr;
using Voting.Stimmregister.Abstractions.Adapter.EVoting.Loganto;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Constants.EVoting;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.EVoting;
using Voting.Stimmregister.Domain.Utils;

namespace Voting.Stimmregister.Core.Services;

public class EVotingService : IEVotingService
{
    private readonly ILogger<EVotingService> _logger;
    private readonly IPermissionService _permissionService;
    private readonly IKewrAdapter _kewrAdapter;
    private readonly ILogantoAdapter _logantoAdapter;
    private readonly IMapper _mapper;
    private readonly ITracingService _tracingService;
    private readonly IDataContext _dbContext;
    private readonly IEVoterRepository _voterRepository;
    private readonly IEVoterAuditRepository _voterAuditRepo;
    private readonly IClock _clock;
    private readonly EVotingConfig _config;
    private readonly IPersonService _personService;

    public EVotingService(
        ILogger<EVotingService> logger,
        IPermissionService permissionService,
        IKewrAdapter kewrAdapter,
        ILogantoAdapter logantoAdapter,
        IMapper mapper,
        ITracingService tracingService,
        IDataContext dbContext,
        IEVoterRepository voterRepo,
        IEVoterAuditRepository voterAuditRepo,
        IClock clock,
        EVotingConfig config,
        IPersonService personService)
    {
        _logger = logger;
        _permissionService = permissionService;
        _kewrAdapter = kewrAdapter;
        _logantoAdapter = logantoAdapter;
        _mapper = mapper;
        _tracingService = tracingService;
        _dbContext = dbContext;
        _voterRepository = voterRepo;
        _voterAuditRepo = voterAuditRepo;
        _clock = clock;
        _config = config;
        _personService = personService;
    }

    public async Task<EVotingInformationModel> GetEVotingInformation(Ahvn13 ahvn13, short bfsCanton)
    {
        var eVotingInformation = new EVotingInformationModel();

        // Get local voter data by AHVN13 if it was already stored at any time in the past.
        var voter = await _voterRepository.Query().FirstOrDefaultAsync(e => e.Ahvn13 == ahvn13.ToNumber());
        if (voter == null)
        {
            _logger.LogDebug("No e-voting data available for the given AHVN13.");
        }

        // Get current voting status based on the local eService data storage.
        if (voter?.EVoterFlag != null)
        {
            eVotingInformation.Status = voter.EVoterFlag.Value ? VotingStatus.Registered : VotingStatus.Unregistered;
        }
        else
        {
            _logger.LogDebug("Could not read the e-voting status for the given AHVN13.");
        }

        // Get person data to evaluate voting permissions.
        eVotingInformation.Person = await GetEVotingPerson(ahvn13, bfsCanton);
        eVotingInformation.RegisteredEVotersInCanton =
            await _voterRepository.Query().CountAsync(x => x.BfsCanton == bfsCanton && x.EVoterFlag == true);

        eVotingInformation.RegisteredEVotersInMunicipality =
            await _voterRepository.Query().CountAsync(x => x.BfsMunicipality == eVotingInformation.Person.BfsMunicipality && x.EVoterFlag == true);

        return eVotingInformation;
    }

    public async Task RegisterForEVoting(Ahvn13 ahvn13, short bfsCanton)
    {
        // Get person details
        var person = await GetEVotingPerson(ahvn13, bfsCanton);

        if (!_config.EnableKewrAndLoganto)
        {
            await SaveEVoter(ahvn13, bfsCanton, person.BfsMunicipality, true, null, null);
            return;
        }

        var eVotingResponse = await _logantoAdapter.RegisterForEVoting(new EVotingRegisterRequestModel(ahvn13)
        {
            MunicipalityId = person.OeidMunicipality,
            RegisterFrom = _clock.UtcNow.AddHours(1),
        });

        // Enable eVoting Flag depending on the returned status code
        bool? eVoterFlag = null;
        if (eVotingResponse.ReturnCode < EkProcessStatusCodes.Status200)
        {
            eVoterFlag = true;
        }

        await SaveEVoter(ahvn13, bfsCanton, person.BfsMunicipality, eVoterFlag, eVotingResponse.ReturnCode, eVotingResponse.Message);

        if (eVotingResponse.ReturnCode >= EkProcessStatusCodes.Status200)
        {
            var processStatusCode = GetProcessStatusCodeByEVotingResponse(eVotingResponse.ReturnCode);
            throw new EVotingRegistrationException($"Registration failed. Subsystem failed with message <{eVotingResponse.Message}> and code <{eVotingResponse.ReturnCode}>.", processStatusCode);
        }
    }

    public async Task UnregisterFromEVoting(Ahvn13 ahvn13, short bfsCanton)
    {
        var person = await GetEVotingPerson(ahvn13, bfsCanton);

        if (!_config.EnableKewrAndLoganto)
        {
            await SaveEVoter(person.Ahvn13, bfsCanton, person.BfsMunicipality, false, null, null);
            return;
        }

        var eVotingResponse = await _logantoAdapter.UnregisterFromEVoting(new EVotingUnregisterRequestModel(ahvn13)
        {
            MunicipalityId = person.OeidMunicipality,
            UnregisterOn = _clock.UtcNow.AddHours(1),
        });

        // Disable eVoting Flag depending on the returned status code
        bool? eVoterFlag = null;
        if (eVotingResponse.ReturnCode < EkProcessStatusCodes.Status200)
        {
            eVoterFlag = false;
        }

        await SaveEVoter(person.Ahvn13, bfsCanton, person.BfsMunicipality, eVoterFlag, eVotingResponse.ReturnCode, eVotingResponse.Message);

        if (eVotingResponse.ReturnCode >= EkProcessStatusCodes.Status200)
        {
            var processStatusCode = GetProcessStatusCodeByEVotingResponse(eVotingResponse.ReturnCode);
            throw new EVotingRegistrationException($"Unregistration failed. Subsystem failed with message <{eVotingResponse.Message}> and code <{eVotingResponse.ReturnCode}>", processStatusCode);
        }
    }

    public async Task<EVotingReportModel?> GetEVotingReport(Ahvn13 ahvn13)
    {
        var eVoter = await _voterRepository
            .Query()
            .FirstOrDefaultAsync(v => v.Ahvn13 == ahvn13.ToNumber());

        if (eVoter == null)
        {
            return null;
        }

        var audits = await _voterAuditRepo
            .Query()
            .Where(a => a.EVoterId == eVoter.Id)
            .OrderByDescending(a => a.AuditInfo.CreatedAt)
            .ToListAsync();

        var result = _mapper.Map<EVotingReportModel>(eVoter);
        result.Audits = _mapper.Map<ICollection<EVotingAuditModel>>(audits);

        return result;
    }

    private async Task<EVotingPersonDataModel> GetEVotingPerson(Ahvn13 ahvn13, short bfsCanton)
    {
        if (_config.EnableKewrAndLoganto)
        {
            return await _kewrAdapter.GetPersonWithMainResidenceByAhvn13(ahvn13, bfsCanton);
        }

        var person = await _personService.GetSingleOrDefaultWithVotingRightsByVnAndCantonBfsIgnoreAcl(ahvn13.ToNumber(), bfsCanton);
        if (person == null)
        {
            throw new EVotingValidationException(
                "Keine Person mit AHVN13 gefunden.",
                ProcessStatusCode.PersonNotFound);
        }

        return new EVotingPersonDataModel(ahvn13)
        {
            AllowedToVote = person.IsVotingAllowed,
            BfsMunicipality = (short)person.MunicipalityId,
            OeidMunicipality = person.MunicipalityId,
            Nationality = person.CountryNameShort,
            DateOfBirth = person.DateOfBirth,
            OfficialName = person.OfficialName,
            FirstName = person.FirstName,
            Sex = person.Sex,
            Address = MapAddress(person),
        };
    }

    private ProcessStatusCode GetProcessStatusCodeByEVotingResponse(short returnCode)
    {
        return returnCode switch
        {
            >= 0 and < EkProcessStatusCodes.Status200 => ProcessStatusCode.Success,
            >= EkProcessStatusCodes.Status200 and < EkProcessStatusCodes.Status900 => ProcessStatusCode.LogantoServiceBusinessError,
            >= EkProcessStatusCodes.Status900 => ProcessStatusCode.LogantoServiceRequestError,
            _ => ProcessStatusCode.Unknown,
        };
    }

    private async Task<EVoterEntity> AddOrUpdateEVoter(Ahvn13 ahvn13, short bfsCanton, short bfsMunicipality, bool? eVoterFlag)
    {
        var voter = await _voterRepository.Query().FirstOrDefaultAsync(e => e.Ahvn13 == ahvn13.ToNumber());
        var existingVoter = voter != null;

        // Only update eVoter if a valid eVoter flag is passed
        if (existingVoter && eVoterFlag == null)
        {
            return voter!;
        }

        voter ??= new EVoterEntity { Ahvn13 = ahvn13.ToNumber() };

        voter.BfsCanton = bfsCanton;
        voter.BfsMunicipality = bfsMunicipality;
        voter.EVoterFlag = eVoterFlag;
        voter.ContextId = _tracingService.ContextId;

        if (existingVoter)
        {
            _permissionService.SetModified(voter);
            await _voterRepository.Update(voter);
        }
        else
        {
            _permissionService.SetCreated(voter);
            await _voterRepository.Create(voter);
        }

        return voter;
    }

    private async Task AddEVoterAudit(EVoterEntity voter, short? statusCode, string? message)
    {
        var auditEntry = new EVoterAuditEntity
        {
            EVoterId = voter.Id,
            BfsCanton = voter.BfsCanton,
            BfsMunicipality = voter.BfsMunicipality,
            EVoterFlag = voter.EVoterFlag,
            ContextId = voter.ContextId,
            StatusCode = statusCode,
            StatusMessage = message,
            EVoterAuditInfo = voter.AuditInfo.Clone(),
        };

        _permissionService.SetCreated(auditEntry);
        await _voterAuditRepo.Create(auditEntry);
    }

    private EVotingAddressModel? MapAddress(PersonEntityModel person)
    {
        if (PersonUtil.HasContactAddress(person))
        {
            return new EVotingAddressModel
            {
                Street = person.ContactAddressStreet ?? string.Empty,
                Town = person.ContactAddressTown ?? string.Empty,
                HouseNumber = person.ContactAddressHouseNumber ?? string.Empty,
                ZipCode = person.ContactAddressZipCode ?? string.Empty,
            };
        }

        if (PersonUtil.HasResidenceAddress(person))
        {
            return new EVotingAddressModel
            {
                Street = person.ResidenceAddressStreet ?? string.Empty,
                Town = person.ResidenceAddressTown ?? string.Empty,
                HouseNumber = person.ResidenceAddressHouseNumber ?? string.Empty,
                ZipCode = person.ResidenceAddressZipCode ?? string.Empty,
            };
        }

        return null;
    }

    private async Task SaveEVoter(Ahvn13 ahvn13, short bfsCanton, short bfsMunicipality, bool? eVoterFlag, short? statusCode, string? message)
    {
        var eVoter = await AddOrUpdateEVoter(ahvn13, bfsCanton, bfsMunicipality, eVoterFlag);
        await AddEVoterAudit(eVoter, statusCode, message);

        await _dbContext.SaveChangesAsync();
    }
}
