using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// PumpStationForm.xaml 的交互逻辑
    /// </summary>
    public partial class PumpStationForm : Window
    {
        ExecuteEventPumpStation excPumpStation = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerPumpStation = null;

        ObservableCollection<RoomInfo> roomInfoList = new ObservableCollection<RoomInfo>();//DataGrid的数据源
        ObservableCollection<RoomInfo> roomInfoSelectList = new ObservableCollection<RoomInfo>();//用于DataGrid的模板加载时提供选项

        List<string> roomNames = new List<string>() { "水泵间", "PUMP ROOM" , "PUMP ROOM" + "\n" + "     " + "水泵间" , "加药间",
        "DOSING ROOM", "DOSING ROOM" + "\n" + "       " + "加药间","控制室","CONTROL ROOM","CONTROL ROOM" + "\n" + "         " + "控制室",
       "储药间", "CHEMICAL STORAGE ROOM","CHEMICAL STORAGE ROOM" + "\n" + "                 " + "储药间" ,"水质监测室", "WATER QUALITY MONITORING ROOM" ,
        "WATER QUALITY MONITORING ROOM" + "\n" + "                       " + "水质监测室" };
        public PumpStationForm()
        {
            InitializeComponent();
            excPumpStation = new ExecuteEventPumpStation();
            eventHandlerPumpStation = Autodesk.Revit.UI.ExternalEvent.Create(excPumpStation);

            roomInfoList.Add(new RoomInfo() { RoomCode = "房间1", RoomLength = "12000", RoomNameList = "水泵间", RoomBottomList = "0.0" });
            roomInfoList.Add(new RoomInfo() { RoomCode = "房间2", RoomLength = "4500", RoomNameList = "控制室", RoomBottomList = "0.0" });
            roomInfoList.Add(new RoomInfo() { RoomCode = "房间3", RoomLength = "4000", RoomNameList = "加药间", RoomBottomList = "0.0" });

            foreach (var item in roomNames)
            {
                roomInfoSelectList.Add(new RoomInfo() { RoomNameList = item });
            }

            RoomSettingGrid.ItemsSource = roomInfoList;//绑定数据源
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            RoomMirro.IsChecked = false;
            Task.Delay(100).ContinueWith(t => LoadWithDelay(), TaskScheduler.FromCurrentSynchronizationContext()); //控件延时显示
        }
        private void LoadWithDelay()
        {
            RoomSettingGrid.Visibility = Visibility.Visible;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTextValue(RoomWidth.Text) || CheckTextValue2(RoomHeight.Text))
            {
                RoomWidth.Text = "6500";
                RoomHeight.Text = "4.0";
            }
            else
            {
                eventHandlerPumpStation.Raise();
                Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
        public string GetComBoxValue(int rowIndex, int cellIndex, string textBlockName)
        {
            DataGridTemplateColumn tempColumn = RoomSettingGrid.Columns[cellIndex] as DataGridTemplateColumn;
            FrameworkElement element = RoomSettingGrid.Columns[cellIndex].GetCellContent(RoomSettingGrid.Items[rowIndex]);
            TextBlock tbox = tempColumn.CellTemplate.FindName(textBlockName, element) as TextBlock;
            return tbox.Text;
        }
        public string GetTextBlockValue(int rowIndex, int cellIndex)
        {
            string name;
            name = (RoomSettingGrid.Columns[cellIndex].GetCellContent(RoomSettingGrid.Items[rowIndex]) as TextBlock).Text;
            return name;
        }
        /// <summary>
        /// RoomNameDropDownClosed 房间名称下拉列表框选择改变刷新
        /// </summary>
        private void ComboBoxRoomName_DropDownClosed(object sender, EventArgs e)
        {
            DataGridHelper.SetRealTimeCommit(RoomSettingGrid, true);//dataGrid为控件名称
        }
        /// <summary>
        /// RoomNameLoaded 房间名称下拉列表框初始化，绑定数据源
        /// </summary>
        private void ComboBoxRoomName_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox curComboBox = sender as ComboBox;
            //为下拉控件绑定数据源，并选择原选项为默认选项
            string text = curComboBox.Text;
            //去除重复项查找，跟数据库连接时可以让数据库来实现
            var query = roomInfoSelectList.GroupBy(p => p.RoomNameList).Select(p => new { RoomNameList = p.FirstOrDefault().RoomNameList });
            int itemcount = 0;
            curComboBox.SelectedIndex = itemcount;
            foreach (var item in query.ToList())
            {
                if (item.RoomNameList == text)
                {
                    curComboBox.SelectedIndex = itemcount;
                    break;
                }
                itemcount++;
            }
            curComboBox.ItemsSource = query;
            curComboBox.IsDropDownOpen = true;//获得焦点后下拉
        }

        /// <summary>
        /// 单击DataGrid下拉ComboBox
        /// </summary>
        private void RoomSettingGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            RoomSettingGrid.BeginEdit();
        }
        private void RoomSettingGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.Column.ToString() == "System.Windows.Controls.DataGridTemplateColumn.CellEditingTemplate")
                ((ComboBox)(e.EditingElement)).IsDropDownOpen = true;
        }

        #region 方法
        /// <summary>
        /// 单击TextBox文字全选
        /// </summary>    
        private void RoomNum_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            tb.Focus();
            e.Handled = true;
        }
        private void RoomNum_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            tb.SelectAll();
            tb.PreviewMouseDown -= new MouseButtonEventHandler(RoomNum_PreviewMouseDown);
        }

        private void RoomNum_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            tb.PreviewMouseDown += new MouseButtonEventHandler(RoomNum_PreviewMouseDown);
        }
        #endregion
        private void RoomNumModifyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int txt = int.Parse(RoomNum.Text);
                if (!(int.Parse(RoomNum.Text) > 0))
                {
                    MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    RoomNum.Text = "3";
                }
                int num = int.Parse(RoomNum.Text);
                int count = RoomSettingGrid.Items.Count;
                if (num >= count)
                {
                    for (int i = 0; i < num - count; i++)
                    {
                        roomInfoList.Add(new RoomInfo() { RoomCode = "房间" + (count + i + 1).ToString(), RoomLength = "12000", RoomNameList = "水泵间", RoomBottomList = "0.0" });
                    }

                }
                if (num <= count)
                {
                    int countExist = RoomSettingGrid.Items.Count;
                    for (int i = 0; i < countExist - num; i++)
                    {
                        foreach (var item in roomInfoList.ToArray())
                        {
                            if (item.RoomCode.Contains((countExist - i).ToString()))
                            {
                                roomInfoList.Remove(item);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message + "请输入大于0的正整数!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                RoomNum.Text = "3";
            }
        }
        private bool CheckTextValue(string value)
        {
            bool isnum = false;
            try
            {
                if (!(int.Parse(value) > 0))
                {
                    isnum = true;
                    MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception exp)
            {
                isnum = true;
                MessageBox.Show(exp.Message + "请输入大于0的正整数!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return isnum;
        }
        private bool CheckTextValue2(string value)
        {
            bool isnum = false;
            try
            {
                double.Parse(value);
            }
            catch (Exception exp)
            {
                isnum = true;
                MessageBox.Show(exp.Message + "请输入大于0的正整数!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return isnum;
        }
    }
}
