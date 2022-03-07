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
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class OutdoorDrangePipe : IExternalCommand
    {
        public static OutdoorDrangePipeForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new OutdoorDrangePipeForm();
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                //mainfrm.Show();
                AutoCreatWells(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
        public void AutoCreatWells(Document doc, UIDocument uidoc)
        {
            Selection selection = uidoc.Selection;
            IList<Reference> refList = selection.PickObjects(ObjectType.Element, new PipeSelectionFilter(), "请选择详图线");
            if (refList.Count == 0)
            {
                TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                AutoCreatWells(doc, uidoc);
            }
            else
            {
                CreatWells(doc, refList);
            }
        }

        public void CreatWells(Document doc, IList<Reference> referenceList)
        {
            // 创建排水检查井主函数
            List<XYZ> wellPoints = new List<XYZ>();
            List<Pipe> pipeList = new List<Pipe>();
            List<Pipe> newPipes = new List<Pipe>();
            List<Pipe> allPipes = new List<Pipe>();

            TransactionGroup tg = new TransactionGroup(doc, "创建室外排水管网");
            tg.Start();

            using (Transaction trans = new Transaction(doc, "批量打断含三通的管道"))
            {
                trans.Start();

                Options opts = new Options { DetailLevel = ViewDetailLevel.Fine, ComputeReferences = true };
                foreach (Reference reference in referenceList)
                {
                    Pipe detailLine = doc.GetElement(reference.ElementId) as Pipe;
                    //var geometry = detailLine.get_Geometry(opts);
                    //Line centerline = geometry.First() as Line;
                    pipeList.Add(detailLine);
                }

                foreach (Pipe item in pipeList)
                {
                    ConnectorSet conset = item.ConnectorManager.Connectors;
                    ElementId sys = item.MEPSystem.GetTypeId();
                    ElementId type = item.PipeType.Id;
                    ElementId level = item.ReferenceLevel.Id;

                    List<XYZ> breakPoints = new List<XYZ>();
                    Line line = item.LocationLine();
                    XYZ startPoint = LineExtension.StartPoint(line);
                    XYZ endPoint = LineExtension.EndPoint(line);

                    double length = Convert.ToInt32(UnitUtils.Convert(line.Length, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS));
                    if (length > 40000 && conset.Size <= 2)
                    {
                        newPipes.Add(item);
                    }

                    if (length <= 40000 && conset.Size <= 2)
                    {
                        allPipes.Add(item);
                    }

                    if (conset.Size > 2)
                    {
                        foreach (Connector con in conset)
                        {
                            XYZ position = con.Origin;
                            breakPoints.Add(position);
                        }

                        breakPoints.Sort((a, b) => a.X.CompareTo(b.X));//排序只考虑管线垂直情况
                        breakPoints.Sort((a, b) => a.Y.CompareTo(b.Y));

                        for (int i = 0; i < breakPoints.Count - 1; i++)
                        {
                            Pipe p = Pipe.CreatePlaceholder(doc, sys, type, level, breakPoints.ElementAt(i), breakPoints.ElementAt(i + 1));
                            newPipes.Add(p);
                            allPipes.Add(p);
                        }

                        //for (int i = 0; i < newPipes.Count - 1; i++)
                        //{
                        //    Connector con1 = PipeExtension.EndCon(newPipes.ElementAt(i));
                        //    Connector con2 = PipeExtension.StartCon(newPipes.ElementAt(i + 1));
                        //    con1.ConnectTo(con2);
                        //}
                        doc.Delete(item.Id);
                    }
                }
                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "批量打断大于40米的管道"))
            {
                trans.Start();
                foreach (Pipe pipe in newPipes)
                {
                    Line newLine = pipe.LocationLine();
                    double newLength = Convert.ToInt32(UnitUtils.Convert(newLine.Length, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS));
                    XYZ newStartPoint = LineExtension.StartPoint(newLine);
                    XYZ newEndPoint = LineExtension.EndPoint(newLine);

                    ElementId sys = pipe.MEPSystem.GetTypeId();
                    ElementId type = pipe.PipeType.Id;
                    ElementId level = pipe.ReferenceLevel.Id;

                    if (newLength > 40000)
                    {
                        allPipes.Remove(pipe);
                        List<XYZ> points = new List<XYZ>();
                        double num = Math.Floor(newLength / 40000);
                        int mod = Convert.ToInt32(newLength % 40000);

                        points.Add(newStartPoint);

                        if (mod == 0)
                        {
                            for (int i = 1; i < num; i++)
                            {
                                XYZ point = new XYZ(((num - i) * newStartPoint.X + i * newEndPoint.X) / num, ((num - i) * newStartPoint.Y + i * newEndPoint.Y) / num, newStartPoint.Z);//n等分点坐标公式
                                points.Add(point);
                            }
                        }
                        else
                        {
                            for (int i = 1; i < num + 1; i++)
                            {
                                double k = num + 1;
                                XYZ point = new XYZ(((k - i) * newStartPoint.X + i * newEndPoint.X) / k, ((k - i) * newStartPoint.Y + i * newEndPoint.Y) / k, newStartPoint.Z);
                                points.Add(point);
                            }
                        }

                        points.Add(newEndPoint);

                        for (int i = 0; i < points.Count - 1; i++)
                        {
                            Pipe p = Pipe.CreatePlaceholder(doc, sys, type, level, points.ElementAt(i), points.ElementAt(i + 1));
                            allPipes.Add(p);
                        }

                        doc.Delete(pipe.Id);
                    }
                }
                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "批量生成排水检查井"))
            {
                trans.Start();



                StructureFamilyLoad(doc, "排水构筑物", "砖砌排水检查井");
                FamilySymbol familySymbol = WaterStructureSymbol(doc, "排水构筑物", "砖砌排水检查井");
                familySymbol.Activate();
                FamilyInstance wellinstance = null;

                foreach (XYZ item in wellPoints)
                {
                    wellinstance = doc.Create.NewFamilyInstance(item, familySymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
                }

                trans.Commit();
            }

            tg.Assimilate();

        }

        //public void BreakPipeMethod(Document doc, Pipe pipe, XYZ point)
        //{
        //    var mep = pipe as MEPCurve;
        //    PlumbingUtils.BreakCurve(doc, mep.Id, point);
        //}

        public bool EqualPoint(XYZ point1, XYZ point2)
        {
            bool equal = false;
            if (point1.X == point2.X && point1.Y == point2.Y && point1.Z == point2.Z)
            {
                equal = true;
            }
            return equal;
        }
        public List<XYZ> GetCrosspoint(List<DetailLine> lines)
        {
            //获取详图线所有交点
            List<XYZ> Points = new List<XYZ>();
            foreach (DetailLine line in lines)
            {
                DetailLine currentLine = line;
                foreach (DetailLine line1 in lines)
                {
                    IntersectionResultArray ira = null;
                    SetComparisonResult scr = currentLine.GeometryCurve.Intersect(line1.GeometryCurve, out ira);
                    if (ira != null)
                    {
                        IntersectionResult ir = ira.get_Item(0);

                        // 判断点是否重复
                        if (!CheckPoint(Points, ir.XYZPoint))
                        {
                            Points.Add(ir.XYZPoint);
                        }
                    }
                }
            }
            return Points;
        }
        public List<XYZ> GetCrosspoint(List<Pipe> pipes)
        {
            //获取管线占位符所有交点
            List<XYZ> Points = new List<XYZ>();

            foreach (Pipe pipe in pipes)
            {
                Pipe currentPipe = pipe;
                foreach (Pipe line1 in pipes)
                {
                    IntersectionResultArray ira = null;
                    SetComparisonResult scr = currentPipe.LocationLine().Intersect(line1.LocationLine(), out ira);
                    if (ira != null)
                    {
                        IntersectionResult ir = ira.get_Item(0);

                        // 判断点是否重复
                        if (!CheckPoint(Points, ir.XYZPoint))
                        {
                            Points.Add(ir.XYZPoint);
                        }
                    }
                }
            }
            return Points;
        }
        public bool CheckPoint(List<XYZ> points, XYZ point)
        {
            bool flag = false;
            foreach (XYZ p in points)
            {
                if (p.IsAlmostEqualTo(point))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        public void StructureFamilyLoad(Document doc, string categoryName, string familyName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(familyName) && item.Name.Contains("结构"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "结构" + "_" + categoryName + "_" + familyName + ".rfa");
            }
        }
        public FamilySymbol WaterStructureSymbol(Document doc, string categoryName, string familyName) //给排水构筑物
        {
            FilteredElementCollector waterStructureCollector = new FilteredElementCollector(doc);
            waterStructureCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel);
            List<FamilySymbol> waterStructureSymbolList = new List<FamilySymbol>();
            FamilySymbol waterStructure = null;
            string fullname = "结构" + "_" + categoryName + "_" + familyName;

            IList<Element> waterStructures = waterStructureCollector.ToElements();
            foreach (FamilySymbol item in waterStructures)
            {
                if (item.Family.Name == fullname)
                {
                    waterStructureSymbolList.Add(item);
                }
            }
            waterStructure = waterStructureSymbolList.FirstOrDefault();
            return waterStructure;
        }
    }
    public class ExecuteEventOutdoorDrangePipe : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                using (Transaction trans = new Transaction(doc, "创建室外排水管网"))
                {
                    trans.Start();





                    trans.Commit();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "创建室外排水管网";
        }
    }

}
