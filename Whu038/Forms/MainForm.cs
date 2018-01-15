using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using stdole;

using Whu038.Classes;
using Whu038.Forms;

namespace Whu038
{
    public partial class MyEngine : Form
    {
        private MapView m_MapView ;
        public MyEngine()
        {
            //Login login = new Login();
            //login.ShowDialog();
            //if (login.DialogResult == DialogResult.OK)
            //{
            //    InitializeComponent();
            //}
            InitializeComponent();
        }

        public class Datal
        {
            public string qs;
            public string zcpj;
        }
        public class Datad
        {
            public string qs;
            public double fenzhi;
            //public double hd;
            //public double qml;
            public double x;
            public double y;
        }

        //获取RGB颜色 默认为我自己的地图底色
        public IColor getRGB(int r = 245, int g = 235, int b = 186)
        {
            IRgbColor pColor = new RgbColorClass();

            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor as IColor;
        }

        //获得图层by名称
        public IGeoFeatureLayer getGeoLayer(string layerName)
        {
            if (null == axMapControl1)
                return null;
            ILayer layer;
            IGeoFeatureLayer geoFeatureLayer;
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                layer = axMapControl1.get_Layer(i);
                if (layer != null && layer.Name== layerName)
                {
                    geoFeatureLayer = layer as IGeoFeatureLayer;
                    return geoFeatureLayer;
                }
            }
            return null;
        }

        /* 随机生成填充颜色组
       * @count 待生成颜色数量
       */
        static public IColorRamp CreateRandomColorRamp(int count)
        {
            
            IEnumColors pEnumRamp;
            IRandomColorRamp pColorRamp;
           
            pColorRamp = new RandomColorRampClass();
            pColorRamp.StartHue = 0;
            pColorRamp.MinValue = 99;
            pColorRamp.MinSaturation = 15;
            pColorRamp.EndHue = 360;
            pColorRamp.MaxValue = 100;
            pColorRamp.MaxSaturation = 30;
            pColorRamp.Size = count * 2;

            bool ok = true;
            pColorRamp.CreateRamp(out ok);
            pEnumRamp = pColorRamp.Colors;
            return pColorRamp;
        }




        private ILayer pGlobalFeatureLayer = null;    //获取当前图层
        private IPoint m_PointPt = null;
        private IPoint m_MovePt = null;

        private string pMapUnits = null;    //地图坐标单位
        private Pan pan = null;

        //空间查询方式
        private int mQueryMode;
        //图层索引
        private int mLayerIndex;

        //几何网络
        private IGeometricNetwork mGeometricNetwork;
        //给定点的集合
        private IPointCollection mPointCollection;
        //获取给定点最近的Network元素
        private IPointToEID mPointToEID;

        //返回结果变量
        private IEnumNetEID mEnumNetEID_Junctions;
        private IEnumNetEID mEnumNetEID_Edges;
        private double mdblPathCost;

