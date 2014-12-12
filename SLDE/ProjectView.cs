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
	/// <summary>
	/// The project view window. Shows a file tree from a given root directory
	/// </summary>
	public partial class ProjectView : UserControl, IClosable
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public ProjectView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// No action
		/// </summary>
		/// <returns><c>true</c></returns>
		public virtual bool TryClose()
		{
			return true;
		}

		string root;

		/// <summary>
		/// Gets and sets the root directory, updating the tree as needed
		/// </summary>
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

		/// <summary>
		/// Called when a file entry is double-clicked
		/// </summary>
		public event OpenFileEventHandler OpenFile;

		/// <summary>
		/// Called when a file entry is double-clicked
		/// </summary>
		/// <param name="path"></param>
		protected virtual void OnOpenFile(string path)
		{
			if (OpenFile != null)
				OpenFile(this, new OpenFileEventArgs(path));
		}

		/// <summary>
		/// Gets the icon index for the given file
		/// </summary>
		/// <param name="file">The file name</param>
		/// <returns>Right now, <c>1</c></returns>
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
						newNode.SelectedImageIndex = newNode.ImageIndex;
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

	/// <summary>
	/// A TreeNode that also stores a file name
	/// </summary>
	[Serializable]
	public class FileNode : TreeNode
	{
		string fileName;

		/// <summary>
		/// Gets and sets the stored file name
		/// </summary>
		public virtual string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		/// <summary>
		/// Gets whether the node represents a file
		/// </summary>
		public virtual bool IsFile
		{
			get { return ImageIndex > 0; }
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="text"></param>
		/// <param name="fileName"></param>
		public FileNode(string text, string fileName)
			: base(text)
		{
			FileName = fileName;
		}
	}

	/// <summary>
	/// The event type for an open file event in <see cref="ProjectView"/>
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void OpenFileEventHandler(object sender, OpenFileEventArgs e);

	/// <summary>
	/// The EventArgs for an open file event in <see cref="ProjectView"/>
	/// </summary>
	public class OpenFileEventArgs : EventArgs
	{
		/// <summary>
		/// The opened file
		/// </summary>
		public string File;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="file">The opened file</param>
		public OpenFileEventArgs(string file)
		{
			File = file;
		}
	}
}
