using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CodeRefactoring2.Vsix;
using NUnit.Framework;
using System.Xml.Serialization;

namespace CodeRefactoring2.Tests
{
    [TestFixture]
    public class UnitTest1
    {
            const string tempSavePath = "c:\\temp\\save.xml";
        [Test]
        public void TestMethod1()
        {
            var testText = "\r\n2017-08-13 21:42:35.6616 | 4872 | 1 | Info | InformationAgent.AgentHostProcess.AgentHostProcess\r\nService configured on port 8085 of channel http. Server object created \r\n\r\n2017-08-13 21:42:37.4712 | 4872 | 1 | Error | InformationAgent.AgentHostProcess.AgentHostProcess\r\nException occured during the AgentHostProcess run System.InvalidOperationException: Клиент обнаружил тип содержимого ответа \"text/html; charset=utf-8\", но ожидается тип \"text/xml\".\r\nСбой запроса с сообщением об ошибке:".Split('\r','\n').Where(_=>!string.IsNullOrWhiteSpace(_));

            foreach (var token in testText)
            {
                var tokenizeLine = new WhiteSpaceLogTokenizer().TokenizeLine(token);
                Dump(tokenizeLine);
            }
        }

        private void Dump(IEnumerable<int> ints)
        {
            foreach (var i in ints)
            {
                Console.Write(i);
                Console.Write(',');

            }
            Console.WriteLine();

         }

        [Test]
        public void FileTokenizationTest()
        {
            var sourceFileHasher = new SourceFileHasher();
            var stopwatch = Stopwatch.StartNew();
            sourceFileHasher.ProcessFile(@"D:\CONGEN\cnt\agent-library\AgentSkills\AgentSkills\InformationAgent.FetchnExtract\fetchnextract.cs");
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            Assert.Pass();
        }

        [Test]
        public void AlTokenizationTest()
        {
            var sourceFileHasher = new SourceFileHasher();
            var stopwatch = Stopwatch.StartNew();
            foreach (var file in Directory.GetFiles(@"D:\CONGEN\cnt\agent-library\","*.cs",SearchOption.AllDirectories))
            {
                sourceFileHasher.ProcessFile(file);
            }

            sourceFileHasher.SaveToFile(tempSavePath);

            stopwatch.Stop();

            var restoreFileHasher = new SourceFileHasher();
            restoreFileHasher.RestoreFromFile(tempSavePath);

            Assert.AreEqual(restoreFileHasher.Entries,sourceFileHasher.Entries);
            Assert.AreEqual(restoreFileHasher.FilesDictionary, sourceFileHasher.FilesDictionary);

            Console.WriteLine(stopwatch.Elapsed);
            Assert.Pass();
        }

        [Test]
        public void RemoveQuotesTest()
        {
            Console.WriteLine(SourceFileHasher.RemoveQuotes("\"\""));
            
        }

        [Test]
        public void EntitiesTest()
        {
            var xmlSerializer = new XmlSerializer(typeof(SourceEntry));

            var memoryStream = new MemoryStream();
            xmlSerializer.Serialize(memoryStream, new SourceEntry(0,0,0,0,0,0));

            memoryStream.Seek(
                0,
                SeekOrigin.Begin);

            Console.WriteLine(memoryStream);
        }

        [Test]
        public void Test()
        {
            Regex _scopeRegex = new Regex("\"(.*)+?\"", RegexOptions.Compiled);

            foreach (Match match in _scopeRegex.Matches("            int.TryParse(ConfigurationManager.AppSettings[\"ParameterSetStorePeriod\"], out _parameterSetStorePeriod);"))
            {
                Console.WriteLine(match);
            }
            /*
             Catastrophic backtracking has been detected and the execution of your expression has been halted. To find out more what this is, please read the following article: Runaway Regular Expressions

I recommend you launch the debugger in the menu to the left and analyze the data to find out the cause.

            http://www.regular-expressions.info/catastrophic.html
             */
        }

        [Test]
        public void CorrelateLogsTest()
        {

           var logLines = new string[]{ "AgentHostProcess is started",
                "Service configured on port 8085 of channel http.Server object created",
                "Execution steps cache initialized, total 22 item(s)" };

        //var logLines = File.ReadAllLines(TestContext.CurrentContext.TestDirectory +"\\out1.txt");

            var tokenizer = new WhiteSpaceLogTokenizer();

            var restoreFileHasher = new SourceFileHasher();
            restoreFileHasher.RestoreFromFile(tempSavePath);
            
            foreach (var logLine in logLines)
            {
                var tokenizedLine = tokenizer.TokenizeLine(logLine);

                var sourceEntries = restoreFileHasher.SearchSequence(tokenizedLine.ToList());

                foreach (var sourceEntry in sourceEntries)
                {
                    PrintSrcFile(restoreFileHasher.FilesDictionary[sourceEntry.FileHash], sourceEntry.LineNumber);
                }
            }
        }

        [Test]
        public void TestPackMan()
        {
            Console.WriteLine(SourceFileHasher.PackMan(new List<int>{1,11},new List<int>{3,1,1,3}));
        }

        private void PrintSrcFile(string path, int line)
        {
            try
            {
                Console.WriteLine(File.ReadAllLines(path)[line]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
