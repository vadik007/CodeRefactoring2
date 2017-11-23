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

                var sourceEntries = restoreFileHasher.SearchSequence(tokenizedLine.ToList(), restoreFileHasher.FilesToTokens);

                foreach (var sourceEntry in sourceEntries)
                {
                    PrintSrcFile(restoreFileHasher.FilesDictionary[sourceEntry.FileHash], sourceEntry.LineNumber);
                }
            }
        }

        [TestCase(new[]{1,1}, new[] { 3, 1, 1, 3}, true)]
        public void TestPackMan(int[] x, int[] y, bool isSubSequece)
        {
            //SourceFileHasher.PackMan(x.ToList() ,y.ToList());
        }

        [TestCase(new[] {1, 1}, new[] {3, 1, 1, 3}, 1, TestName = "new[] {1, 1}, new[] {3, 1, 1, 3}, 1")]
        [TestCase(new[] {-1, 1}, new[] {3, -1, 1, 3}, 1, TestName = "new[] {-1, 1}, new[] {3, -1, 1, 3}, 1")]
        [TestCase(new[] {3, 4}, new[] {3, 1, 1, 3}, -1, TestName = "new[] {3, 4}, new[] {3, 1, 1, 3}, -1")]
        [TestCase(new[] {2, -2}, new[] {3, 1, 1, 3, 2, -2}, 4, TestName = "new[] {2, -2}, new[] {3, 1, 1, 3, 2, -2}")]
        [TestCase(
            new[] { 133152514, -1184914118, 231732805 },
            new[] { -871204755, 973381012, 1874483263, 1624877773, 1543969225, 638668808, -676180319, 372029313, -1226285125, 1464456049, 1849315756, 372029313, -1436242094, -1505079682, 1849315756, 372029313, -1226285125, -745902839, -1332314092, -699674396, 648495949, -1850262934, 905413809, -1226284737, 372029336, 1010958237, 648495949, -537263047, 905413809, 2053027190}, -1, TestName = "long test")]
        [TestCase(new[] { 133152514, -1184914118, 231732805 }, new[] { -1634930328, -1171547949, 372029313, -1436242094, -1692160183, 1543969225, -960115789, -466770094, -2074829595, -212941395, -1171547949, 372029313, -1226285125, -760988419, 372029319, -1022830959, 372029319, 413702276, -1088580612, -1318967004, 179924729, 568694201, 568694201, 44551392, 568694201, 568694201, 568694201, 506422643, 2047781653, 1874605120, 682351901, -1167856832, 502129296, -205543555, 1676951915, -367551303, -2051240644, 1414245146, -1184914118, -1591218326, -644686711, -649887332, -2051240644, 1414245146, -1184914118, -1591218326, -182859838, -2116327277, 656301929, 554914783, -2051240644, 1414245146, -1184914118, -1591218326, -845619555, 1840092177, 1840092177, -2051240644, 1414245146, -1184914118, -1591218326, 656301929, 595427817, -971704226, 682351901, -1105298376, 1802864212, 595427817, 371857150, 741151649, -1772455899, 1461139713, 1502598403, -1979237789, 372029307, 372029307, 339798785, 1502598673, -1985799191, 955473081, -2146507805, -875737927, -2128110318, -1772455899, 1459778593, 1502598403, -1979237789, 372029307, 372029307, 339798785, 1502598673, -1985799191, 955473081, -2146507805, -875737927, -649887332, 1104820737, -484865750, -649887393, 2078431085, 372029331, 372029312, 1043481692, 1344981223, 696029554, -548254997, 372029336, 372029312, 371857150, 1346342343, -758195490, -871205760, 646357719, 371857150, 371857150, 16695879, 419572679, -286988559, 1459778593, -2047301602, -1848288638, 1496915075, 1561970665, -470451862, -971704226, 371857150, 741151649, -1772455899, 1344981223, 1502598403, -1979237789, 372029307, 372029307, 339798785, 1502598673, 1683135521, -1772455899, 1344981223, 1502598403, -1979237789, 372029307, 372029307, 696029554, 1502598673, -284857929, 1104820737, -484865750, -649887393, 1346342343, -758195490, -871205760, 646357719, 371857150, 371857150, 16695879, 419572679, -286988559, 1459778593, -2047301602, -1848288638, 1496915075, 1561970665, -470451862, 656301929, 1840092177, -644686711, 682351901, -1105298376, 1802864212, -1281271461, -1105298376, 129749988, 741151649, -1772455899, 1344981223, 1502598403, -1979237789, 372029307, 372029307, -284857929, -1772455899, 1344981223, 1502598403, -1979237789, 372029307, 372029307, 696029554, 1683135521, 1346342343, -758195490, -871205760, 646357719, 371857150, 371857150, 16695879, 419572679, -286988559, 1459778593, -2047301602, -1848288638, 1496915075, 1561970665, -470451862, -1636101789, 712858132, 1757179279, 712858132, 22409400, -1636101789, -361572265, 372029313, -1436242094, 877801146, -1933764928, -1461554873, -816464124, -460329843, -1966748341, 287061489, 712858132, -214283158, -1476534706, -259436845, 615562268, -873555856, 1757179279, 712858132, 22409400, 215200345, -345812296, -58843243, 683412643, 683412643, 371857150, 372029314, 712858132, -214283158, -1476534706, -259436845, 615562268, -873555856, 1757179279, 712858132, 22409400, -1735709172, 1308698333, 712858132, 1757179279, 22409400, -1217936878, -361572265, 372029313, -1226285125, -1834216175, 372029313, -1436242094, 877801146, -1933764928, -629803013, -1651439926, 1670613009, -1217936878, -1834216175, 372029313, -1226285125, -789586131, 372029313, -1436242094, 877801146, 104529068, -789586131, 372029313, -1226285125, -104687777, 372029313, -1436242094, 877801146, 104529068, 371857150, -104687777, 372029313, -1226285125, -2882042, 372029313, -1436242094, 877801146, 104529068, 581998473, 1945545832, -1226435613, -1688061066, 1314498490, 372029329, -2042057376, 372029319, -1435347082, 1104568029, 729051186, -1732106660, 287061521, 712858132, 1757179279, 712858132, -1370267367, 372029312, 372029319, 1856683490, 372029319, 1229316431, 371857150, -2882042, 372029313, -1226285125, -523111509, 372029313, -1436242094, 371857150, -523111509, 372029313, -1226285125, 1556299777, 372029313, -1436242094, 877801146, -1933764928, -1461554873, 1556299777, 372029313, -1226285125, 1239117161, 372029313, -1436242094, 877801146, -1933764928, -629803013, -1651439926, 1670613009, 1644372160, -616374832, -669363985, -239556721, -410265081, -637985309, -1767991640, -419259730, 1472092850, 1426742466, 110983193, 1542933508, -598824153, 1351771660, 1542933508, 707329661, 1119721018, -874467563, -1034507148, -1052777767, -874467563, 1049641042, 350016039, -665719123, 72872257, 1239117161, 372029313, -1226285125, -351493941, 372029319, 472052342, 372029332, 372029333, 372029331, -351493942, 372029336, -1530483375, 372029319, -303475725, 372029319, 1220271341, 372029319, -267376662, 372029332, 372029333, 372029331, -351493942, 372029336, -1530483375, 372029319, 1957991511, 372029319, -1844639398, 372029319, -681656482, 372029332, 372029333, 372029331, -351493942, 372029336, -1530483375, 372029319, -1712236425, 372029319, -1414503680, 372029319, -1123092804, 372029332, 372029333, 372029331, -351493942, 372029336, -1530483375, 372029319, 18302281, 372029319, -1414503680, 372029319, -1692856436, 372029332, 372029333, 372029331, -351493942, 372029336, -1530483375, 372029319, -2133018653, 372029319, 465358923, -265093935, 372029313, -1436242094, 877801146, 104529068, -265093935, 372029313, -1226285125, 605519264, 372029313, -1436242094, 877801146, 104529068, 605519264, 372029313, -1226285125, 1586309156, 372029313, -1436242094, 877801146, 104529068, 1586309156, 372029313, -1226285125, 1586309156, 372029313, -1436242094, 877801146, 104529068, 1586309156, 372029313, -1226285125, -1699766522, 372029313, -1436242094, 877801146, 104529068, -1699766522, 372029313, -1226285125, -1699766522, 372029313, -1226285125, 1948271663, 372029313, -1436242094, 877801146, 104529068, 1948271663, 372029313, -1226285125, 1948271663, 372029313, -1226285125 }, 51, TestName = "idx 51")]

        public void TestArrInArr(int[] x, int[] y, int idx)
        {
            Assert.AreEqual(idx, SourceFileHasher.FindArrayIndex(x, y));
            if (idx != -1)
            {
                Assert.IsTrue(x[0] == y[idx]);
            }
        }

        [Test]
        public void TestNew()
        {
            var sourceFileHasher = new SourceFileHasher();
            var tokenDictionary = new Dictionary<int, List<SourceFileHasher.NewSourceEntry>>();

            foreach (var file in Directory.GetFiles(@"D:\CONGEN\cnt\agent-library\", "*.cs", SearchOption.AllDirectories))
            {
                sourceFileHasher.ProcessFile2(file, tokenDictionary);
            }

            var logLines = new string[]{ "AgentHostProcess is started",
                "Service configured on port 8085 of channel http.Server object created",
                "Execution steps cache initialized, total 22 item(s)" };

            //var logLines = File.ReadAllLines(TestContext.CurrentContext.TestDirectory +"\\out1.txt");

            var tokenizer = new WhiteSpaceLogTokenizer();
            foreach (var logLine in logLines)
            {
                foreach (var newSourceEntry in sourceFileHasher.SearchNew(tokenizer.TokenizeLine(logLine).ToList(), tokenDictionary))
                {
                    PrintSrcFile(sourceFileHasher.FilesDictionary[newSourceEntry.FileHash], newSourceEntry.Line);
                }
            }

        }

        private void PrintSrcFile(string path, int line)
        {
            try
            {
                var theLine = File.ReadAllLines(path)[line];
                TestContext.WriteLine(theLine);
                Console.WriteLine(theLine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
