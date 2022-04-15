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

namespace FFETOOLS
{
    /// <summary>
    /// CreatDrawings.xaml 的交互逻辑
    /// </summary>
    public partial class CreatDrawings : Window
    {
        public int number = 0;
        public string type { get; set; }
        public string drawingMajorName { get; set; }

        string[] MajorNameList = new string[7] { "工艺专业(PD)", "结构专业(SC)", "建筑专业(B)", "电气专业(E)", "总图专业(GD)", "给排水专业(WD)", "暖通专业(VD)" };

        ExecuteEventCreatDrawing excCreatDrawing = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatDrawing = null;
        public CreatDrawings(List<string> DrawingTypeList)
        {
            InitializeComponent();
            excCreatDrawing = new ExecuteEventCreatDrawing();
            eventHandlerCreatDrawing = Autodesk.Revit.UI.ExternalEvent.Create(excCreatDrawing);

            int index = DrawingTypeList.FindIndex(item => item.Contains("共用_图纸_A1"));
            DrawingTypeCombo.SelectedIndex = index;

            DrawingTypeCombo.ItemsSource = DrawingTypeList;
            MajorCombo.ItemsSource = MajorNameList;        
            MajorCombo.SelectedIndex = 5;
            CH_Button.IsChecked = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawingNumber.Focus();
            CreatTitle.IsChecked = true;
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

                if (int.TryParse(DrawingNumber.Text, out int number))
                {
                    this.number = int.Parse(DrawingNumber.Text);
                    
                }
                
                if (!(this.number > 0))
                {
                    MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    DrawingNumber.Text = "";
                    DrawingNumber.Focus();
                }
                else
                {
                    eventHandlerCreatDrawing.Raise();
                    Close();
                }
            }
            catch (Exception)
            {
                
            }
        }
        private bool isNumberic(string message)
        {
            System.Text.RegularExpressions.Regex rex =
            new System.Text.RegularExpressions.Regex(@"^\d+$");         
            if (rex.IsMatch(message))
            {            
                return true;
            }
            else
                return false;
        }

        private void DrawingTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawingNumber.Focus();
        }

        private void MajorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawingNumber.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            //eventHandlerCreatDrawing.Dispose();
            //eventHandlerCreatDrawing = null;
            //excCreatDrawing = null;
        }
    }
}
