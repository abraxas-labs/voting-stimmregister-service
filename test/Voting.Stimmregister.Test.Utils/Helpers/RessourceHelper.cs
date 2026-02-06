// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public static class RessourceHelper
{
    public static string GetFullPathToFile(string relativePathToFile, [CallerFilePath] string callerFilePath = "")
    {
        var dir = Path.GetDirectoryName(callerFilePath)
                  ?? throw new InvalidOperationException();

        while (dir != null && Directory.GetFiles(dir, "*.csproj", SearchOption.TopDirectoryOnly).Length == 0)
        {
            dir = Path.GetDirectoryName(dir);
        }

        return Path.Combine(dir ?? string.Empty, relativePathToFile);
    }
}
