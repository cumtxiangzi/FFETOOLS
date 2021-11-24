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
using System.Windows.Threading;
using System.Collections;
using System.Data;
using System.Windows.Interop;
using System.Diagnostics;

namespace FFETOOLS
{
    /// <summary>
    /// CreatOutdoorPipesForm.xaml 的交互逻辑
    /// </summary>
    public partial class CreatOutdoorPipesForm : Window
    {
        ExecuteEventCreatOutdoorPipes excCreatOutdoorPipes = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatOutdoorPipes = null;
        string str = null;

        ObservableCollection<OutdoorPipeInfo> pipeInfoList = new ObservableCollection<OutdoorPipeInfo>();//DataGrid的数据源
        ObservableCollection<OutdoorPipeInfo> pipeInfoSelectList = new ObservableCollection<OutdoorPipeInfo>();//用于DataGrid的模板加载时提供选项
        string[] ProfessionList = new string[4] { "工艺专业(PD)", "给排水专业(WD)", "暖通专业(VD)", "电气专业(E)" };

        public CreatOutdoorPipesForm(List<OutdoorPipeSizeInfo> pipeSizeInfoList)
        {
            InitializeComponent();

            //三级联动数据项
            foreach (OutdoorPipeSizeInfo info in pipeSizeInfoList)
            {
                foreach (string size in info.PipeSizeList)
                {
                    pipeInfoSelectList.Add(new OutdoorPipeInfo(info.PipeSystemName, info.PipeTypeName, size));
                }
            }
           
            foreach (OutdoorPipeSizeInfo item in pipeSizeInfoList)
            {
                if (item.PipeTypeName.Contains("焊接钢管"))
                {
                    str = item.PipeTypeName;
                    break;
                }              
            }

            pipeInfoList.Add(new OutdoorPipeInfo("给排水_循环给水管道系统", str, "DN200", "管道1", "150", "3500"));
            pipeInfoList.Add(new OutdoorPipeInfo("给排水_循环给水管道系统", str, "DN200", "管道2", "150", "3500"));
            PipeSettingGrid.ItemsSource = pipeInfoList;//绑定数据源
        }
        private void this_Closing(object sender, CancelEventArgs e)
        {
            eventHandlerCreatOutdoorPipes.Dispose();
            eventHandlerCreatOutdoorPipes = null;
            excCreatOutdoorPipes = null;
        }
        /// <summary>
        /// 单击DataGrid下拉ComboBox
        /// </summary>
        private void PipeSettingGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            PipeSettingGrid.BeginEdit();
        }
        private void PipeSettingGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.Column.ToString() == "System.Windows.Controls.DataGridTemplateColumn.CellEditingTemplate")
                ((ComboBox)(e.EditingElement)).IsDropDownOpen = true;
        }
        /// <summary>
        /// PipeSystemLoaded 管道系统下拉列表框初始化，绑定数据源
        /// </summary>
        void PipeSystemLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox curComboBox = sender as ComboBox;
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
        }
        /// <summary>
        /// PipeTypeDropDownClosed 管道类型下拉列表框选择改变刷新
        /// </summary>
        private void PipeTypeDropDownClosed(object sender, EventArgs e)
        {
            DataGridHelper.SetRealTimeCommit(PipeSettingGrid, true);//dataGrid为控件名称
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
            DistanceTxt.Text = "5000";

            ProfessionCmb.ItemsSource = ProfessionList;
            ProfessionCmb.SelectedIndex = 1;

            excCreatOutdoorPipes = new ExecuteEventCreatOutdoorPipes();
            eventHandlerCreatOutdoorPipes = Autodesk.Revit.UI.ExternalEvent.Create(excCreatOutdoorPipes);
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerCreatOutdoorPipes.Raise();
            Hide();
        }
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Caculate_Button_Click(object sender, RoutedEventArgs e)
        {
           

            CalculateWindow calwin = new CalculateWindow();
            calwin.ShowDialog();
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }

        #region 方法
        /// <summary>
        /// 单击TextBox文字全选
        /// </summary>
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
        private void ModifyDistance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = PipeSettingGrid.SelectedItems.Count;
                if (!(count > 0))
                {
                    MessageBox.Show("请先选择一行或多行！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    int txt = int.Parse(DistanceTxt.Text);
                    if (!(DistanceTxt.Text == null))
                    {
                        var s = PipeSettingGrid.SelectedItems;
                        foreach (OutdoorPipeInfo item in s)
                        {
                            item.PipeHeight = DistanceTxt.Text;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                DistanceTxt.Text = "5000";
            }
        }
        private void NumButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int txt = int.Parse(PipeQuantityTxt.Text);
                if (!(int.Parse(PipeQuantityTxt.Text) > 0))
                {
                    MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    PipeQuantityTxt.Text = "2";
                }
                int num = int.Parse(PipeQuantityTxt.Text);
                int count = PipeSettingGrid.Items.Count;
                if (num >= count)
                {
                    for (int i = 0; i < num - count; i++)
                    {
                        pipeInfoList.Add(new OutdoorPipeInfo("给排水_循环给水管道系统", str, "DN200", "管道" + (count + i + 1).ToString(), "150", "3500"));
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
                MessageBox.Show(exp.Message + "请输入大于0的正整数!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                PipeQuantityTxt.Text = "2";
            }
        }
    }
}