using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
                //Console.WriteLine($"adding {sourceEntry}");
                if (!Entries.ContainsKey(sourceEntry.ThisHash))
                {
                    Entries.Add(sourceEntry.ThisHash, new List<SourceEntry> { sourceEntry });
                }
                else
                {
                    Entries[sourceEntry.ThisHash].Add(sourceEntry);
                }

                if (!FilesToTokens.ContainsKey(sourceEntry.FileHash))
                {
                    FilesToTokens.Add(sourceEntry.FileHash, new List<int>{sourceEntry.ThisHash});
                }
                else
                {
                    FilesToTokens[sourceEntry.FileHash].Add(sourceEntry.ThisHash);
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

            [XmlIgnore]
            public readonly Dictionary<int, List<int>> FilesToTokens = new Dictionary<int, List<int>>();

        }

        public int FileHash;

        public SortedDictionary<int, List<SourceEntry>> Entries => PersistencePart.Entries;
        public Dictionary<int, string> FilesDictionary => PersistencePart.FilesDictionary;
        public Dictionary<int, List<int>> FilesToTokens => PersistencePart.FilesToTokens;


        SourceFileHasherPersistencePart PersistencePart = new SourceFileHasherPersistencePart();

        private XmlSerializer _serializer = new XmlSerializer(typeof(SourceFileHasherPersistencePart));
        private Regex _scopeRegex = new Regex("\"(.*)?\"", RegexOptions.Compiled);


        public SourceFileHasher()
        {
        }

        public class FileEntry
        {
            public string Path { set; get; }
            public int Line { set; get; }
        }

        public IEnumerable<NewSourceEntry> SearchNew(List<int> input, Dictionary<int, List<NewSourceEntry>> fileToWordsDictionary)
        {
            foreach (var fileTokenMap in fileToWordsDictionary)
            {
                var subArray = input.ToArray();
                var parentArray = fileTokenMap.Value.Select(_=>_.WordHash).ToArray();

                //Console.WriteLine("subArray: " + string.Join(", ", subArray));
                //Console.WriteLine("parentArray: " + string.Join(", ", parentArray));

                //var entry = FindArrayIndex(subArray, parentArray);
                if (subArray.Length > parentArray.Length)
                {
                    continue;
                }
                var entry = IsSubArray(subArray.ToList(), parentArray.ToList());// - 2;
                if (entry > -1)
                {
                    // from entry to factual token position > regex > tokenizer

                    var sourceEntry = fileTokenMap.Value[entry];
                    Debug.Assert(input[0] == sourceEntry.WordHash);
                    yield return sourceEntry;
                    //return ParseFile(FilesDictionary[fileTokenMap.Key]).Skip(entry).Take(1).FirstOrDefault();

                    //return new FileEntry {Path = FilesDictionary[fileTokenMap.Key], };
                    //return Entries[input[0]].Where(_ => _.FileHash == entry /*&& _.NextHash == input[1]*/);
                }
            }

            yield break;
        }

        public IEnumerable<SourceEntry> SearchSequence(List<int> input, Dictionary<int, List<int>> fileToWordsDictionary)
        {
            foreach (var fileTokenMap in fileToWordsDictionary)
            {
                var entry = FindArrayIndex(input.ToArray(), fileTokenMap.Value.ToArray());
                if (entry != -1)
                {
                    return Entries[input[0]].Where(_ => _.FileHash == entry /*&& _.NextHash == input[1]*/);
                }
            }

            return Enumerable.Empty<SourceEntry>();
        }

        public IEnumerable<SourceEntry> SearchSequence__(List<int> input, int tolerance = 1)
        {
            var scoreList = new List<int>();

            // get file with most continious matches
            //     get with most matches. Files 
            var step1 = PersistencePart.FilesToTokens
                //.GroupBy()
                .Select(fileData => new {MatchCount = fileData.Value.Intersect(input).Count(), fileData});
            var step2 = step1
                .OrderByDescending(_ => _.MatchCount);
            var mostEntriesFile = step2
                .FirstOrDefault()?.fileData.Key ?? 0;

            Console.WriteLine($"I'm guessing this is {FilesDictionary[mostEntriesFile]}");

            //find first SourceEntry with this file hash
            
            return Entries
                .FirstOrDefault(_ => _.Key == input[0] && _.Value.Select(f => f.FileHash).Contains(mostEntriesFile))
                .Value ?? Enumerable.Empty<SourceEntry>();

            //foreach (var wordHash in input)
            //{
            //    if (Entries.ContainsKey(wordHash))
            //    {
            //        foreach (SourceEntry sourceEntry in Entries[wordHash].AsReadOnly())
            //        {
            //            Entries.First(_=>_.)
            //        }
            //    }
            //}
        }

        public static int IsSubArray(List<int> shortList, List<int> longList, int fitness = 1)
        {
            if (longList.Count < shortList.Count)
            {
                throw new ArgumentException();
            }

            int life = 0;

            int i = 0;
            int j = 0;
            int backtrack = 0;

            while (i < longList.Count /*&& j < shortList.Count*/)
            {
                if (longList[i] == shortList[j])
                {
                    backtrack++;
                }
                else
                {
                    j = -1;
                    i -= backtrack;
                    backtrack = 0;
                    //i+=2;
                }
                i++; j++;
                if (j == shortList.Count) return i - j;
            }
            return -1;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/3940194/find-an-array-inside-another-larger-array
        /// </summary>
        /// <param name="subArray"></param>
        /// <param name="parentArray"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public static int FindArrayIndex(int[] subArray, int[] parentArray, int err = 1)
        {
            if (subArray.Length == 0)
            {
                return -1;
            }
            int sL = subArray.Length;
            int l = parentArray.Length - subArray.Length +1;// +1 
            int k = 0;
            for (int i = 0; i < l; i++)
            {
                var proceed = parentArray[i] == subArray[k];
                if (!proceed && err > 0) proceed = true; 

                if (proceed)
                {
                    for (int j = 0; j < subArray.Length; j++)
                    {
                        if (parentArray[i + j] == subArray[j])
                        {
                            sL--;
                            if (sL == 0)
                            {
                                return i;
                            }
                        }
                    }
                }
                else
                {
                    
                }
            }
            return -1;
        }

        public static int PackMan_(List<int> x, List<int> y, int fitness = 1)
        {
            List<int> longList;
            List<int> shortList;
            int life = 0;

            int l = 0;
            int s = 0;

            if (x.Count > y.Count)
            {
                longList = x;
                shortList = y;
            }
            else
            {
                longList = y;
                shortList = x;
            }
            for (int g = 0; g < longList.Count; g++)
            {
                while (longList.Count > l)
                {
                    while (shortList.Count > s)
                    {
                        life += shortList[s] == longList[l] ? +1 : -1;
                        if (life < fitness)
                        {
                            s = 0;
                            l = g;
                            life = fitness ;
                        }
                        else
                        {
                            s++;
                            if (life >= shortList.Count -1)
                            {
                                return life;
                            }
                        }

                        l++;
                    }
                }
            }

            return life;
        }

        public IEnumerable<SourceEntry> SearchSequence_(
            List<int> input, int tolerance = 1)
        {
            // match buckets [+][-][+][+][+][-]
            // what file contain the most sequential matches ?
            //int errCount = 0;
            //int foundIdx = 0; //how far we were able to find matches
            //int workIdx = 0; 
            //while (errCount< tolerance && workIdx < input.Count)
            //{
            //    if (Entries.ContainsKey(input[workIdx])) //entry exists
            //    {
            //        Entries[input[workIdx]]
            //    }
            //}

            var buckets = input.SelectMany(key =>Entries.ContainsKey(key) ? Entries[key] : Enumerable.Empty<SourceEntry>());
            var stage2 = buckets.Where(_=>_!=null).ToList();

            var stage3 = stage2.GroupBy(_ => _?.FileHash).OrderByDescending((_)=>_.Count());
            var first = stage3.FirstOrDefault() ?? Enumerable.Empty<SourceEntry>();
            return first;
        }

        private int GetDistance(SourceEntry entry1, SourceEntry entry2)
        {
            return 0;
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

        public class NewSourceEntry
        {
            public int WordHash { get; set; }
            public int Line { get; set; }
            public int Position { get; set; }

            public int FileHash { get; set; }

            public override string ToString()
            {
                return $"{nameof(FileHash)}: {FileHash}, {nameof(WordHash)}: {WordHash}, {nameof(Line)}: {Line}, {nameof(Position)}: {Position}";
            }
        }

        private IEnumerable<NewSourceEntry> ParseFile(string file)
        {
            if (!File.Exists(file))
            {
                Debug.Write("Can not access {0}", file);
                yield break;
            }

            var lines = File.ReadAllLines(file);

            var logTokenizer = new WhiteSpaceLogTokenizer();
            for (var lineN = 0; lineN < lines.Length; lineN++)
            {
                var line = lines[lineN];
                foreach (Match match in _scopeRegex.Matches(line))
                {
                    foreach (var wordHash in logTokenizer.TokenizeLine(RemoveQuotes(match.Value)))
                    {
                        yield return new NewSourceEntry {Line = lineN, Position = match.Index, WordHash = wordHash, FileHash = file.GetHashCode()};
                    }
                }
            }
        }

        public void ProcessDir(string dirPath, Dictionary<int, List<NewSourceEntry>> newDictionary)
        {
            foreach (var file in Directory.GetFiles(dirPath, "*.cs", SearchOption.AllDirectories))
            {
                ProcessFile2(file, newDictionary);
            }

        }
        public void ProcessFile2(string filePath, Dictionary<int, List<NewSourceEntry>> newDictionary)
        {
            int fileHash = filePath.GetHashCode();
            PersistencePart.FilesDictionary.Add(fileHash, filePath);
            //int previousHash = 0;

            newDictionary.Add(fileHash, ParseFile(filePath).ToList());
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
                    foreach (var token in logTokenizer.TokenizeLine(RemoveQuotes(match.Value)))
                    {
                        var sourceEntry = new SourceEntry(
                            fileHash: fileHash,
                            lineNumber: lineN,
                            lineOffset: match.Index,
                            thisHash: token,
                            previousHash: prevSourceEntry?.ThisHash ?? 0, // o-0~o
                            nextHash: 0); // o-0~o

                        if (prevSourceEntry != null) prevSourceEntry.NextHash = sourceEntry.ThisHash; // o-0-o

                        AddSourceEntry(sourceEntry);
                        prevSourceEntry = sourceEntry; // current entry is finished
                        //match.
                    }
                }
            }
        }

        private void AddSourceEntry(SourceEntry sourceEntry)
        {
            //Console.WriteLine($"adding {sourceEntry}");
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