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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class DesignNote : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;
                XYZ pickpoint = sel.PickPoint("��ѡ������");           

                IList<Element> col = new FilteredElementCollector(doc).OfClass(typeof(TextNoteType)).ToElements();
                TextNoteType noteType = null;
                foreach (var item in col)
                {
                    TextNoteType type = item as TextNoteType;
                    if (type.Name.Contains("3.5") && type.Name.Contains("����ˮ"))
                    {
                        noteType = type;
                        break;
                    }
                }

                using (Transaction trans = new Transaction(doc, "�������˵��"))
                {
                    trans.Start();

                    TextNote.Create(doc, doc.ActiveView.Id, pickpoint, "test"+"\n"+"���", noteType.Id);

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                messages = e.Message;
                
            }
            return Result.Succeeded;
        }
    }
}
