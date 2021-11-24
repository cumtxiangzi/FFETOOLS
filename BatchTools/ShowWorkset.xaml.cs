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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserMajor : Window
    {
        public string MajorName { get; set; }
        string[] MajorNameList = new string[6] { "工艺专业","给排水专业", "暖通专业", "电气专业", "结构专业" ,"建筑专业"};

        public UserMajor()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            MajorNameCombo.ItemsSource = MajorNameList;
            MajorNameCombo.SelectedIndex = 1;

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            switch (MajorNameCombo.SelectedItem.ToString())
            {
                case "工艺专业":
                    MajorName = "工艺";
                    break;
                case "给排水专业":
                    MajorName = "给排水";
                    break;
                case "暖通专业":
                    MajorName = "暖通";
                    break;
                case "电气专业":
                    MajorName = "电气";
                    break;
                case "结构专业":
                    MajorName = "结构";
                    break;
                case "建筑专业":
                    MajorName = "建筑";
                    break;
            }
            DialogResult = true;
        }

        
    }
}
