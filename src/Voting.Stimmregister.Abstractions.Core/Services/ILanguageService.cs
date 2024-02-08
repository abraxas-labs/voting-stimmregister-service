// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// Language service for getting and setting the language for the data context.
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Gets the current language or the fallback language if none is set.
    /// </summary>
    string Language { get; }

    /// <summary>
    /// Sets the language used for subsequent operations.
    /// </summary>
    /// <param name="lang">The language to use.</param>
    void SetLanguage(string? lang);
}
