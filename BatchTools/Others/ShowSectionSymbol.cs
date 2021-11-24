using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class ShowSectionSymbol : IExternalCommand
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
                using (Transaction trans = new Transaction(doc, "剖面显示与隐藏"))
                {
                    trans.Start();
                    View v = uidoc.ActiveView;
                    if ((v.ViewType == ViewType.FloorPlan) || (v.ViewType == ViewType.Section))
                    {
                        List<ElementId> categories = new List<ElementId>();
                        categories.Add(new ElementId(BuiltInCategory.OST_Sections));
                        bool visualable= v.GetCategoryHidden(categories.ElementAt(0));
                        if (visualable==true)
                        {
                            v.SetCategoryHidden(categories.ElementAt(0), false);
                        }
                        else
                        {
                            v.SetCategoryHidden(categories.ElementAt(0), true);
                        }                     
                    }
                    else
                    {
                        TaskDialog.Show("警告","请在平面或剖面视图中操作");
                    }
                    trans.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
