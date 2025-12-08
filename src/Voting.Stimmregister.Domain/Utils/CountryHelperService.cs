// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using AutoMapper;
using Ech0072_1_0;
using Voting.Lib.Ech;
using Voting.Lib.Ech.Ech0072_1_0.Schemas;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Domain.Utils;

/// <inheritdoc cref="ICountryHelperService"/>
public class CountryHelperService : ICountryHelperService
{
    private const string BfsCountryListFile = "BFSCountryList.xml";
    private const string LogantoCountryListFile = "LogantoCountryList.xml";
    private const string CountryUnknown = "Staat unbekannt";

    private readonly IMapper _mapper;
    private readonly ReadOnlyCollection<CountryModel> _bfsCountryCollection;
    private readonly ReadOnlyCollection<CountryModel> _logantoCountryCollection;
    private readonly ReadOnlyDictionary<ushort, CountryModel> _bfsCountryByBfsNumber;

    public CountryHelperService(IMapper mapper)
    {
        _mapper = mapper;
        _bfsCountryCollection = GetBfsCountryList().AsReadOnly();
        _logantoCountryCollection = GetLogantoCountryList().AsReadOnly();
        _bfsCountryByBfsNumber = GetBfsCountryByBfsNumber().AsReadOnly();
    }

    public ReadOnlyCollection<CountryModel> BfsCountryCollection => _bfsCountryCollection;

    /// <inheritdoc/>
    public string? GetCountryTwoLetterIsoCode(string? countryShortName, string? bfsCountryNumber = null, string? iso2 = null)
    {
        // VOTING-3385: Re-map states not recognized by Switzerland and missing Iso2 code with invalid source system assignment.
        bfsCountryNumber = bfsCountryNumber == "8551" ? "8532" : bfsCountryNumber;

        if (string.IsNullOrEmpty(bfsCountryNumber) || !ushort.TryParse(bfsCountryNumber, out var bfsNumber))
        {
            return _bfsCountryCollection.FirstOrDefault(c =>
                (c.Iso2?.Equals(iso2, StringComparison.InvariantCultureIgnoreCase) == true && iso2 != string.Empty) ||
                c.ShortNameEn?.Equals(countryShortName, StringComparison.InvariantCultureIgnoreCase) == true)?.Iso2;
        }

        if (!_bfsCountryByBfsNumber.TryGetValue(bfsNumber, out var country))
        {
            if (string.IsNullOrEmpty(countryShortName))
            {
                return null;
            }

            return _bfsCountryCollection.FirstOrDefault(c => c.ShortNameEn?.Equals(countryShortName, StringComparison.InvariantCultureIgnoreCase) == true)?.Iso2;
        }

        return country.Iso2;
    }

    /// <inheritdoc/>
    public CountryModel? GetCountryInfo(string countryTwoLetterCode)
    {
        if (string.IsNullOrEmpty(countryTwoLetterCode))
        {
            return null;
        }

        return _bfsCountryCollection.FirstOrDefault(c => c.Iso2?.Equals(countryTwoLetterCode, StringComparison.InvariantCultureIgnoreCase) == true);
    }

    /// <inheritdoc/>
    public string? GetLogantoCountryTwoLetterIsoCode(string? countryLogaId)
    {
        if (string.IsNullOrEmpty(countryLogaId))
        {
            return null;
        }

        var country = _logantoCountryCollection.FirstOrDefault(c => c.SystemId?.Equals(countryLogaId, StringComparison.InvariantCultureIgnoreCase) == true);

        if (country == null)
        {
            return _bfsCountryCollection.FirstOrDefault(c => c.Iso2?.Equals(countryLogaId, StringComparison.InvariantCultureIgnoreCase) == true)?.Iso2;
        }

        return country.Iso2;
    }

    /// <inheritdoc/>
    public CountryModel? GetLogantoCountryTwoLetterIsoAndShortNameDe(string? countryLogaId)
    {
        if (string.IsNullOrEmpty(countryLogaId))
        {
            return new CountryModel { Iso2 = null, ShortNameDe = CountryUnknown, };
        }

        var country = _logantoCountryCollection.FirstOrDefault(c => c.SystemId?.Equals(countryLogaId, StringComparison.InvariantCultureIgnoreCase) == true);
        if (country != null)
        {
            return new CountryModel { Iso2 = country.Iso2, ShortNameDe = country.ShortNameDe, };
        }

        var bsfCountry = _bfsCountryCollection.FirstOrDefault(c => c.Iso2?.Equals(countryLogaId, StringComparison.InvariantCultureIgnoreCase) == true);
        if (bsfCountry == null)
        {
            return new CountryModel { Iso2 = null, ShortNameDe = CountryUnknown, };
        }

        return new CountryModel { Iso2 = bsfCountry.Iso2, ShortNameDe = bsfCountry.ShortNameDe, };
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

    private List<CountryModel> GetBfsCountryList()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Voting.Stimmregister.Domain.Files.Utils.{BfsCountryListFile}";
        using var stream = assembly.GetManifestResourceStream(resourceName)
           ?? throw new FileNotFoundException(resourceName);

        var schemaSet = Ech0072Schemas.LoadEch0072Schemas();
        var countries = new EchDeserializer().DeserializeXml<Countries>(stream, schemaSet).Country;

        return _mapper.Map<List<CountryModel>>(countries);
    }

    private List<CountryModel> GetLogantoCountryList()
    {
        var countries = GetCountryList<CountryXmlRootModel<LogantoCountryModel>, LogantoCountryModel>(LogantoCountryListFile);
        return _mapper.Map<List<CountryModel>>(countries);
    }

    private Dictionary<ushort, CountryModel> GetBfsCountryByBfsNumber()
    {
        return _bfsCountryCollection.ToDictionary(bfs => bfs.BfsId);
    }
}
