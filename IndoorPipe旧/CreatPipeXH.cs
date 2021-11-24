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
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.InteropServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class CreatPipeXH : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains("给排水") && ps.Name.Contains("焊接钢管"))
                {
                    pt = ps;
                    break;
                }
            }

            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = col.ToElements();
            PipingSystemType pipesys = null;
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;
                if (ps.Name.Contains("给排水") && ps.Name.Contains("循环回水"))
                {
                    pipesys = ps;
                    break;
                }
            }

            using (Transaction ts = new Transaction(doc, "创建管道"))
            {
                ts.Start();
                Pipe p = Pipe.Create(doc, pipesys.Id, pt.Id, doc.ActiveView.GenLevel.Id, new XYZ(0, 0, 0), new XYZ(0.1, 0, 0));

                IList<ElementId> list = new List<ElementId>();
                list.Add(p.Id);
                uidoc.Selection.SetElementIds(list);

                RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.CreateSimilar);
                uiApp.PostCommand(cmdId);

                ts.Commit();
                return Result.Succeeded;
            }
        }
    }
}
