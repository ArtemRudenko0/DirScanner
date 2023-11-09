namespace DirScanner
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonChoose = new Button();
            buttonScan = new Button();
            textBoxFolder = new TextBox();
            pictureBox1 = new PictureBox();
            LabelResult = new Label();
            listView1 = new ListView();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // buttonChoose
            // 
            buttonChoose.Location = new Point(454, 12);
            buttonChoose.Name = "buttonChoose";
            buttonChoose.Size = new Size(148, 39);
            buttonChoose.TabIndex = 0;
            buttonChoose.Text = "Вибрати";
            buttonChoose.UseVisualStyleBackColor = true;
            buttonChoose.Click += buttonChoose_Click;
            // 
            // buttonScan
            // 
            buttonScan.Location = new Point(608, 12);
            buttonScan.Name = "buttonScan";
            buttonScan.Size = new Size(180, 39);
            buttonScan.TabIndex = 1;
            buttonScan.Text = "Сканувати";
            buttonScan.UseVisualStyleBackColor = true;
            buttonScan.Click += buttonScan_Click;
            // 
            // textBoxFolder
            // 
            textBoxFolder.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            textBoxFolder.Location = new Point(56, 12);
            textBoxFolder.Multiline = true;
            textBoxFolder.Name = "textBoxFolder";
            textBoxFolder.Size = new Size(392, 39);
            textBoxFolder.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Folder;
            pictureBox1.Location = new Point(3, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(47, 39);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // LabelResult
            // 
            LabelResult.AutoSize = true;
            LabelResult.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            LabelResult.Location = new Point(14, 385);
            LabelResult.Name = "LabelResult";
            LabelResult.Size = new Size(0, 46);
            LabelResult.TabIndex = 4;
            // 
            // listView1
            // 
            listView1.Location = new Point(13, 58);
            listView1.Margin = new Padding(4);
            listView1.Name = "listView1";
            listView1.Size = new Size(773, 323);
            listView1.TabIndex = 5;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.List;
            listView1.ColumnClick += listView1_ColumnClick;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listView1);
            Controls.Add(LabelResult);
            Controls.Add(pictureBox1);
            Controls.Add(textBoxFolder);
            Controls.Add(buttonScan);
            Controls.Add(buttonChoose);
            Name = "Form1";
            Text = "Directory Scanner";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonChoose;
        private Button buttonScan;
        private TextBox textBoxFolder;
        private PictureBox pictureBox1;
        private Label LabelResult;
        private ListView listView1;
    }
}