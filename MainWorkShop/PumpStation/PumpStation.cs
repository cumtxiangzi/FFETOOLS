using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class PumpStation : IExternalCommand
    {
        public static PumpStationForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new PumpStationForm();
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
    }
    public class ExecuteEventPumpStation : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
                View activeView = doc.ActiveView;

                ArrayList labels = GetAllLabelsOfGrids(doc);
                DisplayUnitType dut = GetLengthUnitType(doc);
                CreateOrthogonalGridsData m_data;
                List<Grid> XGrids = new List<Grid>();
                List<Grid> YGrids = new List<Grid>();
                List<Grid> XYGrids = new List<Grid>();

                ViewPlan roomBottomPlan = null;
                ViewPlan viewPlan = null;
                ElementId newViewId = ElementId.InvalidElementId;
                Level roomBottomlevel = null;
                Level roomToplevel = null;

                //string roomBottomElevation = WaterPool.mainfrm.PoolBottomElevation.Text;
                //double roomBottomElevationValue = WaterPool.mainfrm.PoolBottomElevationValue;
                //double roomHeightValue = WaterPool.mainfrm.PoolHeightValue;
                //double roomLehgthValue = WaterPool.mainfrm.PoolLengthValue;
                //double roomWidthValue = WaterPool.mainfrm.PoolWidthValue;

                string roomBottomElevation = "0.0";
                double roomBottomElevationValue = 0;
                double roomHeightValue = 4000;
                double roomLehgthValue = 16000;
                double roomWidthValue = 6000;

                if (activeView.IsTemplate != true && activeView.ViewType == ViewType.FloorPlan)
                {
                    TransactionGroup tg = new TransactionGroup(doc, "创建泵站");
                    tg.Start();

                    using (Transaction trans = new Transaction(doc, "创建标高"))
                    {
                        trans.Start();

                        if (ElevationExist(doc, (roomBottomElevationValue * 1000).ToString()))
                        {
                            roomBottomlevel = GetLevel(doc, (roomBottomElevationValue * 1000).ToString());
                        }
                        else
                        {
                            roomBottomlevel = Level.Create(doc, roomBottomElevationValue * 1000 / 304.8);
                            roomBottomlevel.Name = Convert.ToDouble(roomBottomElevation).ToString("0.000");
                            viewPlan = ViewPlan.Create(doc, GetViewFamilyType(doc).Id, roomBottomlevel.Id);//为新建的标高创建对应的视图                    
                        }

                        if (ElevationExist(doc, (roomBottomElevationValue * 1000 + roomHeightValue).ToString()))
                        {
                            roomToplevel = GetLevel(doc, (roomBottomElevationValue * 1000 + roomHeightValue).ToString());
                        }
                        else
                        {
                            roomToplevel = Level.Create(doc, (roomBottomElevationValue * 1000 + roomHeightValue) / 304.8);
                            roomToplevel.Name = (roomBottomElevationValue + roomHeightValue / 1000).ToString("0.000");
                            ViewPlan.Create(doc, GetViewFamilyType(doc).Id, roomToplevel.Id);
                        }

                        trans.Commit();
                    }

                    using (Transaction trans = new Transaction(doc, "布置轴网"))
                    {
                        trans.Start();

                        CreateOrthogonalGridsData orthogonalData = new CreateOrthogonalGridsData(app, dut, labels);

                        m_data = orthogonalData;
                        m_data.XOrigin = Unit.CovertToAPI(0, m_data.Dut);
                        m_data.YOrigin = Unit.CovertToAPI(0, m_data.Dut);
                        m_data.XNumber = 2;
                        m_data.YNumber = 5;

                        m_data.XSpacing = Unit.CovertToAPI(6000, m_data.Dut);
                        m_data.XBubbleLoc = BubbleLocation.StartPoint;
                        m_data.XFirstLabel = "A";

                        m_data.YSpacing = Unit.CovertToAPI(4000, m_data.Dut);
                        m_data.YBubbleLoc = BubbleLocation.StartPoint;
                        m_data.YFirstLabel = "1";

                        orthogonalData.CreateGrids();
                        XGrids.AddRange(orthogonalData.XGrids);
                        YGrids.AddRange(orthogonalData.YGrids);
                        HideGridBubble(XGrids, activeView);
                        HideGridBubble(YGrids, activeView);

                        XYGrids.AddRange(orthogonalData.XGrids);
                        XYGrids.AddRange(orthogonalData.YGrids);

                        trans.Commit();
                    }

                    List<Wall> newWalls = new List<Wall>();
                    using (Transaction trans = new Transaction(doc, "轴网标注及生墙"))
                    {
                        trans.Start();

                        newWalls = AddWallByCrossGrids(doc, app, XYGrids, roomToplevel, roomBottomlevel, -100, false, false);//此处需要关联窗体参数

                        CreatGridDemesion(doc, activeView, XGrids);
                        CreatGridDemesion(doc, activeView, YGrids);

                        trans.Commit();
                    }

                    Floor roomBottomFloor = null;
                    Floor roomTopFloor = null;
                    XYZ pickpoint = new XYZ();
                    double roomWallThick = 100 / 304.8;

                    using (Transaction trans = new Transaction(doc, "生成地面屋顶及门窗"))
                    {
                        trans.Start();

                        CurveArray array = new CurveArray();
                        XYZ point11 = new XYZ(pickpoint.X - roomWallThick, pickpoint.Y - roomWallThick, 0);
                        XYZ point21 = new XYZ(pickpoint.X - roomWallThick, pickpoint.Y + roomWallThick + roomWidthValue / 304.8, 0);
                        XYZ point31 = new XYZ(pickpoint.X + roomWallThick + roomLehgthValue / 304.8, pickpoint.Y + roomWallThick + roomWidthValue / 304.8, 0);
                        XYZ point41 = new XYZ(pickpoint.X + roomWallThick + roomLehgthValue / 304.8, pickpoint.Y - roomWallThick, 0);
                        array.Append(Line.CreateBound(point11, point21));
                        array.Append(Line.CreateBound(point21, point31));
                        array.Append(Line.CreateBound(point31, point41));
                        array.Append(Line.CreateBound(point41, point11));

                        CurveArray array2 = new CurveArray();
                        XYZ point12 = new XYZ(pickpoint.X - roomWallThick - 800 / 304.8, pickpoint.Y - roomWallThick - 800 / 304.8, 0);
                        XYZ point22 = new XYZ(pickpoint.X - roomWallThick - 800 / 304.8, pickpoint.Y + roomWallThick + roomWidthValue / 304.8 + 800 / 304.8, 0);
                        XYZ point32 = new XYZ(pickpoint.X + roomWallThick + roomLehgthValue / 304.8 + 800 / 304.8, pickpoint.Y + roomWallThick + roomWidthValue / 304.8 + 800 / 304.8, 0);
                        XYZ point42 = new XYZ(pickpoint.X + roomWallThick + roomLehgthValue / 304.8 + 800 / 304.8, pickpoint.Y - roomWallThick - 800 / 304.8, 0);
                        array2.Append(Line.CreateBound(point12, point22));
                        array2.Append(Line.CreateBound(point22, point32));
                        array2.Append(Line.CreateBound(point32, point42));
                        array2.Append(Line.CreateBound(point42, point12));

                        roomBottomFloor = doc.Create.NewFloor(array, PoolFloorType(doc, "100"), roomBottomlevel, true, XYZ.BasisZ);
                        roomBottomFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0);

                        roomTopFloor = doc.Create.NewFloor(array2, PoolFloorType(doc, "100"), roomToplevel, true, XYZ.BasisZ);
                        double topFloorThick = roomTopFloor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();
                        roomTopFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(100 / 304.8);

                        CreatDoorAndWindow(doc, newWalls, roomWidthValue);

                        trans.Commit();
                    }

                    using (Transaction trans = new Transaction(doc, "生成散水"))
                    {
                        trans.Start();

                        GetSlabEdge(doc, roomBottomFloor);

                        trans.Commit();
                    }

                    tg.Assimilate();
                }
                else
                {
                    MessageBox.Show("请在平面视图中操作", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetName()
        {
            return "创建泵站";
        }
        public void CreatDoorAndWindow(Document RevitDoc, List<Wall> walls, double roomWidth)
        {
            FamilySymbol doorType = null;
            FamilySymbol windowType = null;

            IList<FamilySymbol> symbols = CollectorHelper.TCollector<FamilySymbol>(RevitDoc);
            foreach (FamilySymbol element in symbols)
            {
                if (element.FamilyName.Contains("建筑_门_default双扇") && element.Name.Contains("1500x2400"))
                {
                    doorType = element;
                    break;
                }
            }

            foreach (FamilySymbol element in symbols)
            {
                if (element.FamilyName.Contains("铝合金窗") && element.Name.Contains("C-1"))
                {
                    windowType = element;
                    break;
                }
            }

            // 使用族类型创建门 
            foreach (var item in walls)
            {
                Line line = null;
                LocationCurve locationCurve = item.Location as LocationCurve;
                if (locationCurve != null)
                {
                    line = locationCurve.Curve as Line;
                }

                XYZ midPoint = (line.GetEndPoint(0) + line.GetEndPoint(1)) / 2;
                Level wallLevel = RevitDoc.GetElement(item.LevelId) as Level;

                if (line.Direction.X == 1 && line.Direction.Y == 0)
                {
                    if (line.GetEndPoint(0).Y == 0)
                    {
                        // 在墙的中心位置创建一个门 
                        FamilyInstance door = RevitDoc.Create.NewFamilyInstance(midPoint, doorType, item, wallLevel, StructuralType.NonStructural);
                        if (door.CanRotate)
                        {
                            door.rotate();
                        }
                    }
                    else
                    {
                        if (HasMinimalDifference(line.GetEndPoint(0).Y, roomWidth / 304.8, 1))
                        {
                            // 在墙的中心位置创建一个门 
                           FamilyInstance window = RevitDoc.Create.NewFamilyInstance(midPoint, windowType, item, wallLevel, StructuralType.NonStructural);
                            window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(900/304.8);
                            //if (door.CanRotate)
                            //{
                            //    door.rotate();
                            //}
                        }
                    }

                }
            }
        }
        public bool HasMinimalDifference(double value1, double value2, int units)
        {
            long lValue1 = BitConverter.DoubleToInt64Bits(value1);
            long lValue2 = BitConverter.DoubleToInt64Bits(value2);

            // If the signs are different, return false except for +0 and -0.
            if ((lValue1 >> 63) != (lValue2 >> 63))
            {
                if (value1 == value2)
                    return true;

                return false;
            }

            long diff = Math.Abs(lValue1 - lValue2);

            if (diff <= (long)units)
                return true;

            return false;
        }
        public void GetSlabEdge(Document doc, Floor bottomFloor)//创建散水
        {
            SlabEdgeType footSlab = null;
            IList<SlabEdgeType> edges = CollectorHelper.TCollector<SlabEdgeType>(doc);
            foreach (var item in edges)
            {
                if (item.Name.Contains("建筑_散水无垫层_1000"))
                {
                    footSlab = item;
                    break;
                }
            }

            Face normalFace = null;
            Options options = new Options();
            options.ComputeReferences = true;
            GeometryElement geometryElement = bottomFloor.get_Geometry(options);
            foreach (GeometryObject item in geometryElement)
            {
                if (item is Solid solid)
                {
                    List<Face> list = new List<Face>();
                    foreach (Face face in solid.Faces)
                    {
                        list.Add(face);
                    }
                    double AreaMax = list.Max(t => t.Area);
                    normalFace = list.FirstOrDefault(p => p.Area == AreaMax);
                }
            }

            EdgeArrayArray eaa = normalFace.EdgeLoops;
            EdgeArray ea = eaa.get_Item(0);

            ReferenceArray refArr = new ReferenceArray();
            foreach (Edge item in ea)
            {
                refArr.Append(item.Reference);
            }

            SlabEdge newEdge = doc.Create.NewSlabEdge(footSlab, refArr);
            newEdge.get_Parameter(BuiltInParameter.SWEEP_BASE_OFFSET).Set(200 / 304.8);
            newEdge.get_Parameter(BuiltInParameter.SWEEP_BASE_VERT_OFFSET).Set(200 / 304.8);
        }

        public ViewFamilyType GetViewFamilyType(Document doc)
        {
            ViewFamilyType view = null;
            FilteredElementCollector collectorViewFamilyType = new FilteredElementCollector(doc);
            IList<Element> viewFamilyTypes = collectorViewFamilyType.OfClass(typeof(ViewFamilyType)).ToElements();
            foreach (Element elem in viewFamilyTypes)
            {
                ViewFamilyType viewFamilyType = elem as ViewFamilyType;
                if (viewFamilyType.ViewFamily == ViewFamily.FloorPlan)
                {
                    view = viewFamilyType;
                    break;
                }
            }
            return view;
        }
        public FloorType PoolFloorType(Document doc, string floorThick)
        {
            FloorType poolFloorType = null;

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(FloorType));
            IList<Element> levelTypeList = collector.ToElements();
            foreach (Element elem in collector)
            {
                FloorType floorType = elem as FloorType;
                if (floorType.Name.Contains("结构_混凝土楼板") && floorType.Name.Contains(floorThick))
                {
                    poolFloorType = floorType;
                    break;
                }
            }
            return poolFloorType;
        }
        public Level GetLevel(Document doc, string levelOffsetValue)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            IList<Element> levelList = collector.ToElements();
            Level level = null;

            foreach (Element e in levelList)
            {
                Level lev = e as Level;
                if (lev.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsValueString() == levelOffsetValue)
                {
                    level = lev;
                }
            }
            return level;
        }
        public bool ElevationExist(Document doc, string levelOffset)
        {
            bool levelExist = false;
            List<string> levelList = GetAllElevation(doc);
            foreach (string item in levelList)
            {
                if (item == levelOffset)
                {
                    levelExist = true;
                    break;
                }
            }
            return levelExist;
        }
        public List<string> GetAllElevation(Document doc)
        {
            // 获取全部标高
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            IList<Element> levelList = collector.ToElements();
            List<string> levelOffset = new List<string>();

            foreach (Element e in levelList)
            {
                Level level = e as Level;
                levelOffset.Add(level.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsValueString());
            }
            return levelOffset;
        }
        public void CreatGridDemesion(Document doc, View activeView, List<Grid> grids)
        {
            //尺寸标注定位点
            XYZ locationPoint = new XYZ();
            Line gridLine = grids.FirstOrDefault().Curve as Line;
            if (gridLine != null)
            {
                XYZ newPoint = gridLine.StartPoint();
                locationPoint = new XYZ(newPoint.X + 400 / 304.8, newPoint.Y + 400 / 304.8, newPoint.Z);
            }

            //获取轴网类型
            DimensionType dimType = null;
            FilteredElementCollector elems = new FilteredElementCollector(doc);
            foreach (DimensionType dt in elems.OfClass(typeof(DimensionType)))
            {
                if (dt.Name.Contains("给排水") && dt.StyleType == DimensionStyleType.Linear)
                {
                    dimType = dt;
                    break;
                }
            }

            if (dimType != null)
            {
                if (grids.Count > 1)
                {
                    ReferenceArray referenceArray1 = new ReferenceArray();
                    ReferenceArray referenceArray2 = new ReferenceArray();
                    //获得最靠近选择点的轴网为参照基准
                    List<Grid> lineGrid = new List<Grid>();
                    Line referenceLine = null;
                    double dis = double.MaxValue;
                    foreach (Grid g in grids)
                    {
                        double d = g.Curve.Distance(locationPoint);
                        Line line = g.Curve as Line;
                        if (line != null)
                        {
                            lineGrid.Add(g);
                            if (d < dis)
                            {
                                referenceLine = line;
                                dis = d;
                            }
                        }
                    }
                    //获得内侧尺寸标注的引用
                    foreach (Grid g in lineGrid)
                    {
                        Line line = g.Curve as Line;
                        if (line.Direction.IsAlmostEqualTo(referenceLine.Direction) || line.Direction.IsAlmostEqualTo(referenceLine.Direction.Multiply(-1)))
                        {
                            referenceArray1.Append(new Reference(g));
                        }
                    }
                    //获取外侧尺寸标注的引用
                    foreach (Reference refGrid in referenceArray1)
                    {
                        Grid g = doc.GetElement(refGrid) as Grid;
                        Line line = g.Curve as Line;
                        XYZ point1 = line.GetEndPoint(0);
                        XYZ point2 = line.GetEndPoint(1);
                        int i = 0;
                        foreach (Reference _refGrid in referenceArray1)
                        {
                            Grid _g = doc.GetElement(_refGrid) as Grid;
                            Line _line = _g.Curve as Line;
                            XYZ point = _line.GetEndPoint(0);
                            if (PointOnTheLeft(point1, point2, point))
                            {
                                i += 1;
                            }
                        }
                        if (i == 0 || i == referenceArray1.Size - 1)
                        {
                            referenceArray2.Append(new Reference(g));
                        }
                    }
                    //计算尺寸标注位置
                    XYZ lineDir = referenceLine.Direction.CrossProduct(new XYZ(0, 0, 1));
                    XYZ point_s = referenceLine.GetEndPoint(0);
                    XYZ point_e = referenceLine.GetEndPoint(1);
                    if (point_s.DistanceTo(locationPoint) > point_e.DistanceTo(locationPoint))
                    {
                        XYZ temPoint = point_s;
                        point_s = point_e;
                        point_e = temPoint;
                    }
                    XYZ offsetDir = point_e - point_s;
                    double lenght = dimType.get_Parameter(BuiltInParameter.TEXT_SIZE).AsDouble();
                    Line line_o = Line.CreateUnbound(locationPoint, lineDir);
                    Line line_i = Line.CreateUnbound(locationPoint + offsetDir.Normalize() * lenght * activeView.Scale * 1.9, lineDir);
                    //创建尺寸标注
                    if (grids.Count > 2)
                    {
                        doc.Create.NewDimension(activeView, line_o, referenceArray2, dimType);
                        doc.Create.NewDimension(activeView, line_i, referenceArray1, dimType);
                    }
                    else
                    {
                        doc.Create.NewDimension(activeView, line_o, referenceArray2, dimType);
                    }
                }
            }
        }
        public bool PointOnTheLeft(XYZ point1, XYZ point2, XYZ point)
        {
            double r = (point1.X - point2.X) / (point1.Y - point2.Y) * (point.Y - point2.Y) + point2.X;
            if (r > point.X)
            {
                return true;
            }
            return false;
        }
        public List<Wall> AddWallByCrossGrids(Document doc, UIApplication uiapp, List<Grid> wallGrids, Level topLevel,
            Level bottomLevel, double offset, bool isSegemention, bool isStructure)
        {
            List<Wall> res = new List<Wall>();

            WallType wallType = GetWallTypeByName(doc, "default_砌体墙");
            if (null == wallType)
            {
                return res;
            }

            foreach (Grid grid in wallGrids)
            {
                List<Curve> curves = Common.GetSegementsWithGrid(uiapp, grid, wallGrids);

                foreach (Curve curve in curves)
                {
                    List<Wall> newWall = CreateSegementionWall(doc, curve, topLevel, bottomLevel, wallType, isSegemention, isStructure, offset);
                    res.AddRange(newWall);
                }
            }

            return res;
        }
        protected List<Wall> CreateSegementionWall(Document doc, Curve curve, Level topLevel, Level bottomLevel, WallType wallType,
                                                  bool isSegemention, bool isStructural, double offset)
        {
            List<Wall> res = new List<Wall>();
            if (isSegemention)
            {
                double bottom = bottomLevel.Elevation;
                double top = topLevel.Elevation;
                List<Level> Levels = Common.GetSortLevels(doc, bottom, top);
                if (Levels.Count < 2)
                {
                    return res;
                }
                int i = 0;
                for (; i < Levels.Count - 1; ++i)
                {
                    Level curBottomLevel = Levels[i];
                    Level curTopLevel = Levels[i + 1];
                    Wall wall = CreateOneWall(doc, curve, curBottomLevel, curTopLevel, wallType, offset, isStructural);
                    wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(0);
                    wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(0);
                    res.Add(wall);
                }
            }
            else
            {
                Wall wall = CreateOneWall(doc, curve, bottomLevel, topLevel, wallType, offset, isStructural);
                wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(0);
                wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(0);
                res.Add(wall);
            }

            return res;
        }
        protected Wall CreateOneWall(Document doc, Curve curve, Level bottomLevel, Level topLevel, WallType wallType,
                                    double offset, bool isStructural)
        {
            double height = topLevel.Elevation - bottomLevel.Elevation;
            Wall wall = Wall.Create(doc, curve, wallType.Id, bottomLevel.Id, height, offset, false, isStructural);
            Parameter p = wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE);
            if (null != p)
            {
                p.Set(topLevel.Id);
            }
            wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(0);
            wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(0);
            return wall;
        }
        protected WallType GetWallTypeByName(Document doc, string walltypename)
        {
            WallType wallType = null;
            IList<WallType> wallTypes = CollectorHelper.TCollector<WallType>(doc);

            foreach (WallType item in wallTypes)
            {
                if (item.Name.Contains(walltypename))
                {
                    wallType = item;
                    break;
                }
            }

            return wallType;
        }
        public void HideGridBubble(List<Grid> grids, View view)
        {
            foreach (var item in grids)
            {
                item.HideBubbleInView(DatumEnds.End0, view);
            }
        }
        /// <summary>
        /// Get current length display unit type
        /// </summary>
        /// <param name="document">Revit's document</param>
        /// <returns>Current length display unit type</returns>
        private static DisplayUnitType GetLengthUnitType(Document document)
        {
            UnitType unittype = UnitType.UT_Length;
            Units projectUnit = document.GetUnits();
            try
            {
                Autodesk.Revit.DB.FormatOptions formatOption = projectUnit.GetFormatOptions(unittype);
                return formatOption.DisplayUnits;
            }
            catch (System.Exception /*e*/)
            {
                return DisplayUnitType.DUT_DECIMAL_FEET;
            }
        }

        /// <summary>
        /// Get all grid labels in current document
        /// </summary>
        /// <param name="document">Revit's document</param>
        /// <returns>ArrayList contains all grid labels in current document</returns>
        private static ArrayList GetAllLabelsOfGrids(Document document)
        {
            ArrayList labels = new ArrayList();
            FilteredElementIterator itor = new FilteredElementCollector(document).OfClass(typeof(Grid)).GetElementIterator();
            itor.Reset();
            for (; itor.MoveNext();)
            {
                Grid grid = itor.Current as Grid;
                if (null != grid)
                {
                    labels.Add(grid.Name);
                }
            }

            return labels;
        }
    }
}
