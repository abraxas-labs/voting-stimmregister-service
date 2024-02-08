// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Domain.Utils;

/// <summary>
/// Country helper service for ech transformaiton.
/// </summary>
public interface ICountryHelperService
{
    /// <summary>
    /// Gets a country two letter iso code from a passed official country name.
    /// </summary>
    /// <param name="countryShortName">The country name, i.e. 'Austria'.</param>
    /// <param name="bfsCountryNumber">The country name, i.e. '8229'.</param>
    /// <returns>A two letter iso country code, i.e. 'AT'.</returns>
    string? GetCountryTwoLetterIsoCode(string? countryShortName, string? bfsCountryNumber);

    /// <summary>
    /// Gets detailed country information for a given two letter country code.
    /// </summary>
    /// <param name="countryTwoLetterCode">The two-letter code defined in ISO 3166 for the country/region, i.e. 'CH'.</param>
    /// <returns>Detailed country region information.</returns>
    BfsCountryHelperServiceModel? GetCountryInfo(string countryTwoLetterCode);

    /// <summary>
    /// Gets a country two letter iso code from a passed loganto code.
    /// </summary>
    /// <param name="countryLogaId">The country logid from loganto, i.e. 'IRL'.</param>
    /// <returns>A two letter iso country code, i.e. 'IE'.</returns>
    string? GetLogantoCountryTwoLetterIsoCode(string? countryLogaId);

    /// <summary>
    /// Gets a country two letter iso code from a passed loganto code.
    /// </summary>
    /// <param name="countryLogaId">The country logid from loganto, i.e. 'IRL'.</param>
    /// <returns>A two letter iso country code and shorte name de, i.e. 'IE', 'Irland'.</returns>
    CountryHelperServiceResultModel? GetLogantoCountryTwoLetterIsoAndShortNameDe(string? countryLogaId);
}
