using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatWallByGrid : IExternalCommand
    {
        public static CreatWallByGridForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new CreatWallByGridForm();
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
    public class ExecuteEventCreatWallByGrid : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
                View activeView = doc.ActiveView;

                ViewPlan roomBottomPlan = null;
                ViewPlan viewPlan = null;
                ElementId newViewId = ElementId.InvalidElementId;
                Level roomBottomlevel = null;
                Level roomToplevel = null;

                if (activeView.IsTemplate != true && activeView.ViewType == ViewType.FloorPlan)
                {
                    TransactionGroup tg = new TransactionGroup(doc, "轴网生墙");
                    tg.Start();

                    string topElevation = CreatWallByGrid.mainfrm.TopElevation.Text;
                    string bottomElevation = CreatWallByGrid.mainfrm.BottomElevation.Text;
                    List<Grid> newGrids = new List<Grid>();

                    using (Transaction trans = new Transaction(doc, "创建墙"))
                    {
                        trans.Start();

                        if (ElevationExist(doc, (double.Parse(topElevation) * 1000).ToString()))
                        {
                            roomToplevel = GetLevel(doc, (double.Parse(topElevation) * 1000).ToString());
                        }
                        else
                        {
                            roomToplevel = Level.Create(doc, double.Parse(topElevation) * 1000 / 304.8);
                            roomToplevel.Name = double.Parse(topElevation).ToString("0.000");
                            viewPlan = ViewPlan.Create(doc, GetViewFamilyType(doc).Id, roomToplevel.Id);//为新建的标高创建对应的视图                    
                        }

                        if (ElevationExist(doc, (double.Parse(bottomElevation) * 1000).ToString()))
                        {
                            roomBottomlevel = GetLevel(doc, (double.Parse(bottomElevation) * 1000).ToString());
                        }
                        else
                        {
                            roomBottomlevel = Level.Create(doc, double.Parse(bottomElevation) * 1000 / 304.8);
                            roomBottomlevel.Name = double.Parse(bottomElevation).ToString("0.000");
                            viewPlan = ViewPlan.Create(doc, GetViewFamilyType(doc).Id, roomBottomlevel.Id);//为新建的标高创建对应的视图                    
                        }

                        newGrids = AddWallByCrossGrids(app, doc, uidoc, roomToplevel, roomBottomlevel, 0, false, false);
                        trans.Commit();
                    }

                    Floor roomBottomFloor = null;
                    if (CreatWallByGrid.mainfrm.CreatGround.IsChecked == true || CreatWallByGrid.mainfrm.CreatRoof.IsChecked == true)
                    {
                        Floor roomTopFloor = null;
                        double roomWallThick = 100 / 304.8;

                        using (Transaction trans = new Transaction(doc, "创建楼板或屋顶"))
                        {
                            trans.Start();

                            List<XYZ> gridCrossPoints = GetGridCrossPoints(newGrids);

                            gridCrossPoints.Sort((a, b) => a.X.CompareTo(b.X));
                            double minX = gridCrossPoints[0].X;
                            double maxX = gridCrossPoints[gridCrossPoints.Count - 1].X;

                            gridCrossPoints.Sort((a, b) => a.Y.CompareTo(b.Y));
                            double minY = gridCrossPoints[0].Y;
                            double maxY = gridCrossPoints[gridCrossPoints.Count - 1].Y;

                            CurveArray array = new CurveArray();
                            XYZ point11 = new XYZ(minX - roomWallThick, minY - roomWallThick, 0);
                            XYZ point21 = new XYZ(minX - roomWallThick, maxY + roomWallThick, 0);
                            XYZ point31 = new XYZ(maxX + roomWallThick, maxY + roomWallThick, 0);
                            XYZ point41 = new XYZ(maxX + roomWallThick, minY - roomWallThick, 0);
                            array.Append(Line.CreateBound(point11, point21));
                            array.Append(Line.CreateBound(point21, point31));
                            array.Append(Line.CreateBound(point31, point41));
                            array.Append(Line.CreateBound(point41, point11));

                            CurveArray array2 = new CurveArray();
                            XYZ point12 = new XYZ(minX - roomWallThick - 800 / 304.8, minY - roomWallThick - 800 / 304.8, 0);
                            XYZ point22 = new XYZ(minX - roomWallThick - 800 / 304.8, maxY + roomWallThick + 800 / 304.8, 0);
                            XYZ point32 = new XYZ(maxX + roomWallThick + 800 / 304.8, maxY + roomWallThick + 800 / 304.8, 0);
                            XYZ point42 = new XYZ(maxX + roomWallThick + 800 / 304.8, minY - roomWallThick - 800 / 304.8, 0);
                            array2.Append(Line.CreateBound(point12, point22));
                            array2.Append(Line.CreateBound(point22, point32));
                            array2.Append(Line.CreateBound(point32, point42));
                            array2.Append(Line.CreateBound(point42, point12));

                            if (CreatWallByGrid.mainfrm.CreatGround.IsChecked == true)
                            {
                                roomBottomFloor = doc.Create.NewFloor(array, PoolFloorType(doc, "100"), GetLevel(doc, 0.ToString()), true, XYZ.BasisZ);//创建±0.000平面楼板
                                double height=double.Parse(CreatWallByGrid.mainfrm.BottomElevation.Text);
                                roomBottomFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(height*1000/304.8);
                            }

                            if (CreatWallByGrid.mainfrm.CreatRoof.IsChecked == true)
                            {
                                roomTopFloor = doc.Create.NewFloor(array2, PoolFloorType(doc, "100"), roomToplevel, true, XYZ.BasisZ);//创建屋顶楼板
                                double topFloorThick = roomTopFloor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();
                                roomTopFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(100 / 304.8);
                            }

                            trans.Commit();
                        }
                    }

                    if (CreatWallByGrid.mainfrm.CreatSlab.IsChecked==true)
                    {
                        using (Transaction trans = new Transaction(doc, "创建散水"))
                        {
                            trans.Start();

                            GetSlabEdge(doc, roomBottomFloor);//创建散水

                            trans.Commit();
                        }
                    }

                    tg.Assimilate();
                }
                else
                {
                    MessageBox.Show("请在平面视图中操作", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "轴网生墙";
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
                    foreach (Face face in Getupfaces(solid))
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
            newEdge.get_Parameter(BuiltInParameter.SWEEP_BASE_VERT_OFFSET).Set(20 / 304.8);
        }
        public List<Face> Getupfaces(Solid solid)
        {
            var upfaces = new List<Face>();
            var faces = solid.Faces;
            foreach (Face face in faces)
            {
                var normal = face.ComputeNormal(new UV());
                if (normal.IsSameDirection(XYZ.BasisZ))
                {
                    upfaces.Add(face);
                }
            }
            return upfaces;
        }
        public List<XYZ> GetGridCrossPoints(List<Grid> grids)
        {
            List<Line> gridLines = new List<Line>();//创建轴线List
            List<XYZ> intPos = new List<XYZ>();//创建交点List

            foreach (Grid gri in grids)
            {
                gridLines.Add(gri.Curve as Line); //将轴网转换为线
            }

            foreach (Line ln1 in gridLines)//找到第一根线
            {
                foreach (Line ln2 in gridLines)//找到第二根线
                {
                    XYZ normal1 = ln1.Direction;//得到的是直线的方向向量
                    XYZ normal2 = ln2.Direction;

                    //如果两根轴线方向相同,则遍历下一组（目的是排除平行与重合的线）
                    if (normal1.IsAlmostEqualTo(normal2)) continue;
                    IntersectionResultArray results;
                    SetComparisonResult intRst = ln1.Intersect(ln2, out results);//如果两根轴线相交,则输出交点

                    if (intRst == SetComparisonResult.Overlap && results.Size == 1)//排除轴线是曲线，交点不止一个的情况
                    {
                        XYZ tp = results.get_Item(0).XYZPoint;//上面得到的交点
                        //比较得到的交点和intPos数组里面的元素是否相同，不同才Add到intPos数组中，作用是排除重复的点
                        if (intPos.Where(m => m.IsAlmostEqualTo(tp)).Count() == 0)
                        {
                            intPos.Add(tp); //收集所有的交点
                        }
                    }
                }
            }

            return intPos;
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
        public void AddWallByGridSegement(UIApplication uiapp, Document doc, UIDocument uidoc, Level topLevel, Level bottomLevel,
           double offset, bool isSegemention, bool isStructure)
        {
            WallType wallType = GetWallTypeByName(doc, "default_砌体墙");
            if (null == wallType)
            {
                return;
            }

            Reference reference = null;
            Selection sel = uidoc.Selection;
            try
            {
                reference = sel.PickObject(ObjectType.Element, new GridFilter(), "select a segement with axis");
            }
            catch (Exception)
            {
                return;
            }

            Element ele = doc.GetElement(reference);
            Grid grid = ele as Grid;

            if (null == grid)
            {
                return;
            }

            XYZ pickPos = reference.GlobalPoint;
            Curve curve = Common.GetPickSegementWithGrid(uiapp, grid, pickPos);
            CreateSegementionWall(doc, curve, topLevel, bottomLevel, wallType, isSegemention, isStructure, offset);
        }


        public List<Grid> AddWallByCrossGrids(UIApplication uiapp, Document doc, UIDocument uidoc, Level topLevel, Level bottomLevel, double offset,
                                        bool isSegemention, bool isStructure)
        {

            WallType wallType = GetWallTypeByName(doc, "default_砌体墙");
            if (null == wallType)
            {

            }

            IList<Reference> selRefs = null;
            Selection sel = uidoc.Selection;
            try
            {
                selRefs = sel.PickObjects(ObjectType.Element, new GridFilter(), "请选择需要生成墙的轴线");
            }
            catch (Exception)
            {

            }

            List<Grid> Grids = new List<Grid>();
            foreach (Reference aRef in selRefs)
            {
                Element ele = doc.GetElement(aRef);
                Grid grid = ele as Grid;
                if (null != grid)
                {
                    Grids.Add(grid);
                }
            }

            if (Grids.Count < 1)
            {

            }
            foreach (Grid grid in Grids)
            {
                List<Curve> curves = Common.GetSegementsWithGrid(uiapp, grid, Grids);

                foreach (Curve curve in curves)
                {
                    CreateSegementionWall(doc, curve, topLevel, bottomLevel, wallType, isSegemention, isStructure, offset);
                }
            }

            return Grids;
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
                    res.Add(wall);
                }
            }
            else
            {
                Wall wall = CreateOneWall(doc, curve, bottomLevel, topLevel, wallType, offset, isStructural);
                res.Add(wall);
            }

            return res;
        }

        protected Wall CreateOneWall(Document doc, Curve curve, Level bottomLevel, Level topLevel, WallType wallType,
                                     double offset, bool isStructural)
        {
            double height = topLevel.Elevation - bottomLevel.Elevation;
            Wall wall = Wall.Create(doc, curve, wallType.Id, bottomLevel.Id, height, offset, false, isStructural);
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
        public static Level GetWallLevel(Document doc, string Levelname)
        {
            // 获取标高
            Level newlevel = null;
            var levelFilter = new ElementClassFilter(typeof(Level));
            FilteredElementCollector levels = new FilteredElementCollector(doc);
            levels = levels.WherePasses(levelFilter);
            foreach (Level level in levels)
            {
                if (level.Name == Levelname)
                {
                    newlevel = level;
                    break;
                }
            }
            return newlevel;
        }
    }
}
