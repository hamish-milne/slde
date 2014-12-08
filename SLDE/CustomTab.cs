using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SLDE
{
	public interface ITabFactory
	{
		TabControl CreateTabControl();
	}

	[ToolboxItem(true)]
	public partial class CustomTab : TabControl, ITabFactory
	{

		bool dragging = false;
		bool justMoved = false;
		bool mainWindow = false;
		int lastDragIndex = -1;
		ITabFactory tabFactory;

		public event EventHandler DragTab;

		public CustomTab()
		{
			InitializeComponent();
			tabFactory = this;
		}

		public TabControl CreateTabControl()
		{
			return new CustomTab();
		}

		[Browsable(true)]
		public virtual bool MainWindow
		{
			get { return mainWindow; }
			set { mainWindow = value; }
		}

		public int GetTabUnderPosition(Point p)
		{
			for(int i = 0; i < TabCount; i++)
			{
				if (GetTabRect(i).Contains(p))
					return i;
			}
			return -1;
		}

		protected virtual void DragOutTab(TabPage tab, MouseEventArgs e)
		{
			TabPages.Remove(tab);
			var dragTabForm = new Form();
			//dragTabForm.FormBorderStyle = FormBorderStyle.None;
			dragTabForm.Location = e.Location;
			var dragTabs = tabFactory.CreateTabControl();
			dragTabs.Parent = dragTabForm;
			dragTabs.Size = dragTabForm.ClientSize;
			dragTabs.Anchor = Utility.AllAnchors;
			dragTabForm.Size = this.Size;
			dragTabs.TabPages.Add(tab);
			dragTabForm.Show();
			if (!mainWindow && TabCount < 1 && Parent != null)
				Parent.Dispose();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			dragging = false;
			base.OnMouseUp(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			lastDragIndex = GetTabUnderPosition(e.Location);
			dragging = (lastDragIndex >= 0);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(!dragging)
				return;
			int newIndex = GetTabUnderPosition(e.Location);
			if(newIndex < 0)
			{
				// Drag tab out
				if(lastDragIndex < TabCount && lastDragIndex >= 0)
				{
					var t = TabPages[lastDragIndex];
					//TabPages.RemoveAt(lastDragIndex);
					DragOutTab(t, e);
				}
			} else
			{
				// It's (theoretically) possible that a tab could be removed while dragging
				// This make sure the program doesn't crash in that case
				if (lastDragIndex < TabCount && lastDragIndex >= 0)
				{
					if(justMoved)
					{
						if (newIndex == SelectedIndex)
							justMoved = false;
					} else if(newIndex != lastDragIndex)
					{
						// Drag tab along
						var tab1 = TabPages[lastDragIndex];
						TabPages[lastDragIndex] = TabPages[newIndex];
						TabPages[newIndex] = tab1;
						SelectedIndex = newIndex;
						justMoved = true;
					}
				}
			}
			lastDragIndex = newIndex;
		}

		protected override CreateParams CreateParams
		{
			get
			{
				var cp = base.CreateParams;
				cp.ExStyle |= 0x2000000;
				return cp;
			}
		}
		
	}

}
