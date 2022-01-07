using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatWaterFamily : IExternalCommand
    {
        public static CreatWaterFamilyForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new CreatWaterFamilyForm();
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
    }
    public class ExecuteCreatWaterFamily : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
                FamilySymbol symbol = null;
                //TransactionGroup tg = new TransactionGroup(doc, "添加管道附件");
                //tg.Start();未完成
                using (Transaction trans = new Transaction(doc, "布置给排水族"))
                {
                    trans.Start();
                    symbol = CreatWaterFamilyMethod(doc, uidoc, sel);
                    symbol.Activate();

                    trans.Commit();
                }
                uidoc.PostRequestForElementTypePlacement(symbol);
                //tg.Assimilate();                
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "布置给排水族";
        }
        public FamilySymbol CreatWaterFamilyMethod(Document doc, UIDocument uidoc, Selection sel)
        {
            ValveFamilyLoad(doc, "蝶形止回阀H77X-10");
            ValveFamilyLoad(doc, "微阻缓闭式止回阀HH44X-10");
            ValveFamilyLoad(doc, "电动蝶阀D97A1X-10");
            ValveFamilyLoad(doc, "蝶阀D97A1X-10");

            FamilySymbol pipeAccessory = null;
            Pipe p = null;
            pipeAccessory = PipeAccessorySymbol(doc, "DN50", "蝶阀D37A1X");
            return pipeAccessory;
        }
        public FamilySymbol PipeAccessorySymbol(Document doc, string dn, string accessoryName)
        {
            FilteredElementCollector valveCollector = new FilteredElementCollector(doc);
            valveCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessory);
            List<FamilySymbol> valveSymbolList = new List<FamilySymbol>();
            FamilySymbol valve = null;

            IList<Element> pumps = valveCollector.ToElements();
            foreach (FamilySymbol item in pumps)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains(accessoryName))
                {
                    valveSymbolList.Add(item);
                }
            }
            foreach (FamilySymbol item in valveSymbolList)
            {
                if (item.Name.Contains(dn))
                {
                    valve = item;
                    break;
                }
            }
            return valve;
        }
        public void ValveFamilyLoad(Document doc, string categoryName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("给排水"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_阀门_" + categoryName + ".rfa");
            }
        }
        public PipeType GetPipeType(Document doc, UIDocument uidoc, string pipeType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;

                if (pipeType == "PVC")
                {
                    if (ps.Name.Contains("给排水") && ps.Name == "PVC")
                    {
                        pt = ps;
                        break;
                    }
                }

                if (ps.Name.Contains("给排水") && ps.Name.Contains(pipeType))
                {
                    pt = ps;
                    break;
                }
            }
            return pt;
        }
    }
}
