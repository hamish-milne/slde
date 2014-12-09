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

		[Browsable(true)]
		public string Root
		{
			get
			{
				return root;
			}
			set
			{
				root = value;
				Update();
			}
		}

		public event OpenFileEventHandler OpenFile;

		protected virtual void OnOpenFile(string path)
		{
			if (OpenFile != null)
				OpenFile(this, new OpenFileEventArgs(path));
		}

		public virtual void UpdateTree()
		{
			treeView.Nodes.Clear();
			if(String.IsNullOrEmpty(Root))
				return;
			try
			{
				if(!Directory.Exists(Root))
					throw new Exception("Directory \"" + Root + "\" does not exist");
				var node = new TreeNode(Path.GetFileName(Root));
				treeView.Nodes.Add(node);
				AddRecursive(node, Root);
			} catch(Exception e)
			{
				Utility.ShowError(e.Message);
				root = "";
			}
		}

		protected void AddRecursive(TreeNode node, string directory)
		{
			var files = Directory.GetFileSystemEntries(directory);
			for (int i = 0; i < files.Length; i++)
			{
				var f = files[i];
				var newNode = new TreeNode(Path.GetFileName(f));
				if (Directory.Exists(f))
					AddRecursive(newNode, f);
				else
					newNode.ImageIndex = GetImageIndex(f);
				node.Nodes.Add(newNode);
			}
		}

		protected int GetImageIndex(string file)
		{
			return 1;
		}

		private void treeView_DoubleClick(object sender, EventArgs e)
		{
			var tree = (TreeView)sender;
			if(tree.SelectedNode.ImageIndex != 0)
			{
				var node = tree.SelectedNode;
				string path = "";
				do
				{
					path = "/" + node.Text + path;
					node = node.Parent as TreeNode;
				} while (node != null);
				OnOpenFile(Path.GetDirectoryName(Root) + "/" + path);
			}
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
