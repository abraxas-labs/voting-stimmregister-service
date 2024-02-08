// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Runtime.CompilerServices;
using Voting.Lib.Testing.Utils;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public static class RawSnapshots
{
    public static void MatchFormattedXmlSnapshot(
        this string content,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
#if UPDATE_SNAPSHOTS
        const bool updateSnapshot = true;
#else
        const bool updateSnapshot = false;
#endif
        var path = SnapperExtensions.GetSnapshotFilePath(string.Empty, memberName, filePath, "xml");
        content = XmlUtil.FormatTestXml(content);
        content.MatchRawSnapshot(path, updateSnapshot);
    }
}
