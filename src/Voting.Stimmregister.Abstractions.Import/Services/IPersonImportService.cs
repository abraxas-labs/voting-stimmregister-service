// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Abstractions.Import.Services;

/// <summary>
/// Person import service from external registers of persons.
/// </summary>
/// <typeparam name="TRecord">The csv record model type.</typeparam>
public interface IPersonImportService<TRecord> : IImportService
{
}
