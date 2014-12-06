using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SLDE
{
	public static class Utility
	{
		public const AnchorStyles AllAnchors =
			AnchorStyles.Bottom |
			AnchorStyles.Left |
			AnchorStyles.Right |
			AnchorStyles.Top;

		public static Control GetSourceControl(this object sender)
		{
			Control ret = null;
			try
			{
				ret = ((ContextMenuStrip)((ToolStripMenuItem)sender).GetCurrentParent()).SourceControl;
			}
			catch
			{
			}
			return ret;
		}

		public static void FillParent(this Control content)
		{
			content.Location = new Point();
			content.Size = content.Parent.Size;
		}
	}


}
