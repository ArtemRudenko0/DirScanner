using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
namespace DirScanner
{
    public partial class Form1 : Form
    {
        private string resultsPath = "D:\\Results\\files.txt";
        int indentationCount = 1;
        private FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        public Form1()
        {
            InitializeComponent();
            //folderBrowserDialog.Description = "Выберите папку";
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void buttonScan_Click(object sender, EventArgs e)
        {
            string folderPath = textBoxFolder.Text;
            // string folderPath = "D:\\Games";
            if (Directory.Exists(folderPath))
            {
                using (StreamWriter writer = new StreamWriter(resultsPath, false))
                {
                    writer.WriteLine("Вміст папки '" + folderPath +"': ");
                }
                long totalSize = CalculateTotalSize(folderPath);
   
                LabelResult.Text = "Загальний розмір файлів: " + getMeasurement(totalSize);
            }
            else
            {
                LabelResult.Text = "Вказаної папки не існує.";
            }
        }
        private long CalculateTotalSize(string folderPath)
        {
            
            
            long totalSize = 0;
            
            string[] files = Directory.GetFiles(folderPath);
            
            using (StreamWriter writer = new StreamWriter(resultsPath,true))
            {   
                    
                
                foreach (string file in files)
                {   
                    FileInfo fileInfo = new FileInfo(file);
                    totalSize += fileInfo.Length;

                    writer.WriteLine(Convert.ToString(setIndentation(indentationCount) + fileInfo.Name + ": " + getMeasurement(fileInfo.Length)));
                   
                   
                }
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
                       // indentationCount++;
                         using (StreamWriter writer = new StreamWriter(resultsPath, true))
                         {
                            writer.WriteLine(setIndentation(indentationCount) + directoryInfo.Name + ": ");
                         }
                        indentationCount++;
                        totalSize += CalculateTotalSize(subfolder); // Рекурсивный вызов
                        indentationCount--;

                    }
                }
            
            return totalSize;
        }
        private string getMeasurement(long lenght)
        {
            string newMeasurement = " ";
            int iteration = 0;
            for(int i = 3; i > 0; i--)
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
        private string setIndentation(int count)
        {
            string indentation = " ";
            for(int i = 0; i < count; i++)
            {
                indentation += "    ";
            }
            return indentation;
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
    }
}