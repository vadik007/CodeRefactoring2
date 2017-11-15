using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeRefactoring2.Vsix
{
    public class SourceFileHasher
    {
        public int FileHash;

        public SortedDictionary<int, List<SourceEntry>> Entries { get; set; } 
            = new SortedDictionary<int, List<SourceEntry>>();

        public Dictionary<int, string> FilesDictionary { get; private set; } = new Dictionary<int, string>();

        private Regex _scopeRegex = new Regex("\"(.*)+?\"", RegexOptions.Compiled);

        public SourceFileHasher()
        {
        }

        public static string RemoveQuotes(string quotedString)
        {
            return quotedString.Substring(1, quotedString.Length - 2);
        }

        public void ProcessFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.Write("Can not access {0}", filePath);
            }

            int fileHash = filePath.GetHashCode();
            FilesDictionary.Add(fileHash, filePath);

            var logTokenizer = new WhiteSpaceLogTokenizer();

            var lines = File.ReadAllLines(filePath);
            //int previousHash = 0;

            SourceEntry prevSourceEntry = null;

            for (var lineN = 0; lineN < lines.Length; lineN++)
            {
                var line = lines[lineN];
                foreach (Match match in _scopeRegex.Matches(line))
                {
//                    var sourceEntry = new SourceEntry(
//                        fileHash: fileHash,
//                        lineNumber: lineN, 
//                        lineOffset: match.Index,
//                        thisHash: RemoveQuotes(match.Value).GetHashCode(),
//                        previousHash: prevSourceEntry?.ThisHash ?? 0, // o-0~o
//                        nextHash: 0); // o-0~o

                    //if (prevSourceEntry != null) prevSourceEntry.NextHash = sourceEntry.ThisHash; // o-0-o

                    //AddSourceEntry(sourceEntry);
                    //prevSourceEntry = sourceEntry; // current entry is finished
                    //match.
                }
            }
        }

        private void AddSourceEntry(SourceEntry sourceEntry)
        {
            Console.WriteLine($"adding {sourceEntry}");
            if (!Entries.ContainsKey(sourceEntry.ThisHash))
            {
                Entries.Add(sourceEntry.ThisHash, new List<SourceEntry>{sourceEntry});
            }
            else
            {
                Entries[sourceEntry.ThisHash].Add(sourceEntry);
            }
        }

    }
    public class SourceEntry
    {
        /// <inheritdoc />
        public SourceEntry(int fileHash, int lineNumber, int lineOffset, int thisHash, int previousHash, int nextHash)
        {
            FileHash = fileHash;
            LineNumber = lineNumber;
            LineOffset = lineOffset;
            PreviousHash = previousHash;
            NextHash = nextHash;
            ThisHash = thisHash;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(FileHash)}: {FileHash}, {nameof(LineNumber)}: {LineNumber}, {nameof(LineOffset)}: {LineOffset}, {nameof(ThisHash)}: {ThisHash}, {nameof(PreviousHash)}: {PreviousHash}, {nameof(NextHash)}: {NextHash}";
        }

        public int FileHash { get; set; }
        public int LineNumber { get; set; }
        public int LineOffset { get; set; }
        public int ThisHash { get; set; }
        public int PreviousHash { get; set; }
        public int NextHash { get; set; }
    }
}
