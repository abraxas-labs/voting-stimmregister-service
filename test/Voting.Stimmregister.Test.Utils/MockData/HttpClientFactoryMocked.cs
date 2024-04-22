// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Voting.Stimmregister.Adapter.EVoting.Kewr.Models;
using Voting.Stimmregister.Adapter.EVoting.Loganto.Models;
using Voting.Stimmregister.Test.Utils.MockData.EVoting;

namespace Voting.Stimmregister.Test.Utils.MockData;

public class HttpClientFactoryMocked : IHttpClientFactory
{
    public static HttpResponseMessage KewrSearchResponse { get; set; } = new HttpResponseMessage();

    public static HttpResponseMessage KewrGetResponse { get; set; } = new HttpResponseMessage();

    public static HttpResponseMessage LogantoSetFlagResponse { get; set; } = new HttpResponseMessage();

    public static HttpResponseMessage KewrSearchWithHttpStatusErrorCode => new()
    {
        StatusCode = HttpStatusCode.InternalServerError,
    };

    public static HttpResponseMessage KewrSearchWithoutActivePerson => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
                new SearchResultModel
                {
                    MaxResult = 60,
                    MaxResultsExceeded = false,
                    SimpleResidentPojos = new List<PersonIdModel>
                    {
                        new PersonIdModel
                        {
                             ResidentNr = 1,
                             Status = "inaktiv",
                             Mv = "HWS",
                             OeId = 300,
                        },
                    },
                })),
    };

    public static HttpResponseMessage KewrSearchWithoutMainResidence => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
                new SearchResultModel
                {
                    MaxResult = 60,
                    MaxResultsExceeded = false,
                    SimpleResidentPojos = new List<PersonIdModel>
                    {
                        new PersonIdModel
                        {
                             ResidentNr = 1,
                             Status = "aktiv",
                             Mv = "NWS",
                             OeId = 300,
                        },
                    },
                })),
    };

    public static HttpResponseMessage KewrSearchWithMultipleMatches => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
                new SearchResultModel
                {
                    MaxResult = 60,
                    MaxResultsExceeded = false,
                    SimpleResidentPojos = new List<PersonIdModel>
                    {
                        new PersonIdModel
                        {
                             ResidentNr = 1,
                             Status = "aktiv",
                             Mv = "HWS",
                             OeId = 300,
                        },
                        new PersonIdModel
                        {
                             ResidentNr = 2,
                             Status = "aktiv",
                             Mv = "HWS",
                             OeId = 300,
                        },
                    },
                })),
    };

    public static HttpResponseMessage KewrSearchValidResult => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
                new SearchResultModel
                {
                    MaxResult = 60,
                    MaxResultsExceeded = false,
                    SimpleResidentPojos = new List<PersonIdModel>
                    {
                        new PersonIdModel
                        {
                             ResidentNr = 1,
                             Status = "aktiv",
                             Mv = "HWS",
                             OeId = 300,
                        },
                    },
                })),
    };

    public static HttpResponseMessage KewrGetWithHttpStatusErrorCode => new()
    {
        StatusCode = HttpStatusCode.InternalServerError,
    };

    public static HttpResponseMessage KewrGetWithoutAhvn13 => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new PersonModel
            {
                ResidentNr = 1,
                LocalCommunityBfs = EVotingBfsMunicipalityMockedData.BfsStGallen,
                Ahvn13 = string.Empty,
                Nationality = "Schweiz",
                NonVoting = false,
            })),
    };

    public static HttpResponseMessage KewrGetWithoutVotingPermission => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new PersonModel
            {
                ResidentNr = 1,
                LocalCommunityBfs = EVotingBfsMunicipalityMockedData.BfsStGallen,
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
                Nationality = "Schweiz",
                NonVoting = true,
                DateOfBirth = new DateOfBirthModel
                {
                    Day = 1,
                    Month = 1,
                    Year = (short)((short)DateTime.Now.Year - 18),
                },
            })),
    };

    public static HttpResponseMessage KewrGetTooYoungForVoting => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new PersonModel
            {
                ResidentNr = 1,
                LocalCommunityBfs = EVotingBfsMunicipalityMockedData.BfsStGallen,
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
                Nationality = "Schweiz",
                NonVoting = false,
                DateOfBirth = new DateOfBirthModel
                {
                    Day = 1,
                    Month = 1,
                    Year = (short)((short)DateTime.Now.Year - 17),
                },
            })),
    };

    public static HttpResponseMessage KewrGetWithoutSwissNationality => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new PersonModel
            {
                ResidentNr = 1,
                LocalCommunityBfs = EVotingBfsMunicipalityMockedData.BfsStGallen,
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
                Nationality = "Österreich",
                NonVoting = false,
                DateOfBirth = new DateOfBirthModel
                {
                    Day = 1,
                    Month = 1,
                    Year = (short)((short)DateTime.Now.Year - 18),
                },
            })),
    };

    public static HttpResponseMessage KewrGetWithVotingPermission => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new PersonModel
            {
                ResidentNr = 1,
                LocalCommunityBfs = EVotingBfsMunicipalityMockedData.BfsStGallen,
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
                Nationality = "Schweiz",
                NonVoting = false,
                DateOfBirth = new DateOfBirthModel
                {
                    Day = 1,
                    Month = 1,
                    Year = (short)((short)DateTime.Now.Year - 18),
                },
            })),
    };

    public static HttpResponseMessage LogantoSetFlagHttpError => new()
    {
        StatusCode = HttpStatusCode.BadRequest,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new StatusResponseModel
            {
                ReturnCode = 200,
                Message = "lorem ipsum",
            })),
    };

    public static HttpResponseMessage LogantoServiceBusinessError => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new StatusResponseModel
            {
                ReturnCode = 200,
                Message = "lorem ipsum",
            })),
    };

    public static HttpResponseMessage LogantoServiceDataError => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new StatusResponseModel
            {
                ReturnCode = 901,
                Message = "lorem ipsum",
            })),
    };

    public static HttpResponseMessage LogantoSetFlagSuccess => new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            new StatusResponseModel
            {
                ReturnCode = 100,
                Message = "lorem ipsum",
            })),
    };

    public HttpClient CreateClient(string name)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        // KEWR search
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri != null && r.RequestUri.PathAndQuery.Contains("/search")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(KewrSearchResponse);

        // KEWR get
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri != null && r.RequestUri.PathAndQuery.Contains("/residents")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(KewrGetResponse);

        // LOGANTO setFlag
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri != null && r.RequestUri.PathAndQuery.Contains("/setFlag")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(LogantoSetFlagResponse);

        return new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://test-domain.invalid"), };
    }
}
