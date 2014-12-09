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
	/// <summary>
	/// An improved TabControl that handles dragging and has a close button
	/// </summary>
	[ToolboxItem(true)]
	public partial class IDETabControl : TabControl
	{
		Form dragForm;
		bool dragging = false;
		bool justMoved = false;
		bool mainWindow = false;
		int lastDragIndex = -1;
		Point dragPoint;
		ContextMenuStrip contextMenu;

		public IDETabControl()
		{
			InitializeComponent();
		}

		public TabControl CreateTabControl()
		{
			return new IDETabControl();
		}

		[Browsable(true)]
		public virtual bool MainWindow
		{
			get { return mainWindow; }
			set { mainWindow = value; }
		}

		[Browsable(true)]
		new public virtual ContextMenuStrip ContextMenuStrip
		{
			get { return contextMenu; }
			set { contextMenu = value; }
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
			// This gets the top corner of the dragged tab
			var top = PointToScreen(GetTabRect(TabPages.IndexOf(tab)).Location);

			// If this isn't the main window, and the parent is a Form (i.e. there are no split panes)
			bool useThisForm = !mainWindow && Parent is Form && TabCount == 1;
			var dragTabForm = useThisForm ? (Form)Parent : new Form();
			
			// Create a new tab control, or if we're using this form, use the current tab control
			var dragTabs = useThisForm ? this : new IDETabControl();
			if(!useThisForm)
			{
				dragTabs.Parent = dragTabForm;
				dragTabs.Size = dragTabForm.ClientSize;
				dragTabForm.Size = this.Size;
				dragTabs.ContextMenuStrip = ContextMenuStrip;
				dragTabs.ImageList = ImageList;
				dragTabForm.Show();
			}
			
			// Calculate the new location for the form, such that the cursor
			// is at the same place on the new tab
			var screenPoint = this.PointToScreen(dragPoint);
			var mousePoint = this.PointToScreen(e.Location);
			var loc = dragTabForm.Location;
			var screen = dragTabForm.PointToScreen(default(Point));
			
			dragTabForm.Location = new Point(
				mousePoint.X - (screenPoint.X - top.X) - (screen.X - loc.X),
				mousePoint.Y - (screenPoint.Y - top.Y) - (screen.Y - loc.Y)
				);

			// We have to get all the position data *before* switching the tab over
			// otherwise it's horribly offset
			if(!useThisForm)
			{
				dragTabs.TabPages.Add(tab);
				dragTabs.Focus();
			}

			// For some reason, newly created forms won't respond to mouse events
			var tabToSet = !useThisForm ? this : dragTabs;
			tabToSet.dragForm = dragTabForm;
			tabToSet.dragPoint = dragTabs.PointToClient(mousePoint);

			dragTabForm.FormBorderStyle = FormBorderStyle.None;
			if (!mainWindow && !useThisForm && TabCount < 1 && Parent != null)
				Parent.Parent = null;
		}

		public void Close()
		{
			var splitPane = Parent as SplitterPanel;
			if (splitPane == null)
			{
				var form = Parent as Form;
				if(form != null)
				{
					form.Dispose();
				} else
				{
					Parent = null;
				}
				return;
			}
			var container = (SplitContainer)splitPane.Parent;
			var otherPane = splitPane == container.Panel1 ? container.Panel2 : container.Panel1;
			if (otherPane.Controls.Count < 1)
				return;
			var content = otherPane.Controls[0];
			content.Parent = container.Parent;
			content.FillParent();
			container.Parent = null;
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);

			if (TabCount <= 1)
			{
				// We only want to keep the empty control if it's the last
				// one in the main window
				if (!MainWindow || Parent is SplitterPanel)
					Close();
				
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			dragging = false;
			if(dragForm != null)
				dragForm.FormBorderStyle = FormBorderStyle.Sizable;
			dragForm = null;
			base.OnMouseUp(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			lastDragIndex = GetTabUnderPosition(e.Location);
			dragPoint = e.Location;
			dragging = (lastDragIndex >= 0);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			for (int i = 0; i < TabCount; i++)
			{
				var tab = TabPages[i];
				if (tab.ImageIndex != 1)
					tab.ImageIndex = 1;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if(ImageList != null)
				for (int i = 0; i < TabCount; i++)
				{
					var rect = GetTabRect(i);
					rect = new Rectangle(rect.X + Margin.Left, rect.Y + Margin.Top, ImageList.ImageSize.Width, ImageList.ImageSize.Height);
					int newImage = rect.Contains(e.Location) ? 0 : 1;
					var tab = TabPages[i];
					if (tab.ImageIndex != newImage)
						tab.ImageIndex = newImage;
				}

			if(dragForm != null)
			{
				dragForm.Location = Subtract(PointToScreen(e.Location), dragPoint);
				return;
			}
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

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			if (e.Button == MouseButtons.Left)
			{
				for (int i = 0; i < TabCount; i++)
				{
					var tab = TabPages[i];
					if (tab.ImageIndex == 0)
						tab.Dispose();
				}
			}
			else if (e.Button == MouseButtons.Right && ContextMenuStrip != null)
			{
				for (int i = 0; i < TabCount; ++i)
				{
					if (GetTabRect(i).Contains(e.Location))
					{
						SelectedIndex = i;
						ContextMenuStrip.Show(this, e.Location);
						break;
					}
				}
			}
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

		public static Point Add(Point a, Point b)
		{
			return new Point(a.X + b.X, a.Y + b.Y);
		}

		public static Point Subtract(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}
		
	}

}
