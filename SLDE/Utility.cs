using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SLDE
{
	/// <summary>
	/// Provides common utility functions and extension methods
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// All the anchor styles
		/// </summary>
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

		/// <summary>
		/// Gets the source control under the context menu item
		/// </summary>
		/// <param name="sender">The sender of the event</param>
		/// <returns>The source control, or <c>null</c></returns>
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

		/// <summary>
		/// Sets a control to completely fill its parent container
		/// </summary>
		/// <param name="content"></param>
		public static void FillParent(this Control content)
		{
			if (content.Parent == null)
				return;
			content.Location = new Point();
			content.Anchor = Utility.AllAnchors;
			content.Size = content.Parent.ClientSize;
		}

		static ReadOnlyCollection<Type> allTypes;

		/// <summary>
		/// A list of all types that aren't in the System* or Windows* assemblies
		/// </summary>
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
						var fn = asm[i].FullName;
						if (fn.StartsWith("System", StringComparison.Ordinal) ||
							fn.StartsWith("Windows", StringComparison.Ordinal))
							continue;
						try
						{
							typeArray[i] = asm[i].GetTypes();
						} catch(TypeLoadException)
						{
							ShowError("Unable to load " + asm[i].GetName().Name);
						}
						totalCount += typeArray[i].Length;
					}
					var types = new Type[totalCount];
					for(int i = 0, j = 0, k = 0; j < typeArray.Length; )
					{
						if(typeArray[j] != null && i < typeArray[j].Length)
						{
							types[k] = typeArray[j][i];
							i++;
							k++;
						}
						else
						{
							i = 0;
							do { j++; }
							while (j < typeArray.Length && typeArray[j] == null);
						}
					}
					allTypes = new ReadOnlyCollection<Type>(types);
				}
				return allTypes;
			}
		}

		/// <summary>
		/// Creates an instance of each T-derived type that has a TAttribute attribute
		/// </summary>
		/// <typeparam name="T">The instance type</typeparam>
		/// <typeparam name="TAttribute">The attribute type</typeparam>
		/// <param name="onFail">If a type cannot be created, this delegate is called</param>
		/// <returns>A list of all the instances</returns>
		public static List<T> CreateListOf<T, TAttribute>(Action<Type> onFail) where TAttribute : Attribute
		{
			var ret = new List<T>();
			for(int i = 0; i < AllTypes.Count; i++)
			{
				var t = AllTypes[i];
				if(t.IsSubclassOf(typeof(T)) && t.GetCustomAttributes(typeof(TAttribute), false).Length > 0)
				{
					try
					{
						ret.Add((T)Activator.CreateInstance(t));
					}
					catch
					{
						if (onFail != null)
							onFail(t);
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// Shows an error message box
		/// </summary>
		/// <param name="text">The error text</param>
		public static void ShowError(string text)
		{
			MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

	}


}
