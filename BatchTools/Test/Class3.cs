using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Xml;
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
    [Regeneration(RegenerationOption.Manual)]
    class NewElbow : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;

            //选取两个管
           

            return Result.Succeeded;
        }
        /// <summary>
        /// 连接管道
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="duct1"></param>
        /// <param name="duct2"></param>
        
    }

    //接下是三通，三通最需要注意的事角度！！角度！！角度！！，第二根管应该垂直于第一根管，不能超出1°！极为严格！

    //还有主次顺序问题。

    [Transaction(TransactionMode.Manual)]
    class NewTeeFitting : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;

            //选取两个管道
            var reference1 = sel.PickObject(ObjectType.Element, "请选择第1个管");
            MEPCurve duct1 = doc.GetElement(reference1) as MEPCurve;

            var reference2 = sel.PickObject(ObjectType.Element, "请选择第2个管");
            MEPCurve duct2 = doc.GetElement(reference2) as MEPCurve;


            Curve curve1 = (doc.GetElement(reference1).Location as LocationCurve).Curve;

            Curve curve2 = (doc.GetElement(reference2).Location as LocationCurve).Curve;

            var ductList = GetMainDuct(duct1, duct2);

            MEPCurve MainDuct = ductList[0];//the main Pipe
            MEPCurve LessDuct = ductList[1];// the minor Pipe

            MEPCurve duct3 = null;//the main pipe

            IntersectionResultArray intersectPoint = new IntersectionResultArray();

            var x = curve1.Intersect(curve2, out intersectPoint);

            if (x == SetComparisonResult.Overlap)
            {
                using (Transaction tran = new Transaction(doc))
                {
                    tran.Start("1033067630");

                    var OrginPoint = intersectPoint.get_Item(0).XYZPoint;

                    var elementId = PlumbingUtils.BreakCurve(doc, MainDuct.Id, OrginPoint);

                    duct3 = doc.GetElement(elementId) as MEPCurve;

                    tran.Commit();
                }
            }


            ConnectTwoDuctsWithElbow(doc, MainDuct, duct3, LessDuct);

            return Result.Succeeded;
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
                using (Transaction tran = new Transaction(doc))
                {
                    tran.Start("xx");

                    var elbow = doc.Create.NewTeeFitting(connector1, connector2, connector3);

                    tran.Commit();
                }
            }
        }
    }

    //最后就是4通，依旧是角度问题要注意，且连接器内参数，第一个和第二个参数共线，第三个和第四个参数共线（即在同一根线上）

    [Transaction(TransactionMode.Manual)]
    class NewCross : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MEPCurve duct3, duct4;
            duct3 = duct4 = null;
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;

            //选取两个风管
            var reference1 = sel.PickObject(ObjectType.Element, "请选择第1个风管");
            MEPCurve duct1 = doc.GetElement(reference1) as MEPCurve;

            var reference2 = sel.PickObject(ObjectType.Element, "请选择第2个风管");
            MEPCurve duct2 = doc.GetElement(reference2) as MEPCurve;

            Curve curve1 = (doc.GetElement(reference1).Location as LocationCurve).Curve;

            Curve curve2 = (doc.GetElement(reference2).Location as LocationCurve).Curve;

            IntersectionResultArray intersectPoint = new IntersectionResultArray();

            var x = curve1.Intersect(curve2, out intersectPoint);

            if (x == SetComparisonResult.Overlap)
            {
                using (Transaction tran = new Transaction(doc))
                {
                    tran.Start("x");

                    var OrginPoint = intersectPoint.get_Item(0).XYZPoint;

                    var elementId3 = PlumbingUtils.BreakCurve(doc, duct1.Id, OrginPoint);

                    duct3 = doc.GetElement(elementId3) as MEPCurve;//duct1 and duct3 in one line 

                    var elementId4 = PlumbingUtils.BreakCurve(doc, duct2.Id, OrginPoint);

                    duct4 = doc.GetElement(elementId4) as MEPCurve;//duct2 and duct4 in one line

                    tran.Commit();
                }
            }

            ConnectTwoDuctsWithElbow(doc, duct1, duct3, duct2, duct4);//四通

            return Result.Succeeded;
        }
        /// <summary>
        /// 创建连接,四通前2个共线，后2个共线
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="duct1"></param>
        /// <param name="duct2"></param>
        /// <param name="duct3"></param>
        public static void ConnectTwoDuctsWithElbow(Document doc, MEPCurve duct1, MEPCurve duct2, MEPCurve duct3, MEPCurve duct4)
        {
            double minDistance = double.MaxValue;
            Connector connector1, connector2, connector3, connector4;
            connector1 = connector2 = connector3 = connector4 = null;

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

            minDistance = double.MaxValue;//重置
            foreach (Connector con4 in duct4.ConnectorManager.Connectors)
            {
                var dis = con4.Origin.DistanceTo(connector1.Origin);
                if (dis < minDistance)
                {
                    minDistance = dis;
                    connector4 = con4;
                }
            }


            if (connector1 != null && connector2 != null && connector3 != null && connector4 != null)
            {
                using (Transaction tran = new Transaction(doc))
                {
                    tran.Start("xx");

                    var elbow = doc.Create.NewCrossFitting(connector1, connector2, connector3, connector4);

                    tran.Commit();
                }
            }
        }
    }
    
    [Transaction(TransactionMode.Manual)]
    class NewTeeFitting2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;

            //选取管道，管道必须相交，否则intersectPoint为空
            var reference1 = sel.PickObject(ObjectType.Element, "请选择第1个管");
            MEPCurve duct1 = doc.GetElement(reference1) as MEPCurve;

            var reference2 = sel.PickObject(ObjectType.Element, "请选择第2个管");
            MEPCurve duct2 = doc.GetElement(reference2) as MEPCurve;

            var reference3 = sel.PickObject(ObjectType.Element, "请选择第3个管");
            MEPCurve duct3 = doc.GetElement(reference3) as MEPCurve;

            Curve curve1 = (doc.GetElement(reference1).Location as LocationCurve).Curve;
            Curve curve2 = (doc.GetElement(reference2).Location as LocationCurve).Curve;
            Curve curve3 = (doc.GetElement(reference3).Location as LocationCurve).Curve;

            var ductList = GetMainDuct(duct1, duct2);

            MEPCurve MainDuct = ductList[0];//the main Pipe

            MEPCurve LessDuct = ductList[1];// the minor Pipe

            MEPCurve duct4 = null;//the main pipe

            IntersectionResultArray intersectPoint12 = new IntersectionResultArray();//交点数组,管道必须相交
            IntersectionResultArray intersectPoint13 = new IntersectionResultArray();
            IntersectionResultArray intersectPoint23 = new IntersectionResultArray();
            curve2.MakeUnbound(); //判断曲线是否相交时，线段变为直线判断。
            curve3.MakeUnbound();

            var x = curve1.Intersect(curve2, out intersectPoint12);//输出曲线1和曲线2的交点
            var x1 = curve1.Intersect(curve3, out intersectPoint13);
            var x2 = curve2.Intersect(curve3, out intersectPoint23);

            #region Intersect返回值解释

            string info = "曲线1和曲线2：" + x
                + "\n" + "曲线1和曲线3：" + x1
                + "\n" + "曲线2和曲线3：" + x2;
            TaskDialog.Show("1", info);
            //1.SetComparisonResult.Overlap，共面且相交。
            //2.SetComparisonResult.Subset，共线，且只有一个交点，即两条有边界直线共线且首尾相连。
            //3.SetComparisonResult.Superset，共线；注：使用前需将其中一条曲线MakeUnbound();
            //4.SetComparisonResult.Disjoint，无交点,可能是共面且平行，也可能是空间内不共面；
            //5.SetComparisonResult.Equal，两条直线有重合部分（只有一个交点的情况除外）。
            #endregion

            //如果管道1和管道2有交点，则用两管创建三通；无交点则用三个管创建。
            if (x == SetComparisonResult.Overlap)
            {//这一部分代码是两管生成三通
                using (Transaction tran = new Transaction(doc))
                {
                    tran.Start("创建三通");

                    var OrginPoint = intersectPoint12.get_Item(0).XYZPoint;

                    ElementId elementId = null;
                    try
                    {
                        elementId = PlumbingUtils.BreakCurve(doc, MainDuct.Id, OrginPoint);//打断主管道,仅限水管
                    }
                    catch
                    {
                        elementId = MechanicalUtils.BreakCurve(doc, MainDuct.Id, OrginPoint);//打断主管道,仅限风管
                    }

                    duct4 = doc.GetElement(elementId) as MEPCurve;

                    ConnectTwoDuctsWithElbow(doc, MainDuct, duct4, LessDuct);

                    tran.Commit();
                }
            }
            else
            {//这一部分代码是用三管生成三通

                Transaction tran2 = new Transaction(doc);
                tran2.Start("创建三通2");


                ConnectTwoDuctsWithElbow(doc, duct1, duct2, duct3);
                tran2.Commit();
            }



            return Result.Succeeded;
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

            //计算曲线1两个点到曲线2的距离
            var distance1 = curve2.Distance(pointA1);
            var distance2 = curve2.Distance(pointA2);

            //计算曲线2两个点到曲线1的距离
            var distance3 = curve1.Distance(pointB1);
            var distance4 = curve1.Distance(pointB2);

            disList.Add(distance1);
            disList.Add(distance2);
            disList.Add(distance3);
            disList.Add(distance4);

            dis = disList.Min();//得到最小值
            var x = disList.IndexOf(dis);//获取最小值在数组中的位置

            if (x < 2)//曲线2是主管
            {
                mEPCurves.Add(duct2);
                mEPCurves.Add(duct1);
                return mEPCurves;
            }
            else//曲线1是主管
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
                var elbow = doc.Create.NewTeeFitting(connector1, connector2, connector3);

            }
        }
    }
}

