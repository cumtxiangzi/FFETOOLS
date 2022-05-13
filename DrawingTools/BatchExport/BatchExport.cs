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
using System.IO;
using System.Drawing;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class BatchExport : IExternalCommand
    {
        public static BatchExportForm mainfrm;
        public static ExternalCommandData cmd;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;
                cmd = commandData;

                mainfrm = new BatchExportForm(new ExecuteEventBatchExport().GetViewSheetNameList(doc));
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();

            }
            catch (Exception)
            {
                throw;
            }
            return Result.Succeeded;
        }
    }
    public class ExecuteEventBatchExport : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                using (Transaction trans = new Transaction(doc, "批量导出"))
                {
                    trans.Start();

                    if (BatchExport.mainfrm.DWGbutton.IsChecked == true)
                    {
                        foreach (ViewSheet item in GetSelectViewSheetList(doc, BatchExport.mainfrm))
                        {
                            GetDefaultExportDWGSettings(doc);
                            ExportDWG(doc, item, "给排水导出设置");
                            //MessageBox.Show(ss.ToString());
                        }
                    }
                    if (BatchExport.mainfrm.PDFbutton.IsChecked == true)

                    {
                        foreach (ViewSheet item in GetSelectViewSheetList(doc, BatchExport.mainfrm))
                        {
                            //ExportPDF(doc,item);//暂时不做,revit2022版API已有pdf输出类
                            //MessageBox.Show(ss.ToString());
                        }
                    }

                    trans.Commit();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetName()
        {
            return "批量导出";
        }
        public List<string> GetViewSheetNameList(Document doc)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(ViewSheet)).OfCategory(BuiltInCategory.OST_Sheets);
            IList<Element> views = viewCollector.ToElements();
            List<string> viewSheetName = new List<string>();

            foreach (ViewSheet item in views)
            {
                if (item.Title.Contains("WD") && !(item.Name.Contains("材料表")) && !(item.Name.Contains("未命名")))
                {
                    viewSheetName.Add(item.Title);
                }
            }
            viewSheetName.Sort();
            return viewSheetName;
        }
        public List<ViewSheet> GetSelectViewSheetList(Document doc, BatchExportForm form)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(ViewSheet)).OfCategory(BuiltInCategory.OST_Sheets);
            IList<Element> views = viewCollector.ToElements();
            List<ViewSheet> viewSheets = new List<ViewSheet>();

            foreach (ViewSheet item in views)
            {
                foreach (string name in form.SelectDrawingNameList)
                {
                    if (item.Title.Contains(name))
                    {
                        viewSheets.Add(item);
                    }
                }
            }
            return viewSheets;
        }
        public bool ExportDWG(Document document, ViewSheet view, string setupName)
        {
            ProjectInfo pro = document.ProjectInformation;
            Parameter proNum = pro.LookupParameter("工程代号");
            Parameter proName = pro.LookupParameter("工程名称");
            Parameter subproNum = pro.LookupParameter("子项代号");
            Parameter subproName = pro.LookupParameter("子项名称");

            bool exported = false;
            // Get the predefined setups and use the one with the given name.
            IList<string> setupNames = BaseExportOptions.GetPredefinedSetupNames(document);
            foreach (string name in setupNames)
            {
                if (name.CompareTo(setupName) == 0)
                {
                    // Export using the predefined options
                    DWGExportOptions dwgOptions = DWGExportOptions.GetPredefinedOptions(document, name);
                    dwgOptions.FileVersion = ACADVersion.R2007;
                    dwgOptions.MergedViews = true;

                    // Export the active view
                    ICollection<ElementId> views = new List<ElementId>();
                    views.Add(view.Id);

                    // The document has to be saved already, therefore it has a valid PathName.
                    string drawingName = proNum.AsString() + "-" + subproNum.AsString().Replace("/", " ") + "-" + view.SheetNumber;
                    string sPath = Path.GetDirectoryName(document.PathName) + "\\DWG";
                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);//使用Directory类来创建路径
                    }
                    exported = document.Export(Path.GetDirectoryName(document.PathName) + "\\DWG", drawingName, views, dwgOptions);
                    break;
                }
            }
            return exported;
        }
        public ExportDWGSettings GetDefaultExportDWGSettings(Document doc, string defalutSetName = "给排水导出设置")
        {
            ExportDWGSettings predefs = null;
            string exportDwgSettingName = defalutSetName;
            if (ExportDWGSettings.ListNames(doc).Contains(exportDwgSettingName))
            {
                predefs = new FilteredElementCollector(doc)
                     .OfClass(typeof(ExportDWGSettings))
                     .Where(d => d.Name == exportDwgSettingName)
                     .First()
                     as ExportDWGSettings;
            }
            else
            {
                predefs = ExportDWGSettings.Create(doc, exportDwgSettingName);
            }
            return predefs;
        }

        public void ExportPDF(Document doc, ViewSheet view)
        {
            ViewSet printableViews = new ViewSet();
            if (!view.IsTemplate && view.CanBePrinted)
            {
                printableViews.Insert(view);
            }

            PrintMgr pMgr = new PrintMgr(BatchExport.cmd);
            if (null == pMgr.InstalledPrinterNames)
            {
                PrintMgr.MyMessageBox("没有安装打印机");
            }

            string PDFprinter = null;
            foreach (string item in pMgr.InstalledPrinterNames)
            {
                if (item.Contains("PDF") && item.Contains("Adobe"))
                {
                    PDFprinter = item;
                    break;
                }
            }

            PrintManager pm = doc.PrintManager;
            pm.PrintRange = PrintRange.Select;
            pm.SelectNewPrintDriver(PDFprinter);
            pm.Apply();

            // 打印全部可打印视图
            doc.Print(printableViews);
        }
        
    }
}
