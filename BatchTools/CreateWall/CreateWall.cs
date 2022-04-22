using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;

namespace FFETOOLS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class CreateWall : IExternalCommand
    {
        public Result Execute(Autodesk.Revit.UI.ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            //Transaction newTran = null;
            CreateWallForm createWallForm = null;
            UIDocument uidoc = cmdData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                if (null == cmdData)
                {
                    throw new ArgumentNullException("commandData");
                }

                WallCreater creater = new WallCreater(cmdData);
                createWallForm = new CreateWallForm(creater,doc);

                createWallForm.Show();

                return Autodesk.Revit.UI.Result.Succeeded;
            }
            catch (Exception e)
            {
                msg = e.Message;
                if (null != createWallForm)
                    createWallForm.Close();
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
    }

    public class WallCreater
    {
        private Autodesk.Revit.UI.ExternalCommandData m_Revit;
        private List<ElementId> m_WallTypeIds = new List<ElementId>();
        private List<ElementId> m_LevelIds = new List<ElementId>();

        public List<string> WallTypeInfos
        {
            get
            {
                List<string> res = new List<string>();
                foreach (ElementId id in m_WallTypeIds)
                {
                    Element ele = m_Revit.Application.ActiveUIDocument.Document.GetElement(id);
                    res.Add(ele.Name);
                }
                return res;
            }
        }

        public List<string> LevelInfos
        {
            get
            {
                List<string> res = new List<string>();
                foreach (ElementId id in m_LevelIds)
                {
                    Element ele = m_Revit.Application.ActiveUIDocument.Document.GetElement(id);
                    res.Add(ele.Name);
                }
                return res;
            }
        }

        public WallCreater(Autodesk.Revit.UI.ExternalCommandData cmdData)
        {
            m_Revit = cmdData;
            InitWallTypeList();
            InitLevelList();
        }

        protected void InitWallTypeList()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_Revit.Application.ActiveUIDocument.Document);
            IList<Element> familySymbols = collector.OfClass(typeof(WallType)).ToElements();
            foreach (Element element in familySymbols)
            {
                WallType wallType = element as WallType;
                if (null != wallType)
                {
                    m_WallTypeIds.Add(wallType.Id);
                }
            }
        }

        protected void InitLevelList()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_Revit.Application.ActiveUIDocument.Document);
            ICollection<Element> collection = collector.OfClass(typeof(Level)).ToElements();
            foreach (Element element in collection)
            {
                Level systemLevel = element as Level;
                if (null != systemLevel)
                {
                    m_LevelIds.Add(systemLevel.Id);
                }
            }
        }

        protected Element GetWallTypeByIndex(int wallTypeIndex)
        {
            Element res = null;
            if (wallTypeIndex < 0 || wallTypeIndex >= m_WallTypeIds.Count)
                return res;

            ElementId id = m_WallTypeIds[wallTypeIndex];
            res = m_Revit.Application.ActiveUIDocument.Document.GetElement(id);
            return res;
        }

        protected Element GetLevelByIndex(int levelIndex)
        {
            Element res = null;
            int i = 0;
            foreach (ElementId id in m_LevelIds)
            {
                if (i == levelIndex)
                {
                    res = m_Revit.Application.ActiveUIDocument.Document.GetElement(id);
                }
                i++;
            }
            return res;
        }

        public void AddWallByGridSegement(string transeformName, int wallTypeIndex,
                                          int topLevelIndex, int bottomLevelIndex, double offset,
                                          bool isSegemention, bool isStructure)
        {
            Transaction newTran = null;
            Document doc = m_Revit.Application.ActiveUIDocument.Document;
            newTran = new Transaction(doc);
            newTran.Start(transeformName);

            WallType wallType = GetWallTypeByIndex(wallTypeIndex) as WallType;
            if (null == wallType)
            {
                if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                    newTran.RollBack();
                return;
            }
            Level topLevel = GetLevelByIndex(topLevelIndex) as Level;
            Level bottomLevel = GetLevelByIndex(bottomLevelIndex) as Level;

            Reference reference = null;
            Autodesk.Revit.UI.Selection.Selection sel = m_Revit.Application.ActiveUIDocument.Selection;
            try
            {
                reference = sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "select a segement with axis");
            }
            catch (Exception)
            {
                if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                    newTran.RollBack();
                return;
            }
            Element ele = doc.GetElement(reference);

            Grid grid = ele as Grid;

            if (null == grid)
            {
                MessageBox.Show("isn't aixs");
                if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                    newTran.RollBack();
                return;
            }

            XYZ pickPos = reference.GlobalPoint;

            Curve curve = Common.GetPickSegementWithGrid(m_Revit.Application, grid, pickPos);

            CreateSegementionWall(doc,curve, topLevel, bottomLevel, wallType, isSegemention, isStructure, offset);

            newTran.Commit();
        }

        public void AddWallBySingleGrid(string transeformName, int wallTypeIndex,
                                        int topLevelIndex, int bottomLevelIndex, double offset,
                                        bool isSegemention, bool isStructure)
        {
            Transaction newTran = null;
            Document doc = m_Revit.Application.ActiveUIDocument.Document;
            newTran = new Transaction(doc);
            newTran.Start(transeformName);

            WallType wallType = GetWallTypeByIndex(wallTypeIndex) as WallType;
            if (null == wallType)
            {
                if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                    newTran.RollBack();
                return;
            }
            Level topLevel = GetLevelByIndex(topLevelIndex) as Level;
            Level bottomLevel = GetLevelByIndex(bottomLevelIndex) as Level;

            Reference reference = null;
            Autodesk.Revit.UI.Selection.Selection sel = m_Revit.Application.ActiveUIDocument.Selection;
            try
            {
                reference = sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "select a segement with axis");
            }
            catch (Exception)
            {
                if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                    newTran.RollBack();
                return;
            }
            Element ele = doc.GetElement(reference);

            Grid grid = ele as Grid;

            if (null == grid)
            {
                MessageBox.Show("isn't aixs");
                if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                    newTran.RollBack();
                return;
            }

            List<Grid> nullGridList = null;
            List<Curve> curves = Common.GetSegementsWithGrid(m_Revit.Application, grid, nullGridList);

            foreach (Curve curve in curves)
            {
                CreateSegementionWall(doc,curve, topLevel, bottomLevel, wallType, isSegemention, isStructure, offset);
            }
            newTran.Commit();
        }

        public void AddWallByCrossGrids(Document doc, string transeformName, int wallTypeIndex,
                                        int topLevelIndex, int bottomLevelIndex, double offset,
                                        bool isSegemention, bool isStructure)
        {
            Transaction trans = new Transaction(doc, "Éú³É±ê×¢");
            trans.Start();

            WallType wallType = GetWallTypeByIndex(wallTypeIndex) as WallType;
            if (null == wallType)
            {
                //if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                //    newTran.RollBack();
                return;
            }
            Level topLevel = GetLevelByIndex(topLevelIndex) as Level;
            Level bottomLevel = GetLevelByIndex(bottomLevelIndex) as Level;


            IList<Reference> selRefs = null;
            Autodesk.Revit.UI.Selection.Selection sel = m_Revit.Application.ActiveUIDocument.Selection;
            try
            {
                selRefs = sel.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "select axises");
            }
            catch (Exception)
            {
                //if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                //    newTran.RollBack();
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
                MessageBox.Show("not have select axis");
                //if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                //    newTran.RollBack();
                return;
            }
            foreach (Grid grid in Grids)
            {
                List<Curve> curves = Common.GetSegementsWithGrid(m_Revit.Application, grid, Grids);

                foreach (Curve curve in curves)
                {
                    CreateSegementionWall(doc,curve, topLevel, bottomLevel, wallType, isSegemention, isStructure, offset);
                }
            }

            trans.Commit();
        }


        protected List<Wall> CreateSegementionWall(Document doc, Curve curve, Level topLevel, Level bottomLevel, WallType wallType,
                                                   bool isSegemention, bool isStructural, double offset)
        {
            List<Wall> res = new List<Wall>();
            if (isSegemention)
            {
                double bottom = bottomLevel.Elevation;
                double top = topLevel.Elevation;
                List<Level> Levels = Common.GetSortLevels(m_Revit.Application.ActiveUIDocument.Document, bottom, top);
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

            //if (null != wall)
            //{
            //    Parameter baseLevelParameter = wall.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
            //    Parameter topLevelParameter = wall.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);

            //    if (null != baseLevelParameter)
            //    {
            //        baseLevelParameter.Set(bottomLevel.Id);
            //    }

            //    if (null != topLevelParameter)
            //    {
            //        topLevelParameter.Set(topLevel.Id);
            //    }
            //}
            return wall;
        }
    }
}


