using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRefactoring2.Vsix
{
    public interface ILogTokenizer
    {
        IEnumerable<int> TokenizeLine(string line);
    }

    public class WhiteSpaceLogTokenizer : ILogTokenizer
    {
        public IEnumerable<int> TokenizeLine(string line)
        {
            return line.Split(' ').Select(token => token.GetHashCode());
        }
    }
}
