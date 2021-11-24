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
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace FFETOOLS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreatDrawings : Window
    {
        public int number { get; set; }
        public string type { get; set; }
        public string drawingMajorName { get; set; }

        string[] MajorNameList = new string[7] { "工艺专业(PD)", "结构专业(SC)", "建筑专业(B)", "电气专业(E)", "总图专业(GD)", "给排水专业(WD)", "暖通专业(VD)" };
        public CreatDrawings(List<string> DrawingTypeList)
        {
            InitializeComponent();
            DrawingTypeCombo.ItemsSource = DrawingTypeList;
            MajorCombo.ItemsSource = MajorNameList;
            DrawingTypeCombo.SelectedIndex = 0;
            MajorCombo.SelectedIndex = 5;
            CH_Button.IsChecked = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawingNumber.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                type = DrawingTypeCombo.SelectedItem.ToString();

                switch (MajorCombo.SelectedItem.ToString())
                {
                    case "工艺专业(PD)":
                        drawingMajorName = "PD";
                        break;
                    case "结构专业(SC)":
                        drawingMajorName = "SC";
                        break;
                    case "建筑专业(B)":
                        drawingMajorName = "B";
                        break;
                    case "电气专业(E)":
                        drawingMajorName = "E";
                        break;
                    case "总图专业(GD)":
                        drawingMajorName = "GD";
                        break;
                    case "给排水专业(WD)":
                        drawingMajorName = "WD";
                        break;
                    case "暖通专业(VD)":
                        drawingMajorName = "VD";
                        break;
                }

                number = int.Parse(DrawingNumber.Text);
                DialogResult = true;
            }
            catch (Exception)
            {
                MessageBox.Show("请输入大于零的整数", "错误");
                DrawingNumber.Text = "";
                DrawingNumber.Focus();
            }

        }

        private void DrawingTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawingNumber.Focus();
        }

        private void MajorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawingNumber.Focus();
        }
    }
}
