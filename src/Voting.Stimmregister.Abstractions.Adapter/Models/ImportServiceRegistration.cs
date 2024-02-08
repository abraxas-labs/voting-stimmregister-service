// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Abstractions.Adapter.Models;

/// <summary>
/// A service registration of an <see cref="IImportService"/> implementation.
/// </summary>
/// <param name="SourceSystem">The source system of the import.</param>
/// <param name="Type">The import type.</param>
/// <param name="ImporterServiceType">The implementation type, needs to implement <see cref="IImportService"/>.</param>
public record ImportServiceRegistration(
    ImportSourceSystem SourceSystem,
    ImportType Type,
    Type ImporterServiceType);
