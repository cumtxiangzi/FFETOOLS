using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Interop;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Architecture;
using System.Threading;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatPipeSystem : IExternalCommand
    {
        public static CreatPipeSystemForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                mainfrm = new CreatPipeSystemForm();
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
    public class ExecuteEventCreatPipeSystem : IExternalEventHandler
    {
        //public int note;
        //public View3D view3D;
        public List<View3D> view3DList = new List<View3D>();
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = uidoc.Document;

                TransactionGroup tg = new TransactionGroup(doc, "�����ܵ�ϵͳͼ");
                tg.Start();

                CreatPipeSystems(doc, uidoc);

                if (CreatPipeSystem.mainfrm.NotePipeChkBox.IsChecked == true)
                {
                    double notePipeLength = Convert.ToDouble(CreatPipeSystem.mainfrm.NoteLengthTxt.Text);
                    CreatPipeNotes(doc, uidoc, view3DList, notePipeLength);
                }

                tg.Assimilate();

                MessageBox.Show("ϵͳͼ�������", "��Ϣ", MessageBoxButton.OK, MessageBoxImage.Information);
                uidoc.ActiveView = view3DList.FirstOrDefault();
                view3DList.Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public string GetName()
        {
            return "�ܵ�ϵͳͼ��������";
        }
        public void CreatPipeSystems(Document doc, UIDocument uidoc)
        {
            using (Transaction trans = new Transaction(doc, "����ϵͳͼ"))
            {
                int quality = Convert.ToInt32(CreatPipeSystem.mainfrm.QuantityTxt.Text);
                trans.Start();

                XYZ ot = new XYZ();
                if (CreatPipeSystem.mainfrm.SouthEastButton.IsChecked == true)
                {
                    ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                }
                if (CreatPipeSystem.mainfrm.NorthWestButton.IsChecked == true)
                {
                    ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                }

                if (CreatPipeSystem.mainfrm.XJChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "ѭ����ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "ѭ����ˮ");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_ѭ����ˮ�ܵ�ϵͳͼ" + i.ToString(), "ѭ����ˮ�ܵ�ϵͳͼ",
                                               "����ˮ_ѭ����ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }

                if (CreatPipeSystem.mainfrm.XHChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "ѭ����ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "ѭ����ˮ");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_ѭ����ˮ�ܵ�ϵͳͼ" + i.ToString(), "ѭ����ˮ�ܵ�ϵͳͼ",
                                                  "����ˮ_ѭ����ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.JChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "�����ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "�����ˮ");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_�����ˮ�ܵ�ϵͳͼ" + i.ToString(), "��ˮ�ܵ�ϵͳͼ",
                                                  "����ˮ_����ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.WChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "��ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "��ˮ");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_��ˮ�ܵ�ϵͳͼ" + i.ToString(), "��ˮ�ܵ�ϵͳͼ",
                                                  "����ˮ_��ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.RJChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "��ˮ��ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "��ˮ��ˮ");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_��ˮ��ˮ�ܵ�ϵͳͼ" + i.ToString(), "��ˮ��ˮ�ܵ�ϵͳͼ",
                                                  "����ˮ_��ˮ��ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.XFChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "������ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "������ˮ");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_������ˮ�ܵ�ϵͳͼ" + i.ToString(), "������ˮ�ܵ�ϵͳͼ",
                                                  "����ˮ_������ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.QTChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "�������");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "�������");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_�������ܵ�ϵͳͼ" + i.ToString(), "�������ܵ�ϵͳͼ",
                                                  "����ˮ_�������ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.ZPChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "�������");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "�������");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_�������ܵ�ϵͳͼ" + i.ToString(), "�������ܵ�ϵͳͼ",
                                                  "����ˮ_�������ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.HNChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "������");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "������");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_�������ܵ�ϵͳͼ" + i.ToString(), "�������ܵ�ϵͳͼ",
                                                  "����ˮ_�������ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.XDChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "������");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "������");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_�������ܵ�ϵͳͼ" + i.ToString(), "�������ܵ�ϵͳͼ",
                                                  "����ˮ_�������ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.WDChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "�ȶ���");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "�ȶ���");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_�ȶ����ܵ�ϵͳͼ" + i.ToString(), "�ȶ����ܵ�ϵͳͼ",
                                                  "����ˮ_�ȶ����ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.YJChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "ˮԴ��ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "ˮԴ��ˮ");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_ˮԴ��ˮ�ܵ�ϵͳͼ" + i.ToString(), "ˮԴ��ˮ�ܵ�ϵͳͼ",
                                                  "����ˮ_ˮԴ��ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.ZSChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "��ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "��ˮ");
                    for (int i = 1; i <= num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_��ˮ�ܵ�ϵͳͼ" + i.ToString(), "��ˮ�ܵ�ϵͳͼ",
                                                  "����ˮ_��ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }
                if (CreatPipeSystem.mainfrm.FChkBox.IsChecked == true)
                {
                    //int num = SystemNum(doc, "��ˮ");
                    int num = quality;
                    List<string> systemCode = SystemCode(doc, "��ˮ");
                    for (int i = 0; i < num; i++)
                    {
                        View3D view3D = CreatPipeSystemMethod(doc, "����ˮ_��ˮ�ܵ�ϵͳͼ" + i.ToString(), "��ˮ�ܵ�ϵͳͼ",
                                                  "����ˮ_��ˮ�ܵ�ϵͳ(��)", systemCode.ElementAt(0), ot);
                        view3DList.Add(view3D);
                    }
                }

                trans.Commit();
            }
        }
        public void CreatPipeNotes(Document doc, UIDocument uidoc, List<View3D> view3DList, double notePipeLength)
        {
            using (Transaction trans = new Transaction(doc, "�����ܵ���ע"))
            {
                trans.Start();

                IList<Element> pipeTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                FamilySymbol pipeDNtag = null;
                foreach (Element tag in pipeTagsCollect)
                {
                    FamilySymbol pipeTag = tag as FamilySymbol;
                    if (pipeTag.Name.Contains("�ܵ�����ֱ��") && pipeTag.Name.Contains("����ˮ"))
                    {
                        pipeDNtag = pipeTag;
                        break;
                    }
                }

                foreach (View3D item in view3DList)
                {
                    if (item.IsTemplate == false)
                    {
                        FilteredElementCollector pipeCollector = new FilteredElementCollector(doc, item.Id);
                        pipeCollector.OfClass(typeof(Pipe)).OfCategory(BuiltInCategory.OST_PipeCurves);
                        IList<Element> pipes = pipeCollector.ToElements();
                        foreach (Pipe pipe in pipes)
                        {
                            double pipeLength = pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();

                            if ((pipeLength * 304.83) >= notePipeLength)
                            {
                                Reference pipeRef = new Reference(pipe);
                                TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;//�����
                                TagOrientation tagOri = TagOrientation.Horizontal;

                                //XYZ pipeMid = new XYZ();
                                //List<Task> tasks = new List<Task>();
                                //Thread thread = new Thread(() => {

                                //});
                                //thread.Start();

                                //�ڹܵ��в���ӹܾ���ע
                                LocationCurve locCurve = pipe.Location as LocationCurve;
                                XYZ pipeMid = locCurve.Curve.Evaluate(0.5, true);
                                IndependentTag tag = IndependentTag.Create(doc, item.Id, pipeRef, false, tageMode, tagOri, pipeMid);
                                tag.ChangeTypeId(pipeDNtag.Id);
                            }
                        }
                    }
                }
                //view3DList.Clear();


                trans.Commit();
            }

        }
        public View3D CreatPipeSystemMethod(Document doc, string systemName, string drawingName, string filterName,
                                          string systemShortName, XYZ orientation)
        {
            ElementId eid = new ElementId(-1);
            IList<Element> elems = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).ToElements();
            foreach (Element e in elems)
            {
                ViewFamilyType v = e as ViewFamilyType;
                if (v != null && v.ViewFamily == ViewFamily.ThreeDimensional)
                {
                    eid = e.Id;
                    break;
                }
            }

            View3D view3D = View3D.CreateIsometric(doc, eid);
            view3D.DisplayStyle = DisplayStyle.HLR;
            view3D.DetailLevel = ViewDetailLevel.Fine;
            view3D.OrientTo(orientation);
            //view3D.CropBox = BoundSize(doc, view3D, systemShortName);//�ü���ͼ

            view3D.Name = systemName;
            view3D.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).Set(drawingName);//����ͼֽ�ϵı���
            view3D.SaveOrientationAndLock();
            view3D.Scale = 50;
            view3D.Discipline = ViewDiscipline.Mechanical;
            view3D.LookupParameter("�ӹ��").Set("����ˮ");
            view3D.CropBoxActive = true;
            view3D.CropBoxVisible = true;
            view3D.get_Parameter(BuiltInParameter.VIEWER_ANNOTATION_CROP_ACTIVE).Set(1);//����ע�Ͳü�
            view3D.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION).Set(1);//���òü���ͼ

            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_GenericModel));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
            categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
            categories.Add(new ElementId(BuiltInCategory.OST_PlaceHolderPipes));

            view3D.SetCategoryHidden(categories.ElementAt(0), false);
            view3D.SetCategoryHidden(categories.ElementAt(1), false);
            view3D.SetCategoryHidden(categories.ElementAt(2), false);
            view3D.SetCategoryHidden(categories.ElementAt(3), false);
            view3D.SetCategoryHidden(categories.ElementAt(4), false);
            view3D.SetCategoryHidden(categories.ElementAt(5), true);

            OverrideGraphicSettings org = new OverrideGraphicSettings();
            org.SetProjectionLineWeight(1);
            view3D.SetCategoryOverrides(categories.ElementAt(1), org);
            view3D.SetCategoryOverrides(categories.ElementAt(2), org);
            view3D.SetCategoryOverrides(categories.ElementAt(3), org);
            view3D.SetCategoryOverrides(categories.ElementAt(4), org);

            FilteredWorksetCollector collector = new FilteredWorksetCollector(doc);
            collector.OfKind(WorksetKind.UserWorkset);
            IList<Workset> worksets = collector.ToWorksets();
            foreach (Workset sets in worksets)
            {
                if (!(sets.Name.Contains("����ˮ")))
                {
                    view3D.SetWorksetVisibility(sets.Id, WorksetVisibility.Hidden);
                }
                else
                {
                    view3D.SetWorksetVisibility(sets.Id, WorksetVisibility.Visible);
                }
            }

            IList<Element> filters = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement)).ToElements();
            ParameterFilterElement p = null;

            int note = 0;
            foreach (Element e in filters)
            {
                if (e.Name.Contains(filterName))
                {
                    note = 1;
                    p = e as ParameterFilterElement;
                    break;
                }
            }
            if (!(note == 1))
            {
                List<ElementId> category = new List<ElementId>();
                category.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
                category.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
                category.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
                category.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
                category.Add(new ElementId(BuiltInCategory.OST_PlumbingFixtures));
                ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Comments", category);
                parameterFilterElement.Name = filterName;

                FilteredElementCollector parameterCollector = new FilteredElementCollector(doc);
                Parameter parameter = parameterCollector.OfClass(typeof(Pipe)).FirstElement().get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM);

                List<FilterRule> filterRules = new List<FilterRule>();
                if (CreatPipeSystem.mainfrm.JChkBox.IsChecked == true)
                {
                    filterRules.Add(ParameterFilterRuleFactory.CreateNotBeginsWithRule(parameter.Id, Regex.Replace(systemShortName, @"\d", ""), true));
                }
                else
                {
                    filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(parameter.Id, Regex.Replace(systemShortName, @"\d", ""), true));
                }
                parameterFilterElement.SetRules(filterRules);

                OverrideGraphicSettings filterSettings = new OverrideGraphicSettings();
                view3D.SetFilterOverrides(parameterFilterElement.Id, filterSettings);
                view3D.SetFilterVisibility(parameterFilterElement.Id, false);
            }
            else
            {
                view3D.SetFilterVisibility(p.Id, false);
            }
            return view3D;
        }
        public BoundingBoxXYZ BoundSize(Document doc, View3D view3d, string systemShortName)
        {
            BoundingBoxXYZ bs = new BoundingBoxXYZ();
            FilteredElementCollector col = new FilteredElementCollector(doc, view3d.Id);
            col.OfClass(typeof(PipingSystem));
            IList<Element> pipesys = col.ToElements();
            foreach (PipingSystem item in pipesys)
            {
                if (item.Name == systemShortName)
                {
                    bs = item.get_BoundingBox(view3d);
                    break;
                }
            }
            if (CreatPipeSystem.mainfrm.SouthEastButton.IsChecked == true)
            {
                //Ԫ��Element��BoundingBox��ģ�ͣ����磩����ϵ����ά��ͼ3DView��CropBox��BoundingBox����ͼ����ϵ
                //�ж������ǲ���ģ�ͣ����磩����ϵ����transform��origin�����Ƿ��ǣ�0��0��0��
                //ʹ��view3D.CropBox.Transform.Inverse������������� ת������ͼ���ꣻ
                Transform transfIn = view3d.CropBox.Transform.Inverse;

                XYZ pt1 = bs.Max;
                XYZ pt2 = bs.Min;

                XYZ pt3 = new XYZ(pt2.X, pt1.Y, pt1.Z);
                XYZ pt4 = new XYZ(pt1.X, pt2.Y, pt2.Z);

                XYZ transpt1 = transfIn.OfPoint(pt1);
                XYZ transpt2 = transfIn.OfPoint(pt2);
                XYZ transpt3 = transfIn.OfPoint(pt3);
                XYZ transpt4 = transfIn.OfPoint(pt4);

                XYZ ptMax = new XYZ(transpt1.X, transpt3.Y, 0);
                XYZ ptMin = new XYZ(transpt2.X, transpt4.Y, 0);

                BoundingBoxXYZ box = new BoundingBoxXYZ();
                box.Max = ptMax;
                box.Min = ptMin;

                return box;
            }
            else
            {
                Transform transfIn = view3d.CropBox.Transform.Inverse;

                XYZ pt11 = bs.Max;
                XYZ pt22 = bs.Min;

                XYZ pt1 = new XYZ(pt11.X, pt11.Y, pt22.Z);
                XYZ pt2 = new XYZ(pt22.X, pt22.Y, pt11.Z);
                XYZ pt3 = new XYZ(pt11.X, pt22.Y, pt11.Z);
                XYZ pt4 = new XYZ(pt22.X, pt11.Y, pt22.Z);

                XYZ transpt1 = transfIn.OfPoint(pt1);
                XYZ transpt2 = transfIn.OfPoint(pt2);
                XYZ transpt3 = transfIn.OfPoint(pt3);
                XYZ transpt4 = transfIn.OfPoint(pt4);

                XYZ ptMax = new XYZ(transpt2.X, transpt3.Y, 0);
                XYZ ptMin = new XYZ(transpt1.X, transpt4.Y, 0);

                BoundingBoxXYZ box = new BoundingBoxXYZ();
                box.Max = ptMax;
                box.Min = ptMin;

                return box;
            }
        }
        public int SystemNum(Document doc, string systemName)
        {
            int num = 0;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(PipingSystem));
            IList<Element> pipesystems = col.ToElements();
            foreach (Element e in pipesystems)
            {
                PipingSystem ps = e as PipingSystem;
                string systemNameAll = ps.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString();
                if (systemNameAll.Contains(systemName))
                {
                    num = num + 1;
                }
            }
            return num;
        }
        public List<string> SystemCode(Document doc, string systemName)
        {
            List<string> systemCode = new List<string>();
            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(PipingSystem));
            IList<Element> pipesystems = col.ToElements();
            foreach (Element e in pipesystems)
            {
                PipingSystem ps = e as PipingSystem;
                string systemNameAll = ps.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString();
                if (systemNameAll.Contains(systemName) && systemNameAll.Contains("����ˮ"))
                {
                    systemCode.Add(ps.Name);
                }
            }
            return systemCode;
        }
        public bool ContainNote(Document doc, View3D view3d)
        {
            FilteredElementCollector pipeNoteCollector = new FilteredElementCollector(doc, view3d.Id);
            IList<Element> pipeNotes = pipeNoteCollector.OfClass(typeof(IndependentTag)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
            if (pipeNotes.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}

