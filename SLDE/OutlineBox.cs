using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	public partial class OutlineBox : UserControl
	{
		public static OutlineBox Get(Form form)
		{
			if (form == null)
				return null;
			for (int i = 0; i < form.Controls.Count; i++)
			{
				var box = form.Controls[i] as OutlineBox;
				if (box != null)
					return box;
			}
			var newBox = new OutlineBox();
			form.Controls.Add(newBox);
			return newBox;
		}

		public OutlineBox()
		{
			InitializeComponent();
		}
	}
}
