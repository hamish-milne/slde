using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SLDE
{
	public partial class WindowManager : ContainerControl
	{
		public WindowManager()
		{
			InitializeComponent();
		}

		public WindowManager(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}
	}
}
