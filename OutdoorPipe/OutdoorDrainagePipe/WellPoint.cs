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

                List<string> Wellname = WellPoint.mainfrm.Wellname;
                List<string> Xpoints = WellPoint.mainfrm.Xpoints;
                List<string> Ypoints = WellPoint.mainfrm.Ypoints;
                List<string> Zpoints = WellPoint.mainfrm.Zpoints;
                List<XYZ> wellpoints = WellPoint.mainfrm.Wellpoints;

                using (Transaction trans = new Transaction(doc, "生成排水井"))
                {
                    trans.Start();

                    StructureFamilyLoad(doc, "排水构筑物", "砖砌排水检查井");               
                    FamilySymbol familySymbol = WaterStructureSymbol(doc, "排水构筑物", "砖砌排水检查井");
                    familySymbol.Activate();
                    //MessageBox.Show("ss");

                    FamilyInstance wellinstance = null;
                    // wellinstance = doc.Create.NewFamilyInstance(new XYZ(), familySymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);


                    for (int i = 0; i < Xpoints.Count; i++)
                    {

                        wellinstance = doc.Create.NewFamilyInstance(wellpoints.ElementAt(i), familySymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
                        IList<Parameter> list = wellinstance.GetParameters("标记");
                        Parameter param = list[0];
                        param.Set(Wellname.ElementAt(i));
                    }

                    trans.Commit();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "生成排水管及排水井";
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
