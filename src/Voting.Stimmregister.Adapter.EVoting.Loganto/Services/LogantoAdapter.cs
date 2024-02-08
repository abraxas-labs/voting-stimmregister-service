// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Adapter.EVoting.Loganto;
using Voting.Stimmregister.Adapter.EVoting.Loganto.Exceptions;
using Voting.Stimmregister.Adapter.EVoting.Loganto.Models;
using Voting.Stimmregister.Domain.Constants.EVoting;
using Voting.Stimmregister.Domain.Models.EVoting;

namespace Voting.Stimmregister.Adapter.EVoting.Loganto.Services;

public class LogantoAdapter : ILogantoAdapter
{
    private readonly HttpClient _httpClient;

    public LogantoAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EVotingRegisterResponseModel> RegisterForEVoting(EVotingRegisterRequestModel model)
    {
        var requestModel = new RegisterModel
        {
            EVoterFlag = new EVoterFlagModel
            {
                From = model.RegisterFrom.ToString("yyyy-MM-dd"),
            },

            Identification = new IdentificationModel
            {
                Ahvn13 = model.Ahvn13.ToNumber(),
                MunicipalityOeid = model.MunicipalityId,
            },
        };

        var response = await _httpClient.PostAsJsonAsync("v1/setFlag", requestModel);
        if (!response.IsSuccessStatusCode)
        {
            throw new LogantoServiceException(
                $"Es ist ein Fehler bei der Registration für eVoting aufgetreten (HTTP Status Code: {response.StatusCode})",
                ProcessStatusCode.LogantoServiceRequestError);
        }

        var result = await response.Content.ReadFromJsonAsync<StatusResponseModel>();

        if (result == null)
        {
            throw new LogantoServiceException(
                "Es ist ein Fehler bei der Deserialisierung der eVoting Registrations-Antwort aufgetreten.",
                ProcessStatusCode.LogantoServiceDataError);
        }

        return new EVotingRegisterResponseModel
        {
            Message = result.Message,
            ReturnCode = result.ReturnCode,
        };
    }

    public async Task<EVotingUnregisterResponseModel> UnregisterFromEVoting(EVotingUnregisterRequestModel model)
    {
        var requestModel = new UnregisterModel
        {
            EVoterFlag = new EVoterFlagModel
            {
                To = model.UnregisterOn.ToString("yyyy-MM-dd"),
            },

            Identification = new IdentificationModel
            {
                Ahvn13 = model.Ahvn13.ToNumber(),
                MunicipalityOeid = model.MunicipalityId,
            },
        };

        var response = await _httpClient.PostAsJsonAsync("v1/setFlag", requestModel);
        if (!response.IsSuccessStatusCode)
        {
            throw new LogantoServiceException(
                $"Es ist ein Fehler bei der Abmeldung für eVoting aufgetreten (HTTP Status Code: {response.StatusCode})",
                ProcessStatusCode.LogantoServiceRequestError);
        }

        var result = await response.Content.ReadFromJsonAsync<StatusResponseModel>();

        if (result == null)
        {
            throw new LogantoServiceException(
                "Es ist ein Fehler bei der Deserialisierung der eVoting Abmelde-Antwort aufgetreten.",
                ProcessStatusCode.LogantoServiceDataError);
        }

        return new EVotingUnregisterResponseModel
        {
            Message = result.Message,
            ReturnCode = result.ReturnCode,
        };
    }
}
