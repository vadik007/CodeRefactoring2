using System.Collections.Generic;

namespace CodeRefactoring2.Vsix
{
    public class LogEntry
    {
        public string Message { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        public SourceFileHasher.NewSourceEntry SourceEntry { get; set; }
        public static IEnumerable<LogEntry> GetSample => new []
                                                  {
                                                      new LogEntry
                                                      {
                                                          Message = "Message1",
                                                          Properties = new Dictionary<string, object> { {"prop1", 1}, {"prop2", 2}}
                                                      },
                                                      new LogEntry
                                                      {
                                                          Message = "Message2",
                                                          Properties = new Dictionary<string, object> { {"prop1", 1 }, {"prop3", 3}},
                                                          SourceEntry = new SourceFileHasher.NewSourceEntry
                                                          { FileHash = 1, Line = 1, Position = 1, WordHash = 3}
                                                      }
                                                  };
    }
}