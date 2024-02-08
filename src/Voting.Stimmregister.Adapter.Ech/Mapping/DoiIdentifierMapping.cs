// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Ech.Mapping;

internal static class DoiIdentifierMapping
{
    private const string DefaultIdentifierUnknown = "unknown";

    public static string ToEchIdentifier(PersonDoiEntity doiEntity)
        => !string.IsNullOrWhiteSpace(doiEntity.Identifier)
                        ? doiEntity.Identifier
                        : DefaultIdentifierUnknown;
}
