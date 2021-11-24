using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinForm = System.Windows.Forms;
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
using System.Xml;


namespace FFETOOLS
{
    //找洞口
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreatWallOpening : IExternalCommand
    {
        public Result Execute(ExternalCommandData cmdData, ref string messages, ElementSet elements)
        {
            UIApplication uiApp = cmdData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;

            Transaction ts = new Transaction(doc, "批量墙上开洞");
            ts.Start();
            /*
            //选择一面墙
            WallSelectionFilter fWall = new WallSelectionFilter();
            Reference ref1 = uiApp.ActiveUIDocument.Selection.PickObject(ObjectType.Element, fWall, "选择一面墙：");
            Element elem1 = doc.GetElement(ref1);
            Wall wall = elem1 as Wall;
            //选择一个风管
            DuctSelectionFilter fDuct = new DuctSelectionFilter();
            Reference ref2 = uiApp.ActiveUIDocument.Selection.PickObject(ObjectType.Element, fDuct, "选择一个风管：");
            Element elem2 = doc.GetElement(ref2);
            Duct duct = elem2 as Duct;
             */
            //开同心洞
            //CenterOpen(doc, wall, duct, 640, 640);
            List<Pipe> listPipe = FindAllPipeW(doc);
            foreach (Pipe pipe in listPipe)
            {
                double pipeDN = pipe.LookupParameter("直径").AsDouble() * 304.8 + 100;
                List<Wall> listWall = FindPipeWall(doc, pipe);
                foreach (Wall wall in listWall)
                {
                    CenterOpen(doc, wall, pipe, pipeDN, pipeDN);
                }
            }

            ts.Commit();

            return Result.Succeeded;
        }
        //找到所有水管
        List<Pipe> FindAllPipeW(Document doc)
        {
            List<Pipe> listPipe = new List<Pipe>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Pipe)).OfCategory(BuiltInCategory.OST_PipeCurves);

            foreach (Element el in collector)
            {
                Pipe pipe = el as Pipe;
                if ((pipe != null)&&(pipe.Name.Contains("给排水")))
                    listPipe.Add(pipe);
            }
            return listPipe;
        }
        //找到与水管相交的墙
        List<Wall> FindPipeWall(Document doc, Pipe pipe)
        {
            List<Wall> listWall = new List<Wall>();
            //找到outLine
            BoundingBoxXYZ bb = pipe.get_BoundingBox(doc.ActiveView);
            Outline outline = new Outline(bb.Min, bb.Max);
            //
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            BoundingBoxIntersectsFilter invertFilter = new BoundingBoxIntersectsFilter(outline, false);
            IList<Element> noIntersectWalls = collector.OfClass(typeof(Wall)).WherePasses(invertFilter).ToElements();
            foreach (Element el in noIntersectWalls)
            {
                Wall wall = el as Wall;
                if (wall != null)
                    listWall.Add(wall);
            }
            return listWall;
        }
        //开同心洞
        Result CenterOpen(Document doc, Wall wall, Pipe pipe, double dWidth, double dHeigh)
        {
            SubTransaction subTs = new SubTransaction(doc);
            subTs.Start();
            try
            {
                //求面和线的交点
                Face face = FindWallFace(wall);
                Curve curve = FindPipeCurve(pipe);
                XYZ xyz = FindFaceCurve(face, curve);
                ////墙线的向量
                //XYZ wallVector = FindWallVector(wall);
                ////交点向上向墙线正方向移动160(风管宽高320)
                //XYZ pt1 = xyz + new XYZ(0, 0, 1) * dHeigh / 2 / 304.8;
                //pt1 = pt1 + wallVector.Normalize() * dWidth / 2 / 304.8;
                ////交点向下向墙线反方向移动160(风管宽高320)
                //XYZ pt2 = xyz + new XYZ(0, 0, -1) * dHeigh / 2 / 304.8;
                //pt2 = pt2 - wallVector.Normalize() * dWidth / 2 / 304.8;
                ////开洞
                //doc.Create.NewOpening(wall, pt1, pt2);

                IList<Element> openingSymbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel).ToElements();
                FamilySymbol openingSymbol = null;
                foreach (Element os in openingSymbols)
                {
                    FamilySymbol oss = os as FamilySymbol;
                    if (oss.Name.Contains("矩形开孔"))
                    {
                        openingSymbol = oss;
                        break;
                    }
                }

                if (!openingSymbol.IsActive)
                {
                    openingSymbol.Activate();
                }

                FamilyInstance opening = doc.Create.NewFamilyInstance(face, xyz, new XYZ(0,0,0), openingSymbol);
                doc.Regenerate();
                double wallThick = wall.Width*304.8;
                Parameter openingHeight = opening.LookupParameter("孔深");
                openingHeight.SetValueString(wallThick.ToString());
                double pipeDN = Convert.ToDouble(pipe.LookupParameter("尺寸").AsString());
                double openingDN = pipeDN + 100;
                Parameter openingWidth = opening.LookupParameter("孔宽");
                openingWidth.SetValueString(openingDN.ToString());
                Parameter openingLength = opening.LookupParameter("孔长");
                openingLength.SetValueString(openingDN.ToString());


                subTs.Commit();
                return Result.Succeeded;
            }
            catch
            {
                subTs.RollBack();
                return Result.Failed;
            }
        }
        //找到墙线的向量
        XYZ FindWallVector(Wall wall)
        {
            LocationCurve lCurve = wall.Location as LocationCurve;
            XYZ xyz = lCurve.Curve.GetEndPoint(1) - lCurve.Curve.GetEndPoint(0);
            return xyz;
        }
        //求线和面的交点
        XYZ FindFaceCurve(Face face, Curve curve)
        {
            //求交点
            IntersectionResultArray intersectionR = new IntersectionResultArray();//交点集合
            SetComparisonResult comparisonR;//Comparison比较
            comparisonR = face.Intersect(curve, out intersectionR);
            XYZ intersectionResult = null;//交点坐标
            if (SetComparisonResult.Disjoint != comparisonR)//Disjoint不交
            {
                if (!intersectionR.IsEmpty)
                {
                    intersectionResult = intersectionR.get_Item(0).XYZPoint;
                }
            }
            return intersectionResult;
        }
        //找到水管对应的曲线
        Curve FindPipeCurve(Pipe pipe)
        {
            //得到水管曲线
            IList<XYZ> list = new List<XYZ>();
            ConnectorSetIterator csi = pipe.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                list.Add(conn.Origin);
            }
            Curve curve = Line.CreateBound(list.ElementAt(0), list.ElementAt(1)) as Curve;
            return curve;
        }
        //找到墙的正面
        Face FindWallFace(Wall wall)
        {
            Face normalFace = null;
            //
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = Autodesk.Revit.DB.ViewDetailLevel.Medium;
            //
            GeometryElement e = wall.get_Geometry(opt);
            foreach (GeometryObject obj in e)
            {
                Solid solid = obj as Solid;
                if (solid != null && solid.Faces.Size > 0)
                {
                    foreach (Face face in solid.Faces)
                    {
                        PlanarFace pf = face as PlanarFace;
                        if (pf != null)
                        {
                            if (pf.FaceNormal.AngleTo(wall.Orientation) < 0.01)//数值在0到PI之间
                            {
                                normalFace = face;
                            }
                        }
                    }
                }
            }
            return normalFace;
        }
    }   
}
