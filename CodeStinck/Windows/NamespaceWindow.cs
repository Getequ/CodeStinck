using EnvDTE;
using Getequ.CodeStinck.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Getequ.CodeStinck.Windows
{
    public partial class NamespaceWindow : Form
    {
        public NamespaceWindow()
        {
            InitializeComponent();
        }

        NamespaceFinder _finder;
        IEnumerable<NamespaceItem> _data;

        public void SetData(Solution solution)
        {
            _finder = new NamespaceFinder(solution);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            _data = _finder.FindSpaces();

            lbProject.Items.AddRange(_data.Select(x => x.Project).Distinct().OrderBy(x => x).ToArray());

            lbNamespace.Items.Clear();
            lbClass.Items.Clear();
        }

        private void lbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbProject.SelectedIndex == -1)
                return;

            var project = (string)lbProject.SelectedItem;
            lbNamespace.Items.Clear();
            lbNamespace.Items.AddRange(_data.Where(x => x.Project == project && !x.Namespace.Contains(".Version_20")).Select(x => x.Namespace).Distinct().ToArray());

            lbClass.Items.Clear();
        }

        private void lbNamespace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbNamespace.SelectedIndex == -1)
                return;

            var project = (string)lbProject.SelectedItem;
            var nspace = (string)lbNamespace.SelectedItem;
            lbClass.Items.Clear();
            lbClass.Items.AddRange(_data.Where(x => x.Project == project && x.Namespace == nspace).Select(x => x.Class).Distinct().ToArray());
        }
    }
}
