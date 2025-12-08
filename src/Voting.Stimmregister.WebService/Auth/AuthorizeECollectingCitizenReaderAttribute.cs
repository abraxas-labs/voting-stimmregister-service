// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.AspNetCore.Authorization;

namespace Voting.Stimmregister.WebService.Auth;

public class AuthorizeECollectingCitizenReaderAttribute : AuthorizeAttribute
{
    public AuthorizeECollectingCitizenReaderAttribute()
    {
        Roles = Voting.Stimmregister.Domain.Authorization.Roles.ECollectingCitizenReader;
    }
}
