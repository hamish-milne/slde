using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	public class CustomSplitContainer : SplitContainer
	{
		protected override void OnDoubleClick(EventArgs e)
		{
			Orientation = Orientation == Orientation.Horizontal ?
				Orientation.Vertical : Orientation.Horizontal;
		}
	}
}
