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
    ///  PumpSelectForm.xaml 的交互逻辑
    /// </summary>
    public partial class PumpSelectForm : Window
    {
        List<string> ManufactureList = new List<string>() { "山东双轮泵业","上海东方泵业","南方泵业"};
        public PumpSelectForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PumpManufacture.ItemsSource = ManufactureList;
            PumpManufacture.SelectedIndex = 0;
        }
    }

}
