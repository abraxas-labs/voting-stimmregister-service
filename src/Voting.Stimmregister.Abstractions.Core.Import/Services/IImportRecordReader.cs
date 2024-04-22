// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;

namespace Voting.Stimmregister.Abstractions.Core.Import.Services;

public interface IImportRecordReader<out TRecord> : IAsyncDisposable
{
    IAsyncEnumerable<TRecord> ReadRecords();
}
