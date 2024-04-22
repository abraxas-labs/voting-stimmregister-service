// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Database.Models;

namespace Voting.Stimmregister.Domain.Models.Utils;

public class PersonSearchResultPageModel<T> : Page<T>
    where T : PersonEntity
{
    public PersonSearchResultPageModel(Page<T> page, int invalidPersonsCount)
        : base(page.Items, page.TotalItemsCount, page.CurrentPage, page.PageSize)
    {
        InvalidPersonsCount = invalidPersonsCount;
    }

    /// <summary>
    /// Gets the count of invalid persons.
    /// </summary>
    public int InvalidPersonsCount { get; }
}
