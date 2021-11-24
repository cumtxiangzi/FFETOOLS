using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatSectionBox : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                Selection selection = uidoc.Selection;
                View view = uidoc.ActiveView;
                if (view is ViewPlan)
                {

                }
                else
                {
                    TaskDialog.Show("警告", "请在平面视图中进行操作");

                    return Result.Cancelled;
                }

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

                BoundingBoxXYZ buBoxXYZ = new BoundingBoxXYZ();
                double min_x = 0; double min_y = 0; double max_x = 0; double max_y = 0;
                try
                {
                    PickedBox pickedbox = uidoc.Selection.PickBox(PickBoxStyle.Crossing, "请选择要生成局部三维的平面区域");
                    min_x = BackMin(pickedbox.Min.X, pickedbox.Max.X);
                    min_y = BackMin(pickedbox.Min.Y, pickedbox.Max.Y);
                    max_x = BackMax(pickedbox.Min.X, pickedbox.Max.X);
                    max_y = BackMax(pickedbox.Min.Y, pickedbox.Max.Y);
                }
                catch
                {
                    return Result.Cancelled;
                }

                Transaction ts = new Transaction(doc, "局部三维");
                ts.Start();
                try
                {
                    FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
                    viewCollector.OfClass(typeof(View3D)).OfCategory(BuiltInCategory.OST_Views);
                    IList<Element> views = viewCollector.ToElements();
                    foreach (View3D item in views)
                    {
                        if (item.Name.Contains("临时局部三维视图"))
                        {                           
                            doc.Delete(item.Id);
                        }                     
                    }

                    View3D view3D = View3D.CreateIsometric(doc, eid);
                    view3D.IsSectionBoxActive = true;
                    view3D.GetSectionBox().Enabled = true;
                    view3D.CropBoxActive = false;//裁剪视图。
                    view3D.CropBoxVisible = false;//裁剪视图可见性。
                    double max_z = 0;
                    double min_z = 0;
                    TopClipPlan(doc, view, ref max_z);//顶部剪切高度
                    BottomCLipPlan(doc, view, ref min_z);
                    XYZ xyz1 = new XYZ(min_x, min_y, min_z);
                    XYZ xyz2 = new XYZ(max_x, max_y, max_z);
                    buBoxXYZ.set_Bounds(0, xyz1);//设置剖面框min
                    buBoxXYZ.set_Bounds(1, xyz2);//设置剖面框max
                    view3D.SetSectionBox(buBoxXYZ);
                    try
                    {
                        view3D.DisplayStyle = DisplayStyle.Shading;
                        view3D.DetailLevel = ViewDetailLevel.Fine;
                        view3D.Name = "临时局部三维视图";
                    }
                    catch
                    {

                    }
                    ts.Commit();

                    uidoc.ActiveView = view3D;
                }
                catch { ts.Commit(); return Result.Cancelled; }
                return Result.Succeeded;
            }
            catch
            {

                return Result.Cancelled;
            }

        }
        //返回最小值
        private Double BackMin(Double a, Double b)
        {
            if (a > b)
            {
                return b;
            }
            else
            {
                return a;
            }

        }

        //返回最大值
        private Double BackMax(Double a, Double b)
        {
            if (a > b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
        private void TopClipPlan(Document doc, View view, ref double max_z)//顶部偏移
        {
            try
            {
                if (view is ViewPlan)
                {
                    ViewPlan viewplan = view as ViewPlan;
                    PlanViewRange viewRange = viewplan.GetViewRange();
                    ElementId topclipPlane = viewRange.GetLevelId(PlanViewPlane.TopClipPlane);
                    double doffset = viewRange.GetOffset(PlanViewPlane.TopClipPlane);
                    if (topclipPlane.IntegerValue > 0)
                    {
                        Level levelabove = doc.GetElement(topclipPlane) as Level;
                        double level = levelabove.ProjectElevation;
                        max_z = level + doffset;
                    }
                    else
                    {
                        max_z = 1500;
                    }

                }
            }
            catch { }
        }
        private void BottomCLipPlan(Document doc, View view, ref double min_z)//底部偏移
        {
            try
            {
                if (view is ViewPlan)
                {
                    ViewPlan viewplan = view as ViewPlan;
                    PlanViewRange viewRange = viewplan.GetViewRange();
                    ElementId bottomclipPlane = viewRange.GetLevelId(PlanViewPlane.BottomClipPlane);
                    double Boffset = viewRange.GetOffset(PlanViewPlane.BottomClipPlane);
                    if (bottomclipPlane.IntegerValue > 0)
                    {
                        Level levelBottom = doc.GetElement(bottomclipPlane) as Level;
                        double level = levelBottom.ProjectElevation;
                        min_z = level + Boffset;
                    }
                    else
                    {
                        min_z = -1500;
                    }

                }
            }
            catch { }
        }
    
    }
}
