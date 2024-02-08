// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Reflection;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public static class RessourceHelper
{
    public static string GetFullPathToFile(string relativePathToFile)
    {
        var pathAssembly = Assembly.GetExecutingAssembly().Location;

        var folderAssembly = Path.GetDirectoryName(pathAssembly);

        return Path.Combine(folderAssembly!, relativePathToFile);
    }
}
