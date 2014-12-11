﻿using System;
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
		static List<IDETabControl> allControls = new List<IDETabControl>();

		public static IList<IDETabControl> AllControls
		{
			get { return allControls; }
		}

		Form dragForm;
		TabPage dragTab;
		bool dragging = false;
		bool justMoved = false;
		bool mainWindow = false;
		int lastDragIndex = -1;
		Point dragPoint;
		ContextMenuStrip contextMenu;
		TabPosition lastPosition;
		IDETabControl lastControl;

		public IDETabControl()
		{
			InitializeComponent();
			allControls.Add(this);
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
			Rectangle rect;
			return GetTabUnderPosition(p, out rect);
		}

		public int GetTabUnderPosition(Point p, out Rectangle rect)
		{
			rect = default(Rectangle);
			for(int i = 0; i < TabCount; i++)
			{
				rect = GetTabRect(i);
				if (rect.Contains(p))
					return i;
			}
			return -1;
		}

		protected override void Dispose(bool disposing)
		{
			allControls.Remove(this);
			base.Dispose(disposing);
		}

		protected virtual IDETabControl Copy()
		{
			var tabs = new IDETabControl();
			tabs.mainWindow = mainWindow;
			tabs.ImageList = ImageList;
			tabs.ContextMenuStrip = ContextMenuStrip;
			return tabs;
		}

		protected virtual void DragOutTab(TabPage tab, MouseEventArgs e)
		{
			// This gets the top corner of the dragged tab
			var top = PointToScreen(GetTabRect(TabPages.IndexOf(tab)).Location);

			// If this isn't the main window, and the parent is a Form
			// (i.e. there are no split panes)
			bool useThisForm = !mainWindow && Parent is Form && TabCount == 1;
			var dragTabForm = useThisForm ? (Form)Parent : new Form();
			
			// Create a new tab control, or if we're using this form,
			// use the current tab control
			var dragTabs = useThisForm ? this : Copy();
			if(!useThisForm)
			{
				dragTabs.Parent = dragTabForm;
				dragTabs.Size = dragTabForm.ClientSize;
				dragTabForm.ClientSize = this.Size;
				dragTabs.mainWindow = false;
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
			tabToSet.dragTab = tab;
			tabToSet.dragForm = dragTabForm;
			tabToSet.dragPoint = dragTabs.PointToClient(mousePoint);

			dragTabForm.FormBorderStyle = FormBorderStyle.None;
			if (!mainWindow && !useThisForm && TabCount < 1 && Parent.Parent is Form)
				Parent.Parent = null;
		}

		List<TabPage> tabsToRemove = new List<TabPage>();
		bool closing;
		public bool Close()
		{
			if (closing)
				return true;
			closing = true;
			SelectedIndex = 0;
			tabsToRemove.Clear();
			for (int i = 0; i < TabCount; i++)
				tabsToRemove.Add(TabPages[i]);
			for (int i = 0; i < tabsToRemove.Count; i++)
			{
				var tab = tabsToRemove[i] as IDETab;
				if (tab == null)
					tabsToRemove[i].Parent = null;
				else if(!tab.Remove())
				{
					tabsToRemove.Clear();
					closing = false;
					return false;
				}
			}
			tabsToRemove.Clear();
			var splitPane = Parent as SplitterPanel;
			if (splitPane == null)
			{
				var form = Parent as Form;
				if(form != null)
				{
					form.Dispose();
				} else
				{
					if (!MainWindow)
						Parent = null;
				}
				closing = false;
				return true;
			}
			closing = false;
			var container = (SplitContainer)splitPane.Parent;
			var otherPane = splitPane == container.Panel1 ?
				container.Panel2 : container.Panel1;
			if (otherPane.Controls.Count < 1)
				return true;
			var content = otherPane.Controls[0];
			content.Parent = container.Parent;
			content.FillParent();
			container.Parent = null;
			return true;
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			Close();
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			if (TabCount <= 1)
				Close();
		}

		protected enum TabPosition { None, Tab, Left, Right, Bottom }

		protected TabPosition GetTabHover(Point mouseLocation,
			out IDETabControl control, out int index, out Rectangle rect)
		{
			control = null;
			index = -1;
			var screenPoint = this.PointToScreen(mouseLocation);
			rect = default(Rectangle);
			for (int i = 0; i < AllControls.Count; i++)
			{
				control = AllControls[i];
				if (control.Parent == dragForm)
					continue;
				var clientPoint = control.PointToClient(screenPoint);
				if(control.TabCount < 1)
				{
					rect = control.ClientRectangle;
					if(rect.Contains(clientPoint))
					{
						index = 0;
						return TabPosition.Tab;
					}
					continue;
				}
				index = control.GetTabUnderPosition(clientPoint);
				var t0 = control.GetTabRect(0);
				var headerRect = new Rectangle(t0.Location,
					new Size(control.ClientRectangle.Width, t0.Height));
				var tabRect = new Rectangle(control.ClientRectangle.X,
					control.ClientRectangle.Y + headerRect.Height,
					control.ClientRectangle.Width,
					control.ClientRectangle.Height - headerRect.Height);
				rect = tabRect;
				if (index >= 0)
					return TabPosition.Tab;
				if(headerRect.Contains(clientPoint))
				{
					index = control.TabCount;
					return TabPosition.Tab;
				}
				var newHeight = (tabRect.Height / 2);
				rect = new Rectangle(new Point(tabRect.X, tabRect.Y + newHeight),
					new Size(tabRect.Width, newHeight));
				if (rect.Contains(clientPoint))
					return TabPosition.Bottom;
				rect = new Rectangle(tabRect.Location,
					new Size(tabRect.Width / 2, tabRect.Height));
				if (rect.Contains(clientPoint))
					return TabPosition.Left;
				rect = new Rectangle(new Point(rect.X + rect.Width, rect.Y),
					rect.Size);
				if (rect.Contains(clientPoint))
					return TabPosition.Right;
			}
			control = null;
			index = -1;
			return TabPosition.None;
		}

		public virtual IDETabControl Split(bool second, bool vertical)
		{
			var newContainer = new CustomSplitContainer();
			newContainer.Orientation = vertical ? Orientation.Vertical :
				Orientation.Horizontal;
			newContainer.Parent = Parent;
			newContainer.FillParent();
			Parent = second ? newContainer.Panel1 : newContainer.Panel2;
			this.FillParent();
			var newTabs = Copy();
			newTabs.Parent = second ? newContainer.Panel2 : newContainer.Panel1;
			newTabs.FillParent();
			newContainer.SplitterDistance = vertical ? newContainer.Size.Width / 2
				: newContainer.Size.Height / 2;
			return newTabs;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			dragging = false;
			if(dragForm != null)
			{
				IDETabControl control;
				int index;
				Rectangle rect;
				switch(GetTabHover(e.Location, out control, out index, out rect))
				{
					case TabPosition.Tab:
						control.TabPages.Insert(index, dragTab);
						break;
					case TabPosition.Left:
						dragTab.Parent = control.Split(false, true);
						break;
					case TabPosition.Right:
						dragTab.Parent = control.Split(true, true);
						break;
					case TabPosition.Bottom:
						dragTab.Parent = control.Split(true, false);
						break;
					default:
						dragForm.FormBorderStyle = FormBorderStyle.Sizable;
						break;
				}
				dragTab.Focus();
				((TabControl)dragTab.Parent).SelectedTab = dragTab;
				if (control != null && control.FindForm() != null)
					OutlineBox.Get(control.FindForm()).Visible = false;
				dragTab.Refresh();
			}
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

			if(dragForm != null)
			{
				dragForm.Location = Subtract(PointToScreen(e.Location), dragPoint);
				int index;
				Rectangle rect;
				IDETabControl control;
				var newPosition = GetTabHover(e.Location, out control, out index, out rect);
				if (newPosition != TabPosition.None)
				{
					if(newPosition != lastPosition)
					{
						var form = control.FindForm();
						var box = OutlineBox.Get(form);
						box.Rectangle = new Rectangle(Add(rect.Location,
							form.PointToClient(control.PointToScreen(default(Point)))),
							rect.Size);
						box.BringToFront();
						box.Visible = true;
					}
				}
				lastPosition = newPosition;
				if (lastControl != null && control != lastControl
					&& lastControl.FindForm() != null)
				{
					OutlineBox.Get(lastControl.FindForm()).Visible = false;
				}
				lastControl = control;
				return;
			}

			// Handles the close button
			if (ImageList != null)
				for (int i = 0; i < TabCount; i++)
				{
					var rect = GetTabRect(i);
					rect = new Rectangle(rect.X + Margin.Left, rect.Y + Margin.Top,
						ImageList.ImageSize.Width, ImageList.ImageSize.Height);
					int newImage = rect.Contains(e.Location) ? 0 : 1;
					var tab = TabPages[i];
					if (tab.ImageIndex != newImage)
						tab.ImageIndex = newImage;
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
					var tab = TabPages[i] as IDETab;
					if (tab != null && tab.ImageIndex == 0)
					{
						tab.ImageIndex = 1;
						tab.Remove();
					}
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

	public interface IClosable
	{
		bool TryClose();
	}


}
