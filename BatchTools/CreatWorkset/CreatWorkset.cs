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
                if (userName == "liuyiman")
                {
                    userName = "刘一曼";
                }

                string name = proNum.AsString() + "_" + subproNum.AsString() + "_" + "给排水" + "_" + userName;
                string Bname = proNum.AsString() + "_" + subproNum.AsString() + "_" + "建筑" + "_";
                string Sname = proNum.AsString() + "_" + subproNum.AsString() + "_" + "结构" + "_";

                if (CreatWorkset.mainfrm.Main.IsChecked == true)
                {
                    if (doc.IsWorkshared == false)
                    {
                        doc.EnableWorksharing("共享标高和轴网", name);
                    }
                }

                TransactionGroup tg = new TransactionGroup(doc, "创建给排水工作集");
                tg.Start();

                using (Transaction trans = new Transaction(doc, "创建工作集"))
                {
                    trans.Start();

                    if (CreatWorkset.mainfrm.SubMain.IsChecked == true)
                    {
                        GetWorkset(doc, name);
                        doc.GetWorksetTable().SetActiveWorksetId(GetWorkset(doc, name).Id);
                    }

                    if (CreatWorkset.mainfrm.Main.IsChecked == true)
                    {
                        GetWorkset(doc, Bname);
                        GetWorkset(doc, Sname);
                        //doc.GetWorksetTable().SetActiveWorksetId(GetWorkset(doc, name).Id);
                    }

                    trans.Commit();
                }

                using (Transaction trans = new Transaction(doc, "元素归类"))
                {
                    trans.Start();

                    int number = 0;
                    if (CreatWorkset.mainfrm.Main.IsChecked == true)
                    {
                        AddElementsToWorkSet(doc, BuildingElement(doc), Bname);
                        AddElementsToWorkSet(doc, StructureElement(doc), Sname);
                        number++;
                    }

                    if (number != 0)
                    {
                        MessageBox.Show("工作集已创建并归类","消息",MessageBoxButton.OK,MessageBoxImage.Information);
                    }

                    trans.Commit();
                }

                tg.Assimilate();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "创建工作集";
        }
        public List<Element> BuildingElement(Document doc)
        {
            List<Element> list = new List<Element>();
            IList<FamilyInstance> instances = CollectorHelper.TCollector<FamilyInstance>(doc);
            IList<Railing> railings = CollectorHelper.TCollector<Railing>(doc);
            IList<Wall> walls = CollectorHelper.TCollector<Wall>(doc);
            IList<Floor> floors = CollectorHelper.TCollector<Floor>(doc);
            IList<FootPrintRoof> roofs = CollectorHelper.TCollector<FootPrintRoof>(doc);
            IList<Stairs> stairs = CollectorHelper.TCollector<Stairs>(doc);
            IList<HostedSweep> edges = CollectorHelper.TCollector<HostedSweep>(doc);//特别注意散水属于楼板边缘但过滤器不适用slabedeg,需要通过父类过滤         

            foreach (FamilyInstance item in instances)
            {
                if (item.Symbol.FamilyName.Contains("建筑"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in railings)
            {
                if (item.Name.Contains("建筑"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in walls)
            {
                if (item.Name.Contains("建筑") || item.Name.Contains("default_砌体墙") || item.Name.Contains("default_压型钢板墙面") || item.Name.Contains("TB"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in floors)
            {
                if (item.Name.Contains("建筑") || item.Name.Contains("default_钢筋混凝土楼板") ||
                    item.Name.Contains("TB") || item.Name.Contains("default_花纹钢楼板"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in roofs)
            {
                if (item.Name.Contains("建筑") || item.Name.Contains("default_压型钢板屋面") || item.Name.Contains("TB"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in stairs)
            {
                Parameter ps = item.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM);

                if (ps.AsValueString().Contains("建筑"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in edges)
            {
                SlabEdge sg = item as SlabEdge;
                if (sg.Name.Contains("建筑"))
                {
                    list.Add(item);
                }
            }

            return list;
        }
        public List<Element> StructureElement(Document doc)
        {
            List<Element> list = new List<Element>();
            IList<FamilyInstance> instances = CollectorHelper.TCollector<FamilyInstance>(doc);
            IList<Wall> walls = CollectorHelper.TCollector<Wall>(doc);
            IList<Floor> floors = CollectorHelper.TCollector<Floor>(doc);
            IList<Stairs> stairs = CollectorHelper.TCollector<Stairs>(doc);

            foreach (FamilyInstance item in instances)
            {
                if (item.Symbol.FamilyName.Contains("结构") || item.Symbol.FamilyName.Contains("通气管") ||
                    item.Symbol.FamilyName.Contains("阀门井") || item.Symbol.FamilyName.Contains("设备基础"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in walls)
            {
                if (item.Name.Contains("结构"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in floors)
            {
                if (item.Name.Contains("结构"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in stairs)
            {
                Parameter ps = item.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM);

                if (ps.AsValueString().Contains("浇筑"))
                {
                    list.Add(item);
                }
            }

            return list;
        }
        public void AddElementsToWorkSet(Document doc, List<Element> elements, string workSetName)
        {
            Workset workset = null;
            IList<Workset> worksetList = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToWorksets();
            foreach (Workset item in worksetList)
            {
                if (item.Name.Contains(workSetName))
                {
                    workset = item;
                    break;
                }
            }

            if (workset != null)
            {
                var worksetID = workset.Id.IntegerValue;

                foreach (var ele in elements)
                {
                    Parameter wsparam = ele.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                    if (wsparam != null && wsparam.IsReadOnly == false) //此处巨坑，要判断参数是否为只读
                    {
                        wsparam.Set(worksetID);
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
                            newWorkset = workset;
                        }
                    }
                }
            }
            return newWorkset;
        }
    }
}
