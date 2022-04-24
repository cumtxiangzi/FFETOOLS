using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class QuickGridDimension : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) //框选轴线标注轴网
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            //获取轴网类型
            DimensionType dimType = null;
            FilteredElementCollector elems = new FilteredElementCollector(doc);
            foreach (DimensionType dt in elems.OfClass(typeof(DimensionType)))
            {
                if (dt.Name.Contains("给排水") && dt.StyleType==DimensionStyleType.Linear)
                {
                    dimType = dt;
                    //dimensionGrid(uidoc, dt);
                    break;
                }
            }
            if (dimType != null)
            {
                Document document = uidoc.Document;
                IList<Element> grids = uidoc.Selection.PickElementsByRectangle(new GridFilter(), "框选轴网");
                if (grids.Count > 1)
                {
                    XYZ selPoint = uidoc.Selection.PickPoint(ObjectSnapTypes.None, "选择尺寸定位位置");
                    View activeView = uidoc.ActiveView;

                    ReferenceArray referenceArray1 = new ReferenceArray();
                    ReferenceArray referenceArray2 = new ReferenceArray();
                    //获得最靠近选择点的轴网为参照基准
                    List<Grid> lineGrid = new List<Grid>();
                    Line referenceLine = null;
                    double dis = double.MaxValue;
                    foreach (Grid g in grids)
                    {
                        double d = g.Curve.Distance(selPoint);
                        Line line = g.Curve as Line;
                        if (line != null)
                        {
                            lineGrid.Add(g);
                            if (d < dis)
                            {
                                referenceLine = line;
                                dis = d;
                            }
                        }
                    }
                    //获得内侧尺寸标注的引用
                    foreach (Grid g in lineGrid)
                    {
                        Line line = g.Curve as Line;
                        if (line.Direction.IsAlmostEqualTo(referenceLine.Direction) || line.Direction.IsAlmostEqualTo(referenceLine.Direction.Multiply(-1)))
                        {
                            referenceArray1.Append(new Reference(g));
                        }
                    }
                    //获取外侧尺寸标注的引用
                    foreach (Reference refGrid in referenceArray1)
                    {
                        Grid g = doc.GetElement(refGrid) as Grid;
                        Line line = g.Curve as Line;
                        XYZ point1 = line.GetEndPoint(0);
                        XYZ point2 = line.GetEndPoint(1);
                        int i = 0;
                        foreach (Reference _refGrid in referenceArray1)
                        {
                            Grid _g = doc.GetElement(_refGrid) as Grid;
                            Line _line = _g.Curve as Line;
                            //XYZ point1 = _line.GetEndPoint(0);
                            //XYZ point2 = _line.GetEndPoint(1);
                            XYZ point = _line.GetEndPoint(0);
                            if (PointOnTheLeft(point1, point2, point))
                            {
                                i += 1;
                            }
                        }
                        if (i == 0 || i == referenceArray1.Size - 1)
                        {
                           referenceArray2.Append(new Reference(g));
                        }
                    }
                    //计算尺寸标注位置
                    XYZ lineDir = referenceLine.Direction.CrossProduct(new XYZ(0, 0, 1));
                    XYZ point_s = referenceLine.GetEndPoint(0);
                    XYZ point_e = referenceLine.GetEndPoint(1);
                    if (point_s.DistanceTo(selPoint) > point_e.DistanceTo(selPoint))
                    {
                        XYZ temPoint = point_s;
                        point_s = point_e;
                        point_e = temPoint;
                    }
                    XYZ offsetDir = point_e - point_s;
                    double lenght = dimType.get_Parameter(BuiltInParameter.TEXT_SIZE).AsDouble();
                    Line line_o = Line.CreateUnbound(selPoint, lineDir);
                    Line line_i = Line.CreateUnbound(selPoint + offsetDir.Normalize() * lenght * activeView.Scale * 1.9, lineDir);
                    //创建尺寸标注
                    using (Transaction tran = new Transaction(document, "轴网尺寸标注"))
                    {
                        tran.Start();
                        if(grids.Count>2)
                        {
                            document.Create.NewDimension(activeView, line_o, referenceArray2, dimType);
                            document.Create.NewDimension(activeView, line_i, referenceArray1, dimType);
                        }
                        else
                        {
                            document.Create.NewDimension(activeView, line_o, referenceArray2, dimType);
                        }
                        tran.Commit();
                    }
                }
            }
            else
            {
                TaskDialog.Show("错误", "未有轴网尺寸标注类型");
            }
            return Result.Succeeded;
        }
        bool PointOnTheLeft(XYZ point1, XYZ point2, XYZ point)
        {
            double r = (point1.X - point2.X) / (point1.Y - point2.Y) * (point.Y - point2.Y) + point2.X;
            if (r > point.X)
            {
                return true;
            }
            return false;
        }
    }  
}