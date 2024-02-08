// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Lib.Testing.Validation;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;

namespace Voting.Stimmregister.WebService.Integration.Tests.ProtoValidatorTests.ImportStatistics;

public class GetHistory : ProtoValidatorBaseTest<GetImportStatisticHistoryRequest>
{
    public static GetImportStatisticHistoryRequest NewValidRequest(Action<GetImportStatisticHistoryRequest>? action = null)
    {
        var request = new GetImportStatisticHistoryRequest
        {
            ImportType = ImportType.DomainOfInfluence,
            ImportSourceSystem = ImportSourceSystem.Loganto,
            MunicipalityId = 3214,
            Paging = new PagingModel
            {
                PageIndex = 0,
                PageSize = 10,
            },
        };

        action?.Invoke(request);
        return request;
    }

    protected override IEnumerable<GetImportStatisticHistoryRequest> NotOkMessages()
    {
        yield return NewValidRequest(x => x.Paging.PageIndex = -1);
        yield return NewValidRequest(x => x.Paging.PageSize = 0);
    }

    protected override IEnumerable<GetImportStatisticHistoryRequest> OkMessages()
    {
        yield return NewValidRequest();
        yield return NewValidRequest(x => x.MunicipalityId = 0);
    }
}
