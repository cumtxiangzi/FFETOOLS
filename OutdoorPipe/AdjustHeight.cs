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
    class AdjustHeight : IExternalCommand
    {
        private Autodesk.Revit.ApplicationServices.Application app;

        private Document doc;

        private View3D view3D;

        private FamilyInstance pipesuppport;

        private TopographySurface topography;


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;
            Document doc = uidoc.Document;

            FilteredElementCollector SupportCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_PipeAccessory);
            IList<Element> supports = SupportCollector.ToElements();

            FilteredElementCollector TopographCollector = new FilteredElementCollector(doc).OfClass(typeof(TopographySurface)).OfCategory(BuiltInCategory.OST_Topography);
            IList<Element> topographs = TopographCollector.ToElements();
            topography = topographs.ElementAt(0) as TopographySurface;
            //foreach (Element elm in topographs)
            //{
            //    TopographySurface topo = elm as TopographySurface;
            //    if (topo.Name.Contains("表面"))
            //    {
            //        topography = topo;
            //        break;
            //    }
            //}

            using (Transaction trans = new Transaction(doc, "调整独立支架高度"))
            {
                trans.Start();
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                Func<View3D, bool> isNotTemplate = v3 => !(v3.IsTemplate);
                view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First<View3D>(isNotTemplate);

                foreach (Element elm in supports)
                {
                    FamilyInstance support = elm as FamilyInstance;
                    if (support.Name.Contains("给排水支架") && support.Name.Contains("支柱"))
                    {
                        pipesuppport = support;
                        Line line = CalculateHeight();
                        double l = UnitUtils.Convert(line.Length, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
                        Parameter height = pipesuppport.LookupParameter("钢支柱高度调整");
                        double d = Convert.ToDouble(pipesuppport.LookupParameter("管架_直径").AsString());
                        double h = l-180-d/2;
                        height.SetValueString(h.ToString());

                    }
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
            XYZ center = null;
            ConnectorSetIterator set = pipesuppport.MEPModel.ConnectorManager.Connectors.ForwardIterator();
            while (set.MoveNext())
            {
                Connector co = set.Current as Connector;
                center = co.Origin;
            }

            // Project in the negative Z direction down to the floor.
            XYZ rayDirection = new XYZ(0, 0, -1);

            // Look for references to faces where the element is the floor element id.
            ReferenceIntersector referenceIntersector = new ReferenceIntersector(topography.Id, FindReferenceTarget.Mesh, view3D);
            IList<ReferenceWithContext> references = referenceIntersector.Find(center, rayDirection);

            double distance = Double.PositiveInfinity;
            XYZ intersection = null;
            foreach (ReferenceWithContext referenceWithContext in references)
            {
                Reference reference = referenceWithContext.GetReference();
                // Keep the closest matching reference (using the proximity parameter to determine closeness).
                double proximity = referenceWithContext.Proximity;
                if (proximity < distance)
                {
                    distance = proximity;
                    intersection = reference.GlobalPoint;
                }
            }
            // Create line segment from the start point and intersection point.
            Line result = Line.CreateBound(center, intersection);
            return result;
        }
    }
}
