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
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;

namespace FFETOOLS
{
    /// <summary>
    /// UserMajo.xaml 的交互逻辑
    /// </summary>
    public partial class UserMajor : Window
    {      
        ExecuteEventShowWorkset excShowWorkset = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerShowWorkset = null;

        public List<string> SelectWorkSetNameList = new List<string>();
        private ObservableCollection<WorkSetInfo> items = new ObservableCollection<WorkSetInfo>();
        public UserMajor(List<string> workSetNameList)
        {
            InitializeComponent();

            foreach (string info in workSetNameList)
            {
                if (info.Contains("暖通") || info.Contains("电气") || info.Contains("工艺_非标") ||
                    info.Contains("工艺_压缩空气管道") || info.Contains("工艺_罗茨风机管道") || info.Contains("工艺_灭火装置")
                    || info.Contains("结构_钢筋") || info.Contains("工艺_喷水管道")|| info.Contains("HVAC"))
                {
                    items.Add(new WorkSetInfo(info, true));
                }
                else
                {
                    items.Add(new WorkSetInfo(info, false));
                }            
            }
            WorkSetListBox.ItemsSource = items;        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            excShowWorkset = new ExecuteEventShowWorkset();
            eventHandlerShowWorkset = Autodesk.Revit.UI.ExternalEvent.Create(excShowWorkset);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerShowWorkset.Raise();
            SelectWorkSetNameList = SelectWorkSetName(items);
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
        private List<string> SelectWorkSetName(ObservableCollection<WorkSetInfo> items)
        {
            List<string> selectWorkSetName = new List<string>();
            foreach (var item in items)
            {
                if (item.IsSelected == true)
                {
                    selectWorkSetName.Add(item.WorkSetName);
                }
            }
            return selectWorkSetName;
        }
    }
}
