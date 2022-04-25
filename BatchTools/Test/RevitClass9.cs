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
    public class CreatSlab : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                var floor = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Floor)).GetElement(doc) as Floor;
         
                using (Transaction trans = new Transaction(doc, "name"))
                {
                    trans.Start();

                    GetSlabEdge(doc,floor);

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        public void GetSlabEdge(Document doc, Floor bottomFloor)//创建散水
        {
            SlabEdgeType footSlab = null;
            IList<SlabEdgeType> edges = CollectorHelper.TCollector<SlabEdgeType>(doc);
            foreach (var item in edges)
            {
                if (item.Name.Contains("建筑_散水无垫层_1000"))
                {
                    footSlab = item;
                    break;
                }
            }

            Face normalFace = null;
            Options options = new Options();
            options.ComputeReferences = true;
            GeometryElement geometryElement = bottomFloor.get_Geometry(options);
            foreach (GeometryObject item in geometryElement)
            {
                if (item is Solid solid)
                {
                    List<Face> list = new List<Face>();
                    foreach (Face face in solid.Faces)
                    {
                        list.Add(face);
                    }
                    double AreaMax = list.Max(t => t.Area);
                    normalFace = list.FirstOrDefault(p => p.Area == AreaMax);
                }
            }

            EdgeArrayArray eaa = normalFace.EdgeLoops;
            EdgeArray ea = eaa.get_Item(0);

            //List<Line> profile = new List<Line>();
            //foreach (Edge e in ea)
            //{
            //    IList<XYZ> pts = e.Tessellate();
            //    int m = pts.Count;
            //    XYZ p = pts[0];
            //    XYZ q = pts[m - 1];
            //    Line line = Line.CreateBound(p, q);
            //    profile.Add(line);

            //}

            ReferenceArray refArr = new ReferenceArray();
            foreach (Edge item in ea)
            {
                refArr.Append(item.Reference);
            }

            doc.Create.NewSlabEdge(footSlab, refArr);

        }
    }
}
