namespace JazCms.WebProject.WinEditor
{
	partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.dataGridNodesTable = new System.Windows.Forms.DataGridView();
            this.contextMenuStripDataGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pathToPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.namespaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.existingJazFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridNodesTable)).BeginInit();
            this.contextMenuStripDataGridView.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(755, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.projectSettingsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            // 
            // projectSettingsToolStripMenuItem
            // 
            this.projectSettingsToolStripMenuItem.Name = "projectSettingsToolStripMenuItem";
            this.projectSettingsToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.projectSettingsToolStripMenuItem.Text = "Project settings";
            this.projectSettingsToolStripMenuItem.Click += new System.EventHandler(this.projectSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonCreate.FlatAppearance.BorderSize = 3;
            this.buttonCreate.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.buttonCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCreate.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.buttonCreate.Location = new System.Drawing.Point(587, 332);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(146, 33);
            this.buttonCreate.TabIndex = 2;
            this.buttonCreate.Text = "Modify";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // dataGridNodesTable
            // 
            this.dataGridNodesTable.AllowUserToAddRows = false;
            this.dataGridNodesTable.AllowUserToDeleteRows = false;
            this.dataGridNodesTable.AllowUserToOrderColumns = true;
            this.dataGridNodesTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridNodesTable.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dataGridNodesTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridNodesTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridNodesTable.GridColor = System.Drawing.Color.Navy;
            this.dataGridNodesTable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dataGridNodesTable.Location = new System.Drawing.Point(0, 27);
            this.dataGridNodesTable.Name = "dataGridNodesTable";
            this.dataGridNodesTable.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridNodesTable.Size = new System.Drawing.Size(755, 279);
            this.dataGridNodesTable.TabIndex = 3;
            // 
            // contextMenuStripDataGridView
            // 
            this.contextMenuStripDataGridView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.existingJazFilesToolStripMenuItem,
            this.pathToPageToolStripMenuItem,
            this.classNameToolStripMenuItem,
            this.namespaceToolStripMenuItem,
            this.basePageToolStripMenuItem});
            this.contextMenuStripDataGridView.Name = "contextMenuStripDataGridView";
            this.contextMenuStripDataGridView.Size = new System.Drawing.Size(162, 114);
            // 
            // pathToPageToolStripMenuItem
            // 
            this.pathToPageToolStripMenuItem.Checked = true;
            this.pathToPageToolStripMenuItem.CheckOnClick = true;
            this.pathToPageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pathToPageToolStripMenuItem.Enabled = false;
            this.pathToPageToolStripMenuItem.Name = "pathToPageToolStripMenuItem";
            this.pathToPageToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.pathToPageToolStripMenuItem.Text = "Path to page";
            // 
            // classNameToolStripMenuItem
            // 
            this.classNameToolStripMenuItem.Checked = true;
            this.classNameToolStripMenuItem.CheckOnClick = true;
            this.classNameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.classNameToolStripMenuItem.Name = "classNameToolStripMenuItem";
            this.classNameToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.classNameToolStripMenuItem.Text = "Class name";
            // 
            // namespaceToolStripMenuItem
            // 
            this.namespaceToolStripMenuItem.Checked = true;
            this.namespaceToolStripMenuItem.CheckOnClick = true;
            this.namespaceToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.namespaceToolStripMenuItem.Name = "namespaceToolStripMenuItem";
            this.namespaceToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.namespaceToolStripMenuItem.Text = "Namespace";
            // 
            // basePageToolStripMenuItem
            // 
            this.basePageToolStripMenuItem.Checked = true;
            this.basePageToolStripMenuItem.CheckOnClick = true;
            this.basePageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.basePageToolStripMenuItem.Name = "basePageToolStripMenuItem";
            this.basePageToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.basePageToolStripMenuItem.Text = "Base page";
            // 
            // existingJazFilesToolStripMenuItem
            // 
            this.existingJazFilesToolStripMenuItem.Checked = true;
            this.existingJazFilesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.existingJazFilesToolStripMenuItem.Enabled = false;
            this.existingJazFilesToolStripMenuItem.Name = "existingJazFilesToolStripMenuItem";
            this.existingJazFilesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.existingJazFilesToolStripMenuItem.Text = "Existing jaz files";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(755, 408);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dataGridNodesTable);
            this.Controls.Add(this.buttonCreate);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "JazCms: Web Project Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridNodesTable)).EndInit();
            this.contextMenuStripDataGridView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.DataGridView dataGridNodesTable;
        private System.Windows.Forms.ToolStripMenuItem projectSettingsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDataGridView;
        private System.Windows.Forms.ToolStripMenuItem pathToPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem classNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem namespaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basePageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem existingJazFilesToolStripMenuItem;
	}
}

