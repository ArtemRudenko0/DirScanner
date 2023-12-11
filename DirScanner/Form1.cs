using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DirScanner
{
    public partial class Form1 : Form
    {
        ProgressBarForm progressBarForm;

        private string DirectoryPath;
        private int occurrenceCounter = 0;
        private ListViewItemComparer listViewItemComparer;
        ImageList imageList;
        private int sortedColumn = -1;
        private int foldersCount = 0;
        class FolderElements
        {
            public long folders;
            public long files;
            public FolderElements()
            {

            }
            public FolderElements(long folders, long files)
            {
                this.files = files;
                this.folders = folders;
            }
        }
        private ConcurrentDictionary<string, long> folderSizes = new ConcurrentDictionary<string, long>();
        private ConcurrentDictionary<string, FolderElements> folderElements = new ConcurrentDictionary<string, FolderElements>();
        private List<double> columnWidthRatios = new List<double>();


        public int OccurrenceCounter
        {
            get { return occurrenceCounter; }
            set { occurrenceCounter = value; }
        }
        private FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        public Form1()
        {
            InitializeComponent();

            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            imageList = new ImageList();
            imageList.Images.Add(Image.FromFile(@"img\Folder.png"));
            imageList.Images.Add(Image.FromFile(@"img\File.png"));

            listView1.SmallImageList = imageList;
            listView1.View = View.Details;

            listViewItemComparer = new ListViewItemComparer();
            listView1.ListViewItemSorter = listViewItemComparer;
            LabelResult.Text = "Виберіть папку, яку треба сканувати...";
            progressBarForm = new ProgressBarForm();
            buttonScan.Enabled = false;
            SaveColumnWidthRatios();
        }
        private async void LoadFiles(DirectoryInfo dir, int count)
        {

            progressBar2.Visible = true;
            listView1.Items.Clear();
            listView1.ListViewItemSorter = null;
            listViewItemComparer.Order = SortOrder.None;
            List<Task<int>> tasks = new List<Task<int>>();
            if (count > 0)
            {
                ListViewItem back = new ListViewItem("...", 0);
                listView1.Items.Add(back);
            }

            string[] subdirectories = Directory.GetDirectories(dir.FullName);
            progressBar2.Value = 0;
            progressBar2.Maximum = subdirectories.Length;
            foreach (string subdirectory in subdirectories)
            {
                if (CanAccessFolder(subdirectory) && !IsSystemFolder(subdirectory))
                {
                    DirectoryInfo subDir = new DirectoryInfo(subdirectory);
                    ListViewItem item = new ListViewItem(subDir.Name, 0);
                    
                    long folderSize = folderSizes[subDir.FullName];
                    item.SubItems.Add(setMeasurement(folderSize));

                   if (folderSizes[subDir.FullName] != 0)
                    {

                        double ratio = (double)folderSizes[subDir.FullName] / folderSizes[subDir.Parent.FullName] * 100;
                        if (ratio >= 95 || ratio <= 1)
                        {
                            item.SubItems.Add(ratio.ToString("0.0000") + " %");
                        }

                        else item.SubItems.Add(ratio.ToString("0.00") + " %");
                    }
                    else item.SubItems.Add("0.00" + " %");
                    item.SubItems.Add("");

                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add(subDir.CreationTime.ToString());
                    listView1.Items.Add(item);





                }
            }

            string[] files = Directory.GetFiles(dir.FullName, "*.*");

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (CanAccessFile(file) && !IsSystemFile(file))
                    {

                        FileInfo fi = new FileInfo(file);
                        ListViewItem item1 = new ListViewItem(fi.Name, 1);


                        string key = fi.FullName;
                        Icon iconForFile = null;
                        Icon tempIcon = null;
                        if (!imageList.Images.ContainsKey(key))
                        {
                            iconForFile = Icon.ExtractAssociatedIcon(file);
                            tempIcon = iconForFile;

                            tempIcon = RemoveBlackBackground(tempIcon);
                            try
                            {
                                if (tempIcon != null)
                                {
                                    imageList.Images.Add(key, tempIcon);
                                    item1.ImageKey = key;
                                }


                            }
                            catch (Exception)
                            {



                            }
                            

                        }

                        
                        item1.SubItems.Add(setMeasurement(fi.Length));

                      
                       if (folderSizes.ContainsKey(fi.Directory.FullName))
                        {
                            double ratio = (double)fi.Length / folderSizes[fi.Directory.FullName] * 100;
                            if (ratio >= 95 || ratio <= 1)
                            {
                                item1.SubItems.Add(ratio.ToString("0.0000") + " %");
                            }
                            else item1.SubItems.Add(ratio.ToString("0.00") + " %");
                        }
                        else
                        {

                        }

                        item1.SubItems.Add("");

                        item1.SubItems.Add("");
                        item1.SubItems.Add("");
                        item1.SubItems.Add(fi.CreationTime.ToString());
                        listView1.Items.Add(item1);
                    }
                }
            }

            await UpdateFolderCountsAsync();
            progressBar2.Visible = false;
        }

        private async void buttonScan_Click(object sender, EventArgs e)
        {


            DirectoryPath = textBoxFolder.Text;
            OccurrenceCounter = 0;
            DirectoryInfo SubDir = new DirectoryInfo(DirectoryPath);
            folderSizes.Clear();
            if (Directory.Exists(DirectoryPath))
            {
                buttonScan.Enabled = false;
                LabelResult.Text = " ";
                progressBarForm = new ProgressBarForm(foldersCount);

                progressBarForm.Width = listView1.Width;
                progressBarForm.Height = listView1.Height;
                progressBarForm.CenterChildForm(progressBarForm, this, listView1.Location.X, listView1.Location.Y);
                progressBarForm.Show(this);

               
                buttonChoose.Enabled = false;
                textBoxFolder.Enabled = false;
                Application.DoEvents(); 

                await Task.Run(async () =>
                {
                    foldersCount = await CountTotalFoldersAsync(DirectoryPath);
                    progressBarForm.TotalFolders = foldersCount;
                    long totalSize = await CalculateTotalSizeAsync(DirectoryPath);
                    folderSizes.TryAdd(DirectoryPath, totalSize);
                  
                    Invoke(new Action(() =>
                    {
                        LoadFiles(SubDir, OccurrenceCounter);
                        LabelResult.Text = "Загальний розмір файлів: " + setMeasurement(totalSize) + " в "
                            + foldersCount.ToString() + " папках";

                        textBoxFolder.Select(0, 0);


                        progressBarForm.Close();

                    }));
                });
                if (this.isNotActive())
                {
                    ShowNotification("Сканування завершено", "Сканування вашої папки завершено. Перейдіть до програми, щоб побачити результат.");
                }
                buttonChoose.Enabled = true;
                textBoxFolder.Enabled = true;
            }


        }

        private bool isNotActive()
        {
            [DllImport("user32.dll")]
            static extern IntPtr GetForegroundWindow();
            IntPtr foregroundWindowHandle = GetForegroundWindow();
            IntPtr myWindowHandle = Process.GetCurrentProcess().MainWindowHandle;

            if (foregroundWindowHandle == myWindowHandle)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void ShowNotification(string title, string message)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;

            notifyIcon.Icon = SystemIcons.Information;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(3000); 

           
            System.Threading.Thread.Sleep(3000);

            notifyIcon.Dispose();
        }
        private async Task<long> CalculateTotalSizeAsync(string folderPath)
        {
            if (folderSizes.TryGetValue(folderPath, out long size))
            {
                return size;
            }

            long totalSize = 0;
            int processedFolders = 0;


            string[] files;

            try
            {
                files = Directory.GetFiles(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                
                files = new string[0]; 
            }

            foreach (string file in files)
            {
                try
                {
                    if (CanAccessFile(file) && !IsSystemFile(file))
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        totalSize += fileInfo.Length;

                        folderSizes.TryAdd(fileInfo.Name, file.Length);

                    }
                }
                catch (UnauthorizedAccessException)
                {

                }
            }

            string[] subfolders;
            try
            {
                subfolders = Directory.GetDirectories(folderPath);

            }
            catch (UnauthorizedAccessException)
            {
               
                return 0;
            }
            List<Task<long>> tasks = new List<Task<long>>();
            foreach (string subfolder in subfolders)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(subfolder);
                if (CanAccessFolder(directoryInfo.FullName))
                {

                    tasks.Add(Task.Run(() => CalculateTotalSizeAsync(subfolder)));
                
                }
                processedFolders++;
                progressBarForm.UpdateProgress();

            }

          
            await Task.WhenAll(tasks);
           
            foreach (var task in tasks)
            {
                totalSize += await task;
            }
          
            folderSizes.TryAdd(folderPath, totalSize);
            return totalSize;
        }
        private async Task<int> CountTotalFoldersAsync(string folderPath)
        {
            return await CountFoldersRecursivelyAsync(folderPath);
        }
        private async Task<int> CountFoldersRecursivelyAsync(string folderPath)
        {
            int folderCount = 0;


            string[] subfolders;
            try
            {
                subfolders = Directory.GetDirectories(folderPath);
                folderCount = subfolders.Length;
            }
            catch (UnauthorizedAccessException)
            {
                return 0;
            }
            List<Task<int>> tasks = new List<Task<int>>();

            foreach (string subfolder in subfolders)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(subfolder);
                if (CanAccessFolder(directoryInfo.FullName))
                {
                    tasks.Add(Task.Run(() => (CountFoldersRecursivelyAsync(subfolder))));
                }
            }

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                folderCount += await task;
            }


            return folderCount;
        }
        private async Task<int> CountFilesRecursivelyAsync(string folderPath)
        {
            int filesCount = 0;

            string[] subfolders;
            string[] files;
            try
            {
                subfolders = Directory.GetDirectories(folderPath);
                files = Directory.GetFiles(folderPath, "*.*");
                filesCount = files.Length;
            }
            catch (UnauthorizedAccessException)
            {
                return 0;
            }
            List<Task<int>> tasks = new List<Task<int>>();

            foreach (string subfolder in subfolders)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(subfolder);

                if (CanAccessFolder(directoryInfo.FullName))
                {
                    tasks.Add(Task.Run(() => (CountFilesRecursivelyAsync(subfolder))));
                }
            }

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                filesCount += await task;
            }


            return filesCount;
        }
     
        private async Task UpdateFolderCountsAsync()
        {
            long totalFolders = 0, totalFiles = 0, totalElements = 0;
            progressBar2.Maximum = listView1.Items.Count;
            foreach (ListViewItem item in listView1.Items)
            {

                if (item.Text == "...") { }
                else
                { 
                    string folderPath = Path.Combine(DirectoryPath, item.Text);
                    if (Directory.Exists(folderPath))
                    {
                        if (folderElements.TryGetValue(folderPath, out FolderElements fel))
                        {
                            totalFolders = fel.folders;
                            totalFiles = fel.files;
                            totalElements = fel.files + fel.folders;
                        }
                        else
                        {
                            totalFolders = await CountFoldersRecursivelyAsync(folderPath);
                            totalFiles = await CountFilesRecursivelyAsync(folderPath);
                            totalElements = totalFiles + totalFolders;
                            if (totalFolders >= 10000 || totalFiles >= 10000)
                            {
                                FolderElements element = new FolderElements(totalFolders, totalFiles);
                                folderElements.TryAdd(folderPath, element);
                            }

                        }
                        // Найденная папка
                        ListViewItem foundItem = null;
                        foreach (ListViewItem listViewItem in listView1.Items)
                        {
                            if (listViewItem.Text == item.Text)
                            {
                                foundItem = listViewItem;
                                break;
                            }
                        }

                        if (foundItem != null)
                        {

                            // Обновление соответствующего столбца в элементе ListViewItem
                            foundItem.SubItems[3].Text = totalElements.ToString();
                            foundItem.SubItems[4].Text = totalFolders.ToString();
                            foundItem.SubItems[5].Text = totalFiles.ToString();
                        }
                    }
                }
                progressBar2.Value++;
            }

        }
        private string setMeasurement(long lenght)
        {
            string newMeasurement = " ";
            int iteration = 0;
            for (int i = 3; i > 0; i--)
            {
                double temp = (double)lenght / Math.Pow(1024, i);
                if (temp > 1)
                {
                    newMeasurement = temp.ToString("0.00");
                    iteration = i;
                    break;
                }
                else
                {
                    newMeasurement = temp.ToString("0.00");
                    iteration = i;
                }

            }
            switch (iteration)
            {
                case 1:
                    newMeasurement += " Kb";
                    break;
                case 2:
                    newMeasurement += " Mb";
                    break;
                case 3:
                    newMeasurement += " Gb";
                    break;
            }
            return newMeasurement;
        }

        private Icon RemoveBlackBackground(Icon originalIcon)
        {


            try
            {

                Bitmap bitmap = new Bitmap(originalIcon.ToBitmap());


                Bitmap transparentBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                transparentBitmap.MakeTransparent(Color.Black);
                Graphics g = Graphics.FromImage(transparentBitmap);
                g.DrawImage(bitmap, Point.Empty);

                Icon transparentIcon = Icon.FromHandle(transparentBitmap.GetHicon());


                g.Dispose();
                bitmap.Dispose();
                transparentBitmap.Dispose();

                return transparentIcon;
            }
            catch (Exception)
            {
              
                return originalIcon;
            }
        }

        private bool CanAccessFolder(string folderPath) 
        {
            try
            {
                Directory.GetDirectories(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
              
                return false;
            }
        }


        private bool CanAccessFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    fileInfo.Refresh(); 
                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                   
                    return false;
                }
            }
            else return false;
        }
        private bool IsSystemFolder(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                return (directoryInfo.Attributes & FileAttributes.System) != 0;
            }
            catch (UnauthorizedAccessException)
            {
                return false; 
            }
        }
        private bool IsSystemFile(string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                return (fileInfo.Attributes & FileAttributes.System) != 0;
            }
            catch (UnauthorizedAccessException)
            {
                
                return false;
            }
        }
        private void buttonChoose_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolderPath = folderBrowserDialog.SelectedPath;

                textBoxFolder.Text = selectedFolderPath;
                if (DirectoryPath != textBoxFolder.Text)
                {
                    buttonScan.Enabled = true;

                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!progressBar2.Visible)
            {
                ListViewItem selectedItem = listView1.FocusedItem;


                if (selectedItem.Name != null)
                {
                    if (selectedItem.Text.ToString() == "...")
                    {

                        DirectoryPath = Path.GetDirectoryName(DirectoryPath);
                        OccurrenceCounter--;
                        DirectoryInfo Sub_Di = new DirectoryInfo(DirectoryPath);

                        LoadFiles(Sub_Di, OccurrenceCounter);

                    }
                    else
                    {
                        DirectoryPath += "\\" + selectedItem.Text.ToString();


                        if (Directory.Exists(DirectoryPath))
                        {

                            OccurrenceCounter++;
                            DirectoryInfo Sub_Di = new DirectoryInfo(DirectoryPath);

                            LoadFiles(Sub_Di, OccurrenceCounter);

                        }
                        else { DirectoryPath = Path.GetDirectoryName(DirectoryPath); }
                    }

                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listView1.ListViewItemSorter = null;

            listViewItemComparer.directoryPath = DirectoryPath;
          

            if (e.Column != sortedColumn)
            {
                sortedColumn = e.Column;
                listViewItemComparer.Order = SortOrder.Descending;
            }
            else
            {
                
                if (listViewItemComparer.Order == SortOrder.Descending)
                {
                    listViewItemComparer.Order = SortOrder.Ascending;
                }
                else
                {
                    listViewItemComparer.Order = SortOrder.Descending;
                }
            }
            listView1.ListViewItemSorter = new ListViewItemComparer(e.Column, listViewItemComparer.Order);
           
            listView1.Sort();
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            if (progressBarForm != null)
            {
                progressBarForm.CenterChildForm(progressBarForm, this, listView1.Location.X, listView1.Location.Y);
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            progressBarForm.Width = listView1.Width;
            progressBarForm.Height = listView1.Height;
            progressBarForm.CenterChildForm(progressBarForm, this, listView1.Location.X, listView1.Location.Y);
        }

        private void textBoxFolder_TextChanged(object sender, EventArgs e)
        {
            buttonScan.Enabled = true;
        }
        private void SaveColumnWidthRatios() 
        {
            int totalColumnWidth = 0;
            foreach (ColumnHeader column in listView1.Columns)
            {
                totalColumnWidth += column.Width;
            }

            columnWidthRatios.Clear();
            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                double ratio = (double)listView1.Columns[i].Width / totalColumnWidth;
                columnWidthRatios.Add(ratio);
            }
        }
        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            if (listView1.Columns.Count == columnWidthRatios.Count)
            {
                int totalWidth = listView1.ClientSize.Width;
                for (int i = 0; i < listView1.Columns.Count - 1; i++)
                {
                    int newColumnWidth = (int)(totalWidth * columnWidthRatios[i]);
                    listView1.Columns[i].Width = newColumnWidth;
                }
            }
            else
            {

            }
        }


    }

}