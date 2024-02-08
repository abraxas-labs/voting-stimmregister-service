// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

public class PersonSearchFilterCriteriaModel
{
    /// <summary>
    /// Gets or sets the 'reference id' / 'field name' on which the filter criteria should be applied to, i.e. 'FirstName'.
    /// The reference id must be an exact match to the field name since it is used with reflection.
    /// </summary>
    public FilterReference ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the filter value, i.e. the user input term.
    /// </summary>
    public string FilterValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter data type, i.e. 'string'.
    /// </summary>
    public FilterDataType FilterType { get; set; }

    /// <summary>
    /// Gets or sets the filter operator, i.e. 'contains'.
    /// </summary>
    public FilterOperatorType FilterOperator { get; set; }
}
