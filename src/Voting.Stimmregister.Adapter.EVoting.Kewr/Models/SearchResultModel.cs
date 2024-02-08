// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.Models;

public class SearchResultModel
{
    public int MaxResult { get; set; }

    public bool MaxResultsExceeded { get; set; }

    public IEnumerable<PersonIdModel> SimpleResidentPojos { get; set; } = new Collection<PersonIdModel>();
}
