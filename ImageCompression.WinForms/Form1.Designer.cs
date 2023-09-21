namespace ImageCompression.WinForms
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UncompressedPicture = new System.Windows.Forms.PictureBox();
            this.CompressedPicture = new System.Windows.Forms.PictureBox();
            this.CompressionButton = new System.Windows.Forms.Button();
            this.ResultTextBox = new System.Windows.Forms.TextBox();
            this.AlgorithmsComboBox = new System.Windows.Forms.ComboBox();
            this.DecompressButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenButton = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveButton = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.UncompressedPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompressedPicture)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UncompressedPicture
            // 
            this.UncompressedPicture.Location = new System.Drawing.Point(116, 79);
            this.UncompressedPicture.Name = "UncompressedPicture";
            this.UncompressedPicture.Size = new System.Drawing.Size(461, 300);
            this.UncompressedPicture.TabIndex = 0;
            this.UncompressedPicture.TabStop = false;
            // 
            // CompressedPicture
            // 
            this.CompressedPicture.Location = new System.Drawing.Point(734, 79);
            this.CompressedPicture.Name = "CompressedPicture";
            this.CompressedPicture.Size = new System.Drawing.Size(461, 300);
            this.CompressedPicture.TabIndex = 1;
            this.CompressedPicture.TabStop = false;
            // 
            // CompressionButton
            // 
            this.CompressionButton.Location = new System.Drawing.Point(128, 569);
            this.CompressionButton.Name = "CompressionButton";
            this.CompressionButton.Size = new System.Drawing.Size(430, 40);
            this.CompressionButton.TabIndex = 2;
            this.CompressionButton.Text = "Сжатие изображения";
            this.CompressionButton.UseVisualStyleBackColor = true;
            this.CompressionButton.Click += new System.EventHandler(this.CompressionButton_Click);
            // 
            // ResultTextBox
            // 
            this.ResultTextBox.Location = new System.Drawing.Point(744, 503);
            this.ResultTextBox.Multiline = true;
            this.ResultTextBox.Name = "ResultTextBox";
            this.ResultTextBox.Size = new System.Drawing.Size(451, 174);
            this.ResultTextBox.TabIndex = 3;
            // 
            // AlgorithmsComboBox
            // 
            this.AlgorithmsComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AlgorithmsComboBox.FormattingEnabled = true;
            this.AlgorithmsComboBox.Location = new System.Drawing.Point(128, 503);
            this.AlgorithmsComboBox.Name = "AlgorithmsComboBox";
            this.AlgorithmsComboBox.Size = new System.Drawing.Size(430, 37);
            this.AlgorithmsComboBox.TabIndex = 4;
            // 
            // DecompressButton
            // 
            this.DecompressButton.Location = new System.Drawing.Point(128, 637);
            this.DecompressButton.Name = "DecompressButton";
            this.DecompressButton.Size = new System.Drawing.Size(430, 40);
            this.DecompressButton.TabIndex = 5;
            this.DecompressButton.Text = "Востановление изображения";
            this.DecompressButton.UseVisualStyleBackColor = true;
            this.DecompressButton.Click += new System.EventHandler(this.DecompressButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1324, 28);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenButton,
            this.SaveButton});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // OpenButton
            // 
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(152, 26);
            this.OpenButton.Text = "Open";
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(152, 26);
            this.SaveButton.Text = "Save As...";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1324, 771);
            this.Controls.Add(this.DecompressButton);
            this.Controls.Add(this.AlgorithmsComboBox);
            this.Controls.Add(this.ResultTextBox);
            this.Controls.Add(this.CompressionButton);
            this.Controls.Add(this.CompressedPicture);
            this.Controls.Add(this.UncompressedPicture);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.UncompressedPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompressedPicture)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox UncompressedPicture;
        private System.Windows.Forms.PictureBox CompressedPicture;
        private System.Windows.Forms.Button CompressionButton;
        private System.Windows.Forms.TextBox ResultTextBox;
        private System.Windows.Forms.ComboBox AlgorithmsComboBox;
        private System.Windows.Forms.Button DecompressButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenButton;
        private System.Windows.Forms.ToolStripMenuItem SaveButton;
    }
}

