// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// The person search parameters model.
/// </summary>
public class PersonSearchParametersModel
{
    /// <summary>
    /// Gets or sets the pagination model.
    /// </summary>
    public Pageable? Paging { get; set; }

    /// <summary>
    /// Gets or sets the search filter criteria model.
    /// </summary>
    public List<PersonSearchFilterCriteriaModel> Criteria { get; set; } = new();

    /// <summary>
    /// Gets or sets the person search type intention.
    /// </summary>
    public PersonSearchType SearchType { get; set; }
}
