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
        public List<View> DesignNoteViewsCN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && x.Name.Contains("设计说明") && x.Name.Contains("中文")).ToList();
            }
            set => views = value;
        }
        public List<View> DesignNoteViewsCNEN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && x.Name.Contains("设计说明") && x.Name.Contains("中英文")).ToList();
            }
            set => views = value;
        }
        public List<View> DesignNoteViewsEN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && x.Name.Contains("设计说明") && x.Name.Contains("英文")&& !x.Name.Contains("中")).ToList();
            }
            set => views = value;
        }
        public List<View> FlowDrawingViewsCN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && x.Name.Contains("中文") && (x.Name.Contains("WL") || x.Name.Contains("图例"))).ToList();
            }
            set => views = value;
        }
        public List<View> FlowDrawingViewsCNEN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && x.Name.Contains("中英文") && (x.Name.Contains("WL") || x.Name.Contains("图例"))).ToList();
            }
            set => views = value;
        }
        public List<View> FlowDrawingViewsEN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && !x.Name.Contains("中") && x.Name.Contains("英文") && (x.Name.Contains("WL") || x.Name.Contains("图例"))).ToList();
            }
            set => views = value;
        }
        public List<View> StructureListViewsCN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && x.Name.Contains("中文") && (x.Name.Contains("构筑物") || x.Name.Contains("选型表"))).ToList();
            }
            set => views = value;
        }
        public List<View> StructureListViewsCNEN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && x.Name.Contains("中英文") && (x.Name.Contains("构筑物") || x.Name.Contains("选型表"))).ToList();
            }
            set => views = value;
        }
        public List<View> StructureListViewsEN
        {
            get
            {
                return views ?? new FilteredElementCollector(doc)
                     .OfCategory(BuiltInCategory.OST_Views)
                     .WhereElementIsNotElementType()
                     .Cast<View>().Where(x => !x.IsTemplate && !x.Name.Contains("中") && x.Name.Contains("英文") && (x.Name.Contains("构筑物") || x.Name.Contains("选型表"))).ToList();
            }
            set => views = value;
        }
        public PreviewControlModel(DesignNoteForm frm, Document doc)
        {
            Window = frm;
            this.doc = doc;
            PreviewControl = new PreviewControl(doc, DesignNoteViewsCN.First().Id);
            PreviewControl.Loaded += PreviewControlLoad;
            Window.PreviewGrid.Children.Add(PreviewControl);
            Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCN.OrderBy(x => x.Name);

            Window.FlowDrawing_Button.Unchecked += FlowDrawingUnCheck;
            Window.DesignNote_Button.Unchecked += DesignNoteUnCheck;
            Window.StructureList_Button.Unchecked += StructureListUnCheck;

            Window.CH_Button.Unchecked += CHUnCheck;
            Window.CHEN_Button.Unchecked += CHENUnCheck;
            Window.EN_Button.Unchecked += ENUnCheck;

            Window.WorkShopNameListBox.SelectionChanged += ChangeView;
            Window.Closing += CloseWindow;
        }
        private void ChangeView(object sender, SelectionChangedEventArgs e) //视图切换自定义事件
        {
            View view = Window.WorkShopNameListBox.SelectedItem as View;
            if (view != null)
            {
                PreviewControl.Dispose();
                PreviewControl = new PreviewControl(doc, view.Id);
                Window.PreviewGrid.Children.Clear();
                Window.PreviewGrid.Children.Add(PreviewControl);
                PreviewControl.Loaded += PreviewControlLoad;
                Window.WorkShopText.Text = view.Name;
            }
        }
        private void DesignNoteUnCheck(object sender, RoutedEventArgs e) //设计说明不选择自定义事件
        {
            if (Window.FlowDrawing_Button.IsChecked == true)
            {
                if(Window.CH_Button.IsChecked==true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.CHEN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.EN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

            }
            if (Window.StructureList_Button.IsChecked == true)
            {
                if (Window.CH_Button.IsChecked==true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.CHEN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.EN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }             
            }
        }
        private void FlowDrawingUnCheck(object sender, RoutedEventArgs e) //流程图不选择自定义事件
        {
            if (Window.DesignNote_Button.IsChecked == true)
            {

                if (Window.CH_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.CHEN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.EN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }
              
            }
            if (Window.StructureList_Button.IsChecked == true)
            {

                if (Window.CH_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.CHEN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.EN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }
               
            }
        }
        private void StructureListUnCheck(object sender, RoutedEventArgs e) //构筑物一览表不选择自定义事件
        {
            if (Window.DesignNote_Button.IsChecked == true)
            {

                if (Window.CH_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.CHEN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.EN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }
               
            }
            else if (Window.FlowDrawing_Button.IsChecked == true)
            {

                if (Window.CH_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.CHEN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.EN_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }
              
            }
        }
        private void CHUnCheck(object sender, RoutedEventArgs e) //中文不选择自定义事件
        {
            if (Window.CHEN_Button.IsChecked==true)
            {
                if (Window.DesignNote_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.FlowDrawing_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.StructureList_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

            }
            else if (Window.EN_Button.IsChecked == true)
            {
                if (Window.DesignNote_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.FlowDrawing_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.StructureList_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }
            }
        }
        private void CHENUnCheck(object sender, RoutedEventArgs e) //中英文不选择自定义事件
        {
            if (Window.CH_Button.IsChecked == true)
            {
                if (Window.DesignNote_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.FlowDrawing_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.StructureList_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

            }
            else if (Window.EN_Button.IsChecked == true)
            {
                if (Window.DesignNote_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.FlowDrawing_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.StructureList_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }
            }
        }
        private void ENUnCheck(object sender, RoutedEventArgs e) //英文不选择自定义事件
        {
            if (Window.CHEN_Button.IsChecked == true)
            {
                if (Window.DesignNote_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.FlowDrawing_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.StructureList_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsCNEN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

            }
            else if (Window.CH_Button.IsChecked == true)
            {
                if (Window.DesignNote_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = DesignNoteViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.FlowDrawing_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = FlowDrawingViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }

                if (Window.StructureList_Button.IsChecked == true)
                {
                    Window.WorkShopNameListBox.ItemsSource = StructureListViewsCN.OrderBy(x => x.Name);
                    Window.WorkShopNameListBox.SelectedIndex = 0;
                }
            }
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
