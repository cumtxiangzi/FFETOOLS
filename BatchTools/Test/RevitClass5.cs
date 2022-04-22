using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Test.Utils;

namespace Test.����.���߱��
{
    //public class MEPTagCreater
    //{
    //    private XYZ startTagPoint = null;// ��ʼ��ǵ�
    //    private int distance = 0;// ��ǵľ���
    //    private SortedList<double, MEPCurve> disCurves = null;// �뿪ʼ��ǵ�ľ�����������
    //    private Dictionary<MEPCurve, XYZ> curves = null;// ����������ǵ�Ĵ���
    //    private XYZ curveVector = null;
    //    private XYZ tagVector = null;
    //    bool isRight = true;

    //    public MEPTagCreater(XYZ startTagPoint, int distance, bool isRight)
    //    {
    //        this.startTagPoint = startTagPoint;
    //        this.distance = distance;
    //        disCurves = new SortedList<double, MEPCurve>();
    //        curves = new Dictionary<MEPCurve, XYZ>();
    //        this.isRight = isRight;
    //    }

    //    public bool Add(MEPCurve m)
    //    {
    //        Line l = (m.Location as LocationCurve).Curve as Line;
    //        IntersectionResult intersection = l.Project(startTagPoint);
    //        XYZ intersectionP = intersection.XYZPoint;
    //        XYZ res = new XYZ(intersectionP.X, intersectionP.Y, 0.00);
    //        double dis = res.DistanceTo(startTagPoint);


    //        if (curves.Count == 0)
    //        {


    //            XYZ vector = GUtils.getPointVector(res, startTagPoint);
    //            tagVector = vector;

    //            vector = l.Direction;

    //            double X = vector.X;
    //            double Y = vector.Y;

    //            if (Y == -1)
    //            {
    //                vector = new XYZ(vector.X, 1, vector.Z);
    //            }
    //            else if (X < 0)
    //            {
    //                vector = vector.Multiply(-1);
    //            }
    //            if (!isRight)
    //            {
    //                vector = vector.Multiply(-1);
    //            }
    //            curveVector = vector;
    //            disCurves.Add(dis, m);
    //            curves.Add(m, res);
    //            return true;
    //        }
    //        else
    //        {
    //            MEPCurve curve = curves.Keys.FirstOrDefault<MEPCurve>();
    //            if (!GUtils.isParallel(m, curve))
    //                return false;
    //            disCurves.Add(dis, m);
    //            curves.Add(m, res);
    //            return true;
    //        }
    //    }

    //    public void Tag(Document doc)
    //    {
    //        ICollection<ElementId> dls = new List<ElementId>();
    //        IList<double> distances = disCurves.Keys;
    //        distances = distances.Reverse().ToList();
    //        double tagDis = 0;

    //        using (Transaction t = new Transaction(doc))
    //        {
    //            t.Start("������ע");
    //            foreach (double d in distances)
    //            {
    //                MEPCurve curve = disCurves[d];
    //                XYZ vetPoint = curves[curve];
    //                XYZ startPoint = startTagPoint + tagVector.Normalize() * tagDis / 304.8;
    //                IList<DetailCurve> ls = Tag(doc, curve, vetPoint, startPoint);
    //                foreach (DetailCurve c in ls)
    //                {
    //                    dls.Add(c.Id);
    //                }
    //                tagDis += distance;
    //            }
    //            doc.Create.NewGroup(dls);
    //            t.Commit();
    //        }
    //    }

    //    private IList<DetailCurve> Tag(Document doc, MEPCurve curve, XYZ vetPoint, XYZ startPoint)
    //    {
    //        XYZ bVector = curveVector + tagVector;

    //        IList<DetailCurve> ls = new List<DetailCurve>();
    //        Line l1 = Line.CreateBound(vetPoint, startPoint);
    //        Line l2 = Line.CreateBound(startPoint, startPoint + curveVector.Normalize() * 400 / 304.8);
    //        DetailCurve dl1 = doc.Create.NewDetailCurve(doc.ActiveView, l1);
    //        DetailCurve dl2 = doc.Create.NewDetailCurve(doc.ActiveView, l2);

    //        XYZ p1 = vetPoint + bVector.Normalize() * 100 / 304.8;
    //        XYZ p2 = vetPoint - bVector.Normalize() * 100 / 304.8;
    //        Line l3 = Line.CreateBound(p1, p2);
    //        DetailCurve dl3 = doc.Create.NewDetailCurve(doc.ActiveView, l3);

    //        ls.Add(dl1);
    //        ls.Add(dl2);
    //        ls.Add(dl3);


    //        TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;
    //        TagOrientation tagOrientation = TagOrientation.Horizontal;//��ǵĳ���

    //        IndependentTag tag = IndependentTag.Create(doc, doc.ActiveView.Id, new Reference(curve), true, tagMode, tagOrientation, l2.GetEndPoint(1));
    //        if (null == tag)
    //        {
    //            throw new System.Exception("Create IndependentTag Failed.");
    //        }
    //        tag.LeaderEndCondition = LeaderEndCondition.Free;
    //        tag.HasLeader = false;//��Ҫ��ͷ
    //                              //XYZ elbowPnt = mid + new XYZ(2.0, 2.0, 0.0);
    //                              //tag.LeaderElbow = elbowPnt;//�м�ͷ�Ļ�����ͷ�յ�
    //        XYZ headerPnt = l2.GetEndPoint(1);
    //        tag.TagHeadPosition = headerPnt;//��ǵ�λ��
    //        SetOrientation(doc, curve, tag, isRight);
    //        return ls;
    //    }

    //    // ���ı������
    //    private void SetOrientation(Document doc, MEPCurve curve, IndependentTag tag, bool isRight)
    //    {
    //        Element type = null;
    //        FilteredElementCollector collector = new FilteredElementCollector(doc);
    //        if (curve is Pipe)
    //        {
    //            collector.OfCategory(BuiltInCategory.OST_PipeTags).OfClass(typeof(FamilySymbol));
    //            if (isRight)
    //            {
    //                foreach (Element e in collector)
    //                {
    //                    if (e.Name.Equals("�ܵ����-��"))
    //                    {
    //                        type = e;
    //                        break;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                foreach (Element e in collector)
    //                {
    //                    if (e.Name.Equals("�ܵ����-��"))
    //                    {
    //                        type = e;
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //        else if (curve is Duct)
    //        {
    //            collector.OfCategory(BuiltInCategory.OST_DuctTags).OfClass(typeof(FamilySymbol));
    //            if (isRight)
    //            {
    //                foreach (Element e in collector)
    //                {
    //                    if (e.Name.Equals("��ܱ��-��"))
    //                    {
    //                        type = e;
    //                        break;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                foreach (Element e in collector)
    //                {
    //                    if (e.Name.Equals("��ܱ��-��"))
    //                    {
    //                        type = e;
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //        else if (curve is CableTray)
    //        {
    //            collector.OfCategory(BuiltInCategory.OST_CableTrayTags).OfClass(typeof(FamilySymbol));
    //            if (isRight)
    //            {
    //                foreach (Element e in collector)
    //                {
    //                    if (e.Name.Equals("�żܱ��-��"))
    //                    {
    //                        type = e;
    //                        break;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                foreach (Element e in collector)
    //                {
    //                    if (e.Name.Equals("�żܱ��-��"))
    //                    {
    //                        type = e;
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        if (type != null)
    //        {
    //            ICollection<ElementId> eids = new List<ElementId>();
    //            eids.Add(tag.Id);
    //            Element.ChangeTypeId(doc, eids, type.Id);
    //        }
    //    }
    //}
}

