// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.AspNetCore.Authorization;

namespace Voting.Stimmregister.WebService.Auth;

/// <summary>
/// Authorize attribute for the <see cref="Voting.Stimmregister.Domain.Authorization.Roles.EVoting"/>.
/// </summary>
public class AuthorizeEVotingAttribute : AuthorizeAttribute
{
    public AuthorizeEVotingAttribute()
    {
        Roles = Voting.Stimmregister.Domain.Authorization.Roles.EVoting;
    }
}
