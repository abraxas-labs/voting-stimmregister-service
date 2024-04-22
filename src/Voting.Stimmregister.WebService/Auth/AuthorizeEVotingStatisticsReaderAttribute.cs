// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.AspNetCore.Authorization;
using Voting.Stimmregister.Domain.Authorization;

namespace Voting.Stimmregister.WebService.Auth;

/// <summary>
/// Authorize attribute for the <see cref="Roles.EVotingStatisticsReader"/>.
/// </summary>
public class AuthorizeEVotingStatisticsReaderAttribute : AuthorizeAttribute
{
    public AuthorizeEVotingStatisticsReaderAttribute()
    {
        Roles = Voting.Stimmregister.Domain.Authorization.Roles.EVotingStatisticsReader;
    }
}
