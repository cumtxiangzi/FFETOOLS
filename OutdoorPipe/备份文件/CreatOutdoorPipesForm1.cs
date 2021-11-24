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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input.Test;
using System.Windows.Threading;
using System.Collections;

namespace FFETOOLS
{
    /// <summary>
    /// CreatOutdoorPipesForm.xaml 的交互逻辑
    /// </summary>
    public partial class CreatOutdoorPipesForm : Window
    {
        ExecuteEventCreatOutdoorPipes excCreatOutdoorPipes = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatOutdoorPipes = null;
        private void SendToUIThread(UIElement element, string text)
        {
            element.Dispatcher.BeginInvoke(new Action(() => { SendKeys.Send(element, text); }), DispatcherPriority.Input);
        }

        ObservableCollection<OutdoorPipeInfo> pipeInfoList = new ObservableCollection<OutdoorPipeInfo>();//DataGrid的数据源
        ObservableCollection<OutdoorPipeInfo> pipeInfoSelectList = new ObservableCollection<OutdoorPipeInfo>();//用于DataGrid的模板加载时提供选项
        string[] ProfessionList = new string[4] { "工艺专业(PD)", "给排水专业(WD)", "暖通专业(VD)", "电气专业(E)" };

        public CreatOutdoorPipesForm(List<OutdoorPipeSizeInfo> pipeSizeInfoList)
        {
            InitializeComponent();

            //三级联动数据项
            foreach (var info in pipeSizeInfoList)
            {
                foreach (var size in info.PipeSizeList)
                {
                    pipeInfoSelectList.Add(new OutdoorPipeInfo(info.PipeSystemName, info.PipeTypeName, size));
                }
            }
            PipeSettingGrid.ItemsSource = pipeInfoList;//绑定数据源
        }
        public CreatOutdoorPipesForm()
        {

        }
        /// <summary>
        /// PipeSystemLoaded 管道系统下拉列表框初始化，绑定数据源
        /// </summary>
        void PipeSystemLoaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ComboBox curComboBox = sender as System.Windows.Controls.ComboBox;
            //为下拉控件绑定数据源，并选择原选项为默认选项
            string text = curComboBox.Text;
            //去除重复项查找，跟数据库连接时可以让数据库来实现
            var query = pipeInfoSelectList.GroupBy(p => p.PipeSystem).Select(p => new { PipeSystem = p.FirstOrDefault().PipeSystem });
            int itemcount = 0;
            curComboBox.SelectedIndex = itemcount;
            foreach (var item in query.ToList())
            {
                if (item.PipeSystem == text)
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
        /// PipeTypeLoaded 管道类型下拉列表框初始化，绑定数据源
        /// </summary>
        void PipeTypeLoaded(object sender, RoutedEventArgs e)
        {
            //获得当前选中项的管道系统信息
            string pipeSystem = (PipeSettingGrid.SelectedItem as OutdoorPipeInfo).PipeSystem;
            //查找选中管道系统下的管道类型作为数据源
            var query = (from l in pipeInfoSelectList
                         where (l.PipeSystem == pipeSystem)
                         group l by l.PipeType into grouped
                         select new { PipeType = grouped.Key });
            ComboBox curComboBox = sender as ComboBox;
            //为下拉控件绑定数据源，并选择原选项为默认选项  
            string text = curComboBox.Text;
            //去除重复项查找，跟数据库连接时可以让数据库来实现
            int itemcount = 0;
            curComboBox.SelectedIndex = itemcount;
            foreach (var item in query.ToList())
            {
                if (item.PipeType == text)
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
        /// PipeSizeLoaded 管径下拉列表框初始化，绑定数据源
        /// </summary>
        void PipeSizeLoaded(object sender, RoutedEventArgs e)
        {
            string pipeSystem = (PipeSettingGrid.SelectedItem as OutdoorPipeInfo).PipeSystem;
            string pipeType = (PipeSettingGrid.SelectedItem as OutdoorPipeInfo).PipeType;
            //查找选中管道系统下的管道类型作为数据源
            var query = (from l in pipeInfoSelectList
                         where (l.PipeSystem == pipeSystem && l.PipeType == pipeType)
                         group l by l.PipeSize into grouped
                         select new { PipeSize = grouped.Key });

            ComboBox curComboBox = sender as ComboBox;
            //为下拉控件绑定数据源，并选择原选项为默认选项
            string text = curComboBox.Text;
            //去除重复项查找，跟数据库连接时可以让数据库来实现
            int itemcount = 0;
            curComboBox.SelectedIndex = itemcount;
            foreach (var item in query.ToList())
            {
                if (item.PipeSize == text)
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
        /// PipeSystemDropDownClosed 管道系统下拉列表框选择改变刷新
        /// </summary>
        private void PipeSystemDropDownClosed(object sender, EventArgs e)
        {
            DataGridHelper.SetRealTimeCommit(PipeSettingGrid, true);//dataGrid为控件名称
            SendToUIThread(PipeSettingGrid, "{TAB}!");
        }
        /// <summary>
        /// PipeTypeDropDownClosed 管道类型下拉列表框选择改变刷新
        /// </summary>
        private void PipeTypeDropDownClosed(object sender, EventArgs e)
        {
            DataGridHelper.SetRealTimeCommit(PipeSettingGrid, true);//dataGrid为控件名称
            SendToUIThread(PipeSettingGrid, "{TAB}!");
        }
        /// <summary>
        /// PipeSizeDropDownClosed 管径下拉列表框选择改变刷新
        /// </summary>
        private void PipeSizeDropDownClosed(object sender, EventArgs e)
        {
            DataGridHelper.SetRealTimeCommit(PipeSettingGrid, true);//dataGrid为控件名称
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            PipeQuantityTxt.Text = "2";

            ProfessionCmb.ItemsSource = ProfessionList;
            ProfessionCmb.SelectedIndex = 1;

            excCreatOutdoorPipes = new ExecuteEventCreatOutdoorPipes();
            eventHandlerCreatOutdoorPipes = Autodesk.Revit.UI.ExternalEvent.Create(excCreatOutdoorPipes);
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerCreatOutdoorPipes.Raise();

        }

        private void PipeQuantityTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                Int32 txt = Convert.ToInt32(PipeQuantityTxt.Text);
                if (!(int.Parse(PipeQuantityTxt.Text) > 0))
                {
                    MessageBox.Show("请输入大于0的正整数！", "错误");
                    PipeQuantityTxt.Text = "2";
                }
                int num = int.Parse(PipeQuantityTxt.Text);
                int count = PipeSettingGrid.Items.Count;
                if (num >= count)
                {
                    for (int i = 0; i < num - count; i++)
                    {
                        pipeInfoList.Add(new OutdoorPipeInfo("给排水_循环给水管道系统", "给排水_焊接钢管", "DN200", "管道" + (count + i + 1).ToString(), "500", "3500"));
                    }
                }
                if (num <= count)
                {
                    int countExist = PipeSettingGrid.Items.Count;
                    for (int i = 0; i < countExist - num; i++)
                    {
                        foreach (var item in pipeInfoList.ToArray())
                        {
                            if (item.PipeCode.Contains((countExist - i).ToString()))
                            {
                                pipeInfoList.Remove(item);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message + "请输入大于0的正整数！", "错误");
                PipeQuantityTxt.Text = "2";
            }
        }
        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }

        #region 方法
        private void PipeQuantityTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            tb.PreviewMouseDown += new MouseButtonEventHandler(PipeQuantityTxt_PreviewMouseDown);
        }
        private void PipeQuantityTxt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            tb.Focus();
            e.Handled = true;
        }
        private void PipeQuantityTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            tb.SelectAll();
            tb.PreviewMouseDown -= new MouseButtonEventHandler(PipeQuantityTxt_PreviewMouseDown);
        }
        #endregion

        public string GetComBoxValue(int rowIndex, int cellIndex, string textBlockName)
        {
            DataGridTemplateColumn tempColumn = PipeSettingGrid.Columns[cellIndex] as DataGridTemplateColumn;
            FrameworkElement element = PipeSettingGrid.Columns[cellIndex].GetCellContent(PipeSettingGrid.Items[rowIndex]);
            TextBlock tbox = tempColumn.CellTemplate.FindName(textBlockName, element) as TextBlock;
            return tbox.Text;
        }
        public string GetTextBlockValue(int rowIndex, int cellIndex)
        {
            string name;
            name = (PipeSettingGrid.Columns[cellIndex].GetCellContent(PipeSettingGrid.Items[rowIndex]) as TextBlock).Text;
            return name;
        }
    }
}