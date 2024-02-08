// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Ech0045_4_0;
using Voting.Lib.Ech.Ech0045_4_0.Converter;
using Voting.Stimmregister.Abstractions.Adapter.Ech;
using Voting.Stimmregister.Abstractions.Adapter.Extensions;
using Voting.Stimmregister.Adapter.Ech.Configuration;
using Voting.Stimmregister.Adapter.Ech.Mapping;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Ech.Converter;

public class EchService : IEchService
{
    private readonly Ech0045Serializer _serializer;
    private readonly EchConfig _config;
    private readonly IPersonVoterMapping _personVoterMapping;

    public EchService(
        Ech0045Serializer serializer,
        EchConfig config,
        IPersonVoterMapping personVoterMapping)
    {
        _serializer = serializer;
        _config = config;
        _personVoterMapping = personVoterMapping;
    }

    public Task WriteEch0045(
        PipeWriter writer,
        int numberOfPersons,
        IAsyncEnumerable<PersonEntity> voters,
        CancellationToken ct)
    {
        if (numberOfPersons == 0)
        {
            throw new NoDataException();
        }

        var cantonalRegister = new CantonalRegisterType
        {
            CantonAbbreviation = _config.RegisterIdentification,
            RegisterIdentification = _config.RegisterIdentification.ToString().ToUpper(),
            RegisterName = _config.RegisterName,
        };

        var authority = new AuthorityType { CantonalRegister = cantonalRegister };
        var voterList = new VoterListType
        {
            ReportingAuthority = authority,
            Contest = null,
            NumberOfVoters = numberOfPersons.ToString(),
        };

        var votingPersons = voters.Select(x => _personVoterMapping.ToEchVoter(x), ct);
        return _serializer.WriteXml(writer, voterList, votingPersons, true, ct);
    }
}
