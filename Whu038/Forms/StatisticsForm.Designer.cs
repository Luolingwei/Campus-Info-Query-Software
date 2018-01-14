namespace Whu038.Forms
{
    partial class StatisticsForm
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
            this.labelSelection = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxLayers = new System.Windows.Forms.ComboBox();
            this.comboBoxFields = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelStatisticsResult = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelSelection
            // 
            this.labelSelection.AutoSize = true;
            this.labelSelection.Location = new System.Drawing.Point(64, 30);
            this.labelSelection.Name = "labelSelection";
            this.labelSelection.Size = new System.Drawing.Size(355, 15);
            this.labelSelection.TabIndex = 0;
            this.labelSelection.Text = "当前地图选择集共有   个图层的   个要素被选中。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "图层：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(97, 163);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "字段：";
            // 
            // comboBoxLayers
            // 
            this.comboBoxLayers.FormattingEnabled = true;
            this.comboBoxLayers.Location = new System.Drawing.Point(177, 84);
            this.comboBoxLayers.Name = "comboBoxLayers";
            this.comboBoxLayers.Size = new System.Drawing.Size(188, 23);
            this.comboBoxLayers.TabIndex = 3;
            this.comboBoxLayers.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayers_SelectedIndexChanged);
            // 
            // comboBoxFields
            // 
            this.comboBoxFields.FormattingEnabled = true;
            this.comboBoxFields.Location = new System.Drawing.Point(177, 160);
            this.comboBoxFields.Name = "comboBoxFields";
            this.comboBoxFields.Size = new System.Drawing.Size(188, 23);
            this.comboBoxFields.TabIndex = 4;
            this.comboBoxFields.SelectedIndexChanged += new System.EventHandler(this.comboBoxFields_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelStatisticsResult);
            this.groupBox1.Location = new System.Drawing.Point(67, 218);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 141);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "统计结果";
            // 
            // labelStatisticsResult
            // 
            this.labelStatisticsResult.Location = new System.Drawing.Point(30, 21);
            this.labelStatisticsResult.Name = "labelStatisticsResult";
            this.labelStatisticsResult.Size = new System.Drawing.Size(288, 105);
            this.labelStatisticsResult.TabIndex = 0;
            // 
            // StatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 402);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxFields);
            this.Controls.Add(this.comboBoxLayers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelSelection);
            this.Name = "StatisticsForm";
            this.Text = "选择统计";
            this.Load += new System.EventHandler(this.StatisticsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSelection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxLayers;
        private System.Windows.Forms.ComboBox comboBoxFields;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelStatisticsResult;
    }
}