namespace Whu038.Forms
{
    partial class NearbyForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbLayer = new System.Windows.Forms.ComboBox();
            this.tbxRadius = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.Location = new System.Drawing.Point(80, 173);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(148, 28);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "查找对象";
            // 
            // cmbLayer
            // 
            this.cmbLayer.FormattingEnabled = true;
            this.cmbLayer.Location = new System.Drawing.Point(122, 42);
            this.cmbLayer.Name = "cmbLayer";
            this.cmbLayer.Size = new System.Drawing.Size(145, 23);
            this.cmbLayer.TabIndex = 7;
            this.cmbLayer.SelectedIndexChanged += new System.EventHandler(this.cmbLayer_SelectedIndexChanged);
            this.cmbLayer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbLayer_MouseClick);
            // 
            // tbxRadius
            // 
            this.tbxRadius.Location = new System.Drawing.Point(122, 101);
            this.tbxRadius.Name = "tbxRadius";
            this.tbxRadius.Size = new System.Drawing.Size(145, 25);
            this.tbxRadius.TabIndex = 9;
            this.tbxRadius.TextChanged += new System.EventHandler(this.tbxRadius_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "设置半径";
            // 
            // NearbyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 236);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxRadius);
            this.Controls.Add(this.cmbLayer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Name = "NearbyForm";
            this.Text = "附近";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbLayer;
        private System.Windows.Forms.TextBox tbxRadius;
        private System.Windows.Forms.Label label2;
    }
}