// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Models;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Core.Services;

/// <summary>
/// Registry of <see cref="IImportService"/> registrations.
/// </summary>
public class ImportServiceRegistry
{
    private readonly IReadOnlyDictionary<(ImportSourceSystem, ImportType), Type> _importServiceTypes;

    public ImportServiceRegistry(IEnumerable<ImportServiceRegistration> importServices)
    {
        _importServiceTypes = importServices.ToDictionary(x => (x.SourceSystem, x.Type), x => x.ImporterServiceType);
    }

    internal IImportService? GetImportService(IServiceProvider sp, ImportSourceSystem sourceSystem, ImportType type)
    {
        if (!_importServiceTypes.TryGetValue((sourceSystem, type), out var serviceType))
        {
            return null;
        }

        return (IImportService)sp.GetRequiredService(serviceType);
    }
}
