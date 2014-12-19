namespace SLDE
{
	partial class Editor
	{

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// Editor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "Editor";
			this.ShowHRuler = true;
			this.Size = new System.Drawing.Size(200, 200);
			this.CompletionRequest += new System.EventHandler<DigitalRune.Windows.TextEditor.Completion.CompletionEventArgs>(this.Editor_CompletionRequest);
			this.ParentChanged += new System.EventHandler(this.Editor_ParentChanged);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
