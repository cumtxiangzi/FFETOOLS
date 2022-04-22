#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RevitGrid
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        private int _step = 0;
        private int _coordinatesByX = 0;
        private int _coordinatesByY = 0;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            while (_step != 10)
            {
                CreateGridHorizontal(doc);
                _coordinatesByY += 10;
                _step++;
            }
            CreateLinearDimensionVertical(doc);

            while (_step != 20)
            {
                CreateGridVertical(doc);
                _coordinatesByX += 11;
                _step++;
            }
            CreateLinearDimensionHorizontal(doc);

            return Result.Succeeded;
        }

        private Result CreateGridHorizontal(Document doc)
        {
            using (Transaction firstTrans = new Transaction(doc))
            {
                try
                {
                    firstTrans.Start("Start");

                    XYZ start = new XYZ(-20, _coordinatesByY, 0);
                    XYZ end = new XYZ(120, _coordinatesByY, 0);
                    Line geomLine = Line.CreateBound(start, end);

                    Grid lineGrid = Grid.Create(doc, geomLine);


                    if (null == lineGrid)
                    {
                        throw new Exception("Create a new straight grid failed.");
                    }
                    if (_coordinatesByY == 0)
                    {
                        lineGrid.Name = "A";
                    }
                    if (_coordinatesByY > 90)
                    {

                    }
                    firstTrans.Commit();
                    return Result.Succeeded;
                }
                catch (Exception ex)
                {
                    var h = ex;
                    return Result.Failed;
                }
            }
        }

        private Result CreateGridVertical(Document doc)
        {
            using (Transaction firstTrans = new Transaction(doc))
            {
                try
                {
                    firstTrans.Start("Start");
                    XYZ start = new XYZ(_coordinatesByX, -20, 0);
                    XYZ end = new XYZ(_coordinatesByX, 120, 0);
                    Line geomLine = Line.CreateBound(start, end);

                    Grid lineGrid = Grid.Create(doc, geomLine);

                    if (null == lineGrid)
                    {
                        throw new Exception("Create a new straight grid failed.");
                    }
                    if (_coordinatesByX == 0)
                    {
                        lineGrid.Name = "1";
                    }
                    firstTrans.Commit();
                    return Result.Succeeded;
                }
                catch (Exception ex)
                {
                    var h = ex;
                    return Result.Failed;
                }
            }
        }

        public Result CreateLinearDimensionVertical(Document doc)
        {
            FilteredElementCollector newFilter = new FilteredElementCollector(doc);
            ICollection<Element> allGrid = newFilter.OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().ToElements();
            using (Transaction firstTrans = new Transaction(doc))
            {
                try
                {
                    firstTrans.Start("Start");

                    int step = 0;
                    int coordinatesByY = 0;

                    ReferenceArray refArray = new ReferenceArray();

                    foreach (var grid in allGrid)
                    {
                        Grid grid1 = (Grid)doc.GetElement(grid.Id);
                        refArray.Append(new Reference(grid1));

                        if (step >= 1 && coordinatesByY != 90)
                        {
                            XYZ location1 = new XYZ(120, coordinatesByY, 0);
                            XYZ location2 = new XYZ(120, coordinatesByY + 10, 0);

                            Line line = Line.CreateBound(location1, location2);

                            if (!doc.IsFamilyDocument)
                            {
                                doc.Create.NewDimension(
                                  doc.ActiveView, line, refArray);
                            }
                            else
                            {
                                doc.FamilyCreate.NewDimension(
                                  doc.ActiveView, line, refArray);
                            }

                            coordinatesByY += 10;

                            grid1 = (Grid)doc.GetElement(grid.Id);
                            refArray.Clear();
                            refArray.Append(new Reference(grid1));
                        }
                        step++;
                    }

                    firstTrans.Commit();
                    return Result.Succeeded;
                }
                catch
                {
                    return Result.Failed;
                }
            }
        }

        public Result CreateLinearDimensionHorizontal(Document doc)
        {
            FilteredElementCollector newFilter = new FilteredElementCollector(doc);
            ICollection<Element> allGrid = newFilter.OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().ToElements();
            using (Transaction firstTrans = new Transaction(doc))
            {
                try
                {
                    firstTrans.Start("Start");

                    int step = 0;
                    int coordinatesByX = 0;

                    ReferenceArray refArray = new ReferenceArray();

                    foreach (var grid in allGrid)
                    {
                        if (step > 9)
                        {
                            Grid grid1 = (Grid)doc.GetElement(grid.Id);
                            refArray.Append(new Reference(grid1));

                            if (step >= 11 && coordinatesByX != 90)
                            {
                                XYZ location1 = new XYZ(coordinatesByX, 120, 0);
                                XYZ location2 = new XYZ(coordinatesByX + 11, 120, 0);

                                Line line = Line.CreateBound(location1, location2);

                                if (!doc.IsFamilyDocument)
                                {
                                    doc.Create.NewDimension(
                                      doc.ActiveView, line, refArray);
                                }
                                else
                                {
                                    doc.FamilyCreate.NewDimension(
                                      doc.ActiveView, line, refArray);
                                }

                                coordinatesByX += 11;

                                grid1 = (Grid)doc.GetElement(grid.Id);
                                refArray.Clear();
                                refArray.Append(new Reference(grid1));
                            }
                        }
                        step++;
                    }

                    firstTrans.Commit();
                    return Result.Succeeded;
                }
                catch
                {
                    return Result.Failed;
                }
            }
        }
    }
}

