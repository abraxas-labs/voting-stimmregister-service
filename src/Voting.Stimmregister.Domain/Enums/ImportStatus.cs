// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Enums;

/// <summary>
/// The Import status enumeration.
/// </summary>
public enum ImportStatus
{
    Unspecified,
    Queued,
    Running,
    Aborted,
    FinishedWithErrors,
    FinishedSuccessfully,
    Stale,
    Failed,
}
