namespace Whu038
{
    partial class frmSymbol
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSymbol));
            this.SymbologyCtrl = new ESRI.ArcGIS.Controls.AxSymbologyControl();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SymbologyCtrl)).BeginInit();
            this.SuspendLayout();
            // 
            // SymbologyCtrl
            // 
            this.SymbologyCtrl.Location = new System.Drawing.Point(13, 13);
            this.SymbologyCtrl.Margin = new System.Windows.Forms.Padding(4);
            this.SymbologyCtrl.Name = "SymbologyCtrl";
            this.SymbologyCtrl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("SymbologyCtrl.OcxState")));
            this.SymbologyCtrl.Size = new System.Drawing.Size(374, 289);
            this.SymbologyCtrl.TabIndex = 1;
            this.SymbologyCtrl.OnMouseDown += new ESRI.ArcGIS.Controls.ISymbologyControlEvents_Ax_OnMouseDownEventHandler(this.SymbologyCtrl_OnMouseDown_1);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(143, 410);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 29);
            this.button1.TabIndex = 2;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(313, 410);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 29);
            this.button2.TabIndex = 3;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // frmSymbol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 452);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SymbologyCtrl);
            this.Name = "frmSymbol";
            this.Text = "frmSymbol";
            ((System.ComponentModel.ISupportInitialize)(this.SymbologyCtrl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ESRI.ArcGIS.Controls.AxSymbologyControl SymbologyCtrl;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}