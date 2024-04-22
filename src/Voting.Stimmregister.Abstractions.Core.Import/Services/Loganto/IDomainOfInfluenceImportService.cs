// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Abstractions.Core.Import.Services.Loganto;

/// <summary>
/// Domain of influence import service from external sources.
/// </summary>
/// <typeparam name="TRecord">The csv record model type.</typeparam>
public interface IDomainOfInfluenceImportService<TRecord> : IImportService
{
}
