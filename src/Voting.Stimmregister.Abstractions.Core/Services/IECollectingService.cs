// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Services;

public interface IECollectingService
{
    Task<ECollectingPersonEntityModel> GetPersonWithVotingRightByVn(ECollectingPersonSearchByVnParametersModel searchModel);

    Task<Page<ECollectingPersonEntityModel>> GetPeopleByName(ECollectingPeopleSearchByNameParametersModel searchModel);

    Task<List<ECollectingPersonEntityModel>> GetPeopleByIds(ECollectingPeopleSearchByIdsParametersModel searchModel);
}
