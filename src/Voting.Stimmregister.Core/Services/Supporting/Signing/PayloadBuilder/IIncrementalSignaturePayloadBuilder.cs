// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

public interface IIncrementalSignaturePayloadBuilder<TInit, TElement> : ISignaturePayloadBuilder<(TInit InitializationEntity, IEnumerable<TElement> ContentEntities)>
{
    void Init(TInit item);

    void Append(TElement item);

    SignaturePayload BuildAndReset();
}
