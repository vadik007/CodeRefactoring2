using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRefactoring2.Vsix
{
    class HashedSourceFile
    {
        public int FileHash;

        public SortedDictionary<int, SourceEntry> Entries { get; set; } = new SortedDictionary<int, SourceEntry>();
        public void ProcessFile()
        {
            Entries
        }
    }

    class SourceEntry
    {
        public int FileHash { get; set; }
        public int Offset { get; set; }

        public int PreviousHash { get; set; }

        public int NextHash { get; set; }
    }
}
