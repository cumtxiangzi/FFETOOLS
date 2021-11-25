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
        public int note;
        public View3D view3D;
        public List<View3D> view3DList = new List<View3D>();
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = uidoc.Document;

                if (CreatPipeSystem.mainfrm.XJChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "循环给水", view3DList);
                }
                if (CreatPipeSystem.mainfrm.XHChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "循环回水", view3DList);
                }
                if (CreatPipeSystem.mainfrm.JChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "生活给水", view3DList);
                }
                if (CreatPipeSystem.mainfrm.WChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "污水", view3DList);
                }
                if (CreatPipeSystem.mainfrm.RJChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "生活热水", view3DList);
                }
                if (CreatPipeSystem.mainfrm.XFChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "消防给水", view3DList);
                }
                if (CreatPipeSystem.mainfrm.QTChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "气体灭火", view3DList);
                }
                if (CreatPipeSystem.mainfrm.ZPChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "自喷灭火", view3DList);
                }
                if (CreatPipeSystem.mainfrm.HNChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "混凝剂", view3DList);
                }
                if (CreatPipeSystem.mainfrm.XDChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "消毒剂", view3DList);
                }
                if (CreatPipeSystem.mainfrm.WDChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "稳定剂", view3DList);
                }
                if (CreatPipeSystem.mainfrm.YJChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "水源输水", view3DList);
                }
                if (CreatPipeSystem.mainfrm.ZSChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "中水", view3DList);
                }
                if (CreatPipeSystem.mainfrm.FChkBox.IsChecked == true)
                {
                    CompoundOperation(doc, uidoc, "废水", view3DList);
                }

                uidoc.ActiveView = view3D;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public string GetName()
        {
            return "管道系统图批量生成";
        }
        public void CompoundOperation(Document doc, UIDocument uidoc, string pipeSystemName, List<View3D> view3DList)
        {
            // 所有TransactionGroup要用“using”来创建来保证它的正确结束 
            using (TransactionGroup transGroup = new TransactionGroup(doc, "批量生成系统图"))
            {
                if (transGroup.Start() == TransactionStatus.Started)
                {
                    // 我们打算调用两个函数，每个都有一个独立的事务 
                    // 我们打算这个组合操作要么成功，要么失败 
                    // 只要其中有一个失败，我们就撤销所有操作 
                    if (CreatPipeSystems(doc, uidoc, pipeSystemName) && CreatPipeNotes(doc, uidoc, view3DList))
                    {
                        // Assimilate函数会将这两个事务合并成一个，并只显示TransactionGroup的名
                        // 在Undo菜单里 
                        transGroup.Assimilate();
                    }
                    else
                    {
                        // 如果有一个操作失败了，我们撤销在这个事务组里的所有操作 
                        transGroup.RollBack();
                    }
                }
            }
        }
        public bool CreatPipeSystems(Document doc, UIDocument uidoc, string pipeSystemName)
        {
            using (Transaction trans = new Transaction(doc, "生成系统图"))
            {
                if (TransactionStatus.Started == trans.Start())
                {
                    XYZ ot = new XYZ();
                    if (CreatPipeSystem.mainfrm.SouthEastButton.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (CreatPipeSystem.mainfrm.NorthWestButton.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }

                    if (pipeSystemName == "循环给水")
                    {
                        //int num = SystemNum(doc, "循环给水");
                        int num = 3;
                        List<string> systemCode = SystemCode(doc, "循环给水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_循环给水管道系统图" + (i + 1).ToString(), "循环给水管道系统图",
                                                 "给排水_循环给水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "循环回水")
                    {
                        int num = SystemNum(doc, "循环回水");
                        List<string> systemCode = SystemCode(doc, "循环回水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_循环回水管道系统图" + (i + 1).ToString(), "循环回水管道系统图",
                                                  "给排水_循环回水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "生活给水")
                    {
                        int num = SystemNum(doc, "生活给水");
                        List<string> systemCode = SystemCode(doc, "生活给水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_生活给水管道系统图" + (i + 1).ToString(), "给水管道系统图",
                                                  "给排水_生给水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "污水")
                    {
                        int num = SystemNum(doc, "污水");
                        List<string> systemCode = SystemCode(doc, "污水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_排水管道系统图" + (i + 1).ToString(), "排水管道系统图",
                                                  "给排水_排水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "生活热水")
                    {
                        int num = SystemNum(doc, "热水给水");
                        List<string> systemCode = SystemCode(doc, "热水给水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_热水给水管道系统图" + (i + 1).ToString(), "热水给水管道系统图",
                                                  "给排水_热水给水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "消防给水")
                    {
                        int num = SystemNum(doc, "消防给水");
                        List<string> systemCode = SystemCode(doc, "消防给水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_消防给水管道系统图" + (i + 1).ToString(), "消防给水管道系统图",
                                                  "给排水_消防给水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "气体灭火")
                    {
                        int num = SystemNum(doc, "气体灭火");
                        List<string> systemCode = SystemCode(doc, "气体灭火");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_气体灭火管道系统图" + (i + 1).ToString(), "气体灭火管道系统图",
                                                  "给排水_气体灭火管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "自喷灭火")
                    {
                        int num = SystemNum(doc, "自喷灭火");
                        List<string> systemCode = SystemCode(doc, "自喷灭火");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_自喷灭火管道系统图" + (i + 1).ToString(), "自喷灭火管道系统图",
                                                  "给排水_自喷灭火管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "混凝剂")
                    {
                        int num = SystemNum(doc, "混凝剂");
                        List<string> systemCode = SystemCode(doc, "混凝剂");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_混凝剂管道系统图" + (i + 1).ToString(), "混凝剂管道系统图",
                                                  "给排水_混凝剂管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "消毒剂")
                    {
                        int num = SystemNum(doc, "消毒剂");
                        List<string> systemCode = SystemCode(doc, "消毒剂");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_消毒剂管道系统图" + (i + 1).ToString(), "消毒剂管道系统图",
                                                  "给排水_消毒剂管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "稳定剂")
                    {
                        int num = SystemNum(doc, "稳定剂");
                        List<string> systemCode = SystemCode(doc, "稳定剂");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_稳定剂管道系统图" + (i + 1).ToString(), "稳定剂管道系统图",
                                                  "给排水_稳定剂管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "水源输水")
                    {
                        int num = SystemNum(doc, "水源输水");
                        List<string> systemCode = SystemCode(doc, "水源输水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_水源输水管道系统图" + (i + 1).ToString(), "水源输水管道系统图",
                                                  "给排水_水源输水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "中水")
                    {
                        int num = SystemNum(doc, "中水");
                        List<string> systemCode = SystemCode(doc, "中水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_中水管道系统图" + (i + 1).ToString(), "中水管道系统图",
                                                  "给排水_中水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }
                    if (pipeSystemName == "废水")
                    {
                        //int num = SystemNum(doc, "废水");
                        int num = 8;
                        List<string> systemCode = SystemCode(doc, "废水");
                        for (int i = 0; i < num; i++)
                        {
                            CreatPipeSystemMethod(doc, "给排水_废水管道系统图" + (i + 1).ToString(), "废水管道系统图",
                                                  "给排水_废水管道系统(非)", systemCode.ElementAt(i), ot);
                            view3DList.Add(view3D);
                        }
                    }

                    if (TransactionStatus.Committed == trans.Commit())
                    {
                        //uidoc.ActiveView = view3D;
                        return true;
                    }
                    // 如果失败，撤销这个事务 
                    trans.RollBack();
                }
            }
            return false;
        }
        public bool CreatPipeNotes(Document doc, UIDocument uidoc, List<View3D> view3DList)
        {
            using (Transaction trans = new Transaction(doc, "创建管道标注"))
            {
                if (TransactionStatus.Started == trans.Start())
                {
                    IList<Element> pipeTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                    FamilySymbol pipeDNtag = null;
                    foreach (Element tag in pipeTagsCollect)
                    {
                        FamilySymbol pipeTag = tag as FamilySymbol;
                        if (pipeTag.Name.Contains("管道公称直径") && pipeTag.Name.Contains("给排水"))
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
                            pipeCollector.OfClass(typeof(Pipe));
                            IList<Element> pipes = pipeCollector.ToElements();
                            foreach (Pipe pipe in pipes)
                            {
                                double pipeLength = pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();

                                if ((pipeLength * 304.83) >= 500)
                                {
                                    Reference pipeRef = new Reference(pipe);
                                    TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;//类别标记
                                    TagOrientation tagOri = TagOrientation.Horizontal;

                                    //在管道中部添加管径标注
                                    LocationCurve locCurve = pipe.Location as LocationCurve;
                                    XYZ pipeMid = locCurve.Curve.Evaluate(0.5, true);
                                    IndependentTag tag = IndependentTag.Create(doc, item.Id, pipeRef, false, tageMode, tagOri, pipeMid);
                                    tag.ChangeTypeId(pipeDNtag.Id);
                                }
                            }
                        }
                    }
                    view3DList.Clear();

                    if (TransactionStatus.Committed == trans.Commit())
                    {
                        return true;
                    }
                    // 如果失败，撤销这个事务 
                    trans.RollBack();
                }
            }
            return false;
        }
        public void CreatPipeSystemMethod(Document doc, string systemName, string drawingName, string filterName,
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

            view3D = View3D.CreateIsometric(doc, eid);
            view3D.DisplayStyle = DisplayStyle.HLR;
            view3D.DetailLevel = ViewDetailLevel.Fine;
            view3D.OrientTo(orientation);
            view3D.CropBox = BoundSize(doc, view3D, systemShortName);

            view3D.Name = systemName;
            view3D.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).Set(drawingName);//设置图纸上的标题
            view3D.SaveOrientationAndLock();
            view3D.Scale = 50;
            view3D.Discipline = ViewDiscipline.Mechanical;
            view3D.LookupParameter("子规程").Set("给排水");
            view3D.CropBoxActive = true;
            view3D.CropBoxVisible = true;
            view3D.get_Parameter(BuiltInParameter.VIEWER_ANNOTATION_CROP_ACTIVE).Set(1);//设置注释裁剪
            view3D.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION).Set(1);//设置裁剪视图

            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_GenericModel));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
            categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
            view3D.SetCategoryHidden(categories.ElementAt(0), false);
            view3D.SetCategoryHidden(categories.ElementAt(1), false);
            view3D.SetCategoryHidden(categories.ElementAt(2), false);
            view3D.SetCategoryHidden(categories.ElementAt(3), false);
            view3D.SetCategoryHidden(categories.ElementAt(4), false);

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
                if (!(sets.Name.Contains("给排水")))
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
                ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Comments", category);
                parameterFilterElement.Name = filterName;

                FilteredElementCollector parameterCollector = new FilteredElementCollector(doc);
                Parameter parameter = parameterCollector.OfClass(typeof(Pipe)).FirstElement().get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM);

                List<FilterRule> filterRules = new List<FilterRule>();
                filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(parameter.Id, Regex.Replace(systemShortName, @"\d", ""), true));
                parameterFilterElement.SetRules(filterRules);

                OverrideGraphicSettings filterSettings = new OverrideGraphicSettings();
                view3D.SetFilterOverrides(parameterFilterElement.Id, filterSettings);
                view3D.SetFilterVisibility(parameterFilterElement.Id, false);
            }
            else
            {
                view3D.SetFilterVisibility(p.Id, false);
            }
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
                //元素Element的BoundingBox是模型（世界）坐标系，三维视图3DView的CropBox及BoundingBox是视图坐标系
                //判断坐标是不是模型（世界）坐标系，看transform的origin属性是否是（0，0，0）
                //使用view3D.CropBox.Transform.Inverse将点的世界坐标 转换成视图坐标；
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
                if (systemNameAll.Contains(systemName) && systemNameAll.Contains("给排水"))
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

