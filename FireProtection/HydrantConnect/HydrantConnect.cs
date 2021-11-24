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
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class HydrantConnect : IExternalCommand
    {
        public static HydrantConnectForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new HydrantConnectForm();
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
    }
    public class ExecuteEventHydrantConnect : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                HydrantConnectorMethod(doc, sel);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "连室内消火栓";
        }
        public void HydrantConnectorMethod(Document doc, Selection sel)
        {
            var reference = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is FamilyInstance), "请选择需要连接的室内消火栓");
            FamilyInstance hydrant = doc.GetElement(reference) as FamilyInstance;

            var reference1 = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is MEPCurve), "请选择需要连接的管道");
            MEPCurve duct1 = doc.GetElement(reference1) as MEPCurve;
            Pipe pipe1 = doc.GetElement(reference1) as Pipe;

            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("连室内消火栓");

                Connector hydrantCon = null;
                if (HydrantConnect.mainfrm.LeftButton.IsChecked == true)
                {
                    hydrantCon = GetHydrantConnect(hydrant, "left");
                }
                else
                {
                    hydrantCon = GetHydrantConnect(hydrant, "right");
                }

                XYZ point1 = hydrantCon.Origin;
                XYZ point2 = new XYZ(point1.X, point1.Y, point1.Z - 331.9 / 304.8);
                Pipe pipe2 = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, point1, point2);
                ChangePipeSize(pipe2, "65");
                Connector con = GetPointConnector(pipe2, point1);
                con.ConnectTo(hydrantCon);

                if (PipeDirection(pipe1) == "Standpipe")
                {
                    Line line1 = (duct1.Location as LocationCurve).Curve as Line;
                    line1.MakeUnbound();
                    if (HydrantConnect.mainfrm.DirectConnect.IsChecked==true)
                    {
                        IntersectionResult result = line1.Project(point2);
                        XYZ standPoint3 = result.XYZPoint;
                        Pipe standPipe3 = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, point2, standPoint3);
                        ChangePipeSize(standPipe3, "65");
                        ConnectTwoPipesWithElbow(doc, pipe2, standPipe3);
                        CreatTeeMethod(doc, standPipe3, pipe1);
                    }
                    else
                    {
                        XYZ standPoint3 = new XYZ(point2.X, point2.Y + 600 / 304.8, point2.Z);                  
                        IntersectionResult result = line1.Project(point2);
                        XYZ onStandPipe = result.XYZPoint;

                        Line standline1 = Line.CreateBound(point2,standPoint3);
                        standline1.MakeUnbound();
                        IntersectionResult result2 = standline1.Project(onStandPipe);
                        XYZ point4 = result2.XYZPoint;

                        Pipe standPipe3 = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, point2, point4);
                        ChangePipeSize(standPipe3, "65");

                        Pipe standPipe4 = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, point4, onStandPipe);
                        ChangePipeSize(standPipe4, "65");

                        ConnectTwoPipesWithElbow(doc, pipe2, standPipe3);
                        ConnectTwoPipesWithElbow(doc, standPipe3, standPipe4);
                        CreatTeeMethod(doc, standPipe4, pipe1);
                        //未完成各种情况判断
                    }
                }
                else
                {
                    XYZ point3 = new XYZ();
                    Pipe pipe3 = null;
                    if (PipeDirection(pipe1) == "Horizational")
                    {
                        if (HydrantConnect.mainfrm.LeftButton.IsChecked == true)
                        {
                            point3 = new XYZ(point2.X - 600 / 304.8, point2.Y, point2.Z);
                        }
                        else
                        {
                            point3 = new XYZ(point2.X + 600 / 304.8, point2.Y, point2.Z);
                        }
                        pipe3 = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, point2, point3);
                        ChangePipeSize(pipe3, "65");
                    }
                    if (PipeDirection(pipe1) == "Vertical")
                    {
                        if (HydrantConnect.mainfrm.LeftButton.IsChecked == true)
                        {
                            point3 = new XYZ(point2.X, point2.Y - 600 / 304.8, point2.Z);
                        }
                        else
                        {
                            point3 = new XYZ(point2.X, point2.Y + 600 / 304.8, point2.Z);
                        }
                        pipe3 = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, point2, point3);
                        ChangePipeSize(pipe3, "65");
                    }

                    Line line1 = (duct1.Location as LocationCurve).Curve as Line;
                    XYZ startPoitOnLine1 = line1.StartPoint();
                    XYZ endPoitOnLine1 = line1.EndPoint();

                    XYZ point4 = new XYZ(point3.X, point3.Y, startPoitOnLine1.Z);
                    Pipe pipe4 = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, point3, point4);
                    ChangePipeSize(pipe4, "65");

                    ConnectTwoPipesWithElbow(doc, pipe2, pipe3);
                    ConnectTwoPipesWithElbow(doc, pipe3, pipe4);

                    Line line4 = (pipe4.Location as LocationCurve).Curve as Line;
                    if (CurvePosition(line1, line4).Equals(SetComparisonResult.Overlap))
                    {
                        CreatTeeMethod(doc, pipe4, pipe1);
                    }
                    else
                    {
                        line1.MakeUnbound();
                        IntersectionResult result = line1.Project(point4);
                        XYZ point5 = result.XYZPoint;
                        Pipe pipe5 = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, point4, point5);
                        ChangePipeSize(pipe5, "65");
                        ConnectTwoPipesWithElbow(doc, pipe4, pipe5);
                        CreatTeeMethod(doc, pipe5, pipe1);
                    }
                }

                tran.Commit();
            }

            HydrantConnectorMethod(doc, sel);
        }
        public static Connector GetPointConnector(Pipe pipe,XYZ point)
        {
            Connector con = null;
            ConnectorSet set = pipe.ConnectorManager.Connectors;      
            foreach (Connector item in set)
            {
                if (item.Origin.IsAlmostEqualTo(point))
                {
                    con = item;
                    break;
                }
            }
            return con;
        }
        public static string PipeDirection(Pipe pipe)
        {
            string direction = null;
            Line line = (pipe.Location as LocationCurve).Curve as Line;

            if (line.Direction.X == 1 || line.Direction.X == -1)
            {
                direction = "Horizational";
            }
            if (line.Direction.Y == 1 || line.Direction.Y == -1)
            {
                direction = "Vertical";
            }
            if (line.Direction.Z == 1 || line.Direction.Z == -1)
            {
                direction = "Standpipe";
            }
            return direction;
        }
        public static string HydrantConDirection(FamilyInstance hydrant)
        {
            string direction = null;
            ConnectorSet set = hydrant.MEPModel.ConnectorManager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            Connector con1 = conList.ElementAt(0);
            Connector con2 = conList.ElementAt(1);

            if (con1.Id == 2)
            {
                direction = "right";
            }
            if (con1.Id == 3)
            {
                direction = "left";
            }

            return direction;
        }

        public static Connector GetHydrantConnect(FamilyInstance hydrant, string direction)
        {
            ConnectorSet set = hydrant.MEPModel.ConnectorManager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            Connector con = null;
            if (direction == "right")
            {
                foreach (Connector item in conList)
                {
                    if (item.Id == 2)
                    {
                        con = item;
                        break;
                    }
                }
            }
            if (direction == "left")
            {
                foreach (Connector item in conList)
                {
                    if (item.Id == 3)
                    {
                        con = item;
                        break;
                    }
                }
            }
            return con;
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
