namespace ColorTabControlTest
{
    partial class ControlDemo
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
            this.CTabControl = new ColorTabControl.ColorTabControl();
            this.tabOne = new System.Windows.Forms.TabPage();
            this.tabTwo = new System.Windows.Forms.TabPage();
            this.tabThree = new System.Windows.Forms.TabPage();
            this.CTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // CTabControl
            // 
            this.CTabControl.Controls.Add(this.tabOne);
            this.CTabControl.Controls.Add(this.tabTwo);
            this.CTabControl.Controls.Add(this.tabThree);
            this.CTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.CTabControl.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CTabControl.InactiveBGColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.CTabControl.InactiveColorOn = true;
            this.CTabControl.InactiveFGColor = System.Drawing.SystemColors.ControlText;
            this.CTabControl.Location = new System.Drawing.Point(12, 12);
            this.CTabControl.Multiline = true;
            this.CTabControl.Name = "CTabControl";
            this.CTabControl.SelectedIndex = 0;
            this.CTabControl.Size = new System.Drawing.Size(418, 247);
            this.CTabControl.TabIndex = 0;
            // 
            // tabOne
            // 
            this.tabOne.BackColor = System.Drawing.Color.Teal;
            this.tabOne.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabOne.ForeColor = System.Drawing.Color.White;
            this.tabOne.Location = new System.Drawing.Point(4, 23);
            this.tabOne.Name = "tabOne";
            this.tabOne.Padding = new System.Windows.Forms.Padding(3);
            this.tabOne.Size = new System.Drawing.Size(410, 220);
            this.tabOne.TabIndex = 0;
            this.tabOne.Text = "W.Color";
            // 
            // tabTwo
            // 
            this.tabTwo.BackColor = System.Drawing.Color.Peru;
            this.tabTwo.Font = new System.Drawing.Font("Arial", 9.75F);
            this.tabTwo.ForeColor = System.Drawing.Color.Black;
            this.tabTwo.Location = new System.Drawing.Point(4, 23);
            this.tabTwo.Name = "tabTwo";
            this.tabTwo.Padding = new System.Windows.Forms.Padding(3);
            this.tabTwo.Size = new System.Drawing.Size(410, 220);
            this.tabTwo.TabIndex = 1;
            this.tabTwo.Text = "W.Color";
            // 
            // tabThree
            // 
            this.tabThree.BackColor = System.Drawing.Color.Gray;
            this.tabThree.Font = new System.Drawing.Font("Arial", 9.75F);
            this.tabThree.ForeColor = System.Drawing.Color.White;
            this.tabThree.Location = new System.Drawing.Point(4, 23);
            this.tabThree.Name = "tabThree";
            this.tabThree.Padding = new System.Windows.Forms.Padding(3);
            this.tabThree.Size = new System.Drawing.Size(410, 220);
            this.tabThree.TabIndex = 2;
            this.tabThree.Text = "VStyle.Color";
            this.tabThree.UseVisualStyleBackColor = true;
            // 
            // ControlDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 273);
            this.Controls.Add(this.CTabControl);
            this.Name = "ControlDemo";
            this.Text = "Tab Control Demo";
            this.CTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ColorTabControl.ColorTabControl CTabControl;
        private System.Windows.Forms.TabPage tabOne;
        private System.Windows.Forms.TabPage tabTwo;
        private System.Windows.Forms.TabPage tabThree;
    }
}

