// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.AspNetCore.Authorization;

namespace Voting.Stimmregister.WebService.Auth;

/// <summary>
/// Authorize attribute for the <see cref="Voting.Stimmregister.Domain.Authorization.Roles.ApiImporter"/>.
/// </summary>
public class AuthorizeApiImporterAttribute : AuthorizeAttribute
{
    public AuthorizeApiImporterAttribute()
    {
        Roles = Voting.Stimmregister.Domain.Authorization.Roles.ApiImporter;
    }
}
