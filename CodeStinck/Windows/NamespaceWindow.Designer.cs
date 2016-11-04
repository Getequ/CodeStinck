namespace Getequ.CodeStinck.Windows
{
    partial class NamespaceWindow
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
            this.btnGo = new System.Windows.Forms.Button();
            this.lbProject = new System.Windows.Forms.ListBox();
            this.lbNamespace = new System.Windows.Forms.ListBox();
            this.lbClass = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(972, 12);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "Go!";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // lbProject
            // 
            this.lbProject.FormattingEnabled = true;
            this.lbProject.Location = new System.Drawing.Point(12, 51);
            this.lbProject.Name = "lbProject";
            this.lbProject.Size = new System.Drawing.Size(253, 498);
            this.lbProject.TabIndex = 1;
            this.lbProject.SelectedIndexChanged += new System.EventHandler(this.lbProject_SelectedIndexChanged);
            // 
            // lbNamespace
            // 
            this.lbNamespace.FormattingEnabled = true;
            this.lbNamespace.Location = new System.Drawing.Point(271, 51);
            this.lbNamespace.Name = "lbNamespace";
            this.lbNamespace.Size = new System.Drawing.Size(490, 498);
            this.lbNamespace.TabIndex = 2;
            this.lbNamespace.SelectedIndexChanged += new System.EventHandler(this.lbNamespace_SelectedIndexChanged);
            // 
            // lbClass
            // 
            this.lbClass.FormattingEnabled = true;
            this.lbClass.Location = new System.Drawing.Point(767, 51);
            this.lbClass.Name = "lbClass";
            this.lbClass.Size = new System.Drawing.Size(280, 498);
            this.lbClass.TabIndex = 3;
            // 
            // NamespaceWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 567);
            this.Controls.Add(this.lbClass);
            this.Controls.Add(this.lbNamespace);
            this.Controls.Add(this.lbProject);
            this.Controls.Add(this.btnGo);
            this.Name = "NamespaceWindow";
            this.Text = "NamespaceWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.ListBox lbProject;
        private System.Windows.Forms.ListBox lbNamespace;
        private System.Windows.Forms.ListBox lbClass;
    }
}