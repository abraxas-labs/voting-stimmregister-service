// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models;

public class ECollectingPersonSearchByVnParametersModel
{
    public short? CantonBfs { get; set; }

    public int? MunicipalityId { get; set; }

    public long Vn { get; set; }
}
