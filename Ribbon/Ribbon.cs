using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Events;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Ribbon : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public Result OnStartup(UIControlledApplication application)
        {           
            string TabName = "给排水BIM工具箱";
            application.CreateRibbonTab(TabName);
            RibbonPanel P_TabName1 = application.CreateRibbonPanel(TabName, "室外管道设计");
            RibbonPanel P_TabName2 = application.CreateRibbonPanel(TabName, "室内管道设计");
            RibbonPanel P_TabName3 = application.CreateRibbonPanel(TabName, "泵站设计");
            RibbonPanel P_TabName4 = application.CreateRibbonPanel(TabName, "消防设计");
            RibbonPanel P_TabName5 = application.CreateRibbonPanel(TabName, "设计出图辅助");
            RibbonPanel P_TabName6 = application.CreateRibbonPanel(TabName, "其他辅助");
            RibbonPanel P_TabName39 = application.CreateRibbonPanel(TabName, "关于插件");

            Autodesk.Windows.RibbonControl ribbon = Autodesk.Windows.ComponentManager.Ribbon;
            Autodesk.Windows.RibbonTab rt = null;
            foreach (Autodesk.Windows.RibbonTab tab in ribbon.Tabs)

            {
                if (tab.Name == "给排水BIM工具箱")

                {
                    rt = tab;

                    ribbon.Tabs.Remove(tab);

                    break;

                }
            }
            ribbon.Tabs.Insert(0, rt);

            try
            {
                PushButtonData button110 = new PushButtonData("多管绘制", "多管\n绘制", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\OutdoorPipe.dll", "FFETOOLS.CreatOutdoorPipes");
                button110.ToolTip = "室外共线管道批量绘制";
                button110.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\OutdoorPipes.ico"));
                P_TabName1.AddItem(button110);

                PushButtonData button120 = new PushButtonData("批量打断", "批量\n打断", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\OutdoorPipe.dll", "FFETOOLS.BatchBreakPipes");
                button120.ToolTip = "批量打断室外共线管道";
                button120.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\BatchBreakPipes.ico"));
                P_TabName1.AddItem(button120);

                PushButtonData button130 = new PushButtonData("批量升降", "批量\n升降", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\OutdoorPipe.dll", "FFETOOLS.RaisePipes");
                button130.ToolTip = "批量升降室外共线管道";
                button130.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\RaisePipes.ico"));
                P_TabName1.AddItem(button130);

                PushButtonData button140 = new PushButtonData("批量弯头连接", "批量弯头\n连接", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\OutdoorPipe.dll", "FFETOOLS.BatchCreatPipeElbow");
                button140.ToolTip = "批量连接室外共线管道";
                button140.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\BatchCreatPipeElbow.ico"));
                P_TabName1.AddItem(button140);

                PushButtonData button150 = new PushButtonData("批量三通连接", "批量三通\n连接", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\OutdoorPipe.dll", "FFETOOLS.BatchCreatTeeFitting");
                button150.ToolTip = "批量连接室外共线管道";
                button150.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\BatchCreatTeeFitting.ico"));
                P_TabName1.AddItem(button150);

                PushButtonData button160 = new PushButtonData("管道避让", "管道\n避让", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\OutdoorPipe.dll", "FFETOOLS.PipeAvoid");
                button160.ToolTip = "管道碰撞避让调整";
                button160.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\PipeAvoid.ico"));
                P_TabName1.AddItem(button160);

                PushButtonData button170 = new PushButtonData("三通支管升降", "三通支管\n升降", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\OutdoorPipe.dll", "FFETOOLS.RaiseTeeBranch");
                button170.ToolTip = "三通支管升降调整";
                button170.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\RaiseTeeBranch.ico"));
                P_TabName1.AddItem(button170);


                PushButtonData button210 = new PushButtonData("创建管道", "创建\n管道", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\IndoorPipe.dll", "FFETOOLS.CreatPipe");
                button210.ToolTip = "快速创建管道，管材及管道系统无需设置";
                button210.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\CreatPipe.ico"));
                P_TabName2.AddItem(button210);

                PushButtonData button220 = new PushButtonData("管道打断", "管道\n打断", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\IndoorPipe.dll", "FFETOOLS.BreakPipe");
                button220.ToolTip = "管道连续打断";
                button220.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\BreakPipe.ico"));
                P_TabName2.AddItem(button220);

                PushButtonData button230 = new PushButtonData("弯头连接", "弯头\n连接", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\IndoorPipe.dll", "FFETOOLS.CreatPipeElbow");
                button230.ToolTip = "管道连接生成弯头";
                button230.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\CreatPipeElbow.ico"));
                P_TabName2.AddItem(button230);

                PushButtonData button240 = new PushButtonData("三通连接", "三通\n连接", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\IndoorPipe.dll", "FFETOOLS.CreatTeeFitting");
                button240.ToolTip = "管道连接生成三通";
                button240.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\CreatTeeFitting.ico"));
                P_TabName2.AddItem(button240);

                PushButtonData button250 = new PushButtonData("立管生成", "立管\n生成", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\IndoorPipe.dll", "FFETOOLS.UpPipe");
                button250.ToolTip = "管道末端生成立管";
                button250.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\UpPipe.ico"));
                P_TabName2.AddItem(button250);


                PushButtonData button310 = new PushButtonData("布置泵组", "布置\n泵组", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\MainWorkShop.dll", "FFETOOLS.PumpGroup");
                button310.ToolTip = "一键布置水泵";
                button310.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\PumpGroup.ico"));
                P_TabName3.AddItem(button310);

                //PushButtonData button320 = new PushButtonData("布置泵房", "布置\n泵房", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\MainWorkShop.dll", "FFETOOLS.PumpStation");
                //button320.ToolTip = "一键布置水泵房";
                //button320.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\PumpStation.ico"));
                //P_TabName3.AddItem(button320);

                PushButtonData button330 = new PushButtonData("布置水池", "布置\n水池", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\MainWorkShop.dll", "FFETOOLS.WaterPool");
                button330.ToolTip = "一键布置水池";
                button330.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\WaterPool.ico"));
                P_TabName3.AddItem(button330);


                PushButtonData button410 = new PushButtonData("连接消火栓", "连接\n消火栓", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\FireProtection.dll", "FFETOOLS.HydrantConnect");
                button410.ToolTip = "管道连接至消火栓";
                button410.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\HydrantConnect.ico"));
                P_TabName4.AddItem(button410);


                PushButtonData button510 = new PushButtonData("复制视图", "复制\n视图", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.ViewDuplicate");
                button510.ToolTip = "批量复制所有建筑平面视图";
                button510.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\ViewDuplicate.ico"));
                P_TabName5.AddItem(button510);

                PushButtonData button520 = new PushButtonData("批量创建图纸", "创建\n图纸", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.CreatDrawing");
                button520.ToolTip = "批量创建图纸并按专业命名";
                button520.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\CreatDrawing.ico"));
                P_TabName5.AddItem(button520);

                PushButtonData button530 = new PushButtonData("批量隐藏外专业工作集", "隐藏\n工作集", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.ShowWorkset");
                button530.ToolTip = "批量隐藏所有视图外专业工作集,便于出图";
                button530.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\ShowWorkset.ico"));
                P_TabName5.AddItem(button530);

                PushButtonData button540 = new PushButtonData("批量生成管道系统图", "系统图\n生成", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.CreatPipeSystem");
                button540.ToolTip = "批量生成各类管道系统图";
                button540.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\CreatPipeSystem.ico"));
                P_TabName5.AddItem(button540);

                PushButtonData button550 = new PushButtonData("给排水标注工具", "给排水\n标注", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.CreatPipeTag");
                button550.ToolTip = "对模型进行各类标注";
                button550.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\CreatPipeTag.ico"));
                P_TabName5.AddItem(button550);

                PushButtonData button560 = new PushButtonData("平剖面整理工具", "平剖面\n整理", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.PipeShowBold");
                button560.ToolTip = "对给排水平剖面图批量整理，满足出图要求";
                button560.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\PipeShowBold.ico"));
                P_TabName5.AddItem(button560);

                PushButtonData button570 = new PushButtonData("批量标注管径", "批量标注\n管径", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.NotePipes");
                button570.ToolTip = "用于系统图修改后批量标注修改管道管径";
                button570.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\NotePipes.ico"));
                P_TabName5.AddItem(button570);

                PushButtonData button580 = new PushButtonData("导出图纸目录", "导出图纸\n目录", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.ExportDLT");
                button580.ToolTip = "导出给排水图纸目录";
                button580.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\ExportDLT.ico"));
                P_TabName5.AddItem(button580);

                PushButtonData button590 = new PushButtonData("批量导出其他文件格式", "批量\n导出", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\DrawingTools.dll", "FFETOOLS.BatchExport");
                button590.ToolTip = "批量导出其他文件格式，如DWG,PDF等";
                button590.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\BatchExport.ico"));
                P_TabName5.AddItem(button590);


                PushButtonData button610 = new PushButtonData("创建局部三维框", "局部\n三维", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\BatchTools.dll", "FFETOOLS.CreatSectionBox");
                button610.ToolTip = "创建局部三维视图,便于观察三维模型";
                button610.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\CreatSectionBox.ico"));
                P_TabName6.AddItem(button610);

                PushButtonData button620 = new PushButtonData("创建工作集", "创建\n工作集", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\BatchTools.dll", "FFETOOLS.CreatWorkset");
                button620.ToolTip = "创建本专业工作集，名称按自己姓名命名";
                button620.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\CreatWorkset.ico"));
                P_TabName6.AddItem(button620);

                PushButtonData button630 = new PushButtonData("黑白背景", "黑白\n背景", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\BatchTools.dll", "FFETOOLS.SwitchBackGround");
                button630.ToolTip = "快速切换黑白背景";
                button630.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\SwitchBackGround.ico"));
                P_TabName6.AddItem(button630);

                PushButtonData button640 = new PushButtonData("剖面符号显示隐藏", "剖面显示\n隐藏", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\BatchTools.dll", "FFETOOLS.ShowSectionSymbol");
                button640.ToolTip = "剖面符号显示隐藏";
                button640.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\ShowSectionSymbol.ico"));
                P_TabName6.AddItem(button640);


                PushButtonData button3910 = new PushButtonData("关于插件", "信息", @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\BatchTools.dll", "FFETOOLS.About");
                button3910.ToolTip = "插件信息";
                button3910.LargeImage = new BitmapImage(new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Icon\About.ico"));
                P_TabName39.AddItem(button3910);

                return Result.Succeeded;
            }
            catch (Exception)
            {
                throw;            
            }
           
        }
    }


}
