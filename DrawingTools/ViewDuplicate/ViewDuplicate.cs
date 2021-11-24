using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Architecture;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class ViewDuplicate : IExternalCommand
    {
        public static ViewDuplicateForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                mainfrm = new ViewDuplicateForm(new ExecuteEventViewDuplicat().GetArcDrawingName(doc));
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
            }
            return Result.Succeeded;
        }
    }
    public class ExecuteEventViewDuplicat : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = uidoc.Document;

                FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
                viewCollector.OfClass(typeof(ViewPlan)).OfCategory(BuiltInCategory.OST_Views);

                IList<Element> views = viewCollector.ToElements();
                ViewSet arcViews = new ViewSet();
                ViewSet plumbViews = new ViewSet();
                ViewSet newViews = new ViewSet();
                ViewSet withArcViews = new ViewSet();
                ViewSet noArcViews = new ViewSet();

                foreach (ViewPlan view in views)
                {
                    foreach (var item in ViewDuplicate.mainfrm.SelectDrawingNameList)
                    {
                        if (view.ViewType == ViewType.FloorPlan && view.Name == item && view.IsTemplate == false)
                        {
                            arcViews.Insert(view);
                        }
                    }
                }

                foreach (ViewPlan item in arcViews)
                {
                    if (!(item.Name.Contains("建筑")))
                    {
                        noArcViews.Insert(item);
                    }
                }

                foreach (ViewPlan item in arcViews)
                {
                    if (item.Name.Contains("建筑"))
                    {
                        withArcViews.Insert(item);
                    }
                }

                foreach (ViewPlan view in views)
                {
                    if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("给排水"))
                    {
                        plumbViews.Insert(view);
                    }
                }

                foreach (ViewPlan arcview in withArcViews)
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
                    var failure = new ViewDuplicateFailureHandler();
                    //ViewDuplicateFailureHandler.SetFailedHandlerBeforeTransaction(failure, trans);
                    ViewPlan viewCopy = null;
                    trans.Start();
                    foreach (ViewPlan view in newViews)
                    {
                        viewCopy = CreateViewCopy(view, doc);
                        if (ViewDuplicate.mainfrm.DetailCheckBox.IsChecked == false)
                        {
                            ElementTransformUtils.CopyElements(view, GetDimension(doc, view), viewCopy, Transform.Identity, new CopyPasteOptions());
                        }
                    }

                    foreach (ViewPlan view in noArcViews)
                    {
                        viewCopy = CreateViewCopy(view, doc);
                        if (ViewDuplicate.mainfrm.DetailCheckBox.IsChecked == false)
                        {
                            ElementTransformUtils.CopyElements(view, GetDimension(doc, view), viewCopy, Transform.Identity, new CopyPasteOptions());
                        }
                    }
                    trans.Commit();
                    uidoc.ActiveView = viewCopy;
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }

        }
        public string GetName()
        {
            return "建筑图批量复制";
        }
        public ViewPlan CreateViewCopy(ViewPlan view, Document doc)
        {
            ViewPlan viewCopy = null;
            ElementId newViewId = ElementId.InvalidElementId;
            if (view.CanViewBeDuplicated(ViewDuplicateOption.Duplicate) || view.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing))
            {
                if (ViewDuplicate.mainfrm.DetailCheckBox.IsChecked == true)
                {
                    newViewId = view.Duplicate(ViewDuplicateOption.WithDetailing);
                }
                else
                {
                    newViewId = view.Duplicate(ViewDuplicateOption.Duplicate);
                }

                viewCopy = view.Document.GetElement(newViewId) as ViewPlan;

                if (view.Name.Contains("建筑"))
                {
                    viewCopy.Name = view.Name.Replace("建筑", "给排水");
                }
                if (!(view.Name.Contains("建筑")))
                {
                    viewCopy.Name = "给排水" + "_" + view.Name;
                }

                viewCopy.LookupParameter("图纸上的标题").Set((viewCopy.Name.Replace("给排水", "")).Replace("_", ""));
                string title = viewCopy.LookupParameter("图纸上的标题").AsString();
                if (!(title.Contains("平面")))
                {
                    viewCopy.LookupParameter("图纸上的标题").Set(title + "平面图");
                }
                if (title.Contains("平面") && (!(title.Contains("图"))))
                {
                    viewCopy.LookupParameter("图纸上的标题").Set(title + "图");
                }

                viewCopy.ViewTemplateId = new ElementId(-1);
                viewCopy.LookupParameter("子规程").Set("给排水");
                viewCopy.Discipline = ViewDiscipline.Mechanical;
                viewCopy.DetailLevel = ViewDetailLevel.Fine;
                SetW2DViewWorksetVisibility(viewCopy, doc);

                List<ElementId> categories = new List<ElementId>();
                categories.Add(new ElementId(BuiltInCategory.OST_Rebar));
                categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
                categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
                categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
                categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
                categories.Add(new ElementId(BuiltInCategory.OST_Floors));
                categories.Add(new ElementId(BuiltInCategory.OST_Walls));
                viewCopy.SetCategoryHidden(categories.ElementAt(0), true);
                viewCopy.SetCategoryHidden(categories.ElementAt(1), false);
                viewCopy.SetCategoryHidden(categories.ElementAt(2), false);
                viewCopy.SetCategoryHidden(categories.ElementAt(3), false);
                viewCopy.SetCategoryHidden(categories.ElementAt(4), false);

                OverrideGraphicSettings orgFloor = new OverrideGraphicSettings();
                orgFloor.SetCutFillPatternVisible(false);
                orgFloor.SetProjectionFillPatternVisible(false);
                viewCopy.SetCategoryOverrides(categories.ElementAt(5), orgFloor);
                viewCopy.SetCategoryOverrides(categories.ElementAt(6), orgFloor);

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
        public void SetW2DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (sets.Name.Contains("给排水"))
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Visible);
                    break;
                }
            }
        }
        public List<string> GetArcDrawingName(Document doc)
        {
            List<string> arcDrawingNameList = new List<string>();
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(ViewPlan)).OfCategory(BuiltInCategory.OST_Views);
            IList<Element> views = viewCollector.ToElements();

            foreach (ViewPlan view in views)
            {
                if ((view.ViewType == ViewType.FloorPlan && view.Name.Contains("建筑")) || (view.ViewType == ViewType.FloorPlan && view.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).AsValueString().Contains("建筑")))
                {
                    if (view.IsTemplate == false)
                    {
                        arcDrawingNameList.Add(view.Name);
                    }
                }
            }
            arcDrawingNameList.Sort();
            return arcDrawingNameList;
        }
        public IList<ElementId> GetDimension(Document doc, View view)
        {
            List<ElementId> list = new List<ElementId>();

            FilteredElementCollector detailLineCollector = new FilteredElementCollector(doc, view.Id);
            detailLineCollector.WherePasses(new CurveElementFilter(CurveElementType.DetailCurve));
            IList<Element> detailLines = detailLineCollector.ToElements();
            foreach (var item in detailLines)
            {
                list.Add(item.Id);
            }

            FilteredElementCollector spotCollector = new FilteredElementCollector(doc, view.Id);
            spotCollector.OfClass(typeof(SpotDimension)).OfCategory(BuiltInCategory.OST_SpotElevations);
            IList<Element> spots = spotCollector.ToElements();
            foreach (SpotDimension item in spots)
            {
                list.Add(item.Id);
            }

            FilteredElementCollector textCollector = new FilteredElementCollector(doc, view.Id);
            textCollector.OfClass(typeof(TextNote)).OfCategory(BuiltInCategory.OST_TextNotes);
            IList<Element> texts = textCollector.ToElements();
            foreach (TextNote item in texts)
            {
                list.Add(item.Id);
            }

            FilteredElementCollector starisPathCollector = new FilteredElementCollector(doc, view.Id);
            starisPathCollector.OfClass(typeof(StairsPath)).OfCategory(BuiltInCategory.OST_StairsPaths);
            IList<Element> starisPaths = starisPathCollector.ToElements();
            foreach (StairsPath item in starisPaths)
            {
                list.Add(item.Id);
            }

            FilteredElementCollector roomTagCollector = new FilteredElementCollector(doc, view.Id);
            roomTagCollector.WherePasses(new RoomTagFilter());
            IList<Element> roomTags = roomTagCollector.ToElements();
            foreach (RoomTag item in roomTags)
            {
                list.Add(item.Id);
            }

            FilteredElementCollector dimensionCollector = new FilteredElementCollector(doc, view.Id);
            dimensionCollector.OfClass(typeof(Dimension)).OfCategory(BuiltInCategory.OST_Dimensions);
            IList<Element> dimensions = dimensionCollector.ToElements();
            foreach (Dimension item in dimensions)
            {
                int num = 0;
                ReferenceArray refList = item.References;
                foreach (Reference reference in refList)
                {
                    Grid grid = doc.GetElement(reference.ElementId) as Grid;
                    if (!(grid == null))
                    {
                        num = num + 1;
                    }
                }
                if (num == refList.Size)
                {
                    list.Add(item.Id);
                }
            }

            return list;
        }
    }
}
