// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.AspNetCore.Authorization;

namespace Voting.Stimmregister.WebService.Auth;

/// <summary>
/// Authorize attribute for the <see cref="Domain.Authorization.Roles.ManualImporter"/> role.
/// </summary>
public class AuthorizeManualImporterAttribute : AuthorizeAttribute
{
    public AuthorizeManualImporterAttribute()
    {
        Roles = Domain.Authorization.Roles.ManualImporter;
    }
}
