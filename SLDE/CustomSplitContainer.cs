using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	/// <summary>
	/// A SplitContainer that changes orientation when double-clicked
	/// </summary>
	public class CustomSplitContainer : SplitContainer
	{
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);

			var horiz = Orientation == Orientation.Horizontal;
			var setting = (float)SplitterDistance / (float)(horiz ? Height : Width);
			Orientation = horiz
				? Orientation.Vertical : Orientation.Horizontal;
			SplitterDistance = (int)(setting * (float)(horiz ? Width : Height));
		}
	}
}
