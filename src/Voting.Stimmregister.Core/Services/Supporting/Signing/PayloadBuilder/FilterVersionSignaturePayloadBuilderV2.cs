// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.Extensions.ObjectPool;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

internal class FilterVersionSignaturePayloadBuilderV2 : IncrementalPersonPayloadBuilderV2<FilterVersionEntity>
{
    public FilterVersionSignaturePayloadBuilderV2(
        ObjectPool<HashBuilder> hashBuilderPool,
        IHsmCryptoAdapter hsmCryptoAdapter,
        PersonSignaturePayloadBuilderV2 personPayloadBuilder)
        : base(nameof(FilterVersionEntity), hashBuilderPool, hsmCryptoAdapter, personPayloadBuilder)
    {
    }

    public override void Init(FilterVersionEntity item)
    {
        HashBuilder
            .AppendDelimited(item.FilterId)
            .AppendDelimited(item.Name)
            .AppendDelimited(item.Count)
            .AppendDelimited(item.Deadline)
            .AppendDelimited(item.AuditInfo.CreatedByName)
            .AppendDelimited(item.AuditInfo.CreatedAt);
    }

    protected override bool IsOrderValid(PersonEntity previousEntity, PersonEntity currentEntity)
    {
        // sort by municipality id, then by id
        return currentEntity.MunicipalityId > previousEntity.MunicipalityId
            || base.IsOrderValid(previousEntity, currentEntity);
    }
}
