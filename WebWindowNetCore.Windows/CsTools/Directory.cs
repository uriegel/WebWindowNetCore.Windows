using System.IO;
using LinqTools;

namespace CsTools.Extensions;

public static class DirectoryExtensions
{
    public static string EnsureDirectoryExists(this string path)
        => path.SideEffect(p => 
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            });

    public static string AppendPath(this string path, string subPath)
        => Path.Combine(path, subPath);

    public static Stream CreateFile(this string path)
        => File.Create(path);
} 