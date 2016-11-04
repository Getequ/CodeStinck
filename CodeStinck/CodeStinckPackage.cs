using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

using EnvDTE;
using EnvDTE80;
using Getequ.CodeStinck.Windows;

namespace Getequ.CodeStinck
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSEnumClassesPkgString)]
    public sealed class CodeStinckPackage : ExtensionPointPackage
    {
        private static DTE2 _dte;
        public const string Version = "1.2";

        public CodeStinckPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        protected override void Initialize()
        {
            _dte = GetService(typeof(DTE)) as DTE2;
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidVSEnumClassesCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );

                CommandID menuCommandID2 = new CommandID(GuidList.guidVSNamespacesCmdSet, (int)PkgCmdIDList.cmdidMyCommand2);
                MenuCommand menuItem2 = new MenuCommand(NamespaceCallback, menuCommandID2);
                mcs.AddCommand(menuItem2);
            }
        }
        
        private void MenuItemCallback(object sender, EventArgs e)
        {
            UIHierarchyItem item = GetSelectedItem();

            if (item == null)
                return;

            ProjectItem projectItem = item.Object as ProjectItem;
            Project project = item.Object as Project;

            if (project == null)
            {
                project = projectItem.ContainingProject;
            }
            if (project == null)
            {
                throw new ArgumentException("Project not found!");
            }

            var dialog = new MainWindow();
            dialog.SetProject(project, _dte.Solution);
            dialog.Show();
            
            /*// Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "VSEnumClasses",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result));*/
        }

        private void NamespaceCallback(object sender, EventArgs e)
        {
            UIHierarchyItem item = GetSelectedItem();

            if (item == null)
                return;

            ProjectItem projectItem = item.Object as ProjectItem;
            Project project = item.Object as Project;

            if (project == null)
            {
                project = projectItem.ContainingProject;
            }
            if (project == null)
            {
                throw new ArgumentException("Project not found!");
            }

            var dialog = new NamespaceWindow();
            dialog.SetData(_dte.Solution);
            dialog.Show();
        }

        private static UIHierarchyItem GetSelectedItem()
        {
            var items = (Array)_dte.ToolWindows.SolutionExplorer.SelectedItems;

            foreach (UIHierarchyItem selItem in items)
            {
                return selItem;
            }

            return null;
        }
    }
}
