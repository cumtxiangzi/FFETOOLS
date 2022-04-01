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
    /// CreatPipeSystemForm.xaml 的交互逻辑
    /// </summary>
    public partial class CreatPipeSystemForm : Window
    {     
        ExecuteEventCreatPipeSystem excCreatPipeSystem = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatPipeSystem = null;
        public CreatPipeSystemForm()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SouthEastButton.IsChecked = true;
            QuantityTxt.Text = "3";
            NoteLengthTxt.Text = "800";
            NoteLengthTxt.IsEnabled = false;
            NotePipeChkBox.IsChecked = false;

            XJChkBox.IsEnabled = false;
            XHChkBox.IsEnabled = false;

            JChkBox.IsEnabled = false;
            WChkBox.IsEnabled = false;
            RJChkBox.IsEnabled = false;

            XFChkBox.IsEnabled = false;
            QTChkBox.IsEnabled = false;
            ZPChkBox.IsEnabled = false;

            HNChkBox.IsEnabled = false;
            XDChkBox.IsEnabled = false;
            WDChkBox.IsEnabled = false;

            YJChkBox.IsEnabled = false;
            ZSChkBox.IsEnabled = false;
            FChkBox.IsEnabled = false;

            excCreatPipeSystem = new ExecuteEventCreatPipeSystem();
            eventHandlerCreatPipeSystem = Autodesk.Revit.UI.ExternalEvent.Create(excCreatPipeSystem);
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerCreatPipeSystem.Raise();
            Close();
        }
        private void this_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void NotePipeChkBox_Checked(object sender, RoutedEventArgs e)
        {
            NoteLengthTxt.IsEnabled= true;
            QuantityTxt.Text = "1";
        }

        private void NotePipeChkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            NoteLengthTxt.IsEnabled =false;
            QuantityTxt.Text = "3";
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
    }
}
