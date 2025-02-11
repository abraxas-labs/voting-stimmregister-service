// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

public interface ISignaturePayloadBuilder<in T>
{
    byte Version { get; }

    SignaturePayload Build(T entity);
}
