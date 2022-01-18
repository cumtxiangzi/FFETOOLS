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
        public int index = 0;
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
        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            eventHandlerCreatWaterFamily.Dispose();
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
            MeterList.Visibility = Visibility.Visible;
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

        #region 族名称显示
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
            FamilyName.Text = "微阻缓闭式止回阀";
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
            FamilyName.Text = "电磁流量计";
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
        private void ClSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "余(总)氯在线分析仪";
        }
        private void NTUSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "在线浊度仪";
        }
        private void CODSensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "COD在线分析仪";
        }
        private void NH3Sensor_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "NH3-N在线分析仪";
        }
        private void ShaGangFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "砂缸过滤器";
        }
        private void PanShiFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "盘式过滤器";
        }
        private void WuFaFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "重力式无阀过滤器";
        }
        private void QiFuEquipment_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "气浮溶气装置";
        }
        private void SanDuanShiFilter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "三段式给水处理设备";
        }
        private void HunNingJi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "混凝剂加药装置";
        }
        private void ShaJunJi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "杀菌剂加药装置";
        }
        private void NaClO_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "NaCLO加药装置";
        }
        private void ZuGouJi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "阻垢剂加药装置";
        }
        private void SewageTreat1T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "一体式污水处理设备1T";
        }
        private void SewageTreat3T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "一体式污水处理设备3T";
        }
        private void SewageTreat5T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "一体式污水处理设备5T";
        }
        private void SewageTreat10T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "一体式污水处理设备10T";
        }
        private void SewageTreat15T_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "一体式污水处理设备15T";
        }
        private void WaterTank_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "矩形给水箱";
        }
        private void EyeWasher_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "复合式冲淋洗眼器";
        }
        private void HuiHeTee_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "90°汇合三通井座";
        }
        private void HuiHeCross_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "90°汇合四通井座";
        }
        private void ZhiLiElbow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "直立90°弯头井座";
        }
        private void ElbowWell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "90°弯头井座";
        }
        private void ZuoYouTee_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "90°左(右)三通井座";
        }
        private void ZhiTongTee_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "直通式井座";
        }
        private void ZhuanQiValveWell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "砖砌圆形阀门井";
        }
        private void ZhuanQiCheckWell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "砖砌排水检查井";
        }
        private void ConcretCheckWell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "钢筋混凝土排水检查井";
        }
        private void DrainageDitch_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "室内排水沟";
        }
        private void ConcretHuaFenChi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "钢筋混凝土化粪池";
        }
        private void IndoorHydrant_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "室内消火栓箱(明装)";
        }
        private void ShouTiExtinguisher_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "手提干粉灭火器";
        }
        private void TuiCheExtinguisher_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "推车式干粉灭火器";
        }
        private void OilTank_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "柴油消防泵油箱";
        }
        private void GroundAdapter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "地上式水泵接合器";
        }
        private void UnderGroundAdapter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "地下式水泵接合器";
        }
        private void WallAdapter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "墙壁式水泵接合器";
        }
        private void FM200GuiShi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "柜式预制灭火装置(七氟丙烷)";
        }
        private void FM200Cylinder_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "七氟丙烷灭火系统瓶组";
        }
        private void UpGasNozzle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "气体喷头(上喷)";
        }
        private void DownGasNozzle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "气体喷头(下喷)";
        }
        private void IG541Cylinder_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "IG541灭火系统瓶组";
        }
        private void NFPAⅢHydrant_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "NFPA Class III 消火栓箱(明装)";
        }
        private void NPFAⅡHydrant_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "NFPA Class Ⅱ 消火栓箱(明装)";
        }
        private void WenYaPump_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "稳压装置";
        }
        private void GroundHydrant_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "室外地上式消火栓";
        }
        private void UnderGroundHydrant_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "室外地下式消火栓";
        }
        private void ElevationNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "标高";
        }
        private void GroundNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "地面符号";
        }
        private void SoilNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "土壤符号";
        }
        private void WaterLevelNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "水位线";
        }
        private void OnlyArrowNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "纯箭头";
        }
        private void ArrowNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "箭头";
        }
        private void BreakLineNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "折断线";
        }
        private void BreakPipeNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "管道折断线";
        }
        private void AxisNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "轴线";
        }
        private void CompassNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "指北针";
        }
        private void SleeveNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "刚性防水套管";
        }
        private void SectionNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "剖面符号";
        }
        private void ElectricBoxNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "潜水泵控制箱";
        }
        private void ArrowSlopeNote_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "坡度箭头";
        }
        private void LouLvAlarm_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "漏氯报警仪";
        }
        private void WaterFlowGlass_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FamilyName.Text = "水流视镜";
        }
        #endregion
        private void ButterflyValveWoLun_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 1;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ButterflyValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 2;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void GateValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 3;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void GateValveFaLan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 4;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void StopValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 5;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void E_ButterflyValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 6;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void E_GateValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 7;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void CheckValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 8;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void CheckValveWeiZu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 9;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void VentValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 10;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void PressureValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 11;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void BallValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 12;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ControlValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 13;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SolenoidValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 14;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void DaBianValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 15;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void XiaoBianValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 16;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ChaBanValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 17;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void FuQiuValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 18;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void AngleValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 19;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void WaterMeterXuanYi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 20;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void WaterMeterLuoYi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 21;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void FlowMeter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 22;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void PressureGauge_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 23;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void VacuumMeter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 24;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void Thermometer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 25;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

        private void PressureSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 26;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void TemperatureSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 27;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void NTUSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 28;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ClSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 29;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void CODSensor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 30;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void NH3Sensor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 31;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void RubberJoint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 32;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void TypeYFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 33;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void WaterTap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 34;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SuctionBell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 35;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void OverFlowBell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 36;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void FloorDrain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 37;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void VentTap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 38;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void CleanOut_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 39;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void CheckOut_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 40;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SinglePump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 41;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void DoublePump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 42;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void VerticalPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 43;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void LongShaftPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 44;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ChaiYouPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 45;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ZiXiPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 46;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void HengYaPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 47;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void DieYaPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 48;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void DingYaPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 49;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void QianShuiPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 50;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void GuDingPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 51;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

        private void PipePump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 52;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void WenYaPump_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 53;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void OilTank_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 54;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ShaGangFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 55;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void PanShiFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 56;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void WuFaFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 57;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SanDuanShiFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 58;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void QiFuEquipment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 59;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void HunNingJi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 60;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ShaJunJi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 61;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void NaClO_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 62;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ZuGouJi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 63;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SewageTreat1T_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 64;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SewageTreat3T_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 65;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SewageTreat5T_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 66;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SewageTreat10T_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 67;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SewageTreat15T_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 68;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void EyeWasher_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 69;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void WaterTank_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 70;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void DrainageDitch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 71;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ZhuanQiValveWell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 72;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ZhuanQiCheckWell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 73;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ConcretCheckWell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 74;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ConcretHuaFenChi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 75;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ZhiLiElbow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 76;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ElbowWell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 77;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void HuiHeTee_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 78;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void HuiHeCross_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 79;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ZuoYouTee_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 80;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ZhiTongTee_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 81;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void IndoorHydrant_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 82;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ShouTiExtinguisher_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 83;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void TuiCheExtinguisher_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 84;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void GroundAdapter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 85;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void UnderGroundAdapter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 86;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void WallAdapter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 87;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void FM200GuiShi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 88;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void FM200Cylinder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 89;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void UpGasNozzle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 90;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void DownGasNozzle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 91;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void IG541Cylinder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 92;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void NFPAⅢHydrant_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 93;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void NPFAⅡHydrant_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 94;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void GroundHydrant_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 95;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void UnderGroundHydrant_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 96;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ElevationNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 97;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void GroundNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 98;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SoilNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 99;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void WaterLevelNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 100;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void OnlyArrowNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index =101;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ArrowNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 102;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ArrowSlopeNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 103;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }

        private void BreakLineNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 104;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void BreakPipeNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 105;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void AxisNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 106;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void CompassNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 107;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SleeveNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 108;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void SectionNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 109;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void ElectricBoxNote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 110;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }
        private void LouLvAlarm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 111;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
        }     
        private void WaterFlowGlass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                index = 112;
                Helper.SendKeys(Autodesk.Windows.ComponentManager.ApplicationWindow, Keys.Escape);
                eventHandlerCreatWaterFamily.Raise();
                e.Handled = true;
            }
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
