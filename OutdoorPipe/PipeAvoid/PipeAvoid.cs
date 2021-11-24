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
        private bool transfer;
        private double angleH = 1.5707963267948966;
        private XYZ new_P_1;
        private XYZ new_P2_2;
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                using (Transaction trans = new Transaction(doc, "管道翻弯避让"))
                {
                    trans.Start();
                    PipeAvoidMethod(doc, uidoc,app);
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
        public void PipeAvoidMethod(Document doc, UIDocument uidoc, UIApplication uiapp)
        {
            double heigth = PipeAvoid.mainfrm.Height;
            double angle = PipeAvoid.mainfrm.Angle;
            double num = UnitUtils.Convert(Convert.ToDouble(heigth), DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);
            angleH = angle;

            UIApplication expr_10 = uiapp;
            Autodesk.Revit.ApplicationServices.Application arg_16_0 = expr_10.Application;
            UIDocument activeUIDocument = expr_10.ActiveUIDocument;
            Document document = activeUIDocument.Document;
            Document document2 = activeUIDocument.Document;


            Reference reference = activeUIDocument.Selection.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择翻弯的第一个点");
            XYZ globalPoint = reference.GlobalPoint;
            Pipe pipe = document2.GetElement(reference) as Pipe;
            Duct duct = document2.GetElement(reference) as Duct;
            document2.GetElement(reference);
            if (pipe != null && pipe.PipeType.RoutingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Elbows, 0).MEPPartId == new ElementId(-1))
            {
                TaskDialog.Show("警告：", "请检查管道布管系统设置，确保设置了弯头！");            
            }
            if (duct != null && duct.DuctType.RoutingPreferenceManager.GetRule(RoutingPreferenceRuleGroupType.Elbows, 0).MEPPartId == new ElementId(-1))
            {
                TaskDialog.Show("警告：", "请检查管道布管系统设置，确保设置了弯头！");            
            }
            MEPCurve mEPCurve = document2.GetElement(reference) as MEPCurve;
            LocationCurve arg_163_0 = mEPCurve.Location as LocationCurve;
            XYZ globalPoint2 = activeUIDocument.Selection.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择翻弯的第二个点").GlobalPoint;
            XYZ endPoint = arg_163_0.Curve.GetEndPoint(0);
            XYZ endPoint2 = arg_163_0.Curve.GetEndPoint(1);
            Line line = Line.CreateBound(endPoint, endPoint2);
            XYZ xYZPoint = arg_163_0.Curve.Project(globalPoint).XYZPoint;
            XYZ xYZPoint2 = arg_163_0.Curve.Project(globalPoint2).XYZPoint;
            XYZ xYZ = new XYZ(xYZPoint.X, xYZPoint.Y, xYZPoint.Z + num);
            XYZ xYZ2 = new XYZ(xYZPoint2.X, xYZPoint2.Y, xYZPoint2.Z + num);
            List<Line> newLine = this.GetNewLine(xYZPoint, xYZPoint2, num, mEPCurve);
            line.Direction.AngleTo(new XYZ(0.0, 1.0, 0.0));
            ICollection<ElementId> source = ElementTransformUtils.CopyElement(document2, mEPCurve.Id, XYZ.Zero);
            ICollection<ElementId> source2 = ElementTransformUtils.CopyElement(document2, mEPCurve.Id, XYZ.Zero);
            ICollection<ElementId> source3 = ElementTransformUtils.CopyElement(document2, mEPCurve.Id, XYZ.Zero);
            MEPCurve mEPCurve2 = document2.GetElement(source.ElementAt(0)) as MEPCurve;
            MEPCurve mEPCurve3 = document2.GetElement(source3.ElementAt(0)) as MEPCurve;
            this.ChangeLine(document2.GetElement(source.ElementAt(0)), newLine[0]);
            this.ChangeLine(document2.GetElement(source2.ElementAt(0)), newLine[2]);
            this.ChangeLine(document2.GetElement(source3.ElementAt(0)), newLine[1]);
            IList<MEPCurve> list = this.SliceMEPCurveIntoTwo(mEPCurve, xYZPoint, xYZPoint2, document);
            xYZ = this.new_P_1;
            xYZ2 = this.new_P2_2;
            if (!this.transfer)
            {
                Connector arg_365_0 = this.GetConnector(document2.GetElement(source3.ElementAt(0)), xYZPoint);
                Connector arg_35E_0 = this.GetConnector(list[0], xYZPoint);
                this.GetConnector(document2.GetElement(source.ElementAt(0)), xYZPoint2);
                this.GetConnector(list[1], xYZPoint2);
                Transform coordinateSystem = arg_35E_0.CoordinateSystem;
                double num2 = arg_365_0.CoordinateSystem.BasisY.AngleOnPlaneTo(coordinateSystem.BasisZ, XYZ.BasisZ);
                Line line2 = Line.CreateUnbound(xYZ, XYZ.BasisZ);
                Line line3 = Line.CreateUnbound(xYZ2, XYZ.BasisZ);
                if (this.angleH == 1.5707963267948966)
                {
                    mEPCurve2.Location.Rotate(line2, num2);
                    mEPCurve3.Location.Rotate(line3, num2);
                }
                document2.Create.NewElbowFitting(this.GetConnector(document2.GetElement(source3.ElementAt(0)), xYZPoint), this.GetConnector(list[0], xYZPoint));
                document2.Create.NewElbowFitting(this.GetConnector(document2.GetElement(source.ElementAt(0)), this.new_P_1), this.GetConnector(document2.GetElement(source2.ElementAt(0)), this.new_P_1));
                document2.Create.NewElbowFitting(this.GetConnector(document2.GetElement(source2.ElementAt(0)), this.new_P2_2), this.GetConnector(document2.GetElement(source3.ElementAt(0)), this.new_P2_2));
                document2.Create.NewElbowFitting(this.GetConnector(document2.GetElement(source.ElementAt(0)), xYZPoint2), this.GetConnector(list[1], xYZPoint2));
            }
            else
            {
                Connector arg_50F_0 = this.GetConnector(document2.GetElement(source.ElementAt(0)), xYZPoint);
                Connector arg_508_0 = this.GetConnector(list[1], xYZPoint);
                this.GetConnector(document2.GetElement(source3.ElementAt(0)), xYZPoint2);
                this.GetConnector(list[0], xYZPoint2);
                Transform coordinateSystem2 = arg_508_0.CoordinateSystem;
                double num3 = arg_50F_0.CoordinateSystem.BasisY.AngleOnPlaneTo(coordinateSystem2.BasisZ, XYZ.BasisZ);
                Line line4 = Line.CreateUnbound(xYZ, XYZ.BasisZ);
                Line line5 = Line.CreateUnbound(xYZ2, XYZ.BasisZ);
                if (this.angleH == 1.5707963267948966)
                {
                    mEPCurve2.Location.Rotate(line4, num3);
                    mEPCurve3.Location.Rotate(line5, num3);
                }
                document2.Create.NewElbowFitting(this.GetConnector(document2.GetElement(source.ElementAt(0)), xYZPoint), this.GetConnector(list[1], xYZPoint));
                document2.Create.NewElbowFitting(this.GetConnector(document2.GetElement(source.ElementAt(0)), this.new_P_1), this.GetConnector(document2.GetElement(source2.ElementAt(0)), this.new_P_1));
                document2.Create.NewElbowFitting(this.GetConnector(document2.GetElement(source2.ElementAt(0)), this.new_P2_2), this.GetConnector(document2.GetElement(source3.ElementAt(0)), this.new_P2_2));
                document2.Create.NewElbowFitting(this.GetConnector(document2.GetElement(source3.ElementAt(0)), xYZPoint2), this.GetConnector(list[0], xYZPoint2));
                this.transfer = false;
            }
        }
        private List<Line> GetNewLine(XYZ old_P, XYZ old_p2, double h, MEPCurve mep_curve)
        {
            List<Line> list = new List<Line>();
            LocationCurve locationCurve = mep_curve.Location as LocationCurve;
            XYZ endPoint = locationCurve.Curve.GetEndPoint(0);
            XYZ endPoint2 = locationCurve.Curve.GetEndPoint(1);
            Line line = Line.CreateBound(endPoint, old_p2);
            Line line2 = Line.CreateBound(old_P, endPoint2);
            if (line.Length + line2.Length > locationCurve.Curve.Length)
            {
                line = Line.CreateBound(endPoint, old_P);
                line2 = Line.CreateBound(endPoint2, old_p2);
                XYZ xYZ = old_P + line.Direction * (1.0 / Math.Tan(this.angleH)) * Math.Abs(h);
                this.new_P_1 = new XYZ(xYZ.X, xYZ.Y, xYZ.Z + h);
                XYZ xYZ2 = old_p2 + line2.Direction * (1.0 / Math.Tan(this.angleH)) * Math.Abs(h);
                this.new_P2_2 = new XYZ(xYZ2.X, xYZ2.Y, xYZ2.Z + h);
                Line item = Line.CreateBound(old_P, this.new_P_1);
                Line item2 = Line.CreateBound(old_p2, this.new_P2_2);
                list.Add(item);
                list.Add(item2);
                list.Add(Line.CreateBound(this.new_P_1, this.new_P2_2));
            }
            else
            {
                line = Line.CreateBound(endPoint, old_p2);
                line2 = Line.CreateBound(endPoint2, old_P);
                XYZ xYZ3 = old_p2 + line.Direction * (1.0 / Math.Tan(this.angleH)) * Math.Abs(h);
                this.new_P_1 = new XYZ(xYZ3.X, xYZ3.Y, xYZ3.Z + h);
                XYZ xYZ4 = old_P + line2.Direction * (1.0 / Math.Tan(this.angleH)) * Math.Abs(h);
                this.new_P2_2 = new XYZ(xYZ4.X, xYZ4.Y, xYZ4.Z + h);
                Line item = Line.CreateBound(old_p2, this.new_P_1);
                Line item2 = Line.CreateBound(old_P, this.new_P2_2);
                list.Add(item);
                list.Add(item2);
                list.Add(Line.CreateBound(this.new_P_1, this.new_P2_2));
            }
            return list;
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
        private void ChangeLine(Element elem, Line line)
        {
            (elem.Location as LocationCurve).Curve = line;
        }
        private void ChangeLine2(Document doc, Element elem, Line line, double angleDirect)
        {
            (elem.Location as LocationCurve).Curve = line;
            ElementTransformUtils.RotateElement(doc, elem.Id, line, angleDirect);
        }
        private XYZ PickPointOnMEPCurve(UIDocument uiDocument, MEPCurve mep, string promptTextWhilePickingPoint)
        {
            XYZ xYZ = uiDocument.Selection.PickPoint(ObjectSnapTypes.Midpoints, promptTextWhilePickingPoint);
            return (mep.Location as LocationCurve).Curve.Project(xYZ).XYZPoint;
        }
        private IList<MEPCurve> SliceMEPCurveIntoTwo(MEPCurve pipe, XYZ FirstPoint, XYZ SecondPoint, Document document)
        {
            IList<MEPCurve> list = new List<MEPCurve>();
            LocationCurve locationCurve = pipe.Location as LocationCurve;
            XYZ endPoint = locationCurve.Curve.GetEndPoint(0);
            XYZ endPoint2 = locationCurve.Curve.GetEndPoint(1);
            Curve curve = Line.CreateBound(endPoint, SecondPoint);
            Curve curve2 = Line.CreateBound(FirstPoint, endPoint2);
            if (curve.Length + curve2.Length > locationCurve.Curve.Length)
            {
                curve = Line.CreateBound(endPoint, FirstPoint);
                curve2 = Line.CreateBound(SecondPoint, endPoint2);
                this.transfer = true;
            }
            MEPCurve mEPCurve = document.GetElement(ElementTransformUtils.CopyElement(document, pipe.Id, new XYZ(0.0, 0.0, 0.0)).ElementAt(0)) as MEPCurve;
            (mEPCurve.Location as LocationCurve).Curve = curve;
            ConnectorSetIterator connectorSetIterator = pipe.ConnectorManager.Connectors.ForwardIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector connector = connectorSetIterator.Current as Connector;
                if (connector.Origin.IsAlmostEqualTo(endPoint))
                {
                    ConnectorSetIterator connectorSetIterator2 = connector.AllRefs.ForwardIterator();
                    while (connectorSetIterator2.MoveNext())
                    {
                        Connector connector2 = connectorSetIterator2.Current as Connector;
                        if (connector2 != null && (connector2.ConnectorType == ConnectorType.End || connector2.ConnectorType == ConnectorType.Curve ||
                            connector2.ConnectorType == ConnectorType.Physical) && connector2.Owner.UniqueId != pipe.UniqueId)
                        {
                            connector.DisconnectFrom(connector2);
                            ConnectorSetIterator connectorSetIterator3 = mEPCurve.ConnectorManager.Connectors.ForwardIterator();
                            while (connectorSetIterator3.MoveNext())
                            {
                                Connector connector3 = connectorSetIterator3.Current as Connector;
                                if (connector3.Origin.IsAlmostEqualTo(endPoint))
                                {
                                    connector3.ConnectTo(connector2);
                                }
                            }
                        }
                    }
                }
            }
            locationCurve.Curve = curve2;
            list.Add(pipe);
            list.Add(mEPCurve);
            return list;
        }
    }
}
