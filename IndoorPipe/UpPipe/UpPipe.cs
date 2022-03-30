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
    public class UpPipe : IExternalCommand
    {
        public static UpPipeForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new UpPipeForm();
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
    public class ExecuteEventUpPipe : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
               
                MakeUpPipeMethod(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "生成立管";
        }
        public void MakeUpPipeMethod(Document doc, UIDocument uidoc)
        {    
            double num = UnitUtils.Convert(Convert.ToDouble(UpPipe.mainfrm.LengthValue.Text), DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is MEPCurve), "请选择需要末端绘制立管的管道");
            XYZ point = reference.GlobalPoint;
            Pipe pipe = doc.GetElement(reference) as Pipe;

            using (Transaction trans = new Transaction(doc, "自动生成立管"))
            {
                trans.Start();

                ICollection<ElementId> source = ElementTransformUtils.CopyElement(doc, pipe.Id, XYZ.Zero);
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

                XYZ xyz2 = new XYZ(xyz.X, xyz.Y, xyz.Z + num);
                new XYZ(0.0, 0.0, num);

                Line line3 = Line.CreateBound(xyz, xyz2);
                ChangeLine(doc.GetElement(source.ElementAt(0)), line3);
                doc.Create.NewElbowFitting(GetConnector(doc.GetElement(source.ElementAt(0)), xyz), GetConnector(pipe, xyz));

                trans.Commit();
            }
            MakeUpPipeMethod(doc, uidoc);
        }
        private void ChangeLine(Element elem, Line line)
        {
            (elem.Location as LocationCurve).Curve = line;
        }
        private Connector GetConnector(Element elem, XYZ xyz)
        {
            Connector result = null;
            ConnectorSetIterator connectorSetIterator = (elem as MEPCurve).ConnectorManager.Connectors.ForwardIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector connector = connectorSetIterator.Current as Connector;
                if (connector.Origin.IsAlmostEqualTo(xyz))
                {
                    result = connector;
                    return result;
                }
            }
            return result;
        }
    }
}
