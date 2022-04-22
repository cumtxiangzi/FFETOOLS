using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.InteropServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class AdjustHeightWell : IExternalCommand
    {
        private Autodesk.Revit.ApplicationServices.Application app;

        private Document doc;

        private View3D view3D;

        private FamilyInstance well;

        private Pipe pipe;


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;
            Document doc = uidoc.Document;

            FilteredElementCollector wellCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_Site);
            IList<Element> wells = wellCollector.ToElements();

            FilteredElementCollector pipeCollector = new FilteredElementCollector(doc).OfClass(typeof(Pipe)).OfCategory(BuiltInCategory.OST_PipeCurves);
            IList<Element> pipes = pipeCollector.ToElements();           
            Line ln = null;
            List<Line> lines=new List<Line>() ;

            using (Transaction trans = new Transaction(doc, "调整排水井深度"))
            {
                trans.Start();
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                Func<View3D, bool> isNotTemplate = v3 => !(v3.IsTemplate);
                view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First<View3D>(isNotTemplate);

                foreach (Element elm in wells)
                {
                    FamilyInstance w = elm as FamilyInstance;
                    if (w.Name.Contains("给排水") && w.Name.Contains("塑料排水") && w.Name.Contains("直通式"))
                    {                   
                        well = w;
                        foreach (Element elmp in pipes)
                        {
                            Pipe p = elmp as Pipe;
                            if (p.Name.Contains("HDPE"))
                            {
                                pipe = p;
                                ln = CalculateHeight();
                                if (ln!=null)
                                {
                                    lines.Add(ln);                                   
                                }
                            }
                        }
                        Line l1 = lines.ElementAt(0);
                        Line l2 = lines.ElementAt(1);
                        Line line = null;
                        if (l1.Length > l2.Length)
                        {
                            line = l1;
                        }
                        else
                        {
                            line = l2;
                        }
                        double l = UnitUtils.Convert(line.Length, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
                        Parameter height = well.LookupParameter("管中心埋深");
                        //double h = Convert.ToDouble(height.AsValueString());                     
                        height.SetValueString((l-40).ToString());

                        //Plane plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), line.GetEndPoint(0));
                        //SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                        //ModelCurve curve = doc.Create.NewModelCurve(line, sketchPlane);
                        //TaskDialog.Show("Distance", "Distance to floor: " + String.Format("{0:f2}", l));
                        
                    }
                    lines.Clear();
                }
                trans.Commit();
            }
            return Result.Succeeded;

            // Find a 3D view to use for the ray tracing operation

            //View3D view3d = null;
            //view3d = doc.ActiveView as View3D;
            //if (view3d == null)
            //{
            //    TaskDialog.Show("警告", "请在三维视图下操作");
            //    return Result.Failed;
            //}                              
        }

        private Line CalculateHeight()
        {
            Line line = null;
            XYZ center = null;
            ConnectorSetIterator set = well.MEPModel.ConnectorManager.Connectors.ForwardIterator();
            while (set.MoveNext())
            {
                Connector co = set.Current as Connector;
                center = co.Origin;
            }

            XYZ p1 = new XYZ(center.X, center.Y + 350 / 304.799, center.Z);
            XYZ p2 = new XYZ(center.X, center.Y - 350 / 304.799, center.Z);
            XYZ p3 = new XYZ(center.X + 350 / 304.799, center.Y, center.Z);
            XYZ p4 = new XYZ(center.X - 350 / 304.799, center.Y, center.Z);

            // Project in the negative Z direction down to the floor.
            XYZ rayDirection = new XYZ(0, 0, -1);

            // Look for references to faces where the element is the floor element id.
            ReferenceIntersector referenceIntersector1 = new ReferenceIntersector(pipe.Id, FindReferenceTarget.Curve, view3D);
            IList<ReferenceWithContext> references1 = referenceIntersector1.Find(p1, rayDirection);
            double distance1 = Double.PositiveInfinity;
            XYZ intersection1 = null;
            foreach (ReferenceWithContext referenceWithContext in references1)
            {
                Reference reference = referenceWithContext.GetReference();
                // Keep the closest matching reference (using the proximity parameter to determine closeness).
                double proximity = referenceWithContext.Proximity;
                if (proximity < distance1)
                {
                    distance1 = proximity;
                    intersection1 = reference.GlobalPoint;
                }
            }
            if (intersection1 != null)
            {
                Line result1 = Line.CreateBound(p1, intersection1);
                line = result1;
            }

            ReferenceIntersector referenceIntersector2 = new ReferenceIntersector(pipe.Id, FindReferenceTarget.Curve, view3D);
            IList<ReferenceWithContext> references2 = referenceIntersector2.Find(p2, rayDirection);
            double distance2 = Double.PositiveInfinity;
            XYZ intersection2 = null;
            foreach (ReferenceWithContext referenceWithContext in references2)
            {
                Reference reference = referenceWithContext.GetReference();
                // Keep the closest matching reference (using the proximity parameter to determine closeness).
                double proximity = referenceWithContext.Proximity;
                if (proximity < distance2)
                {
                    distance2 = proximity;
                    intersection2 = reference.GlobalPoint;
                }
            }
            if (intersection2 != null)
            {
                Line result2 = Line.CreateBound(center, intersection2);
                line = result2;
            }

            ReferenceIntersector referenceIntersector3 = new ReferenceIntersector(pipe.Id, FindReferenceTarget.Curve, view3D);
            IList<ReferenceWithContext> references3 = referenceIntersector3.Find(p3, rayDirection);
            double distance3 = Double.PositiveInfinity;
            XYZ intersection3 = null;
            foreach (ReferenceWithContext referenceWithContext in references3)
            {
                Reference reference = referenceWithContext.GetReference();
                // Keep the closest matching reference (using the proximity parameter to determine closeness).
                double proximity = referenceWithContext.Proximity;
                if (proximity < distance3)
                {
                    distance3 = proximity;
                    intersection3 = reference.GlobalPoint;
                }
            }
            if (intersection3 != null)
            {
                Line result3 = Line.CreateBound(center, intersection3);
                line = result3;
            }

            ReferenceIntersector referenceIntersector4 = new ReferenceIntersector(pipe.Id, FindReferenceTarget.Curve, view3D);
            IList<ReferenceWithContext> references4 = referenceIntersector4.Find(p4, rayDirection);
            double distance4 = Double.PositiveInfinity;
            XYZ intersection4 = null;
            foreach (ReferenceWithContext referenceWithContext in references4)
            {
                Reference reference = referenceWithContext.GetReference();
                // Keep the closest matching reference (using the proximity parameter to determine closeness).
                double proximity = referenceWithContext.Proximity;
                if (proximity < distance4)
                {
                    distance4 = proximity;
                    intersection4 = reference.GlobalPoint;
                }
            }
            if (intersection4 != null)
            {
                Line result4 = Line.CreateBound(center, intersection4);
                line = result4;
            }
            return line;

        }
    }
}
