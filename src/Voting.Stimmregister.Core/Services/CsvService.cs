// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Voting.Stimmregister.Core.Abstractions;
using Voting.Stimmregister.Domain.Exceptions;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="ICsvService"/>
public class CsvService : ICsvService
{
    private static readonly CsvConfiguration _csvConfiguration = new(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HasHeaderRecord = true,
        TrimOptions = TrimOptions.Trim,
    };

    public async Task Write<TRow>(PipeWriter writer, IAsyncEnumerable<TRow> records, CancellationToken ct = default)
    {
        if (records == null)
        {
            throw new NoDataException(nameof(records));
        }

        // use utf8 with bom (excel requires bom)
        await using var streamWriter = new StreamWriter(writer.AsStream(), Encoding.UTF8, leaveOpen: true);
        await using var csvWriter = new CsvWriter(streamWriter, _csvConfiguration, leaveOpen: true);
        await csvWriter.WriteRecordsAsync(records, ct);
    }
}
