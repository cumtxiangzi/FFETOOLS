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
    public class FamilyManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                FamilyManagerWindow form = new FamilyManagerWindow();
                form.ShowDialog();
                if (form.ShowDialog() == true)
                {
                    using (Transaction tran = new Transaction(doc, "‘ÿ»Î◊Â"))
                    {
                        tran.Start();
                        Family family;
                        doc.LoadFamily(form.FamilyFilePath, UIDocument.GetRevitUIFamilyLoadOptions(), out family);
                        tran.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
