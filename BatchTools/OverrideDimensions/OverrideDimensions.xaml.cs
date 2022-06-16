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
    public partial class OverrideDimensionsForm : Window
    {
        ExecuteEventOverrideDimensions excOverrideDimensions = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerOverrideDimensions = null;
        public OverrideDimensionsForm()
        {
            InitializeComponent();
            excOverrideDimensions = new ExecuteEventOverrideDimensions();
            eventHandlerOverrideDimensions = Autodesk.Revit.UI.ExternalEvent.Create(excOverrideDimensions);
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
            LengthValue.Text = "5000";
            LengthValue.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerOverrideDimensions.Raise();
            Close();
        }

    }
}
