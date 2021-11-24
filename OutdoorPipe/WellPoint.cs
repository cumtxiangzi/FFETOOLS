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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class WellPoint : IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document Doc = uidoc.Document;

            ExecuteHander executeHander = new ExecuteHander("wellpoint");
            ExternalEvent externalEvent = ExternalEvent.Create(executeHander);

            WellPointWindow frm = new WellPointWindow(executeHander,externalEvent,uidoc);

            IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
            WindowInteropHelper helper = new WindowInteropHelper(frm);
            helper.Owner = rvtPtr;

            frm.Show();
            return Result.Succeeded;
        }

    }

}
