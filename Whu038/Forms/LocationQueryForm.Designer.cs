namespace Whu038.Forms
{
    partial class LocationQueryForm
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
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.listBoxField = new System.Windows.Forms.ListBox();
            this.listBoxValue = new System.Windows.Forms.ListBox();
            this.textBoxSql = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Btnequal = new System.Windows.Forms.Button();
            this.btnunequal = new System.Windows.Forms.Button();
            this.btnin = new System.Windows.Forms.Button();
            this.Btnless = new System.Windows.Forms.Button();
            this.btnloe = new System.Windows.Forms.Button();
            this.btnspace = new System.Windows.Forms.Button();
            this.btnunderline = new System.Windows.Forms.Button();
            this.btnempty = new System.Windows.Forms.Button();
            this.btnlike = new System.Windows.Forms.Button();
            this.btnmore = new System.Windows.Forms.Button();
            this.btnnull = new System.Windows.Forms.Button();
            this.btnbetween = new System.Windows.Forms.Button();
            this.btnor = new System.Windows.Forms.Button();
            this.btnand = new System.Windows.Forms.Button();
            this.btnmoe = new System.Windows.Forms.Button();
            this.Btncharacter = new System.Windows.Forms.Button();
            this.Btnnot = new System.Windows.Forms.Button();
            this.btnpercent = new System.Windows.Forms.Button();
            this.btnis = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboLayer
            // 
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(146, 26);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(416, 23);
            this.cboLayer.TabIndex = 0;
            this.cboLayer.SelectedIndexChanged += new System.EventHandler(this.cboLayer_SelectedIndexChanged);
            // 
            // listBoxField
            // 
            this.listBoxField.FormattingEnabled = true;
            this.listBoxField.ItemHeight = 15;
            this.listBoxField.Location = new System.Drawing.Point(49, 98);
            this.listBoxField.Name = "listBoxField";
            this.listBoxField.Size = new System.Drawing.Size(215, 154);
            this.listBoxField.TabIndex = 1;
            this.listBoxField.SelectedIndexChanged += new System.EventHandler(this.listBoxField_SelectedIndexChanged);
            this.listBoxField.DoubleClick += new System.EventHandler(this.listBoxField_DoubleClick);
            // 
            // listBoxValue
            // 
            this.listBoxValue.FormattingEnabled = true;
            this.listBoxValue.ItemHeight = 15;
            this.listBoxValue.Location = new System.Drawing.Point(355, 98);
            this.listBoxValue.Name = "listBoxValue";
            this.listBoxValue.Size = new System.Drawing.Size(207, 154);
            this.listBoxValue.TabIndex = 2;
            this.listBoxValue.DoubleClick += new System.EventHandler(this.listBoxValue_DoubleClick);
            // 
            // textBoxSql
            // 
            this.textBoxSql.Location = new System.Drawing.Point(49, 512);
            this.textBoxSql.Name = "textBoxSql";
            this.textBoxSql.Size = new System.Drawing.Size(513, 25);
            this.textBoxSql.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Btnequal);
            this.groupBox1.Controls.Add(this.btnunequal);
            this.groupBox1.Controls.Add(this.btnin);
            this.groupBox1.Controls.Add(this.Btnless);
            this.groupBox1.Controls.Add(this.btnloe);
            this.groupBox1.Controls.Add(this.btnspace);
            this.groupBox1.Controls.Add(this.btnunderline);
            this.groupBox1.Controls.Add(this.btnempty);
            this.groupBox1.Controls.Add(this.btnlike);
            this.groupBox1.Controls.Add(this.btnmore);
            this.groupBox1.Controls.Add(this.btnnull);
            this.groupBox1.Controls.Add(this.btnbetween);
            this.groupBox1.Controls.Add(this.btnor);
            this.groupBox1.Controls.Add(this.btnand);
            this.groupBox1.Controls.Add(this.btnmoe);
            this.groupBox1.Controls.Add(this.Btncharacter);
            this.groupBox1.Controls.Add(this.Btnnot);
            this.groupBox1.Controls.Add(this.btnpercent);
            this.groupBox1.Controls.Add(this.btnis);
            this.groupBox1.Location = new System.Drawing.Point(49, 277);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 190);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "表达式";
            // 
            // Btnequal
            // 
            this.Btnequal.Location = new System.Drawing.Point(21, 32);
            this.Btnequal.Name = "Btnequal";
            this.Btnequal.Size = new System.Drawing.Size(75, 25);
            this.Btnequal.TabIndex = 5;
            this.Btnequal.Text = "=";
            this.Btnequal.UseVisualStyleBackColor = true;
            this.Btnequal.Click += new System.EventHandler(this.Btnequal_Click);
            // 
            // btnunequal
            // 
            this.btnunequal.Location = new System.Drawing.Point(120, 32);
            this.btnunequal.Name = "btnunequal";
            this.btnunequal.Size = new System.Drawing.Size(75, 25);
            this.btnunequal.TabIndex = 6;
            this.btnunequal.Text = "!=";
            this.btnunequal.UseVisualStyleBackColor = true;
            this.btnunequal.Click += new System.EventHandler(this.btnunequal_Click);
            // 
            // btnin
            // 
            this.btnin.Location = new System.Drawing.Point(219, 112);
            this.btnin.Name = "btnin";
            this.btnin.Size = new System.Drawing.Size(75, 25);
            this.btnin.TabIndex = 22;
            this.btnin.Text = "In";
            this.btnin.UseVisualStyleBackColor = true;
            this.btnin.Click += new System.EventHandler(this.btnin_Click);
            // 
            // Btnless
            // 
            this.Btnless.Location = new System.Drawing.Point(21, 72);
            this.Btnless.Name = "Btnless";
            this.Btnless.Size = new System.Drawing.Size(75, 25);
            this.Btnless.TabIndex = 23;
            this.Btnless.Text = "<";
            this.Btnless.UseVisualStyleBackColor = true;
            this.Btnless.Click += new System.EventHandler(this.Btnless_Click);
            // 
            // btnloe
            // 
            this.btnloe.Location = new System.Drawing.Point(219, 72);
            this.btnloe.Name = "btnloe";
            this.btnloe.Size = new System.Drawing.Size(75, 25);
            this.btnloe.TabIndex = 9;
            this.btnloe.Text = "<=";
            this.btnloe.UseVisualStyleBackColor = true;
            this.btnloe.Click += new System.EventHandler(this.btnloe_Click);
            // 
            // btnspace
            // 
            this.btnspace.Location = new System.Drawing.Point(271, 151);
            this.btnspace.Name = "btnspace";
            this.btnspace.Size = new System.Drawing.Size(98, 25);
            this.btnspace.TabIndex = 10;
            this.btnspace.Text = "空格";
            this.btnspace.UseVisualStyleBackColor = true;
            this.btnspace.Click += new System.EventHandler(this.btnspace_Click);
            // 
            // btnunderline
            // 
            this.btnunderline.Location = new System.Drawing.Point(318, 113);
            this.btnunderline.Name = "btnunderline";
            this.btnunderline.Size = new System.Drawing.Size(75, 25);
            this.btnunderline.TabIndex = 11;
            this.btnunderline.Text = "_";
            this.btnunderline.UseVisualStyleBackColor = true;
            this.btnunderline.Click += new System.EventHandler(this.btnunderline_Click);
            // 
            // btnempty
            // 
            this.btnempty.Location = new System.Drawing.Point(396, 151);
            this.btnempty.Name = "btnempty";
            this.btnempty.Size = new System.Drawing.Size(98, 25);
            this.btnempty.TabIndex = 12;
            this.btnempty.Text = "清空";
            this.btnempty.UseVisualStyleBackColor = true;
            this.btnempty.Click += new System.EventHandler(this.btnempty_Click);
            // 
            // btnlike
            // 
            this.btnlike.Location = new System.Drawing.Point(318, 32);
            this.btnlike.Name = "btnlike";
            this.btnlike.Size = new System.Drawing.Size(75, 25);
            this.btnlike.TabIndex = 13;
            this.btnlike.Text = "like";
            this.btnlike.UseVisualStyleBackColor = true;
            this.btnlike.Click += new System.EventHandler(this.btnlike_Click);
            // 
            // btnmore
            // 
            this.btnmore.Location = new System.Drawing.Point(417, 32);
            this.btnmore.Name = "btnmore";
            this.btnmore.Size = new System.Drawing.Size(75, 25);
            this.btnmore.TabIndex = 14;
            this.btnmore.Text = ">";
            this.btnmore.UseVisualStyleBackColor = true;
            this.btnmore.Click += new System.EventHandler(this.btnmore_Click);
            // 
            // btnnull
            // 
            this.btnnull.Location = new System.Drawing.Point(417, 72);
            this.btnnull.Name = "btnnull";
            this.btnnull.Size = new System.Drawing.Size(75, 25);
            this.btnnull.TabIndex = 15;
            this.btnnull.Text = "Null";
            this.btnnull.UseVisualStyleBackColor = true;
            this.btnnull.Click += new System.EventHandler(this.btnnull_Click);
            // 
            // btnbetween
            // 
            this.btnbetween.Location = new System.Drawing.Point(146, 151);
            this.btnbetween.Name = "btnbetween";
            this.btnbetween.Size = new System.Drawing.Size(98, 25);
            this.btnbetween.TabIndex = 17;
            this.btnbetween.Text = "Between";
            this.btnbetween.UseVisualStyleBackColor = true;
            this.btnbetween.Click += new System.EventHandler(this.btnbetween_Click);
            // 
            // btnor
            // 
            this.btnor.Location = new System.Drawing.Point(318, 71);
            this.btnor.Name = "btnor";
            this.btnor.Size = new System.Drawing.Size(75, 25);
            this.btnor.TabIndex = 16;
            this.btnor.Text = "Or";
            this.btnor.UseVisualStyleBackColor = true;
            this.btnor.Click += new System.EventHandler(this.btnor_Click);
            // 
            // btnand
            // 
            this.btnand.Location = new System.Drawing.Point(120, 112);
            this.btnand.Name = "btnand";
            this.btnand.Size = new System.Drawing.Size(75, 25);
            this.btnand.TabIndex = 18;
            this.btnand.Text = "And";
            this.btnand.UseVisualStyleBackColor = true;
            this.btnand.Click += new System.EventHandler(this.btnand_Click);
            // 
            // btnmoe
            // 
            this.btnmoe.Location = new System.Drawing.Point(120, 72);
            this.btnmoe.Name = "btnmoe";
            this.btnmoe.Size = new System.Drawing.Size(75, 25);
            this.btnmoe.TabIndex = 19;
            this.btnmoe.Text = ">=";
            this.btnmoe.UseVisualStyleBackColor = true;
            this.btnmoe.Click += new System.EventHandler(this.btnmoe_Click);
            // 
            // Btncharacter
            // 
            this.Btncharacter.Location = new System.Drawing.Point(21, 151);
            this.Btncharacter.Name = "Btncharacter";
            this.Btncharacter.Size = new System.Drawing.Size(98, 25);
            this.Btncharacter.TabIndex = 20;
            this.Btncharacter.Text = "\' \'";
            this.Btncharacter.UseVisualStyleBackColor = true;
            this.Btncharacter.Click += new System.EventHandler(this.Btncharacter_Click);
            // 
            // Btnnot
            // 
            this.Btnnot.Location = new System.Drawing.Point(21, 112);
            this.Btnnot.Name = "Btnnot";
            this.Btnnot.Size = new System.Drawing.Size(75, 25);
            this.Btnnot.TabIndex = 21;
            this.Btnnot.Text = "Not";
            this.Btnnot.UseVisualStyleBackColor = true;
            this.Btnnot.Click += new System.EventHandler(this.Btnnot_Click);
            // 
            // btnpercent
            // 
            this.btnpercent.Location = new System.Drawing.Point(417, 113);
            this.btnpercent.Name = "btnpercent";
            this.btnpercent.Size = new System.Drawing.Size(75, 25);
            this.btnpercent.TabIndex = 8;
            this.btnpercent.Text = "%";
            this.btnpercent.UseVisualStyleBackColor = true;
            this.btnpercent.Click += new System.EventHandler(this.btnpercent_Click);
            // 
            // btnis
            // 
            this.btnis.Location = new System.Drawing.Point(219, 32);
            this.btnis.Name = "btnis";
            this.btnis.Size = new System.Drawing.Size(75, 25);
            this.btnis.TabIndex = 7;
            this.btnis.Text = "is";
            this.btnis.UseVisualStyleBackColor = true;
            this.btnis.Click += new System.EventHandler(this.btnis_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 24;
            this.label1.Text = "图层：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 25;
            this.label2.Text = "字段：";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(86, 565);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(96, 27);
            this.btnOK.TabIndex = 24;
            this.btnOK.Text = "查找";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(445, 565);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 27);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(352, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 26;
            this.label3.Text = "取值：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 483);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(222, 15);
            this.label4.TabIndex = 27;
            this.label4.Text = "Select * From Table Where：";
            // 
            // AttributeQueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 638);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxSql);
            this.Controls.Add(this.listBoxValue);
            this.Controls.Add(this.listBoxField);
            this.Controls.Add(this.cboLayer);
            this.Name = "AttributeQueryForm";
            this.Text = "位置查询";
            this.Load += new System.EventHandler(this.AttributeQueryForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.ListBox listBoxField;
        private System.Windows.Forms.ListBox listBoxValue;
        private System.Windows.Forms.TextBox textBoxSql;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Btnequal;
        private System.Windows.Forms.Button btnunequal;
        private System.Windows.Forms.Button btnin;
        private System.Windows.Forms.Button Btnless;
        private System.Windows.Forms.Button btnloe;
        private System.Windows.Forms.Button btnspace;
        private System.Windows.Forms.Button btnunderline;
        private System.Windows.Forms.Button btnempty;
        private System.Windows.Forms.Button btnlike;
        private System.Windows.Forms.Button btnmore;
        private System.Windows.Forms.Button btnnull;
        private System.Windows.Forms.Button btnor;
        private System.Windows.Forms.Button btnbetween;
        private System.Windows.Forms.Button btnand;
        private System.Windows.Forms.Button btnmoe;
        private System.Windows.Forms.Button Btncharacter;
        private System.Windows.Forms.Button Btnnot;
        private System.Windows.Forms.Button btnpercent;
        private System.Windows.Forms.Button btnis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}