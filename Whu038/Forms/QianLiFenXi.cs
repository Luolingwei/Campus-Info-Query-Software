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
    public partial class QianLiFenXi : Form
    {
        public QianLiFenXi()
        {
            InitializeComponent();
        }

        public struct QZ
        {
            public double MD, BL, HB, PD, JL, LS;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.ReadOnly = true;
            ListViewItem lvi = new ListViewItem();

            lvi.Text = comboBox1.Text;
            lvi.SubItems.Add(textBox1.Text);


            this.listView1.Items.Add(lvi);
            textBox2.Text = "";
            if (this.listView1.Items != null)
            {
                foreach (ListViewItem item in this.listView1.Items)
                {


                    textBox2.Text += item.SubItems[0].Text + '*' + item.SubItems[1].Text + " + ";

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)  //选中项遍历
            {
                listView1.Items.RemoveAt(lvi.Index); // 按索引移除
                //listView1.Items.Remove(lvi);   //按项移除
            }
            textBox2.Text = "";
            if (this.listView1.Items != null)
            {
                foreach (ListViewItem item in this.listView1.Items)
                {


                    textBox2.Text += item.SubItems[0].Text + '*' + item.SubItems[1].Text + " + ";

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            QZ quanzhong;
            quanzhong.MD = 0;
            quanzhong.BL = 0;
            quanzhong.HB = 0;
            quanzhong.PD = 0;
            quanzhong.JL = 0;
            quanzhong.LS = 0;

            if (this.listView1.Items != null)
            {
                foreach (ListViewItem item in this.listView1.Items)
                {
                    if (item.SubItems[0].Text == "人口密度")
                    {
                        quanzhong.MD = Convert.ToDouble(item.SubItems[1].Text);
                    }
                    if (item.SubItems[0].Text == "居民地比例")
                    {
                        quanzhong.BL = Convert.ToDouble(item.SubItems[1].Text);
                    }
                    if (item.SubItems[0].Text == "平均海拔")
                    {
                        quanzhong.HB = Convert.ToDouble(item.SubItems[1].Text);
                    }
                    if (item.SubItems[0].Text == "平均坡度")
                    {
                        quanzhong.PD = Convert.ToDouble(item.SubItems[1].Text);
                    }
                    if (item.SubItems[0].Text == "坡度与公路距离")
                    {
                        quanzhong.JL = Convert.ToDouble(item.SubItems[1].Text);
                    }
                    if (item.SubItems[0].Text == "坡向离散度")
                    {
                        quanzhong.LS = Convert.ToDouble(item.SubItems[1].Text);
                    }

                }
            }
        }
    }
}
    
