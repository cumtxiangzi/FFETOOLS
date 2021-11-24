using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(View)).OfCategory(BuiltInCategory.OST_Views);
            IList<Element> views = viewCollector.ToElements();

            UserMajor majForm = new UserMajor();
            majForm.ShowDialog();

            try
            {
                switch (majForm.MajorName)
                {
                    case "工艺":
                        using (Transaction trans = new Transaction(doc, "隐藏外专业工作集"))
                        {
                            trans.Start();
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("工艺"))
                                {
                                    SetP2DViewWorksetVisibility(view, doc);
                                }
                            }
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.Section && view.Name.Contains("工艺"))
                                {
                                    SetP2DViewWorksetVisibility(view, doc);
                                }

                            }
                            trans.Commit();
                        }
                        break;

                    case "给排水":
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
                        break;

                    case "暖通":
                        using (Transaction trans = new Transaction(doc, "隐藏外专业工作集"))
                        {
                            trans.Start();
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("暖通"))
                                {
                                    SetV2DViewWorksetVisibility(view, doc);
                                }
                            }
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.Section && view.Name.Contains("暖通"))
                                {
                                    SetV2DViewWorksetVisibility(view, doc);
                                }

                            }
                            trans.Commit();
                        }
                        break;

                    case "电气":
                        using (Transaction trans = new Transaction(doc, "隐藏外专业工作集"))
                        {
                            trans.Start();
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("电气"))
                                {
                                    SetE2DViewWorksetVisibility(view, doc);
                                }
                            }
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.Section && view.Name.Contains("电气"))
                                {
                                    SetE2DViewWorksetVisibility(view, doc);
                                }

                            }
                            trans.Commit();
                        }
                        break;

                    case "结构":
                        using (Transaction trans = new Transaction(doc, "隐藏外专业工作集"))
                        {
                            trans.Start();
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("结构"))
                                {
                                    SetS2DViewWorksetVisibility(view, doc);
                                }
                            }
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.Section && view.Name.Contains("结构"))
                                {
                                    SetS2DViewWorksetVisibility(view, doc);
                                }

                            }
                            trans.Commit();
                        }
                        break;

                    case "建筑":
                        using (Transaction trans = new Transaction(doc, "隐藏外专业工作集"))
                        {
                            trans.Start();
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("建筑"))
                                {
                                    SetB2DViewWorksetVisibility(view, doc);
                                }
                            }
                            foreach (View view in views)
                            {
                                if (view.ViewType == ViewType.Section && view.Name.Contains("建筑"))
                                {
                                    SetB2DViewWorksetVisibility(view, doc);
                                }

                            }
                            trans.Commit();
                        }
                        break;

                    default:
                        break;
                }

                return Result.Succeeded;

            }
            catch (Exception)
            {
                TaskDialog.Show("警告", "批量隐藏外专业工作集失败");
                return Result.Failed;
            }
        }
        
        public void SetW2DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (sets.Name.Contains("暖通") || sets.Name.Contains("电气") || sets.Name.Contains("工艺_非标") || sets.Name.Contains("工艺_压缩空气管道") || sets.Name.Contains("工艺_罗茨风机管道") || sets.Name.Contains("工艺_灭火装置") || sets.Name.Contains("结构_钢筋"))
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Hidden);
                }
                else
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Visible);
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

        public void SetV2DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (sets.Name.Contains("给排水") || sets.Name.Contains("电气"))
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Hidden);
                }
                else
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Visible);
                }
            }
        }

        public void SetE2DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (sets.Name.Contains("给排水") || sets.Name.Contains("暖通"))
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Hidden);
                }
                else
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Visible);
                }
            }
        }

        public void SetS2DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (sets.Name.Contains("给排水") || sets.Name.Contains("暖通") || sets.Name.Contains("电气"))
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Hidden);
                }
                else
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Visible);
                }
            }
        }

        public void SetB2DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (sets.Name.Contains("给排水") || sets.Name.Contains("暖通") || sets.Name.Contains("电气"))
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Hidden);
                }
                else
                {
                    view.SetWorksetVisibility(sets.Id, WorksetVisibility.Visible);
                }
            }
        }

        public void SetP2DViewWorksetVisibility(View view, Document doc)
        {
            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (sets.Name.Contains("给排水") || sets.Name.Contains("暖通") || sets.Name.Contains("电气"))
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
