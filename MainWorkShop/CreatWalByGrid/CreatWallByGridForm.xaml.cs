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
    public partial class CreatWallByGridForm : Window
    {
        ExecuteEventCreatWallByGrid excCreatWallByGrid = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerCreatWallByGrid = null;
        public CreatWallByGridForm()
        {
            InitializeComponent();

            excCreatWallByGrid = new ExecuteEventCreatWallByGrid();
            eventHandlerCreatWallByGrid = Autodesk.Revit.UI.ExternalEvent.Create(excCreatWallByGrid);
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
            TopElevation.Focus();
            TopElevation.Text = "4.000";
            BottomElevation.Text = "0.000";
            CreatSlab.IsChecked = true;
            CreatGround.IsChecked = true;
            CreatRoof.IsChecked = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerCreatWallByGrid.Raise();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
