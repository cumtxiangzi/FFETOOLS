using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class BatchCreatTeeFitting : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;
               
                using (Transaction tran = new Transaction(doc))
                {
                    tran.Start("批量两管生成三通");
                    BatchCreatTeeFittingMain(doc, uidoc);
                    tran.Commit();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }

        public void BatchCreatTeeFittingMain(Document doc, UIDocument uidoc)
        {
            Selection sel = uidoc.Selection;
            IList<Reference> refList1 = sel.PickObjects(ObjectType.Element, new PipeSelectionFilter(), "请选要互连的第一批管道");
            IList<Reference> refList2 = sel.PickObjects(ObjectType.Element, new PipeSelectionFilter(), "请选要互连的第二批管道");

            List<Pipe> pipeList1 = new List<Pipe>();
            List<Pipe> pipeList2 = new List<Pipe>();
            List<Pipe> pipeListTmp1 = new List<Pipe>();
            List<Pipe> pipeListTmp2 = new List<Pipe>();

            List<string> pipeSystemList1 = new List<string>();
            List<string> pipeSystemList2 = new List<string>();
            List<string> pipeSystemListTmp = new List<string>();

            foreach (Reference ref1 in refList1)
            {
                Pipe p = ref1.GetElement(doc) as Pipe;
                pipeList1.Add(p);
            }
            foreach (Reference ref2 in refList2)
            {
                Pipe p = ref2.GetElement(doc) as Pipe;
                pipeList2.Add(p);
            }

            foreach (Reference item in refList1)
            {
                Pipe pipe = doc.GetElement(item) as Pipe;
                string name = (doc.GetElement(pipe.MEPSystem.GetTypeId()) as PipingSystemType).Name;
                pipeSystemList1.Add(name);
            }
            //List<string> ListTemp = pipeSystemList1.Distinct().ToList();//去除重复项

            foreach (Reference item in refList2)
            {
                Pipe pipe = doc.GetElement(item) as Pipe;
                string name = (doc.GetElement(pipe.MEPSystem.GetTypeId()) as PipingSystemType).Name;
                pipeSystemList2.Add(name);
            }

            foreach (string name1 in pipeSystemList1)
            {
                foreach (string name2 in pipeSystemList2)
                {
                    if (name1 == name2)
                    {
                        pipeSystemListTmp.Add(name1);
                    }
                }
            }

            foreach (string name in pipeSystemListTmp)
            {
                foreach (Pipe p1 in pipeList1)
                {
                    if (p1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString().Contains(name))
                    {
                        pipeListTmp1.Add(p1);
                    }
                }
                foreach (Pipe p2 in pipeList2)
                {
                    if (p2.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString().Contains(name))
                    {
                        pipeListTmp2.Add(p2);
                    }
                }
            }

            if (refList1.Count == 0 || refList2.Count == 0)
            {
                TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                BatchCreatTeeFittingMain(doc, uidoc);
            }
            else
            {
                foreach (Pipe pipe1 in pipeListTmp1)
                {
                    foreach (Pipe pipe2 in pipeListTmp2)
                    {
                        if (pipe1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() == pipe2.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString())
                        {
                            BatchCreatTeeFittingMethod(doc,pipe1, pipe2);
                        }
                    }
                }
            }
        }

        public void BatchCreatTeeFittingMethod(Document doc, Pipe pipe1, Pipe pipe2)
        {
            //选取两个管道
            //var reference1 = sel.PickObject(ObjectType.Element, "请选择第1个管");
            MEPCurve duct1 = pipe1 as MEPCurve;
           
            //var reference2 = sel.PickObject(ObjectType.Element, "请选择第2个管");
            MEPCurve duct2 = pipe2 as MEPCurve;          

            Line line1 = (duct1.Location as LocationCurve).Curve as Line;
            line1.MakeUnbound();

            Line line2 = (duct2.Location as LocationCurve).Curve as Line;
            XYZ startPoitOnLine2 = line2.StartPoint();
            XYZ endPoitOnLine2 = line2.EndPoint();

            IntersectionResult resultStart = line1.Project(startPoitOnLine2);
            XYZ startCrossPoint = resultStart.XYZPoint;
            IntersectionResult resultEnd = line1.Project(endPoitOnLine2);
            XYZ endCrossPoint = resultEnd.XYZPoint;

            XYZ crossPointOnLine1 = new XYZ();
            XYZ pointOnLine2 = new XYZ();

            if (startPoitOnLine2.DistanceTo(startCrossPoint) < endPoitOnLine2.DistanceTo(endCrossPoint))
            {
                crossPointOnLine1 = startCrossPoint;
                pointOnLine2 = startPoitOnLine2;
            }
            else
            {
                crossPointOnLine1 = endCrossPoint;
                pointOnLine2 = endPoitOnLine2;
            }

            line2.MakeUnbound();
            IntersectionResult resultCross = line2.Project(crossPointOnLine1);
            XYZ crossPointOnLine2 = resultCross.XYZPoint;

            //using (Transaction tran = new Transaction(doc))
            //{
               // tran.Start("两管生成三通");

                if (CurvePosition(line1, line2).Equals(SetComparisonResult.Overlap))
                {
                    CreatTeeMethod(doc, pipe1, pipe2);
                }
                else
                {
                    Pipe pipe3 = Pipe.Create(doc, pipe2.MEPSystem.GetTypeId(), pipe2.GetTypeId(), GetPipeLevel(doc, "0.000").Id, crossPointOnLine1, crossPointOnLine2);
                    ChangePipeSize(pipe3, pipe2.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString());
                    ConnectTwoPipesWithElbow(doc, pipe2, pipe3);
                    CreatTeeMethod(doc, pipe1, pipe3);
                }

               // tran.Commit();
            //}

        }
        public static Level GetPipeLevel(Document doc, string Levelname)
        {
            // 获取标高
            Level newlevel = null;
            var levelFilter = new ElementClassFilter(typeof(Level));
            FilteredElementCollector levels = new FilteredElementCollector(doc);
            levels = levels.WherePasses(levelFilter);
            foreach (Level level in levels)
            {
                if (level.Name.Contains(Levelname))
                {
                    newlevel = level;
                    break;
                }
            }
            return newlevel;
        }
        public static void ChangePipeSize(Pipe pipe, string dn)
        {
            //改变管道尺寸
            Parameter diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            diameter.SetValueString(dn);
        }
        public static void ConnectTwoPipesWithElbow(Document doc, Pipe pipe1, Pipe pipe2)
        {
            // 创建弯头
            MEPCurve pipecurve1 = pipe1 as MEPCurve;
            MEPCurve pipecurve2 = pipe2 as MEPCurve;

            double minDistance = double.MaxValue;
            Connector connector1, connector2;
            connector1 = connector2 = null;

            foreach (Connector con1 in pipecurve1.ConnectorManager.Connectors)
            {
                foreach (Connector con2 in pipecurve2.ConnectorManager.Connectors)
                {
                    var dis = con1.Origin.DistanceTo(con2.Origin);
                    if (dis < minDistance)
                    {
                        minDistance = dis;
                        connector1 = con1;
                        connector2 = con2;
                    }
                }
            }
            if (connector1 != null && connector2 != null)
            {
                var elbow = doc.Create.NewElbowFitting(connector1, connector2);
            }
        }
        public static SetComparisonResult CurvePosition(Curve curve1, Curve curve2)
        {
            IntersectionResultArray resultArray = null;
            SetComparisonResult result = curve1.Intersect(curve2, out resultArray);
            return result;
        }
        public void CreatTeeMethod(Document doc, Pipe pipe1, Pipe pipe2)
        {
            // 创建三通
            MEPCurve pipecurve1 = pipe1 as MEPCurve;
            MEPCurve pipecurve2 = pipe2 as MEPCurve;

            Curve curve1 = (pipecurve1.Location as LocationCurve).Curve;

            Curve curve2 = (pipecurve2.Location as LocationCurve).Curve;
            curve2.MakeUnbound();

            var ductList = GetMainDuct(pipecurve1, pipecurve2);

            MEPCurve MainDuct = ductList[0];//the main Pipe
            MEPCurve LessDuct = ductList[1];// the minor Pipe

            MEPCurve duct3 = null;//the main pipe

            IntersectionResultArray intersectPoint = new IntersectionResultArray();

            var x = curve1.Intersect(curve2, out intersectPoint);

            if (x == SetComparisonResult.Overlap)
            {
                var OrginPoint = intersectPoint.get_Item(0).XYZPoint;
                var elementId = PlumbingUtils.BreakCurve(doc, MainDuct.Id, OrginPoint);
                duct3 = doc.GetElement(elementId) as MEPCurve;
                ConnectTwoDuctsWithElbow(doc, MainDuct, duct3, LessDuct);
            }
        }

        /// <summary>
                /// 区分主要管道与次要管道
                /// </summary>
                /// <param name="duct1"></param>
                /// <param name="duct2"></param>
                /// <returns></returns>
        public static List<MEPCurve> GetMainDuct(MEPCurve duct1, MEPCurve duct2)
        {
            List<MEPCurve> mEPCurves = new List<MEPCurve>();

            Curve curve1 = (duct1.Location as LocationCurve).Curve;
            Curve curve2 = (duct2.Location as LocationCurve).Curve;

            List<double> disList = new List<double>();
            double dis = double.MaxValue;
            XYZ pointA1 = curve1.GetEndPoint(0);
            XYZ pointA2 = curve1.GetEndPoint(1);
            XYZ pointB1 = curve2.GetEndPoint(0);
            XYZ pointB2 = curve2.GetEndPoint(1);

            var distance1 = curve2.Distance(pointA1);
            var distance2 = curve2.Distance(pointA2);
            var distance3 = curve1.Distance(pointB1);
            var distance4 = curve1.Distance(pointB2);

            disList.Add(distance1);
            disList.Add(distance2);
            disList.Add(distance3);
            disList.Add(distance4);

            dis = disList.Min();
            var x = disList.IndexOf(dis);

            if (x < 2)
            {
                mEPCurves.Add(duct2);
                mEPCurves.Add(duct1);
                return mEPCurves;
            }
            else
            {
                mEPCurves.Add(duct1);
                mEPCurves.Add(duct2);
                return mEPCurves;
            }
        }

        /// <summary>
                /// 创建连接
                /// </summary>
                /// <param name="doc"></param>
                /// <param name="duct1">主要管道1</param>
                /// <param name="duct2">主要管道2</param>
                /// <param name="duct3">次要管道</param>
        public static void ConnectTwoDuctsWithElbow(Document doc, MEPCurve duct1, MEPCurve duct2, MEPCurve duct3)
        {
            double minDistance = double.MaxValue;
            Connector connector1, connector2, connector3;
            connector1 = connector2 = connector3 = null;

            foreach (Connector con1 in duct1.ConnectorManager.Connectors)
            {
                foreach (Connector con2 in duct2.ConnectorManager.Connectors)
                {
                    var dis = con1.Origin.DistanceTo(con2.Origin);
                    if (dis < minDistance)
                    {
                        minDistance = dis;
                        connector1 = con1;
                        connector2 = con2;
                    }

                }
            }

            minDistance = double.MaxValue;//重置
            foreach (Connector con3 in duct3.ConnectorManager.Connectors)
            {
                var dis = con3.Origin.DistanceTo(connector1.Origin);
                if (dis < minDistance)
                {
                    minDistance = dis;
                    connector3 = con3;
                }
            }

            if (connector1 != null && connector2 != null && connector3 != null)
            {
                doc.Create.NewTeeFitting(connector1, connector2, connector3);
            }
        }
    }
}
