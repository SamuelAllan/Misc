using System.Collections.Generic;
using System.Linq;

namespace DiskUsage
{
    public class Folder
    {
        public Folder(string name, long fileSizeInBytes, IReadOnlyList<Folder> children)
        {
            Name = name;
            Children = children;
            FileSizeInBytes = fileSizeInBytes;
            ChildSizeInBytes = children.Sum(x => x.TotalSizeInBytes);
            TotalSizeInBytes = fileSizeInBytes + ChildSizeInBytes;
        }

        public string Name { get; private set; }
        public IReadOnlyList<Folder> Children { get; private set; }
        public long FileSizeInBytes { get; private set; }
        public long ChildSizeInBytes { get; private set; }
        public long TotalSizeInBytes { get; private set; }
    }
}
