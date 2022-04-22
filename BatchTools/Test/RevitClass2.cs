using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;


namespace FFETOOLS
{
    /// <summary>
    /// 多排管道标注
    /// 只能在平面视图标注
    /// 只标注在管道法线右侧
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    class Cmd_MEPMarking : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // 判断当前视图是否是平面视图
            if (!(doc.ActiveView is ViewPlan))
            {
                TaskDialog.Show("Error", "当前视图不是平面视图");
                return Result.Cancelled;
            }

            Selection sel = uidoc.Selection;

            // 获取需要标注的管道
            IList<Element> elems = sel.PickElementsByRectangle(new PipeSelectionFilter(), "请选择管道");

            IList<MEPCurve> curves = new List<MEPCurve>();

            // 判断是否选择了机电管线
            if (elems.Count == 0)
            {
                TaskDialog.Show("Error", "未选择机电管线");
                return Result.Cancelled;
            }

            foreach (Element e in elems)
            {
                curves.Add(e as MEPCurve);
            }


            // 判断管道是否平行
            if (!isMEPCurvesParallel(curves))
            {
                TaskDialog.Show("Test", "所选择的机电管线不平行");
                return Result.Cancelled;
            }


            // 获取单根管道的向量并确保方向准确
            Line MEPLine1 = (curves[0].Location as LocationCurve).Curve as Line;

            XYZ pipeVector = MEPLine1.Direction;

            double X = pipeVector.X;
            double Y = pipeVector.Y;

            if (Y == -1)
            {
                pipeVector = new XYZ(pipeVector.X, 1, pipeVector.Z);
            }

            else if (X < 0)
            {
                pipeVector = pipeVector.Multiply(-1);
            }


            // 选择标记点
            XYZ tagStart = sel.PickPoint();


            List<double> list = new List<double>();// 距离数组

            Dictionary<XYZ, MEPCurve> map = new Dictionary<XYZ, MEPCurve>();// 管线与垂直交点
            Dictionary<double, XYZ> tagToMep = new Dictionary<double, XYZ>();// 距离与垂直交点


            foreach (MEPCurve mep in curves)
            {
                LocationCurve location = mep.Location as LocationCurve;
                Curve curve = location.Curve;
                IntersectionResult intersection = curve.Project(tagStart);
                XYZ intersectionP = intersection.XYZPoint;
                XYZ interPoint = new XYZ(intersectionP.X, intersectionP.Y, 0.00);

                map.Add(interPoint, mep);

                list.Add(interPoint.DistanceTo(tagStart));

                tagToMep.Add(interPoint.DistanceTo(tagStart), interPoint);
            }
            list.Sort();

            list.Reverse();
            //StringBuilder s = new StringBuilder();
            //foreach(double d in list)
            //{
            //    s.Append(d.ToString()+'\n');
            //}
            //TaskDialog.Show("Test",s.ToString());

            double distance = 0.00;// 标注距离

            IList<Line> lines = new List<Line>();// 所需要绘制的线段
            IList<XYZ> tagPoints = new List<XYZ>();// 标记点位置


            Dictionary<MEPCurve, XYZ> tagMEP = new Dictionary<MEPCurve, XYZ>();// 机电管线与标记点

            foreach (double d in list)
            {
                XYZ verticalPoint = tagToMep[d];
                XYZ verticalVector = getPointVector(verticalPoint, tagStart);
                XYZ tempPoint = verticalPoint + verticalVector.Normalize() * (d + distance);
                distance += 450 / 304.8;
                Line line = Line.CreateBound(verticalPoint, tempPoint);
                lines.Add(line);

                XYZ tagPoint = tempPoint + pipeVector.Normalize() * 400 / 304.8;
                tagPoints.Add(tagPoint);
                line = Line.CreateBound(tempPoint, tagPoint);
                lines.Add(line);

                tagMEP.Add(map[verticalPoint], tagPoint);
            }

            ICollection<ElementId> group = new List<ElementId>();
            using (Transaction t = new Transaction(doc, "XX"))
            {
                t.Start();
                foreach (Line line in lines)
                {
                    DetailCurve detailCurve = doc.Create.NewDetailCurve(doc.ActiveView, line);
                    group.Add(detailCurve.Id);
                }
                doc.Create.NewGroup(group);
                t.Commit();
            }

            TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;
            TagOrientation tagOrientation = TagOrientation.Horizontal;//标记的朝向


            using (Transaction trans = new Transaction(doc))
            {
                trans.Start("Creat  IndependentTag");

                foreach (MEPCurve curve in curves)
                {
                    XYZ tagPoint = tagMEP[curve];

                    IndependentTag tag = IndependentTag.Create(doc, doc.ActiveView.Id, new Reference(curve), true, tagMode, tagOrientation, tagPoint);
                    if (null == tag)
                    {
                        throw new System.Exception("Create IndependentTag Failed.");
                    }

                    tag.LeaderEndCondition = LeaderEndCondition.Free;
                    tag.HasLeader = false;//不要箭头
                                          //XYZ elbowPnt = mid + new XYZ(2.0, 2.0, 0.0);
                                          //tag.LeaderElbow = elbowPnt;//有箭头的话，箭头拐点
                    XYZ headerPnt = tagPoint;
                    tag.TagHeadPosition = headerPnt;//标记的位置

                }


                trans.Commit();
            }


            return Result.Succeeded;
        }

        // 获取两个点的向量
        public XYZ getPointVector(XYZ p1, XYZ p2)
        {
            Line line = Line.CreateBound(p1, p2);
            return line.Direction;
        }

        // 判断多条管道平行
        public bool isMEPCurvesParallel(IList<MEPCurve> mepCurves)
        {
            Stack<MEPCurve> stack = new Stack<MEPCurve>();
            foreach (MEPCurve curve in mepCurves)
            {
                stack.Push(curve);
            }

            while (stack.Count > 1)
            {
                MEPCurve m1 = stack.Pop();
                MEPCurve m2 = stack.Peek();

                if (!isParallel(m1, m2))
                    return false;
            }

            return true;
        }

        // 判断两条直线是否平行
        public bool isParallel(MEPCurve c1, MEPCurve c2)
        {
            Line l1 = CurveZZ(c1) as Line;
            Line l2 = CurveZZ(c2) as Line;

            XYZ vector1 = l1.Direction;
            XYZ vector2 = l2.Direction;
            if (vector1.IsAlmostEqualTo(vector2) || vector1.IsAlmostEqualTo(vector2 * -1))
                return true;

            return false;
        }

        // 将Curve的Z值归零
        public Curve CurveZZ(MEPCurve c)
        {
            Curve curve = (c.Location as LocationCurve).Curve;
            XYZ p1 = curve.GetEndPoint(0);
            XYZ p2 = curve.GetEndPoint(1);
            p1 = new XYZ(p1.X, p1.Y, 0);
            p2 = new XYZ(p2.X, p2.Y, 0);
            return Line.CreateBound(p1, p2);
        }
    }
}


