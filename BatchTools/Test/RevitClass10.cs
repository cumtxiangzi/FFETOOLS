using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;


/// <summary>
/// Ò»¼üÂ¥°å
/// </summary>
namespace FFETOOLS
{
    /// <summary>
    /// Ò»¼üÂ¥°å
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_CreateFloorQukly : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            var acview = doc.ActiveView;
            var beamrefs = sel.PickObjects(ObjectType.Element,
                doc.GetSelectionFilter(m => m.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming),
                "Ñ¡ÔñÉú³É°åµÄÁº");
            var beams = beamrefs.Select(m => m.GetElement(doc));
            Transaction temtran = new Transaction(doc, "temtran");
            temtran.Start();
            foreach (Element beam in beams)
            {
                var joinedelements = JoinGeometryUtils.GetJoinedElements(doc, beam);
                if (joinedelements.Count > 0)
                {
                    foreach (var id in joinedelements)
                    {
                        var temele = id.GetElement(doc);
                        var isjoined = JoinGeometryUtils.AreElementsJoined(doc, beam, temele);
                        if (isjoined)
                        {
                            JoinGeometryUtils.UnjoinGeometry(doc, beam, temele);
                        }
                    }
                }
            }
            temtran.RollBack();
            var solidss = new List<GeometryObject>();
            foreach (var element in beams)
            {
                solidss.AddRange(element.Getsolids());
            }
            var joinedsolid = MergeSolids(solidss.Cast<Solid>().ToList());
            var upfaces = joinedsolid.Getupfaces();
            var edgeArrays = upfaces.First().EdgeLoops.Cast<EdgeArray>().ToList();
            var curveloops = upfaces.First().GetEdgesAsCurveLoops();
            var orderedcurveloops = curveloops.OrderBy(m => m.GetExactLength()).ToList();
            orderedcurveloops.RemoveAt(orderedcurveloops.Count - 1);
            curveloops = orderedcurveloops;
            var curvearrays = curveloops.Select(m => m.ToCurveArray());
            doc.Invoke(m =>
            {
                foreach (var curvearray in curvearrays)
                {
                    doc.Create.NewFloor(curvearray, false);
                }
            }, "Ò»¼üÂ¥°å");
            return Result.Succeeded;
        }
        public Solid MergeSolids(Solid solid1, Solid solid2)
        {
            var result = default(Solid);
            try
            {
                result = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
            }
            catch (Exception e)
            {
                result = null;
            }
            return result;
        }
        public Solid MergeSolids(List<Solid> solids)
        {
            var result = default(Solid);
            foreach (var solid in solids)
            {
                if (result == null)
                {
                    result = solid;
                }
                else
                {
                    var temsolid = MergeSolids(result, solid);
                    if (temsolid == null) continue;
                    result = temsolid;
                }
            }
            return result;
        }
    }
    
}

