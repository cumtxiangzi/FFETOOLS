using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
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
using System.IO;
using Path = System.IO.Path;

namespace FFETOOLS
{
    /// <summary>
    ///  PumpSelectForm.xaml 的交互逻辑
    /// </summary>
    public partial class PumpSelectForm : Window
    {
        List<string> ManufactureList = new List<string>() { "山东双轮泵业", "南方泵业", "上海东方泵业", "上海连成泵业" };
        List<string> ModelList = new List<string>() { "IS", "S", "DL" };
        public PumpGroupForm mainForm { get; set; }
        public List<PumpData> PumpDataSource = new List<PumpData>();

        public PumpSelectForm(PumpGroupForm groupForm)
        {
            InitializeComponent();
            mainForm = groupForm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RoomSettingGrid.ItemsSource = PumpDataSource;

            PumpManufacture.ItemsSource = ManufactureList;
            PumpManufacture.SelectedIndex = 0;

            PumpModel.ItemsSource = ModelList;
            PumpModel.SelectedIndex = 0;

            PumpSection.Source = new BitmapImage(new Uri(@"/MainWorkShop;component/Resources/IS泵参数图示.jpg", UriKind.Relative));

            Flow.Text = "25";
            Lift.Text = "20";
            Task.Delay(100).ContinueWith(t => LoadWithDelay(), TaskScheduler.FromCurrentSynchronizationContext()); //控件延时显示
        }
        private void LoadWithDelay()
        {
            RoomSettingGrid.Visibility = Visibility.Visible;
        }
        private void PumpManufacture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string manufactureName = PumpManufacture.SelectedItem.ToString();
            if (manufactureName.Contains("双轮"))
            {
                PumpModel.SelectedIndex = 0;
                RoomSettingGrid.ItemsSource = mainForm.ShuangLunPumpData;
            }
            if (manufactureName.Contains("连成"))
            {
                PumpModel.SelectedIndex = 1;
                RoomSettingGrid.ItemsSource = mainForm.LianChengPumpData;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {         
            string flow=Flow.Text;
            string lift=Lift.Text;
            string manufactureName = PumpManufacture.SelectedItem.ToString();

            if (manufactureName.Contains("双轮"))
            {
                SelectPumpData(flow,lift, mainForm.ShuangLunPumpData);
            }
            if (manufactureName.Contains("连成"))
            {
                SelectPumpData(flow, lift, mainForm.LianChengPumpData);
            }
           
        }
        public void SelectPumpData(string flow, string lift,List<PumpData> pumpCheckData)
        {
            List<PumpData> checkData = new List<PumpData>();
            foreach (var data in pumpCheckData)
            {
                if (data.Flow == flow && data.Lift == lift)
                {
                    checkData.Add(data);
                }
            }

            if (checkData.Count != 0)
            {
                RoomSettingGrid.ItemsSource = checkData;
            }
            else
            {
                MessageBox.Show("没有合适型号的水泵!", "GPSBIM", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //Close();
            string manufactureName = PumpManufacture.SelectedItem.ToString();
            if (manufactureName.Contains("双轮"))
            {
                PumpModel.SelectedIndex = 0;
                RoomSettingGrid.ItemsSource = mainForm.ShuangLunPumpData;
            }
            if (manufactureName.Contains("连成"))
            {
                PumpModel.SelectedIndex = 1;
                RoomSettingGrid.ItemsSource = mainForm.LianChengPumpData;
            }
        }

        private void RoomSettingGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            Point aP = e.GetPosition(datagrid);
            IInputElement obj = datagrid.InputHitTest(aP);
            DependencyObject target = obj as DependencyObject;

            while (target != null)
            {
                if (target is DataGridRow)
                {
                    PumpData s = RoomSettingGrid.SelectedItem as PumpData;
                    //MessageBox.Show(s.Model + "\n" + s.Flow + "\n" + s.Lift + "\n" + 
                    //    s.Power + "\n" + s.Weight+"\n"+s.InletSize+"\n"+s.OutletSize+ "\n" + s.OutletHeight+"\n" + s.BaseLength);
                    mainForm.PumpModel.Text = s.Model;
                    mainForm.OutletHeight.Text = s.OutletHeight;
                    mainForm.InletDiameter.Text = s.InletSize;
                    mainForm.OutletDiameter.Text = s.OutletSize;

                    mainForm.BaseLength.Text = s.BaseLength;
                    mainForm.BaseWidth.Text = s.BaseWidth;
                    mainForm.HoleLength.Text = s.BoltHoleLength;
                    mainForm.HoleWidth.Text = s.BoltHoleWidth;
                    mainForm.HoleSize.Text = s.BoltHoleSize;
                }
                target = VisualTreeHelper.GetParent(target);
            }

            Close();
        }
    }
}
