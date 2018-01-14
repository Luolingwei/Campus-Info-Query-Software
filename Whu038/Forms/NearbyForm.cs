using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geometry;
using Whu038.Forms;

namespace Whu038.Forms
{
    public partial class NearbyForm : Form
    {              
        MyEngine mainForm;
        AxMapControl mapControl;
        public NearbyForm(MyEngine mainForm, AxMapControl mapControl)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.mapControl = mapControl;
        }
     
        private void btnOK_Click(object sender, EventArgs e)
        {
            mainForm.mTool = "NearbyQuery";
            this.Dispose();         
        }

        private void cmbLayer_MouseClick(object sender, MouseEventArgs e)
        {
            cmbLayer.Items.Clear();
            for (int i = 0; i < mapControl.Map.LayerCount; i++)
            {
                this.cmbLayer.Items.Add(mapControl.Map.get_Layer(i).Name);
            }
        }

        private void cmbLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            mainForm.selectLayer = mapControl.Map.get_Layer(this.cmbLayer.SelectedIndex) as IFeatureLayer;
        }

        private void tbxRadius_TextChanged(object sender, EventArgs e)
        {
            mainForm.selectRadius = Convert.ToDouble(tbxRadius.Text);
        }

       



       

    

 
   }
}




    
