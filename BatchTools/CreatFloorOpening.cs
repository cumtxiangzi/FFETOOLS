using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Creation;
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
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class CreatFloorOpening : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Autodesk.Revit.DB.Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;

            try
            {
                using (Transaction ts = new Transaction(doc, "批量楼板开洞"))
                {
                    ts.Start();

                    List<Pipe> listPipe = FindAllPipeW(doc);
                    foreach (Pipe pipe in listPipe)
                    {
                        double pipeDN = pipe.LookupParameter("直径").AsDouble() * 304.8 + 100;
                        List<Floor> listFloor = FindPipeFloor(doc, pipe);
                        foreach (Floor floor in listFloor)
                        {
                            CenterOpen(doc, floor, pipe, pipeDN, pipeDN);
                        }
                    }

                    ts.Commit();
                }
                return Result.Succeeded;

            }
            catch (Exception)
            {
                //TaskDialog.Show("警告","请载入开孔族");
                //return Result.Failed;
                throw;
            }

        }

        //找到所有水管
        List<Pipe> FindAllPipeW(Autodesk.Revit.DB.Document doc)
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

        //找到与水管相交的楼板
        List<Floor> FindPipeFloor(Autodesk.Revit.DB.Document doc, Pipe pipe)
        {
            List<Floor> listFloor = new List<Floor>();
            //找到outLine
            BoundingBoxXYZ bb = pipe.get_BoundingBox(doc.ActiveView);
            Outline outline = new Outline(bb.Min, bb.Max);
            //
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            BoundingBoxIntersectsFilter invertFilter = new BoundingBoxIntersectsFilter(outline, false);
            IList<Element> noIntersectFloors = collector.OfClass(typeof(Floor)).WherePasses(invertFilter).ToElements();
            foreach (Element el in noIntersectFloors)
            {
                Floor floor = el as Floor;
                if (floor != null)
                    listFloor.Add(floor);
            }
            return listFloor;
        }

        Result CenterOpen(Autodesk.Revit.DB.Document doc, Floor floor, Pipe pipe, double dWidth, double dHeigh)
        {
            SubTransaction subTs = new SubTransaction(doc);
            subTs.Start();
            try
            {
                //求面和线的交点
                Face face = FindFloorFace(floor);
                Curve curve = FindPipeCurve(pipe);
                XYZ xyz = FindFaceCurve(face, curve);

                IList<Element> openingSymbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel).ToElements();
                FamilySymbol openingSymbol = null;
                foreach (Element os in openingSymbols)
                {
                    FamilySymbol oss = os as FamilySymbol;
                    if (oss.Name.Contains("圆形开孔"))
                    {
                        openingSymbol = oss;
                        break;
                    }
                }

                if (!openingSymbol.IsActive)
                {
                    openingSymbol.Activate();
                }

                FamilyInstance opening = doc.Create.NewFamilyInstance(face, xyz, new XYZ(0, 0, 0), openingSymbol);
                doc.Regenerate();
                String floorThick = floor.LookupParameter("厚度").AsValueString();
                Parameter openingHeight = opening.LookupParameter("孔深");
                openingHeight.SetValueString(floorThick);
                double pipeDN = Convert.ToDouble(pipe.LookupParameter("尺寸").AsString());
                double openingDN = pipeDN + 100;
                Parameter openingDia = opening.LookupParameter("孔直径");
                openingDia.SetValueString(openingDN.ToString());

                subTs.Commit();
                return Result.Succeeded;
            }
            catch
            {
                subTs.RollBack();
                return Result.Failed;
            }
        }

        //找到楼板线的向量
        XYZ FindFloorVector(Floor floor)
        {
            LocationCurve lCurve = floor.Location as LocationCurve;
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
        //找到楼板的面
         Face FindFloorFace(Floor floor)
        {
            Face normalFace = null;
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Medium;
            GeometryElement e = floor.get_Geometry(opt);

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
                            if (pf.FaceNormal.AngleTo(new XYZ(0, 0, -1)) < 0.01)//数值在0到PI之间
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
