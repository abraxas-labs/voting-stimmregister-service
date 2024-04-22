// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Core.Import.Loganto.Utils;

public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// Reading the first record/element from an input.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
    /// <param name="asyncEnumerable">An async-enumerable sequence from which to return the first element.</param>
    /// <returns>First element from input.</returns>
    public static async Task<T?> FirstOrDefault<T>(this IAsyncEnumerable<T> asyncEnumerable)
    {
        await using var enumerator = asyncEnumerable.GetAsyncEnumerator();
        return await enumerator.MoveNextAsync() ? enumerator.Current : default;
    }
}
