namespace SLDE
{
	partial class MainIDE
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
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.file = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.edit = new System.Windows.Forms.ToolStripMenuItem();
			this.view = new System.Windows.Forms.ToolStripMenuItem();
			this.tools = new System.Windows.Forms.ToolStripMenuItem();
			this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.thing1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.thing2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mainToolStrip = new System.Windows.Forms.ToolStrip();
			this.back = new System.Windows.Forms.ToolStripButton();
			this.forward = new System.Windows.Forms.ToolStripButton();
			this.newFile = new System.Windows.Forms.ToolStripSplitButton();
			this.openFile = new System.Windows.Forms.ToolStripButton();
			this.saveFile = new System.Windows.Forms.ToolStripButton();
			this.saveAll = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.undo = new System.Windows.Forms.ToolStripButton();
			this.redo = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.compile = new System.Windows.Forms.ToolStripButton();
			this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
			this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuStrip.SuspendLayout();
			this.mainToolStrip.SuspendLayout();
			this.toolStripContainer.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// statusStrip
			// 
			this.statusStrip.Location = new System.Drawing.Point(0, 699);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(807, 22);
			this.statusStrip.TabIndex = 0;
			this.statusStrip.Text = "statusStrip1";
			// 
			// menuStrip
			// 
			this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file,
            this.edit,
            this.view,
            this.tools,
            this.windowToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(807, 24);
			this.menuStrip.TabIndex = 1;
			this.menuStrip.Text = "menuStrip1";
			this.menuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
			// 
			// file
			// 
			this.file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
			this.file.Name = "file";
			this.file.Size = new System.Drawing.Size(37, 20);
			this.file.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.newToolStripMenuItem.Text = "New";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.openToolStripMenuItem.Text = "Open";
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.saveToolStripMenuItem.Text = "Save";
			// 
			// edit
			// 
			this.edit.Name = "edit";
			this.edit.Size = new System.Drawing.Size(39, 20);
			this.edit.Text = "Edit";
			// 
			// view
			// 
			this.view.Name = "view";
			this.view.Size = new System.Drawing.Size(44, 20);
			this.view.Text = "View";
			// 
			// tools
			// 
			this.tools.Name = "tools";
			this.tools.Size = new System.Drawing.Size(47, 20);
			this.tools.Text = "Tools";
			// 
			// windowToolStripMenuItem
			// 
			this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thing1ToolStripMenuItem,
            this.thing2ToolStripMenuItem});
			this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
			this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
			this.windowToolStripMenuItem.Text = "Window";
			// 
			// thing1ToolStripMenuItem
			// 
			this.thing1ToolStripMenuItem.Name = "thing1ToolStripMenuItem";
			this.thing1ToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.thing1ToolStripMenuItem.Text = "Thing1";
			// 
			// thing2ToolStripMenuItem
			// 
			this.thing2ToolStripMenuItem.Name = "thing2ToolStripMenuItem";
			this.thing2ToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.thing2ToolStripMenuItem.Text = "Thing2";
			// 
			// mainToolStrip
			// 
			this.mainToolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.back,
            this.forward,
            toolStripSeparator1,
            this.newFile,
            this.openFile,
            this.saveFile,
            this.saveAll,
            this.toolStripSeparator2,
            this.undo,
            this.redo,
            this.toolStripSeparator3,
            this.compile});
			this.mainToolStrip.Location = new System.Drawing.Point(3, 24);
			this.mainToolStrip.Name = "mainToolStrip";
			this.mainToolStrip.Size = new System.Drawing.Size(246, 25);
			this.mainToolStrip.TabIndex = 0;
			// 
			// back
			// 
			this.back.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.back.Image = global::SLDE.Properties.Resources.NavigateBackwards_6270;
			this.back.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.back.Name = "back";
			this.back.Size = new System.Drawing.Size(23, 22);
			this.back.Text = "toolStripButton1";
			// 
			// forward
			// 
			this.forward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.forward.Image = global::SLDE.Properties.Resources.NavigateForward_6271;
			this.forward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.forward.Name = "forward";
			this.forward.Size = new System.Drawing.Size(23, 22);
			this.forward.Text = "toolStripButton2";
			// 
			// newFile
			// 
			this.newFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newFile.Image = global::SLDE.Properties.Resources.NewFile_6276;
			this.newFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newFile.Name = "newFile";
			this.newFile.Size = new System.Drawing.Size(32, 22);
			this.newFile.Text = "toolStripSplitButton1";
			// 
			// openFile
			// 
			this.openFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openFile.Image = global::SLDE.Properties.Resources.Open_6529;
			this.openFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openFile.Name = "openFile";
			this.openFile.Size = new System.Drawing.Size(23, 22);
			this.openFile.Text = "toolStripButton4";
			// 
			// saveFile
			// 
			this.saveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveFile.Image = global::SLDE.Properties.Resources.Save_6530;
			this.saveFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveFile.Name = "saveFile";
			this.saveFile.Size = new System.Drawing.Size(23, 22);
			this.saveFile.Text = "toolStripButton5";
			// 
			// saveAll
			// 
			this.saveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveAll.Image = global::SLDE.Properties.Resources.Saveall_6518;
			this.saveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveAll.Name = "saveAll";
			this.saveAll.Size = new System.Drawing.Size(23, 22);
			this.saveAll.Text = "toolStripButton6";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// undo
			// 
			this.undo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.undo.Image = global::SLDE.Properties.Resources.Undo_16x;
			this.undo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.undo.Name = "undo";
			this.undo.Size = new System.Drawing.Size(23, 22);
			this.undo.Text = "toolStripButton1";
			// 
			// redo
			// 
			this.redo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.redo.Image = global::SLDE.Properties.Resources.Redo_16x;
			this.redo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.redo.Name = "redo";
			this.redo.Size = new System.Drawing.Size(23, 22);
			this.redo.Text = "toolStripButton2";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// compile
			// 
			this.compile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.compile.Image = global::SLDE.Properties.Resources.startwithoutdebugging_6556;
			this.compile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.compile.Name = "compile";
			this.compile.Size = new System.Drawing.Size(23, 22);
			this.compile.Text = "toolStripButton1";
			// 
			// BottomToolStripPanel
			// 
			this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
			this.BottomToolStripPanel.Name = "BottomToolStripPanel";
			this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
			// 
			// TopToolStripPanel
			// 
			this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
			this.TopToolStripPanel.Name = "TopToolStripPanel";
			this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
			// 
			// RightToolStripPanel
			// 
			this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
			this.RightToolStripPanel.Name = "RightToolStripPanel";
			this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
			// 
			// LeftToolStripPanel
			// 
			this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
			this.LeftToolStripPanel.Name = "LeftToolStripPanel";
			this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
			// 
			// ContentPanel
			// 
			this.ContentPanel.Size = new System.Drawing.Size(150, 175);
			// 
			// toolStripContainer
			// 
			this.toolStripContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// toolStripContainer.ContentPanel
			// 
			this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(807, 648);
			this.toolStripContainer.Location = new System.Drawing.Point(0, -1);
			this.toolStripContainer.Name = "toolStripContainer";
			this.toolStripContainer.Size = new System.Drawing.Size(807, 697);
			this.toolStripContainer.TabIndex = 4;
			this.toolStripContainer.Text = "toolStripContainer1";
			// 
			// toolStripContainer.TopToolStripPanel
			// 
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.mainToolStrip);
			// 
			// MainIDE
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(807, 721);
			this.Controls.Add(this.toolStripContainer);
			this.Controls.Add(this.statusStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainIDE";
			this.Text = "Main IDE";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.mainToolStrip.ResumeLayout(false);
			this.mainToolStrip.PerformLayout();
			this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.PerformLayout();
			this.toolStripContainer.ResumeLayout(false);
			this.toolStripContainer.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem file;
		private System.Windows.Forms.ToolStripMenuItem edit;
		private System.Windows.Forms.ToolStripMenuItem view;
		private System.Windows.Forms.ToolStripMenuItem tools;
		private System.Windows.Forms.ToolStrip mainToolStrip;
		private System.Windows.Forms.ToolStripContainer toolStripContainer;
		private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
		private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
		private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
		private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
		private System.Windows.Forms.ToolStripContentPanel ContentPanel;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem thing1ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem thing2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton back;
		private System.Windows.Forms.ToolStripButton forward;
		private System.Windows.Forms.ToolStripSplitButton newFile;
		private System.Windows.Forms.ToolStripButton openFile;
		private System.Windows.Forms.ToolStripButton saveFile;
		private System.Windows.Forms.ToolStripButton saveAll;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton undo;
		private System.Windows.Forms.ToolStripButton redo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton compile;
	}
}

