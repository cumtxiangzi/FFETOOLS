using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TextBox = System.Windows.Controls.TextBox;
using Binding = System.Windows.Data.Binding;

namespace FFETOOLS
{
    public class PreviewControlModel
    {
        public Document doc { get; set; }
        public DesignNoteForm Window { get; set; }
        public PreviewControl PreviewControl { get; set; }

        private List<View> views;
        public List<View> Views
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && x.Name.Contains("设计说明")).ToList();
            }
            set => views = value;
        }
        public PreviewControlModel(DesignNoteForm frm, Document doc)
        {
            Window = frm;
            this.doc = doc;
            PreviewControl = new PreviewControl(doc, Views.First().Id);
            PreviewControl.Loaded += PreviewControlLoad;
            Window.PreviewGrid.Children.Add(PreviewControl);
            Window.WorkShopNameListBox.ItemsSource = Views.OrderBy(x => x.Name);
            Window.WorkShopNameListBox.SelectionChanged += ChangeView;
            Window.Closing += CloseWindow;
        }
        private void ChangeView(object sender, SelectionChangedEventArgs e) //自定义事件
        {
            View view = Window.WorkShopNameListBox.SelectedItem as View;
            PreviewControl.Dispose();
            PreviewControl = new PreviewControl(doc, view.Id);
            Window.PreviewGrid.Children.Clear();
            Window.PreviewGrid.Children.Add(PreviewControl);
            PreviewControl.Loaded += PreviewControlLoad;
            Window.WorkShopText.Text = view.Name;
        }
        private void CloseWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            doc.Close(false);
        }
        private void PreviewControlLoad(object sender, RoutedEventArgs e)
        {
            PreviewControl.UIView.ZoomToFit();
        }
    }
}
