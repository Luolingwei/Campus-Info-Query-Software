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
    {/// <summary>
    /// 
    /// </summary>
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
            public string rjgd;

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

            #region 雷达图
            if (caozuo == 999)
            {
                //MessageBox.Show("未能找到有效路径" + caozuo, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning); 
                caozuo = 0;
                //新范围
                IGraphicsContainer pGraphicsContainer = axMapControl1.Map as IGraphicsContainer;
                IActiveView pActiveView = pGraphicsContainer as IActiveView;
                pGraphicsContainer.DeleteAllElements();


                m_PointPt = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                #region 定义填充颜色与类型
                IRgbColor pColor = new RgbColorClass();//颜色
                pColor.RGB = System.Drawing.Color.FromArgb(130, 130, 130).ToArgb();//(B,G,R)
                ILineSymbol pLineSymbol = new SimpleLineSymbolClass();//产生一个线符号对象
                pLineSymbol.Width = 0.2;
                pLineSymbol.Color = pColor;
                IFillSymbol pFillSymbol = new SimpleFillSymbolClass();//设置填充符号的属性
                pColor.Transparency = 0;
                pFillSymbol.Color = pColor;
                pFillSymbol.Outline = pLineSymbol;
                #endregion

                IPoint pPoint = new PointClass();
                pPoint.PutCoords(m_PointPt.X, m_PointPt.Y);


                //画圈
                int r = 500;
                for (int i = 0; i < 3; i++)
                {
                    IConstructCircularArc pConstructCircularArc = new CircularArcClass();
                    pConstructCircularArc.ConstructCircle(pPoint, r + i * r, false);
                    ICircularArc pArc = pConstructCircularArc as ICircularArc;
                    ISegment pSegment1 = pArc as ISegment; //通过ISegmentCollection构建Ring对象
                    ISegmentCollection pSegCollection = new RingClass();
                    object o = Type.Missing; //添加Segement对象即圆
                    pSegCollection.AddSegment(pSegment1, ref o, ref o); //QI到IRing接口封闭Ring对象，使其有效
                    IRing pRing = pSegCollection as IRing;
                    pRing.Close(); //通过Ring对象使用IGeometryCollection构建Polygon对象


                    IGeometryCollection pGeometryColl = new PolygonClass();
                    pGeometryColl.AddGeometry(pRing, ref o, ref o); //构建一个CircleElement对象
                    IElement pElement = new CircleElementClass();
                    pElement.Geometry = pGeometryColl as IGeometry;


                    //填充圆的颜色
                    IFillShapeElement pFillShapeElement = pElement as IFillShapeElement;
                    pFillShapeElement.Symbol = pFillSymbol;
                    IGraphicsContainer pGC = this.axMapControl1.ActiveView.GraphicsContainer;
                    pGC.AddElement(pElement, 0);
                }

                //画柱
                for (int i = 0; i < 18; i++)
                {

                    IPoint pPoint2;
                    pPoint2 = new PointClass();
                    double x;
                    double y;
                    x = pPoint.X + 3 * r * Math.Cos(2 * i * Math.PI / 18);
                    y = pPoint.Y + 3 * r * Math.Sin(2 * i * Math.PI / 18);
                    pPoint2.PutCoords(x, y);
                    ILine pLine;
                    pLine = new LineClass();
                    pLine.PutCoords(pPoint, pPoint2);
                    IGeometryCollection pPolyline;
                    pPolyline = new PolylineClass();
                    ISegmentCollection pPath;
                    pPath = new PathClass();
                    object Missing1 = Type.Missing;
                    object Missing2 = Type.Missing;
                    pPath.AddSegment(pLine as ISegment, ref Missing1, ref Missing2);
                    pPolyline.AddGeometry(pPath as IGeometry, ref Missing1, ref Missing2);

                    IRgbColor rGBColor = new RgbColorClass();
                    rGBColor.Red = 130;
                    rGBColor.Green = 130;
                    rGBColor.Blue = 130;

                    IElement element = DrawLineSymbol(pPolyline as IGeometry, rGBColor);

                    pGraphicsContainer.AddElement(element, 0);
                }




                /*
                Graphics g = this.CreateGraphics();
                Pen Mypen = new Pen(Color.Blue, 10);

                System.Drawing.Point p1 = new System.Drawing.Point(38627775, 3354858);
                System.Drawing.Point p2 = new System.Drawing.Point(38627175, 3354258);
                System.Drawing.Point p3 = new System.Drawing.Point(38617175, 3352858);
                System.Drawing.Point p4 = new System.Drawing.Point(38617175, 3353858);
                System.Drawing.Point[] p = { p1, p2, p3, p4 };
                g.DrawPolygon(Mypen, p);
            

                */
                //读txt
                string dir = "D:\\";
                string file = "rjgd.txt";
                StreamReader sr = new StreamReader(dir + file);
                /* while (sr.Peek() > -1)
                 {
                     MessageBox.Show(sr.ReadToEnd());
                 }
                 */
                ArrayList DataStore = new ArrayList();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] sArray = line.Split(',');
                    Datal data = new Datal();
                    data.qs = sArray[0];
                    data.rjgd = sArray[1];
                    DataStore.Add(data);
                }
                
                sr.Close();

                //插注记啦
                for (int i = 0; i < DataStore.Count; i++)
                {
                    double X;
                    double Y;
                    X = pPoint.X + 3.5 * r * Math.Cos(2 * i * Math.PI / 18);
                    Y = pPoint.Y + 3.5 * r * Math.Sin(2 * i * Math.PI / 18);
                    m_PointPt.X = X;
                    m_PointPt.Y = Y;
                    ITextElement ptext = new TextElementClass();
                    Datal a = DataStore[i] as Datal;
                    string str = a.qs;
                    ptext.Text = str;
                    ITextSymbol pSymbol = new TextSymbolClass();
                    pSymbol.Size = 5;
                    ptext.Symbol = pSymbol;
                    IElement pEle = ptext as IElement;
                    pEle.Geometry = m_PointPt;
                    pGraphicsContainer.AddElement(pEle, 0);

                }

                //画数据
                IPoint[] points = new IPoint[DataStore.Count];
                for (int i = 0; i < DataStore.Count; i++)
                {
                    Datal a = DataStore[i] as Datal;
                    string str = a.rjgd;
                    double number = double.Parse(str);

                    double x;
                    double y;
                    x = pPoint.X + number * 3 * r * Math.Cos(2 * i * Math.PI / 18) / 1565;
                    y = pPoint.Y + number * 3 * r * Math.Sin(2 * i * Math.PI / 18) / 1565;
                    IPoint pt = new PointClass();
                    pt.PutCoords(x, y);
                    points[i] = pt;
                    //MessageBox.Show("坐标生成");

                }

                for (int i = 0; i < 17; i++)
                {
                    ILine pLine;
                    pLine = new LineClass();
                    pLine.PutCoords(points[i], points[i + 1]);
                    IGeometryCollection pPolyline;
                    pPolyline = new PolylineClass();
                    ISegmentCollection pPath;
                    pPath = new PathClass();
                    object Missing1 = Type.Missing;
                    object Missing2 = Type.Missing;
                    pPath.AddSegment(pLine as ISegment, ref Missing1, ref Missing2);
                    pPolyline.AddGeometry(pPath as IGeometry, ref Missing1, ref Missing2);

                    IRgbColor rGBColor = new RgbColorClass();
                    rGBColor.Red = 0;
                    rGBColor.Green = 112;
                    rGBColor.Blue = 255;

                    IElement element = DrawLineSymbol(pPolyline as IGeometry, rGBColor);

                    pGraphicsContainer.AddElement(element, 0);
                }
                ILine pLine2;
                pLine2 = new LineClass();
                pLine2.PutCoords(points[17], points[0]);
                IGeometryCollection pPolyline2;
                pPolyline2 = new PolylineClass();
                ISegmentCollection pPath2;
                pPath2 = new PathClass();
                object Missing12 = Type.Missing;
                object Missing22 = Type.Missing;
                pPath2.AddSegment(pLine2 as ISegment, ref Missing12, ref Missing22);
                pPolyline2.AddGeometry(pPath2 as IGeometry, ref Missing12, ref Missing22);

                IRgbColor rGBColor2 = new RgbColorClass();
                rGBColor2.Red = 0;
                rGBColor2.Green = 112;
                rGBColor2.Blue = 255;

                IElement element2 = DrawLineSymbol(pPolyline2 as IGeometry, rGBColor2);

                pGraphicsContainer.AddElement(element2, 0);



                //注记
                IFontDisp pFont = new StdFont()
                {
                    Name = "宋体",
                    Size = 5
                } as IFontDisp;

                ITextSymbol pTextSymbol = new TextSymbolClass()
                {
                    Color = pColor,
                    Font = pFont,
                    Size = 11
                };
                IGraphicsContainer pGraContainer = axMapControl1.Map as IGraphicsContainer;
                                
                this.axMapControl1.Refresh();

            }
            #endregion
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
            axMapControl1.LoadMxFile(@"C:\Users\asus\Desktop\GIS实习\实习二\HCZ2.mxd");
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


        private void 柱状图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enumChartType = EnumChartRenderType.BarChart;
            ILayer pLayer = null;
            IFeatureLayer mFeatureLayer = null;
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                ILayer pLayert;
                pLayert = axMapControl1.get_Layer(i);
                if (pLayert.Name == "")
                {
                    pLayer = pLayert;
                    mFeatureLayer = pLayert as IFeatureLayer;
                }
            }

            Dictionary<string, IRgbColor> _dicFieldAndColor = null;
            _dicFieldAndColor = new Dictionary<string, IRgbColor>();
            IRgbColor pRgbColor = null;
            OperateMap m_OperMap = new OperateMap();

            pRgbColor = m_OperMap.GetRgbColor(252, 141, 98);
            _dicFieldAndColor.Add("水田", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(141, 160, 203);
            _dicFieldAndColor.Add("旱地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(231, 138, 195);
            _dicFieldAndColor.Add("园地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(166, 216, 84);
            _dicFieldAndColor.Add("草地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(255, 217, 47);
            _dicFieldAndColor.Add("水域", pRgbColor);

            ChartRenderer(mFeatureLayer, _dicFieldAndColor);
        }
        private void 饼状图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enumChartType = EnumChartRenderType.PieChart;
            ILayer pLayer = null;
            IFeatureLayer mFeatureLayer = null;
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                ILayer pLayert;
                pLayert = axMapControl1.get_Layer(i);
                if (pLayert.Name == "饼图点")
                {
                    pLayer = pLayert;
                    mFeatureLayer = pLayert as IFeatureLayer;
                }
            }

            Dictionary<string, IRgbColor> _dicFieldAndColor = null;
            _dicFieldAndColor = new Dictionary<string, IRgbColor>();
            IRgbColor pRgbColor = null;
            OperateMap m_OperMap = new OperateMap();

            pRgbColor = m_OperMap.GetRgbColor(252, 141, 98);
            _dicFieldAndColor.Add("水田", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(141, 160, 203);
            _dicFieldAndColor.Add("旱地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(231, 138, 195);
            _dicFieldAndColor.Add("园地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(166, 216, 84);
            _dicFieldAndColor.Add("草地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(255, 217, 47);
            _dicFieldAndColor.Add("水域", pRgbColor);

            ChartRenderer(mFeatureLayer, _dicFieldAndColor);
        }
        private void 堆叠图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _enumChartType = EnumChartRenderType.StackChart;
            ILayer pLayer = null;
            IFeatureLayer mFeatureLayer = null;
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                ILayer pLayert;
                pLayert = axMapControl1.get_Layer(i);
                if (pLayert.Name == "饼图点")
                {
                    pLayer = pLayert;
                    mFeatureLayer = pLayert as IFeatureLayer;
                }
            }

            Dictionary<string, IRgbColor> _dicFieldAndColor = null;
            _dicFieldAndColor = new Dictionary<string, IRgbColor>();
            IRgbColor pRgbColor = null;
            OperateMap m_OperMap = new OperateMap();

            pRgbColor = m_OperMap.GetRgbColor(252, 141, 98);
            _dicFieldAndColor.Add("水田", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(141, 160, 203);
            _dicFieldAndColor.Add("旱地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(231, 138, 195);
            _dicFieldAndColor.Add("园地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(166, 216, 84);
            _dicFieldAndColor.Add("草地", pRgbColor);
            pRgbColor = m_OperMap.GetRgbColor(255, 217, 47);
            _dicFieldAndColor.Add("水域", pRgbColor);

            ChartRenderer(mFeatureLayer, _dicFieldAndColor);
        }



        private void ChartRenderer(IFeatureLayer pFeatLyr, Dictionary<string, IRgbColor> dicFieldAndColor)
        {
            IGeoFeatureLayer pGeoFeatLyr = pFeatLyr as IGeoFeatureLayer;
            IChartRenderer pChartRender = new ChartRendererClass();
            IRendererFields pRenderFields = pChartRender as IRendererFields;
            IFeatureCursor pCursor = null;
            IDataStatistics pDataSta = null;
            double dMax = 0; double dTemp = 0;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pCursor = pGeoFeatLyr.Search(pQueryFilter, true);
            //遍历出所选择的第一个字段的最大值，，作为设置专题图的比例大小的依据
            foreach (KeyValuePair<string, IRgbColor> _keyValue in dicFieldAndColor)
            {
                pRenderFields.AddField(_keyValue.Key, _keyValue.Key);
                pDataSta = new DataStatisticsClass();
                pDataSta.Cursor = pCursor as ICursor;
                pDataSta.Field = _keyValue.Key;
                dTemp = pDataSta.Statistics.Maximum;
                if (dTemp >= dMax)
                {
                    dMax = dTemp;
                }
            }

            IRgbColor pRgbColor = null;
            IChartSymbol pChartSym = null;
            IFillSymbol pFillSymbol = null;
            IMarkerSymbol pMarkerSym = null;
            IBarChartSymbol pBarChartSym = null;
            IPieChartSymbol pPieChartSymbol = null;
            IStackedChartSymbol pStackChartSym = null;

            // 定义并设置渲染样式
            switch (_enumChartType)
            {
                case EnumChartRenderType.PieChart:
                    pPieChartSymbol = new PieChartSymbolClass();
                    pPieChartSymbol.Clockwise = true;//说明饼图是否顺时针方向
                    pPieChartSymbol.UseOutline = true;//说明是否使用轮廓线
                    ILineSymbol pLineSym = new SimpleLineSymbolClass();
                    //     pLineSym.Color = m_OperateMap.GetRgbColor(100, 205, 30) as IColor;
                    pLineSym.Width = 1;
                    pPieChartSymbol.Outline = pLineSym;
                    break;
                case EnumChartRenderType.BarChart:
                    pBarChartSym = new BarChartSymbolClass();
                    pBarChartSym.Width = 6;//设置每个条形图的宽度
                    break;
                case EnumChartRenderType.StackChart:
                    pStackChartSym = new StackedChartSymbolClass();
                    pStackChartSym.Width = 6;//设置每个堆叠图的宽度
                    break;
            }
            if (pPieChartSymbol != null)
            {
                pChartSym = pPieChartSymbol as IChartSymbol;
                pMarkerSym = pPieChartSymbol as IMarkerSymbol;
                pMarkerSym.Size = 20; //设置饼状图的大小
            }
            if (pBarChartSym != null)
            {
                pChartSym = pBarChartSym as IChartSymbol;
                pMarkerSym = pBarChartSym as IMarkerSymbol;
                pMarkerSym.Size = 30;//设置条形图的高度
            }
            else if (pStackChartSym != null)
            {
                pChartSym = pStackChartSym as IChartSymbol;
                pMarkerSym = pStackChartSym as IMarkerSymbol;
                pMarkerSym.Size = 20;//设置堆叠图的高度
            }
            pChartSym.MaxValue = dMax;
            ISymbolArray pSymArray = null;
            if (pBarChartSym != null)
            {
                pSymArray = pBarChartSym as ISymbolArray;
            }
            else if (pStackChartSym != null)
            {
                pSymArray = pStackChartSym as ISymbolArray;
            }
            else if (pPieChartSymbol != null)
            {
                pSymArray = pPieChartSymbol as ISymbolArray;
            }

            foreach (KeyValuePair<string, IRgbColor> _keyValue in dicFieldAndColor)
            {
                //获取渲染字段的颜色值
                pRgbColor = _keyValue.Value;
                pFillSymbol = new SimpleFillSymbolClass();
                pFillSymbol.Color = pRgbColor as IColor;
                pSymArray.AddSymbol(pFillSymbol as ISymbol);
            }
            if (pPieChartSymbol != null)
            {
                pChartRender.ChartSymbol = pPieChartSymbol as IChartSymbol;
            }
            if (pBarChartSym != null)
            {
                pChartRender.ChartSymbol = pBarChartSym as IChartSymbol;
            }
            else if (pStackChartSym != null)
            {
                pChartRender.ChartSymbol = pStackChartSym as IChartSymbol;
            }

            //     pFillSymbol = new SimpleFillSymbolClass();
            //      pFillSymbol.Color = m_OperateMap.GetRgbColor(239, 228, 190);
            //      pChartRender.BaseSymbol = pFillSymbol as ISymbol;// 设置背景符号
            //让符号处于图形中央（若渲染的图层为点图层，则该句应去掉，否则不显示渲染结果）
            //pChartRender.UseOverposter = false; 
            pChartRender.CreateLegend();
            pGeoFeatLyr.Renderer = pChartRender as IFeatureRenderer;
            axMapControl1.Refresh();
            axTOCControl1.Update();
            _enumChartType = EnumChartRenderType.UnKnown;
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