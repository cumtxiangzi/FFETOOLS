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
    public partial class PipeSupportSectionForm : Window
    {
        List<string> pipeSizeList = new List<string> { "DN15","DN20","DN25","DN32", "DN40", "DN50", "DN65", "DN80" ,
                                                                        "DN100", "DN125", "DN150", "DN200" , "DN250", "DN300", "DN350",
                                                                       "DN400", "DN450"};
        List<string> pipeAbbList = new List<string> { "YJ","XJ","XJ1","XJ2", "XH", "XH1", "XH2", "J" ,
                                                                        "XF", "W", "ZJ", "SJ","F" ,"ZS","ZJ", "YW", "YF" };

        ExecuteEventPipeSupportSection excCreatPipeSupportSection = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerPipeSupportSection = null;
        public PipeSupportSectionForm()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TypeC_Button.IsChecked = true;
            TwoFloor.IsChecked = true;
            ThreeFloorGroupBox.IsEnabled = false;
            FourFloorGroupBox.IsEnabled = false;
            OneFloorPipe1.IsChecked = true;
            OneFloorPipe2.IsChecked = true;
            TwoFloorPipe1.IsChecked = true;
            TwoFloorPipe2.IsChecked = true;
            ThreeFloorPipe1.IsChecked = true;
            FourFloorPipe1.IsChecked = true;
            OneFloorPipe3_Size.IsEnabled = false;
            OneFloorPipe4_Size.IsEnabled = false;
            OneFloorPipe3_Abb.IsEnabled = false;
            OneFloorPipe4_Abb.IsEnabled = false;
            TwoFloorPipe3_Size.IsEnabled = false;
            TwoFloorPipe4_Size.IsEnabled = false;
            TwoFloorPipe3_Abb.IsEnabled=false;
            TwoFloorPipe4_Abb.IsEnabled = false;

            OneFloorPipe1_Size.ItemsSource = pipeSizeList;
            OneFloorPipe1_Abb.ItemsSource = pipeAbbList;
            OneFloorPipe2_Size.ItemsSource = pipeSizeList;
            OneFloorPipe2_Abb.ItemsSource = pipeAbbList;
            OneFloorPipe3_Size.ItemsSource = pipeSizeList;
            OneFloorPipe3_Abb.ItemsSource = pipeAbbList;
            OneFloorPipe4_Size.ItemsSource = pipeSizeList;
            OneFloorPipe4_Abb.ItemsSource = pipeAbbList;
            OneFloorPipe1_Size.SelectedIndex = 13;
            OneFloorPipe1_Abb.SelectedIndex = 1;
            OneFloorPipe2_Size.SelectedIndex = 13;
            OneFloorPipe2_Abb.SelectedIndex = 4;
            OneFloorPipe3_Size.SelectedIndex = 12;
            OneFloorPipe3_Abb.SelectedIndex = 8;
            OneFloorPipe4_Size.SelectedIndex = 8;
            OneFloorPipe4_Abb.SelectedIndex = 7;

            TwoFloorPipe1_Size.ItemsSource = pipeSizeList;
            TwoFloorPipe1_Abb.ItemsSource = pipeAbbList;
            TwoFloorPipe2_Size.ItemsSource = pipeSizeList;
            TwoFloorPipe2_Abb.ItemsSource = pipeAbbList;
            TwoFloorPipe3_Size.ItemsSource = pipeSizeList;
            TwoFloorPipe3_Abb.ItemsSource = pipeAbbList;
            TwoFloorPipe4_Size.ItemsSource = pipeSizeList;
            TwoFloorPipe4_Abb.ItemsSource = pipeAbbList;
            TwoFloorPipe1_Size.SelectedIndex = 13;
            TwoFloorPipe1_Abb.SelectedIndex = 1;
            TwoFloorPipe2_Size.SelectedIndex = 13;
            TwoFloorPipe2_Abb.SelectedIndex = 1;
            TwoFloorPipe3_Size.SelectedIndex = 13;
            TwoFloorPipe3_Abb.SelectedIndex = 1;
            TwoFloorPipe4_Size.SelectedIndex = 13;
            TwoFloorPipe4_Abb.SelectedIndex = 1;

            ThreeFloorPipe1_Size.ItemsSource = pipeSizeList;
            ThreeFloorPipe1_Abb.ItemsSource = pipeAbbList;
            ThreeFloorPipe2_Size.ItemsSource = pipeSizeList;
            ThreeFloorPipe2_Abb.ItemsSource = pipeAbbList;
            ThreeFloorPipe1_Size.SelectedIndex = 13;
            ThreeFloorPipe1_Abb.SelectedIndex = 1;
            ThreeFloorPipe2_Size.SelectedIndex = 13;
            ThreeFloorPipe2_Abb.SelectedIndex = 1;

            FourFloorPipe1_Size.ItemsSource = pipeSizeList;
            FourFloorPipe1_Abb.ItemsSource = pipeAbbList;
            FourFloorPipe2_Size.ItemsSource = pipeSizeList;
            FourFloorPipe2_Abb.ItemsSource = pipeAbbList;
            FourFloorPipe1_Size.SelectedIndex = 13;
            FourFloorPipe1_Abb.SelectedIndex = 1;
            FourFloorPipe2_Size.SelectedIndex = 13;
            FourFloorPipe2_Abb.SelectedIndex = 1;

            excCreatPipeSupportSection = new ExecuteEventPipeSupportSection();
            eventHandlerPipeSupportSection = Autodesk.Revit.UI.ExternalEvent.Create(excCreatPipeSupportSection);
        }
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerPipeSupportSection.Raise();
            Hide();
        }
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }

        private void OneFloor_Click(object sender, RoutedEventArgs e)
        {
            if (OneFloor.IsChecked == true)
            {
                OneFloorGroupBox.IsEnabled = true;
                TwoFloorGroupBox.IsEnabled = false;
                ThreeFloorGroupBox.IsEnabled = false;
                FourFloorGroupBox.IsEnabled = false;
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架一层.jpg", UriKind.Relative));
            }
        }

        private void TwoFloor_Click(object sender, RoutedEventArgs e)
        {
            if (TwoFloor.IsChecked == true)
            {
                OneFloorGroupBox.IsEnabled = true;
                TwoFloorGroupBox.IsEnabled = true;
                ThreeFloorGroupBox.IsEnabled = false;
                FourFloorGroupBox.IsEnabled = false;
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架两层.jpg", UriKind.Relative));
            }
        }

        private void ThreeFloor_Click(object sender, RoutedEventArgs e)
        {
            if (ThreeFloor.IsChecked == true)
            {
                OneFloorGroupBox.IsEnabled = true;
                TwoFloorGroupBox.IsEnabled = true;
                ThreeFloorGroupBox.IsEnabled = true;
                FourFloorGroupBox.IsEnabled = false;
            }
        }

        private void FourFloor_Click(object sender, RoutedEventArgs e)
        {
            if (FourFloor.IsChecked == true)
            {
                OneFloorGroupBox.IsEnabled = true;
                TwoFloorGroupBox.IsEnabled = true;
                ThreeFloorGroupBox.IsEnabled = true;
                FourFloorGroupBox.IsEnabled = true;
            }
        }

        private void OneFloorSelectButton_Click(object sender, RoutedEventArgs e)
        {
            OneFloorPipe1.IsChecked = true;
            OneFloorPipe2.IsChecked = true;
            OneFloorPipe3.IsChecked = true;
            OneFloorPipe4.IsChecked = true;
        }

        private void TwoFloorSelectButton_Click(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe1.IsChecked = true;
            TwoFloorPipe2.IsChecked = true;
            TwoFloorPipe3.IsChecked = true;
            TwoFloorPipe4.IsChecked = true;
        }

        private void ThreeFloorSelectButton_Click(object sender, RoutedEventArgs e)
        {
            ThreeFloorPipe1.IsChecked = true;
            ThreeFloorPipe2.IsChecked = true;
        }

        private void FourFloorSelectButton_Click(object sender, RoutedEventArgs e)
        {
            FourFloorPipe1.IsChecked = true;
            FourFloorPipe2.IsChecked = true;
        }

        private void OneFloorPipe1_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorPipe1_Size.IsEnabled = true;
            OneFloorPipe1_Abb.IsEnabled = true;
        }

        private void OneFloorPipe2_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorPipe2_Size.IsEnabled = true;
            OneFloorPipe2_Abb.IsEnabled = true;
        }

        private void OneFloorPipe3_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorPipe3_Size.IsEnabled = true;
            OneFloorPipe3_Abb.IsEnabled = true;
        }

        private void OneFloorPipe4_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorPipe4_Size.IsEnabled = true;
            OneFloorPipe4_Abb.IsEnabled = true;
        }

        private void OneFloorPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorPipe1_Size.IsEnabled = false;
            OneFloorPipe1_Abb.IsEnabled = false;
        }

        private void OneFloorPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorPipe2_Size.IsEnabled = false;
            OneFloorPipe2_Abb.IsEnabled = false;
        }

        private void OneFloorPipe3_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorPipe3_Size.IsEnabled = false;
            OneFloorPipe3_Abb.IsEnabled = false;
        }

        private void OneFloorPipe4_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorPipe4_Size.IsEnabled = false;
            OneFloorPipe4_Abb.IsEnabled = false;
        }

        private void TwoFloorPipe1_Checked(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe1_Size.IsEnabled=true;
            TwoFloorPipe1_Abb.IsEnabled=true;
        }

        private void TwoFloorPipe2_Checked(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe2_Size.IsEnabled = true;
            TwoFloorPipe2_Abb.IsEnabled = true;
        }

        private void TwoFloorPipe3_Checked(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe3_Size.IsEnabled = true;
            TwoFloorPipe3_Abb.IsEnabled = true;
        }

        private void TwoFloorPipe4_Checked(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe4_Size.IsEnabled = true;
            TwoFloorPipe4_Abb.IsEnabled = true;
        }

        private void TwoFloorPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe1_Size.IsEnabled =false;
            TwoFloorPipe1_Abb.IsEnabled = false;
        }

        private void TwoFloorPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe2_Size.IsEnabled = false;
            TwoFloorPipe2_Abb.IsEnabled = false;
        }

        private void TwoFloorPipe3_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe3_Size.IsEnabled = false;
            TwoFloorPipe3_Abb.IsEnabled = false;
        }

        private void TwoFloorPipe4_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFloorPipe4_Size.IsEnabled = false;
            TwoFloorPipe4_Abb.IsEnabled = false;
        }

        private void ThreeFloorPipe1_Checked(object sender, RoutedEventArgs e)
        {
            ThreeFloorPipe1_Size.IsEnabled=true;
            ThreeFloorPipe1_Abb.IsEnabled=true;
        }

        private void ThreeFloorPipe2_Checked(object sender, RoutedEventArgs e)
        {
            ThreeFloorPipe2_Size.IsEnabled = true;
            ThreeFloorPipe2_Abb.IsEnabled = true;
        }

        private void ThreeFloorPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            ThreeFloorPipe1_Size.IsEnabled = false;
            ThreeFloorPipe1_Abb.IsEnabled = false;
        }

        private void ThreeFloorPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            ThreeFloorPipe1_Size.IsEnabled = false;
            ThreeFloorPipe1_Abb.IsEnabled = false;
        }

        private void FourFloorPipe1_Checked(object sender, RoutedEventArgs e)
        {
            FourFloorPipe1_Size.IsEnabled = true;
            FourFloorPipe1_Abb.IsEnabled = true;
        }

        private void FourFloorPipe2_Checked(object sender, RoutedEventArgs e)
        {
            FourFloorPipe2_Size.IsEnabled = true;
            FourFloorPipe2_Abb.IsEnabled = true;
        }

        private void FourFloorPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            FourFloorPipe1_Size.IsEnabled =false;
            FourFloorPipe1_Abb.IsEnabled = false;
        }

        private void FourFloorPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            FourFloorPipe2_Size.IsEnabled = false;
            FourFloorPipe2_Abb.IsEnabled = false;
        }

       
    }
}
