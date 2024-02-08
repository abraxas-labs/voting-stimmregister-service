// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Core.Services;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc/>
public class LanguageService : ILanguageService
{
    private const string FallbackLanguage = Languages.German;
    private readonly ILogger<LanguageService> _logger;
    private readonly IDataContext _dataContext;

    public LanguageService(ILogger<LanguageService> logger, IDataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    /// <inheritdoc/>
    public string Language => _dataContext.Language ?? FallbackLanguage;

    /// <inheritdoc/>
    public void SetLanguage(string? lang)
    {
        if (!Languages.All.Contains(lang))
        {
            _logger.LogWarning("Language {lang} is not valid, using fallback language", lang);
            lang = FallbackLanguage;
        }
        else
        {
            _logger.LogDebug("Using language {lang} for this request", lang);
        }

        _dataContext.Language = lang;
    }
}
