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
    /// ViewDuplicateForm.xaml 的交互逻辑
    /// </summary>
    public partial class ViewDuplicateForm : Window
    {
        public List<string> SelectDrawingNameList = new List<string>();       
        ExecuteEventViewDuplicat excViewDuplicat = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerViewDuplicat = null;

        private ObservableCollection<ArcDrawingNameInfo> items = new ObservableCollection<ArcDrawingNameInfo>();
        public ViewDuplicateForm(List<string> drawingNameList)
        {
            InitializeComponent();
            foreach (string info in drawingNameList)
            {
                items.Add(new ArcDrawingNameInfo(info, false));
            }
            DrawingNameListBox.ItemsSource = items;
            DetailCheckBox.IsChecked=true;              
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            excViewDuplicat = new ExecuteEventViewDuplicat();
            eventHandlerViewDuplicat = Autodesk.Revit.UI.ExternalEvent.Create(excViewDuplicat);
        }
        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerViewDuplicat.Raise();
            SelectDrawingNameList = SelectDrawingName(items);
            Close();
        }

        private void ClickButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in items)
            {
                item.IsSelected = true;
            }
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in items)
            {
                item.IsSelected = false;
            }
        }
        private List<string> SelectDrawingName(ObservableCollection<ArcDrawingNameInfo> items)
        {
            List<string> selectDrawingName = new List<string>();
            foreach (var item in items)
            {
                if (item.IsSelected == true)
                {
                    selectDrawingName.Add(item.DrawingName);
                }
            }
            return selectDrawingName;
        }
    }
}
