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
        public PipeSupportSectionForm()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TypeC_Button.IsChecked = true;
            OneFloor.IsChecked = true;
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


        }

        private void OneFloor_Click(object sender, RoutedEventArgs e)
        {
            if (OneFloor.IsChecked == true)
            {
                OneFloorGroupBox.IsEnabled = true;
                TwoFloorGroupBox.IsEnabled = false;
                ThreeFloorGroupBox.IsEnabled = false;
                FourFloorGroupBox.IsEnabled = false;
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

        private void OneFloorPipe1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OneFloorPipe2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OneFloorPipe3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OneFloorPipe4_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
