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
    class ExtendPipe : IExternalCommand
    {     //管线延长未测试
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            Reference reference = sel.PickObject(ObjectType.Element, "选择一个管道");
            Pipe pipe = doc.GetElement(reference) as Pipe;
            DebugUtils.ExtendPipe(pipe);
            return Result.Succeeded;
        }
    }
    public static class DebugUtils
    {
        public static void ExtendPipe(Pipe p)
        {
            Document doc = p.Document;
            UIDocument uidoc = new UIDocument(doc);
            Selection sel = uidoc.Selection;

            XYZ point = sel.PickPoint();

            LocationCurve lc = p.Location as LocationCurve;

            Line l = lc.Curve as Line;

            XYZ endpoint1 = null;

            if (l.GetEndPoint(0).DistanceTo(point) < l.GetEndPoint(1).DistanceTo(point))
            {
                endpoint1 = l.GetEndPoint(1);
            }
            else
            {
                endpoint1 = l.GetEndPoint(0);
            }

            Transaction ts = new Transaction(doc, "延长管线");
            ts.Start();
            lc.Curve = Line.CreateBound(endpoint1, point);
            ts.Commit();
        }
    }


}
