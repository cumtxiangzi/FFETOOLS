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
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class ConstructionPlan : IExternalCommand
    {
        public static ConstructionPlanForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                ExecuteEventConstructionPlan plan = new ExecuteEventConstructionPlan();
                mainfrm = new ConstructionPlanForm(plan.GetPlanName(doc), plan.GetDrawingName(doc), plan.GetSectionName(doc), plan.GetSystemViewName(doc),
                    plan.GetDetailName(doc), plan.GetDraftingName(doc), plan.GetScheduleName(doc));

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
    public class ExecuteEventConstructionPlan : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                List<string> drawings = ConstructionPlan.mainfrm.AllDrawings;
                List<List<string>> plans = ConstructionPlan.mainfrm.AllPlans;

                List<ViewSheet> viewSheet = new List<ViewSheet>();
                List<ViewPlan> viewPlan = new List<ViewPlan>();

                using (Transaction trans = new Transaction(doc, "����ʩ��ͼ"))
                {
                    trans.Start();

                    IList<View> views = CollectorHelper.TCollector<View>(doc);
                    IList<ViewSheet> viewsheets = CollectorHelper.TCollector<ViewSheet>(doc);
                    List<View> filterViews=new List<View>();

                    for (int i = 0; i < drawings.Count; i++)
                    {
                        ViewSheet drawingSheet = null;
                        View viewOnDrawing = null;

                        foreach (var item in viewsheets)
                        {
                            if (item.Title.Contains(drawings[i]))
                            {
                                drawingSheet = item;
                                break;
                            }
                        }

                        if (plans[i].Count != 0)
                        {
                            foreach (var plan in plans[i])
                            {
                                foreach (var view in views)
                                {
                                    if (view.Title.Contains(plan))
                                    {
                                        viewOnDrawing = view;
                                        CreateViewport(doc, drawingSheet, viewOnDrawing.Id, GetPoint(drawingSheet));
                                    }

                                }
                            }
                        }



                        //MessageBox.Show(drawings[i]);



                    }


                    foreach (View view in views)
                    {
                       
                    }
                    //foreach (ViewPlan view in views)
                    //{
                    //    if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("����ˮ"))
                    //    {
                    //        if (view.IsTemplate == false && !(view.LookupParameter("ͼֽ���").AsString().Contains("WD")))
                    //        {
                    //            planNameList.Add(view.Name);
                    //        }
                    //    }
                    //}




                    //string drawname = null;
                    //string planname = null;
                    //foreach (string drawing in drawings)
                    //{
                    //    drawname += drawing;
                    //    drawname += "\n";
                    //}

                    //foreach (var plan in plans)
                    //{

                    //    foreach (string plan1 in plan)
                    //    {
                    //        planname += plan1;
                    //        planname += "\n";
                    //    }

                    //}
                    //MessageBox.Show(drawname + "\n" + planname);


                    trans.Commit();
                }
            }
            catch (Exception)
            {

            }
        }
        public string GetName()
        {
            return "����ʩ��ͼ";
        }

        /// <summary>
        /// ͼֽ�м�����ͼ
        /// </summary>
        /// <param name="viewSheet"></param>
        /// <param name="viewID"></param>
        /// <param name="point"></param>
        private void CreateViewport(Document document, ViewSheet viewSheet, ElementId viewID, XYZ point)
        {
            bool isok = Viewport.CanAddViewToSheet(document, viewSheet.Id, viewID);
            if (isok)
            {
                //ͼֽ������ͼ
                Viewport.Create(document, viewSheet.Id, viewID, point);
            }
        }
        /// <summary>
        /// �õ�ͼ�����ĵ�
        /// ��������������ͼ������ĵ㣬�������ݿ�����ĵ���Ҫ����ָ����ͼ������ȥ�Ӽ��������ҵı߾�
        /// </summary>
        /// <param name="viewSheet">ͼֽ</param>
        /// <returns></returns>
        private XYZ GetPoint(ViewSheet viewSheet)
        {
            UV loc = new UV((viewSheet.Outline.Max.U + viewSheet.Outline.Min.U) / 2, (viewSheet.Outline.Max.V + viewSheet.Outline.Min.V) / 2);
            XYZ point = new XYZ(loc.U, loc.V, 0);
            return point;
        }

        public List<string> GetPlanName(Document doc)//ƽ����ͼ
        {
            List<string> planNameList = new List<string>();
            IList<ViewPlan> views = CollectorHelper.TCollector<ViewPlan>(doc);

            foreach (ViewPlan view in views)
            {
                if (view.ViewType == ViewType.FloorPlan && view.Name.Contains("����ˮ"))
                {
                    if (view.IsTemplate == false && !(view.LookupParameter("ͼֽ���").AsString().Contains("WD")))
                    {
                        planNameList.Add(view.Name);
                    }
                }
            }
            planNameList.Sort();
            return planNameList;
        }
        public List<string> GetSectionName(Document doc)//������ͼ
        {
            List<string> sectionNameList = new List<string>();
            IList<ViewSection> views = CollectorHelper.TCollector<ViewSection>(doc);

            foreach (ViewSection view in views)
            {
                if (view.ViewType == ViewType.Section && view.Name.Contains("����ˮ"))
                {
                    if (view.IsTemplate == false && !(view.LookupParameter("ͼֽ���").AsString().Contains("WD")))
                    {
                        sectionNameList.Add(view.Name);
                    }
                }
            }
            sectionNameList.Sort();
            return sectionNameList;
        }
        public List<string> GetSystemViewName(Document doc)//��ά��ͼ
        {
            List<string> systemNameList = new List<string>();
            IList<View3D> views = CollectorHelper.TCollector<View3D>(doc);

            foreach (View3D view in views)
            {
                if (view.ViewType == ViewType.ThreeD && view.Name.Contains("����ˮ"))
                {
                    if (view.IsTemplate == false && !(view.LookupParameter("ͼֽ���").AsString().Contains("WD")))
                    {
                        systemNameList.Add(view.Name.Replace("����ˮ", ""));
                    }
                }
            }
            systemNameList.Sort();
            return systemNameList;
        }
        public List<string> GetDraftingName(Document doc)//������ͼ
        {
            List<string> draftingNameList = new List<string>();
            IList<ViewDrafting> views = CollectorHelper.TCollector<ViewDrafting>(doc);

            foreach (ViewDrafting view in views)
            {
                if (view.ViewType == ViewType.DraftingView && view.Name.Contains("����ˮ"))
                {
                    if (view.IsTemplate == false && !(view.LookupParameter("ͼֽ���").AsString().Contains("WD")))
                    {
                        draftingNameList.Add(view.Name);
                    }
                }
            }
            draftingNameList.Sort();
            return draftingNameList;
        }
        public List<string> GetDetailName(Document doc)//��ͼ��ͼ
        {
            List<string> sectionNameList = new List<string>();
            IList<ViewSection> views = CollectorHelper.TCollector<ViewSection>(doc);

            foreach (ViewSection view in views)
            {
                if (view.ViewType == ViewType.Detail && view.Name.Contains("����ˮ"))
                {
                    if (view.IsTemplate == false && !(view.LookupParameter("ͼֽ���").AsString().Contains("WD")))
                    {
                        sectionNameList.Add(view.Name);
                    }
                }
            }
            sectionNameList.Sort();
            return sectionNameList;
        }
        public List<string> GetScheduleName(Document doc)//��ϸ��
        {
            List<string> scheduleNameList = new List<string>();
            IList<ViewSchedule> views = CollectorHelper.TCollector<ViewSchedule>(doc);

            List<string> pipesysName = new List<string>();
            IList<Pipe> allPipes = CollectorHelper.TCollector<Pipe>(doc);
            foreach (Pipe pipe in allPipes)
            {
                if (pipe.Name.Contains("����ˮ"))
                {
                    pipesysName.Add(pipe.LookupParameter("ϵͳ����").AsValueString());
                }
            }
            List<string> sysList = pipesysName.Distinct().ToList();

            foreach (ViewSchedule view in views)
            {
                if (view.ViewType == ViewType.Schedule && view.Name.Contains("����ˮ"))
                {
                    if (view.IsTemplate == false)
                    {
                        foreach (string item in sysList)
                        {
                            if (view.Name.Contains(item.Replace("�ܵ�ϵͳ", "")))
                            {
                                scheduleNameList.Add(view.Name.Replace("����ˮ", ""));
                            }
                        }
                    }
                }
            }
            scheduleNameList.Sort();
            return scheduleNameList;
        }
        public List<string> GetDrawingName(Document doc)//ͼֽ
        {
            List<string> drawingNameList = new List<string>();
            IList<ViewSheet> views = CollectorHelper.TCollector<ViewSheet>(doc);

            foreach (ViewSheet view in views)
            {
                if (view.ViewType == ViewType.DrawingSheet && view.SheetNumber.Contains("W"))
                {
                    if (view.IsTemplate == false)
                    {
                        ICollection<ElementId> allDrawings = view.GetAllViewports();
                        if (!(allDrawings.Count > 0))
                        {
                            drawingNameList.Add(view.Title.Replace("ͼֽ:", ""));
                        }
                    }
                }
            }
            drawingNameList.Sort();
            return drawingNameList;
        }

    }

}
