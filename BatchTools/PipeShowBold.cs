using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class PipeShowBold : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(View)).OfCategory(BuiltInCategory.OST_Views);
            IList<Element> views = viewCollector.ToElements();
            try
            {
                using (Transaction ts = new Transaction(doc, "给排水平剖面整理"))
                {
                    ts.Start();
                    foreach (View view in views)
                    {
                        if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("给排水"))
                        {
                            SetPipeShowBold(view);
                        }
                    }
                    foreach (View view in views)
                    {
                        if (view.ViewType == ViewType.Section && view.Name.Contains("给排水"))
                        {
                            SetPipeShowBold(view);
                        }

                    }
                    ts.Commit();
                }
                return Result.Succeeded;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetPipeShowBold(View view)
        {
            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_Rebar));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
            categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
            view.SetCategoryHidden(categories.ElementAt(0), true);
            view.SetCategoryHidden(categories.ElementAt(1), false);
            view.SetCategoryHidden(categories.ElementAt(2), false);
            view.SetCategoryHidden(categories.ElementAt(3), false);
            view.SetCategoryHidden(categories.ElementAt(4), false);

            OverrideGraphicSettings org5 = new OverrideGraphicSettings();
            org5.SetProjectionLineWeight(5);
            view.SetCategoryOverrides(categories.ElementAt(1), org5);
            view.SetCategoryOverrides(categories.ElementAt(2), org5);
            OverrideGraphicSettings org1 = new OverrideGraphicSettings();
            org1.SetProjectionLineWeight(1);
            view.SetCategoryOverrides(categories.ElementAt(3), org1);
            view.SetCategoryOverrides(categories.ElementAt(4), org1);
        }

    }
}
