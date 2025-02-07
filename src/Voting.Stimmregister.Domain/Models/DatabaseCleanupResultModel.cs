// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models;

public class DatabaseCleanupResultModel
{
    public int FilterVersionsDeleted { get; set; }

    public int PersonVersionsDeleted { get; set; }
}
