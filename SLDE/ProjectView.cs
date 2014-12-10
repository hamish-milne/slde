using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SLDE
{
	public partial class ProjectView : UserControl
	{
		public ProjectView()
		{
			InitializeComponent();
		}

		string root;

		public string Root
		{
			get
			{
				return root;
			}
			set
			{
				try
				{
					if (!Directory.Exists(value))
						throw new Exception("Directory \"" + value + "\" does not exist");
					treeView.Nodes.Clear();
					if (!String.IsNullOrEmpty(value))
					{
						var newNode = new FileNode(Path.GetFileName(value), value);
						newNode.Nodes.Add(new TreeNode());
						treeView.Nodes.Add(newNode);
					}
					root = value;
				} catch(Exception e)
				{
					Utility.ShowError(e.Message);
				}
			}
		}

		public event OpenFileEventHandler OpenFile;

		protected virtual void OnOpenFile(string path)
		{
			if (OpenFile != null)
				OpenFile(this, new OpenFileEventArgs(path));
		}

		protected int GetImageIndex(string file)
		{
			return 1;
		}

		private void treeView_DoubleClick(object sender, EventArgs e)
		{
			var node = ((TreeView)sender).SelectedNode as FileNode;
			if(node != null && node.IsFile)
				OnOpenFile(node.FileName);
		}

		private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			var node = e.Node as FileNode;
			if(node != null && !node.IsFile)
			{
				e.Node.Nodes.Clear();
				try
				{
					var files = Directory.GetFileSystemEntries(node.FileName);
					for(int i = 0; i < files.Length; i++)
					{
						var f = files[i];
						var newNode = new FileNode(Path.GetFileName(f), f);
						if (Directory.Exists(f))
							newNode.Nodes.Add(new TreeNode());
						else
							newNode.ImageIndex = GetImageIndex(f);
						e.Node.Nodes.Add(newNode);
					}
				} catch(Exception ex)
				{
					Utility.ShowError(ex.Message);
					e.Cancel = true;
				}
			}
		}

		private void treeView_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			var node = e.Node as FileNode;
			if (node != null)
			{
				e.Node.Nodes.Clear();
				e.Node.Nodes.Add(new TreeNode());
			}
		}

		private void ProjectView_ParentChanged(object sender, EventArgs e)
		{
			var tab = Parent as TabPage;
		}
	}

	public class FileNode : TreeNode
	{
		string fileName;

		public virtual string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		public virtual bool IsFile
		{
			get { return ImageIndex > 0; }
		}

		public FileNode(string text, string fileName)
			: base(text)
		{
			FileName = fileName;
		}
	}

	public delegate void OpenFileEventHandler(object sender, OpenFileEventArgs e);

	public class OpenFileEventArgs : EventArgs
	{
		public string File;
		public OpenFileEventArgs(string file)
		{
			File = file;
		}
	}
}
