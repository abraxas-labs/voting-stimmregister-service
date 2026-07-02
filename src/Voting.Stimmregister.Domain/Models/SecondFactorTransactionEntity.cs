// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Lib.Database.Models;

namespace Voting.Stimmregister.Domain.Models;

public class SecondFactorTransactionEntity : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public int PollCount { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpireAt { get; set; }

    public string ActionIdHash { get; set; } = string.Empty;

    public List<string>? NevisExternalTokenJwtIds { get; set; }

    public bool Verified { get; set; }

    public bool Used { get; set; }
}
