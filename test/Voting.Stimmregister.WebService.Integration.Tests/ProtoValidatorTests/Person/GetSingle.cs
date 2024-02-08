// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Lib.Testing.Validation;
using Voting.Stimmregister.Proto.V1.Services.Requests;

namespace Voting.Stimmregister.WebService.Integration.Tests.ProtoValidatorTests.Person;

public class GetSingle : ProtoValidatorBaseTest<PersonServiceGetSingleRequest>
{
    public static PersonServiceGetSingleRequest NewValidRequest(Action<PersonServiceGetSingleRequest>? action = null)
    {
        var request = new PersonServiceGetSingleRequest
        {
            RegisterId = Guid.NewGuid().ToString(),
        };

        action?.Invoke(request);
        return request;
    }

    protected override IEnumerable<PersonServiceGetSingleRequest> NotOkMessages()
    {
        yield return NewValidRequest(x => x.RegisterId = string.Empty);
        yield return NewValidRequest(x => x.RegisterId = "not-a-guid-value");
    }

    protected override IEnumerable<PersonServiceGetSingleRequest> OkMessages()
    {
        yield return NewValidRequest();
    }
}
