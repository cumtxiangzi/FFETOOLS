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
    /// PumpStationForm.xaml 的交互逻辑
    /// </summary>
    public partial class PumpStationForm : Window
    {
        ExecuteEventPumpStation excPumpStation = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerPumpStation = null;
        public PumpStationForm()
        {         
            InitializeComponent();
            excPumpStation = new ExecuteEventPumpStation();
            eventHandlerPumpStation = Autodesk.Revit.UI.ExternalEvent.Create(excPumpStation);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerPumpStation.Raise();
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
    }
}
