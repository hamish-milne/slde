namespace SLDE
{
	partial class CodeEditor
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
			this.textEditorControl = new DigitalRune.Windows.TextEditor.TextEditorControl();
			this.SuspendLayout();
			// 
			// textEditorControl
			// 
			this.textEditorControl.AllowDrop = true;
			this.textEditorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEditorControl.Location = new System.Drawing.Point(0, 0);
			this.textEditorControl.Name = "textEditorControl";
			this.textEditorControl.ShowHRuler = true;
			this.textEditorControl.ShowLineNumbers = false;
			this.textEditorControl.Size = new System.Drawing.Size(147, 147);
			this.textEditorControl.TabIndex = 0;
			// 
			// CodeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textEditorControl);
			this.Name = "CodeEditor";
			this.ResumeLayout(false);

		}

		#endregion

		public DigitalRune.Windows.TextEditor.TextEditorControl textEditorControl;

	}
}
