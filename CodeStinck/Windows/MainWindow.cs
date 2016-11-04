using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using EnvDTE;
using EnvDTE80;
using FastColoredTextBoxNS;

namespace Getequ.CodeStinck.Windows
{
    using Domain;
    using Models;

    public partial class MainWindow : Form
    {
        ControllerDomain _controllerDomain;
        IEnumerable<ControllerStinck> _stinckData;
        int _contrCount;
        Dictionary<string, FastColoredTextBox> _codeEditors;
        ClassGenerationResult _lastResult;

        public MainWindow()
        {
            InitializeComponent();

            _codeEditors = new Dictionary<string, FastColoredTextBox>();

            CreateEditor(tabVmClass,      "ViewModelClass");
            CreateEditor(tabVmInterface,  "ViewModelInterface");
            CreateEditor(tabSvcClass,     "ServiceClass");
            CreateEditor(tabSvcInterface, "ServiceInterface");
            CreateEditor(tabDsClass,      "DomainService");
            CreateEditor(tabController,   "Controller");
        }

        public void SetProject(Project project, Solution solution)
        {
            _controllerDomain = new ControllerDomain(project, solution);
        }

        void CreateEditor(Control panel, string name)
        {
            var editor = new FastColoredTextBox();
            editor.Name = "fctb" + name;
            editor.Left = 0;
            editor.Top = 0;
            editor.Language = FastColoredTextBoxNS.Language.Custom;
            editor.Width = panel.ClientSize.Width;
            editor.Height = panel.ClientSize.Height;
            editor.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            editor.BorderStyle = BorderStyle.FixedSingle;
            editor.Font = new Font("Consolas", 10F);
            editor.ReadOnly = true;
            editor.CharHeight = 15;
            editor.CharWidth = 8;
            editor.Tag = new List<string>();

            TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
            TextStyle BoldStyle = new TextStyle(new SolidBrush(Color.FromArgb(43, 145, 175)), null, FontStyle.Regular);
            TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
            TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
            TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
            TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular);
            TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
            MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

            editor.TextChanged += (s, e) =>
            {
                if (((FastColoredTextBox)s).Language == FastColoredTextBoxNS.Language.Custom)
                {
                    ((FastColoredTextBox)s).LeftBracket = '(';
                    ((FastColoredTextBox)s).RightBracket = ')';
                    ((FastColoredTextBox)s).LeftBracket2 = '\x0';
                    ((FastColoredTextBox)s).RightBracket2 = '\x0';
                    //clear style of changed range
                    e.ChangedRange.ClearStyle(BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle);

                    //string highlighting
                    e.ChangedRange.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
                    //comment highlighting
                    e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
                    e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
                    e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
                    //number highlighting
                    e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
                    //attribute highlighting
                    e.ChangedRange.SetStyle(GrayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
                    //class name highlighting
                    e.ChangedRange.SetStyle(BoldStyle, @"\b(" + string.Join("|", ((IEnumerable<string>)((FastColoredTextBox)s).Tag)) + @")\b");
                    //keyword highlighting
                    e.ChangedRange.SetStyle(BlueStyle, @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|var|where|yield)\b|#region\b|#endregion\b");

                    //clear folding markers
                    e.ChangedRange.ClearFoldingMarkers();

                    //set folding markers
                    e.ChangedRange.SetFoldingMarkers("{", "}");//allow to collapse brackets block
                    e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");//allow to collapse #region blocks
                    e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");//allow to collapse comment block
                }
            };

            panel.Controls.Add(editor);
            _codeEditors.Add(name, editor);
        }

        private void btnFindShit_Click(object sender, EventArgs e)
        {
            var controllers = _controllerDomain.FindControllers();
            _contrCount = controllers.Count();
            _stinckData = _controllerDomain.FindStinck(controllers);

            UpdateForm();
        }

