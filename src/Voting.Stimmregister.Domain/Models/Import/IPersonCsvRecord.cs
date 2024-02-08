// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models.Import;

/// <summary>
/// Person csv import model interface.
/// </summary>
public interface IPersonCsvRecord : IMunicipalityCsvRecord
{
    /// <summary>
    /// Gets or sets the 'eCH-0044:vnType' AHV number, i.e. '7560000111122' for '756.0000.1111.22'.
    /// A person is uniquely identified by the AHV number (AHVN). The AHVN can exist several times for the same person if several residential status exist (HWS, NWS).
    /// </summary>
    long? Vn { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the person entry as provided by the source system, i.e. '123456789'.
    /// </summary>
    string SourceSystemId { get; set; }
}
