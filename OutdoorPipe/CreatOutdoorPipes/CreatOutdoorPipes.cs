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
using System.Text.RegularExpressions;
using System.Windows.Interop;
using System.Diagnostics;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreatOutdoorPipes : IExternalCommand
    {
        public static CreatOutdoorPipesForm mainfrm;     
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication app = commandData.Application;
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;

                List<OutdoorPipeSizeInfo> pipeInfoList = new List<OutdoorPipeSizeInfo>();
                foreach (string sys in ExecuteEventCreatOutdoorPipes.GetPipeSystemType(doc, "给排水"))
                {
                    foreach (string type in ExecuteEventCreatOutdoorPipes.GetPipeType(doc, "给排水"))
                    {
                        List<string> pipesizes = ExecuteEventCreatOutdoorPipes.GetPipeSizeMinAndMax(doc, type);
                        pipeInfoList.Add(new OutdoorPipeSizeInfo(sys, type, pipesizes));
                    }
                }              

                mainfrm = new CreatOutdoorPipesForm(pipeInfoList);            
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
    public class ExecuteEventCreatOutdoorPipes : IExternalEventHandler
    {

        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                using (Transaction trans = new Transaction(doc, "批量创建室外管道"))
                {
                    trans.Start();
                    AutoCreatPipes(doc, uidoc, CreatOutdoorPipes.mainfrm);
                    trans.Commit();
                }
                CreatOutdoorPipes.mainfrm.Show();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "批量创建室外管道";
        }
        public void AutoCreatPipes(Document doc, UIDocument uidoc, CreatOutdoorPipesForm win)
        {
            Selection selection = uidoc.Selection;
            IList<Reference> refList = selection.PickObjects(ObjectType.Element, new DetailineSelectionFilter(), "请选择详图线");
            if (refList.Count == 0)
            {
                TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                AutoCreatPipes(doc, uidoc, win);
            }
            else
            {
                for (int i = 0; i < win.PipeSettingGrid.Items.Count; i++)
                {
                    CreatPipes(doc, refList, "给排水", win.GetComBoxValue(i, 1, "TextPipeSystem"), win.GetComBoxValue(i, 2, "TextPipeType"), Regex.Replace(win.GetComBoxValue(i, 3, "TextPipeSize"), @"[^0-9]+", ""), win.GetTextBlockValue(i, 4), win.GetTextBlockValue(i, 5));
                }
            }
        }
        public static void CreatPipes(Document doc, IList<Reference> referenceList, string profession, string pipeSystemType, string pipeType, string dn, string pipeHeight, string pipeOffset)
        {
            // 创建管道主函数
            Options opts = new Options { DetailLevel = ViewDetailLevel.Fine, ComputeReferences = true };
            List<Pipe> pipeList = new List<Pipe>();
           
            foreach (Reference reference in referenceList)
            {
                DetailLine detailLine = doc.GetElement(reference.ElementId) as DetailLine;
                var geometry = detailLine.get_Geometry(opts);
                Line centerline = geometry.First() as Line;
                Line newline = centerline.CreateOffset(-UnitUtils.Convert(int.Parse(pipeOffset), DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), new XYZ(0, 0, 1)) as Line;

                var start = newline.GetEndPoint(0);
                var end = newline.GetEndPoint(1);
                PipeType steelpipe = GetPipeType(doc, profession, pipeType);
                PipingSystemType xh = GetPipeSystemType(doc, profession, pipeSystemType);
                Pipe p = Pipe.Create(doc, xh.Id, steelpipe.Id, doc.ActiveView.GenLevel.Id, start, end);

                ChangePipeSize(p, dn);
                ChangePipeHeight(p, pipeHeight);
                pipeList.Add(p);
            }

            for (int i = 0; i < pipeList.Count - 1; i++)
            {
                MEPCurve pipe1 = pipeList.ElementAt(i) as MEPCurve;
                MEPCurve pipe2 = pipeList.ElementAt(i + 1) as MEPCurve;
                Curve curve1 = (pipe1.Location as LocationCurve).Curve;
                Curve curve2 = (pipe2.Location as LocationCurve).Curve;

                if (!(CurvePosition(curve1, curve2).Equals(SetComparisonResult.Subset)))
                {
                    ConnectTwoPipesWithElbow(doc, pipe1, pipe2);
                }
            }

            //List<Connector> allConList = new List<Connector>();
            //foreach (Pipe item in pipeList)
            //{
            //    ConnectorSet set = item.ConnectorManager.Connectors;
            //    foreach (Connector con in set)
            //    {
            //        allConList.Add(con);
            //    }
            //}
            //foreach (Connector con1 in allConList)
            //{
            //    foreach (Connector con2 in allConList)
            //    {
            //        if ((con1.Origin == con2.Origin) && !((con1.CoordinateSystem.BasisX == con2.CoordinateSystem.BasisX) &&
            //            (con1.CoordinateSystem.BasisY == con2.CoordinateSystem.BasisY) && (con1.CoordinateSystem.BasisZ == con2.CoordinateSystem.BasisZ)))
            //        {
            //            doc.Create.NewElbowFitting(con1, con2);
            //        }
            //    }
            //}
            //List<Connector> connectors = GetConnectors(pipeList);
            //List<MyConnector> conn = GetUsefulConnectors(connectors);
            //CreateElbow(doc, conn);
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
                if (level.Name == Levelname)
                {
                    newlevel = level;
                    break;
                }
            }
            return newlevel;
        }
        public static PipeType GetPipeType(Document doc, string profession, string pipetype)
        {
            // 获取管道类型
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains(profession) && ps.Name.Contains(pipetype))
                {
                    pt = ps;
                    break;
                }
            }
            return pt;
        }
        public static PipingSystemType GetPipeSystemType(Document doc, string profession, string pipesystemtype)
        {
            // 获取管道系统
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = collector.ToElements();
            PipingSystemType pipesys = null;
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;
                if (ps.Name.Contains(profession) && ps.Name.Contains(pipesystemtype))
                {
                    pipesys = ps;
                    break;
                }
            }
            return pipesys;
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
        public static void ChangePipeSize(Pipe pipe, string dn)
        {
            //改变管道尺寸
            Parameter diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            diameter.SetValueString(dn);
        }
        public static void ChangePipeHeight(Pipe pipe, string height)
        {
            //改变管道高度
            Parameter pipeDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            string pipeDiameterValue = pipeDiameter.AsValueString();
            string bottomHeight = (double.Parse(height) + double.Parse(pipeDiameterValue) / 2).ToString();

            Parameter ht = pipe.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            ht.SetValueString(bottomHeight);
        }
        public static void ChangeFittingSize(FamilyInstance fi, double dWidth, double dHeight)
        {
            // 改变管件尺寸
            ConnectorSetIterator csi = fi.MEPModel.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                conn.Width = dWidth / 304.8;
                conn.Height = dHeight / 304.8;
            }
        }
        public static List<string> GetPipeSystemType(Document doc, string profession)
        {
            // 获取管道系统名称列表
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = collector.ToElements();
            List<string> pipesystemname = new List<string>();
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;
                if (ps.Name.Contains(profession))
                {
                    pipesystemname.Add(ps.Name);
                }
            }
            return pipesystemname;
        }
        public static List<string> GetPipeType(Document doc, string profession)
        {
            // 获取管道类型列表
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            List<string> pipetypename = new List<string>();
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains(profession))
                {
                    pipetypename.Add(ps.Name);
                }
            }
            return pipetypename;
        }
        public static List<string> GetAllPipeSize(Document doc, string pipetype)
        {
            // 获取管道全部尺寸类型
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeSegment));
            IList<Element> pipesizes = collector.ToElements();
            List<string> pipesizename = new List<string>();
            ICollection<MEPSize> mepsize;
            foreach (Element e in pipesizes)
            {
                PipeSegment ps = e as PipeSegment;
                if (ps.Name.Contains(pipetype))
                {
                    mepsize = ps.GetSizes();
                    foreach (var item in mepsize)
                    {
                        pipesizename.Add("DN" + (item.NominalDiameter * 304.8).ToString());
                    }
                    break;
                }
            }
            return pipesizename;
        }
        public static List<string> GetPipeSizeMinAndMax(Document doc, string pipetype)
        {
            // 获取管道布管系统全部尺寸类型
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();

            FilteredElementCollector collector1 = new FilteredElementCollector(doc).OfClass(typeof(PipeSegment));
            IList<Element> pipesizes = collector1.ToElements();
            List<string> pipesizename = new List<string>();
            ICollection<MEPSize> mepsize;
            string pipetypeNew = pipetype.Replace("_厂区","");

            foreach (Element e in pipetypes)
            {
                PipeType pt = e as PipeType;
                if (pt.Name.Contains("给排水") && pt.Name.Contains(pipetypeNew))
                {
                    RoutingPreferenceManager rpf = pt.RoutingPreferenceManager;
                    int i = rpf.GetNumberOfRules(RoutingPreferenceRuleGroupType.Segments);//获取管段设置数量
                    RoutingPreferenceRule rpr = rpf.GetRule(RoutingPreferenceRuleGroupType.Segments, 0);//获取第一个参数管段设置
                    Element mid = doc.GetElement(rpr.MEPPartId);//获取管段
                    PrimarySizeCriterion psc = rpr.GetCriterion(0) as PrimarySizeCriterion;//获取管段的设置值
                    double minimunSize = psc.MinimumSize;
                    double maximunSize = psc.MaximumSize;

                    foreach (Element e1 in pipesizes)
                    {
                        PipeSegment ps = e1 as PipeSegment;
                        if (ps.Name.Contains(pipetypeNew))
                        {
                            mepsize = ps.GetSizes();
                            foreach (var item in mepsize)
                            {
                                if ((item.NominalDiameter >= minimunSize) && (item.NominalDiameter <= maximunSize))
                                {
                                    pipesizename.Add("DN" + (item.NominalDiameter * 304.8).ToString());
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return pipesizename;
        }
        public static SetComparisonResult CurvePosition(Curve curve1, Curve curve2)
        {
            IntersectionResultArray resultArray = null;
            SetComparisonResult result = curve1.Intersect(curve2, out resultArray);
            return result;
        }

        /// <summary>
        /// 获取管道两端的Connector
        /// </summary>
        /// <param name="ducts"></param>
        /// <returns></returns>
        public static List<Connector> GetConnectors(List<Pipe> pipes)
        {
            List<Connector> connectors = new List<Connector>();
            foreach (Pipe pi in pipes)
            {
                ConnectorSet connectorSet = pi.ConnectorManager.Connectors;
                foreach (Connector cn in connectorSet)
                {
                    connectors.Add(cn);
                }
            }
            return connectors;
        }

        /// <summary>
        ///过滤可以创建弯头的connector 
        /// </summary>
        /// <param name="connectors"></param>
        /// <returns></returns>
        public static List<MyConnector> GetUsefulConnectors(List<Connector> connectors)
        {
            List<MyConnector> myConnectors = new List<MyConnector>();
            for (int i = 0; i < connectors.Count; i++)
            {
                for (int j = 0; j < connectors.Count; j++)
                {
                    if (connectors[i].Owner.Id != connectors[j].Owner.Id && connectors[i].Origin.IsAlmostEqualTo(connectors[j].Origin))
                    {
                        MyConnector con = new MyConnector(connectors[i], connectors[j]);
                        // connectors.Remove(connectors[i]);
                        connectors.Remove(connectors[j]);
                        myConnectors.Add(con);
                    }
                }
            }
            return myConnectors;
        }

        /// <summary>
        /// 创建弯头
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="myConnectors"></param>
        public static void CreateElbow(Document doc, List<MyConnector> myConnectors)
        {
            foreach (MyConnector mc in myConnectors)
            {
                doc.Create.NewElbowFitting(mc.First, mc.Second);
            }
        }
    }

    /// <summary>
    /// 这个类会定义两个Connector类型的属性来保存两个相近的，可创建弯头的Connector
    /// </summary>
    public class MyConnector
    {
        private Connector _first;
        private Connector _second;

        public Connector First { get => _first; set => _first = value; }
        public Connector Second { get => _second; set => _second = value; }
        public MyConnector(Connector c1, Connector c2)
        {
            First = c1;
            Second = c2;
        }
    }
}
