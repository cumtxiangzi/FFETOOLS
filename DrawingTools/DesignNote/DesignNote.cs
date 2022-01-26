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
using System.Collections.ObjectModel;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class DesignNote : IExternalCommand
    {
        public static DesignNoteForm mainfrm;
        public static Document newdoc;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Application app = uiapp.Application;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                string filepath = @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\给排水项目标准.rvt";
                newdoc = doc.Application.OpenDocumentFile(filepath);

                mainfrm = new DesignNoteForm();
                PreviewControlModel control = new PreviewControlModel(mainfrm, newdoc);
                control.Window.ShowDialog();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }

    }
    public class ExecuteEventDesignNote : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                CreatDesigNote(doc, DesignNote.newdoc, DesignNote.mainfrm.WorkShopText.Text);
                DesignNote.mainfrm.Close();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "给排水设计说明";
        }
        public void CreatDesigNote(Document doc, Document newdoc, string workShopName)
        {
            FilteredElementCollector fec = new FilteredElementCollector(newdoc);
            fec.OfCategory(BuiltInCategory.OST_Views);
            IList<Element> fecList = fec.ToElements();
            ICollection<ViewDrafting> copyIds = new Collection<ViewDrafting>();
            foreach (View v in fecList)
            {
                if (v.Name.Contains(workShopName))
                {
                    copyIds.Add(v as ViewDrafting);
                    break;
                }
            }
            DuplicateViewUtils.DuplicateDraftingViews(newdoc, copyIds, doc);
        }
    }
}
