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
using Autodesk.Revit.DB.Visual;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatWorkset : IExternalCommand
    {
        public static CreatWorksetForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new CreatWorksetForm();
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

    public class ExecuteEventCreatWorkset : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                if (CreatWorkset.mainfrm.Main.IsChecked == true)
                {
                    if (doc.IsWorkshared == false)
                    {
                        doc.EnableWorksharing("共享标高和轴网", "工作集1");
                    }
                }

                ProjectInfo pro = doc.ProjectInformation;
                Parameter proNum = pro.LookupParameter("工程代号");
                Parameter subproNum = pro.LookupParameter("子项代号");
                string userName = app.Application.Username;

                if (userName == "weiqixiang")
                {
                    userName = "魏祺祥";
                }
                if (userName == "zhangwanchang")
                {
                    userName = "张万昌";
                }
                if (userName == "xiezhiying")
                {
                    userName = "谢志英";
                }
                if (userName == "zhangping")
                {
                    userName = "张平";
                }
                if (userName == "xuanyiqun")
                {
                    userName = "宣轶群";
                }
                if (userName == "zhangyunxia")
                {
                    userName = "张云霞";
                }
                if (userName == "shimengting")
                {
                    userName = "时梦婷";
                }
                if (userName == "xiongzhongliang")
                {
                    userName = "熊忠良";
                }
                if (userName == "tangqinghua")
                {
                    userName = "唐清华";
                }
                if (userName == "yangxue")
                {
                    userName = "杨雪";
                }

                string name = proNum.AsString() + "_" + subproNum.AsString() + "_" + "给排水" + "_" + userName;

                using (Transaction trans = new Transaction(doc, "创建工作集"))
                {
                    trans.Start();

                    if (CreatWorkset.mainfrm.SubMain.IsChecked == true)
                    {
                        GetWorkset(doc, name);
                        doc.GetWorksetTable().SetActiveWorksetId(GetWorkset(doc, name).Id);
                    }
                    trans.Commit();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "创建工作集";
        }
        public void AddElementsToWorkSet(Document doc, List<Element> elements, string workSetName)
        {
            if (doc.IsWorkshared == true)
            {
                var workset = GetWorkset(doc, workSetName);
                if (workset != null)
                {
                    var worksetID = workset.Id.IntegerValue;
                    foreach (var ele in elements)
                    {
                        Parameter wsparam = ele.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                        if (wsparam != null)
                        {
                            wsparam.Set(worksetID);
                        }
                    }
                }
            }
        }
        public Workset GetWorkset(Document doc, string workSetName)
        {
            Workset newWorkset = null;
            // Worksets can only be created in a document with worksharing enabled
            if (doc.IsWorkshared)
            {
                string worksetName = workSetName;
                // Workset name must not be in use by another workset
                if (WorksetTable.IsWorksetNameUnique(doc, worksetName))
                {
                    newWorkset = Workset.Create(doc, worksetName);
                }
                else
                {
                    IList<Workset> worksetList = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToWorksets();
                    foreach (Workset workset in worksetList)
                    {
                        if (workset.Name.Contains(worksetName))
                        {
                            return workset;
                        }
                    }
                }
            }
            return newWorkset;
        }
    }
}
