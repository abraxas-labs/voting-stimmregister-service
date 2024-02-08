// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// Configuration model for person import settings mapped from appsettings.
/// </summary>
public class PersonImportConfig : BaseImportConfig
{
    public LogantoPersonImportConfig Loganto { get; set; } = new();
}
