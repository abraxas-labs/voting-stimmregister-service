// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Voting.Lib.VotingExports.Models;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// Model for file downloads.
/// </summary>
public class FileModel
{
    private readonly Func<PipeWriter, CancellationToken, Task> _writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileModel"/> class.
    /// </summary>
    /// <param name="filename">The file name.</param>
    /// <param name="format">The strongly typed export format.</param>
    /// <param name="writer">The writer function.</param>
    public FileModel(
        string filename,
        ExportFileFormat format,
        Func<PipeWriter, CancellationToken, Task> writer)
    {
        _writer = writer;
        Filename = filename;
        Format = format;
    }

    /// <summary>
    /// Gets or sets the file donwload name.
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// Gets the file format.
    /// </summary>
    public ExportFileFormat Format { get; }

    /// <summary>
    /// Gets the mime type according to the defined <see cref="Format"/>, i.e. 'text/csv'.
    /// </summary>
    public string MimeType => Format.GetMimeType();

    /// <summary>
    /// Gets the stream.
    /// </summary>
    /// <param name="writer">The pipeline writer to write the file sream to.</param>
    /// <param name="ct">The cancellation token to properly handle async operations.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous write operation.</returns>
    public Task Write(PipeWriter writer, CancellationToken ct = default) => _writer(writer, ct);
}
