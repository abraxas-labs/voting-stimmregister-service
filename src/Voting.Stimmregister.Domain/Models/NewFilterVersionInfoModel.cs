// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// Information of newly created filter version.
/// </summary>
/// <param name="FilterVersionId">The id of the new created entity.</param>
/// <param name="Count">The number of persons.</param>
public record NewFilterVersionInfoModel(Guid FilterVersionId, int Count);
