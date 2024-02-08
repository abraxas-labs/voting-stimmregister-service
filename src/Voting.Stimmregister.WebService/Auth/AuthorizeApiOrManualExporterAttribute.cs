// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.AspNetCore.Authorization;

namespace Voting.Stimmregister.WebService.Auth;

/// <summary>
/// Authorize attribute for the <see cref="Domain.Authorization.Roles.ApiExporter"/> or <see cref="Domain.Authorization.Roles.ManualExporter"/>.
/// </summary>
public class AuthorizeApiOrManualExporterAttribute : AuthorizeAttribute
{
    public AuthorizeApiOrManualExporterAttribute()
    {
        Roles = string.Join(",", Domain.Authorization.Roles.ApiExporter, Domain.Authorization.Roles.ManualExporter);
    }
}
