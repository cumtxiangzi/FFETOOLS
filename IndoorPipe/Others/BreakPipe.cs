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
    public class BreakPipe : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;
                BreakPipeMethod(doc, sel);             
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                
            }
            return Result.Succeeded;
        }
        public void BreakPipeMethod(Document doc, Selection sel)
        {
            using (Transaction trans = new Transaction(doc, "管道打断"))
            {
                trans.Start();
                var eleref = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is MEPCurve), "拾取管线打断点");
                var pickpoint = eleref.GlobalPoint;
                var mep = eleref.GetElement(doc) as MEPCurve;
                var locationline = (mep.Location as LocationCurve).Curve as Line;
                PlumbingUtils.BreakCurve(doc, mep.Id, pickpoint);
                trans.Commit();
            }
            BreakPipeMethod(doc, sel);
        }
    }
}
