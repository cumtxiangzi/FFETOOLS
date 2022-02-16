using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.InteropServices;
using Autodesk.Revit.DB.Structure;
using MessageBox = System.Windows.MessageBox;
using View = Autodesk.Revit.DB.View;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class WellPoint : IExternalCommand
    {
        public static WellPointWindow mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new WellPointWindow();
                mainfrm.ShowDialog();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }

    }
    public class ExecuteEventWellPoint : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                View view = uidoc.ActiveView;
                if (view is ViewPlan)
                {
                    CreatDraiagePipeNet(doc);
                }
                else
                {
                    TaskDialog.Show("警告", "请在平面视图中进行操作");
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
        public void CreatDraiagePipeNet(Document doc)
        {
            List<string> Wellname = WellPoint.mainfrm.Wellname;
            List<string> Xpoints = WellPoint.mainfrm.Xpoints;
            List<string> Ypoints = WellPoint.mainfrm.Ypoints;
            List<string> Zpoints = WellPoint.mainfrm.Zpoints;
            List<XYZ> wellpoints = WellPoint.mainfrm.Wellpoints;
            List<double> wellBottomValues = WellPoint.mainfrm.wellBottomValue;
            List<DataTable> results = WellPoint.mainfrm.Results;

            TransactionGroup tg = new TransactionGroup(doc, "创建室外排水管网");
            tg.Start();
            using (Transaction trans = new Transaction(doc, "生成排水井"))
            {
                trans.Start();

                StructureFamilyLoad(doc, "排水构筑物", "砖砌排水检查井");
                FamilySymbol familySymbol = WaterStructureSymbol(doc, "排水构筑物", "砖砌排水检查井");
                familySymbol.Activate();

                FamilyInstance wellinstance = null;
                for (int i = 0; i < Xpoints.Count; i++)
                {

                    wellinstance = doc.Create.NewFamilyInstance(wellpoints.ElementAt(i), familySymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
                    IList<Parameter> list = wellinstance.GetParameters("标记");
                    Parameter param = list[0];
                    param.Set(Wellname.ElementAt(i));

                    IList<Parameter> list1 = wellinstance.GetParameters("管中心高");
                    Parameter param1 = list1[0];
                    param1.Set(wellBottomValues.ElementAt(i));
                }

                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "生成管道"))
            {
                trans.Start();

                FilteredElementCollector pipetype = new FilteredElementCollector(doc);
                pipetype.OfClass(typeof(PipeType));
                IList<Element> pipetypes = pipetype.ToElements();
                PipeType pt = null;
                foreach (Element pipe in pipetypes)
                {
                    PipeType ps = pipe as PipeType;
                    if (ps.Name.Contains("给排水") && ps.Name.Contains("HDPE"))
                    {
                        pt = ps;
                        break;
                    }
                }

                FilteredElementCollector pipesystem = new FilteredElementCollector(doc);
                pipesystem.OfClass(typeof(PipingSystemType));
                IList<Element> pipesystems = pipesystem.ToElements();
                PipingSystemType pipesys = null;
                foreach (Element sys in pipesystems)
                {
                    PipingSystemType ps = sys as PipingSystemType;
                    if (ps.Name.Contains("给排水") && ps.Name.Contains("污水"))
                    {
                        pipesys = ps;
                        break;
                    }
                }

                foreach (DataTable item in results)
                {
                    List<string> pipeXpoints = WellPointWindow.DataGridVaule(item, 2);
                    List<string> pipeYpoints = WellPointWindow.DataGridVaule(item, 3);
                    List<string> pipeZpoints = WellPointWindow.DataGridVaule(item, 5);
                    List<XYZ> pipepoints = new List<XYZ>();

                    for (int i = 0; i < pipeXpoints.Count; i++)
                    {
                        pipepoints.Add(new XYZ(Convert.ToDouble(pipeYpoints.ElementAt(i)) * 3.28083989501312, Convert.ToDouble(pipeXpoints.ElementAt(i)) * 3.28083989501312,
                            Convert.ToDouble(pipeZpoints.ElementAt(i)) * 3.28083989501312));
                    }
                    for (int i = 0; i < pipeXpoints.Count - 1; i++)
                    {
                        Pipe pipe = Pipe.Create(doc, pipesys.Id, pt.Id, doc.ActiveView.GenLevel.Id, pipepoints.ElementAt(i), pipepoints.ElementAt(i + 1));
                        ChangePipeSize(pipe, "300");
                    }
                }

                trans.Commit();
            }
            tg.Assimilate();
            MessageBox.Show("排水管网生成完成", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static void ChangePipeSize(Pipe pipe, string diameter)
        {
            Parameter pdiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            pdiameter.SetValueString(diameter);
        }
        public static XYZ ChangePoint(XYZ point, double z)
        {
            double zz = z / 304.8;
            XYZ pt = new XYZ(point.X, point.Y, point.Z + zz);
            return pt;
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
}
