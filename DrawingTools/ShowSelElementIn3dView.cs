using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TestBIM
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Element3dView : IExternalCommand
    {
        public Result Execute(Autodesk.Revit.UI.ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            Transaction newTran = null;
            try
            {
                if (null == cmdData)
                {
                    throw new ArgumentNullException("commandData");
                }

                Document doc = cmdData.Application.ActiveUIDocument.Document;

                newTran = new Transaction(doc);
                newTran.Start("shows select element in the 3dView");

                CreateElement3dView creater = new CreateElement3dView(cmdData);
                View3D view3d = null;
                creater.Create(ref view3d);

                newTran.Commit();
                if (null != view3d)
                {
                    cmdData.Application.ActiveUIDocument.ActiveView = view3d;
                }

                return Autodesk.Revit.UI.Result.Succeeded;
            }
            catch (Exception e)
            {
                msg = e.Message;
                if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                    newTran.RollBack();
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
    }


    class CreateElement3dView
    {
        private Autodesk.Revit.UI.ExternalCommandData m_Revit;

        public CreateElement3dView(Autodesk.Revit.UI.ExternalCommandData cmdData)
        {
            m_Revit = cmdData;
        }

        public void Create(ref View3D view3d)
        {
            ViewPlan view = m_Revit.View as ViewPlan;

            if (null == view
              || view.ViewType != ViewType.FloorPlan)
            {
                MessageBox.Show("Please run this command in an floor plan view.");
                return;
            }

            Autodesk.Revit.UI.Selection.Selection sel = m_Revit.Application.ActiveUIDocument.Selection;
            Autodesk.Revit.DB.Document doc = m_Revit.Application.ActiveUIDocument.Document;
            Element ele = doc.GetElement(sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "select a element"));

            if (null == ele)
            {
                return;
            }

            XYZ direction = new XYZ(1, 1, -1);
            view3d = m_Revit.Application.ActiveUIDocument.Document.Create.NewView3D(direction);
            if (null != view3d)
            {
                //view3d.SectionBox = ele.get_BoundingBox(view3d);
                HiddUnSelElements(ele, view3d);
            }

            Common.get3dViewName(doc, view3d);
        }

        protected void HiddUnSelElements(Element selElement, Autodesk.Revit.DB.View view)
        {
            ICollection<Element> collection = new FilteredElementCollector(m_Revit.Application.ActiveUIDocument.Document)
                .WhereElementIsNotElementType().ToElements();
            ElementSet elementSet = new ElementSet();
            foreach (Element ele in collection)
            {
                if (ele is FamilyInstance || ele is Wall ||
                    ele is Floor || ele is ContFooting ||
                    ele is CeilingAndFloor || ele is CurtainGridLine ||
                    ele is FaceWall || ele is RoofBase ||
                    ele is CurtainSystemBase || ele is HostedSweep ||
                    ele is WallSweep || ele is MEPCurve ||
                    ele is Wire || ele is CableTray ||
                    ele is Conduit || ele is DuctInsulation ||
                    ele is DuctLining || ele is PipeInsulation||              
                    ele is Duct || ele is FlexDuct ||
                    ele is FlexPipe || ele is Pipe ||
                    ele is DividedSurface || ele is TopographySurface||
                    ele is BeamSystem || ele is ModelArc||
                    ele is ModelEllipse || ele is ModelHermiteSpline||
                    ele is ModelLine || ele is ModelNurbSpline)
                {
                    if (ele.Id.Equals(selElement.Id) || ele.Id.Equals(view.Id))
                        continue;
                    if (ele.CanBeHidden(view))
                        elementSet.Insert(ele);
                }
                if (ele is Element)
                {
                    Category category = ele.Category;
                    if (null == category)
                        continue;
                    if ((int)BuiltInCategory.OST_Stairs == category.Id.IntegerValue ||
                       (int)BuiltInCategory.OST_StairsRailing == category.Id.IntegerValue)
                    {
                        if (ele.Id.Equals(selElement.Id) || ele.Id.Equals(view.Id))
                            continue;
                        if (ele.CanBeHidden(view))
                            elementSet.Insert(ele);
                    }
                }
            }
            view.HideElements(elementSet.);
        }

    }
}
