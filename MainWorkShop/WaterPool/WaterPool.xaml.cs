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
    /// WaterPoolForm.xaml 的交互逻辑
    /// </summary>
    public partial class WaterPoolForm : Window
    {
        public int PoolLengthValue { get; set; }
        public int PoolWidthValue { get; set; }
        public int PoolHeightValue { get; set; }
        public int SumpLengthValue { get; set; }
        public int SumpWidthValue { get; set; }
        public int SumpHeightValue { get; set; }
        public int ManHoleSizeValue { get; set; }
        public double PoolBottomElevationValue { get; set; }

        ExecuteEventWaterPool excWaterPool = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerWaterPool = null;
        public WaterPoolForm()
        {
            InitializeComponent();

            excWaterPool = new ExecuteEventWaterPool();
            eventHandlerWaterPool = Autodesk.Revit.UI.ExternalEvent.Create(excWaterPool);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PoolColumn.IsChecked = true;
            PoolShape.IsChecked = true;

            PoolLength.Text = "11700";
            PoolWidth.Text = "11700";
            PoolHeight.Text = "4000";
            PoolBottomElevation.Text = "-2.5";
            SumpLength.Text = "1500";
            SumpWidth.Text = "1500";
            SumpHeight.Text = "1500";
            ManHoleSize.Text = "1000";

            string[] StandSizeList = new string[] { "50", "100", "150", "200", "300", "400", "500", "600", "800", "1000", "1500", "2000" };
            StandardSize.ItemsSource = StandSizeList;
            StandardSize.SelectedIndex = 6;

            string[] FlowList = new string[] { "8", "15", "30", "50", "75", "100", "150", "200", "300", "400", "500", "600", "700", "1000", };
            CoolTowerFlow.ItemsSource = FlowList;
            CoolTowerFlow.SelectedIndex = 10;
            CoolTowerFlow.IsEnabled = false;
            CoolTower.IsChecked = false;
            
            string[] TexeList = new string[] { "生产循环水池", "消防水池"};
            PoolText.ItemsSource = TexeList;
            PoolText.SelectedIndex = 0;

            string volumn = "V=" + StandardSize.Text + "m³";
            PoolVolumnText.Text = "水池有效容积"+"\n"+"   "+volumn;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PoolLength.Text, out int number1))
            {
                PoolLengthValue = int.Parse(PoolLength.Text);
            }

            if (int.TryParse(PoolWidth.Text, out int number2))
            {
                PoolWidthValue = int.Parse(PoolWidth.Text);
            }

            if (int.TryParse(PoolHeight.Text, out int number3))
            {
                PoolHeightValue = int.Parse(PoolHeight.Text);
            }

            if (int.TryParse(SumpLength.Text, out int number4))
            {
                SumpLengthValue = int.Parse(SumpLength.Text);
            }

            if (int.TryParse(SumpWidth.Text, out int number5))
            {
                SumpWidthValue = int.Parse(SumpWidth.Text);
            }

            if (int.TryParse(SumpHeight.Text, out int number6))
            {
                SumpHeightValue = int.Parse(SumpHeight.Text);
            }

            if (int.TryParse(ManHoleSize.Text, out int number7))
            {
                ManHoleSizeValue = int.Parse(ManHoleSize.Text);
            }

            if (double.TryParse(PoolBottomElevation.Text, out double number8))
            {
                PoolBottomElevationValue = double.Parse(PoolBottomElevation.Text);
            }

            if (!(PoolLengthValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                PoolLength.Text = "";
                PoolLength.Focus();
            }
            else if (!(PoolWidthValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                PoolWidth.Text = "";
                PoolWidth.Focus();
            }
            else if (!(PoolHeightValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                PoolHeight.Text = "";
                PoolHeight.Focus();
            }
            else if (!(SumpLengthValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                SumpLength.Text = "";
                SumpLength.Focus();
            }
            else if (!(SumpWidthValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                SumpWidth.Text = "";
                SumpWidth.Focus();
            }
            else if (!(SumpHeightValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                SumpHeight.Text = "";
                SumpHeight.Focus();
            }
            else if (!(ManHoleSizeValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                ManHoleSize.Text = "";
                ManHoleSize.Focus();
            }
            else if (!(double.TryParse(PoolBottomElevation.Text, out double number9)))
            {
                MessageBox.Show("请输入数字！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                PoolBottomElevation.Text = "";
                PoolBottomElevation.Focus();
            }
            else
            {
                eventHandlerWaterPool.Raise();
                Hide();
            }
        }

        private void StandardSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string volumn = "V=" + StandardSize.SelectedItem.ToString() + "m³";
            PoolVolumnText.Text = "水池有效容积" + "\n" + "   " + volumn;

            if (StandardSize.SelectedItem.ToString() == "50")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "3900";
                    PoolWidth.Text = "3900";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "6300";
                    PoolWidth.Text = "3000";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "100")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "5600";
                    PoolWidth.Text = "5600";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "8000";
                    PoolWidth.Text = "4000";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "150")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "6800";
                    PoolWidth.Text = "6800";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "9450";
                    PoolWidth.Text = "5000";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "200")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "7800";
                    PoolWidth.Text = "7800";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "9600";
                    PoolWidth.Text = "6300";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "300")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "9900";
                    PoolWidth.Text = "9900";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "13900";
                    PoolWidth.Text = "6900";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "400")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "11400";
                    PoolWidth.Text = "11400";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "16000";
                    PoolWidth.Text = "8000";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "500")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "11700";
                    PoolWidth.Text = "11700";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "16400";
                    PoolWidth.Text = "8200";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "600")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "12900";
                    PoolWidth.Text = "12900";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "20000";
                    PoolWidth.Text = "8000";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "800")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "14800";
                    PoolWidth.Text = "14800";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "18800";
                    PoolWidth.Text = "11200";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "1000")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "15900";
                    PoolWidth.Text = "15900";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "22800";
                    PoolWidth.Text = "11400";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "1500")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "19800";
                    PoolWidth.Text = "19800";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "26400";
                    PoolWidth.Text = "15000";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "2000")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "23400";
                    PoolWidth.Text = "23400";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "27300";
                    PoolWidth.Text = "19500";
                    PoolHeight.Text = "4000";
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }

        private void PoolShape_Click(object sender, RoutedEventArgs e)
        {
            if (StandardSize.SelectedItem.ToString() == "50")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "3900";
                    PoolWidth.Text = "3900";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "6300";
                    PoolWidth.Text = "3000";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "100")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "5600";
                    PoolWidth.Text = "5600";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "8000";
                    PoolWidth.Text = "4000";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "150")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "6800";
                    PoolWidth.Text = "6800";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "9450";
                    PoolWidth.Text = "5000";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "200")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "7800";
                    PoolWidth.Text = "7800";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "9600";
                    PoolWidth.Text = "6300";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "300")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "9900";
                    PoolWidth.Text = "9900";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "13900";
                    PoolWidth.Text = "6900";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "400")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "11400";
                    PoolWidth.Text = "11400";
                    PoolHeight.Text = "3500";
                }
                else
                {
                    PoolLength.Text = "16000";
                    PoolWidth.Text = "8000";
                    PoolHeight.Text = "3500";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "500")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "11700";
                    PoolWidth.Text = "11700";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "16400";
                    PoolWidth.Text = "8200";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "600")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "12900";
                    PoolWidth.Text = "12900";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "20000";
                    PoolWidth.Text = "8000";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "800")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "14800";
                    PoolWidth.Text = "14800";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "18800";
                    PoolWidth.Text = "11200";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "1000")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "15900";
                    PoolWidth.Text = "15900";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "22800";
                    PoolWidth.Text = "11400";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "1500")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "19800";
                    PoolWidth.Text = "19800";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "26400";
                    PoolWidth.Text = "15000";
                    PoolHeight.Text = "4000";
                }
            }
            else if (StandardSize.SelectedItem.ToString() == "2000")
            {
                if (PoolShape.IsChecked == true)
                {
                    PoolLength.Text = "23400";
                    PoolWidth.Text = "23400";
                    PoolHeight.Text = "4000";
                }
                else
                {
                    PoolLength.Text = "27300";
                    PoolWidth.Text = "19500";
                    PoolHeight.Text = "4000";
                }
            }
        }

        private void CoolTower_Click(object sender, RoutedEventArgs e)
        {
            if (CoolTower.IsChecked == true)
            {
                CoolTowerFlow.IsEnabled = true;
            }
            else
            {
                CoolTowerFlow.IsEnabled = false;
            }
        }
    }
}
