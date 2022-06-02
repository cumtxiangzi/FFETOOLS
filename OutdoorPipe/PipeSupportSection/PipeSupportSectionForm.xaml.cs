using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class PipeSupportSectionForm : Window
    {
        List<string> pipeSizeList = new List<string> { "DN15","DN20","DN25","DN32", "DN40", "DN50", "DN65", "DN80" ,
                                                                        "DN100", "DN125", "DN150", "DN200" , "DN250", "DN300", "DN350",
                                                                       "DN400", "DN450"};
        List<string> pipeAbbList = new List<string> { "YJ","XJ","XJ1","XJ2", "XH", "XH1", "XH2", "J" ,
                                                                        "XF", "W", "ZJ", "SJ","F" ,"ZS","ZJ", "YW", "YF" };
        public int clickNum = 1;
        public string name = null;

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
            ThreeFloorGroupBoxLeft.IsEnabled = false;
            ThreeFloorGroupBoxRight.IsEnabled = false;

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
            TwoFloorPipe3_Abb.IsEnabled = false;
            TwoFloorPipe4_Abb.IsEnabled = false;

            OneFloorLeftPipe1_Size.IsEnabled = false;
            OneFloorLeftPipe2_Size.IsEnabled = false;
            OneFloorLeftPipe1_Abb.IsEnabled = false;
            OneFloorLeftPipe2_Abb.IsEnabled = false;
            OneFloorRightPipe1_Size.IsEnabled = false;
            OneFloorRightPipe2_Size.IsEnabled = false;
            OneFloorRightPipe1_Abb.IsEnabled = false;
            OneFloorRightPipe2_Abb.IsEnabled = false;

            TwoFloorLeftPipe1_Size.IsEnabled = false;
            TwoFloorLeftPipe2_Size.IsEnabled = false;
            TwoFloorLeftPipe1_Abb.IsEnabled = false;
            TwoFloorLeftPipe2_Abb.IsEnabled = false;
            TwoFloorRightPipe1_Size.IsEnabled = false;
            TwoFloorRightPipe2_Size.IsEnabled = false;
            TwoFloorRightPipe1_Abb.IsEnabled = false;
            TwoFloorRightPipe2_Abb.IsEnabled = false;

            ThreeFloorLeftPipe1_Size.IsEnabled = false;
            ThreeFloorLeftPipe2_Size.IsEnabled = false;
            ThreeFloorLeftPipe1_Abb.IsEnabled = false;
            ThreeFloorLeftPipe2_Abb.IsEnabled = false;
            ThreeFloorRightPipe1_Size.IsEnabled = false;
            ThreeFloorRightPipe2_Size.IsEnabled = false;
            ThreeFloorRightPipe1_Abb.IsEnabled = false;
            ThreeFloorRightPipe2_Abb.IsEnabled = false;

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

            OneFloorLeftPipe1_Size.ItemsSource = pipeSizeList;
            OneFloorLeftPipe1_Abb.ItemsSource = pipeAbbList;
            OneFloorLeftPipe2_Size.ItemsSource = pipeSizeList;
            OneFloorLeftPipe2_Abb.ItemsSource = pipeAbbList;
            OneFloorLeftPipe1_Size.SelectedIndex = 13;
            OneFloorLeftPipe1_Abb.SelectedIndex = 1;
            OneFloorLeftPipe2_Size.SelectedIndex = 13;
            OneFloorLeftPipe2_Abb.SelectedIndex = 1;

            TwoFloorLeftPipe1_Size.ItemsSource = pipeSizeList;
            TwoFloorLeftPipe1_Abb.ItemsSource = pipeAbbList;
            TwoFloorLeftPipe2_Size.ItemsSource = pipeSizeList;
            TwoFloorLeftPipe2_Abb.ItemsSource = pipeAbbList;
            TwoFloorLeftPipe1_Size.SelectedIndex = 13;
            TwoFloorLeftPipe1_Abb.SelectedIndex = 1;
            TwoFloorLeftPipe2_Size.SelectedIndex = 13;
            TwoFloorLeftPipe2_Abb.SelectedIndex = 1;

            ThreeFloorLeftPipe1_Size.ItemsSource = pipeSizeList;
            ThreeFloorLeftPipe1_Abb.ItemsSource = pipeAbbList;
            ThreeFloorLeftPipe2_Size.ItemsSource = pipeSizeList;
            ThreeFloorLeftPipe2_Abb.ItemsSource = pipeAbbList;
            ThreeFloorLeftPipe1_Size.SelectedIndex = 13;
            ThreeFloorLeftPipe1_Abb.SelectedIndex = 1;
            ThreeFloorLeftPipe2_Size.SelectedIndex = 13;
            ThreeFloorLeftPipe2_Abb.SelectedIndex = 1;

            OneFloorRightPipe1_Size.ItemsSource = pipeSizeList;
            OneFloorRightPipe1_Abb.ItemsSource = pipeAbbList;
            OneFloorRightPipe2_Size.ItemsSource = pipeSizeList;
            OneFloorRightPipe2_Abb.ItemsSource = pipeAbbList;
            OneFloorRightPipe1_Size.SelectedIndex = 13;
            OneFloorRightPipe1_Abb.SelectedIndex = 1;
            OneFloorRightPipe2_Size.SelectedIndex = 13;
            OneFloorRightPipe2_Abb.SelectedIndex = 1;

            TwoFloorRightPipe1_Size.ItemsSource = pipeSizeList;
            TwoFloorRightPipe1_Abb.ItemsSource = pipeAbbList;
            TwoFloorRightPipe2_Size.ItemsSource = pipeSizeList;
            TwoFloorRightPipe2_Abb.ItemsSource = pipeAbbList;
            TwoFloorRightPipe1_Size.SelectedIndex = 13;
            TwoFloorRightPipe1_Abb.SelectedIndex = 1;
            TwoFloorRightPipe2_Size.SelectedIndex = 13;
            TwoFloorRightPipe2_Abb.SelectedIndex = 1;

            ThreeFloorRightPipe1_Size.ItemsSource = pipeSizeList;
            ThreeFloorRightPipe1_Abb.ItemsSource = pipeAbbList;
            ThreeFloorRightPipe2_Size.ItemsSource = pipeSizeList;
            ThreeFloorRightPipe2_Abb.ItemsSource = pipeAbbList;
            ThreeFloorRightPipe1_Size.SelectedIndex = 13;
            ThreeFloorRightPipe1_Abb.SelectedIndex = 1;
            ThreeFloorRightPipe2_Size.SelectedIndex = 13;
            ThreeFloorRightPipe2_Abb.SelectedIndex = 1;

            OneFloorGroupBox.Visibility = Visibility.Visible;
            TwoFloorGroupBox.Visibility = Visibility.Visible;
            ThreeFloorGroupBox.Visibility = Visibility.Visible;
            FourFloorGroupBox.Visibility = Visibility.Visible;

            OneFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            OneFloorGroupBoxRight.Visibility = Visibility.Collapsed;
            TwoFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            TwoFloorGroupBoxRight.Visibility = Visibility.Collapsed;
            ThreeFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            ThreeFloorGroupBoxRight.Visibility = Visibility.Collapsed;
            SupportCode.Text = "C1详图";

            excCreatPipeSupportSection = new ExecuteEventPipeSupportSection();
            eventHandlerPipeSupportSection = Autodesk.Revit.UI.ExternalEvent.Create(excCreatPipeSupportSection);
        }
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerPipeSupportSection.Raise();

            string result = Regex.Replace(SupportCode.Text, @"[^0-9]+", ""); //只保留数字
            name = SupportCode.Text.Replace(result, "");
            clickNum = Convert.ToInt32(result);
            SupportCode.Text = name.Insert(1, clickNum.ToString());

            clickNum++;
            Hide();
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

                TwoFloorGroupBoxLeft.IsEnabled = false;
                ThreeFloorGroupBoxLeft.IsEnabled = false;
                TwoFloorGroupBoxRight.IsEnabled = false;
                ThreeFloorGroupBoxRight.IsEnabled = false;

                OneFloorPipe2.IsChecked = true;
                TwoFloorPipe2.IsChecked = true;

                if (TypeA_Button.IsChecked == true)
                {
                    if (CableTray.IsChecked == true)
                    {
                        PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架一层带桥架.jpg", UriKind.Relative));
                    }
                    else
                    {
                        PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架一层.jpg", UriKind.Relative));
                    }
                }
                if (TypeB_Button.IsChecked == true)
                {
                    if (CableTray.IsChecked == true)
                    {
                        PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架一层带桥架.jpg", UriKind.Relative));
                    }
                    else
                    {
                        PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架一层.jpg", UriKind.Relative));
                    }
                }
                if (TypeC_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架一层.jpg", UriKind.Relative));
                }
                if (TypeD_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/D型支架一层.jpg", UriKind.Relative));
                }
                if (TypeE_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/E型支架一层.jpg", UriKind.Relative));
                }
                if (TypeF_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/F型支架.jpg", UriKind.Relative));
                }

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

                OneFloorGroupBoxLeft.IsEnabled = true;
                TwoFloorGroupBoxLeft.IsEnabled = true;
                ThreeFloorGroupBoxLeft.IsEnabled = false;

                OneFloorGroupBoxRight.IsEnabled = true;
                TwoFloorGroupBoxRight.IsEnabled = true;
                ThreeFloorGroupBoxRight.IsEnabled = false;

                OneFloorPipe2.IsChecked = true;
                TwoFloorPipe2.IsChecked = true;

                if (TypeA_Button.IsChecked == true)
                {
                    if (CableTray.IsChecked == true)
                    {
                        PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架二层带桥架.jpg", UriKind.Relative));
                    }
                    else
                    {
                        PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架二层.jpg", UriKind.Relative));
                    }
                }
                if (TypeB_Button.IsChecked == true)
                {
                    if (CableTray.IsChecked == true)
                    {
                        PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架二层带桥架.jpg", UriKind.Relative));
                    }
                    else
                    {
                        PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架二层.jpg", UriKind.Relative));
                    }
                }
                if (TypeC_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架二层.jpg", UriKind.Relative));
                }
                if (TypeD_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/D型支架二层.jpg", UriKind.Relative));
                }
                if (TypeE_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/E型支架二层.jpg", UriKind.Relative));
                }

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

                OneFloorGroupBoxLeft.IsEnabled = true;
                TwoFloorGroupBoxLeft.IsEnabled = true;
                ThreeFloorGroupBoxLeft.IsEnabled = true;

                OneFloorGroupBoxRight.IsEnabled = true;
                TwoFloorGroupBoxRight.IsEnabled = true;
                ThreeFloorGroupBoxRight.IsEnabled = true;

                OneFloorPipe2.IsChecked = false;
                TwoFloorPipe2.IsChecked = false;

                if (TypeB_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架三层.jpg", UriKind.Relative));
                }
                if (TypeC_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架三层.jpg", UriKind.Relative));
                }
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

                OneFloorPipe2.IsChecked = false;
                TwoFloorPipe2.IsChecked = false;

                if (TypeC_Button.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架四层.jpg", UriKind.Relative));
                }
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
            TwoFloorPipe1_Size.IsEnabled = true;
            TwoFloorPipe1_Abb.IsEnabled = true;
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
            TwoFloorPipe1_Size.IsEnabled = false;
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
            ThreeFloorPipe1_Size.IsEnabled = true;
            ThreeFloorPipe1_Abb.IsEnabled = true;
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
            FourFloorPipe1_Size.IsEnabled = false;
            FourFloorPipe1_Abb.IsEnabled = false;
        }

        private void FourFloorPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            FourFloorPipe2_Size.IsEnabled = false;
            FourFloorPipe2_Abb.IsEnabled = false;
        }      

        private void TypeA_Button_Checked(object sender, RoutedEventArgs e)
        {
            SupportCode.Text = "A1详图";
            ThreeFloor.Visibility = Visibility.Collapsed;
            TwoFloor.IsChecked = true;
            OneFloor.Visibility = Visibility.Visible;
            TwoFloor.Visibility = Visibility.Visible;

            ThreeFloorGroupBoxLeft.IsEnabled = false;
            ThreeFloorGroupBoxRight.IsEnabled = false;
            TwoFloorGroupBoxLeft.IsEnabled = true;
            TwoFloorGroupBoxRight.IsEnabled = true;


            if (OneFloor.IsChecked == true)
            {
                if (CableTray.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架一层带桥架.jpg", UriKind.Relative));
                }
                else
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架一层.jpg", UriKind.Relative));
                }
            }
            if (TwoFloor.IsChecked == true)
            {
                if (CableTray.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架二层带桥架.jpg", UriKind.Relative));
                }
                else
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架二层.jpg", UriKind.Relative));
                }
            }
        }
        private void TypeA_Button_Unchecked(object sender, RoutedEventArgs e)
        {
            ThreeFloor.Visibility = Visibility.Visible;
        }

        private void TypeB_Button_Checked(object sender, RoutedEventArgs e)
        {
            SupportCode.Text = "B1详图";
            TwoFloor.IsChecked = true;
            OneFloor.Visibility = Visibility.Visible;
            TwoFloor.Visibility = Visibility.Visible;
            ThreeFloor.Visibility = Visibility.Visible;

            TwoFloorGroupBoxLeft.IsEnabled = true;
            TwoFloorGroupBoxRight.IsEnabled = true;


            if (OneFloor.IsChecked == true)
            {
                if (CableTray.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架一层带桥架.jpg", UriKind.Relative));
                }
                else
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架一层.jpg", UriKind.Relative));
                }
            }
            if (TwoFloor.IsChecked == true)
            {
                if (CableTray.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架二层带桥架.jpg", UriKind.Relative));
                }
                else
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架二层.jpg", UriKind.Relative));
                }
            }
            if (ThreeFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架三层带桥架.jpg", UriKind.Relative));
            }
        }
        private void TypeC_Button_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorGroupBox.Visibility = Visibility.Visible;
            TwoFloorGroupBox.Visibility = Visibility.Visible;
            ThreeFloorGroupBox.Visibility = Visibility.Visible;
            FourFloorGroupBox.Visibility = Visibility.Visible;

            OneFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            OneFloorGroupBoxRight.Visibility = Visibility.Collapsed;
            TwoFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            TwoFloorGroupBoxRight.Visibility = Visibility.Collapsed;
            ThreeFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            ThreeFloorGroupBoxRight.Visibility = Visibility.Collapsed;

            TwoFloor.Visibility = Visibility.Visible;
            ThreeFloor.Visibility = Visibility.Visible;
            FourFloor.Visibility = Visibility.Visible;
            SupportCode.Text = "C1详图";

            TwoFloor.IsChecked = true;
            OneFloorGroupBox.IsEnabled = true;
            TwoFloorGroupBox.IsEnabled = true;
            ThreeFloorGroupBox.IsEnabled = false;
            FourFloorGroupBox.IsEnabled = false;

            if (OneFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架一层.jpg", UriKind.Relative));
            }
            if (TwoFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架二层.jpg", UriKind.Relative));
            }
            if (ThreeFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架三层.jpg", UriKind.Relative));
            }
            if (FourFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/C型支架四层.jpg", UriKind.Relative));
            }
        }

        private void TypeC_Button_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorGroupBox.Visibility = Visibility.Collapsed;
            TwoFloorGroupBox.Visibility = Visibility.Collapsed;
            ThreeFloorGroupBox.Visibility = Visibility.Collapsed;
            FourFloorGroupBox.Visibility = Visibility.Collapsed;

            OneFloorGroupBoxLeft.Visibility = Visibility.Visible;
            OneFloorGroupBoxRight.Visibility = Visibility.Visible;
            TwoFloorGroupBoxLeft.Visibility = Visibility.Visible;
            TwoFloorGroupBoxRight.Visibility = Visibility.Visible;
            ThreeFloorGroupBoxLeft.Visibility = Visibility.Visible;
            ThreeFloorGroupBoxRight.Visibility = Visibility.Visible;

            FourFloor.Visibility = Visibility.Collapsed;
        }
        private void TypeD_Button_Checked(object sender, RoutedEventArgs e)
        {
            SupportCode.Text = "D1详图";
            TwoFloor.IsChecked = true;
            TwoFloor.Visibility = Visibility.Visible;
            ThreeFloor.Visibility = Visibility.Collapsed;
            FourFloor.Visibility = Visibility.Collapsed;

            ThreeFloorGroupBoxLeft.IsEnabled = false;
            ThreeFloorGroupBoxRight.IsEnabled = false;
            TwoFloorGroupBoxLeft.IsEnabled = true;
            TwoFloorGroupBoxRight.IsEnabled = true;


            if (OneFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/D型支架一层.jpg", UriKind.Relative));
            }
            if (TwoFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/D型支架二层.jpg", UriKind.Relative));
            }

        }

        private void TypeE_Button_Checked(object sender, RoutedEventArgs e)
        {
            SupportCode.Text = "E1详图";
            OneFloor.IsChecked = true;
            ThreeFloor.Visibility = Visibility.Collapsed;
            FourFloor.Visibility = Visibility.Collapsed;
            TwoFloor.Visibility = Visibility.Visible;

            ThreeFloorGroupBoxLeft.IsEnabled = false;
            ThreeFloorGroupBoxRight.IsEnabled = false;
            TwoFloorGroupBoxLeft.IsEnabled = false;
            TwoFloorGroupBoxRight.IsEnabled = false;

            if (OneFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/E型支架一层.jpg", UriKind.Relative));
            }
            if (TwoFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/E型支架二层.jpg", UriKind.Relative));
            }
        }

        private void TypeF_Button_Checked(object sender, RoutedEventArgs e)
        {
            SupportCode.Text = "F1详图";
            OneFloor.IsChecked = true;
            TwoFloor.Visibility = Visibility.Collapsed;
            ThreeFloor.Visibility = Visibility.Collapsed;
            FourFloor.Visibility = Visibility.Collapsed;

            OneFloorGroupBox.Visibility = Visibility.Visible;
            TwoFloorGroupBox.Visibility = Visibility.Visible;
            ThreeFloorGroupBox.Visibility = Visibility.Visible;
            FourFloorGroupBox.Visibility = Visibility.Visible;

            TwoFloorGroupBox.IsEnabled = false;
            ThreeFloorGroupBox.IsEnabled = false;
            FourFloorGroupBox.IsEnabled = false;

            OneFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            OneFloorGroupBoxRight.Visibility = Visibility.Collapsed;
            TwoFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            TwoFloorGroupBoxRight.Visibility = Visibility.Collapsed;
            ThreeFloorGroupBoxLeft.Visibility = Visibility.Collapsed;
            ThreeFloorGroupBoxRight.Visibility = Visibility.Collapsed;

            if (OneFloor.IsChecked == true)
            {
                PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/F型支架.jpg", UriKind.Relative));
            }

        }
        private void TypeF_Button_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorGroupBox.Visibility = Visibility.Collapsed;
            TwoFloorGroupBox.Visibility = Visibility.Collapsed;
            ThreeFloorGroupBox.Visibility = Visibility.Collapsed;
            FourFloorGroupBox.Visibility = Visibility.Collapsed;

            OneFloorGroupBoxLeft.Visibility = Visibility.Visible;
            OneFloorGroupBoxRight.Visibility = Visibility.Visible;
            TwoFloorGroupBoxLeft.Visibility = Visibility.Visible;
            TwoFloorGroupBoxRight.Visibility = Visibility.Visible;
            ThreeFloorGroupBoxLeft.Visibility = Visibility.Visible;
            ThreeFloorGroupBoxRight.Visibility = Visibility.Visible;

        }

        private void TypeG_Button_Checked(object sender, RoutedEventArgs e)//G型支架预留
        {
            SupportCode.Text = "G1详图";
        }
        private void OneFloorLeftPipe1_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorLeftPipe1_Size.IsEnabled = true;
            OneFloorLeftPipe1_Abb.IsEnabled = true;
        }

        private void OneFloorLeftPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorLeftPipe1_Size.IsEnabled = false;
            OneFloorLeftPipe1_Abb.IsEnabled =false;
        }

        private void OneFloorLeftPipe2_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorLeftPipe2_Size.IsEnabled = true;
            OneFloorLeftPipe2_Abb.IsEnabled = true;
        }

        private void OneFloorLeftPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorLeftPipe2_Size.IsEnabled = false;
            OneFloorLeftPipe2_Abb.IsEnabled = false;
        }

        private void OneFloorLeftSelectButton_Click(object sender, RoutedEventArgs e)
        {
            OneFloorLeftPipe1.IsChecked = true;
            OneFloorLeftPipe2.IsChecked = true;
        }
        private void OneFloorRightPipe1_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorRightPipe1_Size.IsEnabled =true;
            OneFloorRightPipe1_Abb.IsEnabled = true;
        }

        private void OneFloorRightPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorRightPipe1_Size.IsEnabled = false;
            OneFloorRightPipe1_Abb.IsEnabled = false;
        }

        private void OneFloorRightPipe2_Checked(object sender, RoutedEventArgs e)
        {
            OneFloorRightPipe2_Size.IsEnabled = true;
            OneFloorRightPipe2_Abb.IsEnabled = true;
        }

        private void OneFloorRightPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            OneFloorRightPipe2_Size.IsEnabled = false;
            OneFloorRightPipe2_Abb.IsEnabled = false;
        }

        private void OneFloorRightSelectButton_Click(object sender, RoutedEventArgs e)
        {
            OneFloorRightPipe1.IsChecked = true;
            OneFloorRightPipe2.IsChecked = true;
        }

        private void TwoFloorLeftPipe1_Checked(object sender, RoutedEventArgs e)
        {
            TwoFloorLeftPipe1_Size.IsEnabled = true;
            TwoFloorLeftPipe1_Abb.IsEnabled = true;
        }

        private void TwoFloorLeftPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFloorLeftPipe1_Size.IsEnabled = false;
            TwoFloorLeftPipe1_Abb.IsEnabled = false;
        }

        private void TwoFloorLeftPipe2_Checked(object sender, RoutedEventArgs e)
        {
            TwoFloorLeftPipe2_Size.IsEnabled = true;
            TwoFloorLeftPipe2_Abb.IsEnabled = true;
        }

        private void TwoFloorLeftPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFloorLeftPipe2_Size.IsEnabled = false;
            TwoFloorLeftPipe2_Abb.IsEnabled = false;
        }
        private void TwoFloorRightSelectButton_Click(object sender, RoutedEventArgs e)
        {
            TwoFloorRightPipe1.IsChecked = true;
            TwoFloorRightPipe2.IsChecked = true;
        }
        private void TwoFloorLeftSelectButton_Click(object sender, RoutedEventArgs e)
        {
            TwoFloorLeftPipe1.IsChecked = true;
            TwoFloorLeftPipe2.IsChecked = true;
        }

        private void TwoFloorRightPipe1_Checked(object sender, RoutedEventArgs e)
        {
            TwoFloorRightPipe1_Size.IsEnabled = true;
            TwoFloorRightPipe1_Abb.IsEnabled = true;
        }

        private void TwoFloorRightPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFloorRightPipe1_Size.IsEnabled = false;
            TwoFloorRightPipe1_Abb.IsEnabled =false;
        }

        private void TwoFloorRightPipe2_Checked(object sender, RoutedEventArgs e)
        {
            TwoFloorRightPipe2_Size.IsEnabled = true;
            TwoFloorRightPipe2_Abb.IsEnabled = true;
        }

        private void TwoFloorRightPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            TwoFloorRightPipe2_Size.IsEnabled = false;
            TwoFloorRightPipe2_Abb.IsEnabled = false;
        }
        private void ThreeFloorLeftPipe1_Checked(object sender, RoutedEventArgs e)
        {
            ThreeFloorLeftPipe1_Size.IsEnabled =true;
            ThreeFloorLeftPipe1_Abb.IsEnabled = true;
        }

        private void ThreeFloorLeftPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            ThreeFloorLeftPipe1_Size.IsEnabled = false;
            ThreeFloorLeftPipe1_Abb.IsEnabled = false;
        }

        private void ThreeFloorLeftPipe2_Checked(object sender, RoutedEventArgs e)
        {
            ThreeFloorLeftPipe2_Size.IsEnabled = true;
            ThreeFloorLeftPipe2_Abb.IsEnabled = true;
        }

        private void ThreeFloorLeftPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            ThreeFloorLeftPipe2_Size.IsEnabled = false;
            ThreeFloorLeftPipe2_Abb.IsEnabled = false;
        }

        private void ThreeFloorLeftSelectButton_Click(object sender, RoutedEventArgs e)
        {
            ThreeFloorLeftPipe1.IsChecked = true;
            ThreeFloorLeftPipe2.IsChecked = true;
        }

        private void ThreeFloorRightPipe1_Checked(object sender, RoutedEventArgs e)
        {
            ThreeFloorRightPipe1_Size.IsEnabled = true;
            ThreeFloorRightPipe1_Abb.IsEnabled = true;
        }

        private void ThreeFloorRightPipe1_Unchecked(object sender, RoutedEventArgs e)
        {
            ThreeFloorRightPipe1_Size.IsEnabled = false;
            ThreeFloorRightPipe1_Abb.IsEnabled = false;
        }

        private void ThreeFloorRightPipe2_Checked(object sender, RoutedEventArgs e)
        {
            ThreeFloorRightPipe2_Size.IsEnabled = true;
            ThreeFloorRightPipe2_Abb.IsEnabled = true;
        }

        private void ThreeFloorRightPipe2_Unchecked(object sender, RoutedEventArgs e)
        {
            ThreeFloorRightPipe2_Size.IsEnabled = false;
            ThreeFloorRightPipe2_Abb.IsEnabled = false;
        }

        private void ThreeFloorRightSelectButton_Click(object sender, RoutedEventArgs e)
        {
            ThreeFloorRightPipe1.IsChecked = true;
            ThreeFloorRightPipe2.IsChecked = true;
        }

        private void CableTray_Checked(object sender, RoutedEventArgs e)
        {
            if (TypeA_Button.IsChecked == true)
            {
                if (OneFloor.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架一层带桥架.jpg", UriKind.Relative));
                }
                if (TwoFloor.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架二层带桥架.jpg", UriKind.Relative));
                }
            }

            if (TypeB_Button.IsChecked == true)
            {
                if (OneFloor.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架一层带桥架.jpg", UriKind.Relative));
                }
                if (TwoFloor.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架二层带桥架.jpg", UriKind.Relative));
                }
            }
        }

        private void CableTray_Unchecked(object sender, RoutedEventArgs e)
        {
            if (TypeA_Button.IsChecked == true)
            {
                if (OneFloor.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架一层.jpg", UriKind.Relative));
                }
                if (TwoFloor.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/A型支架二层.jpg", UriKind.Relative));
                }
            }

            if (TypeB_Button.IsChecked == true)
            {
                if (OneFloor.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架一层.jpg", UriKind.Relative));
                }
                if (TwoFloor.IsChecked == true)
                {
                    PipeSectionImage.Source = new BitmapImage(new Uri(@"/OutdoorPipe;component/Resources/B型支架二层.jpg", UriKind.Relative));
                }
            }
        }
        
    }
}
