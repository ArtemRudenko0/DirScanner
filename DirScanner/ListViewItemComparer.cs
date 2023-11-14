using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace DirScanner
{
    public class ListViewItemComparer : IComparer
    {
        private int columnToSort;
        private SortOrder orderOfSort;
        public string directoryPath;
        public ListViewItemComparer()
        {
            columnToSort = 0; // По умолчанию сортируем по первому столбцу
            orderOfSort = SortOrder.Ascending; // По умолчанию сортируем по возрастанию
        }

        public ListViewItemComparer(int column, SortOrder order)
        {
            columnToSort = column;
            this.orderOfSort = order;
            
        }
       
        public int Compare(object x, object y)
        {
            if (((ListViewItem)x).SubItems[0].Text == "..." )
                
            {
                return -1;
            }
            else
            {
               
                string textX = ((ListViewItem)x).SubItems[columnToSort].Text;
                string textY = ((ListViewItem)y).SubItems[columnToSort].Text;
                int compareResult = 0;
                long numFirst, numSecond;

                if (columnToSort == 0)
                {   
                    compareResult = string.Compare(((ListViewItem)x).SubItems[columnToSort].Text,
                        ((ListViewItem)y).SubItems[columnToSort].Text);
                    if (compareResult == 1)
                       compareResult = -1;
                    else if (compareResult == 0) { }
                    else compareResult = 1;
                }
                else
                {

                    numFirst = GetMeasurement(textX);
                    numSecond = GetMeasurement(textY);
                    if (numFirst < numSecond)
                        compareResult = -1;
                    else if (numFirst > numSecond)
                        compareResult = 1;
                    else
                        compareResult = 0;
                }

                // Сравниваем элементы по выбранному столбцу


                // В зависимости от порядка сортировки, меняем знак результата
                if (orderOfSort == SortOrder.Ascending)
                {
                    return compareResult;
                }
                else if (orderOfSort == SortOrder.Descending)
                {
                    return -compareResult;
                }
                else
                {
                    return 0;
                }
            }
        }
        public long GetMeasurement(string number)
        {
            string format = number.Substring(number.Length - 3);
            string valuePart = number.Substring(0, number.Length - 3);
            if (format == " Kb")
            {
                return (long)Convert.ToDouble(valuePart) * 1024; // Килобайты в байты
            }
            else if (format == " Mb")
            {
                return (long)Convert.ToDouble(valuePart) * 1024 * 1024; // Мегабайты в байты
            }
            else if (format == " Gb")
            {
                return (long)Convert.ToDouble(valuePart) * 1024 * 1024 * 1024; // Гигабайты в байты
            }
            return 0;


        }
        public int SortColumn
        {
            get { return columnToSort; }
            set { columnToSort = value; }
        }

        public SortOrder Order
        {
            get { return orderOfSort; }
            set { orderOfSort = value; }
        }
    }
}
