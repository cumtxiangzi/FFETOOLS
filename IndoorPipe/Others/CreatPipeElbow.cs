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
    public class CreatPipeElbow : IExternalCommand
    {
        //连接上下层管道
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                CreatPipeElbowMethod(doc, sel);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }

        public void CreatPipeElbowMethod(Document doc, Selection sel)
        {
            //选取两个管道
            var reference1 = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is MEPCurve), "请选择第1个管");
            MEPCurve duct1 = doc.GetElement(reference1) as MEPCurve;
            Pipe pipe1 = doc.GetElement(reference1) as Pipe;
            XYZ point1 = GetNearPoint(pipe1, reference1);

            var reference2 = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is MEPCurve), "请选择第2个管");
            MEPCurve duct2 = doc.GetElement(reference2) as MEPCurve;
            Pipe pipe2 = doc.GetElement(reference2) as Pipe;
            XYZ point2 = GetNearPoint(pipe2, reference2);

            Line line1 = (duct1.Location as LocationCurve).Curve as Line;
            line1.MakeUnbound();

            Line line2 = (duct2.Location as LocationCurve).Curve as Line;
            line2.MakeUnbound();

            IntersectionResult result = line1.Project(point2);
            XYZ crossPoint = result.XYZPoint;

            IntersectionResult result1 = line2.Project(crossPoint);
            XYZ crossPoint1 = result1.XYZPoint;

            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("连接上下层管道");

                if (CurvePosition(line1, line2).Equals(SetComparisonResult.Equal))
                {
                    Connector con1 = GetNearConnector(pipe1, point1);
                    Connector con2 = GetFarConnector(pipe2, point2);
                    con1.Origin = con2.Origin;

                    Connector con = GetLinkConnector(pipe2);
                    if (!(con == null))
                    {
                        con1.ConnectTo(con);
                    }
                    doc.Delete(pipe2.Id);
                }
                else if (CurvePosition(line1, line2).Equals(SetComparisonResult.Overlap))
                {
                    ConnectTwoPipesWithElbow(doc, pipe1, pipe2);
                }
                else
                {
                    Pipe pipe3 = Pipe.Create(doc, pipe2.MEPSystem.GetTypeId(), pipe2.GetTypeId(), GetPipeLevel(doc, "0.000").Id, crossPoint, crossPoint1);
                    ChangePipeSize(pipe3, pipe2.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString());
                    ConnectTwoPipesWithElbow(doc, pipe2, pipe3);
                    ConnectTwoPipesWithElbow(doc, pipe1, pipe3);
                }
                tran.Commit();
            }
            CreatPipeElbowMethod(doc, sel);
        }
        public XYZ GetNearPoint(Pipe pipe, Reference reference)
        {
            XYZ point = reference.GlobalPoint;
            Curve curve = (pipe.Location as LocationCurve).Curve;

            XYZ endPoint0 = curve.GetEndPoint(0);
            XYZ endPoint1 = curve.GetEndPoint(1);

            Line line0 = Line.CreateBound(point, endPoint0);
            Line line1 = Line.CreateBound(point, endPoint1);

            double length0 = line0.Length;
            double length1 = line1.Length;
            XYZ xyz;

            if (length0 < length1)
            {
                xyz = endPoint0;
            }
            else
            {
                xyz = endPoint1;
            }

            return xyz;
        }

        public static Connector GetLinkConnector(Pipe pipe)
        {
            ConnectorSetIterator csi = pipe.ConnectorManager.Connectors.ForwardIterator();
            Connector con = null;
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                if (conn.IsConnected == true)//是否有连接
                {
                    ConnectorSet connectorSet = conn.AllRefs;//找到所有连接器连接的连接器【这句很重要】
                    ConnectorSetIterator csiChild = connectorSet.ForwardIterator();
                    while (csiChild.MoveNext())
                    {
                        Connector connected = csiChild.Current as Connector;
                        if (null != connected && connected.Owner.UniqueId != conn.Owner.UniqueId)
                        {
                            // look for physical connections 
                            if (connected.ConnectorType == ConnectorType.End || connected.ConnectorType == ConnectorType.Curve || connected.ConnectorType == ConnectorType.Physical)
                            {
                                //判断是不是管件
                                if (connected.Owner is FamilyInstance)
                                {
                                    con = connected;
                                }
                            }
                        }
                    }
                }
            }
            return con;
        }

        public static Connector GetNearConnector(Pipe pipe, XYZ point)
        {
            ConnectorManager manager = pipe.ConnectorManager;
            ConnectorSet set = manager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            Connector con = null;
            Connector con1 = conList.ElementAt(0);
            Connector con2 = conList.ElementAt(1);
            if (con1.Origin.DistanceTo(point) > con2.Origin.DistanceTo(point))
            {
                con = con2;
            }
            else
            {
                con = con1;
            }
            return con;
        }
        public static Connector GetFarConnector(Pipe pipe, XYZ point)
        {
            ConnectorManager manager = pipe.ConnectorManager;
            ConnectorSet set = manager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            Connector con = null;
            Connector con1 = conList.ElementAt(0);
            Connector con2 = conList.ElementAt(1);
            if (con1.Origin.DistanceTo(point) > con2.Origin.DistanceTo(point))
            {
                con = con1;
            }
            else
            {
                con = con2;
            }
            return con;
        }
        public static void ChangePipeSize(Pipe pipe, string dn)
        {
            //改变管道尺寸
            Parameter diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            diameter.SetValueString(dn);
        }
        public static SetComparisonResult CurvePosition(Curve curve1, Curve curve2)
        {
            IntersectionResultArray resultArray = null;
            SetComparisonResult result = curve1.Intersect(curve2, out resultArray);
            return result;
        }
        public static void ConnectTwoPipesWithElbow(Document doc, MEPCurve pipe1, MEPCurve pipe2)
        {
            // 创建弯头
            double minDistance = double.MaxValue;
            Connector connector1, connector2;
            connector1 = connector2 = null;

            foreach (Connector con1 in pipe1.ConnectorManager.Connectors)
            {
                foreach (Connector con2 in pipe2.ConnectorManager.Connectors)
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
        public List<Connector> GetPipeConnectors(Pipe pipe)
        {
            ConnectorManager manager = pipe.ConnectorManager;
            ConnectorSet set = manager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }
            return conList;
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
    }
}
