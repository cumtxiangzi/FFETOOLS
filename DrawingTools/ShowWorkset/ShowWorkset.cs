using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Interop;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class ShowWorkset : IExternalCommand
    {
        public static UserMajor mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new UserMajor(new ExecuteEventShowWorkset().GetWorkSetName(doc));
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
    class ExecuteEventShowWorkset : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
                viewCollector.OfClass(typeof(View)).OfCategory(BuiltInCategory.OST_Views);
                IList<Element> views = viewCollector.ToElements();

                using (Transaction trans = new Transaction(doc, "隐藏外专业工作集"))
                {
                    trans.Start();
                    foreach (View view in views)
                    {
                        if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("给排水"))
                        {
                            SetW2DViewWorksetVisibility(view, doc);
                        }
                    }
                    foreach (View view in views)
                    {
                        if (view.ViewType == ViewType.Section && view.Name.Contains("给排水"))
                        {
                            SetW2DViewWorksetVisibility(view, doc);
                        }
                    }
                    foreach (View view in views)
                    {
                        if (view.ViewType == ViewType.ThreeD && view.Name.Contains("给排水"))
                        {
                            SetW3DViewWorksetVisibility(view, doc);
                        }
                    }
                    trans.Commit();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "外专业工作集隐藏";
        }
        public List<string> GetWorkSetName(Document doc)
        {
            List<string> workSetNameList = new List<string>();
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();

            foreach (Workset sets in worksets)
            {
                if (!(sets.Name.Contains("给排水") || sets.Name.Contains("建筑") || sets.Name.Contains("工作集") || sets.Name.Contains("共享标高") || sets.Name.Contains("轴网")))
                {
                    workSetNameList.Add(sets.Name);
                }
            }
            workSetNameList.Sort();
            return workSetNameList;
        }
        public void SetW2DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                foreach (var item in ShowWorkset.mainfrm.SelectWorkSetNameList)
                {
                    if (sets.Name == item)
                    {
                        view.SetWorksetVisibility(sets.Id, WorksetVisibility.Hidden);
                    }
                }
            }
        }
        public void SetW3DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (!(sets.Name.Contains("给排水")))
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Hidden);
                }
                else
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Visible);
                }
            }
        }
    }
}
