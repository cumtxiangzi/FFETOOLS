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
                DetailDrawingTitleLoad(doc, "图名");

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
            using (Transaction trans = new Transaction(doc, "创建图名"))
            {
                trans.Start();
                CreatTitle(doc, pickpoint, typeC_Section);

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
                        CreatDimensionX(doc, typeC_Section, "一层支架边界线", "一层管道中心线", dimPosition);
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        double height1 = MaximumDiameter(doc, typeC_Section, doc.ActiveView, "一层管道");
                        typeC_Section.LookupParameter("一层支架净高H1").SetValueString((height1 * 304.8 + 150).ToString());

                        XYZ dimPosition1 = new XYZ(pickpoint.X, pickpoint.Y - 250 / 304.8, pickpoint.Z);
                        CreatDimensionX(doc, typeC_Section, "一层支架边界线", "一层管道中心线", dimPosition1);

                        double height2 = MaximumDiameter(doc, typeC_Section, doc.ActiveView, "二层管道");
                        XYZ dimPosition2 = new XYZ(pickpoint.X, pickpoint.Y + height1 + height2 + 600 / 304.8, pickpoint.Z); //此处有BUG，小直径管道尺寸线位置不准确
                        CreatDimensionX(doc, typeC_Section, "二层支架边界线", "二层管道中心线", dimPosition2);

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
                        CreatDimensionY(doc, typeC_Section, "一层支架边界线", dimPosition3);
                    }
                }


                trans.Commit();
            }

            tg.Assimilate();
            PipeSupportSection.mainfrm.Show();
        }
        public void ModifyParameter(FamilyInstance sectionInstance) //修改参数
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
                OneFloorPipeSection(sectionInstance);

            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                OneFloorPipeSection(sectionInstance);
                TwoFloorPipeSection(sectionInstance);
            }
        }
        public void OneFloorPipeSection(FamilyInstance sectionInstance) //修改一层管道参数
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
        public void TwoFloorPipeSection(FamilyInstance sectionInstance) //修改二层管道参数
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

                if (HaveBrace(twoFloorPipe1_Size))
                {
                    sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                    TwoFloorPipeSet(sectionInstance, oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size, oneFloorPipe4_Size,
                                             oneFloorPipe1_Check, oneFloorPipe2_Check, oneFloorPipe3_Check, oneFloorPipe4_Check);
                }
                else
                {
                    sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                    sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
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

                if (HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size))
                {
                    sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                    TwoFloorPipeSet(sectionInstance, oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size, oneFloorPipe4_Size,
                                            oneFloorPipe1_Check, oneFloorPipe2_Check, oneFloorPipe3_Check, oneFloorPipe4_Check);
                }
                else
                {
                    sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                    sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                                                                                      PipeDistance(twoFloorPipe2_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item3).ToString());
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

                if (HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size, twoFloorPipe3_Size))
                {
                    sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                    TwoFloorPipeSet(sectionInstance, oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size, oneFloorPipe4_Size,
                                            oneFloorPipe1_Check, oneFloorPipe2_Check, oneFloorPipe3_Check, oneFloorPipe4_Check);
                }
                else
                {
                    sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                    sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                                                                                     PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item3).ToString());
                }
            }
        }
        public void TwoFloorPipeSet(FamilyInstance sectionInstance, string oneFloorPipe1_Size, string oneFloorPipe2_Size, string oneFloorPipe3_Size, string oneFloorPipe4_Size,
                                                 bool oneFloorPipe1_Check, bool oneFloorPipe2_Check, bool oneFloorPipe3_Check, bool oneFloorPipe4_Check) // 二层管道时一层管道支架长度设置
        {

            if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 + 100).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 + 100).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
            }
            if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                      PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 + 100).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                      PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 + 100).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                      PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
            }
            if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                      PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 + 100).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                      PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 + 100).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                     PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
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
        public void CreatTitle(Document doc, XYZ pickpoint, FamilyInstance typeC_Section) //创建图名
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
        public void CreatDimensionX(Document doc, FamilyInstance section, string supportBoundary, string pipeCenterLine, XYZ pickPoint) //创建X方向尺寸标注
        {
            List<Line> lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, supportBoundary, pipeCenterLine);
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
        public void CreatDimensionY(Document doc, FamilyInstance section, string supportBoundary, XYZ pickPoint) //创建Y方向尺寸标注
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
    }
}
