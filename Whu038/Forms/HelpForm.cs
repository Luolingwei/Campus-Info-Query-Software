using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Whu038.Forms
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            this.timer1.Start();
            this.timer1.Interval = 100;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Time.Text = DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss");
            Application.DoEvents();
        }
   
       

       
    }
}
