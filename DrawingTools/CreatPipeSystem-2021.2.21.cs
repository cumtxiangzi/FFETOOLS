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
    class CreatPipeSystem2 : IExternalCommand
    {
        public int note { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;
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

                CreatPipeSystemForm crsForm = new CreatPipeSystemForm();
                crsForm.ShowDialog();


                if (crsForm.WChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);


                    int num = systemNum(doc, "W");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_排水管道系统图" + (i + 1).ToString(), "排水管道系统图", "给排水_污水管道系统(非)", "W", i + 1, ot);
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CreatPipeSystems(Document doc, UIDocument uidoc, UIApplication uiapp, ElementId eid, string sysname, string drawingname, string filtername, string sysabbr, int i, XYZ orientation)
        {

            using (Transaction ts = new Transaction(doc, "管道系统图"))
            {

                if (TransactionStatus.Started == ts.Start())
                {

                    View3D view3D = View3D.CreateIsometric(doc, eid);

                    view3D.DisplayStyle = DisplayStyle.HLR;
                    view3D.DetailLevel = ViewDetailLevel.Fine;
                    view3D.OrientTo(orientation);
                    view3D.Name = sysname;
                    view3D.LookupParameter("图纸上的标题").Set(drawingname);
                    view3D.SaveOrientationAndLock();
                    view3D.Scale = 50;
                    view3D.Discipline = ViewDiscipline.Mechanical;
                    view3D.LookupParameter("子规程").Set("给排水");
                    view3D.CropBoxActive = true;
                    view3D.CropBoxVisible = true;
                    view3D.LookupParameter("注释裁剪").Set(1);

                    //元素Element的BoundingBox是模型（世界）坐标系，三维视图3DView的CropBox及BoundingBox是视图坐标系
                    //判断坐标是不是模型（世界）坐标系，看transform的origin属性是否是（0，0，0）
                    //使用view3D.CropBox.Transform.Inverse将点的世界坐标 转换成视图坐标；
                    BoundingBoxXYZ pipesysBound = BoundSize(doc, sysabbr, i, view3D);
                    Transform transfIn = view3D.CropBox.Transform.Inverse;

                    XYZ pt1 = pipesysBound.Max;
                    XYZ pt2 = pipesysBound.Min;
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
                    view3D.CropBox = box;



                    List<ElementId> categories1 = new List<ElementId>();
                    categories1.Add(new ElementId(BuiltInCategory.OST_GenericModel));
                    categories1.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
                    categories1.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
                    categories1.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
                    categories1.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
                    view3D.SetCategoryHidden(categories1.ElementAt(0), false);
                    view3D.SetCategoryHidden(categories1.ElementAt(1), false);
                    view3D.SetCategoryHidden(categories1.ElementAt(2), false);
                    view3D.SetCategoryHidden(categories1.ElementAt(3), false);
                    view3D.SetCategoryHidden(categories1.ElementAt(4), false);

                    OverrideGraphicSettings org1 = new OverrideGraphicSettings();
                    org1.SetProjectionLineWeight(1);
                    view3D.SetCategoryOverrides(categories1.ElementAt(1), org1);
                    view3D.SetCategoryOverrides(categories1.ElementAt(2), org1);
                    view3D.SetCategoryOverrides(categories1.ElementAt(3), org1);
                    view3D.SetCategoryOverrides(categories1.ElementAt(4), org1);

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
                        if (e.Name.Contains(filtername))
                        {
                            note = 1;
                            p = e as ParameterFilterElement;
                            break;
                        }
                    }
                    if (!(note == 1))
                    {
                        List<ElementId> categories = new List<ElementId>();
                        categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
                        categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
                        categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
                        categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
                        ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Comments", categories);
                        parameterFilterElement.Name = filtername;

                        FilteredElementCollector parameterCollector = new FilteredElementCollector(doc);
                        Parameter parameter = parameterCollector.OfClass(typeof(Pipe)).FirstElement().get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM);

                        List<FilterRule> filterRules = new List<FilterRule>();
                        filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(parameter.Id, sysabbr, true));
                        parameterFilterElement.SetRules(filterRules);

                        OverrideGraphicSettings filterSettings = new OverrideGraphicSettings();
                        view3D.SetFilterOverrides(parameterFilterElement.Id, filterSettings);
                        view3D.SetFilterVisibility(parameterFilterElement.Id, false);
                    }
                    else
                    {
                        view3D.SetFilterVisibility(p.Id, false);
                    }

                    if (TransactionStatus.Committed == ts.Commit())
                    {
                        uidoc.ActiveView = view3D;
                        return true;
                    }

                }
                ts.RollBack();

            }
            return false;

        }

        public bool CreatPipeNotes(Document doc, UIDocument uidoc)
        {
            using (Transaction ts = new Transaction(doc, "管道系统图标注"))
            {
                if (TransactionStatus.Started == ts.Start())
                {
                    IList<Element> pipetagscollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                    FamilySymbol pipeDNtag = null;
                    foreach (Element tag in pipetagscollect)
                    {
                        FamilySymbol pipetag = tag as FamilySymbol;
                        if (pipetag.Name.Contains("管道公称直径") && pipetag.Name.Contains("给排水"))
                        {
                            pipeDNtag = pipetag;
                            break;
                        }
                    }


                    FilteredElementCollector pipeCollector = new FilteredElementCollector(doc);
                    pipeCollector.OfClass(typeof(Pipe));
                    IList<Element> pipes = pipeCollector.ToElements();
                    foreach (Element pipe in pipes)
                    {

                        double pipeLength = pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();

                        if ((pipeLength * 304.83) >= 500)
                        {
                            Reference pipeRef = new Reference(pipe);
                            TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                            TagOrientation tagOri = TagOrientation.Horizontal;
                            //Add the tag to the middle of duct
                            LocationCurve locCurve = pipe.Location as LocationCurve;
                            XYZ pipeMid = locCurve.Curve.Evaluate(0.5, true);
                            IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, pipeMid);
                            tag.ChangeTypeId(pipeDNtag.Id);
                        }

                    }
                    if (TransactionStatus.Committed == ts.Commit())
                    {
                        return true;
                    }

                }
                ts.RollBack();

            }
            return false;

        }

        public void CompoundOperation(Document doc, UIDocument uidoc, UIApplication uiapp, ElementId eid, String sysname, string drawingname, string filtername, string sysabbr, int i, XYZ orientation)
        {
            using (TransactionGroup transGroup = new TransactionGroup(doc, "创建管道系统"))
            {
                if (transGroup.Start() == TransactionStatus.Started)
                {
                    if (CreatPipeSystems(doc, uidoc, uiapp, eid, sysname, drawingname, filtername, sysabbr, i, orientation) && CreatPipeNotes(doc, uidoc))
                    {

                        transGroup.Assimilate();

                    }
                    else
                    {
                        transGroup.RollBack();
                    }

                }

            }

        }

        public int systemNum(Document doc, string sysName)
        {
            int num = new int();
            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(PipingSystem));
            IList<Element> pipesystems = col.ToElements();
            foreach (Element e in pipesystems)
            {
                PipingSystem ps = e as PipingSystem;
                if (ps.Name.Contains(sysName))
                {
                    num = num + 1;
                }
            }
            return num;
        }

        BoundingBoxXYZ BoundSize(Document doc, string sysName, int i, View3D view)
        {
            BoundingBoxXYZ bs = new BoundingBoxXYZ();
            FilteredElementCollector col = new FilteredElementCollector(doc, view.Id);
            col.OfClass(typeof(PipingSystem));
            IList<Element> pipesystems = col.ToElements();
            foreach (Element e in pipesystems)
            {
                PipingSystem ps = e as PipingSystem;
                if ((ps.Name.Contains(sysName)) && (ps.Name.Contains(i.ToString())))
                {
                    bs = ps.get_BoundingBox(view);
                    //TaskDialog.Show("测试",bs.Max.ToString()+bs.Min.ToString()+sysName+i.ToString());
                    break;
                }
            }
            return bs;
        }

    }
}
