// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// An entity with an <see cref="AuditInfo"/>.
/// </summary>
public interface IAuditedEntity
{
    AuditInfo AuditInfo { get; set; }
}
