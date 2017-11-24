using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRefactoring2.Vsix
{
    public interface ILogParser
    {
        IEnumerable<LogEntry> GetItems();
    }

    class JsonLogParser : ILogParser
    {
        /// <inheritdoc />
        public IEnumerable<LogEntry> GetItems()
        {
            
        }
    }

    public class LogEntry
    {
        public string Message { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}
