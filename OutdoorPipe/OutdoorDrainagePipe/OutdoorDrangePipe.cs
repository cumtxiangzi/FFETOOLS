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

                //mainfrm = new OutdoorDrangePipeForm(); //�������壬Ϊ��������ʹ��
                //IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                //WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                // helper.Owner = rvtPtr;
                //mainfrm.Show();
                IList<TopographySurface> tpSurface = CollectorHelper.TCollector<TopographySurface>(doc);
                int surfaceNum = tpSurface.Count;

                View view = uidoc.ActiveView;
                if (view is View3D)
                {
                    if (surfaceNum != 0)
                    {
                        AutoCreatWells(doc, uidoc);
                    }
                    else
                    {
                        TaskDialog.Show("����", "��ȷ����ά��ͼ�д�����ͼ���β���ʾ");
                    }
                }
                else
                {
                    TaskDialog.Show("����", "������ά��ͼ�н��в���");
                    //PipeSupportSection.mainfrm.Show();
                    //mainfrm.Show();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
        public void AutoCreatWells(Document doc, UIDocument uidoc)
        {
            Selection selection = uidoc.Selection;
            IList<Reference> refList = selection.PickObjects(ObjectType.Element, new DrainagePipeSelectionFilter(), "��ѡ����ˮ�ܶ�λ��(�ܵ�ռλ��)");
            if (refList.Count == 0)
            {
                TaskDialog.Show("����", "��û��ѡ���κ�Ԫ�أ�������ѡ��");
                AutoCreatWells(doc, uidoc);
            }
            else
            {
                CreatWells(doc, refList);
            }
        }

        public void CreatWells(Document doc, IList<Reference> referenceList)
        {
            // ������ˮ��龮������
            List<XYZ> wellPoints = new List<XYZ>();
            List<Pipe> pipeList = new List<Pipe>();
            List<Pipe> newPipes = new List<Pipe>();
            List<Pipe> allPipes = new List<Pipe>();

            TransactionGroup tg = new TransactionGroup(doc, "����������ˮ����");
            tg.Start();

            using (Transaction trans = new Transaction(doc, "������Ϻ���ͨ�Ĺܵ�"))
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

                        breakPoints.Sort((a, b) => a.X.CompareTo(b.X));//����ֻ���ǹ��ߴ�ֱ���
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

            using (Transaction trans = new Transaction(doc, "������ϴ���40�׵Ĺܵ�"))
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
                                XYZ point = new XYZ(((num - i) * newStartPoint.X + i * newEndPoint.X) / num, ((num - i) * newStartPoint.Y + i * newEndPoint.Y) / num, newStartPoint.Z);//n�ȷֵ����깫ʽ
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

            using (Transaction trans = new Transaction(doc, "����������ˮ��龮"))
            {
                trans.Start();

                wellPoints = GetCrosspoint(allPipes);
                StructureFamilyLoad(doc, "��ˮ������", "ש����ˮ��龮");
                FamilySymbol familySymbol = WaterStructureSymbol(doc, "��ˮ������", "ש����ˮ��龮");
                familySymbol.Activate();
                FamilyInstance wellinstance = null;

                int num = 1;
                foreach (XYZ item in wellPoints)
                {
                    wellinstance = doc.Create.NewFamilyInstance(item, familySymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
                    Line line = CalculateHeight(doc, item);
                    double lgh = UnitUtils.Convert(line.Length, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
                    Parameter height = wellinstance.LookupParameter("ƫ��");
                    height.SetValueString(lgh.ToString());
                    Parameter depth = wellinstance.LookupParameter("�����ĸ�");
                    depth.SetValueString("1500");

                    Parameter code = wellinstance.LookupParameter("���");
                    code.Set("P" + num.ToString());
                    num++;
                }

                List<Pipe> pipesHDPE = new List<Pipe>();
                List<Pipe> pipesUPVC = new List<Pipe>();

                foreach (Pipe pipe in allPipes)
                {
                    bool oneEqual = false;
                    bool twoEqual = false;
                    Line newLine = pipe.LocationLine();
                    XYZ newStartPoint = LineExtension.StartPoint(newLine);
                    XYZ newEndPoint = LineExtension.EndPoint(newLine);

                    foreach (XYZ point in wellPoints)
                    {
                        if (newStartPoint.IsAlmostEqualTo(point))
                        {
                            oneEqual = true;
                            break;
                        }
                    }
                    foreach (XYZ point in wellPoints)
                    {
                        if (newEndPoint.IsAlmostEqualTo(point))
                        {
                            twoEqual = true;
                            break;
                        }
                    }

                    if (oneEqual && twoEqual)
                    {
                        pipesHDPE.Add(pipe);
                    }
                    else
                    {
                        pipesUPVC.Add(pipe);
                    }
                }

                ElementId sys = GetPipeSystemType(doc, "����ˮ", "��ˮ�ܵ�").Id;
                ElementId typeHDPE = GetPipeType(doc, "����ˮ", "HDPE��").Id;
                ElementId typeUPVC = GetPipeType(doc, "����ˮ", "UPVC��").Id;
                ElementId level = GetPipeLevel(doc, "0.000").Id;

                foreach (Pipe pipe in pipesHDPE)
                {
                    Line newLine = pipe.LocationLine();
                    XYZ newStartPoint = LineExtension.StartPoint(newLine);
                    XYZ newEndPoint = LineExtension.EndPoint(newLine);

                    Line startline = CalculateHeight(doc, newStartPoint);
                    Line endline = CalculateHeight(doc, newEndPoint);

                    XYZ realStartPoint = new XYZ(newStartPoint.X, newStartPoint.Y, newStartPoint.Z + startline.Length - 1500 / 304.8);
                    XYZ realEndPoint = new XYZ(newEndPoint.X, newEndPoint.Y, newEndPoint.Z + endline.Length - 1500 / 304.8);

                    Pipe p = Pipe.Create(doc, sys, typeHDPE, level, realStartPoint, realEndPoint);
                    ChangePipeSize(p, "300");
                }

                foreach (Pipe pipe in pipesUPVC)
                {
                    Line newLine = pipe.LocationLine();
                    XYZ newStartPoint = LineExtension.StartPoint(newLine);
                    XYZ newEndPoint = LineExtension.EndPoint(newLine);

                    Line startline = CalculateHeight(doc, newStartPoint);
                    Line endline = CalculateHeight(doc, newEndPoint);

                    XYZ realStartPoint = new XYZ(newStartPoint.X, newStartPoint.Y, newStartPoint.Z + startline.Length - 1000 / 304.8);
                    XYZ realEndPoint = new XYZ(newEndPoint.X, newEndPoint.Y, newEndPoint.Z + endline.Length - 1000 / 304.8);

                    Pipe p = Pipe.Create(doc, sys, typeUPVC, level, realStartPoint, realEndPoint);
                    ChangePipeSize(p, "100");
                }

                trans.Commit();
            }

            //using (Transaction trans = new Transaction(doc, "ɾ���ܵ�ϵͳ"))
            //{
            //    trans.Start();

            //    List<ElementId> elements = new List<ElementId>();
            //    foreach (Pipe item in allPipes)
            //    {
            //        if (item.MEPSystem != null)
            //        {
            //            //elements.Add(item.MEPSystem.Id);
            //        }
            //    }

            //    if (elements.Count > 0)
            //    {
            //        //doc.Delete(elements);
            //    }

            //    trans.Commit();
            //}
            MessageBox.Show("��ˮ�����������", "��ʾ", MessageBoxButton.OK, MessageBoxImage.Information);
            tg.Assimilate();
        }

        //public void BreakPipeMethod(Document doc, Pipe pipe, XYZ point)
        //{
        //    var mep = pipe as MEPCurve;
        //    PlumbingUtils.BreakCurve(doc, mep.Id, point);
        //}
        public Line CalculateHeight(Document doc, XYZ center)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            Func<View3D, bool> isNotTemplate = v3 => !(v3.IsTemplate);
            View3D view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First<View3D>(isNotTemplate);

            FilteredElementCollector TopographCollector = new FilteredElementCollector(doc).OfClass(typeof(TopographySurface)).OfCategory(BuiltInCategory.OST_Topography);
            IList<Element> topographs = TopographCollector.ToElements();
            TopographySurface topography = topographs.ElementAt(0) as TopographySurface;

            // Project in the negative Z direction down to the floor.�ر�ע��Zֵ,���������ߵķ���,-1����,1����
            XYZ rayDirection = new XYZ(0, 0, 1);

            // Look for references to faces where the element is the floor element id.
            ReferenceIntersector referenceIntersector = new ReferenceIntersector(topography.Id, FindReferenceTarget.Mesh, view3D);
            IList<ReferenceWithContext> references = referenceIntersector.Find(center, rayDirection);

            double distance = Double.PositiveInfinity;
            XYZ intersection = null;
            foreach (ReferenceWithContext referenceWithContext in references)
            {
                Reference reference = referenceWithContext.GetReference();
                // Keep the closest matching reference (using the proximity parameter to determine closeness).
                double proximity = referenceWithContext.Proximity;
                if (proximity < distance)
                {
                    distance = proximity;
                    intersection = reference.GlobalPoint;
                }
            }
            // Create line segment from the start point and intersection point.
            Line result = Line.CreateBound(center, intersection);
            return result;
        }
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
            //��ȡ��ͼ�����н���
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

                        // �жϵ��Ƿ��ظ�
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
            //��ȡ����ռλ�����н���
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

                        // �жϵ��Ƿ��ظ�
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
        public static PipeType GetPipeType(Document doc, string profession, string pipetype)
        {
            // ��ȡ�ܵ�����
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains(profession) && ps.Name.Contains(pipetype))
                {
                    pt = ps;
                    break;
                }
            }
            return pt;
        }
        public static PipingSystemType GetPipeSystemType(Document doc, string profession, string pipesystemtype)
        {
            // ��ȡ�ܵ�ϵͳ
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = collector.ToElements();
            PipingSystemType pipesys = null;
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;
                if (ps.Name.Contains(profession) && ps.Name.Contains(pipesystemtype))
                {
                    pipesys = ps;
                    break;
                }
            }
            return pipesys;
        }
        public static void ChangePipeSize(Pipe pipe, string dn)
        {
            //�ı�ܵ��ߴ�
            Parameter diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            diameter.SetValueString(dn);
        }
        public static Level GetPipeLevel(Document doc, string Levelname)
        {
            // ��ȡ���
            Level newlevel = null;
            var levelFilter = new ElementClassFilter(typeof(Level));
            FilteredElementCollector levels = new FilteredElementCollector(doc);
            levels = levels.WherePasses(levelFilter);
            foreach (Level level in levels)
            {
                if (level.Name == Levelname)
                {
                    newlevel = level;
                    break;
                }
            }
            return newlevel;
        }
        public void StructureFamilyLoad(Document doc, string categoryName, string familyName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(familyName) && item.Name.Contains("�ṹ"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "�ṹ" + "_" + categoryName + "_" + familyName + ".rfa");
            }
        }
        public FamilySymbol WaterStructureSymbol(Document doc, string categoryName, string familyName) //����ˮ������
        {
            FilteredElementCollector waterStructureCollector = new FilteredElementCollector(doc);
            waterStructureCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel);
            List<FamilySymbol> waterStructureSymbolList = new List<FamilySymbol>();
            FamilySymbol waterStructure = null;
            string fullname = "�ṹ" + "_" + categoryName + "_" + familyName;

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

                using (Transaction trans = new Transaction(doc, "����������ˮ����"))
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
            return "����������ˮ����";
        }
    }

}
