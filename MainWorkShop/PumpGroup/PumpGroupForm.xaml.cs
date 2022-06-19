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
//using SQLiteDemo;

namespace FFETOOLS
{
    /// <summary>
    /// PumpGroupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PumpGroupForm : Window
    {
        public int InletHeightValue { get; set; }
        public int OutletHeightValue { get; set; }
        public int BaseLengthValue { get; set; }
        public int BaseWidthValue { get; set; }
        public int HoleLengthValue { get; set; }
        public int HoleWidthValue { get; set; }
        public int HoleSizeValue { get; set; }
        public int ClickNum { get; set; }
        public string PumpModelValue { get; set; }
        public List<PumpData> ShuangLunPumpData { get; set; }
        public List<PumpData> LianChengPumpData { get; set; }

        ExecuteEventPumpGroup excPumpGroup = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerPumpGroup = null;
        public PumpGroupForm(List<string> pipeSizeList, List<string> pipeTypeList, List<string> pipeSystemList
            ,List<PumpData> shuangLun,List<PumpData> lianCheng)
        {
            InitializeComponent();

            ShuangLunPumpData = shuangLun;
            LianChengPumpData = lianCheng;

            InletDiameter.ItemsSource = pipeSizeList;
            OutletDiameter.ItemsSource = pipeSizeList;
            InletDiameter.SelectedIndex = 8;
            OutletDiameter.SelectedIndex = 7;

            InLetPipeSize.ItemsSource = pipeSizeList;
            OutLetPipeSize.ItemsSource = pipeSizeList;
            InLetPipeSize.SelectedIndex = 11;
            OutLetPipeSize.SelectedIndex = 10;

            PipeType.ItemsSource = pipeTypeList;
            PipeSystemType.ItemsSource = pipeSystemList;
            PipeType.SelectedIndex = 2;
            PipeSystemType.SelectedIndex = 5;

            excPumpGroup = new ExecuteEventPumpGroup();
            eventHandlerPumpGroup = Autodesk.Revit.UI.ExternalEvent.Create(excPumpGroup);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            PumpModelValue = PumpModel.Text;

            if (int.TryParse(InletHeight.Text, out int number1))
            {
                InletHeightValue = int.Parse(InletHeight.Text);
            }

            if (int.TryParse(OutletHeight.Text, out int number2))
            {
                OutletHeightValue = int.Parse(OutletHeight.Text);
            }

            if (int.TryParse(BaseLength.Text, out int number3))
            {
                BaseLengthValue = int.Parse(BaseLength.Text);
            }

            if (int.TryParse(BaseWidth.Text, out int number4))
            {
                BaseWidthValue = int.Parse(BaseWidth.Text);
            }

            if (int.TryParse(HoleLength.Text, out int number5))
            {
                HoleLengthValue = int.Parse(HoleLength.Text);
            }

            if (int.TryParse(HoleWidth.Text, out int number6))
            {
                HoleWidthValue = int.Parse(HoleWidth.Text);
            }
            if (int.TryParse(HoleSize.Text, out int number7))
            {
                HoleSizeValue = int.Parse(HoleSize.Text);
            }

            if (PumpModelValue == "")
            {
                MessageBox.Show("请输入水泵型号！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                PumpModel.Text = "";
                PumpModel.Focus();
            }
            else if (!(InletHeightValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                InletHeight.Text = "";
                InletHeight.Focus();
            }
            else if (!(OutletHeightValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                OutletHeight.Text = "";
                OutletHeight.Focus();
            }
            else if (!(BaseLengthValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                BaseLength.Text = "";
                BaseLength.Focus();
            }
            else if (!(BaseWidthValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                BaseWidth.Text = "";
                BaseWidth.Focus();
            }
            else if (!(HoleLengthValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                HoleLength.Text = "";
                HoleLength.Focus();
            }
            else if (!(HoleWidthValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                HoleWidth.Text = "";
                HoleWidth.Focus();
            }
            else if (!(HoleSizeValue > 0))
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                HoleSize.Text = "";
                HoleSize.Focus();
            }
            else
            {
                ClickNum = 1;
                eventHandlerPumpGroup.Raise();
                Hide();
            }
        }

        private void PumpSelectButton_Click(object sender, RoutedEventArgs e)
        {
            ClickNum = 2;
            eventHandlerPumpGroup.Raise();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            S_HorizontalButton.IsChecked = true;                    
            PumpModel.Focus();
            PumpModel.Text = "IS";
            InletHeight.Text = "400";
            OutletHeight.Text = "700";
            BaseLength.Text = "1200";
            BaseWidth.Text = "600";
            HoleLength.Text = "800";
            HoleWidth.Text = "300";
            HoleSize.Text = "100";
        }

        private void PipeSystemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PipeSystemType.SelectedItem.ToString().Contains("消防"))
            {
                PipeType.SelectedIndex = 3;
            }
            if (PipeSystemType.SelectedItem.ToString().Contains("循环") || PipeSystemType.SelectedItem.ToString().Contains("中水") || PipeSystemType.SelectedItem.ToString().Contains("污水"))
            {
                PipeType.SelectedIndex = 2;
            }
            if (PipeSystemType.SelectedItem.ToString().Contains("生活"))
            {
                PipeType.SelectedIndex = 3;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }

        private void S_HorizontalButton_Checked(object sender, RoutedEventArgs e)
        {
            PumpImage.Source = new BitmapImage(new Uri(@"/MainWorkShop;component/Resources/卧式单吸泵.jpg", UriKind.Relative));
            BaseParameter.Visibility = Visibility.Visible;
        }

        private void M_HorizontalButton_Checked(object sender, RoutedEventArgs e)
        {
            PumpImage.Source = new BitmapImage(new Uri(@"/MainWorkShop;component/Resources/卧式双吸泵.jpg", UriKind.Relative));
            //BaseParameter.Visibility = Visibility.Hidden;
        }

        private void VerticalButton_Checked(object sender, RoutedEventArgs e)
        {
            PumpImage.Source = new BitmapImage(new Uri(@"/MainWorkShop;component/Resources/立式水泵.jpg", UriKind.Relative));
        }

        private void GroupPumpButton_Checked(object sender, RoutedEventArgs e)
        {
            PumpImage.Source = new BitmapImage(new Uri(@"/MainWorkShop;component/Resources/水泵组.jpg", UriKind.Relative));
        }
    }
}
