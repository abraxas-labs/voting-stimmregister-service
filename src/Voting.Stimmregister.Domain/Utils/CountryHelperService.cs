// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Domain.Utils;

/// <inheritdoc cref="ICountryHelperService"/>
public class CountryHelperService : ICountryHelperService
{
    private const string BfsCountryListFile = "BFSCountryList.xml";
    private const string LogantoCountryListFile = "LogantoCountryList.xml";
    private const string CountryUnknown = "Staat unbekannt";

    private readonly List<BfsCountryHelperServiceModel> _bfsCountryCollection;
    private readonly List<LogantoCountryHelperServiceModel> _logantoCountryCollection;
    private readonly Dictionary<int, BfsCountryHelperServiceModel> _bfsCountryByBfsNumber;

    public CountryHelperService()
    {
        _bfsCountryCollection = GetBfsCountryList();
        _logantoCountryCollection = GetLogantoCountryList();

        _bfsCountryByBfsNumber = GetBfsCountryByBfsNumber();
    }

    /// <inheritdoc/>
    public string? GetCountryTwoLetterIsoCode(string? countryShortName, string? bfsCountryNumber)
    {
        // VOTING-3385: Temporarily fix known invalid mapping until source system has fixed all entries.
        bfsCountryNumber = bfsCountryNumber == "8551" ? "8532" : bfsCountryNumber;

        if (string.IsNullOrEmpty(bfsCountryNumber) || !int.TryParse(bfsCountryNumber, out var bfsNumber))
        {
            return _bfsCountryCollection.Find(c => c.ShortNameEn.Equals(countryShortName, StringComparison.InvariantCultureIgnoreCase))?.Iso2Id;
        }

        if (!_bfsCountryByBfsNumber.TryGetValue(bfsNumber, out var country))
        {
            if (string.IsNullOrEmpty(countryShortName))
            {
                return null;
            }

            return _bfsCountryCollection.Find(c => c.ShortNameEn.Equals(countryShortName, StringComparison.InvariantCultureIgnoreCase))?.Iso2Id;
        }

        return country.Iso2Id;
    }

    /// <inheritdoc/>
    public BfsCountryHelperServiceModel? GetCountryInfo(string countryTwoLetterCode)
    {
        if (string.IsNullOrEmpty(countryTwoLetterCode))
        {
            return null;
        }

        return _bfsCountryCollection.Find(c => c.Iso2Id.Equals(countryTwoLetterCode, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <inheritdoc/>
    public string? GetLogantoCountryTwoLetterIsoCode(string? countryLogaId)
    {
        if (string.IsNullOrEmpty(countryLogaId))
        {
            return null;
        }

        var country = _logantoCountryCollection.Find(c => c.LogaId.Equals(countryLogaId, StringComparison.InvariantCultureIgnoreCase));

        if (country == null)
        {
            return _bfsCountryCollection.Find(c => c.Iso2Id.Equals(countryLogaId, StringComparison.InvariantCultureIgnoreCase))?.Iso2Id;
        }

        return country.Iso2Id;
    }

    /// <inheritdoc/>
    public CountryHelperServiceResultModel? GetLogantoCountryTwoLetterIsoAndShortNameDe(string? countryLogaId)
    {
        if (string.IsNullOrEmpty(countryLogaId))
        {
            return new CountryHelperServiceResultModel { Iso2Id = null, ShortNameDe = CountryUnknown, };
        }

        var country = _logantoCountryCollection.Find(c => c.LogaId.Equals(countryLogaId, StringComparison.InvariantCultureIgnoreCase));
        if (country != null)
        {
            return new CountryHelperServiceResultModel { Iso2Id = country.Iso2Id, ShortNameDe = country.ShortNameDe, };
        }

        var bsfCountry = _bfsCountryCollection.Find(c => c.Iso2Id.Equals(countryLogaId, StringComparison.InvariantCultureIgnoreCase));
        if (bsfCountry == null)
        {
            return new CountryHelperServiceResultModel { Iso2Id = null, ShortNameDe = CountryUnknown, };
        }

        return new CountryHelperServiceResultModel { Iso2Id = bsfCountry.Iso2Id, ShortNameDe = bsfCountry.ShortNameDe, };
    }

    private static List<TElement> GetCountryList<TRoot, TElement>(string fileName)
        where TRoot : CountryXmlRootModel<TElement>
    {
        TRoot? countries;

        var serializer = new XmlSerializer(typeof(TRoot));

        var assembly = Assembly.GetExecutingAssembly();

        var resourceName = $"Voting.Stimmregister.Domain.Files.Utils.{fileName}";

        using var stream = assembly.GetManifestResourceStream(resourceName)
           ?? throw new FileNotFoundException(resourceName);

        using var reader = new StreamReader(stream);
        countries = serializer.Deserialize(reader) as TRoot;

        ArgumentNullException.ThrowIfNull(countries?.Country);

        return countries!.Country.ToList();
    }

    private List<BfsCountryHelperServiceModel> GetBfsCountryList()
    {
        return GetCountryList<CountryXmlRootModel<BfsCountryHelperServiceModel>, BfsCountryHelperServiceModel>(BfsCountryListFile);
    }

    private List<LogantoCountryHelperServiceModel> GetLogantoCountryList()
    {
        return GetCountryList<CountryXmlRootModel<LogantoCountryHelperServiceModel>, LogantoCountryHelperServiceModel>(LogantoCountryListFile);
    }

    private Dictionary<int, BfsCountryHelperServiceModel> GetBfsCountryByBfsNumber()
    {
        return _bfsCountryCollection.ToDictionary(bfs => bfs.Id);
    }
}
