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

                using (Transaction trans = new Transaction(doc, "轴网生墙"))
                {
                    trans.Start();

                    Level topLevel = GetWallLevel(doc, "4.000");
                    Level bottomLevel = GetWallLevel(doc, "0.000");
                    AddWallByCrossGrids(app, doc, uidoc, topLevel, bottomLevel, 0, false, false);

                    trans.Commit();
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


        public void AddWallByCrossGrids(UIApplication uiapp, Document doc, UIDocument uidoc, Level topLevel, Level bottomLevel, double offset,
                                        bool isSegemention, bool isStructure)
        {

            WallType wallType = GetWallTypeByName(doc, "default_砌体墙");
            if (null == wallType)
            {
                return;
            }

            IList<Reference> selRefs = null;
            Selection sel = uidoc.Selection;
            try
            {
                selRefs = sel.PickObjects(ObjectType.Element, new GridFilter(), "请选择需要生成墙的轴线");
            }
            catch (Exception)
            {
                return;
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
                return;
            }
            foreach (Grid grid in Grids)
            {
                List<Curve> curves = Common.GetSegementsWithGrid(uiapp, grid, Grids);

                foreach (Curve curve in curves)
                {
                    CreateSegementionWall(doc, curve, topLevel, bottomLevel, wallType, isSegemention, isStructure, offset);
                }
            }
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
