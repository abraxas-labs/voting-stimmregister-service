// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;

namespace Voting.Stimmregister.Domain.Models.Utils;

/// <summary>
/// A container with a count and async streamed data.
/// </summary>
/// <typeparam name="T">The type of the data entries.</typeparam>
public class PersonSearchStreamedResultModel<T>
{
    private IAsyncEnumerable<T> _data;

    public PersonSearchStreamedResultModel(int count, int invalidCount, IAsyncEnumerable<T> data)
    {
        Count = count;
        _data = data;
        InvalidCount = invalidCount;
        ValidCount = count - invalidCount;
    }

    /// <summary>
    /// Gets the count of persons in <see cref="Data"/>.
    /// This is the sum of <see cref="ValidCount"/> and <see cref="InvalidCount"/>.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the count of valid persons in <see cref="Data"/>.
    /// </summary>
    public int ValidCount { get; }

    /// <summary>
    /// Gets the count of invalid persons in <see cref="Data"/>.
    /// </summary>
    public int InvalidCount { get; }

    public IAsyncEnumerable<T> Data => _data;

    public void Peek(Action<T> onData)
    {
        _data = _data.Select(x =>
        {
            onData(x);
            return x;
        });
    }
}
