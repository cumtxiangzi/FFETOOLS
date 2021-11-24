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
    class Ribbon : IExternalApplication
    {
        static string AddInPath = typeof(Ribbon).Assembly.Location;
        // Button icons directory
        static string ButtonIconsFolder = Path.GetDirectoryName(AddInPath);

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string TabName = "公用所BIM工具";
            application.CreateRibbonTab(TabName);
            RibbonPanel P_TabName1 = application.CreateRibbonPanel(TabName, "批量工具");
            //RibbonPanel P_TabName2 = application.CreateRibbonPanel(TabName, "查看工具");
            RibbonPanel P_TabName3 = application.CreateRibbonPanel(TabName, "关于插件");

            PushButtonData button110 = new PushButtonData("复制视图", "复制\n视图", AddInPath, "FFETOOLS.ViewDuplicate");
            button110.ToolTip = "批量复制所有建筑平面视图";
            button110.LargeImage = new BitmapImage(new Uri(AddInPath.Replace("FFETOOLS.dll", @"\Resources\ViewDuplicate.png"), UriKind.Absolute));
            P_TabName1.AddItem(button110);

            PushButtonData button120 = new PushButtonData("批量创建图纸", "创建\n图纸", AddInPath, "FFETOOLS.CreatDrawing");
            button120.ToolTip = "批量创建图纸并按专业命名";
            button120.LargeImage = new BitmapImage(new Uri(AddInPath.Replace("FFETOOLS.dll", @"\Resources\CreatDrawing.png"), UriKind.Absolute));
            P_TabName1.AddItem(button120);

            PushButtonData button130 = new PushButtonData("批量隐藏外专业工作集", "隐藏\n工作集", AddInPath, "FFETOOLS.ShowWorkset");
            button130.ToolTip = "批量隐藏所有视图外专业工作集,便于出图";
            button130.LargeImage = new BitmapImage(new Uri(AddInPath.Replace("FFETOOLS.dll", @"\Resources\ShowWorkset.png"), UriKind.Absolute));
            P_TabName1.AddItem(button130);

            //PushButtonData button210 = new PushButtonData("创建局部三维框", "局部\n三维", AddInPath, "FFETOOLS.CreatSectionBox");
            //button210.ToolTip = "创建局部三维视图,便于观察三维模型";
            //button210.LargeImage = new BitmapImage(new Uri(AddInPath.Replace("FFETOOLS.dll", @"\Resources\CreatSectionBox.png"), UriKind.Absolute));
            //P_TabName2.AddItem(button210);

            PushButtonData button310 = new PushButtonData("关于插件", "信息", AddInPath, "FFETOOLS.About");
            button310.ToolTip = "插件信息";
            button310.LargeImage = new BitmapImage(new Uri(AddInPath.Replace("FFETOOLS.dll", @"\Resources\About.png"), UriKind.Absolute));
            P_TabName3.AddItem(button310);
            
            return Result.Succeeded;
        }
    }
}
