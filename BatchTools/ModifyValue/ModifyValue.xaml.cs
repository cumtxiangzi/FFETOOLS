using Autodesk.Revit.DB;
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
    public partial class ModifyValueWindow : Window
    {
        Document _document;
        List<Category> categories = new List<Category>();
        ObservableCollection<Parameter> parameters = new ObservableCollection<Parameter>();
        ObservableCollection<ViewObject> viewObjects = new ObservableCollection<ViewObject>();

        public ModifyValueWindow(Document document)
        {
            InitializeComponent();

            _document = document;

            //读取所有类别
            foreach (Category category in _document.Settings.Categories)
            {
                if (category.Id.IntegerValue < 0)
                {
                    FilteredElementCollector cate_Type = new FilteredElementCollector(_document).OfCategory((BuiltInCategory)category.Id.IntegerValue).WhereElementIsElementType();
                    FilteredElementCollector cate_Ins = new FilteredElementCollector(_document).OfCategory((BuiltInCategory)category.Id.IntegerValue).WhereElementIsNotElementType();
                    if (cate_Type.Count() == 0 && cate_Ins.Count() == 0) continue;

                    categories.Add(category);
                }
            }
            categories.Sort((x, y) => x.Name.CompareTo(y.Name));

            Sel_Category.ItemsSource = categories;
            Sel_Parameter.ItemsSource = parameters;
            View_Objects.ItemsSource = viewObjects;
        }

        private void Click_Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        //选择类别后更新数据
        private void Sel_Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sel_Category.SelectedItem != null)
            {
                Sel_ParameterType.IsEnabled = true;
            }
            RefreshObjects();
            RefreshParameter();
        }

        //选择参数类型后更新数据
        private void Sel_ParameterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sel_ParameterType != null)
            {
                Sel_Parameter.IsEnabled = true;
            }
            RefreshObjects();
            RefreshParameter();

        }

        //选择参数后允许设置修改模式
        private void Sel_Parameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sel_Parameter.SelectedItem != null)
            {
                Sel_ModifyType.IsEnabled = true;
                if (Sel_ModifyType.SelectedIndex == 0)
                {
                    Input_Text1.IsEnabled = Input_Text2.IsEnabled = true;
                }
                else
                {
                    Input_Text1.IsEnabled = true;
                    Input_Text2.IsEnabled = false;
                }
            }
            else
            {
                Sel_ModifyType.IsEnabled = Input_Text1.IsEnabled = Input_Text2.IsEnabled = false;
            }
        }

        //选择对象后刷新参数选项
        private void View_Objects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshParameter();
        }

        //设置修改模式后允许输入文本框
        private void Sel_ModifyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sel_ModifyType.IsEnabled)
            {
                if (Sel_ModifyType.SelectedIndex == 0)
                {
                    Input_Text1.IsEnabled = Input_Text2.IsEnabled = true;
                }
                else
                {
                    Input_Text1.IsEnabled = true;
                    Input_Text2.IsEnabled = false;
                }
            }
            //else
            //{
            //    Sel_ModifyType.IsEnabled = Input_Text1.IsEnabled = Input_Text2.IsEnabled = false;
            //}
        }

        //刷新参数选项
        private void RefreshParameter()
        {
            if (Sel_ParameterType.SelectedItem != null)
            {
                parameters.Clear();
                //如果选择了某个对象,使用此对象的参数，否则使用所有对象的共有参数
                if (View_Objects.SelectedItem != null)
                {
                    ViewObject viewObject = View_Objects.SelectedItem as ViewObject;
                    foreach (Parameter parameter in viewObject.Element.Parameters)
                    {
                        if (!parameter.IsReadOnly)
                        {
                            if (parameter.StorageType == StorageType.String)
                            {
                                parameters.Add(parameter);
                            }
                        }
                    }
                }
                else
                {
                    List<int> set1 = new List<int>();
                    foreach (ViewObject viewObject in View_Objects.Items)
                    {
                        List<int> set2 = new List<int>();
                        foreach (Parameter parameter in viewObject.Element.Parameters)
                        {
                            if (!parameter.IsReadOnly)
                            {
                                if (parameter.StorageType == StorageType.String)
                                {
                                    set2.Add(parameter.Id.IntegerValue);
                                }
                            }
                        }
                        if (set1.Count == 0)
                        {
                            set1 = set2.ToList();
                        }
                        else
                        {
                            set1 = set1.Intersect(set2).ToList();
                        }
                    }
                    foreach (int i in set1)
                    {
                        foreach (ViewObject viewObject in View_Objects.Items)
                        {
                            foreach (Parameter parameter in viewObject.Element.Parameters)
                            {
                                if (!parameter.IsReadOnly && parameter.StorageType == StorageType.String && parameter.Id.IntegerValue == i)
                                {
                                    parameters.Add(parameter);
                                    break;
                                }
                            }
                            break;
                        }

                    }
                }
            }
        }

        //刷新对象
        private void RefreshObjects()
        {
            if (Sel_Category.SelectedItem != null && Sel_ParameterType != null)
            {
                viewObjects.Clear();

                FilteredElementCollector elements = new FilteredElementCollector(_document);
                Category selCategory = Sel_Category.SelectedItem as Category;
                elements.OfCategory((BuiltInCategory)selCategory.Id.IntegerValue);
                if (Sel_ParameterType.SelectedIndex == 0)
                {
                    foreach (ElementType elementType in elements.WhereElementIsElementType())
                    {
                        ViewObject viewObject = new ViewObject(elementType);
                        viewObjects.Add(viewObject);
                    }
                }
                if (Sel_ParameterType.SelectedIndex == 1)
                {
                    foreach (Element element in elements.WhereElementIsNotElementType())
                    {
                        ViewObject viewObject = new ViewObject(element);
                        viewObjects.Add(viewObject);
                    }
                }
            }

        }

        #region 公共属性

        public List<Element> GetElements
        {
            get
            {
                List<Element> elements = new List<Element>();
                foreach (ViewObject vo in View_Objects.Items)
                {
                    elements.Add(vo.Element);
                }
                return elements;
            }
        }

        public Definition ParameterDefinition
        {
            get
            {
                if (Sel_Parameter.SelectedItem != null)
                {
                    Parameter parameter = Sel_Parameter.SelectedItem as Parameter;
                    return parameter.Definition;
                }
                return null;
            }
        }

        public int ModifyType
        {
            get
            {
                if (Sel_ModifyType.IsEnabled)
                {
                    return Sel_ModifyType.SelectedIndex;
                }
                return -1;
            }
        }

        public string Text1
        {
            get
            {
                if (Input_Text1.IsEnabled)
                {
                    return Input_Text1.Text;
                }
                return null;
            }
        }

        public string Text2
        {
            get
            {
                if (Input_Text2.IsEnabled)
                {
                    return Input_Text2.Text;
                }
                return null;
            }
        }

        #endregion
    }

    class ViewObject
    {
        public ViewObject(ElementType elementType)
        {
            KeyInformation = elementType.Id + " " + elementType.FamilyName + " : " + elementType.Name;
            Element = elementType;
        }

        public ViewObject(Element element)
        {
            KeyInformation = element.Id + " " + element.Name;
            Element = element;
        }

        public string KeyInformation { get; set; }
        public Element Element { get; set; }
    }
}
