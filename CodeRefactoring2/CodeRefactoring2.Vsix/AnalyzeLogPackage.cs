using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace CodeRefactoring2.Vsix
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(AnalyzeLogPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideToolWindow(typeof(CodeRefactoring2.Vsix.ToolWindow1))]
    public sealed class AnalyzeLogPackage : Package
    {
        /// <summary>
        /// AnalyzeLogPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "07f232d7-a3ec-49ee-b564-0fb4f7f8bb68";

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeLog"/> class.
        /// </summary>
        public AnalyzeLogPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        public static DTE Dte;

        public static string[] ReadLogFile(string path)
        {
            var logLines = File.ReadAllLines(path);
            foreach (var logLine in logLines)
            {
            }

            return new string[5];

        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            //GetService(typeof(Microsoft.VisualStudio.ComponentModelHost))
            //((IVsUIShell)GetService(typeof(IServiceProvider)))
            //Microsoft.VisualStudio.Services.IVsCodeContainerProviderService u;
            //Microsoft.VisualStudio.Services.SVsCodeContainerProviderService i;i
            //var service = ((SVsComponentModelHost)this.GetService(typeof(SVsComponentModelHost)));

            Dte = GetService<DTE>();

            AnalyzeLog.Initialize(this);
            base.Initialize();
            CodeRefactoring2.Vsix.ToolWindow1Command.Initialize(this);
        }

        private T GetService<T>()
        {
            return (T)base.GetService(typeof(T));
        }

        public static void GotoLineAtFile(string filename, int line)
        {
            //dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");
            //AnalyzeLogPackage.Dte.MainWindow.Activate();
            Window w = Dte.ItemOperations.OpenFile(filename, EnvDTE.Constants.vsViewKindTextView);
            ((TextSelection)Dte.ActiveDocument.Selection).GotoLine(line, true);
        }
        #endregion
    }
}
