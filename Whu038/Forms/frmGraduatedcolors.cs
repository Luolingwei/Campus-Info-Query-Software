using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using Whu038;
using Whu038.Classes;
using Whu038.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;

namespace Whu038
{
    public partial class frmGraduatedcolors : Form
    {
        List<IFeatureClass> _lstFeatCls = null;
        public delegate void GraduatedcolorsEventHandler(string sFeatClsName, string sFieldName, int intnumclassess);
        public event GraduatedcolorsEventHandler Graduatedcolors = null;
        public frmGraduatedcolors()
        {
            InitializeComponent();
        }
        private IMap _map;
        public IMap Map
        {
            get { return _map; }
            set { _map = value; }
        }
        AxMapControl mapControl;
        AxTOCControl tocControl;
        public void InitUI()
        {
            string sClsName = string.Empty;
            cmbSelLyr.Items.Clear();
            IFeatureClass pFeatCls = null;
            OperateMap _OperateMap = new OperateMap();
            _lstFeatCls = _OperateMap.GetLstFeatCls(_map);
            for (int i = 0; i < _lstFeatCls.Count; i++)
            {
                pFeatCls = _lstFeatCls[i];
                sClsName = pFeatCls.AliasName;
                if (!cmbSelLyr.Items.Contains(sClsName))
                {
                    cmbSelLyr.Items.Add(sClsName);
                }
            }
        }
        public void GetMap(AxMapControl mapControl,AxTOCControl tocControl)
        {
            this.mapControl = mapControl;
            this.tocControl = tocControl;
        }
        private bool check()
        {
            if (cmbSelLyr.SelectedIndex == -1)
            {
                MessageBox.Show("请选择符号化图层！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (cmbSelField.SelectedIndex == -1)
            {
                MessageBox.Show("请选择符号化字段！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (cmbnumclasses.SelectedIndex == -1)
            {
                MessageBox.Show("请选择颜色分级数目！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        private IFeatureClass GetFeatClsByName(string sFeatClsName)
        {
            IFeatureClass pFeatCls = null;
            for (int i = 0; i < _lstFeatCls.Count; i++)
            {
                pFeatCls = _lstFeatCls[i];
                if (pFeatCls.AliasName == sFeatClsName)
                {
                    break;
                }
            }
            return pFeatCls;
        }

        void ClassifyColor()
        {
            IGeoFeatureLayer geoFeatureLayer = mapControl.Map.get_Layer(17) as IGeoFeatureLayer;
            string a = geoFeatureLayer.Name;
            object dataFrequency, dataValues;


            ITableHistogram tableHistogram = new BasicTableHistogramClass();
            IBasicHistogram basicHistogram = (IBasicHistogram)tableHistogram;
            tableHistogram.Field = cmbSelField.Text;
            tableHistogram.Table = geoFeatureLayer.FeatureClass as ITable;



            basicHistogram.GetHistogram(out dataValues, out dataFrequency);
            IClassifyGEN classifyGEN = new EqualIntervalClass();
            classifyGEN.Classify(dataValues, dataFrequency, Convert.ToInt32(cmbnumclasses.Text));


            double[] classes = classifyGEN.ClassBreaks as double[];
            int classesCount = classes.GetUpperBound(0);
            IClassBreaksRenderer classBreaksRenderer = new ClassBreaksRendererClass();



            classBreaksRenderer.Field = cmbSelField.Text;
            classBreaksRenderer.BreakCount = classesCount;
            classBreaksRenderer.SortClassesAscending = true;



            IHsvColor fromColor = new HsvColorClass();
            fromColor.Hue = 0; fromColor.Saturation = 50;
            fromColor.Value = 96;
            IHsvColor toColor = new HsvColorClass();
            toColor.Hue = 80;
            toColor.Saturation = 100;
            toColor.Value = 96;
            bool ok;



            //产生色带
            IAlgorithmicColorRamp algorithmicCR = new AlgorithmicColorRampClass();
            algorithmicCR.Algorithm = esriColorRampAlgorithm.esriHSVAlgorithm;
            algorithmicCR.FromColor = fromColor;
            algorithmicCR.ToColor = toColor;
            algorithmicCR.Size = classesCount;
            algorithmicCR.CreateRamp(out ok);



            //获得颜色
            IEnumColors enumColors = algorithmicCR.Colors;
            for (int breakIndex = 0; breakIndex <= classesCount - 1; breakIndex++)
            {
                IColor color = enumColors.Next();
                ISimpleFillSymbol simpleFill = new SimpleFillSymbolClass();
                simpleFill.Color = color;
                simpleFill.Style = esriSimpleFillStyle.esriSFSSolid;
                classBreaksRenderer.set_Symbol(breakIndex, (ISymbol)simpleFill);
                classBreaksRenderer.set_Break(breakIndex, classes[breakIndex + 1]);
            }

            geoFeatureLayer.Renderer = (IFeatureRenderer)classBreaksRenderer;
            mapControl.Refresh();
            mapControl.ActiveView.Refresh();
             

            tocControl.Update();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            ClassifyColor();
            Close();           
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }

  
        private void cmbSelLyr_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbSelField.Items.Clear();
            cmbSelField.Text = "";
            IField pField = null;
            IFeatureClass pFeatCls = GetFeatClsByName(cmbSelLyr.SelectedItem.ToString());
            for (int i = 0; i < pFeatCls.Fields.FieldCount; i++)
            {
                pField = pFeatCls.Fields.get_Field(i);
                if (pField.Type == esriFieldType.esriFieldTypeDouble ||
                    pField.Type == esriFieldType.esriFieldTypeInteger ||
                    pField.Type == esriFieldType.esriFieldTypeSingle ||
                    pField.Type == esriFieldType.esriFieldTypeSmallInteger)
                {
                    if (!cmbSelField.Items.Contains(pField.Name))
                    {
                        cmbSelField.Items.Add(pField.Name);
                    }
                }
            }
            for (int j = 0; j <= 10; j++)
            {
                cmbnumclasses.Items.Add(j);
            }
        }

  
    }
}
