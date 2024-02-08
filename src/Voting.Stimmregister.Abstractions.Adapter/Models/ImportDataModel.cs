// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Abstractions.Adapter.Models;

/// <summary>
/// Data of an import to be run.
/// </summary>
/// <param name="Id">The id of the <see cref="ImportStatisticEntity"/>.</param>
/// <param name="ReceivedTimestamp">The timestamp when the file was received (this is actually the creation date of the <see cref="ImportStatisticEntity"/>.</param>
/// <param name="Name">The name of the imported file.</param>
/// <param name="Content">The contents of the file to be imported.</param>
/// <param name="SourceSystem">The import source system.</param>
public record ImportDataModel(Guid Id, DateTime ReceivedTimestamp, string Name, Stream Content, ImportSourceSystem SourceSystem);
