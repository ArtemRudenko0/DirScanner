﻿namespace DirScanner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            buttonChoose = new Button();
            buttonScan = new Button();
            textBoxFolder = new TextBox();
            pictureBox1 = new PictureBox();
            LabelResult = new Label();
            listView1 = new ListView();
            column = new ColumnHeader();
            column1 = new ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // buttonChoose
            // 
            buttonChoose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            buttonScan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            textBoxFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            LabelResult.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelResult.AutoSize = true;
            LabelResult.Font = new Font("Segoe UI Semilight", 16.2F, FontStyle.Regular, GraphicsUnit.Point);
            LabelResult.Location = new Point(14, 385);
            LabelResult.Name = "LabelResult";
            LabelResult.Size = new Size(0, 38);
            LabelResult.TabIndex = 4;
            // 
            // listView1
            // 
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listView1.Columns.AddRange(new ColumnHeader[] { column, column1 });
            listView1.Font = new Font("Segoe UI Semilight", 9F, FontStyle.Regular, GraphicsUnit.Point);
            listView1.FullRowSelect = true;
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
            // column
            // 
            column.Text = "Ім'я";
            column.Width = 200;
            // 
            // column1
            // 
            column1.Text = "Розмір";
            column1.Width = 200;
            // 
            // Form1
            // 
            AcceptButton = buttonScan;
            AutoScaleMode = AutoScaleMode.Inherit;
            BackColor = SystemColors.Control;
            ClientSize = new Size(800, 450);
            Controls.Add(listView1);
            Controls.Add(LabelResult);
            Controls.Add(pictureBox1);
            Controls.Add(textBoxFolder);
            Controls.Add(buttonScan);
            Controls.Add(buttonChoose);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Location = new Point(150, 100);
            Name = "Form1";
            RightToLeft = RightToLeft.No;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Directory Scanner";
            Load += Form1_Load;
            LocationChanged += Form1_LocationChanged;
            SizeChanged += Form1_SizeChanged;
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
        private ColumnHeader column;
        private ColumnHeader column1;
    }
}