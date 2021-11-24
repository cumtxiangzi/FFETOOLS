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
using OutdoorPipe.Properties;

namespace FFETOOLS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PipeAvoidForm : Window
    {
        public int Height { get; set; }
        public double Angle { get; set; }

        ExecuteEventPipeAvoid excPipeAvoid = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerPipeAvoid = null;
        public PipeAvoidForm()
        {
            InitializeComponent();
            excPipeAvoid = new ExecuteEventPipeAvoid();
            eventHandlerPipeAvoid = Autodesk.Revit.UI.ExternalEvent.Create(excPipeAvoid);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HeightValue.Text = Settings.Default.PipeAvoidHeight;
            HeightValue.Focus();
            SingleSelect.IsChecked = true;
            Angle90.IsChecked = true;
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
            if (Angle30.IsChecked == true)
            {
                Angle = 0.52359877559829882;
            }
            if (Angle45.IsChecked == true)
            {
                Angle = 0.78539816339744828;
            }
            if (Angle60.IsChecked == true)
            {
                Angle = 1.0471975511965976;
            }
            if (Angle90.IsChecked == true)
            {
                Angle = 1.5707963267948966;
            }

            if (isInt())
            {
                eventHandlerPipeAvoid.Raise();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.PipeAvoidHeight= HeightValue.Text;
            Settings.Default.Save();
        }
    }
}
