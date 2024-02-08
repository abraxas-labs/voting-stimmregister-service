// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.AspNetCore.Authorization;

namespace Voting.Stimmregister.WebService.Auth;

/// <summary>
/// Authorize attribute for the <see cref="Domain.Authorization.Roles.ApiImporter"/> or <see cref="Domain.Authorization.Roles.ManualImporter"/>.
/// </summary>
public class AuthorizeApiOrManualImporterAttribute : AuthorizeAttribute
{
    public AuthorizeApiOrManualImporterAttribute()
    {
        Roles = string.Join(",", Domain.Authorization.Roles.ApiImporter, Domain.Authorization.Roles.ManualImporter);
    }
}
