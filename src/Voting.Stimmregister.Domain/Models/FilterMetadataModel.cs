// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

public record FilterMetadataModel(
    int CountOfPersons,
    int CountOfInvalidPersons,
    bool IsActual,
    DateTime? ActualityDate);
