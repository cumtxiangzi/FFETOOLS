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
            FamilyName.Text = "闸阀Z15T-10";
        }
        private void GateValveFaLan_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "闸阀Z45-10";
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
        private void CheckValveWeiZu_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "微阻缓闭止回阀";
        }
        private void VentValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "自动排气阀";
        }
        private void PressureValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "泄压阀";
        }
        private void BallValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "球阀";
        }
        private void ControlValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "液压水位控制阀";
        }
        private void SolenoidValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "电磁阀";
        }
        private void DaBianValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "大便器自闭式冲洗阀";
        }
        private void XiaoBianValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "小便器自闭式冲洗阀";
        }
        private void ChaBanValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "插板阀";
        }
        private void FuQiuValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "浮球阀";
        }
        private void AngleValve_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "角阀";
        }
        private void WaterMeterXuanYi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "旋翼式水表";
        }
        private void WaterMeterLuoYi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "螺翼式水表";
        }
        private void FlowMeter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "旋翼式水表";
        }
        private void PressureGauge_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "压力表";
        }
        private void VacuumMeter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "真空表";
        }
        private void PressureSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "压力变送器";
        }
        private void TemperatureSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "温度变送器";
        }
        private void Thermometer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "温度计";
        }
        private void RubberJoint_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "挠性橡胶接头";
        }
        private void TypeYFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "Y型过滤器";
        }
        private void WaterTap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "水嘴";
        }
        private void SuctionBell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "吸水喇叭口";
        }
        private void OverFlowBell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "溢流喇叭口";
        }
        private void FloorDrain_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "圆形地漏";
        }
        private void VentTap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "通气帽";
        }
        private void CleanOut_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "清扫口";
        }
        private void CheckOut_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "检查口";
        }
        private void SinglePump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "卧式单吸泵";
        }
        private void DoublePump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "卧式双吸泵";
        }
        private void DingYaPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "定压补水装置";
        }
        private void LongShaftPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "立式长轴泵";
        }
        private void QianShuiPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "潜水泵(移动式安装)";
        }
        private void GuDingPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "潜水泵(固定式安装)";
        }
        private void VerticalPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "立式消防泵";
        }
        private void ChaiYouPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "柴油消防泵";
        }
        private void HengYaPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "恒压变频供水设备";
        }
        private void DieYaPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "管网叠压供水设备";
        }
        private void PipePump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "立式管道泵";
        }
        private void ZiXiPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "自吸泵";
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

       

        private void GateValveFaLan_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

      

        private void CheckValveWeiZu_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }



        private void DingYaPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void LongShaftPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void ZiXiPump_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void BallValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void ControlValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void SolenoidValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void DaBianValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void XiaoBianValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

      

        private void ChaBanValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       

        private void FuQiuValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

       
        private void AngleValve_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ClSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void ClSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void NTUSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void NTUSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CODSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void CODSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void NH3Sensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void NH3Sensor_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ShaGangFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void ShaGangFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void PanShiFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void PanShiFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void WuFaFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void WuFaFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SanDuanShiFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void SanDuanShiFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void QiFuEquipment_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void QiFuEquipment_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void HunNingJi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void HunNingJi_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void XiaoDuJi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void XiaoDuJi_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void NaClO_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void NaClO_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ZuGouJi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void ZuGouJi_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SewageTreat1T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void SewageTreat1T_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SewageTreat3T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void SewageTreat3T_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SewageTreat5T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void SewageTreat5T_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SewageTreat10T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void SewageTreat10T_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SewageTreat15T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void SewageTreat15T_MouseDown(object sender, MouseButtonEventArgs e)
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
