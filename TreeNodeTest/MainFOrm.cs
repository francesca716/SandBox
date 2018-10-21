using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeNodeTest
{
    public partial class MainForm : Form
    {
        #region Constructor
        public MainForm()
        {
            InitializeComponent();

            foreach (var drive in Environment.GetLogicalDrives())
            {
                TreeNode treeNode = new TreeNode(drive);
                treeNode.Nodes.Add(new TreeNode());
                filesTreeView.Nodes.Add(treeNode);
            }
            initializeContentListView(Environment.GetLogicalDrives().First());
        }
        #endregion

        #region Methods
        private void initializeContentListView(string filePath)
        {
            this.contentListView.View = View.Details;
            this.contentListView.Clear();
            this.contentListView.Columns.Add("名前");
            this.contentListView.Columns.Add("更新日時");
            this.contentListView.Columns.Add("サイズ");

            this.contentListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            try
            {
                if(Directory.Exists(filePath))
                {
                    //フォルダ一覧
                    DirectoryInfo directoryList = new DirectoryInfo(filePath);

                    foreach (var directory in directoryList.GetDirectories())
                    {
                        ListViewItem item = new ListViewItem(directory.Name);
                        item.SubItems.Add(String.Format("{0:yyyy/MM/dd HH:mm:ss}", directory.LastAccessTime));
                        item.SubItems.Add("");
                        contentListView.Items.Add(item);
                    }

                    // ファイル一覧
                    List<String> fileList = Directory.GetFiles(filePath).ToList<String>();

                    foreach (var file in fileList)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        ListViewItem item = new ListViewItem(fileInfo.Name);
                        item.SubItems.Add(String.Format("{0:yyyy/MM/dd HH:mm:ss}", fileInfo.LastAccessTime));
                        item.SubItems.Add(getFileSize(fileInfo.Length));
                    }

                }

            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message, "選択エラー");
            }
        }

        /// <summary>
        /// ファイルサイズを単位付きに変換して返す
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        private string getFileSize(long fileSize)
        {
            String ret = fileSize + " byte";
            if (fileSize > (1024f * 1024f * 1024f))
            {
                ret = Math.Round((fileSize / 1024f / 1024f / 1024f), 2).ToString() + " GB";
            }
            else if (fileSize > (1024f * 1024f))
            {
                ret = Math.Round((fileSize / 1024f / 1024f), 2).ToString() + " MB";
            }
            else if (fileSize > 1024f)
            {
                ret = Math.Round((fileSize / 1024f)).ToString() + " KB";
            }

            return ret;
        }
        /// <summary>
        /// ツリービューの指定されたディレクトリパスの項目を選択状態にします.
        /// </summary>
        /// <param name="dirPath"></param>
        private void selectTreeViewItem(TreeView treeView, string dirPath)
        {
            // ツリービューのノードを走査
            foreach (TreeNode node in treeView.Nodes)
            {
                selectTreeViewItem(node, dirPath);
            }
        }

        /// <summary>
        /// ツリービューの指定されたディレクトリパスの項目を選択状態にします（再帰処理用）.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dirPath"></param>
        private void selectTreeViewItem(TreeNode node, string dirPath)
        {
            foreach (TreeNode treeNode in node.Nodes)
            {
                FileInfo fi = new FileInfo(treeNode.FullPath);
                // リストビューで表示されたディレクトリ名と一致する場合
                if (dirPath == fi.FullName)
                {
                    this.filesTreeView.Focus();
                    this.filesTreeView.SelectedNode = treeNode;
                    break;
                }
                selectTreeViewItem(treeNode, dirPath);
            }
        }

        #endregion

        #region Events

        private void filesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            initializeContentListView(e.Node.FullPath);
        }

        private void filesTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            TreeNode childNode;
            String path = node.FullPath;
            node.Nodes.Clear();

            try
            {
                if(Directory.Exists(path))
                {
                    DirectoryInfo directoryList = new DirectoryInfo(path);

                    foreach (var directory in directoryList.GetDirectories())
                    {
                        childNode = new TreeNode(directory.Name);
                        childNode.Nodes.Add(new TreeNode());
                        node.Nodes.Add(childNode);
                    }
                    List<String> fileList = Directory.GetFiles(path).ToList();
                    foreach (var fi in fileList)
                    {
                        childNode = new TreeNode(fi);
                        childNode.Nodes.Add(new TreeNode());
                        node.Nodes.Add(childNode);
                    }
                }             
            }
            catch (IOException ie)
            {
                MessageBox.Show(ie.Message, "選択エラー");
            }
        }
        #endregion
    }
}

