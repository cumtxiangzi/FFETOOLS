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
    /// HydrantConnectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HydrantConnectForm : Window
    {
        ExecuteEventHydrantConnect excHydrantConnect = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerHydrantConnect = null;
        public HydrantConnectForm()
        {
            InitializeComponent();

            excHydrantConnect = new ExecuteEventHydrantConnect();
            eventHandlerHydrantConnect = Autodesk.Revit.UI.ExternalEvent.Create(excHydrantConnect);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerHydrantConnect.Raise();
            Close();
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            LeftButton.IsChecked = true;
            DirectConnect.IsChecked = true;
        }
    }
}
