// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

public record FilterActualityModel(
    bool IsActual,
    DateTime? ActualityDate);
