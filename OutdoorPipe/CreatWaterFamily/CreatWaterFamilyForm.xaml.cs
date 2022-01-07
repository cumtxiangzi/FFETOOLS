using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FFETOOLS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreatWaterFamilyForm : Window
    {
        ExecuteCreatWaterFamily excCreatWaterFamily = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatWaterFamily = null;
        BindingList<BitmapImage> imgItems = new BindingList<BitmapImage>();

        public CreatWaterFamilyForm()
        {
            InitializeComponent();
            excCreatWaterFamily = new ExecuteCreatWaterFamily();
            eventHandlerCreatWaterFamily = Autodesk.Revit.UI.ExternalEvent.Create(excCreatWaterFamily);

            String path = @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Image\ValveFamily";
            var files = Directory.GetFiles(path, "*.jpg");
            foreach (string fileName in files)
            {
                Uri uri = new Uri(fileName);
                BitmapImage bitmap = new BitmapImage(uri);
                imgItems.Add(bitmap);
            }
            //FamilyImageList.ItemsSource = imgItems;

        }
        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
            ValveButton.IsChecked = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ValveButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Visible;
        }
        private void MeterButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
        }
        private void ButterflyValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "蝶阀";
        }

        private void GateValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "闸阀";
        }

        private void StopValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "截止阀";
        }

        private void E_ButterflyValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "电动蝶阀";
        }

        private void E_GateValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "电动闸阀";
        }

        private void CheckValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "止回阀";
        }

        private void VentValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "自动排气阀";
        }

        private void ButterflyValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
       
        private void GateValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

        private void StopValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void E_ButterflyValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void E_GateValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CheckValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void VentValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
    public static class Helper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        /// <summary>
        /// 发送键盘消息
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="key"></param>
        public static void SendKeys(IntPtr proc, Keys key)
        {
            SetActiveWindow(proc);
            SetForegroundWindow(proc);
            keybd_event(key, 0, 0, 0);
            keybd_event(key, 0, 2, 0);
            keybd_event(key, 0, 0, 0);
            keybd_event(key, 0, 2, 0);
        }
    }
}
