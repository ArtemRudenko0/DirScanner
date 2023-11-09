using System;
using System.Collections;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DirScanner
{
    public partial class Form1 : Form
    {

        private string DirectoryPath;
        private int occurrenceCounter = 0;
        private ListViewItemComparer listViewItemComparer;
        ImageList imageList;
        private int sortedColumn = -1;
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
           
            listView1.Columns.Add("Ім'я", 200);
            listView1.Columns.Add("Розмір", 200);


        }

        private void LoadFiles(DirectoryInfo dir, int count)
        {
            
            

            listView1.Items.Clear();
            listView1.ListViewItemSorter = null;
            listViewItemComparer.Order = SortOrder.None;
            // listViewItemComparer.Order = SortOrder.Ascending;
            //listView1.Sort();
            if (count > 0)
            {

                ListViewItem back = new ListViewItem("...", 0);
                listView1.Items.Add(back);
            }
            string[] subdirectories = Directory.GetDirectories(dir.FullName);
            foreach (string subdirectory in subdirectories)
            {
                DirectoryInfo subDir = new DirectoryInfo(subdirectory);
                ListViewItem item = new ListViewItem(subDir.Name, 0);
                item.SubItems.Add(setMeasurement(CalculateTotalSize(DirectoryPath + "\\" + subDir.Name)));
                listView1.Items.Add(item);
            }
            string[] Files = Directory.GetFiles(dir.FullName, "*.*");
            if (Files.Length > 0)
            {

                foreach (string file in Files)
                {
                    FileInfo fi = new FileInfo(file);

                 
                    ListViewItem item1 = new ListViewItem(fi.Name,1);
                   

                    //Додавання іконки в масив, якщо такої не має, а потім встановлення до Item
                    if (!imageList.Images.ContainsKey(fi.Extension))
                    {
                        // If not, add the image to the image list.
                        Icon iconForFile = Icon.ExtractAssociatedIcon(fi.FullName);
                        imageList.Images.Add(fi.Extension, iconForFile);
                    }
                    item1.ImageKey = fi.Extension;


                    item1.SubItems.Add(setMeasurement(fi.Length));

                    listView1.Items.Add(item1);
                }
            }

        }

        private void buttonScan_Click(object sender, EventArgs e)
        {

            if (DirectoryPath != textBoxFolder.Text)
            {
                DirectoryPath = textBoxFolder.Text;
                OccurrenceCounter = 0;
                DirectoryInfo SubDir = new DirectoryInfo(DirectoryPath);
                LoadFiles(SubDir, OccurrenceCounter);
                if (Directory.Exists(DirectoryPath))
                {

                    long totalSize = CalculateTotalSize(DirectoryPath);

                    LabelResult.Text = "Загальний розмір файлів: " + setMeasurement(totalSize);
                }
                else
                {
                    LabelResult.Text = "Вказаної папки не існує.";
                }
            }
        }
        private long CalculateTotalSize(string folderPath)
        {

            long totalSize = 0;

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
            }

            return totalSize;
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
        private void buttonChoose_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolderPath = folderBrowserDialog.SelectedPath;
                textBoxFolder.Text = selectedFolderPath;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem selectedItem = listView1.FocusedItem;

            
            if (selectedItem.Name != null)
            {
                if (selectedItem.Text.ToString() == "...")
                {
                    // listView1.ListViewItemSorter = null;
                    //DirectoryPath = Path.GetDirectoryName(DirectoryPath);
                    // listViewItemComparer.Order = SortOrder.Descending;
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
    }

}