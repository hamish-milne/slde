namespace SLDE
{
	partial class ErrorList
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorList));
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.separator2 = new System.Windows.Forms.ToolStripSeparator();
			this.separator3 = new System.Windows.Forms.ToolStripSeparator();
			this.textBox = new System.Windows.Forms.TextBox();
			this.filterButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.errorButton = new System.Windows.Forms.ToolStripButton();
			this.warningButton = new System.Windows.Forms.ToolStripButton();
			this.messageButton = new System.Windows.Forms.ToolStripButton();
			this.Icon = new System.Windows.Forms.DataGridViewImageColumn();
			this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.File = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Line = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToOrderColumns = true;
			this.dataGridView.AllowUserToResizeRows = false;
			this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
			this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Icon,
            this.Description,
            this.File,
            this.Line,
            this.Column});
			this.dataGridView.Location = new System.Drawing.Point(0, 28);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.RowHeadersVisible = false;
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView.Size = new System.Drawing.Size(400, 372);
			this.dataGridView.TabIndex = 0;
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "StatusAnnotations_Information_16xLG.png");
			this.imageList.Images.SetKeyName(1, "StatusAnnotations_Information_16xLG_color.png");
			this.imageList.Images.SetKeyName(2, "StatusAnnotations_Warning_16xLG.png");
			this.imageList.Images.SetKeyName(3, "StatusAnnotations_Warning_16xLG_color.png");
			this.imageList.Images.SetKeyName(4, "StatusAnnotations_Critical_16xLG.png");
			this.imageList.Images.SetKeyName(5, "StatusAnnotations_Critical_16xLG_color.png");
			// 
			// toolStrip
			// 
			this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterButton,
            this.separator1,
            this.errorButton,
            this.separator2,
            this.warningButton,
            this.separator3,
            this.messageButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(293, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip_ItemClicked);
			// 
			// separator1
			// 
			this.separator1.Name = "separator1";
			this.separator1.Size = new System.Drawing.Size(6, 25);
			// 
			// separator2
			// 
			this.separator2.Name = "separator2";
			this.separator2.Size = new System.Drawing.Size(6, 25);
			// 
			// separator3
			// 
			this.separator3.Name = "separator3";
			this.separator3.Size = new System.Drawing.Size(6, 25);
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(266, 3);
			this.textBox.MaximumSize = new System.Drawing.Size(200, 20);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(131, 20);
			this.textBox.TabIndex = 2;
			// 
			// filterButton
			// 
			this.filterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.filterButton.Image = global::SLDE.Properties.Resources.Filter;
			this.filterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.filterButton.Name = "filterButton";
			this.filterButton.Size = new System.Drawing.Size(29, 22);
			this.filterButton.ToolTipText = "Filter";
			// 
			// errorButton
			// 
			this.errorButton.Image = global::SLDE.Properties.Resources.CriticalColor_16;
			this.errorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.errorButton.Name = "errorButton";
			this.errorButton.Size = new System.Drawing.Size(57, 22);
			this.errorButton.Text = "Errors";
			// 
			// warningButton
			// 
			this.warningButton.Image = global::SLDE.Properties.Resources.WarningColor;
			this.warningButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.warningButton.Name = "warningButton";
			this.warningButton.Size = new System.Drawing.Size(77, 22);
			this.warningButton.Text = "Warnings";
			// 
			// messageButton
			// 
			this.messageButton.Image = global::SLDE.Properties.Resources.InformationColor;
			this.messageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.messageButton.Name = "messageButton";
			this.messageButton.Size = new System.Drawing.Size(78, 22);
			this.messageButton.Text = "Messages";
			// 
			// Icon
			// 
			this.Icon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Icon.HeaderText = "";
			this.Icon.Name = "Icon";
			this.Icon.ReadOnly = true;
			this.Icon.Width = 30;
			// 
			// Description
			// 
			this.Description.HeaderText = "Description";
			this.Description.Name = "Description";
			this.Description.ReadOnly = true;
			// 
			// File
			// 
			this.File.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.File.HeaderText = "File";
			this.File.Name = "File";
			this.File.ReadOnly = true;
			// 
			// Line
			// 
			this.Line.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Line.HeaderText = "Line";
			this.Line.Name = "Line";
			this.Line.ReadOnly = true;
			this.Line.Width = 40;
			// 
			// Column
			// 
			this.Column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Column.HeaderText = "Column";
			this.Column.Name = "Column";
			this.Column.ReadOnly = true;
			this.Column.Width = 50;
			// 
			// ErrorList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.dataGridView);
			this.Name = "ErrorList";
			this.Size = new System.Drawing.Size(400, 400);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripDropDownButton filterButton;
		private System.Windows.Forms.ToolStripSeparator separator1;
		private System.Windows.Forms.ToolStripButton errorButton;
		private System.Windows.Forms.ToolStripSeparator separator2;
		private System.Windows.Forms.ToolStripButton warningButton;
		private System.Windows.Forms.ToolStripSeparator separator3;
		private System.Windows.Forms.ToolStripButton messageButton;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.DataGridViewImageColumn Icon;
		private System.Windows.Forms.DataGridViewTextBoxColumn Description;
		private System.Windows.Forms.DataGridViewTextBoxColumn File;
		private System.Windows.Forms.DataGridViewTextBoxColumn Line;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column;

	}
}
