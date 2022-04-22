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
    class CreatFloorOpeningByMaual : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;
            Autodesk.Revit.Creation.Application aCreate = commandData.Application.Application.Create;

            try
            {
                using (Transaction ts = new Transaction(doc, "手动楼板开洞"))
                {
                    ts.Start();
                    //楼板
                    Reference refFloor = sel.PickObject(ObjectType.Element, new FloorSelectionFilter(), "请选择需要开洞的楼板");
                    Floor floor = doc.GetElement(refFloor) as Floor;
                    
                    Face face = FindFloorFace(floor);
                    PlanarFace pf = face as PlanarFace;

                    XYZ xyz = refFloor.GlobalPoint;
                    xyz = face.Project(xyz).XYZPoint;
                    XYZ normal = pf.FaceNormal;
                    XYZ refdir = normal.CrossProduct(XYZ.BasisZ);
                    
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

                    //IList<Reference> reflist = new List<Reference>();
                    //reflist = uiDoc.Selection.PickObjects(ObjectType.Element,"请选择管道");
                    
                    CurveArray curves = aCreate.NewCurveArray();
                    //水管
                    Reference refPipe = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择管道");
                    Pipe pipe = doc.GetElement(refPipe) as Pipe;
                    Curve curve = FindPipeCurve(pipe);
                    //求交点
                    XYZ xyzint = FindFaceCurve(face, curve);                

                    FamilyInstance opening = doc.Create.NewFamilyInstance(face, xyzint, refdir, openingSymbol);
                    doc.Regenerate();
                    String floorThick = floor.LookupParameter("厚度").AsValueString();
                    Parameter openingHeight = opening.LookupParameter("孔深");
                    openingHeight.SetValueString(floorThick);
                    double pipeDN = Convert.ToDouble(pipe.LookupParameter("尺寸").AsString());
                    double openingDN = pipeDN + 100;
                    Parameter openingDia = opening.LookupParameter("孔直径");
                    openingDia.SetValueString(openingDN.ToString());
                    


                    ///*开矩形洞*/
                    //XYZ xyz1 = xyz + new XYZ(1, 1, 0) * 200 / 304.8;
                    //XYZ xyz2 = xyz + new XYZ(1, -1, 0) * 200 / 304.8;
                    //XYZ xyz3 = xyz + new XYZ(-1, -1, 0) * 200 / 304.8;
                    //XYZ xyz4 = xyz + new XYZ(-1, 1, 0) * 200 / 304.8;
                    //Curve c1 = aCreate.NewLine(xyz1, xyz2, true);
                    //Curve c2 = aCreate.NewLine(xyz2, xyz3, true);
                    //Curve c3 = aCreate.NewLine(xyz3, xyz4, true);
                    //Curve c4 = aCreate.NewLine(xyz4, xyz1, true);
                    //curves.Append(c1);
                    //curves.Append(c2);
                    //curves.Append(c3);
                    //curves.Append(c4);

                    //开圆形洞
                    //double startAngle = 0;
                    //double midAngle = Math.PI;
                    //double endAngle = 2 * Math.PI;

                    //XYZ xAxis = XYZ.BasisX;
                    //XYZ yAxis = XYZ.BasisY;
                    //double radius = 180 / 304.8;

                    //Arc arc1 = Arc.Create(xyz, radius, startAngle, midAngle, xAxis, yAxis);
                    //Arc arc2 = Arc.Create(xyz, radius, midAngle, endAngle, xAxis, yAxis);

                    //curves.Append(arc1);
                    //curves.Append(arc2);

                    //doc.Create.NewOpening(floor, curves, true);

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
         Curve FindPipeCurve(Pipe p)
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
        //找到楼板的面
         Face FindFloorFace(Floor floor)
        {
            Face normalFace = null;
            //
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Medium;
            //
            GeometryElement e = floor.get_Geometry(opt);
            /*下版改进
            IEnumerator<GeometryObject> enm = e.GetEnumerator();
            while (enm.MoveNext())
            {
                Solid solid = enm.Current as Solid;
            }*/
            foreach (GeometryObject obj in e)//待改2013
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
