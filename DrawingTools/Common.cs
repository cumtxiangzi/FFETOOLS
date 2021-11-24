using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TestBIM
{
    #region Common
    public class Common
    {
        const double _eps = 1.0e-9;

        public class GridAndInfos
        {
            public int m_Nummber;
            public Grid m_Grid;

            public GridAndInfos(int nummber, Grid grid)
            {
                m_Nummber = nummber;
                m_Grid = grid;
            }
            static public bool operator <(GridAndInfos g1, GridAndInfos g2)
            {
                return g1.m_Nummber < g2.m_Nummber;
            }
            static public bool operator >(GridAndInfos g1, GridAndInfos g2)
            {
                return g1.m_Nummber > g2.m_Nummber;
            }

            static public bool operator !=(GridAndInfos g1, GridAndInfos g2)
            {
                return g1.m_Nummber != g2.m_Nummber;
            }
            static public bool operator ==(GridAndInfos g1, GridAndInfos g2)
            {
                return g1.m_Nummber == g2.m_Nummber;
            }

        }


        static public double MMtoIntch(double val)
        {
            return val / 304.8;
        }

        static public double IntchtoMM(double val)
        {
            return val * 304.8;
        }

        static public int Chr2AscII(string character)
        {
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                throw new Exception("Character is not valid.");
            }

        }

        static public string AscII2Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }

        static public bool IsCharacterWithAxisNummber(string name, ref int ascII)
        {
            if (name.Length == 1)
            {
                string chr = new string(name[0], 1);
                ascII = Chr2AscII(chr);
                if ((ascII >= 65 && ascII <= 90) ||
                    (ascII >= 97 && ascII <= 122))
                    return true;
            }
            //int i = 0;
            //for (; i < name.Length; ++i)
            //{
            //    string chr = new string(name[i], 1);
            //    int asc = Chr2AscII(chr);
            //    if (asc >= 65 && asc <= 90)
            //        return true;
            //}
            return false;
        }

        static public bool IsNummbersWithAxisNummber(string name, ref int nummber)
        {
            int i = 0;
            for (; i < name.Length; ++i)
            {
                string chr = new string(name[i], 1);
                int ascII = Chr2AscII(chr);
                if (!(ascII >= 48 && ascII <= 57))
                    return false;
            }
            nummber = System.Convert.ToInt16(name);
            return true;
        }

        static public string CharacterAxiseNummberIncrease(string chrNumb)
        {
            int ascII = -1;
            if (!IsCharacterWithAxisNummber(chrNumb, ref ascII))
            {
                return chrNumb;
            }
            if(ascII == 90 || ascII == 122)
            {
                return chrNumb;
            }
            if (ascII == 72 || ascII == 78 ||
               ascII == 104 || ascII == 110)
            {
                return Common.AscII2Chr(ascII + 2);
            }
            return Common.AscII2Chr(ascII + 1);
        }

        static public string CharacterAxiseNummberDecreasing(string chrNumb)
        {
            int ascII = -1;
            if (!IsCharacterWithAxisNummber(chrNumb, ref ascII))
            {
                return chrNumb;
            }
            if (ascII == 65 || ascII == 97)
            {
                return chrNumb;
            }
            if (ascII == 74 || ascII == 80 ||
               ascII == 106 || ascII == 112)
            {
                return Common.AscII2Chr(ascII - 2);
            }
            return Common.AscII2Chr(ascII - 1);
        }

        static public void AllCharacterAxises(Document doc, ref List<GridAndInfos> nummbers)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> collection = collector.OfClass(typeof(Grid)).ToElements();
            foreach (Element element in collection)
            {
                Grid grid = element as Grid;

                string name = grid.Name;
                int ascII = -1;
                if (IsCharacterWithAxisNummber(name, ref ascII))
                { 
                    string firstChar = new string(name[0], 1);
                    int asc = Chr2AscII(firstChar);
                    GridAndInfos infos = new GridAndInfos(asc, grid);
                    nummbers.Add(infos);
                }
            }
        }

        static public void AllNummbersAxises(Document doc, ref List<GridAndInfos> nummbers)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> collection = collector.OfClass(typeof(Grid)).ToElements();
            foreach (Element element in collection)
            {
                Grid grid = element as Grid;

                string name = grid.Name;
                int ascII = -1;
                if (IsNummbersWithAxisNummber(name, ref ascII))
                {
                    int asc = System.Convert.ToInt16(name);
                    GridAndInfos infos = new GridAndInfos(asc, grid);
                    nummbers.Add(infos);
                }
            }
        }

        static public bool isDuplicationName(Document doc, string testName, ref Grid duplicationNameGrid)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> collection = collector.OfClass(typeof(Grid)).ToElements();
            foreach (Element element in collection)
            {
                Grid grid = element as Grid;
                if (0 == grid.Name.CompareTo(testName))
                {
                    duplicationNameGrid = grid;
                    return true;
                }
            }
            return false;
        }


        static public void ChangeGridName(Document doc, Grid grid, string newName)
        {
            Grid duplicationNameGrid = null;
            if (isDuplicationName(doc, newName, ref duplicationNameGrid))
            {
                string oldName = grid.Name;
                grid.Name = "temp";
                duplicationNameGrid.Name = oldName;
            }
            grid.Name = newName;
        }

        static public void get3dViewName(Document doc, View3D view3d)
        {
            List<int> namesNummber = new List<int>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            FilteredElementIterator itor = collector.OfClass(typeof(Autodesk.Revit.DB.View)).GetElementIterator();
            itor.Reset();
            while (itor.MoveNext())
            {
                Autodesk.Revit.DB.View view = itor.Current as Autodesk.Revit.DB.View;
                if (null == view || view.IsTemplate)
                {
                    continue;
                }
                if (view.ViewType == Autodesk.Revit.DB.ViewType.ThreeD)
                {
                    string name = view.Name;
                    string subName = "局部三维视图";
                    int pos = name.LastIndexOf(subName);
                    if (pos > -1)
                    {
                        string strNummber = name.Substring(pos + subName.Length);
                        int nummber = System.Convert.ToInt32(strNummber);
                        namesNummber.Add(nummber);
                    }
                }
            }
            namesNummber.Sort();
            int maxNummber = 0;
            if (namesNummber.Count > 0)
            {
                maxNummber = namesNummber[namesNummber.Count - 1];
            }
            maxNummber++;

            view3d.Name = "局部三维视图" + maxNummber.ToString();
        }

        static public void UpdateModel(UIDocument uiDoc, bool autoJoin)
        {
            // in order to be able to see changes to the model before 
            // the current transaction is committed, we have to regenerate
            // the model manually.
            uiDoc.Document.Regenerate();

            // auto-joining is optional, but may be necessary to see connection details.
            if (autoJoin)
            {
                uiDoc.Document.AutoJoinElements();
            }

            // to see the changes immediately, we need to refresh the view
            uiDoc.RefreshActiveView();
        }

        static public List<Level> GetSortLevels(Document doc, double bottom, double top)
        {
            List<Level> res = new List<Level>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> collection = collector.OfClass(typeof(Level)).ToElements();
            foreach (Element element in collection)
            {
                Level level = element as Level;
                double elevation = level.Elevation;
                if((elevation >= bottom) && (elevation <= top))
                    res.Add(level);
            }
            res.Sort(new Generics.LevelComparer());
            return res;
        }

        static protected List<Grid> GetAllGridInCurrentView(UIApplication uiApp)
        {
            List<Grid> res = new List<Grid>();
            ElementId viewId = uiApp.ActiveUIDocument.ActiveView.Id;
            FilteredElementCollector collector = new FilteredElementCollector(uiApp.ActiveUIDocument.Document, viewId);
            ICollection<Element> collection = collector.OfClass(typeof(Grid)).ToElements();
            foreach (Element element in collection)
            {
                Grid grid = element as Grid;
                res.Add(grid);
            }
            return res;
        }

        static protected List<XYZ> IntersectGridAndGrids(Grid selGrid, List<Grid> grids)
        {
            Curve SelGridCurve = selGrid.Curve;
            List<XYZ> intersectPoints = new List<XYZ>();
            foreach (Element element in grids)
            {
                Grid grid = element as Grid;
                if (grid.Id == selGrid.Id)
                    continue;

                Curve gridCurve = grid.Curve;

                IntersectionResultArray resultArray;
                SetComparisonResult resIntersect = gridCurve.Intersect(SelGridCurve, out resultArray);
                if (resIntersect == Autodesk.Revit.DB.SetComparisonResult.Overlap)
                {
                    foreach (IntersectionResult interResult in resultArray)
                    {
                        if (null != interResult.XYZPoint)
                            intersectPoints.Add(interResult.XYZPoint);
                    }
                }
            }
            List<XYZ> res = UniqueAndSortIntersectPoints(SelGridCurve, intersectPoints);
            return res;
        }

        static public Curve GetPickSegementWithGrid(UIApplication uiApp, Grid selGrid, XYZ pickPos)
        {
            Curve SelGridCurve = selGrid.Curve;
            List<Grid> allGrids = GetAllGridInCurrentView(uiApp);
            List<XYZ> intersectPoints = IntersectGridAndGrids(selGrid, allGrids);
            if (intersectPoints.Count < 2)
                return SelGridCurve;            

            double minDistance = Double.MaxValue;
            int i = 0, findIndex = -1;
            for (; i < intersectPoints.Count - 1; ++i )
            {
                XYZ pt1 = intersectPoints[i];
                XYZ pt2 = intersectPoints[i + 1];
                double distance = pt1.DistanceTo(pickPos);
                distance += pt2.DistanceTo(pickPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    findIndex = i;
                }
            }
            if (findIndex == -1)
                return null;
            Curve res = null;
            Arc arc = SelGridCurve as Arc;
            if (null != arc)
            {
                XYZ ptStart = intersectPoints[findIndex];
                XYZ ptEnd = intersectPoints[findIndex+1];
                XYZ ptCenter = arc.Center;
                double angleStart = XYZ.BasisX.AngleOnPlaneTo(ptStart-ptCenter, XYZ.BasisZ);
                double angleEnd = XYZ.BasisX.AngleOnPlaneTo(ptEnd-ptCenter, XYZ.BasisZ);
                Arc newArc = uiApp.Application.Create.NewArc(arc.Center, arc.Radius, angleStart, angleEnd, XYZ.BasisX, XYZ.BasisY);
                res = newArc;
            }
            Line line = SelGridCurve as Line;
            if (null != line)
            {
                Line newline = Line.get_Bound(intersectPoints[findIndex], intersectPoints[findIndex + 1]);
                res = newline;
            }
            return res;
        }

        static public List<Curve> GetSegementsWithGrid(UIApplication uiApp, Grid selGrid, List<Grid> allGrids)
        {
            List<Curve> res = new List<Curve>();
            Curve SelGridCurve = selGrid.Curve;
            if(null == allGrids)
                allGrids = GetAllGridInCurrentView(uiApp);
            List<XYZ> intersectPoints = IntersectGridAndGrids(selGrid, allGrids);
            if (intersectPoints.Count < 2)
            {
                res.Add(SelGridCurve);
                return res;
            }

            int i = 0;
            for (; i < intersectPoints.Count - 1; ++i )
            {
                Arc arc = SelGridCurve as Arc;
                if (null != arc)
                {
                    XYZ ptStart = intersectPoints[i];
                    XYZ ptEnd = intersectPoints[i+1];
                    XYZ ptCenter = arc.Center;
                    double angleStart = XYZ.BasisX.AngleOnPlaneTo(ptStart - ptCenter, XYZ.BasisZ);
                    double angleEnd = XYZ.BasisX.AngleOnPlaneTo(ptEnd - ptCenter, XYZ.BasisZ);
                    if (angleEnd < angleStart)
                        angleStart -= Math.PI * 2;
                    Arc newArc = uiApp.Application.Create.NewArc(arc.Center, arc.Radius, angleStart, angleEnd, XYZ.BasisX, XYZ.BasisY);
                    res.Add(newArc);
                }
                Line line = SelGridCurve as Line;
                if (null != line)
                {
                    Line newline = Line.get_Bound(intersectPoints[i], intersectPoints[i+1]);
                    res.Add(newline);
                }
            }
            return res;
        }

        static public List<XYZ> UniqueAndSortIntersectPoints(Curve curve, List<XYZ> intersectPoints)
        {
            Arc arc = curve as Arc;
            if (null != arc)
            {
                XYZ ptStart = arc.get_EndPoint(0);
                XYZ normal = arc.Normal;
                if (normal.Z < 0)
                    ptStart = arc.get_EndPoint(1);
                intersectPoints.Sort(new Generics.AngleComparer(ptStart, arc.Center));
            }
            Line line = curve as Line;
            if (null != line)
            {
                XYZ ptStart = line.get_EndPoint(0);
                intersectPoints.Sort(new Generics.DistanceComparer(ptStart));
            }

            IEnumerable<XYZ> pts = intersectPoints.Distinct(new Generics.Comparint<XYZ>(delegate(XYZ x, XYZ y)
            {
                if (null != x && null != y)
                    return (x.DistanceTo(y) < 0.001);
                return false;
            }));
            List<XYZ> res = new List<XYZ>();
            foreach(XYZ pt in pts)
            {
                res.Add(pt);
            }
            return res;
        }
    }
    #endregion

    public class FamilyInfos
    {
        public string m_FamilyName;
        public List<ElementId> m_FamilySymbolsId = new List<ElementId>();
    }

    public class FamilyNameInfos
    {
        public string m_FamilyName;
        public List<string> m_FamilySymbolsName = new List<string>();
    }


}
