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
    public class BatchCreatPipeElbow : IExternalCommand
    {
        //批量连接上下层管道
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                using (Transaction trans = new Transaction(doc, "批量连接上下层管道"))
                {
                    trans.Start();
                    CreatPipeElbowMain(doc, uidoc);
                    trans.Commit();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
        public void CreatPipeElbowMain(Document doc, UIDocument uidoc)
        {
            Selection sel = uidoc.Selection;

            IList<Reference> refList = sel.PickObjects(ObjectType.Element, new PipeSelectionFilter(), "请选要互连的管道");

            List<Pipe> pipeList = new List<Pipe>();
            List<string> pipeSystemList = new List<string>();

            foreach (Reference item in refList)
            {
                Pipe pipe = doc.GetElement(item) as Pipe;
                string name = (doc.GetElement(pipe.MEPSystem.GetTypeId()) as PipingSystemType).Name;
                pipeSystemList.Add(name);
            }
            List<string> ListTemp = pipeSystemList.Distinct().ToList();//去除重复项

            if (refList.Count == 0)
            {
                TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                CreatPipeElbowMain(doc, uidoc);
            }
            else
            {
                foreach (string item in ListTemp)
                {
                    CreatPipeElbowMethod(doc, refList, item);
                }
            }
        }
        public void CreatPipeElbowMethod(Document doc, IList<Reference> refList, string pipeSystemName)
        {
            List<Pipe> pipeList = new List<Pipe>();
            foreach (var item in refList)
            {
                Pipe p = item.GetElement(doc) as Pipe;
                if (p.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString().Contains(pipeSystemName))
                {
                    pipeList.Add(p);
                }
            }

            MEPCurve pipe1 = pipeList.ElementAt(0) as MEPCurve;
            MEPCurve pipe2 = pipeList.ElementAt(1) as MEPCurve;
            Curve curve1 = (pipe1.Location as LocationCurve).Curve;
            Curve curve2 = (pipe2.Location as LocationCurve).Curve;

            var start = curve1.GetEndPoint(1);
            var end = curve2.GetEndPoint(0);

            List<Connector> conList1 = GetPipeConnectors(pipeList.ElementAt(0));
            List<Connector> conList2 = GetPipeConnectors(pipeList.ElementAt(1));
            List<double> distanceList = new List<double>();

            foreach (var con10 in conList1)
            {
                foreach (var con20 in conList2)
                {
                    distanceList.Add(con10.Origin.DistanceTo(con20.Origin));
                }
            }
            distanceList.Sort();
            double minDistance = distanceList.ElementAt(0);

            Connector con1 = null;
            Connector con2 = null;
            foreach (var con10 in conList1)
            {
                foreach (var con20 in conList2)
                {
                    if (con10.Origin.DistanceTo(con20.Origin) == minDistance)
                    {
                        con1 = con10;
                        con2 = con20;
                        break;
                    }
                }
            }

            Pipe newPipe = Pipe.Create(doc, pipe1.MEPSystem.GetTypeId(), pipe1.GetTypeId(), GetPipeLevel(doc, "0.000").Id, con1.Origin, con2.Origin);
            MEPCurve pipe3 = newPipe as MEPCurve;
            ChangePipeSize(newPipe, pipe1.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString());

            if (!(CurvePosition(curve1, curve2).Equals(SetComparisonResult.Subset)))
            {
                ConnectTwoPipesWithElbow(doc, pipe1, pipe3);
                ConnectTwoPipesWithElbow(doc, pipe3, pipe2);
            }
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
