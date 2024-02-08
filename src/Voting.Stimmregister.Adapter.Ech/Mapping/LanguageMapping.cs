// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Ech0045_4_0;
using Voting.Lib.Common;

namespace Voting.Stimmregister.Adapter.Ech.Mapping;

internal static class LanguageMapping
{
    public static LanguageType ToEchLanguage(this string language)
    {
        return language switch
        {
            Languages.Italian => LanguageType.It,
            Languages.French => LanguageType.Fr,
            Languages.Romansh => LanguageType.Rm,
            _ => LanguageType.De,
        };
    }
}
