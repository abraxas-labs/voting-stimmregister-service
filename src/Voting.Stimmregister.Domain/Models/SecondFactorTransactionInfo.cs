// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

public record SecondFactorTransactionInfo(
    Guid Id,
    IReadOnlyList<SecondFactorTransactionProvider> AvailableProviders,
    SecondFactorTransactionNevisInfo? Nevis);
