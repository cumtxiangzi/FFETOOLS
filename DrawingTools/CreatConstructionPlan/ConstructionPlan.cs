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
                    List<View> filterViews = new List<View>();
                    List<ViewSheet> waterViewsheets = new List<ViewSheet>();

                    ViewSheet activeSheet = null;
                    foreach (var item in views)
                    {
                        if (item.ViewType == ViewType.FloorPlan || item.ViewType == ViewType.Section || item.ViewType == ViewType.ThreeD
                            || item.ViewType == ViewType.DraftingView || item.ViewType == ViewType.Detail || item.ViewType == ViewType.Schedule)
                        {
                            if (item.IsTemplate == false && (item.Name.Contains("����ˮ") || item.Name.Contains("���˵��") || item.Name.Contains("WL")))
                            {
                                filterViews.Add(item);
                            }
                        }
                    }

                    for (int i = 0; i < drawings.Count; i++)
                    {
                        ViewSheet drawingSheet = null;
                        View viewOnDrawing = null;

                        foreach (var item in viewsheets)
                        {
                            if (item.Title.Contains(drawings[i]))
                            {
                                drawingSheet = item;
                                if (drawingSheet.Title.Contains("WD"))
                                {
                                    waterViewsheets.Add(drawingSheet);
                                }
                                break;
                            }
                        }

                        if (plans[i].Count != 0)
                        {
                            foreach (var plan in plans[i])
                            {
                                foreach (var view in filterViews)
                                {
                                    if (view.ViewType == ViewType.Schedule && view.Name.Replace("����ˮ_", "") == plan)
                                    {
                                        viewOnDrawing = view;
                                        ScheduleSheetInstance.Create(doc, drawingSheet.Id, viewOnDrawing.Id, GetPoint(drawingSheet));
                                    }

                                    if (view.ViewType == ViewType.DraftingView && view.ViewName == plan && (plan.Contains("���˵��") || plan.Contains("WL")))
                                    {
                                        viewOnDrawing = view;
                                        if (plan.Contains("���˵��"))
                                        {
                                            XYZ designNotePoint = new XYZ(GetPoint(drawingSheet).X-200/304.8, GetPoint(drawingSheet).Y-200/304.8, 0);
                                            CreateViewport(doc, drawingSheet, viewOnDrawing.Id, designNotePoint, GetViewPortType(doc));
                                        }
                                        else
                                        {
                                            CreateViewport(doc, drawingSheet, viewOnDrawing.Id, GetPoint(drawingSheet), GetViewPortType(doc));
                                        }
                                    }

                                    if (view.ViewType == ViewType.FloorPlan && view.ViewName == plan && view.ViewType != ViewType.Schedule)
                                    {
                                        viewOnDrawing = view;
                                        string name = view.LookupParameter("ͼֽ�ϵı���").AsString();
                                        int nameLength = name.Length;
                                        CreateViewport(doc, drawingSheet, viewOnDrawing.Id, GetPoint(drawingSheet), GetViewPortType(doc, nameLength));
                                    }

                                    if (view.ViewType != ViewType.FloorPlan && view.Title.Contains(plan) && view.ViewType != ViewType.Schedule)
                                    {
                                        viewOnDrawing = view;
                                        string name = view.LookupParameter("ͼֽ�ϵı���").AsString();
                                        int nameLength = name.Length;
                                        CreateViewport(doc, drawingSheet, viewOnDrawing.Id, GetPoint(drawingSheet), GetViewPortType(doc, nameLength));
                                    }

                                }

                            }
                        }
                    }
                    activeSheet = waterViewsheets[0];

                    trans.Commit();
                    uidoc.ActiveView = activeSheet;
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
        private Viewport CreateViewport(Document document, ViewSheet viewSheet, ElementId viewID, XYZ point, ElementType viewPortType)
        {
            Viewport viewport = null;
            bool isok = Viewport.CanAddViewToSheet(document, viewSheet.Id, viewID);
            if (isok)
            {
                //ͼֽ������ͼ
                viewport = Viewport.Create(document, viewSheet.Id, viewID, point);
                viewport.ChangeTypeId(viewPortType.Id);
            }
            return viewport;
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
        public ElementType GetViewPortType(Document doc, int length) //δ�������������������
        {
            ElementType type = null;
            IList<ElementType> types = CollectorHelper.TCollector<ElementType>(doc);
            //List<ElementType> filterTypes = new List<ElementType>();
            int titleLength = 60;

            if (length > 6 && length < 10)
            {
                titleLength = 80;
            }
            if (length > 10)
            {
                titleLength = 100;
            }

            foreach (var item in types)
            {
                if (item.FamilyName == "�ӿ�" && item.Name.Contains("����ˮ") && item.Name.Contains(titleLength.ToString()))
                {
                    type = item;
                    break;
                }
            }

            return type;
        }
        public ElementType GetViewPortType(Document doc) //�ޱ�������
        {
            ElementType type = null;
            IList<ElementType> types = CollectorHelper.TCollector<ElementType>(doc);
            //List<ElementType> filterTypes = new List<ElementType>();

            foreach (var item in types)
            {
                if (item.FamilyName == "�ӿ�" && item.Name.Contains("����") && item.Name.Contains("�ޱ���"))
                {
                    type = item;
                    break;
                }
            }

            return type;
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
                        systemNameList.Add(view.Name.Replace("����ˮ_", ""));
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
                if (view.ViewType == ViewType.DraftingView && (view.Name.Contains("����ˮ") || view.Name.Contains("���˵��") || view.Name.Contains("WL")))
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
                                scheduleNameList.Add(view.Name.Replace("����ˮ_", ""));
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
