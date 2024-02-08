// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Database.Models;

namespace Voting.Stimmregister.Domain.Models.Utils;

public class PersonSearchResultPage<T> : Page<T>
    where T : PersonEntity
{
    public PersonSearchResultPage(Page<T> page, int invalidPersonsCount)
        : base(page.Items, page.TotalItemsCount, page.CurrentPage, page.PageSize)
    {
        InvalidPersonsCount = invalidPersonsCount;
    }

    /// <summary>
    /// Gets the count of invalid persons.
    /// </summary>
    public int InvalidPersonsCount { get; }
}
