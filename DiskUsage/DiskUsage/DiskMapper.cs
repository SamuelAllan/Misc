using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiskUsage
{
    public static class DiskMapper
    {
        public static Folder Map(string rootPath)
        {
            if (Directory.Exists(rootPath))
            {
                return MapDirectory(rootPath);
            }
            throw new ArgumentException("Invalid path!", "rootPath");
        }

        private static Folder MapDirectory(this string path)
        {
            var name = Path.GetFileName(path);
            var fileSize = Directory.GetFiles(path)
                .Select(p => new FileInfo(p))
                .Sum(f => f.Length);
            var children = Directory.GetDirectories(path).MapDirectories().ToList();
            return new Folder(name, fileSize, children);
        }

        private static IEnumerable<Folder> MapDirectories(this IEnumerable<string> paths)
        {
            return paths.Select(x =>
                {
                    try
                    {
                        return x.MapDirectory();
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(x => x != null);
        }
    }
}
