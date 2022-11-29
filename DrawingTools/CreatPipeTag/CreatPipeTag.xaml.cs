using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FFETOOLS
{
    /// <summary>
    ///CreatPipeTagForm.xaml 的交互逻辑
    /// </summary>
    public partial class CreatPipeTagForm : Window
    {
        public int clicked = 0;
        List<string> TextWithLineList = new List<string>()
        {"接风机(613FA18)循环水进口","接润滑装置(613LU10)循环水进口","接选粉机(416SP17)减速机循环水进口","接辊压机(216RP04)辊轴循环水进口","接辊压机(416RP04)轴承座循环水进口",
            "接辊压机稀油站(416LU10)循环水进口","预留洞400X300","楼板留洞Φ100","楼板及屋顶留洞Φ150",
            "池底预埋钢板1000X950X20mm","工字钢", "压力传感器(XP1)","温度传感器(XT3)","爬梯","导流墙",
            "人孔","活动栏杆","土建专业设钢盖板","集水坑400X400X400","收水堰","回水台","见详图A","接至水质检测室","接生产给水泵(919PU43)吸水管",
            "排水至散水","溢流口上方加防雨罩","消防水泵接合器","灭火砂箱","屋顶试验消火栓","洗眼器","楼板贴壁留洞Φ100","接小便器排水管","接大便器排水管",
            "接小便器自闭式冲洗阀","接大便器自闭式冲洗阀","接大便器冲洗水箱","设备安装后封墙"       
        };

        List<string> TextWithLineListEN = new List<string>() { "TO INLET OF 216RP07-BEARING", "FROM OUTLET OF 216RP07-BEARING", "TO INLET OF 216LU13-LUBRICATION" ,
        "TO INLET OF 216RP07-ROLLER","DRAINAGE TO APROLL SLOPE","SEE DETAIL A","SUMP400X400X400","MANHOLE","STEEL COVER SEE CIVIL DRAWING","LADDER",
        "MOVABLE RAILING","1000X950X20mm STEEL PLATE BE PREFORMED","BUILT WALL AFTER EQUIPMENT INSTALLATION","工-STEEL FOR HOIST","DIVERSION WALL"};

        List<string> LanguageList = new List<string>() { "中文", "英文" };
        List<string> LineNumList = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8" };

        ExecuteEventCreatPipeTag excCreatPipe = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatPipe = null;
        public CreatPipeTagForm()
        {
            InitializeComponent();
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                //Close();
            }
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            //e.Handled = true;
            TextInputCmb.Focus();
            TextInputCmb.ItemsSource = TextWithLineList;
            LanguageCmb.ItemsSource = LanguageList;
            LanguageCmb.SelectedIndex = 0;
            LineNumCmb.ItemsSource= LineNumList;
            LineNumCmb.SelectedIndex = 0;

            Button9.Content = "编 号 批 量" + "\n" + "    标 注";
            Button10.Content = "管 道 附 件" + "\n" + "    标 注";
            Button7.Content = "刚 性 套 管" + "\n" + "图 集 标 注";
            Button8.Content = "柔 性 套 管" + "\n" + "图 集 标 注";
            Button6.Content = "管 道 基 础" + "\n" + "    留 洞";
            Button11.Content = "管 道 楼 板" + "\n" + "留 洞 方 形";
            Button12.Content = "管 道 墙 壁" + "\n" + "    留 洞";
            Button17.Content = " 潜 水 泵" + "\n" + " 电 控 箱";
            Button18.Content = "消 火 栓 箱" + "\n" + "    留 洞";
            Button19.Content = "支 架 剖 面" + "\n" + "管 道 标 注";
            Button20.Content = "侧 壁 预 埋" + "\n" + "  板 标 注";
            Button21.Content = "刚 性 套 管" + "\n" + "字 母 标 注";
            Button22.Content = "柔 性 套 管" + "\n" + "字 母 标 注";
            Button23.Content = "管 道 楼 板" + "\n" + "留 洞 圆 形";
            Button26.Content = "  预 留 洞" + "\n" + "字 母 标 注";
            Button27.Content = "轴 网 间 距" + "\n" + "    标 注";
            Button28.Content = "设 备 间 距" + "\n" + "    标 注";

            excCreatPipe = new ExecuteEventCreatPipeTag();
            eventHandlerCreatPipe = Autodesk.Revit.UI.ExternalEvent.Create(excCreatPipe);
        }
        private void this_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            clicked = 1;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            clicked = 2;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            clicked = 3;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            clicked = 4;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            clicked = 5;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            clicked = 6;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            clicked = 7;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            clicked = 8;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            clicked = 9;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            clicked = 10;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button11_Click(object sender, RoutedEventArgs e)
        {
            clicked = 11;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button12_Click(object sender, RoutedEventArgs e)
        {
            clicked = 12;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button13_Click(object sender, RoutedEventArgs e)
        {
            clicked = 13;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button14_Click(object sender, RoutedEventArgs e)
        {
            clicked = 14;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button15_Click(object sender, RoutedEventArgs e)
        {
            clicked = 15;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button16_Click(object sender, RoutedEventArgs e)
        {
            clicked = 16;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button17_Click(object sender, RoutedEventArgs e)
        {
            clicked = 17;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button18_Click(object sender, RoutedEventArgs e)
        {
            clicked = 18;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button19_Click(object sender, RoutedEventArgs e)
        {
            clicked = 19;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button20_Click(object sender, RoutedEventArgs e)
        {
            clicked = 20;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button21_Click(object sender, RoutedEventArgs e)
        {
            clicked = 21;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button22_Click(object sender, RoutedEventArgs e)
        {
            clicked = 22;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button23_Click(object sender, RoutedEventArgs e)
        {
            clicked = 23;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button24_Click(object sender, RoutedEventArgs e)
        {
            clicked = 24;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button25_Click(object sender, RoutedEventArgs e)
        {
            clicked = 25;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button26_Click(object sender, RoutedEventArgs e)
        {
            clicked = 26;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button27_Click(object sender, RoutedEventArgs e)
        {
            clicked = 27;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void Button28_Click(object sender, RoutedEventArgs e)
        {
            clicked = 28;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void Button29_Click(object sender, RoutedEventArgs e)
        {
            clicked = 29;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            clicked = 100;
            Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            eventHandlerCreatPipe.Raise();
        }

        private void ButtonTemperature_Click(object sender, RoutedEventArgs e)
        {
            TextInputCmb.Text += "℃";
            TextInputCmb.Focus();
        }

        private void ButtonPositive_Click(object sender, RoutedEventArgs e)
        {
            TextInputCmb.Text += "±";
            TextInputCmb.Focus();
        }

        private void ButtonCircle_Click(object sender, RoutedEventArgs e)
        {
            TextInputCmb.Text += "Φ";
            TextInputCmb.Focus();
        }

        private void ButtonPercent_Click(object sender, RoutedEventArgs e)
        {
            TextInputCmb.Text += "％";
            TextInputCmb.Focus();
        }

        private void TextInputCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clicked = 100;
            //Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, System.Windows.Forms.Keys.Escape);
            //eventHandlerCreatPipe.Raise();
        }

        private void LanguageCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageCmb.SelectedItem.ToString() == "中文")
            {
                TextInputCmb.ItemsSource = TextWithLineList;
            }

            if (LanguageCmb.SelectedItem.ToString() == "英文")
            {
                TextInputCmb.ItemsSource = TextWithLineListEN;
            }

            if (LanguageCmb.SelectedItem.ToString() == "中英文")
            {
                TextInputCmb.ItemsSource = TextWithLineListEN;
            }

        }

    }
}
