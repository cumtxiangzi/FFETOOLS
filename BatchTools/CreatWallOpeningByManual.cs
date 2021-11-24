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
    class CreatWallOpeningByManual : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            Autodesk.Revit.Creation.Application aCreate = commandData.Application.Application.Create;

            try
            {
                using (Transaction ts = new Transaction(doc, "手动墙体开洞"))
                {
                    ts.Start();
                    //墙体
                    Reference refWall = sel.PickObject(ObjectType.Element, new WallSelectionFilter(), "请选择需要开洞的墙体");
                    Wall wall = doc.GetElement(refWall) as Wall;
                    Face face = FindWallFace(wall);
                    PlanarFace pf = face as PlanarFace;

                    //XYZ xyz = refWall.GlobalPoint;
                    //xyz = face.Project(xyz).XYZPoint;
                    //XYZ normal = pf.FaceNormal;
                    //XYZ refdir = normal.CrossProduct(XYZ.BasisZ);

                    CurveArray curves = aCreate.NewCurveArray();
                    //水管
                    Reference refPipe = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择管道");
                    Pipe pipe = doc.GetElement(refPipe) as Pipe;
                    Curve curve = FindPipeCurve(pipe);
                    //求交点
                    XYZ xyzint = FindFaceCurve(face, curve);

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

                    FamilyInstance opening = doc.Create.NewFamilyInstance(face, xyzint, new XYZ(0,0,0), openingSymbol);
                    doc.Regenerate();
                    double wallThick = wall.Width * 304.8;
                    Parameter openingHeight = opening.LookupParameter("孔深");
                    openingHeight.SetValueString(wallThick.ToString());
                    double pipeDN = Convert.ToDouble(pipe.LookupParameter("尺寸").AsString());
                    double openingDN = pipeDN + 100;
                    Parameter openingWidth = opening.LookupParameter("孔宽");
                    openingWidth.SetValueString(openingDN.ToString());
                    Parameter openingLength = opening.LookupParameter("孔长");
                    openingLength.SetValueString(openingDN.ToString());

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

        //找到墙线的向量
        //XYZ FindWallVector(Wall wall)
        //{
        //    LocationCurve lCurve = wall.Location as LocationCurve;
        //    XYZ xyz = lCurve.Curve.GetEndPoint(1) - lCurve.Curve.GetEndPoint(0);
        //    return xyz;
        //}

        //求线和面的交点
        public XYZ FindFaceCurve(Face face, Curve curve)
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
        public Curve FindPipeCurve(Pipe p)
        {
            //得到水管曲线
            IList<XYZ> list = new List<XYZ>();
            ConnectorSetIterator csi = p.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                list.Add(conn.Origin);
            }
            Curve curve = Line.CreateBound(list.ElementAt(0), list.ElementAt(1)) as Curve;
            curve.MakeUnbound();
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
    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return e is Wall;
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            return true;
        }
    }
}
