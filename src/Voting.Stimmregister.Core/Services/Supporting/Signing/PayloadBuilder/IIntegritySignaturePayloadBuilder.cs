// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

public interface IIntegritySignaturePayloadBuilder : IIncrementalSignaturePayloadBuilder<BfsIntegrityEntity, PersonEntity>, IIncrementalSignaturePayloadBuilder<BfsIntegrityEntity, DomainOfInfluenceEntity>
{
    new void Init(BfsIntegrityEntity item);

    new SignaturePayload BuildAndReset();
}
