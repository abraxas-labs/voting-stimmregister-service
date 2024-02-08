// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace Voting.Stimmregister.Abstractions.Import.Services;

public static class CsvImportHelper
{
    public static IImportRecordReader<TRecord> CreateRecordReader<TRecord>(Stream content, Encoding defaultEncoding, string delimiter)
    {
        var fileReader = new StreamReader(content, defaultEncoding, detectEncodingFromByteOrderMarks: true);
        fileReader.Peek(); // peeking to read preamble so encoding can be evaluated

        var csvReader = new CsvReader(
            fileReader,
            BuildCsvConfig(fileReader.CurrentEncoding, defaultEncoding, delimiter));

        // handle empty strings as null
        csvReader.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add(string.Empty);

        return new CsvReaderAdapter<TRecord>(csvReader);
    }

    /// <summary>
    /// Gets the csv configuration with a corresponding delimiter and encoding configuration.
    /// </summary>
    /// <param name="currentEncoding">The encoding to use if available.</param>
    /// <param name="defaultEncoding">The default encoding to use as a fallback to <paramref name="currentEncoding"/>.</param>
    /// <param name="delimiter">The csv delimiter to use.</param>
    /// <returns>A <see cref="CsvConfiguration"/>.</returns>
    private static CsvConfiguration BuildCsvConfig(Encoding? currentEncoding, Encoding defaultEncoding, string delimiter)
    {
        return new(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter,
            Encoding = currentEncoding ?? defaultEncoding,
            HasHeaderRecord = true,
        };
    }

    private sealed class CsvReaderAdapter<T> : IImportRecordReader<T>
    {
        private readonly CsvReader _reader;

        public CsvReaderAdapter(CsvReader reader)
        {
            _reader = reader;
        }

        public IAsyncEnumerable<T> ReadRecords()
            => _reader.GetRecordsAsync<T>();

        public ValueTask DisposeAsync()
        {
            _reader.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
