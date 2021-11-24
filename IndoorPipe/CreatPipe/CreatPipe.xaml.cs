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
using System.Windows.Input.Test;
using System.Windows.Threading;

namespace FFETOOLS
{
    /// <summary>
    ///CreatPipeForm.xaml 的交互逻辑
    /// </summary>
    public partial class CreatPipeForm : Window
    {      
        public int clicked = 0;
        ExecuteEventCreatPipe excCreatPipe = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatPipe = null;
        public CreatPipeForm()
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
            excCreatPipe = new ExecuteEventCreatPipe();
            eventHandlerCreatPipe = Autodesk.Revit.UI.ExternalEvent.Create(excCreatPipe);
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
        private void this_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            clicked = 20;
            eventHandlerCreatPipe.Raise();
        }

        #region 右键菜单
        private void HNPipe_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "混凝剂管";
            TextBlock9.Text = "HN";
        }
        private void StabilizerPipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "水质稳定剂";
            TextBlock9.Text = "WD";
        }           
        private void WastewaterPipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "废水管";
            TextBlock9.Text = "F";
        }
        private void HeatSupply1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "热水给水";
            TextBlock9.Text = "RJ";
        }
        private void HeatReturn1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "热水回水";
            TextBlock9.Text = "RH";
        }
        private void PressureSewage1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "压力污水";
            TextBlock9.Text = "YW";
        }
        private void PressureWastewater1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "压力废水";
            TextBlock9.Text = "YF";
        }
        private void RainPipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "雨水管";
            TextBlock9.Text = "Y";
        }
        private void SlugePipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "排泥管";
            TextBlock9.Text = "PN";
        }
        private void PreActionFire1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "预作用消防";
            TextBlock9.Text = "XF";
        }
        private void DryFire1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "干式消防";
            TextBlock9.Text = "XF";
        }
        private void XJ1Pipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "熟料线" + "\n" + "循环给水";
            TextBlock9.Text = "XJ1";
        }
        private void XH1Pipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "熟料线" + "\n" + "循环回水";
            TextBlock9.Text = "XH1";
        }
        private void XJ2Pipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "水泥磨" + "\n" + "循环给水";
            TextBlock9.Text = "XJ2";
        }
        private void XH2Pipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "水泥磨" + "\n" + "循环回水";
            TextBlock9.Text = "XH2";
        }
        private void XJPipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "余热发电" + "\n" + "循环给水";
            TextBlock9.Text = "XJ";
        }

        private void XHPipe1_Click(object sender, RoutedEventArgs e)
        {
            Button9.Content = "余热发电" + "\n" + "循环回水";
            TextBlock9.Text = "XH";
        }
        #endregion

        #region 右键菜单
        private void XDPipe_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "消毒剂管";
            TextBlock10.Text = "XD";
        }
        private void StabilizerPipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "水质稳定剂";
            TextBlock10.Text = "WD";
        }
        private void WastewaterPipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "废水管";
            TextBlock10.Text = "F";
        }

        private void HeatSupply2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "热水给水";
            TextBlock10.Text = "RJ";
        }

        private void HeatReturn2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "热水回水";
            TextBlock10.Text = "RH";
        }

        private void PressureSewage2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "压力污水";
            TextBlock10.Text = "YW";
        }

        private void PressureWastewater2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "压力废水";
            TextBlock10.Text = "YF";
        }

        private void RainPipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "雨水管";
            TextBlock10.Text = "Y";
        }

        private void SlugePipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "排泥管";
            TextBlock10.Text = "PN";
        }

        private void PreActionFire2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "预作用消防";
            TextBlock10.Text = "XF";
        }

        private void DryFire2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "干式消防";
            TextBlock10.Text = "XF";
        }

        private void XJ1Pipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "熟料线" + "\n" + "循环给水";
            TextBlock10.Text = "XJ1";
        }

        private void XH1Pipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "熟料线" + "\n" + "循环回水";
            TextBlock10.Text = "XH1";
        }

        private void XJ2Pipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "水泥磨" + "\n" + "循环给水";
            TextBlock10.Text = "XJ2";
        }

        private void XH2Pipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "水泥磨" + "\n" + "循环回水";
            TextBlock10.Text = "XH2";
        }

        private void XJPipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "余热发电"+"\n"+ "循环给水";          
            TextBlock10.Text = "XJ";
        }

        private void XHPipe2_Click(object sender, RoutedEventArgs e)
        {
            Button10.Content = "余热发电" + "\n" + "循环回水";
            TextBlock10.Text = "XH";
        }
        #endregion
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }     
    }
}
