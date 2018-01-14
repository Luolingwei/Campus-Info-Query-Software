using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;


namespace Whu038
{
    public partial class FrmAttribute : Form
    {
        public FrmAttribute()
        {
            InitializeComponent();
        }

        public FrmAttribute(ILayer pLyr)
        {
            InitializeComponent();
            pLayer = pLyr;
        }

        private ILayer pLayer;//打开属性表的图层
        private IFeatureLayer pFeatureLayer;
        private IFeatureClass pFeatureClass;
        private ILayerFields pLayerFields;

        private void FrmAttribute_Load(object sender, EventArgs e)
        {
            try
            {
                string tableName;
                tableName = getValidFeatureClassName(pLayer.Name);
                this.Text = tableName + "属性表".ToString();//替换窗体名称
                pFeatureLayer = pLayer as IFeatureLayer;
                pFeatureClass = pFeatureLayer.FeatureClass;
                pLayerFields = pFeatureLayer as ILayerFields;
                DataSet ds = new DataSet("dsTest");
                DataTable dt = new DataTable(pFeatureLayer.Name);
                DataColumn dc = null;
                for (int i = 0; i < pLayerFields.FieldCount; i++)
                {
                    dc = new DataColumn(pLayerFields.get_Field(i).Name);
                    dt.Columns.Add(dc);
                    dc = null;
                }
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < pLayerFields.FieldCount; j++)
                    {
                        if (pLayerFields.FindField(pFeatureClass.ShapeFieldName) == j)
                        {
                            dr[j] = pFeatureClass.ShapeType.ToString();
                        }
                        else
                        {
                            dr[j] = pFeature.get_Value(j);
                        }
                    }
                    dt.Rows.Add(dr);
                    pFeature = pFeatureCursor.NextFeature();
                }
                dataGridView1.DataSource = dt;
            }
            catch (Exception exc)
            {
                MessageBox.Show("读取属性表失败：" + exc.Message);
                this.Dispose();
            }
        
                 
        }
        public static string getValidFeatureClassName(string FCname)
        {
            int dot = FCname.IndexOf(".");
            if (dot != -1)
            {
                return FCname.Replace(".", "_");
            }
            return FCname;
        }






    }
}
