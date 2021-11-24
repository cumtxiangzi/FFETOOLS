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
    ///CreatPipeTagForm.xaml 的交互逻辑
    /// </summary>
    public partial class CreatPipeTagForm : Window
    {
        public int clicked = 0;

        ExecuteEventCreatPipeTag excCreatPipe = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatPipe = null;
        public CreatPipeTagForm()
        {
            InitializeComponent();
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                //Close();
            }
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            //e.Handled = true;

            excCreatPipe = new ExecuteEventCreatPipeTag();
            eventHandlerCreatPipe = Autodesk.Revit.UI.ExternalEvent.Create(excCreatPipe);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            clicked = 1;
            eventHandlerCreatPipe.Raise();

        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            clicked = 2;
            eventHandlerCreatPipe.Raise();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            clicked = 3;
            eventHandlerCreatPipe.Raise();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            clicked = 4;
            eventHandlerCreatPipe.Raise();
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            clicked = 5;
            eventHandlerCreatPipe.Raise();
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            clicked = 6;
            eventHandlerCreatPipe.Raise();
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            clicked = 7;
            eventHandlerCreatPipe.Raise();
        }
        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            clicked = 8;
            eventHandlerCreatPipe.Raise();
        }
        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            clicked = 9;
            eventHandlerCreatPipe.Raise();
        }
        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            clicked = 10;
            eventHandlerCreatPipe.Raise();
        }
        private void this_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
