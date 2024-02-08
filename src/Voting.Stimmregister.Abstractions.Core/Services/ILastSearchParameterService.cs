// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// Service for last used search parameters.
/// </summary>
public interface ILastSearchParameterService
{
    /// <summary>
    /// Fetches the last used parameters for the current user with the current tenant for a given search type.
    /// </summary>
    /// <param name="searchType">The search type.</param>
    /// <returns>The found parameters or a default instance if none were found.</returns>
    Task<LastSearchParameterEntity> GetForCurrentUserAndTenant(PersonSearchType searchType);

    /// <summary>
    /// Updates the last used parameters for the current user.
    /// </summary>
    /// <param name="searchType">The search type.</param>
    /// <param name="pageable">The paging info used.</param>
    /// <param name="criteria">The search criteria.</param>
    /// <returns>A <see cref="Task"/> representing the async operation.</returns>
    Task Store(PersonSearchType searchType, Pageable? pageable, List<FilterCriteriaEntity> criteria);
}
