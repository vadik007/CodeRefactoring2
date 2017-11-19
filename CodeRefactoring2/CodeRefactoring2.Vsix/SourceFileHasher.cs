using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CodeRefactoring2.Vsix
{
    public class SourceFileHasher
    {
        [Serializable]
        public class SourceFileHasherPersistencePart
        {
            [XmlIgnore]
            public SortedDictionary<int, List<SourceEntry>> Entries { get; set; } = new SortedDictionary<int, List<SourceEntry>>();
            [XmlIgnore]
            public Dictionary<int, string> FilesDictionary { get; private set; } = new Dictionary<int, string>();

            public int I { get; set; } = 1;

            [XmlArray]
            public SourceEntry[] EntriesBackField
            {
                get
                {
                    return Entries.SelectMany(_ => _.Value)
                        .ToArray();
                }
                set
                {
                    foreach (var sourceEntry in value)
                    {
                        AddSourceEntry(sourceEntry);
                        //Entries.Add(sourceEntry.ThisHash, sourceEntry);
                    }
                }
            }

            private void AddSourceEntry(SourceEntry sourceEntry)
            {
                Console.WriteLine($"adding {sourceEntry}");
                if (!Entries.ContainsKey(sourceEntry.ThisHash))
                {
                    Entries.Add(sourceEntry.ThisHash, new List<SourceEntry> { sourceEntry });
                }
                else
                {
                    Entries[sourceEntry.ThisHash].Add(sourceEntry);
                }
            }

            [XmlArray]
            public MyTuple<int, string>[] FilesDictionaryBackField
            {
                set
                {
                    foreach (var tuple in value)
                    {
                        FilesDictionary.Add(tuple.Item1, tuple.Item2);
                    }
                }
                get
                {
                    return FilesDictionary.Select(_ => new MyTuple<int, string>(_.Key,_.Value)).ToArray();
                }
            }
        }

        public int FileHash;

        public SortedDictionary<int, List<SourceEntry>> Entries => PersistencePart.Entries;
        public Dictionary<int, string> FilesDictionary => PersistencePart.FilesDictionary;


        SourceFileHasherPersistencePart PersistencePart = new SourceFileHasherPersistencePart();

        private XmlSerializer _serializer = new XmlSerializer(typeof(SourceFileHasherPersistencePart));
        private Regex _scopeRegex = new Regex("\"(.*)?\"", RegexOptions.Compiled);

        public SourceFileHasher()
        {
        }

        public void RestoreFromFile(string file)
        {
            try
            {
                using (var fileStream = File.OpenRead(file))
                {
                    var deserialized = _serializer.Deserialize(fileStream);
                    PersistencePart = (SourceFileHasherPersistencePart)deserialized;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SaveToFile(string file)
        {
            try
            {
                using (var fileStream = File.Create(file, 1024 * 1024, FileOptions.Asynchronous))
                {
                    _serializer.Serialize(fileStream, PersistencePart);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
            PersistencePart.FilesDictionary.Add(fileHash, filePath);

            var logTokenizer = new WhiteSpaceLogTokenizer();

            var lines = File.ReadAllLines(filePath);
            //int previousHash = 0;

            SourceEntry prevSourceEntry = null;

            for (var lineN = 0; lineN < lines.Length; lineN++)
            {
                var line = lines[lineN];
                foreach (Match match in _scopeRegex.Matches(line))
                {
                    var sourceEntry = new SourceEntry(
                        fileHash: fileHash,
                        lineNumber: lineN,
                        lineOffset: match.Index,
                        thisHash: RemoveQuotes(match.Value).GetHashCode(),
                        previousHash: prevSourceEntry?.ThisHash ?? 0, // o-0~o
                        nextHash: 0); // o-0~o

                    if (prevSourceEntry != null) prevSourceEntry.NextHash = sourceEntry.ThisHash; // o-0-o

                    AddSourceEntry(sourceEntry);
                    prevSourceEntry = sourceEntry; // current entry is finished
                    //match.
                }
            }
        }

        private void AddSourceEntry(SourceEntry sourceEntry)
        {
            Console.WriteLine($"adding {sourceEntry}");
            if (!PersistencePart.Entries.ContainsKey(sourceEntry.ThisHash))
            {
                PersistencePart.Entries.Add(sourceEntry.ThisHash, new List<SourceEntry>{sourceEntry});
            }
            else
            {
                PersistencePart.Entries[sourceEntry.ThisHash].Add(sourceEntry);
            }
        }

    }

    [Serializable]
    public class MyTuple<T,TK>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public MyTuple()
        {
            
        }
        public TK Item2 { get; set; }
        public T Item1 { get; set; }

        public MyTuple(
            T t,
            TK tk)
        {
            Item1 = t;
            Item2 = tk;
        }

    }
}