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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatTextNote : IExternalCommand //创建文字示例
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                using (Transaction trans = new Transaction(doc, "name"))
                {
                    trans.Start();

                    AddNewTextNote(uidoc);

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        public TextNote AddNewTextNote(UIDocument uiDoc)
        {
            Document doc = uiDoc.Document;
            XYZ textLoc = uiDoc.Selection.PickPoint("Pick a point for sample text.");
            ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            double noteWidth = .2;

            // make sure note width works for the text type
            double minWidth = TextNote.GetMinimumAllowedWidth(doc, defaultTextTypeId);
            double maxWidth = TextNote.GetMaximumAllowedWidth(doc, defaultTextTypeId);
            if (noteWidth < minWidth)
            {
                noteWidth = minWidth;
            }
            else if (noteWidth > maxWidth)
            {
                noteWidth = maxWidth;
            }

            TextNoteOptions opts = new TextNoteOptions(defaultTextTypeId);
            opts.HorizontalAlignment = HorizontalTextAlignment.Left;
            opts.Rotation = Math.PI / 4;

            TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, textLoc, noteWidth, "New sample text", opts);

            return textNote;
        }

    }
}