        private void UpdateForm()
        {
            lvStinck.Items.Clear();

            foreach (var stinck in _stinckData.OrderBy(x => x.Name))
            {
                var lvi = lvStinck.Items.Add(stinck.Name);
                lvi.Tag = stinck;

                lvi.SubItems.Add(stinck.Class.Base);
                lvi.SubItems.Add(stinck.BadPublicMethods.Count.ToString());
                lvi.SubItems.Add(stinck.NonServiceProps.Count.ToString());
                lvi.SubItems.Add(stinck.NonPublicMethods.Count.ToString());
                lvi.SubItems.Add(stinck.NestedClasses.Count.ToString());

                lvi.SubItems.Add((stinck.BadPublicMethods.Count
                 + stinck.NonServiceProps.Count
                 + stinck.NonPublicMethods.Count
                 + stinck.NestedClasses.Count).ToString());
            }

            lbTotal.Text = string.Format("Контроллеров: {0} из {2}, Всего несоответствий: {1}", _stinckData.Count(), _stinckData.Sum(stinck => stinck.BadPublicMethods.Count
                 + stinck.NonServiceProps.Count
                 + stinck.NonPublicMethods.Count
                 + stinck.NestedClasses.Count), _contrCount);
        }

        private void lvStinck_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvStinck.SelectedItems.Count == 0)
                return;

            ControllerStinck stinck = (ControllerStinck)lvStinck.SelectedItems[0].Tag;

            lbDSVM.Items.Clear();
            lbDSVM.Items.AddRange(stinck.BadPublicMethods.Select(x => x.Name).ToArray());

            lbNested.Items.Clear();
            lbNested.Items.AddRange(stinck.NestedClasses.Select(x => x.Name).ToArray());
            
            lbVmOther.Items.Clear();
            lbVmOther.Items.AddRange(stinck.ViewModelMethods.Select(x => x.Name).ToArray());

            lbSvcOther.Items.Clear();
            lbSvcOther.Items.AddRange(stinck.ServiceMethods.Select(x => x.Name).ToArray());

            lbPrivate.Items.Clear();
            lbPrivate.Items.AddRange(stinck.NonPublicMethods.Select(x => x.Name).ToArray());

            lbProperties.Items.Clear();
            lbProperties.Items.AddRange(stinck.NonServiceProps.Select(x => x.Name).ToArray());

            var editor = _codeEditors["Controller"];
            editor.Text = string.Join(Environment.NewLine, ProjectItemHelper.GetFileLines(stinck.ProjectItem));
            tabCode.SelectTab("tabController");
        }

        private void lvStinck_DoubleClick(object sender, EventArgs e)
        {
        }

        private void btnCreateViewModel_Click(object sender, EventArgs e)
        {
            if (lvStinck.SelectedItems.Count == 0)
                return;
            
            _lastResult = null;
            ControllerStinck stinck = (ControllerStinck)lvStinck.SelectedItems[0].Tag;
            try
            {
                //foreach (CodeImport import in stinck.ProjectItem.FileCodeModel.CodeElements.OfType<CodeImport>())
                //{
                //    Debug.WriteLine(import.Name + "     " + import.Namespace + "        - ");
                //}

                //foreach (CodeNamespace ns in stinck.ProjectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>())
                //{
                //    foreach (CodeImport import in ns.Members.OfType<CodeImport>())
                //        Debug.WriteLine("ns:  " + import.Name + "     " + import.Namespace + "        - ");
                //}
                //return;
                var result = _controllerDomain.GenerateRefactoringCode(stinck);

                foreach (var editor in _codeEditors.Values)
                {
                    editor.Tag = result.KnownTypes;
                }
                _codeEditors["ViewModelClass"].Text = result.ViewModelClass;
                _codeEditors["ViewModelInterface"].Text = result.ViewModelInterface;
                _codeEditors["ServiceClass"].Text = result.ServiceClass;
                _codeEditors["ServiceInterface"].Text = result.ServiceInterface;
                //_codeEditors["DomainService"].Text = result.DomainServiceClass;
                _codeEditors["Controller"].Text = result.Controller;

                tabCode.SelectTab("tabController");
                _lastResult = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCreateService_Click(object sender, EventArgs e)
        {
        }

        private void btnAddToProject_Click(object sender, EventArgs e)
        {
            _controllerDomain.AddToProject(_lastResult);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var controllers = _controllerDomain.FindControllers(true);
            _contrCount = controllers.Count();
            _stinckData = _controllerDomain.FindStinck(controllers);

            UpdateForm();
        }
    }
}
