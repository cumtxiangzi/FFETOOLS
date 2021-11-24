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
    class CreatPipeSystem1 : IExternalCommand
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

                if (crsForm.XJChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "XJ");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_循环给水管道系统图" + (i + 1).ToString(), "循环给水管道系统图", "给排水_循环给水管道系统(非)", "XJ", i + 1, ot);
                    }

                }
                if (crsForm.XHChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "XH");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_循环回水管道系统图" + (i + 1).ToString(), "循环回水管道系统图", "给排水_循环回水管道系统(非)", "XH", i + 1,ot);
                    }
                }
                if (crsForm.JChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "J");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_给水管道系统图" + (i + 1).ToString(), "给水管道系统图", "给排水_生活给水管道系统(非)", "J", i + 1,ot);
                    }
                }
                if (crsForm.WChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "W");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_排水管道系统图" + (i + 1).ToString(), "排水管道系统图", "给排水_污水管道系统(非)", "W", i + 1,ot);
                    }
                }
                if (crsForm.XFChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "XF");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_消防给水管道系统图" + (i + 1).ToString(), "消防给水管道系统图", "给排水_消防给水管道系统(非)", "XF", i + 1,ot);
                    }
                }
                if (crsForm.HNChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "HN");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_混凝剂管道系统图" + (i + 1).ToString(), "混凝剂管道系统图", "给排水_混凝剂管道系统(非)", "HN", i + 1,ot);
                    }
                }
                if (crsForm.XDChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "XD");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_消毒剂管道系统图" + (i + 1).ToString(), "消毒剂管道系统图", "给排水_消毒剂管道系统(非)", "XD", i + 1,ot);
                    }
                }
                if (crsForm.WDChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "WD");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_水质稳定剂管道系统图" + (i + 1).ToString(), "水质稳定剂管道系统图", "给排水_水质稳定剂管道系统(非)", "WD", i + 1,ot);
                    }
                }
                if (crsForm.YJChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "YJ");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_水源输水管道系统图" + (i + 1).ToString(), "水源输水管道系统图", "给排水_水源输水管道系统(非)", "YJ", i + 1,ot);
                    }
                }
                if (crsForm.ZJChkBox.IsChecked == true)
                {
                    XYZ ot = new XYZ();
                    if (crsForm.E_Button.IsChecked == true)
                    {
                        ot = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);
                    }
                    if (crsForm.W_Button.IsChecked == true)
                    {
                        ot = new XYZ(0.577350269189626, -0.577350269189626, -0.577350269189626);
                    }
                    int num = systemNum(doc, "ZJ");
                    for (int i = 0; i < num; i++)
                    {
                        CompoundOperation(doc, uidoc, uiapp, eid, "给排水_中水管道系统图" + (i + 1).ToString(), "中水管道系统图", "给排水_中水管道系统(非)", "ZJ", i + 1,ot);
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



                    //BoundingBoxXYZ pipesysBound = BoundSize(doc, sysabbr, i, view3D);

                    //Transform transf = view3D.CropBox.Transform;
                    //Transform transfIn = view3D.CropBox.Transform.Inverse;

                    //XYZ pt1 = transfIn.OfPoint(pipesysBound.Max);
                    //XYZ pt2 = transfIn.OfPoint(pipesysBound.Min);

                    //XYZ pt3 = new XYZ(pt1.X, pt1.Y, view3D.CropBox.Max.Z);
                    //XYZ pt4 = new XYZ(pt2.X, pt2.Y, view3D.CropBox.Min.Z);                 

                    //BoundingBoxXYZ box = new BoundingBoxXYZ();
                    //box.Max = pt3;
                    //box.Min = pt4;
                    //view3D.CropBox = box;
                    //view3D.CropBoxActive = true;
                    //view3D.CropBoxVisible = false;

                    //RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.Window);
                    //uiapp.PostCommand(cmdId);



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
                    if (!(note==1))
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

        //BoundingBoxXYZ BoundSize(Document doc, string sysName, int i, View3D view)
        //{
        //    BoundingBoxXYZ bs = new BoundingBoxXYZ();
        //    FilteredElementCollector col = new FilteredElementCollector(doc);
        //    col.OfClass(typeof(PipingSystem));
        //    IList<Element> pipesystems = col.ToElements();
        //    foreach (Element e in pipesystems)
        //    {
        //        PipingSystem ps = e as PipingSystem;
        //        if ((ps.Name.Contains(sysName)) && (ps.Name.Contains(i.ToString())))
        //        {
        //            bs = ps.get_BoundingBox(view);
        //            //TaskDialog.Show("测试",bs.Max.ToString()+bs.Min.ToString()+sysName+i.ToString());
        //            break;
        //        }
        //    }
        //    return bs;
        //}       

    }
}
