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
    class ViewDuplicate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(ViewPlan)).OfCategory(BuiltInCategory.OST_Views);
            IList<Element> views = viewCollector.ToElements();
            ViewSet arcViews = new ViewSet();
            ViewSet plumbViews = new ViewSet();
            ViewSet newViews = new ViewSet();
            try
            {
                foreach (ViewPlan view in views)
                {
                    if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("建筑"))
                    {
                        arcViews.Insert(view);
                    }
                }

                foreach (ViewPlan view in views)
                {
                    if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("给排水"))
                    {
                        plumbViews.Insert(view);
                    }
                }

                foreach (ViewPlan arcview in arcViews)
                {
                    List<string> plbViewName = new List<string>();
                    foreach (ViewPlan plbview in plumbViews)
                    {
                        plbViewName.Add(plbview.Name.Replace("给排水", "建筑"));
                    }
                    if (!(plbViewName.Contains(arcview.Name)))
                    {
                        newViews.Insert(arcview);
                    }
                }

                using (Transaction trans = new Transaction(doc, "批量复制建筑视图"))
                {
                    trans.Start();                   
                    foreach (ViewPlan view in newViews)
                    {
                        if (message.Length==0)
                        {
                            CreateViewCopy(view);
                        }
                        else
                        {
                            message = "";
                            continue;  
                        }
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

        public ViewPlan CreateViewCopy(ViewPlan view)
        {
            ViewPlan viewCopy = null;
            ElementId newViewId = ElementId.InvalidElementId;
            if (view.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing))
            {
                newViewId = view.Duplicate(ViewDuplicateOption.WithDetailing);
                viewCopy = view.Document.GetElement(newViewId) as ViewPlan;              
                viewCopy.Name = view.Name.Replace("建筑", "给排水");
                viewCopy.LookupParameter("图纸上的标题").Set((viewCopy.Name.Replace("给排水", "")).Replace("_",""));
                string title = viewCopy.LookupParameter("图纸上的标题").AsString();
                if (!(title.Contains("平面")))
                {
                    viewCopy.LookupParameter("图纸上的标题").Set(title+"平面图");
                }
                if (title.Contains("平面")&&(!(title.Contains("图"))))
                {
                    viewCopy.LookupParameter("图纸上的标题").Set(title + "图");
                }

                viewCopy.ViewTemplateId = new ElementId(-1);
                viewCopy.LookupParameter("子规程").Set("给排水");
                viewCopy.Discipline = ViewDiscipline.Mechanical;
                viewCopy.DetailLevel = ViewDetailLevel.Fine;

                List<ElementId> categories = new List<ElementId>();
                categories.Add(new ElementId(BuiltInCategory.OST_Rebar));
                categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
                categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
                categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
                categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
                viewCopy.SetCategoryHidden(categories.ElementAt(0), true);
                viewCopy.SetCategoryHidden(categories.ElementAt(1), false);
                viewCopy.SetCategoryHidden(categories.ElementAt(2), false);
                viewCopy.SetCategoryHidden(categories.ElementAt(3), false);
                viewCopy.SetCategoryHidden(categories.ElementAt(4), false);

                OverrideGraphicSettings org5 = new OverrideGraphicSettings();
                org5.SetProjectionLineWeight(5);
                viewCopy.SetCategoryOverrides(categories.ElementAt(1), org5);
                viewCopy.SetCategoryOverrides(categories.ElementAt(2), org5);
                OverrideGraphicSettings org1 = new OverrideGraphicSettings();
                org1.SetProjectionLineWeight(1);
                viewCopy.SetCategoryOverrides(categories.ElementAt(3), org1);
                viewCopy.SetCategoryOverrides(categories.ElementAt(4), org1);

            }
            return viewCopy;
        }
        
    }
}
