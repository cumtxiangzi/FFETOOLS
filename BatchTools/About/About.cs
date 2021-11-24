using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;


namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class About : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            AboutWindow abw = new AboutWindow();
            abw.ShowDialog();

            return Result.Succeeded;
        }
    }
}
