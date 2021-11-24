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
    public class PipeAvoid : IExternalCommand
    {
        public static PipeAvoidForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new PipeAvoidForm();
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
    public class ExecuteEventPipeAvoid : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                double a = Math.Cos(90 * Math.PI / 180);
                MessageBox.Show(a.ToString());

                using (Transaction trans = new Transaction(doc, "管道翻弯避让"))
                {
                    trans.Start();
                    PipeAvoidMethod(doc, uidoc);
                    trans.Commit();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "管道翻弯避让";
        }
        public void PipeAvoidMethod(Document doc, UIDocument uidoc)
        {
            double heigth = PipeAvoid.mainfrm.Height;
            double angle = PipeAvoid.mainfrm.Angle;
            //获取要处理的管道和创建管道所需的所有点
            List<Line> lines = FirstStep(uidoc, angle, heigth, out Pipe pi);
            Pipe pipe = pi;

            //收集新生成的管道，共后面生成弯头
            List<Pipe> pipes = new List<Pipe>();

            pipes = FinalStep(uidoc, lines, pipe);
            List<Connector> connectors = GetConnectors(pipes);
            List<MyConnector> conn = GetUsefulConnectors(connectors);
            CreateElbow(uidoc, conn);
            //TaskDialog.Show("number", conn.Count.ToString());
            uidoc.Document.Delete(pipe.Id);
        }

        /// <summary>
        /// 创建弯头
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="myConnectors"></param>
        public static void CreateElbow(UIDocument uidoc, List<MyConnector> myConnectors)
        {
            foreach (MyConnector mc in myConnectors)
            {
                uidoc.Document.Create.NewElbowFitting(mc.First, mc.Second);
            }
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
        /// 获取新创建的五个管道两端的十个Connector
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
        /// 复制原来的管道，把Locationcurve用新创建的线来代替
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="lines"></param>
        /// <param name="duct"></param>
        public static List<Pipe> FinalStep(UIDocument uidoc, List<Line> lines, Pipe pipe)
        {
            List<Pipe> pipes = new List<Pipe>();
            foreach (Line ll in lines)
            {
                ElementId id = ElementTransformUtils.CopyElement(uidoc.Document, pipe.Id, new XYZ(1, 0, 0)).First();
                Pipe tempPipe = uidoc.Document.GetElement(id) as Pipe;
                (tempPipe.Location as LocationCurve).Curve = ll;
                pipes.Add(tempPipe);
            }
            return pipes;
        }

        /// <summary>
        /// 按用户的输入生成五条line
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="angle"></param>
        /// <param name="heigth"></param>
        /// <returns></returns>
        public static List<Line> FirstStep(UIDocument uidoc, double angle, double heigth, out Pipe p)
        {
            Selection selection = uidoc.Selection;
            Document doc = uidoc.Document;
            Reference ref1 = selection.PickObject(ObjectType.PointOnElement, new PipeSelectionFilter(), "请选择第一个点");
            XYZ pt1 = ref1.GlobalPoint;
            Reference ref2 = selection.PickObject(ObjectType.PointOnElement, new PipeSelectionFilter(), "请选择第二个点");
            XYZ pt2 = ref2.GlobalPoint;
            Pipe pipe = doc.GetElement(ref1) as Pipe;
            p = pipe;
            LocationCurve lc = pipe.Location as LocationCurve;

            XYZ A = lc.Curve.GetEndPoint(0);
            XYZ F = lc.Curve.GetEndPoint(1);

            XYZ B = lc.Curve.Project(pt1).XYZPoint;
            XYZ E = lc.Curve.Project(pt2).XYZPoint;

            XYZ C = GetBentPoint((lc.Curve as Line).Direction, B, angle, heigth / 304.8);
            XYZ D = GetBentPoint(-(lc.Curve as Line).Direction, E, angle, heigth / 304.8);

            //判断离端点较近的点
            XYZ middle1 = GetNearPoint(A, B, E);
            XYZ middle2 = GetNearPoint(F, B, E);

            Line l1 = Line.CreateBound(A, middle1);
            Line l2 = Line.CreateBound(middle1, C);
            Line l3 = Line.CreateBound(C, D);
            Line l4 = Line.CreateBound(D, middle2);
            Line l5 = Line.CreateBound(middle2, F);
            List<Line> lines = new List<Line>();
            lines.Add(l1);
            lines.Add(l2);
            lines.Add(l3);
            lines.Add(l4);
            lines.Add(l5);

            return lines;
        }
        /// <summary>
        /// 按照角度计算点的坐标
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <param name="heigth"></param>
        /// <returns></returns>
        public static XYZ GetNearPoint(XYZ origin, XYZ pt1, XYZ pt2)
        {
            if (origin.DistanceTo(pt1) > origin.DistanceTo(pt2))
            {
                return pt2;
            }
            else
                return pt1;
        }
        /// <summary>
        /// 获取翻弯点
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <param name="heigth"></param>
        /// <returns></returns>
        public static XYZ GetBentPoint(XYZ dir, XYZ point, double angle, double heigth)
        {
            Transform transform = Transform.CreateTranslation(dir);
            double length = Math.Tan(angle) * heigth;

            XYZ res = transform.OfPoint(point);

            XYZ result = new XYZ(res.X, res.Y, res.Z + heigth);

            return result;
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
