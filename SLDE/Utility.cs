using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		static Utility()
		{
			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		}

		static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			allTypes = null;
		}

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

		static ReadOnlyCollection<Type> allTypes;

		public static ReadOnlyCollection<Type> AllTypes
		{
			get
			{
				if(allTypes == null)
				{
					var asm = AppDomain.CurrentDomain.GetAssemblies();
					var typeArray = new Type[asm.Length][];
					int totalCount = 0;
					for (int i = 0; i < asm.Length; i++)
					{
						typeArray[i] = asm[i].GetTypes();
						totalCount += typeArray[i].Length;
					}
					var types = new Type[totalCount];
					for(int i = 0, j = 0, k = 0; j < typeArray.Length; )
					{
						if(i < typeArray[j].Length)
						{
							types[k] = typeArray[j][i];
							i++;
							k++;
						}
						else
						{
							i = 0;
							j++;
						}
					}
					allTypes = new ReadOnlyCollection<Type>(types);
				}
				return allTypes;
			}
		}

		public static void ShowError(string text)
		{
			MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static void Call(this EventHandler handler, object sender, EventArgs e)
		{
			if (handler != null)
				handler(sender, e);
		}

	}


}
