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
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //Autodesk.Revit.Exceptions.OperationCanceledException
            }
        }
        public string GetName()
        {
            return "管道支架剖面";
        }
        public void CreatPipeSupportSection(Document doc, Selection sel)
        {
            XYZ pickpoint = sel.PickPoint("请选择插入点");

            FamilyInstance typeA_Section = null;
            FamilyInstance typeB_Section = null;
            FamilyInstance typeC_Section = null;
            FamilyInstance typeC_Section_UpThree = null;
            FamilyInstance typeD_Section = null;
            FamilyInstance typeE_Section = null;
            FamilyInstance typeF_Section = null;

            TransactionGroup tg = new TransactionGroup(doc, "创建管道支架详图");
            tg.Start();

            using (Transaction trans = new Transaction(doc, "载入支架详图族"))
            {
                trans.Start();

                if (PipeSupportSection.mainfrm.TypeA_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "A型支架");
                }
                else if (PipeSupportSection.mainfrm.TypeB_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "B型支架");
                }
                else if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "C型支架");
                    DetailDrawingFamilyLoad(doc, "C型支架三四层");
                }
                else if (PipeSupportSection.mainfrm.TypeD_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "D型支架");
                }
                else if (PipeSupportSection.mainfrm.TypeE_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "E型支架");
                }
                else if (PipeSupportSection.mainfrm.TypeF_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "F型支架");
                }

                DetailDrawingTitleLoad(doc, "图名");
                DetailDrawingTitleLoad(doc, "支架剖面管道标注");

                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "布置支架详图族"))
            {
                trans.Start();

                if (PipeSupportSection.mainfrm.TypeA_Button.IsChecked == true)
                {
                    FamilySymbol typeA_SectionSymbol = null;
                    typeA_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeA_Button.Content.ToString());
                    typeA_SectionSymbol.Activate();
                    typeA_Section = doc.Create.NewFamilyInstance(pickpoint, typeA_SectionSymbol, doc.ActiveView);
                    TypeA_ModifyParameter(typeA_Section);
                }
                else if (PipeSupportSection.mainfrm.TypeB_Button.IsChecked == true)
                {
                    FamilySymbol typeB_SectionSymbol = null;
                    typeB_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeB_Button.Content.ToString());
                    typeB_SectionSymbol.Activate();
                    typeB_Section = doc.Create.NewFamilyInstance(pickpoint, typeB_SectionSymbol, doc.ActiveView);
                    TypeB_ModifyParameter(typeB_Section);
                }
                else if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true || PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        FamilySymbol typeC_SectionSymbol = null;
                        typeC_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeC_Button.Content.ToString());
                        typeC_SectionSymbol.Activate();
                        typeC_Section = doc.Create.NewFamilyInstance(pickpoint, typeC_SectionSymbol, doc.ActiveView);
                        TypeC_ModifyParameter(typeC_Section);
                    }
                    else
                    {
                        FamilySymbol typeC_Section_UpThreeSymbol = null;
                        typeC_Section_UpThreeSymbol = PipeSupportSectionSymbol(doc, "C型支架三四层");
                        typeC_Section_UpThreeSymbol.Activate();
                        typeC_Section_UpThree = doc.Create.NewFamilyInstance(pickpoint, typeC_Section_UpThreeSymbol, doc.ActiveView);
                    }
                }
                else if (PipeSupportSection.mainfrm.TypeD_Button.IsChecked == true)
                {
                    FamilySymbol typeD_SectionSymbol = null;
                    typeD_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeD_Button.Content.ToString());
                    typeD_SectionSymbol.Activate();
                    typeD_Section = doc.Create.NewFamilyInstance(pickpoint, typeD_SectionSymbol, doc.ActiveView);
                    TypeD_ModifyParameter(typeD_Section);
                }
                else if (PipeSupportSection.mainfrm.TypeE_Button.IsChecked == true)
                {
                    FamilySymbol typeE_SectionSymbol = null;
                    typeE_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeE_Button.Content.ToString());
                    typeE_SectionSymbol.Activate();
                    typeE_Section = doc.Create.NewFamilyInstance(pickpoint, typeE_SectionSymbol, doc.ActiveView);
                    //ModifyParameter(typeE_Section);
                }
                else if (PipeSupportSection.mainfrm.TypeF_Button.IsChecked == true)
                {
                    FamilySymbol typeF_SectionSymbol = null;
                    typeF_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeF_Button.Content.ToString());
                    typeF_SectionSymbol.Activate();
                    typeF_Section = doc.Create.NewFamilyInstance(pickpoint, typeF_SectionSymbol, doc.ActiveView);
                    //ModifyParameter(typeF_Section);
                }

                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "创建尺寸标注"))
            {
                trans.Start();
                if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
                    {
                        double height = MaximumDiameter(doc, typeC_Section, doc.ActiveView, "一层管道");
                        XYZ dimPosition = new XYZ(pickpoint.X, pickpoint.Y + height + 100 / 304.8, pickpoint.Z);
                        TypeC_CreatDimensionX(doc, typeC_Section, "一层支架边界线", "一层管道中心线", dimPosition);
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        double height1 = MaximumDiameter(doc, typeC_Section, doc.ActiveView, "一层管道");
                        typeC_Section.LookupParameter("一层支架净高H1").SetValueString((height1 * 304.8 + 150).ToString());

                        XYZ dimPosition1 = new XYZ(pickpoint.X, pickpoint.Y - 250 / 304.8, pickpoint.Z);
                        TypeC_CreatDimensionX(doc, typeC_Section, "一层支架边界线", "一层管道中心线", dimPosition1);

                        double height2 = MaximumDiameter(doc, typeC_Section, doc.ActiveView, "二层管道");
                        XYZ dimPosition2 = new XYZ(pickpoint.X, pickpoint.Y + height1 + height2 + 600 / 304.8, pickpoint.Z); //此处有BUG，小直径管道尺寸线位置不准确
                        TypeC_CreatDimensionX(doc, typeC_Section, "二层支架边界线", "二层管道中心线", dimPosition2);

                        double width = 0;
                        double width1 = typeC_Section.LookupParameter("一层支架长L1").AsDouble();
                        double width2 = typeC_Section.LookupParameter("二层支架长L2").AsDouble();
                        if (width1 > width2)
                        {
                            width = width1;
                        }
                        else
                        {
                            width = width2;
                        }
                        XYZ dimPosition3 = new XYZ(pickpoint.X + width + 100 / 304.8, pickpoint.Y, pickpoint.Z);
                        TypeC_CreatDimensionY(doc, typeC_Section, "一层支架边界线", dimPosition3);
                    }
                }
                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "创建图名及管道信息标注"))
            {
                trans.Start();
                if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true || PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        TypeC_CreatTitle(doc, pickpoint, typeC_Section);
                    }

                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
                    {
                        TypeC_CreatOneFloorPipeNote(doc, typeC_Section);
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        TypeC_CreatOneFloorPipeNote(doc, typeC_Section);
                        TypeC_CreatTwoFloorPipeNote(doc, typeC_Section);
                    }
                }

                trans.Commit();
            }

            tg.Assimilate();

            PipeSupportSection.mainfrm.SupportCode.Text = PipeSupportSection.mainfrm.name.Insert(1, PipeSupportSection.mainfrm.clickNum.ToString());
            PipeSupportSection.mainfrm.Show();
        }
        public void TypeA_ModifyParameter(FamilyInstance sectionInstance) //A型支架修改参数
        {
            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.CableTray.IsChecked == false)
                {
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(0);
                    sectionInstance.LookupParameter("一层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("三层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层净高H").SetValueString("-150");
                }
                else
                {
                    sectionInstance.LookupParameter("一层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                }

                TypeA_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.CableTray.IsChecked == false)
                {
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(0);
                    sectionInstance.LookupParameter("二层净高H").SetValueString("-150");
                }
                else
                {
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(1);
                }

                TypeA_TwoFloorPipeSection(sectionInstance);
            }

        }
        public void TypeA_OneFloorPipeSection(FamilyInstance sectionInstance) //A型支架修改一层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            if (!cableTray_Check)
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("支架净长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 * 2).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLength.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                         PipeDistance(oneFloorRightPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLenght.ToString());
                }

                if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item3 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLenght.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                       PipeDistance(oneFloorRightPipe2_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLength.ToString());
                }
            }
            else
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("支架净长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2).ToString());
                    sectionInstance.LookupParameter("二层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLength.ToString());
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                         PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLenght.ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLenght.ToString());
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                       PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLength.ToString());
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }
            }
        }
        public void TypeA_TwoFloorPipeSection(FamilyInstance sectionInstance) //A型支架修改二层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            if (!cableTray_Check)
            {
                int oneFloorLenght = 1000;
                int twoFloorLenght = 1000;

                //一层管道参数修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                    sectionInstance.LookupParameter("一层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    oneFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                         PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    oneFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    oneFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                       PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    oneFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                //二层管道参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    twoFloorLenght = PipeDistance(twoFloorLeftPipe1_Size).Item3 * 2;
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorRightPipe1_Size).Item3 +
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());

                    twoFloorLenght = totalLength;
                }

                if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                         PipeDistance(twoFloorRightPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3
                        - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保二层左侧管道1、二层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    twoFloorLenght = totalLenght;
                }

                if (twoFloorRightPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item3 +
                         PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3
                        - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保二层右侧管道1、二层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    twoFloorLenght = totalLenght;
                }

                if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                       PipeDistance(twoFloorRightPipe2_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 -
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 -
                        PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    twoFloorLenght = totalLength;
                }

                if (twoFloorLenght > oneFloorLenght)
                {
                    sectionInstance.LookupParameter("支架净长度L").SetValueString(twoFloorLenght.ToString());
                }
                else
                {
                    sectionInstance.LookupParameter("支架净长度L").SetValueString(oneFloorLenght.ToString());
                }
            }
            else
            {
                int oneFloorLenght = 1000;
                int twoFloorLenght = 1000;

                //一层管道参数修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                    sectionInstance.LookupParameter("一层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    oneFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                         PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    oneFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    oneFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                       PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    oneFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                //二层管道参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    twoFloorLenght = PipeDistance(twoFloorLeftPipe1_Size).Item3 * 2;
                    sectionInstance.LookupParameter("二层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(twoFloorLeftPipe1_Size)).ToString());
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorRightPipe1_Size).Item3 +
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());

                    twoFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());

                }

                if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                         PipeDistance(twoFloorRightPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3
                        - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保二层左侧管道1、二层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    twoFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());

                }

                if (twoFloorRightPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item3 +
                         PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3
                        - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保二层右侧管道1、二层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    twoFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());

                }

                if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                       PipeDistance(twoFloorRightPipe2_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 -
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 -
                        PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    twoFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }

                if (twoFloorLenght > oneFloorLenght)
                {
                    sectionInstance.LookupParameter("支架净长度L").SetValueString(twoFloorLenght.ToString());
                }
                else
                {
                    sectionInstance.LookupParameter("支架净长度L").SetValueString(oneFloorLenght.ToString());
                }
            }
        }
        public void TypeB_ModifyParameter(FamilyInstance sectionInstance) //B型支架修改参数
        {
            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.CableTray.IsChecked == false)
                {
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                }
                else
                {
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("三层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(1);
                }

                TypeB_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.CableTray.IsChecked == false)
                {
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                }
                else
                {
                    sectionInstance.LookupParameter("三层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("三层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(1);
                }

                TypeB_TwoFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true)
            {
                TypeB_ThreeFloorPipeSection(sectionInstance);
            }
        }
        public void TypeB_OneFloorPipeSection(FamilyInstance sectionInstance) //B型支架修改一层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            if (!cableTray_Check)
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 * 2).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                         PipeDistance(oneFloorRightPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
                }

                if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item3 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                       PipeDistance(oneFloorRightPipe2_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
                }
            }
            else
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorRightPipe1_Size)).ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                      + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                     + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
            }

        }
        public void TypeB_TwoFloorPipeSection(FamilyInstance sectionInstance) //B型支架修改二层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            if (!cableTray_Check)
            {
                //二层管道参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3 * 2).ToString());
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorRightPipe1_Size).Item3 +
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
                }

                if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                         PipeDistance(twoFloorRightPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3
                        - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
                }

                if (twoFloorRightPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item3 +
                         PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3
                        - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
                }

                if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                       PipeDistance(twoFloorRightPipe2_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 -
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 -
                        PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
                }

                //一层管道参数修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                      + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                }
                if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                     + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }
            }
            else
            {
                //二层管道参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                }
                if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3 +
                        PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                        + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3
                        + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                      + PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());
                }
                if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe2_Size).Item3
                        + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                        + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                     + PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());
                }
                if (twoFloorRightPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe2_Size).Item3
                        + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3
                     + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                }

                if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                        + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                     + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                //一层管道参数修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                      + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                }
                if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                     + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                }

                if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }
            }

        }
        public void TypeB_ThreeFloorPipeSection(FamilyInstance sectionInstance) //B型支架修改三层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorLeftPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorLeftPipe2_Size = PipeSupportSection.mainfrm.ThreeFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorRightPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorRightPipe2_Size = PipeSupportSection.mainfrm.ThreeFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;
            bool threeFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorLeftPipe1.IsChecked;
            bool threeFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorLeftPipe2.IsChecked;
            bool threeFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorRightPipe1.IsChecked;
            bool threeFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorRightPipe2.IsChecked;

            //三层管道参数修改
            if (threeFloorLeftPipe1_Check && !threeFloorLeftPipe2_Check && !threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                sectionInstance.LookupParameter("三层横撑长度L").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item3 * 2).ToString());
            }

            if (threeFloorLeftPipe1_Check && !threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(threeFloorRightPipe1_Size);
                sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(threeFloorLeftPipe1_Size).Item3 + PipeDistance(threeFloorRightPipe1_Size).Item3 +
                    PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(threeFloorLeftPipe1_Size).Item3).ToString());
                sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(threeFloorRightPipe1_Size).Item3).ToString());

                sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
            }

            if (threeFloorRightPipe1_Check && threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(threeFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(threeFloorRightPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(threeFloorLeftPipe2_Size).Item3 +
                     PipeDistance(threeFloorRightPipe1_Size).Item3 + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item3 - PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item3
                    - PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(threeFloorRightPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item3 - PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(threeFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
            }

            if (threeFloorRightPipe1_Check && !threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(threeFloorRightPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(threeFloorRightPipe2_Size);
                sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(threeFloorRightPipe2_Size).Item3 +
                     PipeDistance(threeFloorLeftPipe1_Size).Item3 + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(threeFloorRightPipe2_Size).Item3 - PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(threeFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(threeFloorRightPipe2_Size).Item3
                    - PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 - PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(threeFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(threeFloorRightPipe2_Size).Item3 - PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(threeFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(threeFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
            }

            if (threeFloorRightPipe1_Check && threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(threeFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(threeFloorRightPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(threeFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(threeFloorLeftPipe2_Size).Item3 +
                   PipeDistance(threeFloorRightPipe2_Size).Item3 + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(threeFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item3 -
                    PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(threeFloorRightPipe2_Size).Item3 -
                    PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 - PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
            }


            //二层管道参数修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
            }
            if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3 +
                    PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                    + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
            }
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3
                    + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                  + PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());
            }
            if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe2_Size).Item3
                    + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
            }

            if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                    + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                 + PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());
            }
            if (twoFloorRightPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe2_Size).Item3
                    + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3
                 + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
            }

            if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                    + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                 + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
            }

            //一层管道参数修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
            }
            if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3 +
                    PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                    + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
            }
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                    + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                  + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
            }
            if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                    + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
            }

            if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                    + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                 + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
            }
            if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                    + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
            }

            if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                    + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                 + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
            }


        }
        public void TypeC_ModifyParameter(FamilyInstance sectionInstance) //C型支架修改参数
        {
            sectionInstance.LookupParameter("支柱名称").Set(PipeSupportSection.mainfrm.WorkshopGridName.Text);
            sectionInstance.LookupParameter("支架底部标高").Set(PipeSupportSection.mainfrm.LevelValue.Text);
            sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
            sectionInstance.LookupParameter("支柱名称下划线长度L").SetValueString((PipeSupportSection.mainfrm.WorkshopGridName.Text.Length * 3.57 + 10).ToString());

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                sectionInstance.LookupParameter("二层支架横撑显隐").Set(0);
                sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                sectionInstance.LookupParameter("二层管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层管道3可见性").Set(0);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                TypeC_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                TypeC_TwoFloorPipeSection(sectionInstance);
            }
        }
        public void TypeC_OneFloorPipeSection(FamilyInstance sectionInstance) //C型支架修改一层管道参数
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
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
                if (HaveBrace(oneFloorPipe1_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 250).ToString());
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
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                             PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 250).ToString());
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
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                             PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 250).ToString());
                }
            }
        }
        public void TypeC_TwoFloorPipeSection(FamilyInstance sectionInstance) //C型支架修改二层管道参数
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
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistance(twoFloorPipe1_Size).Item1.ToString());

                if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());

                    if (HaveBrace(oneFloorPipe1_Size) || HaveBrace(twoFloorPipe1_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size) || HaveBrace(twoFloorPipe1_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道直径D3").SetValueString(oneFloorPipe3_Size);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size) || HaveBrace(twoFloorPipe1_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());

                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
                    }
                }
            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层管道直径D1").SetValueString(twoFloorPipe1_Size);
                sectionInstance.LookupParameter("二层管道直径D2").SetValueString(twoFloorPipe2_Size);
                sectionInstance.LookupParameter("二层管道3可见性").Set(0);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistance(twoFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层管道1与管道2中心间距").SetValueString((PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2).ToString());

                if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());

                    if (HaveBrace(oneFloorPipe1_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道直径D3").SetValueString(oneFloorPipe3_Size);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());

                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3).ToString());
                    }
                }

            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层管道直径D1").SetValueString(twoFloorPipe1_Size);
                sectionInstance.LookupParameter("二层管道直径D2").SetValueString(twoFloorPipe2_Size);
                sectionInstance.LookupParameter("二层管道直径D3").SetValueString(twoFloorPipe3_Size);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistance(twoFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层管道1与管道2中心间距").SetValueString((PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层管道2与管道3中心间距").SetValueString((PipeDistance(twoFloorPipe2_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2).ToString());

                if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());

                    if (HaveBrace(oneFloorPipe1_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size, twoFloorPipe3_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size, twoFloorPipe3_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道直径D3").SetValueString(oneFloorPipe3_Size);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size, twoFloorPipe3_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());

                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3).ToString());
                    }
                }
            }
        }
        public void TypeD_ModifyParameter(FamilyInstance sectionInstance) //D型支架修改参数
        {
            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                sectionInstance.LookupParameter("二层支架可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层高度H").SetValueString("0");

                TypeD_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                TypeD_TwoFloorPipeSection(sectionInstance);
            }

        }
        public void TypeD_OneFloorPipeSection(FamilyInstance sectionInstance) //D型支架修改一层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;

            int oneFloorLenght = 1000;
            //一层管道参数修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                sectionInstance.LookupParameter("一层高度H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                     PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                    - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                     PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                    - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                   PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                    PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            sectionInstance.LookupParameter("支架长度L").SetValueString(oneFloorLenght.ToString());
        }
        public void TypeD_TwoFloorPipeSection(FamilyInstance sectionInstance) //D型支架修改二层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;

            int oneFloorLenght = 1000;
            int twoFloorLenght = 1000;

            //一层管道参数修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                sectionInstance.LookupParameter("一层高度H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                     PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                    - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorRightPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                     PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                    - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorRightPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                   PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                    PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            //二层管道参数修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                twoFloorLenght = PipeDistance(twoFloorLeftPipe1_Size).Item1 * 2;
                sectionInstance.LookupParameter("二层高度H").SetValueString(GetFloorHeight(Convert.ToInt32(twoFloorLeftPipe1_Size)).ToString());
            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item1 +
                    PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());

                twoFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层高度H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item1 +
                     PipeDistance(twoFloorRightPipe1_Size).Item1+ PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item1 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item1
                    - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item1 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保二层左侧管道1、二层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                twoFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层高度H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorRightPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item1 +
                     PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item1 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item1
                    - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item1 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保二层右侧管道1、二层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                twoFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层高度H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorRightPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item1 +
                   PipeDistance(twoFloorRightPipe2_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item1 -
                    PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item1 -
                    PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                twoFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层高度H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorLenght > oneFloorLenght)
            {
                sectionInstance.LookupParameter("支架长度L").SetValueString(twoFloorLenght.ToString());
            }
            else
            {
                sectionInstance.LookupParameter("支架长度L").SetValueString(oneFloorLenght.ToString());
            }


        }
        public void TypeC_CreatOneFloorPipeNote(Document doc, FamilyInstance typeC_Section) //C型支架创建一层全部管道信息标注
        {

            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString();
            string oneFloorPipe2_Size = PipeSupportSection.mainfrm.OneFloorPipe2_Size.SelectedItem.ToString();
            string oneFloorPipe3_Size = PipeSupportSection.mainfrm.OneFloorPipe3_Size.SelectedItem.ToString();
            string oneFloorPipe4_Size = PipeSupportSection.mainfrm.OneFloorPipe4_Size.SelectedItem.ToString();

            string oneFloorPipe1_Abb = PipeSupportSection.mainfrm.OneFloorPipe1_Abb.SelectedItem.ToString();
            string oneFloorPipe2_Abb = PipeSupportSection.mainfrm.OneFloorPipe2_Abb.SelectedItem.ToString();
            string oneFloorPipe3_Abb = PipeSupportSection.mainfrm.OneFloorPipe3_Abb.SelectedItem.ToString();
            string oneFloorPipe4_Abb = PipeSupportSection.mainfrm.OneFloorPipe4_Abb.SelectedItem.ToString();

            bool oneFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe1.IsChecked;
            bool oneFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe2.IsChecked;
            bool oneFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe3.IsChecked;
            bool oneFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe4.IsChecked;

            string oneFloorPipe1_Weight = PipeWeight(oneFloorPipe1_Size);
            string oneFloorPipe2_Weight = PipeWeight(oneFloorPipe2_Size);
            string oneFloorPipe3_Weight = PipeWeight(oneFloorPipe3_Size);
            string oneFloorPipe4_Weight = PipeWeight(oneFloorPipe4_Size);

            if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double length = typeC_Section.LookupParameter("一层管道1距墙净距L1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
            }
            else if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double length1 = typeC_Section.LookupParameter("一层管道1距墙净距L1").AsDouble();
                double length2 = typeC_Section.LookupParameter("一层管道1与管道2中心间距").AsDouble();
                XYZ instancePoint1 = new XYZ(positionList.ElementAt(0).X + length2, positionList.ElementAt(0).Y - length2 * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                XYZ instancePoint2 = new XYZ(positionList.ElementAt(1).X + length2, positionList.ElementAt(1).Y - length2 * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint1, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
                CreatPipeNote(doc, positionList.ElementAt(1), instancePoint2, typeC_Section, oneFloorPipe2_Abb + "-" + oneFloorPipe2_Size, PipeWeight(oneFloorPipe2_Size));
            }
            else if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double length1 = typeC_Section.LookupParameter("一层管道1距墙净距L1").AsDouble();
                double length2 = typeC_Section.LookupParameter("一层管道1与管道2中心间距").AsDouble();
                double length3 = typeC_Section.LookupParameter("一层管道2与管道3中心间距").AsDouble();
                XYZ instancePoint1 = new XYZ(positionList.ElementAt(0).X + length2, positionList.ElementAt(0).Y - length2 * Math.Tan(60 * Math.PI / 180) - 240 / 304.8, 0);
                XYZ instancePoint2 = new XYZ(positionList.ElementAt(1).X + length2, positionList.ElementAt(1).Y - length2 * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                XYZ instancePoint3 = new XYZ(positionList.ElementAt(2).X + length3, positionList.ElementAt(2).Y - length3 * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint1, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
                CreatPipeNote(doc, positionList.ElementAt(1), instancePoint2, typeC_Section, oneFloorPipe2_Abb + "-" + oneFloorPipe2_Size, PipeWeight(oneFloorPipe2_Size));
                CreatPipeNote(doc, positionList.ElementAt(2), instancePoint3, typeC_Section, oneFloorPipe3_Abb + "-" + oneFloorPipe3_Size, PipeWeight(oneFloorPipe3_Size));
            }
        }
        public void TypeC_CreatTwoFloorPipeNote(Document doc, FamilyInstance typeC_Section) //C型支架创建二层全部管道信息标注
        {
            string twoFloorPipe1_Size = PipeSupportSection.mainfrm.TwoFloorPipe1_Size.SelectedItem.ToString();
            string twoFloorPipe2_Size = PipeSupportSection.mainfrm.TwoFloorPipe2_Size.SelectedItem.ToString();
            string twoFloorPipe3_Size = PipeSupportSection.mainfrm.TwoFloorPipe3_Size.SelectedItem.ToString();
            string twoFloorPipe4_Size = PipeSupportSection.mainfrm.TwoFloorPipe4_Size.SelectedItem.ToString();

            string twoFloorPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorPipe1_Abb.SelectedItem.ToString();
            string twoFloorPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorPipe2_Abb.SelectedItem.ToString();
            string twoFloorPipe3_Abb = PipeSupportSection.mainfrm.TwoFloorPipe3_Abb.SelectedItem.ToString();
            string twoFloorPipe4_Abb = PipeSupportSection.mainfrm.TwoFloorPipe4_Abb.SelectedItem.ToString();

            bool twoFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe1.IsChecked;
            bool twoFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe2.IsChecked;
            bool twoFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe3.IsChecked;
            bool twoFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe4.IsChecked;

            string twoFloorPipe1_Weight = PipeWeight(twoFloorPipe1_Size);
            string twoFloorPipe2_Weight = PipeWeight(twoFloorPipe2_Size);
            string twoFloorPipe3_Weight = PipeWeight(twoFloorPipe3_Size);
            string twoFloorPipe4_Weight = PipeWeight(twoFloorPipe4_Size);

            if (twoFloorPipe1_Check && !twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层管道");
                double length = typeC_Section.LookupParameter("二层管道1距墙净距L1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorPipe1_Abb + "-" + twoFloorPipe1_Size, PipeWeight(twoFloorPipe1_Size));
            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层管道");
                double length1 = typeC_Section.LookupParameter("二层管道1距墙净距L1").AsDouble();
                double length2 = typeC_Section.LookupParameter("二层管道1与管道2中心间距").AsDouble();
                XYZ instancePoint1 = new XYZ(positionList.ElementAt(0).X + length2, positionList.ElementAt(0).Y + length2 * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                XYZ instancePoint2 = new XYZ(positionList.ElementAt(1).X + length2, positionList.ElementAt(1).Y + length2 * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint1, typeC_Section, twoFloorPipe1_Abb + "-" + twoFloorPipe1_Size, PipeWeight(twoFloorPipe1_Size));
                CreatPipeNote(doc, positionList.ElementAt(1), instancePoint2, typeC_Section, twoFloorPipe2_Abb + "-" + twoFloorPipe2_Size, PipeWeight(twoFloorPipe2_Size));
            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层管道");
                double length1 = typeC_Section.LookupParameter("二层管道1距墙净距L1").AsDouble();
                double length2 = typeC_Section.LookupParameter("二层管道1与管道2中心间距").AsDouble();
                double length3 = typeC_Section.LookupParameter("二层管道2与管道3中心间距").AsDouble();
                XYZ instancePoint1 = new XYZ(positionList.ElementAt(0).X + length2, positionList.ElementAt(0).Y + length2 * Math.Tan(60 * Math.PI / 180) + 240 / 304.8, 0);
                XYZ instancePoint2 = new XYZ(positionList.ElementAt(1).X + length2, positionList.ElementAt(1).Y + length2 * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                XYZ instancePoint3 = new XYZ(positionList.ElementAt(2).X + length3, positionList.ElementAt(2).Y + length3 * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint1, typeC_Section, twoFloorPipe1_Abb + "-" + twoFloorPipe1_Size, PipeWeight(twoFloorPipe1_Size));
                CreatPipeNote(doc, positionList.ElementAt(1), instancePoint2, typeC_Section, twoFloorPipe2_Abb + "-" + twoFloorPipe2_Size, PipeWeight(twoFloorPipe2_Size));
                CreatPipeNote(doc, positionList.ElementAt(2), instancePoint3, typeC_Section, twoFloorPipe3_Abb + "-" + twoFloorPipe3_Size, PipeWeight(twoFloorPipe3_Size));
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
        public void TypeC_CreatTitle(Document doc, XYZ pickpoint, FamilyInstance typeC_Section) //C型支架创建图名
        {
            XYZ titlePosition = new XYZ(pickpoint.X + typeC_Section.LookupParameter("一层支架长L1").AsDouble() / 2,
                    pickpoint.Y - typeC_Section.LookupParameter("支架底部膨胀螺栓定位H1").AsDouble() - 600 / 304.8, 0);

            FamilySymbol typeC_TitleSymbol = null;
            FamilyInstance typeC_Title = null;
            typeC_TitleSymbol = TitleSymbol(doc, "图名");
            typeC_TitleSymbol.Activate();
            typeC_Title = doc.Create.NewFamilyInstance(titlePosition, typeC_TitleSymbol, doc.ActiveView);
            typeC_Title.LookupParameter("标题名称").Set(PipeSupportSection.mainfrm.SupportCode.Text);
            typeC_Title.LookupParameter("横线长度").SetValueString((PipeSupportSection.mainfrm.SupportCode.Text.Length * 5 + 10).ToString());
        }
        public void CreatPipeNote(Document doc, XYZ leaderPoint, XYZ instancePoint, FamilyInstance typeC_Section, string pipeAbb, string pipeWeight)//创建管道信息标注
        {
            FamilySymbol typeC_NoteSymbol = null;
            AnnotationSymbol typeC_Note = null;
            typeC_NoteSymbol = TitleSymbol(doc, "支架剖面管道标注");
            typeC_NoteSymbol.Activate();
            typeC_Note = doc.Create.NewFamilyInstance(instancePoint, typeC_NoteSymbol, doc.ActiveView) as AnnotationSymbol;
            typeC_Note.LookupParameter("管道类型及尺寸").Set(pipeAbb);
            typeC_Note.LookupParameter("管道重量").Set(pipeWeight);
            typeC_Note.addLeader();
            IList<Leader> leadList = typeC_Note.GetLeaders();
            Leader lead = leadList[0];
            lead.End = leaderPoint;
        }
        public void TypeC_CreatDimensionX(Document doc, FamilyInstance section, string supportBoundary, string pipeCenterLine, XYZ pickPoint) //C型支架创建X方向尺寸标注
        {
            List<Line> lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, supportBoundary, pipeCenterLine);
            if (section.LookupParameter("二层支架竖撑显隐").AsInteger() == 1)
            {
                //lineList.RemoveAt(0);
            }
            //MessageBox.Show(lineList.Count.ToString());

            ReferenceArray refArray = new ReferenceArray();
            foreach (Line item in lineList)
            {
                refArray.Append(item.Reference);
            }
            Line tempLine = lineList.FirstOrDefault();
            tempLine.MakeUnbound();
            pickPoint = new XYZ(pickPoint.X, pickPoint.Y, 0);
            XYZ targetPoint = tempLine.Project(pickPoint).XYZPoint;
            XYZ direction = (targetPoint - pickPoint).Normalize();
            Line dimLine = Line.CreateUnbound(pickPoint, direction);

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            doc.Create.NewDimension(doc.ActiveView, dimLine, refArray, dimType);
        }
        public void TypeC_CreatDimensionY(Document doc, FamilyInstance section, string supportBoundary, XYZ pickPoint) //创建Y方向尺寸标注
        {
            List<Line> lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, supportBoundary);
            ReferenceArray refArray = new ReferenceArray();
            foreach (Line item in lineList)
            {
                refArray.Append(item.Reference);
            }
            Line tempLine = lineList.FirstOrDefault();
            tempLine.MakeUnbound();
            pickPoint = new XYZ(pickPoint.X, pickPoint.Y, 0);
            XYZ targetPoint = tempLine.Project(pickPoint).XYZPoint;
            XYZ direction = (targetPoint - pickPoint).Normalize();
            Line dimLine = Line.CreateUnbound(pickPoint, direction);

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            doc.Create.NewDimension(doc.ActiveView, dimLine, refArray, dimType);
        }
        private static List<Line> GetReferenceOfDetailComponent(Document doc
            , Element element, View view, string supportBoundary, string pipeCenterLine) //获取详图项目尺寸标注参照
        {
            List<Line> lineList = new List<Line>();
            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            if (view != null)
            {
                options.View = view;
            }
            else
            {
                options.DetailLevel = ViewDetailLevel.Fine;
            }

            var geoElem = element.get_Geometry(options);
            foreach (var item in geoElem)
            {
                GeometryInstance geoInst = item as GeometryInstance;
                if (geoInst != null)
                {
                    GeometryElement geoElemTmp = geoInst.GetSymbolGeometry();
                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Line line = geomObjTmp as Line;
                        if (line != null)
                        {
                            if (line.Direction.Y == -1 || line.Direction.Y == 1)
                            {
                                ElementId styleID = line.GraphicsStyleId;
                                GraphicsStyle style = doc.GetElement(styleID) as GraphicsStyle;
                                if (style.Name == supportBoundary || style.Name == pipeCenterLine)
                                {
                                    lineList.Add(line);
                                }
                            }
                        }
                    }
                }
            }
            return lineList;
        }
        private static List<Line> GetReferenceOfDetailComponent(Document doc
            , Element element, View view, string supportBoundary) //获取详图项目尺寸标注参照
        {
            List<Line> lineList = new List<Line>();
            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            if (view != null)
            {
                options.View = view;
            }
            else
            {
                options.DetailLevel = ViewDetailLevel.Fine;
            }

            var geoElem = element.get_Geometry(options);
            foreach (var item in geoElem)
            {
                GeometryInstance geoInst = item as GeometryInstance;
                if (geoInst != null)
                {
                    GeometryElement geoElemTmp = geoInst.GetSymbolGeometry();
                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Line line = geomObjTmp as Line;
                        if (line != null)
                        {
                            if (line.Direction.X == -1 || line.Direction.X == 1)
                            {
                                ElementId styleID = line.GraphicsStyleId;
                                GraphicsStyle style = doc.GetElement(styleID) as GraphicsStyle;
                                if (style.Name == supportBoundary)
                                {
                                    lineList.Add(line);
                                }
                            }
                        }
                    }
                }
            }
            return lineList;
        }
        public double MaximumDiameter(Document doc, Element element, View view, string pipeOutline) //获取每层直径最大的管道
        {
            double max = 0;
            List<double> arcList = new List<double>();

            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            if (view != null)
            {
                options.View = view;
            }
            else
            {
                options.DetailLevel = ViewDetailLevel.Fine;
            }

            var geoElem = element.get_Geometry(options);
            foreach (var item in geoElem)
            {
                GeometryInstance geoInst = item as GeometryInstance;
                if (geoInst != null)
                {
                    GeometryElement geoElemTmp = geoInst.GetSymbolGeometry();
                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Arc arc = geomObjTmp as Arc;
                        if (arc != null)
                        {
                            ElementId styleID = arc.GraphicsStyleId;
                            GraphicsStyle style = doc.GetElement(styleID) as GraphicsStyle;
                            if (style.Name.Contains(pipeOutline))
                            {
                                arcList.Add(arc.Radius);
                            }
                        }
                    }
                }
            }
            arcList.Sort();

            return max = arcList.ElementAt(arcList.Count - 1) * 2;
        }
        public List<XYZ> PipeCenterPosition(Document doc, FamilyInstance element, View view, string pipeOutline)//获取管道中心定位
        {

            List<XYZ> arcList = new List<XYZ>();

            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            if (view != null)
            {
                options.View = view;
            }
            else
            {
                options.DetailLevel = ViewDetailLevel.Fine;
            }

            var geoElem = element.get_Geometry(options);
            foreach (var item in geoElem)
            {
                GeometryInstance geoInst = item as GeometryInstance;
                if (geoInst != null)
                {
                    GeometryElement geoElemTmp = geoInst.GetSymbolGeometry();
                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Arc arc = geomObjTmp as Arc;
                        if (arc != null)
                        {
                            ElementId styleID = arc.GraphicsStyleId;
                            GraphicsStyle style = doc.GetElement(styleID) as GraphicsStyle;
                            if (style.Name.Contains(pipeOutline))
                            {
                                Transform trans = element.GetTransform();
                                arcList.Add(trans.OfPoint(arc.Center));
                            }
                        }
                    }
                }
            }
            //arcList.Sort();
            return arcList;
        }
        public Tuple<int, int, int> PipeDistance(string nominal_Diameter) //管道间距
        {
            int distance1 = 0; //  距墙间距L1
            int distance2 = 0; //  管道之间间距L2
            int distance3 = 0; //  距支架边缘间距L3
            if (PipeSupportSection.mainfrm.Insulation.IsChecked == true)
            {
                if (nominal_Diameter == "15")
                {
                    distance1 = 130;
                    distance2 = 190;
                    distance3 = 100;
                }
                else if (nominal_Diameter == "20")
                {
                    distance1 = 140;
                    distance2 = 190;
                    distance3 = 100;
                }
                else if (nominal_Diameter == "25")
                {
                    distance1 = 140;
                    distance2 = 200;
                    distance3 = 110;
                }
                else if (nominal_Diameter == "32")
                {
                    distance1 = 150;
                    distance2 = 210;
                    distance3 = 110;
                }
                else if (nominal_Diameter == "40")
                {
                    distance1 = 160;
                    distance2 = 210;
                    distance3 = 110;
                }
                else if (nominal_Diameter == "50")
                {
                    distance1 = 160;
                    distance2 = 230;
                    distance3 = 120;
                }
                else if (nominal_Diameter == "65")
                {
                    distance1 = 170;
                    distance2 = 250;
                    distance3 = 130;
                }
                else if (nominal_Diameter == "80")
                {
                    distance1 = 190;
                    distance2 = 260;
                    distance3 = 140;
                }
                else if (nominal_Diameter == "100")
                {
                    distance1 = 200;
                    distance2 = 300;
                    distance3 = 150;
                }
                else if (nominal_Diameter == "125")
                {
                    distance1 = 220;
                    distance2 = 320;
                    distance3 = 170;
                }
                else if (nominal_Diameter == "150")
                {
                    distance1 = 230;
                    distance2 = 350;
                    distance3 = 180;
                }
                else if (nominal_Diameter == "200")
                {
                    distance1 = 260;
                    distance2 = 400;
                    distance3 = 210;
                }
                else if (nominal_Diameter == "250")
                {
                    distance1 = 290;
                    distance2 = 470;
                    distance3 = 250;
                }
                else if (nominal_Diameter == "300")
                {
                    distance1 = 330;
                    distance2 = 520;
                    distance3 = 270;
                }
                else if (nominal_Diameter == "350")
                {
                    distance1 = 360;
                    distance2 = 580;
                    distance3 = 300;
                }
                else if (nominal_Diameter == "400")
                {
                    distance1 = 390;
                    distance2 = 640;
                    distance3 = 330;
                }
                else if (nominal_Diameter == "450")
                {
                    distance1 = 420;
                    distance2 = 700;
                    distance3 = 360;
                }
            }
            else
            {
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
            }

            Tuple<int, int, int> tup = new Tuple<int, int, int>(distance1, distance2, distance3);
            return tup;
        }
        public int GetFloorHeight(int pipeSize)
        {
            int height = 500;
            if (pipeSize == 15 || pipeSize == 20 || pipeSize == 25 || pipeSize == 32 || pipeSize == 40 || pipeSize == 50)
            {
                height = 200;
            }
            else if (pipeSize == 65 || pipeSize == 80 || pipeSize == 100 || pipeSize == 125 || pipeSize == 150)
            {
                height = 300;
            }
            else if (pipeSize == 200 || pipeSize == 250)
            {
                height = 400;
            }
            else if (pipeSize == 300 || pipeSize == 350)
            {
                height = 500;
            }
            else if (pipeSize == 400 || pipeSize == 450)
            {
                height = 600;
            }

            return height;
        }
        public int GetPipeDistance10(double num)
        {
            return (int)Math.Ceiling(num / 10) * 10;
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
        public void DetailDrawingTitleLoad(Document doc, string categoryName)
        {
            IList<Element> titleCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family title = null;

            foreach (Family item in titleCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("给排水"))
                {
                    title = item;
                    break;
                }
            }
            if (title == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_注释符号_" + categoryName + ".rfa");
            }
        }
        public FamilySymbol TitleSymbol(Document doc, string symbolName)
        {
            FamilySymbol symbol = null;
            FilteredElementCollector sectionCollector = new FilteredElementCollector(doc);
            sectionCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericAnnotation);

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
        public string PipeWeight(string nominal_Diameter) //管道重量
        {
            string weight = null;
            if (PipeSupportSection.mainfrm.Insulation.IsChecked == true)
            {
                if (nominal_Diameter == "DN15")
                {
                    weight = "3.3";
                }
                else if (nominal_Diameter == "DN20")
                {
                    weight = "4.0";
                }
                else if (nominal_Diameter == "DN25")
                {
                    weight = "5.3";
                }
                else if (nominal_Diameter == "DN32")
                {
                    weight = "6.7";
                }
                else if (nominal_Diameter == "DN40")
                {
                    weight = "7.9";
                }
                else if (nominal_Diameter == "DN50")
                {
                    weight = "10.3";
                }
                else if (nominal_Diameter == "DN65")
                {
                    weight = "14.2";
                }
                else if (nominal_Diameter == "DN80")
                {
                    weight = "17.8";
                }
                else if (nominal_Diameter == "DN100")
                {
                    weight = "25.3";
                }
                else if (nominal_Diameter == "DN125")
                {
                    weight = "34.0";
                }
                else if (nominal_Diameter == "DN150")
                {
                    weight = "45.3";
                }
                else if (nominal_Diameter == "DN200")
                {
                    weight = "77.6";
                }
                else if (nominal_Diameter == "DN250")
                {
                    weight = "112.4";
                }
                else if (nominal_Diameter == "DN300")
                {
                    weight = "155.7";
                }
                else if (nominal_Diameter == "DN350")
                {
                    weight = "211.0";
                }
                else if (nominal_Diameter == "DN400")
                {
                    weight = "255.8";
                }
                else if (nominal_Diameter == "DN450")
                {
                    weight = "295.7";
                }
            }
            else
            {
                if (nominal_Diameter == "DN15")
                {
                    weight = "1.7";
                }
                else if (nominal_Diameter == "DN20")
                {
                    weight = "2.2";
                }
                else if (nominal_Diameter == "DN25")
                {
                    weight = "3.3";
                }
                else if (nominal_Diameter == "DN32")
                {
                    weight = "4.6";
                }
                else if (nominal_Diameter == "DN40")
                {
                    weight = "5.7";
                }
                else if (nominal_Diameter == "DN50")
                {
                    weight = "7.8";
                }
                else if (nominal_Diameter == "DN65")
                {
                    weight = "11.3";
                }
                else if (nominal_Diameter == "DN80")
                {
                    weight = "14.8";
                }
                else if (nominal_Diameter == "DN100")
                {
                    weight = "21.7";
                }
                else if (nominal_Diameter == "DN125")
                {
                    weight = "29.9";
                }
                else if (nominal_Diameter == "DN150")
                {
                    weight = "40.7";
                }
                else if (nominal_Diameter == "DN200")
                {
                    weight = "71.8";
                }
                else if (nominal_Diameter == "DN250")
                {
                    weight = "105.5";
                }
                else if (nominal_Diameter == "DN300")
                {
                    weight = "147.7";
                }
                else if (nominal_Diameter == "DN350")
                {
                    weight = "201.9";
                }
                else if (nominal_Diameter == "DN400")
                {
                    weight = "245.7";
                }
                else if (nominal_Diameter == "DN450")
                {
                    weight = "284.8";
                }
            }
            return weight;
        }
    }
}
