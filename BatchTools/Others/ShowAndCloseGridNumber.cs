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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class ShowAndCloseGridNumber : IExternalCommand//框选关闭轴网编号
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            PickedBox pickBox = null;
            try
            {
                pickBox = uidoc.Selection.PickBox(PickBoxStyle.Crossing, "请选择需要处理的轴线");
            }
            catch { return Result.Succeeded; }
            //请框选需要打开或者关闭的标高符号
            //投影到视图中
            XYZ maxPoint = pickBox.Max;
            XYZ minPoint = pickBox.Min;
            //极值
            double minX = Math.Min(minPoint.X, maxPoint.X);
            double maxX = Math.Max(minPoint.X, maxPoint.X);
            double minY = Math.Min(minPoint.Y, maxPoint.Y);
            double maxY = Math.Max(minPoint.Y, maxPoint.Y);
            List<XYZ> pointList = TwoPointGetPointList(new XYZ(minX, minY, 0), new XYZ(maxX, maxY, 0));
            List<Grid> gridList = new FilteredElementCollector(doc, uidoc.ActiveGraphicalView.Id).OfClass(typeof(Grid)).OfType<Grid>().ToList();

            Transaction trans = new Transaction(doc, "显示或者关闭轴号");
            trans.Start();
            foreach (var grid in gridList)
            {
                Curve c = grid.Curve;
                if (IsInPolygon(c.GetEndPoint(0).SetZ(), pointList))
                {
                    SetGridBubbleVisible(grid, uidoc.ActiveGraphicalView, DatumEnds.End1);
                }
                else if (IsInPolygon(c.GetEndPoint(1).SetZ(), pointList))
                {
                    SetGridBubbleVisible(grid, uidoc.ActiveGraphicalView, DatumEnds.End0);
                }
            }
            trans.Commit();


            return Result.Succeeded;
        }
        public void SetGridBubbleVisible(Grid grid, View view, DatumEnds ends)
        {
            bool visible = grid.IsBubbleVisibleInView(ends, view);
            if (visible)
            {
                grid.HideBubbleInView(ends, view);
            }
            else
            {
                grid.ShowBubbleInView(ends, view);
            }
        }
        public List<XYZ> TwoPointGetPointList(XYZ minPoint, XYZ maxPoint)
        {
            List<XYZ> result = new List<XYZ>();
            try
            {
                XYZ p1 = minPoint;
                XYZ p3 = maxPoint;
                XYZ p2 = new XYZ(maxPoint.X, minPoint.Y, 0);
                XYZ p4 = new XYZ(minPoint.X, maxPoint.Y, 0);
                result.Add(p1);
                result.Add(p2);
                result.Add(p3);
                result.Add(p4);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("框选轮廓过小，无法判别");
                result = new List<XYZ>();
            }

            return result;
        }
        public bool IsInPolygon(XYZ checkPoint, List<XYZ> polygonPoints)
        {
            bool inSide = false;
            int pointCount = polygonPoints.Count;
            XYZ p1, p2;
            for (int i = 0, j = pointCount - 1;
                i < pointCount;
                j = i, i++)
            {
                p1 = polygonPoints[i];
                p2 = polygonPoints[j];
                if (checkPoint.Y < p2.Y)
                {
                    if (p1.Y <= checkPoint.Y)
                    {
                        if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) > (checkPoint.X - p1.X) * (p2.Y - p1.Y)
                        )
                        {
                            inSide = (!inSide);
                        }
                    }
                }
                else if (checkPoint.Y < p1.Y)
                {
                    if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) < (checkPoint.X - p1.X) * (p2.Y - p1.Y)
                    )
                    {
                        inSide = (!inSide);
                    }
                }
            }

            return inSide;
        }
    }

}
