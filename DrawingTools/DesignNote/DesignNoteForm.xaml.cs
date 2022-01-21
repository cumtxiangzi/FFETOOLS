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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DesignNoteForm : Window
    {
        ExecuteEventDesignNote excDesignNote = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerDesignNote = null;
        private ObservableCollection<WorkShopNameInfo> items = new ObservableCollection<WorkShopNameInfo>();
        public List<string> SelectWorkShopNameList = new List<string>();
        public DesignNoteForm(List<string> workShopNameList)
        {
            InitializeComponent();
            foreach (string info in workShopNameList)
            {
                items.Add(new WorkShopNameInfo(info, false));
            }
            WorkShopNameListBox.ItemsSource = items;         
        }
        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Cement_Button.IsChecked = true;
            CH_Button.IsChecked = true;
            excDesignNote = new ExecuteEventDesignNote();
            eventHandlerDesignNote = Autodesk.Revit.UI.ExternalEvent.Create(excDesignNote);
        }
        private void this_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            eventHandlerDesignNote.Dispose();
        }
        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerDesignNote.Raise();
            SelectWorkShopNameList = SelectWorkShopName(items);
            Close();
        }
        private List<string> SelectWorkShopName(ObservableCollection<WorkShopNameInfo> items)
        {
            List<string> selectWorkShopName = new List<string>();
            foreach (var item in items)
            {
                if (item.IsSelected == true)
                {
                    selectWorkShopName.Add(item.WorkShopName);
                }
            }
            return selectWorkShopName;
        }
    }
}
