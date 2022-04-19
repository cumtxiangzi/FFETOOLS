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
    public partial class NotePipesForm : Window
    {
        public List<string> SelectDrawingNameList = new List<string>();
        ExecuteEventNotePipes excNotePipes = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerNotePipes = null;

        private ObservableCollection<WDrawingNameInfo> items = new ObservableCollection<WDrawingNameInfo>();
        public NotePipesForm(List<string> drawingNameList)
        {
            InitializeComponent();

            foreach (string info in drawingNameList)
            {
                items.Add(new WDrawingNameInfo(info, false));
            }

            DrawingNameListBox.ItemsSource = items;
            excNotePipes = new ExecuteEventNotePipes();
            eventHandlerNotePipes = Autodesk.Revit.UI.ExternalEvent.Create(excNotePipes);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LengthValue.Text = "700";
            LengthValue.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerNotePipes.Raise();
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
        private List<string> SelectDrawingName(ObservableCollection<WDrawingNameInfo> items)
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
