using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.InteropServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class PipeSupport : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //选择一点
            Reference ref_point = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.PointOnElement);
            XYZ point1 = ref_point.GlobalPoint;

            //射线方向及工作平面法向量
            XYZ rayDirection = XYZ.BasisZ;
            XYZ skVector = XYZ.BasisX;
            //当选择的主体为平面时，射线根据选择的点与此面的法线方向进行放射
            if (ref_point.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_SURFACE)
            {
                PlanarFace pFace = null;
                //主体是链接的图元时获取平面的方法
                if (ref_point.LinkedElementId.IntegerValue != -1)
                {
                    RevitLinkInstance linkIns = doc.GetElement(ref_point) as RevitLinkInstance;
                    Document linkDoc = linkIns.GetLinkDocument();
                    Element linkElem = linkDoc.GetElement(ref_point.LinkedElementId);
                    Options opt = new Options();
                    opt.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement geomElem = linkElem.get_Geometry(opt);
                    pFace = GetTarFace(geomElem, point1);
                }
                else
                {
                    //判断是否FamilyInstance类型的族，采用不同的获取方法
                    Element elem = doc.GetElement(ref_point);
                    if (elem is FamilyInstance)
                    {
                        Options opt = new Options();
                        opt.DetailLevel = ViewDetailLevel.Fine;
                        GeometryElement ge = elem.get_Geometry(opt);
                        pFace = GetTarFace(ge, point1);
                    }
                    else
                    {
                        pFace = elem.GetGeometryObjectFromReference(ref_point) as PlanarFace;
                    }
                }

                //修正射线方向及工作平面法向量
                if (pFace != null)
                {
                    rayDirection = pFace.FaceNormal;
                    skVector = pFace.XVector;
                }
            }

            //视图    
            View3D v3d = doc.ActiveView as View3D;

            //创建射线测量出第二点
            ExclusionFilter filter = new ExclusionFilter(new ElementId[] { ref_point.ElementId, ref_point.LinkedElementId });
            ReferenceIntersector refIntersector = new ReferenceIntersector(filter, FindReferenceTarget.All, v3d);
            refIntersector.FindReferencesInRevitLinks = true;
            ReferenceWithContext rwc = refIntersector.FindNearest(point1, rayDirection);
            if (rwc != null)
            {
                XYZ point2 = rwc.GetReference().GlobalPoint;
                //创建模型线
                Line line = Line.CreateBound(point1, point2);
                TaskDialog.Show("距离", Math.Round(UnitUtils.ConvertFromInternalUnits(line.Length, DisplayUnitType.DUT_MILLIMETERS), 2).ToString());
                using (Transaction tran = new Transaction(doc, "尺寸"))
                {
                    tran.Start();
                    SketchPlane sk = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(skVector, point1));
                    ModelCurve modelCurve = doc.Create.NewModelCurve(line, sk);
                    tran.Commit();
                }
            }
            else
            {
                TaskDialog.Show("返回结果", "未检测到图元");
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// 获得与UV点相交的面
        /// </summary>
        /// <param name="geometryElement"></param>
        /// <param name="uvPoint"></param>
        /// <returns></returns>
        PlanarFace GetTarFace(GeometryElement geometryElement, XYZ point)
        {
            PlanarFace face = null;
            foreach (GeometryObject geomObj in geometryElement)
            {
                Solid solid = geomObj as Solid;
                if (solid != null && solid.Faces.Size > 0)
                {
                    foreach (Face f in solid.Faces)
                    {
                        PlanarFace pFace = f as PlanarFace;
                        if (pFace != null)
                        {
                            try
                            {
                                if (Math.Round(pFace.Project(point).Distance, 2) == 0)
                                {
                                    face = pFace;
                                    break;
                                }
                            }
                            catch
                            {
                                continue;
                            }

                        }
                    }
                }
                if (face != null)
                {
                    break;
                }
            }
            if (face == null)
            {
                foreach (GeometryObject geomObj in geometryElement)
                {
                    GeometryInstance geomIns = geomObj as GeometryInstance;
                    if (geomIns != null)
                    {
                        face = GetTarFace(geomIns.GetInstanceGeometry(), point);
                    }
                }
            }
            return face;
        }

    }
}
