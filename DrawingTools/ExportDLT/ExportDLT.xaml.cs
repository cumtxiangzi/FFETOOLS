using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class ExportDLTWindow : Window
    {
        public int eltNum { get; set; }
        public ExportDLTWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(Note.Text, out int number))
                {
                    eltNum = int.Parse(Note.Text);

                }

                if (!(eltNum > 0))
                {
                    MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Note.Text = "";
                    Note.Focus();
                }
                else
                {
                    Close();
                }
            }
            catch (Exception)
            {
                
            }        
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {      
            Note.Text = "";
            Note.Focus();
            CH_Button.IsChecked = true;
            MainWorkShop.IsChecked = false;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)//Esc键      
                {
                    Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("请输入大于0的正整数！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
