using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Whu038.Forms;

namespace Whu038.Forms
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            label1.BackColor = Color.Transparent;
            label1.Parent = pictureBox1;
            label1.BringToFront();
            label2.BackColor = Color.Transparent;
            label2.Parent = pictureBox1;
            label2.BringToFront();
            label3.BackColor = Color.Transparent;
            label3.Parent = pictureBox1;
            label3.BringToFront();
        }

        private void Login_Click(object sender, EventArgs e)
        {
            if (Username.Text.ToString() == "Whu" && Password.Text.ToString() == "123")
            {
                MessageBox.Show("登录成功！");
                this.DialogResult = DialogResult.OK;
            }
            else if (Username.Text.ToString() == "" || Password.Text.ToString() == "")
            {
                MessageBox.Show("请输入完整的账号和密码");
            }
            else
            {
                MessageBox.Show("账号或密码错误！");
            }
        }
    }
}
