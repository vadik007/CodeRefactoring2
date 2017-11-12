using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefactoring2.Vsix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeRefactoring2.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
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
    }
}
