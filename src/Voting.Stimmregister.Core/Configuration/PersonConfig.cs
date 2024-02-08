// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Core.Configuration;

public class PersonConfig
{
    /// <summary>
    /// Gets or sets the time span how long an update of a person is valid until the person is considered out of date.
    /// </summary>
    public TimeSpan ActualityTimeSpan { get; set; } = TimeSpan.FromDays(1);
}
