using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace SPExam
{
    public partial class Form1 : Form
    {
        List<FileStatistics> totalStatistics = new();
        List<string> forbiddenWords = new List<string>();
        Dictionary<string, int> badWords;

        private int overallFileAmount = 0;
        
        public Form1()
        {
            InitializeComponent();
            badWords = new Dictionary<string, int>();
            forbiddenWords.AddRange(new string[4] { "java", "azaza", "javascript", "hello"});
            forbiddenWords.ForEach(word => badWords.Add(word, 0));
            Scan();
            progressBar1.Maximum = overallFileAmount;
            progressBar1.Step = 1;
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

                overallFileAmount += files.Length;


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

        private void Search(DirectoryInfo di)
        {

            try
            {
                DirectoryInfo[] directories = di.GetDirectories();

                foreach (DirectoryInfo dir in directories)
                    Search(dir);

                if (directories.Count() == 0 || di.GetFiles().Count() > 0)
                {
                    //if ((di.GetFiles().Count() / 2 * 100) < 100)
                    //    UpdateProgressBar(di.GetFiles().Count() / 2 * 100);

                    //else
                    //    UpdateProgressBar(100);

                    FileInfo[] files = di.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        FileStatistics statistics = new();
                        statistics.FileInfo = file;
                        int countWordsFound = 0;
                        FileInfo fi = null;
                        bool isFound = false;
                        // читает файл по пути
                        string[] str = File.ReadAllLines(file.FullName);
                        

                        //UpdateLable(file.FullName);

                        foreach (string word in forbiddenWords)
                        {
                            isFound = false;
                            for (int i = 0; i < str.Count(); i++)
                            {
                                if (Regex.IsMatch(str[i], word, RegexOptions.IgnoreCase))
                                {
                                    badWords[word]++;
                                    str[i] = str[i].Replace(word, "*******");
                                    string[] wordsStr = str[i].Split(new char[] { ' ', '!', ',', '?' }, StringSplitOptions.RemoveEmptyEntries);

                                    //countWordsFound += wordsStr.Where(w => w == "*******").Count();

                                    statistics.Counter++;

                                    isFound = true;
                                }
                            }
                        }

                        if (isFound)
                        {
                            File.WriteAllLines($"../../../copied-files/{file.Name} - copy{file.Extension}", str, Encoding.UTF8);





                            //fi = new FileInfo(file.FullName);

                            //UpdateList(file.FullName);

                            //File.Copy(fi.FullName, pathFoundFiles + fi.Name, true);

                            //listStatistic.Add(new Statistic(word, file.FullName, (int)fi.Length, countWordsFound, countWordsFound));
                            //listAbridget.Add(new TopWords(word, countWordsFound));

                            //if (!Directory.Exists(pathFoundFiles + "newFiles\\"))
                            //    Directory.CreateDirectory(pathFoundFiles + "newFiles");

                            //using (FileStream fs = new FileStream(pathFoundFiles + "newFiles\\" + Path.GetFileNameWithoutExtension(file.Name) + DateTime.Now.Second + ".txt", FileMode.OpenOrCreate))
                            //{
                            //    byte[] b = Encoding.ASCII.GetBytes(FullTextInString(str));
                            //    fs.Write(b, 0, b.Length);
                            //}
                        }

                        //var topWords = badWords.Keys.OrderByDescending(item => item).;
                        //var topWords = badWords.Values.OrderByDescending(word => word).ToList();
                        this.Invoke(() =>
                        {
                            progressBar1.PerformStep();

                        });
                    }

                    var topWords = badWords.OrderBy(u => u.Value);

                    List<string> words = new List<string>();

                    foreach (var number in topWords)
                    {
                        words.Add(number.Key);

                        if (words.Count >= 5)
                            break;
                    }
                    this.Invoke(() =>
                    {
                        words.Reverse();
                        listBox1.DataSource = words;

                    });


                    return;

                }
            }
            catch (Exception) { }
        }

        private void OnSearchButtonClick(object sender, EventArgs e)
        {
            if (mainTreeView.SelectedNode != null)
            {

                //progressBarSearch.Value = 0;
                //listBoxFiles.Items.Clear();
                //btnShowStatistics.Visible = false;
                //progressBarSearch.Visible = true;
                //lblFiles.Visible = true;

                TreeNode tn = mainTreeView.SelectedNode;
                DirectoryInfo di = new DirectoryInfo(tn.FullPath);

                Task taskSearch = Task.Run(() =>
                {
                    //listStatistic = new List<Statistic>();
                    //listAbridget = new List<TopWords>();

                    Search(di);

                    //    if (listStatistic.Count > 0)
                    //        UpdateButton(true);

                    //    Task task = Task.Run(() =>
                    //    {
                    //        try
                    //        {
                    //            db.statisticClass.AddRange(listStatistic);
                    //            db.TopWords.AddRange(listAbridget);
                    //            db.SaveChanges();
                    //        }
                    //        catch (Exception) { }
                    //    });
                });
            }
        }
    }
}