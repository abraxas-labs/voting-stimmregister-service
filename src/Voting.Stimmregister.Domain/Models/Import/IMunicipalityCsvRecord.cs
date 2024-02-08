// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models.Import;

/// <summary>
/// Person csv import model interface.
/// </summary>
public interface IMunicipalityCsvRecord
{
    /// <summary>
    /// Gets or sets the eCH-0007:swissMunicipalityType, i.e. '3203'.
    /// </summary>
    int MunicipalityId { get; set; }

    /// <summary>
    /// Gets a record identifier for troubleshooting if a record couldn't be imported due to record validation errors.
    /// </summary>
    /// <returns>A record identifier literal.</returns>
    string BuildRecordIdentifier();
}
