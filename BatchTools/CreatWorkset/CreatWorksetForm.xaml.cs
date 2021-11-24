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
    /// CreatWorksetForm.xaml 的交互逻辑
    /// </summary>
    public partial class CreatWorksetForm : Window
    {
        ExecuteEventCreatWorkset excCreatWorkset = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatWorkset = null;
        public CreatWorksetForm()
        {
            InitializeComponent();

            excCreatWorkset = new ExecuteEventCreatWorkset();
            eventHandlerCreatWorkset = Autodesk.Revit.UI.ExternalEvent.Create(excCreatWorkset);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerCreatWorkset.Raise();
            Close();
        }
        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            SubMain.IsChecked = true;
        }
    }
}
