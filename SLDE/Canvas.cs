using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SLDE
{
	public class Canvas : Control
	{
		Rectangle rectangle; //= new Rectangle(50, 50, 50, 500);

		public Canvas()
		{
			//SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			/*SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.ResizeRedraw, true);*/
			//this.BackColor = Color.Transparent;
		}

		/*protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x20;
				return cp;
			}
		}*/

		public virtual Rectangle Rectangle
		{
			get { return rectangle; }
			set { rectangle = value; Refresh(); }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//e.Graphics.FillRectangle(new SolidBrush(Color.Transparent), this.ClientRectangle);
			e.Graphics.DrawRectangle(Pens.Blue, rectangle);
			base.OnPaint(e);
		}
	}
}
