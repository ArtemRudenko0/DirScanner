using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
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
        private Dictionary<string, long> folderSizes = new Dictionary<string, long>();
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

        }
        private void LoadFiles(DirectoryInfo dir, int count)
        {
            listView1.Items.Clear();
            listView1.ListViewItemSorter = null;
            listViewItemComparer.Order = SortOrder.None;

            if (count > 0)
            {
                ListViewItem back = new ListViewItem("...", 0);
                listView1.Items.Add(back);
            }

            string[] subdirectories = Directory.GetDirectories(dir.FullName);
            foreach (string subdirectory in subdirectories)
            {
                if (CanAccessFolder(subdirectory))
                {
                    DirectoryInfo subDir = new DirectoryInfo(subdirectory);
                    ListViewItem item = new ListViewItem(subDir.Name, 0);
                    //long folderSize = CalculateTotalSize(subdirectory);
                    long folderSize = folderSizes[subDir.FullName];
                    item.SubItems.Add(setMeasurement(folderSize));

                    listView1.Items.Add(item);
                }
            }

            string[] files = Directory.GetFiles(dir.FullName, "*.*");
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (CanAccessFile(file))
                    {
                        FileInfo fi = new FileInfo(file);
                        ListViewItem item1 = new ListViewItem(fi.Name, 1);

                        string key = fi.FullName;
                        if (!imageList.Images.ContainsKey(key))
                        {
                            Icon iconForFile = Icon.ExtractAssociatedIcon(fi.FullName);
                            iconForFile = RemoveBlackBackground(iconForFile);
                            imageList.Images.Add(key, iconForFile);
                        }

                        item1.ImageKey = key;
                        item1.SubItems.Add(setMeasurement(fi.Length));
                        listView1.Items.Add(item1);
                    }
                }
            }
        }

        private async void buttonScan_Click(object sender, EventArgs e)
        {

            buttonScan.Enabled = false;
            DirectoryPath = textBoxFolder.Text;
            OccurrenceCounter = 0;
            DirectoryInfo SubDir = new DirectoryInfo(DirectoryPath);
            folderSizes.Clear();
            if (Directory.Exists(DirectoryPath))
            {

                LabelResult.Text = " ";
                progressBarForm = new ProgressBarForm(foldersCount);

                progressBarForm.Width = listView1.Width;
                progressBarForm.Height = listView1.Height;
                progressBarForm.CenterChildForm(progressBarForm, this, listView1.Location.X, listView1.Location.Y);
                progressBarForm.Show(this);

                // this.Enabled = false;
                buttonChoose.Enabled = false;
                textBoxFolder.Enabled = false;
                Application.DoEvents(); // Обновление интерфейса
               
                await Task.Run(() =>
                {
                    foldersCount = CountTotalFolders(DirectoryPath);
                    progressBarForm.TotalFolders = foldersCount;
                    long totalSize = CalculateTotalSize(DirectoryPath);

                    // Обновляем интерфейс в основном потоке
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
            notifyIcon.ShowBalloonTip(3000); // Показать всплывающее уведомление на 2 секунды

            // Задержка для отображения всплывающего уведомления
            System.Threading.Thread.Sleep(3000);

            notifyIcon.Dispose(); // Освобождение ресурсов
        }

        private long CalculateTotalSize(string folderPath)
        {

            if (folderSizes.ContainsKey(folderPath))
            {
                return folderSizes[folderPath];
            }
            long totalSize = 0;
            int processedFolders = 0;

            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                totalSize += fileInfo.Length;

            }
            string[] subfolders;
            try
            {
                subfolders = Directory.GetDirectories(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                // Обработка ошибки доступа к папке
                return 0;
            }
            foreach (string subfolder in subfolders)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(subfolder);
                if (CanAccessFolder(directoryInfo.FullName))
                {

                    totalSize += CalculateTotalSize(subfolder); // Рекурсивный вызов
                }
                processedFolders++;
                progressBarForm.UpdateProgress();
            }
            folderSizes[folderPath] = totalSize;

            return totalSize;
        }
        private int CountTotalFolders(string folderPath)
        {
            int folderCount = 0;
            CountFoldersRecursively(folderPath, ref folderCount);
            return folderCount;
        }
        private void CountFoldersRecursively(string folderPath, ref int folderCount)
        {
            if (CanAccessFolder(folderPath))
            {
                folderCount++;

                try
                {
                    foreach (string subfolder in Directory.GetDirectories(folderPath))
                    {
                        CountFoldersRecursively(subfolder, ref folderCount);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // Обработка ошибки доступа к папке
                }
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
            Bitmap iconBitmap = originalIcon.ToBitmap();
            iconBitmap.MakeTransparent(Color.Black); // Удаляем черный фон
            return Icon.FromHandle(iconBitmap.GetHicon());
        }
        private bool CanAccessFolder(string folderPath) //Перевірка доступу до папки
        {
            try
            {
                Directory.GetDirectories(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                // Обработка ошибки доступа к папке
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
                    fileInfo.Refresh(); // Обновляем информацию о файле
                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    // Обработка ошибки доступа к файлу
                    return false;
                }
            }
            else return false;
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
            ListViewItem selectedItem = listView1.FocusedItem;


            if (selectedItem.Name != null)
            {
                if (selectedItem.Text.ToString() == "...")
                {

                    DirectoryPath = Path.GetDirectoryName(DirectoryPath);
                    OccurrenceCounter--;
                    DirectoryInfo Sub_Di = new DirectoryInfo(DirectoryPath);

                    LoadFiles(Sub_Di, OccurrenceCounter);
                    //listView1.Sort();
                }
                else
                {
                    DirectoryPath += "\\" + selectedItem.Text.ToString();

                    // MessageBox.Show(DirectoryPath);
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

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listView1.ListViewItemSorter = null;

            listViewItemComparer.directoryPath = DirectoryPath;
            // Устанавливаем столбец, по которому нужно сортировать

            if (e.Column != sortedColumn)
            {
                sortedColumn = e.Column;
                listViewItemComparer.Order = SortOrder.Descending;
            }
            else
            {
                // Изменяем порядок сортировки на противоположный, если столбец уже был выбран для сортировки
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
            // Вызываем метод для сортировки
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
    }

}