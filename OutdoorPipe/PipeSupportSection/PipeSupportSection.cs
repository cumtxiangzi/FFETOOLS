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
    public class PipeSupportSection : IExternalCommand
    {
        public static PipeSupportSectionForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new PipeSupportSectionForm();
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
    public class ExecuteEventPipeSupportSection : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                View view = uidoc.ActiveView;
                if (view is ViewDrafting)
                {
                    CreatPipeSupportSection(doc, sel);
                }
                else
                {
                    TaskDialog.Show("警告", "请在绘制视图中进行操作");
                    PipeSupportSection.mainfrm.Show();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "管道支架剖面";
        }
        public void CreatPipeSupportSection(Document doc, Selection sel)
        {
            XYZ pickpoint = sel.PickPoint("请选择插入点");

            FamilyInstance typeC_Section = null;

            TransactionGroup tg = new TransactionGroup(doc, "创建管道支架详图");
            tg.Start();

            using (Transaction trans = new Transaction(doc, "载入支架详图族"))
            {
                trans.Start();
                DetailDrawingFamilyLoad(doc, "C型支架");

                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "布置支架详图族"))
            {
                trans.Start();
                if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    FamilySymbol typeC_SectionSymbol = null;
                    typeC_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeC_Button.Content.ToString());
                    typeC_SectionSymbol.Activate();
                    typeC_Section = doc.Create.NewFamilyInstance(pickpoint, typeC_SectionSymbol, doc.ActiveView);
                    ModifyParameter(typeC_Section);
                }
                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "修改支架详图族参数"))
            {
                trans.Start();



                trans.Commit();
            }


            tg.Assimilate();
            PipeSupportSection.mainfrm.Show();
        }
        public void DetailDrawingFamilyLoad(Document doc, string categoryName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("给排水"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_详图项目_" + categoryName + ".rfa");
            }
        }
        public FamilySymbol PipeSupportSectionSymbol(Document doc, string symbolName)
        {
            FamilySymbol symbol = null;
            FilteredElementCollector sectionCollector = new FilteredElementCollector(doc);
            sectionCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_DetailComponents);

            IList<Element> sections = sectionCollector.ToElements();
            foreach (FamilySymbol item in sections)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains(symbolName))
                {
                    symbol = item;
                    break;
                }
            }
            return symbol;
        }
        public void ModifyParameter(FamilyInstance sectionInstance)
        {
            sectionInstance.LookupParameter("支柱名称").Set(PipeSupportSection.mainfrm.WorkshopGridName.Text);
            sectionInstance.LookupParameter("支架底部标高").Set(PipeSupportSection.mainfrm.LevelValue.Text);
            sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                sectionInstance.LookupParameter("二层支架横撑显隐").Set(0);
                sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                sectionInstance.LookupParameter("二层管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层管道3可见性").Set(0);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                OneFloorPipeSection(sectionInstance);
                TwoFloorPipeSection(sectionInstance);
            }
        }
        public void OneFloorPipeSection(FamilyInstance sectionInstance)
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe2_Size = PipeSupportSection.mainfrm.OneFloorPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe3_Size = PipeSupportSection.mainfrm.OneFloorPipe3_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe4_Size = PipeSupportSection.mainfrm.OneFloorPipe4_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe1.IsChecked;
            bool oneFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe2.IsChecked;
            bool oneFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe3.IsChecked;
            bool oneFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe4.IsChecked;

            if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                if (HaveBrace(oneFloorPipe1_Size))
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(1);
                }
                else
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(0);
                }
                sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("一层管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistanceUnwarm(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistanceUnwarm(oneFloorPipe1_Size).Item1 + PipeDistanceUnwarm(oneFloorPipe1_Size).Item3).ToString());
                if (HaveBrace(oneFloorPipe1_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistanceUnwarm(oneFloorPipe1_Size).Item1 + PipeDistanceUnwarm(oneFloorPipe1_Size).Item3 - 250).ToString());
                }
            }
            else if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size))
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(1);
                }
                else
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(0);
                }
                sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistanceUnwarm(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistanceUnwarm(oneFloorPipe1_Size).Item2 / 2 + PipeDistanceUnwarm(oneFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistanceUnwarm(oneFloorPipe1_Size).Item1 + PipeDistanceUnwarm(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistanceUnwarm(oneFloorPipe2_Size).Item2 / 2 + PipeDistanceUnwarm(oneFloorPipe2_Size).Item3).ToString());
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistanceUnwarm(oneFloorPipe1_Size).Item1 + PipeDistanceUnwarm(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistanceUnwarm(oneFloorPipe2_Size).Item2 / 2 + PipeDistanceUnwarm(oneFloorPipe2_Size).Item3 - 250).ToString());
                }
            }
            else if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size))
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(1);
                }
                else
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(0);
                }
                sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                sectionInstance.LookupParameter("一层管道直径D3").SetValueString(oneFloorPipe3_Size);
                sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistanceUnwarm(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistanceUnwarm(oneFloorPipe1_Size).Item2 / 2 + PipeDistanceUnwarm(oneFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistanceUnwarm(oneFloorPipe2_Size).Item2 / 2 + PipeDistanceUnwarm(oneFloorPipe3_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistanceUnwarm(oneFloorPipe1_Size).Item1 + PipeDistanceUnwarm(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistanceUnwarm(oneFloorPipe2_Size).Item2 + PipeDistanceUnwarm(oneFloorPipe3_Size).Item2 / 2 + PipeDistanceUnwarm(oneFloorPipe3_Size).Item3).ToString());
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistanceUnwarm(oneFloorPipe1_Size).Item1 + PipeDistanceUnwarm(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistanceUnwarm(oneFloorPipe2_Size).Item2 + PipeDistanceUnwarm(oneFloorPipe3_Size).Item2 / 2 + PipeDistanceUnwarm(oneFloorPipe3_Size).Item3 - 250).ToString());
                }
            }
        }
        public void TwoFloorPipeSection(FamilyInstance sectionInstance) //二层管道设置
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe2_Size = PipeSupportSection.mainfrm.OneFloorPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe3_Size = PipeSupportSection.mainfrm.OneFloorPipe3_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe4_Size = PipeSupportSection.mainfrm.OneFloorPipe4_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe1.IsChecked;
            bool oneFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe2.IsChecked;
            bool oneFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe3.IsChecked;
            bool oneFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe4.IsChecked;

            string twoFloorPipe1_Size = PipeSupportSection.mainfrm.TwoFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorPipe2_Size = PipeSupportSection.mainfrm.TwoFloorPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorPipe3_Size = PipeSupportSection.mainfrm.TwoFloorPipe3_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorPipe4_Size = PipeSupportSection.mainfrm.TwoFloorPipe4_Size.SelectedItem.ToString().Replace("DN", "");

            bool twoFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe1.IsChecked;
            bool twoFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe2.IsChecked;
            bool twoFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe3.IsChecked;
            bool twoFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe4.IsChecked;



            if (twoFloorPipe1_Check && !twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层管道直径D1").SetValueString(twoFloorPipe1_Size);
                sectionInstance.LookupParameter("二层管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层管道3可见性").Set(0);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistanceUnwarm(twoFloorPipe1_Size).Item1.ToString());
                if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    if (!HaveBrace(oneFloorPipe1_Size) && !HaveBrace(twoFloorPipe1_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistanceUnwarm(twoFloorPipe1_Size).Item1 + PipeDistanceUnwarm(twoFloorPipe1_Size).Item3).ToString());
                    }
                }
            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层管道直径D1").SetValueString(twoFloorPipe1_Size);
                sectionInstance.LookupParameter("二层管道直径D2").SetValueString(twoFloorPipe2_Size);
                sectionInstance.LookupParameter("二层管道3可见性").Set(0);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistanceUnwarm(twoFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层管道1与管道2中心间距").SetValueString((PipeDistanceUnwarm(twoFloorPipe1_Size).Item2 / 2 + PipeDistanceUnwarm(twoFloorPipe2_Size).Item2 / 2).ToString());
            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层管道直径D1").SetValueString(twoFloorPipe1_Size);
                sectionInstance.LookupParameter("二层管道直径D2").SetValueString(twoFloorPipe2_Size);
                sectionInstance.LookupParameter("二层管道直径D3").SetValueString(twoFloorPipe3_Size);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistanceUnwarm(twoFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层管道1与管道2中心间距").SetValueString((PipeDistanceUnwarm(twoFloorPipe1_Size).Item2 / 2 + PipeDistanceUnwarm(twoFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层管道2与管道3中心间距").SetValueString((PipeDistanceUnwarm(twoFloorPipe2_Size).Item2 / 2 + PipeDistanceUnwarm(twoFloorPipe3_Size).Item2 / 2).ToString());
            }
        }
        public bool HaveBrace(string nominal_Diameter) //单管是否有斜撑
        {
            bool haveBrace = false;

            if (nominal_Diameter == "200" || nominal_Diameter == "250" || nominal_Diameter == "300"
                || nominal_Diameter == "350" || nominal_Diameter == "400" || nominal_Diameter == "450")
            {
                haveBrace = true;
            }

            return haveBrace;
        }
        public bool HaveBrace(string nominal_Diameter1, string nominal_Diameter2) //双管是否有斜撑
        {
            bool haveBrace = false;

            if (nominal_Diameter1 == "125" || nominal_Diameter1 == "150" || nominal_Diameter1 == "200"
                || nominal_Diameter1 == "250" || nominal_Diameter1 == "300" || nominal_Diameter1 == "350"
                || nominal_Diameter1 == "400" || nominal_Diameter1 == "450" || nominal_Diameter2 == "125"
                || nominal_Diameter2 == "150" || nominal_Diameter2 == "200" || nominal_Diameter2 == "250"
                || nominal_Diameter2 == "300" || nominal_Diameter2 == "350" || nominal_Diameter2 == "400"
                || nominal_Diameter2 == "450")
            {
                haveBrace = true;
            }

            return haveBrace;
        }
        public bool HaveBrace(string nominal_Diameter1, string nominal_Diameter2, string nominal_Diameter3) //三管管是否有斜撑
        {
            bool haveBrace = false;

            if (nominal_Diameter1 == "125" || nominal_Diameter1 == "150" || nominal_Diameter1 == "200"
                || nominal_Diameter1 == "250" || nominal_Diameter1 == "300" || nominal_Diameter1 == "350"
                || nominal_Diameter1 == "400" || nominal_Diameter1 == "450" || nominal_Diameter2 == "125"
                || nominal_Diameter2 == "150" || nominal_Diameter2 == "200" || nominal_Diameter2 == "250"
                || nominal_Diameter2 == "300" || nominal_Diameter2 == "350" || nominal_Diameter2 == "400"
                || nominal_Diameter2 == "450" || nominal_Diameter3 == "125" || nominal_Diameter3 == "150"
                || nominal_Diameter3 == "200" || nominal_Diameter3 == "250" || nominal_Diameter3 == "300"
                || nominal_Diameter3 == "350" || nominal_Diameter3 == "400" || nominal_Diameter3 == "450"
                || nominal_Diameter1 == "100" || nominal_Diameter2 == "100" || nominal_Diameter3 == "100")
            {
                haveBrace = true;
            }

            return haveBrace;
        }
        public Tuple<int, int, int> PipeDistanceUnwarm(string nominal_Diameter) //不保温间距
        {
            int distance1 = 0; //  距墙间距L1
            int distance2 = 0; //  管道之间间距L2
            int distance3 = 0; //  距支架边缘间距L3
            if (nominal_Diameter == "15")
            {
                distance1 = 70;
                distance2 = 100;
                distance3 = 40;
            }
            else if (nominal_Diameter == "20")
            {
                distance1 = 80;
                distance2 = 110;
                distance3 = 40;
            }
            else if (nominal_Diameter == "25")
            {
                distance1 = 80;
                distance2 = 120;
                distance3 = 50;
            }
            else if (nominal_Diameter == "32")
            {
                distance1 = 90;
                distance2 = 140;
                distance3 = 50;
            }
            else if (nominal_Diameter == "40")
            {
                distance1 = 100;
                distance2 = 150;
                distance3 = 50;
            }
            else if (nominal_Diameter == "50")
            {
                distance1 = 100;
                distance2 = 170;
                distance3 = 60;
            }
            else if (nominal_Diameter == "65")
            {
                distance1 = 110;
                distance2 = 190;
                distance3 = 70;
            }
            else if (nominal_Diameter == "80")
            {
                distance1 = 130;
                distance2 = 210;
                distance3 = 80;
            }
            else if (nominal_Diameter == "100")
            {
                distance1 = 140;
                distance2 = 240;
                distance3 = 90;
            }
            else if (nominal_Diameter == "125")
            {
                distance1 = 160;
                distance2 = 260;
                distance3 = 110;
            }
            else if (nominal_Diameter == "150")
            {
                distance1 = 170;
                distance2 = 300;
                distance3 = 120;
            }
            else if (nominal_Diameter == "200")
            {
                distance1 = 200;
                distance2 = 350;
                distance3 = 150;
            }
            else if (nominal_Diameter == "250")
            {
                distance1 = 230;
                distance2 = 410;
                distance3 = 190;
            }
            else if (nominal_Diameter == "300")
            {
                distance1 = 270;
                distance2 = 460;
                distance3 = 210;
            }
            else if (nominal_Diameter == "350")
            {
                distance1 = 300;
                distance2 = 530;
                distance3 = 240;
            }
            else if (nominal_Diameter == "400")
            {
                distance1 = 330;
                distance2 = 590;
                distance3 = 270;
            }
            else if (nominal_Diameter == "450")
            {
                distance1 = 360;
                distance2 = 650;
                distance3 = 300;
            }

            Tuple<int, int, int> tup = new Tuple<int, int, int>(distance1, distance2, distance3);
            return tup;
        }

    }
}
