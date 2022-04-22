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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Cmd_DimPipe : IExternalCommand //标注管道长度
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var acview = uidoc.ActiveView;

            var pipe = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Pipe)).GetElement(doc) as Pipe;

            var geoele = pipe.get_Geometry(new Options()
            { DetailLevel = ViewDetailLevel.Fine, ComputeReferences = true });

            var line = pipe.LocationLine();

            var refs = GetEndPlanRefs(geoele);

            doc.Invoke(m =>
            {
                doc.Create.NewDimension(acview, line, refs);

            }, "创建管道长度标注");

            return Result.Succeeded;
        }

        private ReferenceArray GetEndPlanRefs(GeometryElement geoele)
        {
            var result = new ReferenceArray();

            var geometrys = geoele.Cast<GeometryObject>().ToList();

            foreach (GeometryObject geo in geometrys)
            {
                if (geo is Solid so)
                {
                    var faces = so.Faces;
                    foreach (var face in faces)
                    {
                        if (face is PlanarFace pface)
                        {
                            result.Append(pface.Reference);
                        }
                    }
                }
                else
                    continue;
            }

            return result;
        }
    }


}