        public int caozuo = 0; 
        public IFeatureWorkspace pFWorkspace;
        private EnumChartRenderType _enumChartType = EnumChartRenderType.UnKnown;       
        string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        private void OpenMxd_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenMXD = new OpenFileDialog(); //可实例化类
            // Gets or sets the file dialog box title. (Inherited from FileDialog.)
            OpenMXD.Title = "打开地图"; // OpenFileDialog类的属性Title
            // Gets or sets the initial directory displayed by the file dialog box. 
            OpenMXD.InitialDirectory = "C:";
            // Gets or sets the current file name filter string ,Save as file type
            OpenMXD.Filter = "Map Documents (*.mxd)|*.mxd";
            if (OpenMXD.ShowDialog() == DialogResult.OK) //ShowDialog是类的方法
            {
                //FileName:Gets or sets a string containing the file name selected in the file dialog box
                string MxdPath = OpenMXD.FileName;
                axMapControl1.LoadMxFile(MxdPath); //IMapControl2的方法
            }
        }

        private void MenuAddSHP_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenShpFile = new OpenFileDialog();
            OpenShpFile.Title = "打开Shape文件";
            OpenShpFile.InitialDirectory = "C:";
            OpenShpFile.Filter = "Shape文件(*.shp)|*.shp";
            if (OpenShpFile.ShowDialog() == DialogResult.OK)
            {
                string ShapPath = OpenShpFile.FileName;
                int Position = ShapPath.LastIndexOf("\\"); //利用"\\"将文件路径分成两部分
                string FilePath = ShapPath.Substring(0, Position);
                string ShpName = ShapPath.Substring(Position + 1);
                axMapControl1.AddShapeFile(FilePath, ShpName);
            }
        }

        private void MenuAddLyr_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenLyrFile = new OpenFileDialog();
            OpenLyrFile.Title = "打开Lyr";
            OpenLyrFile.InitialDirectory = "C:";
            OpenLyrFile.Filter = "lyr文件(*.lyr)|*.lyr";
            if (OpenLyrFile.ShowDialog() == DialogResult.OK)
            {
                string LayPath = OpenLyrFile.FileName;
                axMapControl1.AddLayerFromFile(LayPath);
            }
        }

        private void axMapControl1_OnExtentUpdated(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            try
            {
                // 得到新范围
                IEnvelope pEnvelope = (IEnvelope)e.newEnvelope;
                IGraphicsContainer pGraphicsContainer = axMapControl2.Map as IGraphicsContainer;
                IActiveView pActiveView = pGraphicsContainer as IActiveView;
                //在绘制前，清除axMapControl2中的任何图形元素
                pGraphicsContainer.DeleteAllElements();
                IRectangleElement pRectangleEle = new RectangleElementClass();
                IElement pElement = pRectangleEle as IElement;
                pElement.Geometry = pEnvelope;
                //设置鹰眼图中的红线框
                IRgbColor pColor = new RgbColorClass();
                pColor.Red = 255;
                pColor.Green = 0;
                pColor.Blue = 0;
                pColor.Transparency = 255;
                //产生一个线符号对象
                ILineSymbol pOutline = new SimpleLineSymbolClass();
                pOutline.Width = 3;
                pOutline.Color = pColor;
                //设置颜色属性
                pColor = new RgbColorClass();
                pColor.Red = 255;
                pColor.Green = 0;
                pColor.Blue = 0;
                pColor.Transparency = 0;
                //设置填充符号的属性
                IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                pFillSymbol.Color = pColor;
                pFillSymbol.Outline = pOutline;
                IFillShapeElement pFillShapeEle = pElement as IFillShapeElement;
                pFillShapeEle.Symbol = pFillSymbol;
                pGraphicsContainer.AddElement((IElement)pFillShapeEle, 0);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            catch { };
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            if (axMapControl1.LayerCount > 0)
            {
                axMapControl2.Map = new MapClass();
                for (int i = 0; i <= axMapControl1.Map.LayerCount - 1; i++)
                {
                    axMapControl2.AddLayer(axMapControl1.get_Layer(i));
                }
                axMapControl2.Extent = axMapControl1.Extent;
                axMapControl2.Refresh();
            }

            CopyMapFromMapControlToPageLayoutControl();//调用地图复制函数


            #region 坐标单位替换
            esriUnits mapUnits = axMapControl1.MapUnits;
            switch (mapUnits)
            {
                case esriUnits.esriCentimeters:
                    pMapUnits = "Centimeters";
                    break;
                case esriUnits.esriDecimalDegrees:
                    pMapUnits = "Decimal Degrees";
                    break;
                case esriUnits.esriDecimeters:
                    pMapUnits = "Decimeters";
                    break;
                case esriUnits.esriFeet:
                    pMapUnits = "Feet";
                    break;
                case esriUnits.esriInches:
                    pMapUnits = "Inches";
                    break;
                case esriUnits.esriKilometers:
                    pMapUnits = "Kilometers";
                    break;
                case esriUnits.esriMeters:
                    pMapUnits = "Meters";
                    break;
                case esriUnits.esriMiles:
                    pMapUnits = "Miles";
                    break;
                case esriUnits.esriMillimeters:
                    pMapUnits = "Millimeters";
                    break;
                case esriUnits.esriNauticalMiles:
                    pMapUnits = "NauticalMiles";
                    break;
                case esriUnits.esriPoints:
                    pMapUnits = "Points";
                    break;
                case esriUnits.esriUnknownUnits:
                    pMapUnits = "Unknown";
                    break;
                case esriUnits.esriYards:
                    pMapUnits = "Yards";
                    break;
            }
            #endregion


        }

        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button == 1)
            {
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(e.mapX, e.mapY);
                axMapControl1.CenterAt(pPoint);
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }

        }

        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (axMapControl2.Map.LayerCount > 0)
            {
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    axMapControl1.CenterAt(pPoint);
                    axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                else if (e.button == 2)
                {
                    IEnvelope pEnv = axMapControl2.TrackRectangle();
                    axMapControl1.Extent = pEnv;
                    axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }


        }

        private IElement DrawLineSymbol(IGeometry pGeometry, IRgbColor pColor)
        {
            IElement element = null;
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Color = pColor;
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Width = 1;
            ILineElement lineElement = new ESRI.ArcGIS.Carto.LineElementClass();
            lineElement.Symbol = simpleLineSymbol;
            element = (IElement)lineElement;
            element.Geometry = pGeometry;
            return element;
        }


        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            if (axMapControl1.LayerCount > 0)
            {
                esriTOCControlItem pItem = new esriTOCControlItem();
                pGlobalFeatureLayer = new FeatureLayerClass();
                IBasicMap pBasicMap = new MapClass();
                object pOther = new object();
                object pIndex = new object();
                // Returns the item in the TOCControl at the specified coordinates.
                axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pBasicMap, ref pGlobalFeatureLayer, ref pOther, ref pIndex);
            }//TOCControl类的ITOCControl接口的HitTest方法
            if (e.button == 2)
            {
                contextMenuStrip1.Show(axTOCControl1, e.x, e.y);
            }

        }

        private void 打开属性表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //传入图层，在右击事件里返回的图层
            // FrmAttribute frm1 = new FrmAttribute(pLayer as IFeatureLayer);
            // frm1.Show();
            FrmAttribute Ft = new FrmAttribute(pGlobalFeatureLayer as IFeatureLayer);
            Ft.Show();

        }

        //将MapControl中的地图复制到PageLayoutControl中去
        private void CopyMapFromMapControlToPageLayoutControl()
        {
            try
            {
                //获得IObjectCopy接口
                IObjectCopy pObjectCopy = new ObjectCopyClass();
                //获得要拷贝的图层 
                System.Object pSourceMap = axMapControl1.Map;
                //获得拷贝图层
                System.Object pCopiedMap = pObjectCopy.Copy(pSourceMap);
                //获得要重绘的地图 
                System.Object pOverwritedMap = axPageLayoutControl1.ActiveView.FocusMap;
                //重绘pagelayout地图
                pObjectCopy.Overwrite(pCopiedMap, ref pOverwritedMap);
            }

            catch { }
        }

        private void axMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            //获得IActiveView接口
            IActiveView pPageLayoutView = (IActiveView)axPageLayoutControl1.ActiveView.FocusMap;
            //获得IDisplayTransformation接口
            IDisplayTransformation pDisplayTransformation = pPageLayoutView.ScreenDisplay.DisplayTransformation;
            //设置可视范围
            pDisplayTransformation.VisibleBounds = axMapControl1.Extent;
            axPageLayoutControl1.ActiveView.Refresh(); //刷新地图
            //根据MapControl的视图范围，确定PageLayoutControl的视图范围             CopyMapFromMapControlToPageLayoutControl();

        }

        private void axMapControl1_OnViewRefreshed(object sender, IMapControlEvents2_OnViewRefreshedEvent e)
        {
            axTOCControl1.Refresh();
            CopyMapFromMapControlToPageLayoutControl();
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            //漫游（BaseTool方法）
            if (pan != null)
                pan.OnMouseMove(e.button, e.shift, e.x, e.y);

            // 取得鼠标所在工具的索引号  
            int index = axToolbarControl1.HitTest(e.x, e.y, false);
            if (index != -1)
            {
                // 取得鼠标所在工具的 ToolbarItem  
                IToolbarItem toolbarItem = axToolbarControl1.GetItem(index);
                // 设置状态栏信息  
                MessageLabel.Text = toolbarItem.Command.Message;
            }
            else
            {
                MessageLabel.Text = " 就绪 ";
            }
            // 显示当前比例尺
            ScaleLabel.Text = " 比例尺 1:" + ((long)this.axMapControl1.MapScale).ToString();
            // 显示当前坐标
            CoordinateLabel.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits;

            CoordinateLabel.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits.ToString();
        }

        public ToolStripMenuItem menuView { get; set; }

        //放大
        private void menuZoomIn_Click(object sender, EventArgs e)
        {
            //调用封装的工具类实现
            ESRI.ArcGIS.SystemUI.ITool tool = new ControlsMapZoomInToolClass();
            ESRI.ArcGIS.SystemUI.ICommand command = tool as ESRI.ArcGIS.SystemUI.ICommand;
            command.OnCreate(this.axMapControl1.Object);
            this.axMapControl1.CurrentTool = tool;
        }

        //缩小
        private void menuZoomOut_Click(object sender, EventArgs e)
        {
            //调用封装的工具类实现
            ESRI.ArcGIS.SystemUI.ITool tool = new ControlsMapZoomOutToolClass();
            ESRI.ArcGIS.SystemUI.ICommand cmd = tool as ESRI.ArcGIS.SystemUI.ICommand;
            cmd.OnCreate(this.axMapControl1.Object);
            cmd.OnClick();
            this.axMapControl1.CurrentTool = tool;
        }

        //中心放大
        private void menuFixedZoomIn_Click(object sender, EventArgs e)
        {
            //声明与初始化 
            FixedZoomIn fixedZoomin = new FixedZoomIn();
            //与MapControl关联 
            fixedZoomin.OnCreate(this.axMapControl1.Object);
            fixedZoomin.OnClick();
        }

        //中心缩小
        private void menuFixedZoomOut_Click(object sender, EventArgs e)
        {
            ICommand command = new ControlsMapZoomOutFixedCommandClass();
            command.OnCreate(this.axMapControl1.Object);
            command.OnClick();
        }

        //漫游
        private void menuPan_Click(object sender, EventArgs e)
        {
            //调用封装的工具类实现
            ESRI.ArcGIS.SystemUI.ITool tool = new ControlsMapPanToolClass();
            ESRI.ArcGIS.SystemUI.ICommand cmd = tool as ESRI.ArcGIS.SystemUI.ICommand;
            cmd.OnCreate(this.axMapControl1.Object);
            this.axMapControl1.CurrentTool = tool;
        }

        //全图显示
        private void menuFullExtent_Click(object sender, EventArgs e)
        {
            ICommand command = new ControlsMapFullExtentCommandClass();
            command.OnCreate(this.axMapControl1.Object);
            command.OnClick();
        }

        public double selectRadius;
        public IFeatureLayer selectLayer;
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (pan != null)
                pan.OnMouseDown(e.button, e.shift, e.x, e.y);

            this.axMapControl1.Map.ClearSelection();
            //获取当前视图
            IActiveView pActiveView = this.axMapControl1.ActiveView;
            //获取鼠标点
            IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            switch (mTool)
            {

                case "NetWork":
                    //记录鼠标点击的点
                    IPoint pNewPoint = new PointClass();
                    pNewPoint.PutCoords(e.mapX, e.mapY);

                    if (mPointCollection == null)
                        mPointCollection = new MultipointClass();
                    //添加点，before和after标记添加点的索引，这里不定义
                    object before = Type.Missing;
                    object after = Type.Missing;
                    mPointCollection.AddPoint(pNewPoint, ref before, ref after);
                    break;

                case "SpaceQuery":
                    IGeometry pGeometry = null;
                    if (this.mQueryMode == 0)//矩形查询
                    {
                        pGeometry = this.axMapControl1.TrackRectangle();
                    }
                    else if (this.mQueryMode == 1)//线查询
                    {
                        pGeometry = this.axMapControl1.TrackLine();
                    }
                    else if (this.mQueryMode == 2)//点查询
                    {
                        ITopologicalOperator pTopo;
                        IGeometry pBuffer;
                        pGeometry = pPoint;
                        pTopo = pGeometry as ITopologicalOperator;
                        //根据点位创建缓冲区，缓冲半径为0.1，可修改
                        pBuffer = pTopo.Buffer(0.1);
                        pGeometry = pBuffer.Envelope;
                    }
                    else if (this.mQueryMode == 3)//圆查询
                    {
                        pGeometry = this.axMapControl1.TrackCircle();
                    }

                    IFeatureLayer pFeatureLayer = this.axMapControl1.get_Layer(this.mLayerIndex) as IFeatureLayer;
                    DataTable pDataTable = this.LoadQueryResult(this.axMapControl1, pFeatureLayer, pGeometry);
                    this.dataGridView1.DataSource = pDataTable.DefaultView;
                    this.dataGridView1.Refresh();
                    break;

                case "NearbyQuery":
                    {
                        ITopologicalOperator pTopo;
                        IGeometry pBuffer;
                        pGeometry = pPoint;
                        pTopo = pGeometry as ITopologicalOperator;
                        //根据点位创建缓冲区，缓冲半径可修改
                        pBuffer = pTopo.Buffer(selectRadius);
                        pGeometry = pBuffer.Envelope;
                    }

                    IFeatureLayer nFeatureLayer = selectLayer;
                    DataTable nDataTable = this.LoadQueryResult(this.axMapControl1, nFeatureLayer, pGeometry);
                    this.dataGridView1.DataSource = nDataTable.DefaultView;
                    this.dataGridView1.Refresh();

                    break;

                case "ChooseQuery":
                    {
                        pGeometry = pPoint;
                    }
                   
                    IFeatureLayer mFeatureLayer = this.axMapControl1.get_Layer(this.mLayerIndex) as IFeatureLayer;
                    DataTable mDataTable = this.LoadQueryResult(this.axMapControl1, mFeatureLayer, pGeometry);
                    this.dataGridView1.DataSource = mDataTable.DefaultView;
                    this.dataGridView1.Refresh();
                    break;
                    
                    
                 
                default:
                    break;
            }
        }

        private void axMapControl1_OnMouseUp_1(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            //漫游（BaseTool方法）
            if (pan != null)
                pan.OnMouseUp(e.button, e.shift, e.x, e.y);

        }

        private void 属性查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LocationQueryForm frmattributequery = new LocationQueryForm(this.axMapControl1);
            frmattributequery.Show();
        }

        private DataTable LoadQueryResult(AxMapControl mapControl, IFeatureLayer featureLayer, IGeometry geometry)
        {
            IFeatureClass pFeatureClass = featureLayer.FeatureClass;

            //根据图层属性字段初始化DataTable
            IFields pFields = pFeatureClass.Fields;
            DataTable pDataTable = new DataTable();
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                string strFldName;
                strFldName = pFields.get_Field(i).Name;
                pDataTable.Columns.Add(strFldName);
            }

            //空间过滤器
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = geometry;

            //根据图层类型选择缓冲方式
            switch (pFeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }

            //定义空间过滤器的空间字段
            pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;

            IQueryFilter pQueryFilter;
            IFeatureCursor pFeatureCursor;
            IFeature pFeature;
            //利用要素过滤器查询要素
            pQueryFilter = pSpatialFilter as IQueryFilter;
            pFeatureCursor = featureLayer.Search(pQueryFilter, true);
            pFeature = pFeatureCursor.NextFeature();

            while (pFeature != null)
            {
                string strFldValue = null;
                DataRow dr = pDataTable.NewRow();
                //遍历图层属性表字段值，并加入pDataTable
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    string strFldName = pFields.get_Field(i).Name;
                    if (strFldName == "SHAPE")
                    {
                        strFldValue = Convert.ToString(pFeature.Shape.GeometryType);
                    }
                    else
                        strFldValue = Convert.ToString(pFeature.get_Value(i));
                    dr[i] = strFldValue;
                }
                pDataTable.Rows.Add(dr);
                //高亮选择要素
                mapControl.Map.SelectFeature((ILayer)featureLayer, pFeature);
                mapControl.ActiveView.Refresh();
                pFeature = pFeatureCursor.NextFeature();
            }
            return pDataTable;
        }



        public string mTool { get; set; }

        private void 开始分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pCommand;
            pCommand = new ShortPathSolveCommand();
            pCommand.OnCreate(axMapControl1.Object);
            pCommand.OnClick();
            pCommand = null;
        }

        private void 清除分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMapControl1.CurrentTool = null;
            try
            {
                string name = NetWorkAnalysClass.getPath(path) + "\\data\\新建文件地理数据库.gdb";
                //打开工作空间
                pFWorkspace = NetWorkAnalysClass.OpenWorkspace(name) as IFeatureWorkspace;
                IGraphicsContainer pGrap = this.axMapControl1.ActiveView as IGraphicsContainer;
                pGrap.DeleteAllElements();//删除所添加的图片要素
                IFeatureClass inputFClass = pFWorkspace.OpenFeatureClass("Stops");
                //删除站点要素
                if (inputFClass.FeatureCount(null) > 0)
                {
                    ITable pTable = inputFClass as ITable;
                    pTable.DeleteSearchedRows(null);
                }
                IFeatureClass barriesFClass = pFWorkspace.OpenFeatureClass("Barries");//删除障碍点要素
                if (barriesFClass.FeatureCount(null) > 0)
                {
                    ITable pTable = barriesFClass as ITable;
                    pTable.DeleteSearchedRows(null);
                }
                for (int i = 0; i < axMapControl1.LayerCount; i++)//删除分析结果
                {
                    ILayer pLayer = axMapControl1.get_Layer(i);
                    if (pLayer.Name == ShortPathSolveCommand.m_NAContext.Solver.DisplayName)
                    {
                        axMapControl1.DeleteLayer(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.axMapControl1.Refresh();
        }

        private void 网络分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //路径分析的实现
        private void SolvePath(string weightName)
        {
            //创建ITraceFlowSolverGEN
            ITraceFlowSolverGEN pTraceFlowSolverGEN = new TraceFlowSolverClass();
            INetSolver pNetSolver = pTraceFlowSolverGEN as INetSolver;
            //初始化用于路径计算的Network
            INetwork pNetWork = mGeometricNetwork.Network;
            pNetSolver.SourceNetwork = pNetWork;

            //获取分析经过的点的个数
            int intCount = mPointCollection.PointCount;
            if (intCount < 1)
                return;

            INetFlag pNetFlag;
            //用于存储路径计算得到的边
            IEdgeFlag[] pEdgeFlags = new IEdgeFlag[intCount];

            IPoint pEdgePoint = new PointClass();
            int intEdgeEID;
            IPoint pFoundEdgePoint;
            double dblEdgePercent;

            //用于获取几何网络元素的UserID, UserClassID,UserSubID
            INetElements pNetElements = pNetWork as INetElements;
            int intEdgeUserClassID;
            int intEdgeUserID;
            int intEdgeUserSubID;
            for (int i = 0; i < intCount; i++)
            {
                pNetFlag = new EdgeFlagClass();
                //获取用户点击点
                pEdgePoint = mPointCollection.get_Point(i);
                //获取距离用户点击点最近的边
                mPointToEID.GetNearestEdge(pEdgePoint, out intEdgeEID, out pFoundEdgePoint, out dblEdgePercent);
                if (intEdgeEID <= 0)
                    continue;
                //根据得到的边查询对应的几何网络中的元素UserID, UserClassID,UserSubID
                pNetElements.QueryIDs(intEdgeEID, esriElementType.esriETEdge,
                    out intEdgeUserClassID, out intEdgeUserID, out intEdgeUserSubID);
                if (intEdgeUserClassID <= 0 || intEdgeUserID <= 0)
                    continue;

                pNetFlag.UserClassID = intEdgeUserClassID;
                pNetFlag.UserID = intEdgeUserID;
                pNetFlag.UserSubID = intEdgeUserSubID;
                pEdgeFlags[i] = pNetFlag as IEdgeFlag;
            }
            //设置路径求解的边
            pTraceFlowSolverGEN.PutEdgeOrigins(ref pEdgeFlags);

            //路径计算权重
            INetSchema pNetSchema = pNetWork as INetSchema;
            INetWeight pNetWeight = pNetSchema.get_WeightByName(weightName);
            if (pNetWeight == null)
                return;

            //设置权重，这里双向的权重设为一致
            INetSolverWeights pNetSolverWeights = pTraceFlowSolverGEN as INetSolverWeights;
            pNetSolverWeights.ToFromEdgeWeight = pNetWeight;
            pNetSolverWeights.FromToEdgeWeight = pNetWeight;

            object[] arrResults = new object[intCount - 1];
            //执行路径计算
            pTraceFlowSolverGEN.FindPath(esriFlowMethod.esriFMConnected, esriShortestPathObjFn.esriSPObjFnMinSum,
                out mEnumNetEID_Junctions, out mEnumNetEID_Edges, intCount - 1, ref arrResults);

            //获取路径计算总代价（cost）
            mdblPathCost = 0;
            for (int i = 0; i < intCount - 1; i++)
                mdblPathCost += (double)arrResults[i];
        }

        //路径分析结果到几何要素的转换，在地图上显示
        private IPolyline PathToPolyLine()
        {
            IPolyline pPolyLine = new PolylineClass();
            IGeometryCollection pNewGeometryCollection = pPolyLine as IGeometryCollection;
            if (mEnumNetEID_Edges == null)
                return null;

            IEIDHelper pEIDHelper = new EIDHelperClass();
            //获取几何网络
            pEIDHelper.GeometricNetwork = mGeometricNetwork;
            //获取地图空间参考
            ISpatialReference pSpatialReference = this.axMapControl1.Map.SpatialReference;
            pEIDHelper.OutputSpatialReference = pSpatialReference;
            pEIDHelper.ReturnGeometries = true;
            //根据边的ID获取边的信息
            IEnumEIDInfo pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(mEnumNetEID_Edges);
            int intCount = pEnumEIDInfo.Count;
            pEnumEIDInfo.Reset();

            IEIDInfo pEIDInfo;
            IGeometry pGeometry;
            for (int i = 0; i < intCount; i++)
            {
                pEIDInfo = pEnumEIDInfo.Next();
                //获取边的几何要素
                pGeometry = pEIDInfo.Geometry;
                pNewGeometryCollection.AddGeometryCollection((IGeometryCollection)pGeometry);
            }
            return pPolyLine;
        }



        private void MyEngine_Load(object sender, EventArgs e)
        {
            axMapControl1.LoadMxFile(@"C:\Users\asus\Desktop\GIS实习\实习二\LLW2.mxd");
            axMapControl1.Refresh();

            /*
            m_MapView = new MapView(this, axMapControl1);
            //获取几何网络文件路径
            //此路径为当前存储路径
            string strPath = @"C:\Users\asus\Desktop\GIS实习\实习二\新实习.mdb";
            //打开工作空间
            IWorkspaceFactory pWorkspaceFactory = new AccessWorkspaceFactory();
            IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(strPath, 0) as IFeatureWorkspace;
            //获取要素数据集
            //注意名称的设置要与上面创建保持一致
            
            IFeatureDataset pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset("Road");
            //获取network集合
            INetworkCollection pNetWorkCollection = pFeatureDataset as INetworkCollection;
            //d获取network的数量,为零时返回           

            //FeatureDataset可能包含多个network，我们获取指定的network
            //注意network的名称的设置要与上面创建保持一致
            mGeometricNetwork = pNetWorkCollection.get_GeometricNetworkByName("Road_Net");

            //将Network中的每个要素类作为一个图层加入地图控件
            IFeatureClassContainer pFeatClsContainer = mGeometricNetwork as IFeatureClassContainer;
            //获取要素类数量，为零时返回
            int intFeatClsCount = pFeatClsContainer.ClassCount;
            if (intFeatClsCount < 1)
                return;
            IFeatureClass pFeatureClass;
            IFeatureLayer pFeatureLayer;
            for (int i = 0; i < intFeatClsCount; i++)
            {
                //获取要素类
                pFeatureClass = pFeatClsContainer.get_Class(i);
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pFeatureClass.AliasName;
                //加入地图控件
                this.axMapControl1.AddLayer((ILayer)pFeatureLayer, 0);
            }

            //计算snap tolerance为图层最大宽度的1/100
            //获取图层数量
            int intLayerCount = this.axMapControl1.LayerCount;
            IGeoDataset pGeoDataset;
            IEnvelope pMaxEnvelope = new EnvelopeClass();
            for (int i = 0; i < intLayerCount; i++)
            {
                //获取图层
                pFeatureLayer = this.axMapControl1.get_Layer(i) as IFeatureLayer;
                pGeoDataset = pFeatureLayer as IGeoDataset;
                if (pGeoDataset != null)
                {
                    //通过Union获得较大图层范围
                    pMaxEnvelope.Union(pGeoDataset.Extent);
                }
            }
            double dblWidth = pMaxEnvelope.Width;
            double dblHeight = pMaxEnvelope.Height;
            double dblSnapTol;
            if (dblHeight < dblWidth)
                dblSnapTol = dblWidth * 0.01;
            else
                dblSnapTol = dblHeight * 0.01;

            //设置源地图，几何网络以及捕捉容差
            mPointToEID = new PointToEIDClass();
            mPointToEID.SourceMap = this.axMapControl1.Map;
            mPointToEID.GeometricNetwork = mGeometricNetwork;
            mPointToEID.SnapTolerance = dblSnapTol;

            #region load窗体时加入默认地图
            
            if (axMapControl1.LayerCount > 0)
            {
                axMapControl2.Map = new MapClass();
                for (int i = 0; i <= axMapControl1.Map.LayerCount - 1; i++)
                {
                    axMapControl2.AddLayer(axMapControl1.get_Layer(i));
                }
                axMapControl2.Extent = axMapControl1.Extent;
                axMapControl2.Refresh();
            }

            CopyMapFromMapControlToPageLayoutControl();//调用地图复制函数
            #endregion
            */
        }

        private void axMapControl1_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            try
            {
                //路径计算
                //注意权重名称与设置保持一致
                SolvePath("LENGTH");
                //路径转换为几何要素
                IPolyline pPolyLineResult = PathToPolyLine();
                //获取屏幕显示
                IActiveView pActiveView = this.axMapControl1.ActiveView;
                IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
                //设置显示符号
                ILineSymbol pLineSymbol = new CartographicLineSymbolClass();
                IRgbColor pColor = new RgbColorClass();
                pColor.Red = 255;
                pColor.Green = 0;
                pColor.Blue = 0;
                //设置线宽
                pLineSymbol.Width = 4;
                //设置颜色
                pLineSymbol.Color = pColor as IColor;
                //绘制线型符号
                pScreenDisplay.StartDrawing(0, 0);
                pScreenDisplay.SetSymbol((ISymbol)pLineSymbol);
                pScreenDisplay.DrawPolyline(pPolyLineResult);
                pScreenDisplay.FinishDrawing();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("路径分析出现错误:" + "\r\n" + ex.Message);
            }
            //点集设为空
            mPointCollection = null;
        }


        private void 缓冲区分袖ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            NearbyForm BufferAnalysis = new NearbyForm(this, axMapControl1);
            BufferAnalysis.Show();
        }

        private void 关于软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm Help = new HelpForm();
            Help.Show();
        }

        private void 信息查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //初始化空间查询窗体
            BoxSelectionQueryForm spatialQueryForm = new BoxSelectionQueryForm(this.axMapControl1);
            if (spatialQueryForm.ShowDialog() == DialogResult.OK)
            {
                //标记为“空间查询”
                this.mTool = "SpaceQuery";
                //获取查询方式和图层
                this.mQueryMode = spatialQueryForm.mQueryMode;
                this.mLayerIndex = spatialQueryForm.mLayerIndex;
                //定义鼠标形状
                this.axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerCrosshair;
            }
        }

        private void MapExport_Click(object sender, EventArgs e)
        {
            MapExportForm map = new MapExportForm(axMapControl1);
            map.GetGeometry=axMapControl1.ActiveView.Extent;
            map.Show(); 
        }

        private void ChooseItem_Click(object sender, EventArgs e)
        {
            this.mTool = "ChooseQuery";
            m_MapView = new MapView(axMapControl1);
            m_MapView.CurMapOperation = MapOperation.MapSelect;
        }

        private void ChooseItem_DoubleClick(object sender, EventArgs e)
        {
            this.mTool = null;
        }

        #region 统计
        private void Statistic_Click(object sender, EventArgs e)
        {
            
        }

        private void 选择统计ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatisticsForm statistic = new StatisticsForm(axMapControl1);
            statistic.Show();
        }
        
 
        private void 雷达图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            caozuo = 999;
        }

        private void 分区柱状图ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ArrayList DataStored = new ArrayList();
            ArrayList DataMax = new ArrayList();
            //MessageBox.Show("未能找到有效路径" + statisticqu.DataStore[0], "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning); 
            string path = "D:\\data.txt";
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] sArray = line.Split(',');
                Datad data = new Datad();
                data.qs = sArray[0];
                //MessageBox.Show("未能找到有效路径" + data.qs, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning); 
                data.fenzhi = double.Parse(sArray[1]);

                DataStored.Add(data);
                DataMax.Add(data.fenzhi);
            }
            //获取定位点
            //MapControl中没有图层时返回 
            if (axMapControl1.LayerCount <= 0)
                return;
            //获取MapControl中的全部图层名称，并加入ComboBox 
            //图层 
            ILayer pLayer;
            int index = 0;
            //图层名称 
            string strLayerName;
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                pLayer = axMapControl1.get_Layer(i);
                strLayerName = pLayer.Name;
                if (strLayerName == "地类图斑_新融合")
                {
                    index = i;
                }
            }
            //获取地类图斑图层所有属性数据
            IFeatureLayer mFeatureLayer;
            //根据所选择的图层查询得到的特征类
            IFeatureClass pFeatureClass = null;
            mFeatureLayer = axMapControl1.get_Layer(index) as IFeatureLayer;
            pFeatureClass = mFeatureLayer.FeatureClass;

            //绘制矩形，注意最大值的判断
            DataMax.Sort();
            DataMax.Reverse();
            for (int i = 0; i < DataStored.Count; i++)
            {
                Datad a = DataStored[i] as Datad;
                //获取定位点
                //MessageBox.Show("未能找到有效路径" + a.qs, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning); 
                Double x = new double();
                Double y = new double();
                index = pFeatureClass.Fields.FindField("Sheet1$.总分值");
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.SubFields = "Sheet1$.总分值";
                pQueryFilter.WhereClause = "地类图斑_新融合.name='" + a.qs + "'";
                IFeatureCursor featureCursor;
                featureCursor = pFeatureClass.Search(pQueryFilter, false);
                IFeature pFeature;
                pFeature = featureCursor.NextFeature();
                x = Double.Parse(pFeature.get_Value(index).ToString());


                //index = pFeatureClass.Fields.FindField("y");
                //pQueryFilter.SubFields = "y";
                //pQueryFilter.WhereClause = "权属名称='" + a.qs + "'";
                //featureCursor = pFeatureClass.Search(pQueryFilter, false);
                //pFeature = featureCursor.NextFeature();
                //y = Double.Parse(pFeature.get_Value(index).ToString());

                //画图啦
                IMap pMap;
                IActiveView pActiveView;
                pMap = axMapControl1.Map;
                pActiveView = pMap as IActiveView;

                ISegmentCollection pSegColl = new RingClass();
                ILine pLine = new LineClass();
                IPoint p1 = new PointClass();
                p1.PutCoords(x, y);
                IPoint p2 = new PointClass();
                p2.PutCoords(x + 300, y);
                double h = a.fenzhi * 1200 / double.Parse(DataMax[0].ToString());
                IPoint p3 = new PointClass();
                p3.PutCoords(x + 300, y + h);
                IPoint p4 = new PointClass();
                p4.PutCoords(x, y + h);
                pLine.PutCoords(p1, p2);
                object Missing1 = Type.Missing;
                object Missing2 = Type.Missing;
                pSegColl.AddSegment(pLine as ISegment, ref Missing1, ref Missing2);
                pLine = new LineClass();
                pLine.PutCoords(p2, p3);
                pSegColl.AddSegment(pLine as ISegment, ref Missing1, ref Missing2);
                pLine = new LineClass();
                pLine.PutCoords(p3, p4);
                pSegColl.AddSegment(pLine as ISegment, ref Missing1, ref Missing2);
                IRing pRing = pSegColl as IRing;
                pRing.Close();
                IGeometryCollection pPolygon = new PolygonClass();
                pPolygon.AddGeometry(pRing, ref Missing1, ref Missing2);
                ISimpleFillSymbol pSimpleFillsym = new SimpleFillSymbolClass();
                pSimpleFillsym.Style = esriSimpleFillStyle.esriSFSSolid;
                IRgbColor rGBColor = new RgbColorClass();
                rGBColor.Red = 191;
                rGBColor.Green = 255;
                rGBColor.Blue = 255;
                pSimpleFillsym.Color = rGBColor;
                IFillShapeElement pPolygonEle = new PolygonElementClass();
                pPolygonEle.Symbol = pSimpleFillsym;
                IElement pEle = pPolygonEle as IElement;
                pEle.Geometry = (IGeometry)pPolygon;
                IGraphicsContainer pContainer = pMap as IGraphicsContainer;
                pContainer.AddElement(pEle, 0);

            }
    }

        private void 柱状图ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            List<string> renderFields = new List<string>();
            renderFields.Add("地类图斑_新融合.fenzhi");
           
            //对应颜色
            List<IColor> renderColor = new List<IColor>();
            renderColor.Add(getRGB(1, 2, 3));
         

            IColor BgColor = getRGB();//背景颜色使用默认颜色（我的是255，）
            createBarChart("地类图斑_新融合", renderFields, renderColor, BgColor);
                      
        }
        private void 饼状图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> renderFields = new List<string>();            
            renderFields.Add("Sheet1$.居民地比例"); 
            renderFields.Add("Sheet1$.人口密度");
            renderFields.Add("Sheet1$.平均海拔");
            renderFields.Add("Sheet1$.平均坡度");
            renderFields.Add("Sheet1$.与公路距离");
            renderFields.Add("Sheet1$.坡向离散度");

            //对应颜色
            List<IColor> renderColor = new List<IColor>();
            renderColor.Add(getRGB(10, 200, 10));
            renderColor.Add(getRGB(50, 20, 10));
            renderColor.Add(getRGB(30, 20, 120));
            renderColor.Add(getRGB(43, 90, 40));
            renderColor.Add(getRGB(140, 10, 50));
            renderColor.Add(getRGB(50, 50, 50));


            IColor BgColor = getRGB();//背景颜色使用默认颜色（我的是255，）
            createPieChart("地类图斑_新融合", renderFields, renderColor, BgColor);
        }




        /*添加柱状图
        * @layerName图层名
        * @renderFields字段
        * @alias别名
        * @renderColor对应的填充颜色
        * @BgColor地图填充色
        */
        public void createBarChart(string layerName, List<string> renderFields, List<IColor> renderColor, IColor BgColor, List<string> alias = null)
        {
            IGeoFeatureLayer geoFeatureLayer;
            IFeatureLayer featureLayer;
            ITable table;

            geoFeatureLayer = getGeoLayer(layerName);
            featureLayer = geoFeatureLayer as IFeatureLayer;
            table = featureLayer as ITable;
            geoFeatureLayer.ScaleSymbols = true;
            IChartRenderer chartRenderer = new ChartRendererClass();
            IBarChartSymbol barChartSymbol = new BarChartSymbolClass();
            IRendererFields rendererFields = chartRenderer as IRendererFields;
            for (int i = 0; i < renderFields.Count; i++)
            {
                if (null != alias)
                    rendererFields.AddField(renderFields[i], alias[i]);
                else
                    rendererFields.AddField(renderFields[i], renderFields[i]);
            }

            //找到所有地物类中的最大值
            double maxValue = 0;
            ICursor cursor = table.Search(null, true);
            IRowBuffer rowBuffer = cursor.NextRow();
            while (rowBuffer != null)
            {
                for (int i = 0; i < renderFields.Count; i++)
                {
                    double fieldValue;
                    try
                    {
                        fieldValue = double.Parse(rowBuffer.get_Value(table.FindField(renderFields[i])).ToString());
                    }
                    catch (Exception ex)
                    {
                        fieldValue = 0;
                    }
                    maxValue = Math.Max(maxValue, fieldValue);
                }
                rowBuffer = cursor.NextRow();
            }

            barChartSymbol.Width = 5;
            IMarkerSymbol markerSymbol = barChartSymbol as IMarkerSymbol;
            markerSymbol.Size = 50;
            IChartSymbol chartSymbol = barChartSymbol as IChartSymbol;
            chartSymbol.MaxValue = maxValue;

            //添加渲染符号  
            ISymbolArray symbolArray = barChartSymbol as ISymbolArray;
            IFillSymbol fillSymbol;
            IColorRamp colors = CreateRandomColorRamp(renderFields.Count);//ramp舷梯
            for (int i = 0; i < renderFields.Count; i++)
            {
                fillSymbol = new SimpleFillSymbolClass();
                fillSymbol.Color = colors.get_Color(i);
                symbolArray.AddSymbol(fillSymbol as ISymbol);
            }
            //设置柱状图符号  
            chartRenderer.ChartSymbol = barChartSymbol as IChartSymbol;

            //行政区填充
            fillSymbol = new SimpleFillSymbolClass();
            fillSymbol.Color = getRGB(245, 235, 186);
            fillSymbol.Color.Transparency = 2;
            chartRenderer.BaseSymbol = fillSymbol as ISymbol;

            chartRenderer.UseOverposter = false;

            //创建图例  
            chartRenderer.CreateLegend();
            geoFeatureLayer.Renderer = chartRenderer as IFeatureRenderer;
            geoFeatureLayer.DisplayField = "地类图斑_新融合.OBJECTID";

            IActiveView pActiveView = axMapControl1.Map as IActiveView;
            pActiveView.Refresh();
            axTOCControl1.Update();
        }


        /*添加饼状图
         * @layerName要图层名
         * @renderFields字段
        * @renderColor对应的填充颜色
         * @BgColor地图填充色
         */
        public void createPieChart(string layerName, List<string> renderFields, List<IColor> renderColor, IColor BgColor)
        {
            IGeoFeatureLayer geoFeatureLayer;
            IFeatureLayer featureLayer;
            ITable table;
            ICursor cursor;
            IRowBuffer rowBuffer;

            //获取渲染图层
            geoFeatureLayer = getGeoLayer(layerName);
            featureLayer = geoFeatureLayer as IFeatureLayer;
            table = featureLayer as ITable;
            geoFeatureLayer.ScaleSymbols = true;
            IChartRenderer chartRenderer = new ChartRendererClass();
            IPieChartRenderer pieChartRenderer = chartRenderer as IPieChartRenderer;
            IRendererFields rendererFields = chartRenderer as IRendererFields;
            for (int i = 0; i < renderFields.Count; i++)
            {
                rendererFields.AddField(renderFields[i], renderFields[i]);
            }
            //获取渲染要素的最大值
            double fieldValue = 0.0, maxValue = 0.0;
            cursor = table.Search(null, true);
            rowBuffer = cursor.NextRow();
            while (rowBuffer != null)
            {
                for (int i = 0; i < renderFields.Count; i++)
                {
                    int index = table.FindField(renderFields[i]);
                    fieldValue = double.Parse(rowBuffer.get_Value(index).ToString());
                    if (fieldValue > maxValue)
                        maxValue = fieldValue;
                }
                rowBuffer = cursor.NextRow();
            }
            //设置饼图符号
            IPieChartSymbol pieChartSymbol = new PieChartSymbolClass();
            pieChartSymbol.Clockwise = true;
            pieChartSymbol.UseOutline = true;
            IChartSymbol chartSymbol = pieChartSymbol as IChartSymbol;
            chartSymbol.MaxValue = maxValue;
            ILineSymbol lineSymbol = new SimpleLineSymbolClass();
            lineSymbol.Color = getRGB(255, 192, 203);//默认和背景一致
            lineSymbol.Width = 1.5;
            pieChartSymbol.Outline = lineSymbol;
            IMarkerSymbol markerSymbol = pieChartSymbol as IMarkerSymbol;
            markerSymbol.Size = 30;

            //添加渲染符号
            ISymbolArray symbolArray = pieChartSymbol as ISymbolArray;
            IFillSymbol[] fillsymbol = new IFillSymbol[renderFields.Count];
            for (int i = 0; i < renderFields.Count; i++)
            {
                fillsymbol[i] = new SimpleFillSymbolClass();
                fillsymbol[i].Color = renderColor[i];
                symbolArray.AddSymbol(fillsymbol[i] as ISymbol);
            }

            //设置背景
            chartRenderer.ChartSymbol = pieChartSymbol as IChartSymbol;
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = BgColor;
            chartRenderer.BaseSymbol = pFillSymbol as ISymbol;
            chartRenderer.UseOverposter = false;
            //创建图例
            chartRenderer.CreateLegend();
            geoFeatureLayer.Renderer = chartRenderer as IFeatureRenderer;

            IActiveView pActiveView = axMapControl1.Map as IActiveView;
            pActiveView.Refresh();
            axTOCControl1.Update();
        }
        
        
        
        /*添加雷达图
        * @layerName图层名
        * @renderFields字段
        * @renderColor线条颜色
        * @BgColor雷达图背景色
        */
        public void creatRadarChart(string layerName, List<string> renderFields, IColor renderColor, IColor BgColor)
        {
            IGeoFeatureLayer geoFeatureLayer;
            IFeatureLayer featureLayer;
            IFeatureClass featureClass;//IFeatureClass 控制要访问的要素类
            IFeatureCursor featureCursor;//游标，用于访问要素
            IFeature feature;//每一个要素
            ITable table;

            IGraphicsContainer pGraContainer = axMapControl1.Map as IGraphicsContainer;

            geoFeatureLayer = getGeoLayer(layerName);
            if (null == geoFeatureLayer)
                return;
            featureLayer = geoFeatureLayer as IFeatureLayer;
            table = featureLayer as ITable;

            //找到所有地物类中所有字段的最大值
            double maxValue = 0;
            ICursor cursor = table.Search(null, true);
            IRowBuffer rowBuffer = cursor.NextRow();
            while (rowBuffer != null)
            {
                for (int i = 0; i < renderFields.Count; i++)
                {
                    double fieldValue;
                    try
                    {
                        fieldValue = double.Parse(rowBuffer.get_Value(table.FindField(renderFields[i])).ToString());
                    }
                    catch (Exception ex)
                    {
                        fieldValue = 0;
                    }
                    maxValue = Math.Max(maxValue, fieldValue);
                }
                rowBuffer = cursor.NextRow();
            }


            featureClass = featureLayer.FeatureClass;
            featureCursor = featureClass.Search(null, false);//访问地图要素的游标
            feature = featureCursor.NextFeature();

            List<int> index = new List<int>();//储存字段索引
            for (int i = 0; i < renderFields.Count; i++)
                index.Add(feature.Fields.FindField(renderFields[i]));//FindField返回要标注字段的索引值

            IGraphicsContainer container = axMapControl1.Map as IGraphicsContainer;
            IEnvelope pEnv = null;//获取每个要素(行政村)的外接矩形
            //对地图要素(行政村)进行循环
            while (feature != null)
            {
                //使用地理对象的中心作为标注的位置
                pEnv = feature.Extent;
                IPoint centerPt = new PointClass();
                centerPt.PutCoords(pEnv.XMin + pEnv.Width * 0.5, pEnv.YMin + pEnv.Height * 0.5);


                //雷达图由同心多边形、柱线、数据线构成
                IPolyline polyline_pillar = new PolylineClass();//柱线

                int radius = 200;//最小多边形半径
                //绘制4个同心多边形
                for (int i = 0; i < 4; i++)
                {
                    IPolygon polygon_frame = new PolygonClass();//同心多边形
                    IPolygon polygon_data = new PolygonClass();//数据多边形
                    object missing = Type.Missing;

                    //多边形分成renderFields.count个等角区域
                    double currentAngle = Math.PI / 2;//从PI/2处开始旋转
                    for (int j = 0; j < renderFields.Count; j++)
                    {
                        IPoint pt_frame = new PointClass();
                        pt_frame.X = radius * Math.Cos(currentAngle) + centerPt.X;
                        pt_frame.Y = radius * Math.Sin(currentAngle) + centerPt.Y;
                        (polygon_frame as IPointCollection).AddPoint(pt_frame, ref missing, ref missing);

                        //绘制最后一个同心多边形，同时开始绘制线柱以及绘制数据
                        if (i == 3)
                        {
                            //柱线端点
                            (polyline_pillar as IPointCollection).AddPoint(pt_frame);
                            (polyline_pillar as IPointCollection).AddPoint(centerPt);

                            //数据点
                            IPoint pt_data = new PointClass();
                            pt_data.X = Convert.ToDouble(feature.get_Value(index[j])) / maxValue * radius * Math.Cos(currentAngle) + centerPt.X;
                            pt_data.Y = Convert.ToDouble(feature.get_Value(index[j])) / maxValue * radius * Math.Sin(currentAngle) + centerPt.Y;
                            (polygon_data as IPointCollection).AddPoint(pt_data);
                        }
                        currentAngle += 2 * Math.PI / renderFields.Count;//旋转到下一个字段
                    }
                    radius += 150;//增加半径,绘制下一个同心多边形


                    //雷达图样式
                    //symbol_frame
                    ISimpleFillSymbol fillSymbol_0 = new SimpleFillSymbolClass();
                    fillSymbol_0.Style = esriSimpleFillStyle.esriSFSNull;//esriSFSDiagonalCross
                    fillSymbol_0.Color = getRGB(240, 240, 240);

                    //symbol_pillar && symbol_frame线样式
                    ILineSymbol lineSymbol_0 = new SimpleLineSymbolClass();
                    lineSymbol_0.Width = 0.05;//esriSFSDiagonalCross
                    lineSymbol_0.Color = getRGB(150, 150, 150);
                    fillSymbol_0.Outline = lineSymbol_0;

                    //框架多边形element
                    IFillShapeElement fillElem_frame = new PolygonElementClass();
                    fillElem_frame.Symbol = fillSymbol_0;
                    IElement pElement_frame = fillElem_frame as IElement;
                    pElement_frame.Geometry = polygon_frame as IGeometry;
                    container.AddElement(pElement_frame, 0);

                    if (i == 3)
                    {
                        //柱线
                        ILineElement lineElem_pillar = new LineElementClass();
                        lineElem_pillar.Symbol = lineSymbol_0;
                        IElement pElement1 = lineElem_pillar as IElement;
                        pElement1.Geometry = polyline_pillar as IGeometry;
                        container.AddElement(pElement1, 0);



                        //数据线样式
                        ISimpleFillSymbol fillSymbol_1 = new SimpleFillSymbolClass();
                        fillSymbol_1.Style = esriSimpleFillStyle.esriSFSSolid;//esriSFSDiagonalCross
                        fillSymbol_1.Color = getRGB(230, 0, 0);

                        ILineSymbol lineSymbol_1 = new SimpleLineSymbolClass();
                        lineSymbol_1.Width = 1;//esriSFSDiagonalCross
                        lineSymbol_1.Color = getRGB(230, 0, 0);

                        fillSymbol_1.Outline = lineSymbol_1;

                        //数据折线
                        IFillShapeElement fillElem_data = new PolygonElementClass();
                        fillElem_data.Symbol = fillSymbol_1;
                        IElement pElement_data = fillElem_data as IElement;
                        pElement_data.Geometry = polygon_data as IGeometry;

                        container.AddElement(pElement_data, 0);
                    }
                }
                feature = featureCursor.NextFeature();//循环遍历元素
            }//对地图元素（行政区）进行循环
            IActiveView activeView = axMapControl1.Map as IActiveView;
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);//axMapControl1.Extent
        }

        #endregion






        #region 整饰
        private frmSymbol frmSym = null;
        //  private frmPrintPreview frmPrintPreview = null; // 打印                      
        //对地图的基本操作类
        private OperatePageLayout m_OperatePageLayout = null;
        private INewEnvelopeFeedback pNewEnvelopeFeedback;
        private EnumMapSurroundType _enumMapSurType = EnumMapSurroundType.None;
        private IStyleGalleryItem pStyleGalleryItem;

        private void 指北针ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enumMapSurType = EnumMapSurroundType.NorthArrow;
            if (frmSym == null || frmSym.IsDisposed)
            {
                frmSym = new frmSymbol();
                frmSym.GetSelSymbolItem += new frmSymbol.GetSelSymbolItemEventHandler(frmSym_GetSelSymbolItem);
            }
            frmSym.EnumMapSurType = _enumMapSurType;
            frmSym.InitUI();
            frmSym.ShowDialog();
        }

        private void frmSym_GetSelSymbolItem(ref IStyleGalleryItem pStyleItem)
        {
            pStyleGalleryItem = pStyleItem;
        }

        private void 比例尺ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _enumMapSurType = EnumMapSurroundType.ScaleBar;
                if (frmSym == null || frmSym.IsDisposed)
                {
                    frmSym = new frmSymbol();
                    frmSym.GetSelSymbolItem += new frmSymbol.GetSelSymbolItemEventHandler(frmSym_GetSelSymbolItem);
                }
                frmSym.EnumMapSurType = _enumMapSurType;
                frmSym.InitUI();
                frmSym.ShowDialog();
            }
            catch (Exception ex)
            {
            }
        }
         
        private void 图例ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _enumMapSurType = EnumMapSurroundType.Legend;
            }
            catch (Exception ex)
            {

            }
        }

        private void 图廓ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IEnvelope NTK = new EnvelopeClass();
                ISelection selection = axMapControl1.Map.FeatureSelection;
                IEnumFeature enumFeature = (IEnumFeature)selection;
                enumFeature.Reset();
                IFeature sFeature = enumFeature.Next();
                while (sFeature != null)
                {
                    NTK.Union(sFeature.Extent);
                    sFeature = enumFeature.Next();
                }
                if (NTK.IsEmpty != true)
                {
                    NTK.Expand(1.1, 1.1, true);
                }
                IMap map = axMapControl1.Map;
                IGraphicsContainer pContainer = map as IGraphicsContainer;
                IActiveView pActiveView = (IActiveView)map;
                IRectangleElement pRectangleEle = new RectangleElementClass();
                IElement pEle = pRectangleEle as IElement;
                pEle.Geometry = NTK;
                IRgbColor pColor = new RgbColorClass();
                pColor.Red = 255;
                pColor.Green = 0;
                pColor.Blue = 0;
                pColor.Transparency = 255;
                //产生一个线符号对象
                ILineSymbol pOutline = new SimpleLineSymbolClass();
                pOutline.Width = 1;
                pOutline.Color = pColor;
                //设置颜色属性
                pColor = new RgbColorClass();
                pColor.Red = 255;
                pColor.Green = 0;
                pColor.Blue = 0;
                pColor.Transparency = 0;
                //设置填充符号的属性
                IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                pFillSymbol.Color = pColor;
                pFillSymbol.Outline = pOutline;
                IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
                pFillShapeEle.Symbol = pFillSymbol;
                pContainer.AddElement((IElement)pFillShapeEle, 0);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            catch
            {
            }
        }

        private void axPageLayoutControl1_OnMouseMove_1(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            try
            {
                if (_enumMapSurType != EnumMapSurroundType.None)
                {
                    if (pNewEnvelopeFeedback != null)
                    {
                        m_MovePt = (axPageLayoutControl1.PageLayout as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                        pNewEnvelopeFeedback.MoveTo(m_MovePt);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void axPageLayoutControl1_OnMouseUp_1(object sender, IPageLayoutControlEvents_OnMouseUpEvent e)
        {
            if (_enumMapSurType != EnumMapSurroundType.None)
            {
                if (pNewEnvelopeFeedback != null)
                {
                    IActiveView pActiveView = null;
                    pActiveView = axPageLayoutControl1.PageLayout as IActiveView;
                    IEnvelope pEnvelope = pNewEnvelopeFeedback.Stop();
                    AddMapSurround(pActiveView, _enumMapSurType, pEnvelope);
                    pNewEnvelopeFeedback = null;
                    _enumMapSurType = EnumMapSurroundType.None;
                }
            }
        }

        private void AddMapSurround(IActiveView pAV, EnumMapSurroundType _enumMapSurroundType, IEnvelope pEnvelope)
        {
            try
            {
                switch (_enumMapSurroundType)
                {
                    case EnumMapSurroundType.NorthArrow:
                        addNorthArrow(axPageLayoutControl1.PageLayout, pEnvelope, pAV);
                        break;
                    case EnumMapSurroundType.ScaleBar:
                        makeScaleBar(pAV, axPageLayoutControl1.PageLayout, pEnvelope);
                        break;
                    case EnumMapSurroundType.Legend:
                        MakeLegend(pAV, axPageLayoutControl1.PageLayout, pEnvelope);
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        //指北针
        void addNorthArrow(IPageLayout pPageLayout, IEnvelope pEnv, IActiveView pActiveView)
        {
            // MessageBox.Show("1");
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            if (pStyleGalleryItem == null)
            {
                // MessageBox.Show("2");
                return;
            }
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            INorthArrow pNorthArrow = new MarkerNorthArrowClass();
            pNorthArrow = pStyleGalleryItem.Item as INorthArrow;
            pNorthArrow.Size = pEnv.Width * 50;
            pMapSurroundFrame.MapSurround = (IMapSurround)pNorthArrow;//根据用户的选取，获取相应的MapSurround            
            IElement pElement = axPageLayoutControl1.FindElementByName("NorthArrows");//获取PageLayout中的指北针元素
            if (pElement != null)
            {
                pGraphicsContainer.DeleteElement(pElement);  //如果存在指北针，删除已经存在的指北针
            }
            IElementProperties pElePro = null;
            pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = (IGeometry)pEnv;
            pElePro = pElement as IElementProperties;
            pElePro.Name = "NorthArrows";
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        //比例尺
        public void makeScaleBar(IActiveView pActiveView, IPageLayout pPageLayout, IEnvelope pEnv)
        {
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            if (pStyleGalleryItem == null) return;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            pMapSurroundFrame.MapSurround = (IMapSurround)pStyleGalleryItem.Item;
            IElement pElement = axPageLayoutControl1.FindElementByName("ScaleBar");
            if (pElement != null)
            {
                pGraphicsContainer.DeleteElement(pElement);  //删除已经存在的比例尺
            }
            IElementProperties pElePro = null;
            pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = (IGeometry)pEnv;
            pElePro = pElement as IElementProperties;
            pElePro.Name = "ScaleBar";
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        //图例
        private void MakeLegend(IActiveView pActiveView, IPageLayout pPageLayout, IEnvelope pEnv)
        {
            UID pID = new UID();
            pID.Value = "esriCarto.Legend";
            IGraphicsContainer pGraphicsContainer = pPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pActiveView.FocusMap) as IMapFrame;
            IMapSurroundFrame pMapSurroundFrame = pMapFrame.CreateSurroundFrame(pID, null);//根据唯一标示符，创建与之对应MapSurroundFrame
            IElement pDeletElement = axPageLayoutControl1.FindElementByName("Legend");//获取PageLayout中的图例元素
            if (pDeletElement != null)
            {
                pGraphicsContainer.DeleteElement(pDeletElement);  //如果已经存在图例，删除已经存在的图例
            }
            //设置MapSurroundFrame背景
            ISymbolBackground pSymbolBackground = new SymbolBackgroundClass();
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ILineSymbol pLineSymbol = new SimpleLineSymbolClass();
            pLineSymbol.Color = m_OperatePageLayout.GetRgbColor(0, 0, 0);
            pFillSymbol.Color = m_OperatePageLayout.GetRgbColor(240, 240, 240);
            pFillSymbol.Outline = pLineSymbol;
            pSymbolBackground.FillSymbol = pFillSymbol;
            pMapSurroundFrame.Background = pSymbolBackground;
            //添加图例
            IElement pElement = pMapSurroundFrame as IElement;
            pElement.Geometry = pEnv as IGeometry;
            IMapSurround pMapSurround = pMapSurroundFrame.MapSurround;
            ILegend pLegend = pMapSurround as ILegend;
            pLegend.ClearItems();
            pLegend.Title = "图例";
            for (int i = 0; i < pActiveView.FocusMap.LayerCount; i++)
            {
                ILegendItem pLegendItem = new HorizontalLegendItemClass();
                pLegendItem.Layer = pActiveView.FocusMap.get_Layer(i);//获取添加图例关联图层             
                pLegendItem.ShowDescriptions = false;
                pLegendItem.Columns = 1;
                pLegendItem.ShowHeading = true;
                pLegendItem.ShowLabels = true;
                pLegend.AddItem(pLegendItem);//添加图例内容
            }
            pGraphicsContainer.AddElement(pElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

        }


        #endregion
    }
}