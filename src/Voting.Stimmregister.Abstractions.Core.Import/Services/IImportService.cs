// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Adapter.Models;

namespace Voting.Stimmregister.Abstractions.Core.Import.Services;

/// <summary>
/// An import service imports file based data into the system.
/// </summary>
public interface IImportService
{
    /// <summary>
    /// Imports the data into the system.
    /// </summary>
    /// <param name="data">The description and contents of the import.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RunImport(ImportDataModel data);

    /// <summary>
    /// Gets the municipality id from the first csv record of the file stream.
    /// </summary>
    /// <param name="content">The stream to get the municipality id from.</param>
    /// <returns>The municipality id.</returns>
    Task<int?> PeekMunicipalityIdFromStream(Stream content);
}
