using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
namespace FFETOOLS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class RevitGeometry : IExternalCommand //����ʵ����ȡ�ü�����Ϣ
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document revitDoc = commandData.Application.ActiveUIDocument.Document;  //ȡ���ĵ�
            Application revitApp = commandData.Application.Application;             //ȡ��Ӧ�ó���
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Selection sel = uiDoc.Selection;
            Reference ref1 = sel.PickObject(ObjectType.Element, "ѡ��һ����ʵ��");
            Element elem = revitDoc.GetElement(ref1);
            FamilyInstance familyInstance = elem as FamilyInstance;
            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement e = familyInstance.get_Geometry(opt);

            foreach (GeometryObject obj in e)
            {
                GeometryInstance geoInstance = obj as GeometryInstance;
                GeometryElement geoElement = geoInstance.GetInstanceGeometry();
                Transform insTransform = geoInstance.Transform;
                foreach (GeometryObject obj2 in geoElement)
                {
                    Solid solid2 = obj2 as Solid;
                    //if (solid2.Faces.Size > 0)
                    //{
                        FindBottomFace(solid2);
                        //FindEdge(solid2);
                        //FindLine(solid2);
                        //FindPoint(solid2);
                       // transformPointAndUaPoint(solid2, insTransform);
                        TaskDialog.Show("�Ǻ�", "������");
                    //}
                }
            }
            return Result.Succeeded;
        }
        /// <summary>
        /// �õ�����µıߵ������ԭ������
        /// </summary>
        /// <param name=" solid "></param>
        /// <returns></returns>
        Face FindBottomFace(Solid solid)
        {
            PlanarFace pf = null;
            foreach (Face face in solid.Faces)
            {
                pf = face as PlanarFace;
                if (null != pf)
                {
                    if (Math.Abs(pf.FaceNormal.X) < 0.01 && Math.Abs(pf.FaceNormal.Y) < 0.01 && pf.FaceNormal.Z < 0)
                    {
                        TaskDialog.Show("Wall Bottom Face", "Area is " + pf.Area.ToString() + "; Origin = (" + pf.Origin.X.ToString() + "  " + pf.Origin.Y.ToString() + "  " + pf.Origin.Z.ToString() + ")");
                        break;
                    }
                }
            }
            return pf;
        }
        /// <summary>
        /// ͨ��curve�õ�12���ߵĳ���
        /// </summary>
        /// <param name=" solid "></param>
        public void FindEdge(Solid solid)
        {
            string strParamInfo = null;
            foreach (Edge e in solid.Edges)
            {
                strParamInfo += e.ApproximateLength + "\n";
            }
            TaskDialog.Show("REVIT", strParamInfo);
        }
        /// <summary>
        /// ͨ��Line�õ�12���ߵĳ���
        /// </summary>
        /// <param name=" solid "></param>
        public void FindLine(Solid solid)
        {
            string strParamInfo = null;
            foreach (Edge e in solid.Edges)
            {
                Line line = e.AsCurve() as Line;
                strParamInfo += line.ApproximateLength + "\n";
            }
            TaskDialog.Show("REVIT", strParamInfo);
        }
        /// <summary>
        /// ͨ��curve����line�ҵ���
        /// </summary>
        /// <param name=" solid "></param>
        public void FindPoint(Solid solid)
        {
            string strParamInfo1 = null;
            string strParamInfo2 = null;
            //string strParamInfo3 = null;
            foreach (Edge e in solid.Edges)
            {
                foreach (XYZ ii in e.Tessellate())
                {
                    XYZ point = ii;
                    strParamInfo1 += ii.X + "," + ii.Y + "," + ii.Z + "\n";

                }
                Line line = e.AsCurve() as Line;
                foreach (XYZ ii in line.Tessellate())
                {
                    XYZ point = ii;
                    strParamInfo2 += ii.X + "," + ii.Y + "," + ii.Z + "\n";
                }

            }
            TaskDialog.Show("ͨ��curve�ҵ��������", strParamInfo1);
            TaskDialog.Show("ͨ��line�ҵ��������", strParamInfo2);
        }
        public void transformPointAndUaPoint(Solid solid, Transform insTransform)
        {
            string strParamInfo1 = null;
            string strParamInfo2 = null;
            //string strParamInfo3 = null;
            foreach (Edge e in solid.Edges)
            {
                foreach (XYZ ii in e.Tessellate())
                {
                    XYZ point = ii;
                    strParamInfo1 += point.X + "," + point.Y + "," + point.Z + "\n";
                    XYZ transformPoint = insTransform.OfPoint(point);
                    strParamInfo2 += transformPoint.X + "," + transformPoint.Y + "," + transformPoint.Z + "\n";
                }
            }
            TaskDialog.Show("δ��transform��ת��������", strParamInfo1);
            TaskDialog.Show("����transformת��������ϵ", strParamInfo2);
        }
    }
}
