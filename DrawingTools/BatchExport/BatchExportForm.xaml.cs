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
    /// BatchExportForm.xaml 的交互逻辑
    /// </summary>
    public partial class BatchExportForm : Window
    {
        public List<string> SelectDrawingNameList = new List<string>();
        ExecuteEventBatchExport excBatchExport = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerBatchExport = null;

        private ObservableCollection<WaterDrawingNameInfo> items = new ObservableCollection<WaterDrawingNameInfo>();
        public BatchExportForm(List<string> drawingNameList)
        {
            InitializeComponent();
            foreach (string info in drawingNameList)
            {
                items.Add(new WaterDrawingNameInfo(info, false));
            }
            DrawingNameListBox.ItemsSource = items;
            DWGbutton.IsChecked = true;
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            excBatchExport = new ExecuteEventBatchExport();
            eventHandlerBatchExport = Autodesk.Revit.UI.ExternalEvent.Create(excBatchExport);
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
            //System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            //sfd.FileName = "";
            //sfd.Filter = "Excel 工作薄（*.xlsx）|*.xlsx";
            //sfd.ShowDialog();

            eventHandlerBatchExport.Raise();
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
        private List<string> SelectDrawingName(ObservableCollection<WaterDrawingNameInfo> items)
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

