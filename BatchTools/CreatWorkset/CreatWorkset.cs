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
                Parameter proNum = pro.LookupParameter("���̴���");
                Parameter subproNum = pro.LookupParameter("�������");
                string userName = app.Application.Username;

                if (userName == "weiqixiang")
                {
                    userName = "κ����";
                }
                if (userName == "zhangwanchang")
                {
                    userName = "�����";
                }
                if (userName == "xiezhiying")
                {
                    userName = "л־Ӣ";
                }
                if (userName == "zhangping")
                {
                    userName = "��ƽ";
                }
                if (userName == "xuanyiqun")
                {
                    userName = "����Ⱥ";
                }
                if (userName == "zhangyunxia")
                {
                    userName = "����ϼ";
                }
                if (userName == "shimengting")
                {
                    userName = "ʱ����";
                }
                if (userName == "xiongzhongliang")
                {
                    userName = "������";
                }
                if (userName == "tangqinghua")
                {
                    userName = "���廪";
                }
                if (userName == "yangxue")
                {
                    userName = "��ѩ";
                }
                if (userName == "liuyiman")
                {
                    userName = "��һ��";
                }

                string name = proNum.AsString() + "_" + subproNum.AsString() + "_" + "����ˮ" + "_" + userName;
                string Bname = proNum.AsString() + "_" + subproNum.AsString() + "_" + "����" + "_";
                string Sname = proNum.AsString() + "_" + subproNum.AsString() + "_" + "�ṹ" + "_";

                if (CreatWorkset.mainfrm.Main.IsChecked == true)
                {
                    if (doc.IsWorkshared == false)
                    {
                        doc.EnableWorksharing("�����ߺ�����", name);
                    }
                }

                TransactionGroup tg = new TransactionGroup(doc, "��������ˮ������");
                tg.Start();

                using (Transaction trans = new Transaction(doc, "����������"))
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

                using (Transaction trans = new Transaction(doc, "Ԫ�ع���"))
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
                        MessageBox.Show("�������Ѵ���������","��Ϣ",MessageBoxButton.OK,MessageBoxImage.Information);
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
            return "����������";
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
            IList<HostedSweep> edges = CollectorHelper.TCollector<HostedSweep>(doc);//�ر�ע��ɢˮ����¥���Ե��������������slabedeg,��Ҫͨ���������         

            foreach (FamilyInstance item in instances)
            {
                if (item.Symbol.FamilyName.Contains("����"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in railings)
            {
                if (item.Name.Contains("����"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in walls)
            {
                if (item.Name.Contains("����") || item.Name.Contains("default_����ǽ") || item.Name.Contains("default_ѹ�͸ְ�ǽ��") || item.Name.Contains("TB"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in floors)
            {
                if (item.Name.Contains("����") || item.Name.Contains("default_�ֽ������¥��") ||
                    item.Name.Contains("TB") || item.Name.Contains("default_���Ƹ�¥��"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in roofs)
            {
                if (item.Name.Contains("����") || item.Name.Contains("default_ѹ�͸ְ�����") || item.Name.Contains("TB"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in stairs)
            {
                Parameter ps = item.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM);

                if (ps.AsValueString().Contains("����"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in edges)
            {
                SlabEdge sg = item as SlabEdge;
                if (sg.Name.Contains("����"))
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
                if (item.Symbol.FamilyName.Contains("�ṹ") || item.Symbol.FamilyName.Contains("ͨ����") ||
                    item.Symbol.FamilyName.Contains("���ž�") || item.Symbol.FamilyName.Contains("�豸����"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in walls)
            {
                if (item.Name.Contains("�ṹ"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in floors)
            {
                if (item.Name.Contains("�ṹ"))
                {
                    list.Add(item);
                }
            }

            foreach (var item in stairs)
            {
                Parameter ps = item.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM);

                if (ps.AsValueString().Contains("����"))
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
                    if (wsparam != null && wsparam.IsReadOnly == false) //�˴��޿ӣ�Ҫ�жϲ����Ƿ�Ϊֻ��
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
