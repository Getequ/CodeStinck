namespace Getequ.CodeStinck.Windows
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.btnFindShit = new System.Windows.Forms.Button();
            this.lvStinck = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbDSVM = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbNested = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbPrivate = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbProperties = new System.Windows.Forms.ListBox();
            this.lbTotal = new System.Windows.Forms.Label();
            this.btnCreateViewModel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lbSvcOther = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lbVmOther = new System.Windows.Forms.ListBox();
            this.tabCode = new System.Windows.Forms.TabControl();
            this.tabController = new System.Windows.Forms.TabPage();
            this.tabVmClass = new System.Windows.Forms.TabPage();
            this.tabVmInterface = new System.Windows.Forms.TabPage();
            this.tabSvcClass = new System.Windows.Forms.TabPage();
            this.tabSvcInterface = new System.Windows.Forms.TabPage();
            this.tabDsClass = new System.Windows.Forms.TabPage();
            this.btnAddToProject = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabCode.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFindShit
            // 
            this.btnFindShit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFindShit.Location = new System.Drawing.Point(12, 553);
            this.btnFindShit.Name = "btnFindShit";
            this.btnFindShit.Size = new System.Drawing.Size(120, 24);
            this.btnFindShit.TabIndex = 3;
            this.btnFindShit.Text = "Найти в решении";
            this.btnFindShit.UseVisualStyleBackColor = true;
            this.btnFindShit.Click += new System.EventHandler(this.btnFindShit_Click);
            // 
            // lvStinck
            // 
            this.lvStinck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvStinck.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader7,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvStinck.FullRowSelect = true;
            this.lvStinck.Location = new System.Drawing.Point(12, 12);
            this.lvStinck.Name = "lvStinck";
            this.lvStinck.Size = new System.Drawing.Size(443, 530);
            this.lvStinck.TabIndex = 4;
            this.lvStinck.UseCompatibleStateImageBehavior = false;
            this.lvStinck.View = System.Windows.Forms.View.Details;
            this.lvStinck.SelectedIndexChanged += new System.EventHandler(this.lvStinck_SelectedIndexChanged);
            this.lvStinck.DoubleClick += new System.EventHandler(this.lvStinck_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Контроллер";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Base";
            this.columnHeader7.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "VM/DS";
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Prop";
            this.columnHeader3.Width = 50;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Private";
            this.columnHeader4.Width = 50;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Nested";
            this.columnHeader5.Width = 50;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Всего";
            this.columnHeader6.Width = 50;
            // 
            // lbDSVM
            // 
            this.lbDSVM.FormattingEnabled = true;
            this.lbDSVM.Location = new System.Drawing.Point(461, 29);
            this.lbDSVM.Name = "lbDSVM";
            this.lbDSVM.Size = new System.Drawing.Size(179, 147);
            this.lbDSVM.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(461, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Методы ViewModel/DomainService";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(646, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Вложенные классы";
            // 
            // lbNested
            // 
            this.lbNested.FormattingEnabled = true;
            this.lbNested.Location = new System.Drawing.Point(646, 29);
            this.lbNested.Name = "lbNested";
            this.lbNested.Size = new System.Drawing.Size(188, 147);
            this.lbNested.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(646, 366);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Скрытые методы";
            // 
            // lbPrivate
            // 
            this.lbPrivate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbPrivate.FormattingEnabled = true;
            this.lbPrivate.Location = new System.Drawing.Point(646, 382);
            this.lbPrivate.Name = "lbPrivate";
            this.lbPrivate.Size = new System.Drawing.Size(188, 160);
            this.lbPrivate.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(461, 366);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Свойства";
            // 
            // lbProperties
            // 
            this.lbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbProperties.FormattingEnabled = true;
            this.lbProperties.Location = new System.Drawing.Point(461, 382);
            this.lbProperties.Name = "lbProperties";
            this.lbProperties.Size = new System.Drawing.Size(179, 160);
            this.lbProperties.TabIndex = 9;
            // 
            // lbTotal
            // 
            this.lbTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbTotal.AutoSize = true;
            this.lbTotal.Location = new System.Drawing.Point(138, 559);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(10, 13);
            this.lbTotal.TabIndex = 13;
            this.lbTotal.Text = "-";
            // 
            // btnCreateViewModel
            // 
            this.btnCreateViewModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreateViewModel.Location = new System.Drawing.Point(461, 555);
            this.btnCreateViewModel.Name = "btnCreateViewModel";
            this.btnCreateViewModel.Size = new System.Drawing.Size(373, 45);
            this.btnCreateViewModel.TabIndex = 14;
            this.btnCreateViewModel.Text = "Составить классы ViewModel, Service";
            this.btnCreateViewModel.UseVisualStyleBackColor = true;
            this.btnCreateViewModel.Click += new System.EventHandler(this.btnCreateViewModel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(646, 183);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Сервис-методы";
            // 
            // lbSvcOther
            // 
            this.lbSvcOther.FormattingEnabled = true;
            this.lbSvcOther.Location = new System.Drawing.Point(646, 199);
            this.lbSvcOther.Name = "lbSvcOther";
            this.lbSvcOther.Size = new System.Drawing.Size(188, 160);
            this.lbSvcOther.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(461, 183);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Прочие VM-методы";
            // 
            // lbVmOther
            // 
            this.lbVmOther.FormattingEnabled = true;
            this.lbVmOther.Location = new System.Drawing.Point(461, 199);
            this.lbVmOther.Name = "lbVmOther";
            this.lbVmOther.Size = new System.Drawing.Size(179, 160);
            this.lbVmOther.TabIndex = 18;
            // 
            // tabCode
            // 
            this.tabCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCode.Controls.Add(this.tabController);
            this.tabCode.Controls.Add(this.tabVmClass);
            this.tabCode.Controls.Add(this.tabVmInterface);
            this.tabCode.Controls.Add(this.tabSvcClass);
            this.tabCode.Controls.Add(this.tabSvcInterface);
            this.tabCode.Controls.Add(this.tabDsClass);
            this.tabCode.Location = new System.Drawing.Point(840, 14);
            this.tabCode.Name = "tabCode";
            this.tabCode.SelectedIndex = 0;
            this.tabCode.Size = new System.Drawing.Size(494, 528);
            this.tabCode.TabIndex = 22;
            // 
            // tabController
            // 
            this.tabController.Location = new System.Drawing.Point(4, 22);
            this.tabController.Name = "tabController";
            this.tabController.Padding = new System.Windows.Forms.Padding(3);
            this.tabController.Size = new System.Drawing.Size(486, 502);
            this.tabController.TabIndex = 2;
            this.tabController.Text = "Controller";
            this.tabController.UseVisualStyleBackColor = true;
            // 
            // tabVmClass
            // 
            this.tabVmClass.Location = new System.Drawing.Point(4, 22);
            this.tabVmClass.Name = "tabVmClass";
            this.tabVmClass.Padding = new System.Windows.Forms.Padding(3);
            this.tabVmClass.Size = new System.Drawing.Size(486, 502);
            this.tabVmClass.TabIndex = 0;
            this.tabVmClass.Text = "ViewModel";
            this.tabVmClass.UseVisualStyleBackColor = true;
            // 
            // tabVmInterface
            // 
            this.tabVmInterface.Location = new System.Drawing.Point(4, 22);
            this.tabVmInterface.Name = "tabVmInterface";
            this.tabVmInterface.Padding = new System.Windows.Forms.Padding(3);
            this.tabVmInterface.Size = new System.Drawing.Size(486, 502);
            this.tabVmInterface.TabIndex = 1;
            this.tabVmInterface.Text = "VM Interface";
            this.tabVmInterface.UseVisualStyleBackColor = true;
            // 
            // tabSvcClass
            // 
            this.tabSvcClass.Location = new System.Drawing.Point(4, 22);
            this.tabSvcClass.Name = "tabSvcClass";
            this.tabSvcClass.Padding = new System.Windows.Forms.Padding(3);
            this.tabSvcClass.Size = new System.Drawing.Size(486, 502);
            this.tabSvcClass.TabIndex = 3;
            this.tabSvcClass.Text = "Service";
            this.tabSvcClass.UseVisualStyleBackColor = true;
            // 
            // tabSvcInterface
            // 
            this.tabSvcInterface.Location = new System.Drawing.Point(4, 22);
            this.tabSvcInterface.Name = "tabSvcInterface";
            this.tabSvcInterface.Padding = new System.Windows.Forms.Padding(3);
            this.tabSvcInterface.Size = new System.Drawing.Size(486, 502);
            this.tabSvcInterface.TabIndex = 4;
            this.tabSvcInterface.Text = "Service interface";
            this.tabSvcInterface.UseVisualStyleBackColor = true;
            // 
            // tabDsClass
            // 
            this.tabDsClass.Location = new System.Drawing.Point(4, 22);
            this.tabDsClass.Name = "tabDsClass";
            this.tabDsClass.Padding = new System.Windows.Forms.Padding(3);
            this.tabDsClass.Size = new System.Drawing.Size(486, 502);
            this.tabDsClass.TabIndex = 5;
            this.tabDsClass.Text = "-";
            this.tabDsClass.UseVisualStyleBackColor = true;
            // 
            // btnAddToProject
            // 
            this.btnAddToProject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddToProject.Location = new System.Drawing.Point(840, 555);
            this.btnAddToProject.Name = "btnAddToProject";
            this.btnAddToProject.Size = new System.Drawing.Size(490, 45);
            this.btnAddToProject.TabIndex = 23;
            this.btnAddToProject.Text = "Создать в проекте";
            this.btnAddToProject.UseVisualStyleBackColor = true;
            this.btnAddToProject.Click += new System.EventHandler(this.btnAddToProject_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(13, 581);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 23);
            this.button1.TabIndex = 24;
            this.button1.Text = "Найти в проекте";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1346, 616);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnAddToProject);
            this.Controls.Add(this.tabCode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbSvcOther);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbVmOther);
            this.Controls.Add(this.btnCreateViewModel);
            this.Controls.Add(this.lbTotal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbPrivate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbProperties);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbNested);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbDSVM);
            this.Controls.Add(this.lvStinck);
            this.Controls.Add(this.btnFindShit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Бой коду в контроллерах!";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabCode.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFindShit;
        private System.Windows.Forms.ListView lvStinck;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ListBox lbDSVM;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbNested;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbPrivate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbProperties;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.Button btnCreateViewModel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lbSvcOther;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lbVmOther;
        private System.Windows.Forms.TabControl tabCode;
        private System.Windows.Forms.TabPage tabVmClass;
        private System.Windows.Forms.TabPage tabVmInterface;
        private System.Windows.Forms.TabPage tabController;
        private System.Windows.Forms.Button btnAddToProject;
        private System.Windows.Forms.TabPage tabSvcClass;
        private System.Windows.Forms.TabPage tabSvcInterface;
        private System.Windows.Forms.TabPage tabDsClass;
        private System.Windows.Forms.Button button1;
    }
}