namespace Whu038.Forms
{
    partial class MapExportForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxHeight = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxWidth = new System.Windows.Forms.TextBox();
            this.cmbResolution = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExPath = new System.Windows.Forms.Button();
            this.tbxExPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbxHeight);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbxWidth);
            this.groupBox1.Controls.Add(this.cmbResolution);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(35, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(656, 148);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "导出设置";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(572, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "像素";
            // 
            // tbxHeight
            // 
            this.tbxHeight.Location = new System.Drawing.Point(452, 41);
            this.tbxHeight.Name = "tbxHeight";
            this.tbxHeight.Size = new System.Drawing.Size(100, 25);
            this.tbxHeight.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(364, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "输出高度：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(295, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "像素";
            // 
            // tbxWidth
            // 
            this.tbxWidth.Location = new System.Drawing.Point(155, 41);
            this.tbxWidth.Name = "tbxWidth";
            this.tbxWidth.Size = new System.Drawing.Size(121, 25);
            this.tbxWidth.TabIndex = 3;
            // 
            // cmbResolution
            // 
            this.cmbResolution.FormattingEnabled = true;
            this.cmbResolution.Location = new System.Drawing.Point(155, 88);
            this.cmbResolution.Name = "cmbResolution";
            this.cmbResolution.Size = new System.Drawing.Size(121, 23);
            this.cmbResolution.TabIndex = 2;
            this.cmbResolution.SelectedIndexChanged += new System.EventHandler(this.cmbResolution_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "输出分辨率：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "导出宽度：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnExPath);
            this.groupBox2.Controls.Add(this.tbxExPath);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(35, 205);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(656, 144);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "导出路径";
            // 
            // btnExPath
            // 
            this.btnExPath.Location = new System.Drawing.Point(488, 60);
            this.btnExPath.Name = "btnExPath";
            this.btnExPath.Size = new System.Drawing.Size(90, 28);
            this.btnExPath.TabIndex = 2;
            this.btnExPath.Text = "浏览";
            this.btnExPath.UseVisualStyleBackColor = true;
            this.btnExPath.Click += new System.EventHandler(this.btnExPath_Click);
            // 
            // tbxExPath
            // 
            this.tbxExPath.Location = new System.Drawing.Point(157, 64);
            this.tbxExPath.Name = "tbxExPath";
            this.tbxExPath.Size = new System.Drawing.Size(271, 25);
            this.tbxExPath.TabIndex = 1;
            this.tbxExPath.Click += new System.EventHandler(this.tbxExPath_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(73, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "请选择：";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(212, 375);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 28);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "导出";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(439, 375);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MapExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 433);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MapExportForm";
            this.Text = "地图导出";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MapExportForm_FormClosed);
            this.Load += new System.EventHandler(this.MapExportForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxWidth;
        private System.Windows.Forms.ComboBox cmbResolution;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnExPath;
        private System.Windows.Forms.TextBox tbxExPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
    }
}