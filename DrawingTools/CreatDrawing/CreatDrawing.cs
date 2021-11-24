using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Selection;
using System.Diagnostics;
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatDrawing : IExternalCommand
    {
        public static CreatDrawings mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                DrawingFamilyLoad(doc, "A0");
                DrawingFamilyLoad(doc, "A1");
                DrawingFamilyLoad(doc, "A2");
                DrawingFamilyLoad(doc, "A3");
                DrawingFamilyLoad(doc, "版号栏");
                DrawingFamilyLoad(doc, "图签");

                FilteredElementCollector DrawingCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks);
                IList<Element> drawings = DrawingCollector.ToElements();
                List<string> DrawingType = new List<string>();
                foreach (Element elm in DrawingCollector)
                {
                    FamilySymbol drw = elm as FamilySymbol;
                    if (drw.FamilyName.Contains("A1")|| drw.FamilyName.Contains("A2")||drw.FamilyName.Contains("A3")|| drw.FamilyName.Contains("A0"))
                    {
                        DrawingType.Add(drw.FamilyName + "：" + drw.Name);
                    }
                }
                DrawingType.Sort();

                List<Element> noTitleType = new List<Element>();
                foreach (Element elm in DrawingCollector)
                {
                    FamilySymbol drw = elm as FamilySymbol;
                    if (drw.FamilyName.Contains("共用_图纸"))
                    {
                        noTitleType.Add(drw);
                    }
                }

                mainfrm = new CreatDrawings(DrawingType);
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
        public void DrawingFamilyLoad(Document doc, string categoryName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("共用_图纸"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                using (Transaction transaction = new Transaction(doc, "批量创建图纸"))
                {
                    transaction.Start();
                    doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "共用_图纸_" + categoryName + ".rfa");
                    transaction.Commit();
                }
            }
        }
    }
    public class ExecuteEventCreatDrawing : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;             

                using (Transaction transaction = new Transaction(doc, "批量创建图纸"))
                {
                    transaction.Start();
                    int num = ExistingDrawingNumber(doc, CreatDrawing.mainfrm.drawingMajorName);
                    if (CreatDrawing.mainfrm.type.Contains("共用_图纸_A1"))
                    {
                        MainMethod(doc, "共用_图纸_A1", CreatDrawing.mainfrm);
                    }
                    else if (CreatDrawing.mainfrm.type.Contains("共用_图纸_A0"))
                    {
                        MainMethod(doc, "共用_图纸_A0", CreatDrawing.mainfrm);
                    }
                    else if (CreatDrawing.mainfrm.type.Contains("共用_图纸_A2"))
                    {
                        MainMethod(doc, "共用_图纸_A2", CreatDrawing.mainfrm);
                    }
                    else if (CreatDrawing.mainfrm.type.Contains("共用_图纸_A3"))
                    {
                        MainMethod(doc, "共用_图纸_A3", CreatDrawing.mainfrm);
                    }
                    else
                    {                       
                        for (int i = 0; i < CreatDrawing.mainfrm.number; i++)
                        {
                            CreateDrawingView(doc, CreatDrawing.mainfrm.type, i + num + 1, CreatDrawing.mainfrm.drawingMajorName);
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "创建图纸";
        }
        private ViewSheet CreateDrawingView(Document document, string typeName, int number, String name)
        {
            IEnumerable<FamilySymbol> familyList = from elem in new FilteredElementCollector(document).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                   let type = elem as FamilySymbol
                                                   where (type.FamilyName + "：" + type.Name).Equals(typeName)
                                                   select type;

            ViewSheet viewSheet = ViewSheet.Create(document, familyList.First().Id);
            viewSheet.SheetNumber = name + "-" + number.ToString().PadLeft(3, '0');
            if (null == viewSheet)
            {
                throw new Exception("创建图纸失败");
            }
            return viewSheet;
        }

        private int ExistingDrawingNumber(Document doc, string name)
        {
            FilteredElementCollector existingDrawing = new FilteredElementCollector(doc);
            existingDrawing.OfClass(typeof(ViewSheet));
            IList<Element> existingDrawings = existingDrawing.ToElements();
            List<ViewSheet> existingDrawingsc = new List<ViewSheet>();
            foreach (Element elm in existingDrawings)
            {
                ViewSheet drw = elm as ViewSheet;
                if (drw.SheetNumber.Contains(name))
                {
                    existingDrawingsc.Add(drw);
                }

            }
            return existingDrawingsc.Count;
        }

        private FamilySymbol TitleBlock(Document doc)
        {
            FilteredElementCollector titleCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks);
            IList<Element> titles = titleCollect.ToElements();
            FamilySymbol title = null;
            foreach (Element elm in titles)
            {
                FamilySymbol tl = elm as FamilySymbol;
                if (tl.FamilyName.Contains("共用_图纸_图签"))
                {
                    title = tl;
                    break;
                }
            }          
            return title;
        }

        private FamilySymbol Edition(Document doc)
        {
            FilteredElementCollector editionCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks);
            IList<Element> editions = editionCollect.ToElements();
            FamilySymbol edition = null;
            foreach (Element elm in editions)
            {
                FamilySymbol ed = elm as FamilySymbol;
                if (ed.FamilyName.Contains("共用_图纸_版号"))
                {
                    edition = ed;
                    break;
                }
            }         
            return edition;
        }
        private void MainMethod(Document doc, string name, CreatDrawings crwForm)
        {
            int num = ExistingDrawingNumber(doc, crwForm.drawingMajorName);
            XYZ titlelocation = new XYZ();
            XYZ editionlocation = new XYZ();

            ViewSheet vws = null;
            FamilySymbol title = null;
            FamilySymbol edition = null;
            title = TitleBlock(doc);
            edition = Edition(doc);

            if (crwForm.type.Contains("共用_图纸_A1"))
            {
                titlelocation = new XYZ(1.88748115805841, 4.4597850267791, 0);
                editionlocation = new XYZ(1.88748115805841, 4.60742282205469, 0);
            }
            if (crwForm.type.Contains("共用_图纸_A0"))
            {
                titlelocation = new XYZ(3.02921344152298, 4.4597850267791, 0);
                editionlocation = new XYZ(3.02921344152298, 4.60742282205469, 0);
            }
            if (crwForm.type.Contains("共用_图纸_A2"))
            {
                titlelocation = new XYZ(1.07710058063059, 4.4597850267791, 0);
                editionlocation = new XYZ(1.07710058063059, 4.60742282205469, 0);
            }
            if (crwForm.type.Contains("共用_图纸_A3"))
            {
                titlelocation = new XYZ(0.52265176173295, 4.4597850267791, 0);
                editionlocation = new XYZ(0.52265176173295, 4.60742282205469, 0);
            }

            for (int i = 0; i < crwForm.number; i++)
            {
                vws = CreateDrawingView(doc, crwForm.type, i + num + 1, crwForm.drawingMajorName);
                FamilyInstance titlein = doc.Create.NewFamilyInstance(titlelocation, title, vws);
                FamilyInstance editionin = doc.Create.NewFamilyInstance(editionlocation, edition, vws);
                if (crwForm.CH_Button.IsChecked == true)
                {
                    titlein.LookupParameter("中文").Set(1);
                    editionin.LookupParameter("中文").Set(1);
                    titlein.LookupParameter("英文").Set(0);
                    editionin.LookupParameter("英文").Set(0);
                    titlein.LookupParameter("中英文").Set(0);
                    editionin.LookupParameter("中英文").Set(0);
                    titlein.LookupParameter("比例可见").Set(0);
                }

                if (crwForm.EN_Button.IsChecked == true)
                {
                    titlein.LookupParameter("中文").Set(0);
                    editionin.LookupParameter("中文").Set(0);
                    titlein.LookupParameter("英文").Set(1);
                    editionin.LookupParameter("英文").Set(1);
                    titlein.LookupParameter("中英文").Set(0);
                    editionin.LookupParameter("中英文").Set(0);
                    titlein.LookupParameter("比例可见").Set(0);
                }

                if (crwForm.CH_EN_Button.IsChecked == true)
                {
                    titlein.LookupParameter("中文").Set(0);
                    editionin.LookupParameter("中文").Set(0);
                    titlein.LookupParameter("英文").Set(0);
                    editionin.LookupParameter("英文").Set(0);
                    titlein.LookupParameter("中英文").Set(1);
                    editionin.LookupParameter("中英文").Set(1);
                    titlein.LookupParameter("比例可见").Set(0);
                }
            }
        }
        
    }
}
