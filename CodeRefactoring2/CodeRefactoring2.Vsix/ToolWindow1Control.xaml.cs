using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio.LanguageServices;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace CodeRefactoring2.Vsix
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {
        [Import(nameof(VisualStudioWorkspace))]
        public VisualStudioWorkspace MyWorkspace { get; set; }

        public ObservableCollection<LogEntry> Items { get; } = new ObservableCollection<LogEntry>(LogEntry.GetSample);

        private SourceFileHasher _sourceFileHasher = new SourceFileHasher();

        private Task _scanTask = Task.CompletedTask;
        Dictionary<int, List<SourceFileHasher.NewSourceEntry>> storeDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void LoadLog(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(AnalyzeLogPackage.Dte.Solution.FileName);

            /*MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'. My workspace {1}", ToString(), MyWorkspace?.ToString() ?? "Null"),
                "ToolWindow1");*/

            var openFileDialog = new OpenFileDialog { CheckFileExists = true};// Image files(*.bmp, *.jpg) , Filter = "Logs(*.txt;*.log)"

            if (openFileDialog.ShowDialog() == true)
            {
                var dumbLogParser = new DumbLogParser();
                foreach (var logEntry in dumbLogParser.GetItems(openFileDialog.FileName)
                    .Take(10))
                {
                    Items.Add(new LogEntry {Message = logEntry.Message});
                }
            }

            storeDictionary = new Dictionary<int, List<SourceFileHasher.NewSourceEntry>>();

            var folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _sourceFileHasher.ProcessDir(folderBrowserDialog.SelectedPath, storeDictionary );
                InitBackgroundCorrelation();
            }
        }

        private void InitBackgroundCorrelation()
        {
            if (!_scanTask.IsCompleted || _sourceFileHasher == null) return;

            _scanTask = Task.Run(
                () =>
                    {
                        var tokenizer = new WhiteSpaceLogTokenizer();
                        for (var i = 0; i < Items.Count; i++)
                        {
                            var logEntry = Items[i];
                            var tokenizedLine = tokenizer.TokenizeLine(logEntry.Message);
                            var sourceEntry = _sourceFileHasher.SearchNew(
                                                         tokenizedLine.ToList(),
                                                         storeDictionary)
                                                     .FirstOrDefault();

                            logEntry.SourceEntry = sourceEntry;
                        }

                        Console.WriteLine("all done");
                    });
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {//https://social.msdn.microsoft.com/Forums/vstudio/en-US/b797980f-f6be-4a5c-93ef-f179e431d113/datagridhyperlinkcolumn-run-code-on-click-and-read-cell-text?forum=wpf
            Hyperlink link = e.OriginalSource as Hyperlink;
            if (link.DataContext is LogEntry le)
            {
                System.Windows.Forms.MessageBox.Show($"{le?.SourceEntry}");
            }
        }
    }
}