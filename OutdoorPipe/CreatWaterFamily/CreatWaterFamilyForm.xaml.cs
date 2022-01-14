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

            //String path = @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Image\ValveFamily";
            //var files = Directory.GetFiles(path, "*.jpg");
            //foreach (string fileName in files)
            //{
            //    Uri uri = new Uri(fileName);
            //    BitmapImage bitmap = new BitmapImage(uri);
            //    imgItems.Add(bitmap);
            //}
            //FamilyImageList.ItemsSource = imgItems;
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Hidden;

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
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Hidden;
        }
        private void MeterButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
            MeterList.Visibility= Visibility.Visible;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Hidden;
        }
        private void AccessoriesButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Visible;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Hidden;
        }

        private void PumpButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Visible;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Hidden;
        }

        private void WaterTreatButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Visible;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Hidden;
        }

        private void SewageTreatButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Visible;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Hidden;
        }

        private void WaterComponentsButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Visible;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Hidden;
        }

        private void FireProtectionButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Visible;
            AnnotationSymbol.Visibility = Visibility.Hidden;
        }

        private void NoteButton_Click(object sender, RoutedEventArgs e)
        {
            ValveList.Visibility = Visibility.Hidden;
            MeterList.Visibility = Visibility.Hidden;
            AccessoriesList.Visibility = Visibility.Hidden;
            WaterSupplyEquipment.Visibility = Visibility.Hidden;
            WaterSupplyTreat.Visibility = Visibility.Hidden;
            SewageTreat.Visibility = Visibility.Hidden;
            WaterSupplyComponent.Visibility = Visibility.Hidden;
            FireEquipment.Visibility = Visibility.Hidden;
            AnnotationSymbol.Visibility = Visibility.Visible;
        }
        private void ButterflyValveWoLun_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "蝶阀D37A1X-10";
        }
        private void ButterflyValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "蝶阀D7A1X-10";
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
        private void PressureValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "泄压阀";
        }
        private void WaterMeterXuanYi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void WaterMeterLuoYi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void FlowMeter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void PressureGauge_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void VacuumMeter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void PressureSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void TemperatureSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void Thermometer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void RubberJoint_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void TypeYFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void WaterTap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void SuctionBell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void OverFlowBell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void FloorDrain_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void VentTap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void CleanOut_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void CheckOut_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void SinglePump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void DoublePump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void QianShuiPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void GuDingPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void VerticalPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void ChaiYouPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void HengYaPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void DieYaPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void PipePump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        private void ButterflyValveWoLun_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
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
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

        private void E_ButterflyValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

        private void E_GateValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

        private void CheckValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

        private void VentValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }  

        private void PressureValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

       

        private void WaterMeterXuanYi_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void WaterMeterLuoYi_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void FlowMeter_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void PressureGauge_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void VacuumMeter_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void PressureSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void TemperatureSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void Thermometer_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        
        private void RubberJoint_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void TypeYFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void WaterTap_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void SuctionBell_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void OverFlowBell_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void FloorDrain_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void VentTap_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void CleanOut_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        
        private void CheckOut_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       
        private void SinglePump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void DoublePump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void QianShuiPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void GuDingPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void VerticalPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void ChaiYouPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void HengYaPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

      

        private void DieYaPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void PipePump_MouseDown(object sender, MouseButtonEventArgs e)
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
