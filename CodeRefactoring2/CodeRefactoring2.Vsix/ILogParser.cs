using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRefactoring2.Vsix
{
    public interface ILogParser
    {
        IEnumerable<LogEntry> GetItems(string file);
    }

    public class DumbLogParser : ILogParser
    {
        public IEnumerable<LogEntry> GetItems(string file)
        {
            return File.ReadLines(file).Select(_=>new LogEntry {Message = _});
        }
    }

    class JsonLogParser : ILogParser
    {
        /// <inheritdoc />
        public IEnumerable<LogEntry> GetItems(string file)
        {
            return Enumerable.Empty<LogEntry>();
        }
    }
}
