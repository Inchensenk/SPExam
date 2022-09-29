using System.IO;

namespace SPExam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Scan();
        }
        public void Scan()
        {
            mainTreeView.Nodes.Clear();

            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.Name != @"C:\")
                    {
                        TreeNode driveNode = new TreeNode(drive.Name);
                        driveNode.Tag = drive;
                        GetChildNode(driveNode, drive.Name);

                        UpdateTreeView(driveNode);
                    }
                }
            }
            catch (Exception) { }
        }

        private void UpdateTreeView(TreeNode driveNode)
        {
            if (mainTreeView.InvokeRequired)
                mainTreeView.Invoke(new Action<TreeNode>(UpdateTreeView), driveNode);
            else
                mainTreeView.Nodes.Add(driveNode);
        }

        private void GetChildNode(TreeNode driveNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                if (dirs.Length == 0 && files.Length == 0) return;


                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new TreeNode();
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);
                    dirNode.Tag = dir;

                    GetChildNode(dirNode, dir);
                    driveNode.Nodes.Add(dirNode);
                }

                foreach (string file in files)
                {
                    TreeNode fileNode = new TreeNode();
                    fileNode.Text = file.Remove(0, file.LastIndexOf("\\") + 1);
                    fileNode.Tag = file;
                                        
                    driveNode.Nodes.Add(fileNode);
                }

            }
            catch (Exception) { }
        }
    }
}