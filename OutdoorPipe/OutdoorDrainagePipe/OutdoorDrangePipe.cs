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
                using (Transaction trans = new Transaction(doc, "����������ˮ����"))
                {
                    trans.Start();
                    AutoCreatWells(doc, uidoc);




                    trans.Commit();
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
            IList<Reference> refList = selection.PickObjects(ObjectType.Element, new DetailineSelectionFilter(), "��ѡ����ͼ��");
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
            StructureFamilyLoad(doc, "��ˮ������", "ש����ˮ��龮");
            FamilySymbol familySymbol = WaterStructureSymbol(doc, "��ˮ������", "ש����ˮ��龮");
            familySymbol.Activate();
            FamilyInstance wellinstance = null;

            Options opts = new Options { DetailLevel = ViewDetailLevel.Fine, ComputeReferences = true };
            List<XYZ> wellPoints = new List<XYZ>();
            List<DetailLine> lineList = new List<DetailLine>();

            foreach (Reference reference in referenceList)
            {
                DetailLine detailLine = doc.GetElement(reference.ElementId) as DetailLine;
                //var geometry = detailLine.get_Geometry(opts);
                //Line centerline = geometry.First() as Line;
                lineList.Add(detailLine);
            }

            wellPoints = GetCrosspoint(lineList);
            foreach (XYZ item in wellPoints)
            {
                wellinstance = doc.Create.NewFamilyInstance(item, familySymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
            }

        }
        public List<XYZ> GetCrosspoint(List<DetailLine> lines)
        {
            //����ͼ�����н���
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

            foreach (DetailLine item in lines)
            {
                double length = Convert.ToInt32(UnitUtils.Convert(item.GeometryCurve.Length, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS));
                Line line = item.GeometryCurve as Line;
                XYZ startPoint = LineExtension.StartPoint(line);
                XYZ endPoint = LineExtension.EndPoint(line);

                if (length > 40000)
                {
                    double num = Math.Floor(length / 40000);
                    int mod = Convert.ToInt32(length % 40000);

                    if (mod == 0)
                    {

                        for (int i = 1; i < num; i++)
                        {
                            XYZ point = new XYZ(((num - i) * startPoint.X + i * endPoint.X) / num, ((num - i) * startPoint.Y + i * endPoint.Y) / num, startPoint.Z);//n�ȷֵ����깫ʽ
                            Points.Add(point);
                        }
                    }
                    else
                    {
                        for (int i = 1; i < num + 1; i++)
                        {
                            double k =num + 1;
                            XYZ point = new XYZ(((k - i) * startPoint.X + i * endPoint.X) / k, ((k - i) * startPoint.Y + i * endPoint.Y) / k, startPoint.Z);

                            Points.Add(point);
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
