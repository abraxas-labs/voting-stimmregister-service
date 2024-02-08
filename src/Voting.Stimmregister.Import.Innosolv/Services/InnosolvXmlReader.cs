// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ABX_Voting_1_0;
using Voting.Lib.Ech.AbxVoting_1_0.Converter;
using Voting.Stimmregister.Abstractions.Import.Services;

namespace Voting.Stimmregister.Import.Innosolv.Services;

internal class InnosolvXmlReader : IImportRecordReader<PersonInfoType>
{
    private readonly AbxVotingDeserializer _deserializer;
    private readonly Stream _stream;

    public InnosolvXmlReader(Stream stream, AbxVotingDeserializer deserializer)
    {
        _deserializer = deserializer;
        _stream = stream;
    }

    public IAsyncEnumerable<PersonInfoType> ReadRecords()
        => _deserializer.ReadVoters(_stream, default);

    public ValueTask DisposeAsync()
    {
        _stream.Dispose();
        return ValueTask.CompletedTask;
    }
}
