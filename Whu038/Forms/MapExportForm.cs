using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using Whu038.Classes;

namespace Whu038.Forms
{  
    public partial class MapExportForm : Form
    {  
     
        private string pSavePath = "";
        private IActiveView pActiveView;
        private IEnvelope pGeometry = null;
        /// <summary>
        /// 只读属性，地图导出空间图形
        /// </summary>
        public IEnvelope GetGeometry
        {
            set
            {
                pGeometry = value;
            }
        }
        private bool bRegion = true;
        /// <summary>
        /// 只读属性，是全域导出还是自由区域导出
        /// </summary>
        public bool IsRegion
        {
            set
            {
                bRegion = value;
            }
        }
        public MapExportForm(AxMapControl mainAxMapControl)
        {
            InitializeComponent();
            pActiveView = mainAxMapControl.ActiveView;
        }

        private void MapExportForm_Load(object sender, EventArgs e)
        {
            InitFormSize();
        }
        private void InitFormSize()
        {
            cmbResolution.Text = pActiveView.ScreenDisplay.DisplayTransformation.Resolution.ToString();
            cmbResolution.Items.Add(cmbResolution.Text);
            if (bRegion)
            {
                IEnvelope pEnvelope = pGeometry.Envelope;
                tagRECT pRECT = new tagRECT();
                pActiveView.ScreenDisplay.DisplayTransformation.TransformRect(pEnvelope, ref pRECT, 9);
                if (cmbResolution.Text != "")
                {
                    tbxWidth.Text = pRECT.right.ToString();
                    tbxHeight.Text = pRECT.bottom.ToString();
                }
            }
            else
            {
                if (cmbResolution.Text != "")
                {
                    tbxWidth.Text = pActiveView.ExportFrame.right.ToString();
                    tbxHeight.Text = pActiveView.ExportFrame.bottom.ToString();
                }
            }
        }

        private void cmbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            double num = (int)Math.Round(pActiveView.ScreenDisplay.DisplayTransformation.Resolution);
            if (cmbResolution.Text == "")
            {
                tbxWidth.Text = "";
                tbxHeight.Text = "";
                return;
            }
            if (bRegion)
            {
                IEnvelope pEnvelope = pGeometry.Envelope;
                tagRECT pRECT = new tagRECT();
                pActiveView.ScreenDisplay.DisplayTransformation.TransformRect(pEnvelope, ref pRECT, 9);
                if (cmbResolution.Text != "")
                {
                    tbxWidth.Text = Math.Round((double)(pRECT.right * (double.Parse(cmbResolution.Text) / (double)num))).ToString();
                    tbxHeight.Text = Math.Round((double)(pRECT.bottom * (double.Parse(cmbResolution.Text) / (double)num))).ToString();
                }
            }
            else
            {
                tbxWidth.Text = Math.Round((double)(pActiveView.ExportFrame.right * (double.Parse(cmbResolution.Text) / (double)num))).ToString();
                tbxHeight.Text = Math.Round((double)(pActiveView.ExportFrame.bottom * (double.Parse(cmbResolution.Text) / (double)num))).ToString();
            }
        }

        private void tbxExPath_Click(object sender, EventArgs e)
        {
            //SaveFileDialog sfdExportMap = new SaveFileDialog();
            //sfdExportMap.DefaultExt = "jpg|bmp|gig|tif|png|pdf";
            //sfdExportMap.Filter = "JPGE 文件(*.jpg)|*.jpg|BMP 文件(*.bmp)|*.bmp|GIF 文件(*.gif)|*.gif|TIF 文件(*.tif)|*.tif|PNG 文件(*.png)|*.png|PDF 文件(*.pdf)|*.pdf";
            //sfdExportMap.OverwritePrompt = true;
            //sfdExportMap.Title = "保存为";
            //tbxExPath.Text = "";
            //if (sfdExportMap.ShowDialog() != DialogResult.Cancel)
            //{
            //    pSavePath = sfdExportMap.FileName;
            //    tbxExPath.Text = sfdExportMap.FileName;
            //}  
        }
       
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (tbxExPath.Text == "")
            {
                MessageBox.Show("请先确定导出路径!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (cmbResolution.Text == "")
            {
                if (tbxExPath.Text == "")
                {
                    MessageBox.Show("请输入分辨率！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else if (Convert.ToInt16(cmbResolution.Text) == 0)
            {
                MessageBox.Show("请正确输入分辨率！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                try
                {
                    int resolution = int.Parse(cmbResolution.Text);  //输出图片的分辨率
                    int width = int.Parse(tbxWidth.Text);            //输出图片的宽度，以像素为单位
                    int height = int.Parse(tbxHeight.Text);          //输出图片的高度，以像素为单位                   
                    ExportMap.ExportView(pActiveView, pGeometry, resolution, width, height, pSavePath, bRegion);
                    pActiveView.GraphicsContainer.DeleteAllElements();
                    pActiveView.Refresh();                                     
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("导出失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //局部导出时没有导出图像就退出
            pActiveView.GraphicsContainer.DeleteAllElements();
            pActiveView.Refresh();
            Dispose();
        }

        private void MapExportForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //局部导出时没有导出图像就关闭
            pActiveView.GraphicsContainer.DeleteAllElements();
            pActiveView.Refresh();
            Dispose();
        }

        private void btnExPath_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdExportMap = new SaveFileDialog();
            sfdExportMap.DefaultExt = "jpg|bmp|gig|tif|png|pdf";
            sfdExportMap.Filter = "JPGE 文件(*.jpg)|*.jpg|BMP 文件(*.bmp)|*.bmp|GIF 文件(*.gif)|*.gif|TIF 文件(*.tif)|*.tif|PNG 文件(*.png)|*.png|PDF 文件(*.pdf)|*.pdf";
            sfdExportMap.OverwritePrompt = true;
            sfdExportMap.Title = "保存为";
            tbxExPath.Text = "";
            if (sfdExportMap.ShowDialog() != DialogResult.Cancel)
            {
                pSavePath = sfdExportMap.FileName;
                tbxExPath.Text = sfdExportMap.FileName;
            }  
        }

      
    }
}
