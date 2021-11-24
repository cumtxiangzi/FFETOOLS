using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class CreatFloorOpening2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection selduc = uiApp.ActiveUIDocument.Selection;
            Selection selfloor = uiApp.ActiveUIDocument.Selection;

            try
            {
                using (Transaction ts = new Transaction(doc, "管道楼板开洞"))
                {
                    ts.Start();

                    Reference reference = selduc.PickObject(ObjectType.Element, new ElementSelectionFilterDuc(doc), "请选择风管");
                    Element ductelm = doc.GetElement(reference);
                    Duct duc = ductelm as Duct;
                    CreatOpening(doc, selfloor,duc);
                    ts.Commit();
                }                              
                return Result.Succeeded;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 楼板开洞方法
        /// </summary>
        /// <param name="doc"></param>
        public void CreatOpening(Autodesk.Revit.DB.Document doc, Selection selection,Duct duc)
        {
            Reference reference = selection.PickObject(ObjectType.Element, new ElementSelectionFilter(doc), "请选择需要开洞的图元");
            Element openingElement = doc.GetElement(reference);
            Face face = FindCeilingAndFloorFace(openingElement as CeilingAndFloor);

            Curve curve = FindElemntLocationCurve(duc);
            XYZ intersection = CaculateIntersection(face, curve);
            TaskDialog.Show("t", intersection.X.ToString());
            CurveArray curveArray = new CurveArray();
            Arc arc1 = Arc.Create(intersection, Math.PI, 0, Math.PI, XYZ.BasisX, XYZ.BasisY);
            Arc arc2 = Arc.Create(intersection, Math.PI, Math.PI, Math.PI * 2, XYZ.BasisX, XYZ.BasisY);
            curveArray.Append(arc1);
            curveArray.Append(arc2);
            doc.Create.NewOpening(openingElement, curveArray, true);
        }

        /// <summary>
        /// 求面和线的交点
        /// </summary>
        /// <param name="face"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public XYZ CaculateIntersection(Face face, Curve curve)
        {
            //求交点
            XYZ intersection = new XYZ();
            IntersectionResultArray resultArray = new IntersectionResultArray();
            SetComparisonResult setComparisonResult = face.Intersect(curve, out resultArray);
            if (SetComparisonResult.Disjoint != setComparisonResult)
            {
                if (!resultArray.IsEmpty)
                {
                    intersection = resultArray.get_Item(0).XYZPoint;
                }
            }
            //  TaskDialog.Show("t", resultArray.Size.ToString());
            return intersection;

        }

        /// <summary>
        /// 获得MEP的位置信息的方法
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public Curve FindElemntLocationCurve(Duct duct)
        {
            ////过滤得到MEP
            //FilteredElementCollector elementCollector = new FilteredElementCollector(doc);
            //elementCollector.OfClass(typeof(MEPCurve));
            //ICollection<Element> mepCurves = elementCollector.ToElements();
            //// TaskDialog.Show("t", mepCurves.Count.ToString());
            ////遍历得到的MEP，获得其位置信息
            //LocationCurve mepLocationCurve = null;
            //foreach (var mepCurve in mepCurves)
            //{
            //    mepLocationCurve = mepCurve.Location as LocationCurve;
            //    // TaskDialog.Show("t", mepCurve.Name);
            //}

            //return mepLocationCurve.Curve;

            //得到风管曲线
            IList<XYZ> list = new List<XYZ>();
            ConnectorSetIterator csi = duct.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                list.Add(conn.Origin);
            }
            Curve curve = Line.CreateBound(list.ElementAt(0), list.ElementAt(1)) as Curve;
            curve.MakeUnbound();
            return curve;
        }

        /// <summary>
        /// 找到宿主的面
        /// </summary>
        /// <param name="ceilingAndFloor"></param>
        /// <returns></returns>
        public Face FindCeilingAndFloorFace(CeilingAndFloor ceilingAndFloor)
        {
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Medium;
            GeometryElement geometryElement = ceilingAndFloor.get_Geometry(opt);

            Face normalFace = null;
            foreach (GeometryObject geometryObject in geometryElement)
            {
                Solid solid = geometryObject as Solid;
                if (solid != null && solid.Faces.Size > 0)
                {
                    foreach (Face face in solid.Faces)
                    {
                        PlanarFace planarFace = face as PlanarFace;
                        if (planarFace != null)
                        {
                            if (planarFace.FaceNormal.AngleTo(new XYZ(1, 1, 0)) == 0 || (planarFace.FaceNormal.AngleTo(new XYZ(1, 1, 0)) == Math.PI))
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
    public class ElementSelectionFilter : ISelectionFilter
    {

        private Autodesk.Revit.DB.Document _doc;
        public ElementSelectionFilter(Autodesk.Revit.DB.Document doc)
        {
            _doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            //return elem is Pipe;
            return elem is CeilingAndFloor;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }

    public class ElementSelectionFilterDuc : ISelectionFilter
    {

        private Autodesk.Revit.DB.Document _doc;
        public ElementSelectionFilterDuc(Autodesk.Revit.DB.Document doc)
        {
            _doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            //return elem is Pipe;
            return elem is Duct;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
