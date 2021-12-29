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
    public class PipeSupportSection : IExternalCommand
    {
        public static PipeSupportSectionForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new PipeSupportSectionForm();
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
    public class ExecuteEventPipeSupportSection : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                View view = uidoc.ActiveView;
                if (view is ViewDrafting)
                {
                    CreatPipeSupportSection(doc, sel);
                }
                else
                {
                    TaskDialog.Show("警告", "请在绘制视图中进行操作");
                    PipeSupportSection.mainfrm.Show();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "管道支架剖面";
        }
        public void CreatPipeSupportSection(Document doc, Selection sel)
        {
            XYZ pickpoint = sel.PickPoint("请选择插入点");

           

            TransactionGroup tg = new TransactionGroup(doc, "创建管道支架详图");
            tg.Start();

            using (Transaction trans = new Transaction(doc, "载入支架详图族"))
            {
                trans.Start();
                DetailDrawingFamilyLoad(doc,"C型支架");

                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "布置支架详图族"))
            {
                trans.Start();
               
                if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked==true)
                {
                    FamilyInstance typeC_Section = null;
                    FamilySymbol typeC_SectionSymbol = null;
                    typeC_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeC_Button.Content.ToString());
                    typeC_SectionSymbol.Activate();
                    //ModifyPumpParameter(pumpSymbol);
                    typeC_Section = doc.Create.NewFamilyInstance(pickpoint, typeC_SectionSymbol, doc.ActiveView);
                }
               
               

                trans.Commit();
            }


            tg.Assimilate();
            PipeSupportSection.mainfrm.Show();
        }
        public void DetailDrawingFamilyLoad(Document doc, string categoryName)
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
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_详图项目_" + categoryName + ".rfa");
            }
        }    
        public FamilySymbol PipeSupportSectionSymbol(Document doc, string symbolName)
        {
            FamilySymbol symbol = null;
            FilteredElementCollector sectionCollector = new FilteredElementCollector(doc);
            sectionCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_DetailComponents);

            IList<Element> sections = sectionCollector.ToElements();
            foreach (FamilySymbol item in sections)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains(symbolName))
                {
                    symbol=item;
                    break;
                }
            }
            return symbol;
        }
    }
}
