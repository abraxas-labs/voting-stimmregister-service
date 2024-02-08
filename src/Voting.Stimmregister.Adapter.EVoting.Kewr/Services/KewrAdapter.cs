// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.EVoting.Kewr;
using Voting.Stimmregister.Adapter.EVoting.Kewr.Converters;
using Voting.Stimmregister.Adapter.EVoting.Kewr.Exceptions;
using Voting.Stimmregister.Adapter.EVoting.Kewr.Models;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Constants.EVoting;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models.EVoting;

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.Services;

public class KewrAdapter : IKewrAdapter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        Converters = { new DateTimeJsonConverter() },
    };

    private readonly EVotingConfig _config;
    private readonly ILogger<KewrAdapter> _logger;
    private readonly HttpClient _httpClient;
    private readonly IClock _clock;

    public KewrAdapter(
        EVotingConfig config,
        ILogger<KewrAdapter> logger,
        HttpClient httpClient,
        IClock clock)
    {
        _config = config;
        _logger = logger;
        _httpClient = httpClient;
        _clock = clock;
    }

    public async Task<EVotingPersonDataModel> GetPersonWithMainResidenceByAhvn13(Ahvn13 ahvn13, short bfsCanton)
    {
        // Evaluate main residence
        var searchResult = await FindPersonByAhvn13(ahvn13, bfsCanton);
        var persons = searchResult.SimpleResidentPojos;
        var mainResidenceMainResidence = EvaluateMainResidence(persons);

        // Get person detail information
        var orgUnitId = GetOrganisationUnitId(bfsCanton);
        var personDetailUrl = $"organisationUnits/{orgUnitId}/residents/{mainResidenceMainResidence.ResidentNr}/residentOe/{mainResidenceMainResidence.OeId}";
        var response = await _httpClient.GetAsync(personDetailUrl);
        if (!response.IsSuccessStatusCode)
        {
            throw new KewrServiceException(
                $"Es ist ein Fehler bei der Abfrage von Personendetails aufgetreten (HTTP Status Code: {response.StatusCode})",
                ProcessStatusCode.KewrServiceRequestError);
        }

        var result = await response.Content.ReadFromJsonAsync<PersonModel>(JsonOptions);

        if (!Ahvn13.TryParse(result?.Ahvn13, out var resultingAhvn13))
        {
            throw new KewrServiceException(
                "Es ist ein Fehler bei der Deserialisierung der Personendetails aufgetreten.",
                ProcessStatusCode.KewrServiceDataError);
        }

        _logger.LogDebug($"{nameof(GetPersonWithMainResidenceByAhvn13)}: Successfully retrieved person details from kewr service.");

        return new EVotingPersonDataModel(resultingAhvn13)
        {
            AllowedToVote = !result.NonVoting,
            BfsMunicipality = result.LocalCommunityBfs,
            OeidMunicipality = mainResidenceMainResidence.OeId,
            Nationality = result.Nationality,
            DateOfBirth = new DateOnly(result.DateOfBirth.Year, result.DateOfBirth.Month, result.DateOfBirth.Day),
            OfficialName = result.OfficialName,
            FirstName = result.FirstName,
            Sex = MapGenderToSex(result.Gender),
            Address = MapAddress(result.LivingAddress),
        };
    }

    public async Task<SearchResultModel> FindPersonByAhvn13(Ahvn13 ahvn13, short bfsCanton)
    {
        var orgUnitId = GetOrganisationUnitId(bfsCanton);
        var response = await _httpClient.GetAsync($"organisationUnits/{orgUnitId}/search?ahvExtnumber={ahvn13}");
        if (!response.IsSuccessStatusCode)
        {
            throw new KewrServiceException(
                $"Es ist ein Fehler bei der Personensuche aufgetreten (HTTP Status Code: {response.StatusCode})",
                ProcessStatusCode.KewrServiceRequestError);
        }

        var result = await response.Content.ReadFromJsonAsync<SearchResultModel>(JsonOptions);

        if (result == null)
        {
            throw new KewrServiceException(
                "Es ist ein Fehler bei der Deserialisierung der Personensuche aufgetreten.",
                ProcessStatusCode.KewrServiceDataError);
        }

        _logger.LogInformation("Found {count} person results", result?.SimpleResidentPojos?.Count());

        return result!;
    }

    private PersonIdModel EvaluateMainResidence(IEnumerable<PersonIdModel> persons)
    {
        var activePersons = persons
            .Where(p => p.Status.Equals("aktiv", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (activePersons.Length == 0)
        {
            throw new KewrServiceException("Es existiert kein aktiver Wohnsitz für die Person.", ProcessStatusCode.KewrServicePersonError);
        }

        var personsWithMainResidence = activePersons
            .Where(p => p.Mv.Equals("HWS", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        return personsWithMainResidence.Length switch
        {
            0 => throw new KewrServiceException("Es existiert kein Hauptwohnsitz für die Person.", ProcessStatusCode.KewrServicePersonError),
            1 => personsWithMainResidence[0],
            _ => throw new KewrServiceException("Es existiert mehr als ein Hauptwohnsitz für die Person.", ProcessStatusCode.KewrServicePersonError),
        };
    }

    /// <summary>
    /// Converts the canton bfs number into the corresponding loganto organisation unit (OE) id.
    /// Loganto internally uses its own OE identifiers. The mapping is not available via a service
    /// and therefore configured through eService appsettigns.
    /// </summary>
    /// <param name="bfsCanton">The BFS canton number.</param>
    /// <returns>The organisation unit (OE) id.</returns>
    private int GetOrganisationUnitId(short bfsCanton)
    {
        if (_config.LogantoOeidToOrgUnitMapping.TryGetValue(bfsCanton.ToString(), out var orgUnitId))
        {
            return orgUnitId;
        }

        throw new EVotingValidationException(
            $"Die Organisationseinheit(OE) für die Kantons BFS Nummer {bfsCanton} existiert nicht. " +
            "Es muss überprüft werden, ob die OE<>BFS Mapping Tabelle akutell ist.",
            ProcessStatusCode.LogantoOrganisationUnitNotFound);
    }

    private SexType MapGenderToSex(GenderModel gender)
    {
        return gender.CodeECH switch
        {
            "1" => SexType.Male,
            "2" => SexType.Female,
            _ => SexType.Undefined,
        };
    }

    private EVotingAddressModel? MapAddress(AddressModel[] addresses)
    {
        var now = _clock.UtcNow;
        var foundAddress = Array.Find(addresses, a => a.ValidFrom <= now && a.ValidUntil >= now);
        if (foundAddress == null)
        {
            return null;
        }

        return new EVotingAddressModel
        {
            Street = foundAddress.Street,
            Town = foundAddress.Town,
            HouseNumber = foundAddress.HouseNumber,
            ZipCode = foundAddress.ZipCode,
        };
    }
}
