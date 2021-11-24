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

namespace FFETOOLS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RaiseTeeBranchForm : Window
    {
        public int Height { get; set; }

        ExecuteEventRaiseTeeBranch excRaiseTeeBranch = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerRaiseTeeBranch = null;
        public RaiseTeeBranchForm()
        {
            InitializeComponent();
            
            excRaiseTeeBranch = new ExecuteEventRaiseTeeBranch();
            eventHandlerRaiseTeeBranch = Autodesk.Revit.UI.ExternalEvent.Create(excRaiseTeeBranch);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HeightValue.Text = "800";
            HeightValue.Focus();
            SingleSelect.IsChecked = true;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (isInt())
            {
                eventHandlerRaiseTeeBranch.Raise();
                Close();
            }         
        }
        public bool isInt()
        {
            if (int.TryParse(HeightValue.Text, out int number))
            {
                Height = int.Parse(HeightValue.Text);
                return true;
            }
            else
            {
                MessageBox.Show("请输入整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                HeightValue.Text = "";
                HeightValue.Focus();
                return false;
            }
        }
    }
}
