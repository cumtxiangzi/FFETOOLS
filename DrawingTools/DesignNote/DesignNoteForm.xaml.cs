using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TextBox = System.Windows.Controls.TextBox;
using Binding = System.Windows.Data.Binding;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace FFETOOLS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DesignNoteForm : Window
    {
        ExecuteEventDesignNote excDesignNote = null;
        ExternalEvent eventHandlerDesignNote = null;
        public DesignNoteForm()
        {
            InitializeComponent();
            excDesignNote = new ExecuteEventDesignNote();
            eventHandlerDesignNote = ExternalEvent.Create(excDesignNote);
        }
        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Cement_Button.IsChecked = true;
            CH_Button.IsChecked = true;
            WorkShopText.Visibility = System.Windows.Visibility.Hidden;
        }
        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerDesignNote.Raise();
            Hide();
        }
    }
}
