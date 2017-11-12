using System.Collections.ObjectModel;
using System.Composition;
using EnvDTE;
using Microsoft.VisualStudio.LanguageServices;

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

        public ObservableCollection<string> Items { get; } = new ObservableCollection<string>() { "1", "2", "3" };

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(AnalyzeLogPackage.Dte.Solution.FileName);
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'. My workspace {1}", this.ToString(), MyWorkspace?.ToString() ?? "Null"),
                "ToolWindow1");
        }
    }
}