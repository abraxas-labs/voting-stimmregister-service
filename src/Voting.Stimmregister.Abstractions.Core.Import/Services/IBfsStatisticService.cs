// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Core.Import.Models;

namespace Voting.Stimmregister.Abstractions.Core.Import.Services;

public interface IBfsStatisticService
{
    Task CreateOrUpdateStatistics(PersonImportStateModel state);
}
