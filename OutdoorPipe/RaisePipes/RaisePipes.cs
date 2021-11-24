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
    public class RaisePipes : IExternalCommand
    {
        public static RaisePipesForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new RaisePipesForm();
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
    public class ExecuteEventRaisePipes : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                using (Transaction trans = new Transaction(doc, "管道批量升降"))
                {
                    trans.Start();

                    RaisePipesMain(doc, uidoc);
                    trans.Commit();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "管道批量升降";
        }
        public void RaisePipesMain(Document doc, UIDocument uidoc)
        {
            Selection sel = uidoc.Selection;
            IList<Reference> refList = sel.PickObjects(ObjectType.Element, new PipeSelectionFilter(), "请选择要升降的管道");
            List<Pipe> pipeList = new List<Pipe>();
            double raiseHeight = RaisePipes.mainfrm.Height;

            if (refList.Count == 0)
            {
                TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                RaisePipesMain(doc, uidoc);
            }
            else
            {
                foreach (var item in refList)
                {
                    Pipe p = item.GetElement(doc) as Pipe;
                    pipeList.Add(p);
                }
                foreach (Pipe item in pipeList)
                {
                    RaisePipesMethod(item, raiseHeight);
                }
            }
        }
        public void RaisePipesMethod(Pipe pipe, double height)
        {
            Parameter ht = pipe.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            string raiseHeight = null;

            raiseHeight = (height + double.Parse(ht.AsValueString())).ToString();

            ht.SetValueString(raiseHeight);
        }
    }
}
