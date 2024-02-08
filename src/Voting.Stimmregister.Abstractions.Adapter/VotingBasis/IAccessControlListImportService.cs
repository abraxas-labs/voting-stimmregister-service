// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;

namespace Voting.Stimmregister.Abstractions.Adapter.VotingBasis;

/// <summary>
/// Service for importing domain of influence (DOI) based access control list (ACL).
/// </summary>
public interface IAccessControlListImportService
{
    /// <summary>
    /// Import domain of influence based access control lists.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation on success.</returns>
    Task ImportAcl();
}
