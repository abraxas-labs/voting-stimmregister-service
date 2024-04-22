// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// Access control list entity based on political domain of influence.
/// </summary>
public class AccessControlListDoiEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the name, i.e. 'St.Gallen'.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the BFS number, i.e. '3203'.
    /// </summary>
    public string? Bfs { get; set; }

    /// <summary>
    /// Gets or sets the tenant name as defined by the IAM, i.e. 'St.Gallen'.
    /// </summary>
    public string TenantName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant identifier as defined by the IAM, i.e. '38012345678900'.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type, i.e. 'MU'.
    /// </summary>
    public DomainOfInfluenceType Type { get; set; }

    /// <summary>
    /// Gets or sets the canton, i.e. 'SG'.
    /// </summary>
    public Canton Canton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity data has passed validation or not.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets validation error reasons if any occurs.
    /// </summary>
    public string? ValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets the parent identifier of the current DOI within the hierarchical tree.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent DOI within the hierarchical tree. Is null if the current DOI is a root node.
    /// </summary>
    public AccessControlListDoiEntity? Parent { get; set; }

    /// <summary>
    /// Gets or sets the import id reference.
    /// </summary>
    public Guid? ImportStatisticId { get; set; }

    /// <summary>
    /// Gets or sets the import entity reference.
    /// </summary>
    public ImportStatisticEntity? ImportStatistic { get; set; }

    /// <summary>
    /// Gets or sets the children DOI within the hierarchical tree.
    /// Empty if the current DOI is a leaf node, otherwise it may have one to many children.
    /// </summary>
    public ICollection<AccessControlListDoiEntity> Children { get; set; } = new HashSet<AccessControlListDoiEntity>();

    /// <summary>
    /// Gets or sets the return address of the current DOI.
    /// Empty if the current DOI is not a leaf node or not responsible for voting cards.
    /// </summary>
    public AccessControlListDoiReturnAddress? ReturnAddress { get; set; }
}
