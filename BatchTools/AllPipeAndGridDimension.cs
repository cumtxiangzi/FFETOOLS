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
    public class AllPipeAndGridDimension : IExternalCommand //所有管道和轴网创建标注
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
                ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
                ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
                LogicalOrFilter orFilter = new LogicalOrFilter(filter1, filter2);
                collector.WherePasses(orFilter);

                ReferenceArray refArrayX = new ReferenceArray();
                ReferenceArray refArrayY = new ReferenceArray();
                XYZ xDir = new XYZ(1, 0, 0);
                XYZ yDir = new XYZ(0, 1, 0);

                double minY = double.MaxValue;
                double maxY = double.MinValue;
                XYZ minPoint = null;
                XYZ maxPoint = null;

                double minX = double.MaxValue;
                double maxX = double.MinValue;
                XYZ minPointX = null;
                XYZ maxPointX = null;

                foreach (Element elem in collector.ToElements())
                {
                    if (elem is Grid)
                    {
                        Grid grid = elem as Grid;
                        Line gLine = grid.Curve as Line;
                        XYZ gDir = gLine.Direction;

                        if (gDir.IsAlmostEqualTo(xDir) || gDir.IsAlmostEqualTo(-xDir))
                        {
                            XYZ startPoint = gLine.GetEndPoint(0);
                            double y = startPoint.Y;
                            if (y < minY)
                            {
                                minY = y;
                                minPoint = startPoint;
                            }
                            else if (y > maxY)
                            {
                                maxY = y;
                                maxPoint = startPoint;
                            }
                            refArrayX.Append(new Reference(elem));
                        }
                        else if (gDir.IsAlmostEqualTo(yDir) || gDir.IsAlmostEqualTo(-yDir))
                        {
                            XYZ startPoint = gLine.GetEndPoint(0);
                            double x = startPoint.X;
                            if (x < minX)
                            {
                                minX = x;
                                minPointX = startPoint;
                            }
                            else if (x > maxX)
                            {
                                maxX = x;
                                maxPointX = startPoint;
                            }
                            refArrayY.Append(new Reference(elem));
                        }
                    }
                    else if (elem is Pipe)
                    {
                        Pipe pipe = elem as Pipe;
                        Line pLine = (pipe.Location as LocationCurve).Curve as Line;
                        XYZ pDir = pLine.Direction;
                        if (pDir.IsAlmostEqualTo(xDir) || pDir.IsAlmostEqualTo(-xDir))
                        {
                            XYZ startPoint = pLine.GetEndPoint(0);
                            double y = startPoint.Y;
                            if (y < minY)
                            {
                                minY = y;
                                minPoint = startPoint;
                            }
                            else if (y > maxY)
                            {
                                maxY = y;
                                maxPoint = startPoint;
                            }
                            refArrayX.Append(new Reference(elem));
                        }
                        else if (pDir.IsAlmostEqualTo(yDir) || pDir.IsAlmostEqualTo(-yDir))
                        {

                            XYZ startPoint = pLine.GetEndPoint(0);
                            double x = startPoint.X;
                            if (x < minX)
                            {
                                minX = x;
                                minPointX = startPoint;
                            }
                            else if (x > maxX)
                            {
                                maxX = x;
                                maxPointX = startPoint;
                            }
                            refArrayY.Append(new Reference(elem));
                        }
                    }
                }
                //拾取一个点，基于该点的X、Y放置标注
                XYZ selectPoint = uidoc.Selection.PickPoint(ObjectSnapTypes.None);

                XYZ yPoint1 = new XYZ(selectPoint.X, minY, 0);
                XYZ yPoint2 = new XYZ(selectPoint.X, maxY, 0);
                Line yLine = Line.CreateBound(yPoint1, yPoint2);

                XYZ xPoint1 = new XYZ(minX, selectPoint.Y, 0);
                XYZ xPoint2 = new XYZ(maxX, selectPoint.Y, 0);
                Line xLine = Line.CreateBound(xPoint1, xPoint2);
                using (Transaction trans = new Transaction(doc))
                {
                    trans.Start("Create Dimension");
                    doc.Create.NewDimension(doc.ActiveView, yLine, refArrayX);

                    doc.Create.NewDimension(doc.ActiveView, xLine, refArrayY);
                    trans.Commit();
                }

            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
