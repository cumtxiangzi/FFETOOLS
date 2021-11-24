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
    class CreatPipeSystem : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
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
                CreatPipeSystem cps = new CreatPipeSystem();
                cps.CompoundOperation(doc, uidoc, eid);

            }
            catch (Exception e)
            {

                TaskDialog.Show("系统图创建", e.Message);
                return Result.Failed;
            }

            return Result.Succeeded;

        }

        public bool CreatPipeSystems(Document doc, UIDocument uidoc, ElementId eid)
        {
            using (Transaction ts = new Transaction(doc, "管道系统图"))
            {
                if (TransactionStatus.Started == ts.Start())
                {
                    View3D view3D = View3D.CreateIsometric(doc, eid);
                    view3D.DisplayStyle = DisplayStyle.HLR;
                    view3D.DetailLevel = ViewDetailLevel.Fine;
                    view3D.OrientTo(new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626));
                    view3D.Name = "循环回水管道系统图";
                    view3D.SaveOrientationAndLock();
                    view3D.Scale = 50;
                    view3D.Discipline = ViewDiscipline.Mechanical;

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

                    List<ElementId> categories = new List<ElementId>();
                    categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
                    categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
                    categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
                    categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
                    ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Comments = foo", categories);
                    parameterFilterElement.Name = "给排水_消防给水管道系统2";

                    FilteredElementCollector parameterCollector = new FilteredElementCollector(doc);
                    Parameter parameter = parameterCollector.OfClass(typeof(Pipe)).FirstElement().get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM);

                    List<FilterRule> filterRules = new List<FilterRule>();
                    filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(parameter.Id, "气体消防系统", true));
                    parameterFilterElement.SetRules(filterRules);

                    OverrideGraphicSettings filterSettings = new OverrideGraphicSettings();
                    view3D.SetFilterOverrides(parameterFilterElement.Id, filterSettings);
                    view3D.SetFilterVisibility(parameterFilterElement.Id, false);
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

        public void CompoundOperation(Document doc, UIDocument uidoc, ElementId eid)
        {
            using (TransactionGroup transGroup = new TransactionGroup(doc, "创建管道系统"))
            {
                if (transGroup.Start() == TransactionStatus.Started)
                {
                    if (CreatPipeSystems(doc, uidoc, eid) && CreatPipeNotes(doc, uidoc))
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

    }
}
