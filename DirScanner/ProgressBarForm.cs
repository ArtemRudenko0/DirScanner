using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DirScanner
{   

    public partial class ProgressBarForm : Form
    {
       private int totalFolders;
        public ProgressBarForm()
        {
            InitializeComponent();
        }
        public ProgressBarForm(int totalFolders)
        {//, Form ParentForm
            InitializeComponent();
            this.totalFolders = totalFolders;
            // CenterChildForm(this, ParentForm);
            progressBar.Minimum = 0;
            progressBar.Maximum = this.totalFolders;
        }
        public void CenterChildForm(Form childForm, Form parentForm, int ListViewX, int ListViewY)
        {
            childForm.StartPosition = FormStartPosition.Manual;

            childForm.Location = new Point(
                
                 parentForm.Location.X + ListViewX + 13 - 4,
                parentForm.Location.Y + ListViewY + 58 - 20
            );
            progressBar.Location = new Point(
                   (childForm.Width - progressBar.Width) / 2,
                     (childForm.Height - progressBar.Height) / 2);
            label1.Location = new Point(
                    progressBar.Location.X,
                     ((childForm.Height - label1.Height) / 2) - progressBar.Height - 10);
        }
        public void UpdateProgress()
        {
            if (progressBar.Value < progressBar.Maximum)
            {
                progressBar.Value++;
            }

        }


        private void ProgressBarForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }

        public int TotalFolders
        {
            get { return totalFolders; }
            set { progressBar.Maximum = value; }

        }
    }

}
