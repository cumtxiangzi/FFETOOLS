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
    public partial class FamilyManagerWindow : Window //族库管理有问题
    {
        //族库路径
        DirectoryInfo dirInfo = new DirectoryInfo(@"D:\工作J盘\族库整理结果-2018");
        //载入族路径
        string familyFilePath;

        public FamilyManagerWindow()
        {
            InitializeComponent();
        }

        //树控件载入事件
        private void FamilyTreeList_Loaded(object sender, RoutedEventArgs e)
        {
            //遍历文件夹
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
            {
                //创建子项
                TreeViewItem item = new TreeViewItem();
                item.Tag = di;
                item.Header = di.Name;
                //占位符
                if (di.GetDirectories().Length > 0 || di.GetFiles("*.rfa").Length > 0) item.Items.Add("*");
                //添加子项
                FamilyTreeList.Items.Add(item);
            }
            //遍历族文件
            CreateFamilyItems(dirInfo, FamilyTreeList);
        }

        //节点展开事件
        private void FamilyTreeList_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;
            item.Items.Clear();
            //遍历文件夹
            DirectoryInfo di = (DirectoryInfo)item.Tag;
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                //创建子项
                TreeViewItem subItem = new TreeViewItem();
                subItem.Tag = subDi;
                subItem.Header = subDi.Name;
                //占位符
                if (subDi.GetDirectories().Length > 0 || subDi.GetFiles("*.rfa").Length > 0) subItem.Items.Add("*");
                //添加子项
                item.Items.Add(subItem);
            }
            //遍历族文件
            CreateFamilyItems(di, item);
        }

        //节点选择事件
        private void FamilyTreeList_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.Source;
            if (item.Tag is FileInfo)
            {
                Btn_Load.IsEnabled = true;
            }
            else
            {
                Btn_Load.IsEnabled = false;
            }
        }

        //载入按钮
        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        {
            if (FamilyTreeList.SelectedItem != null)
            {
                TreeViewItem item = (TreeViewItem)FamilyTreeList.SelectedItem;
                FileInfo familyFi = (FileInfo)item.Tag;
                familyFilePath = familyFi.FullName;
                DialogResult = true;
            }
            Close();
        }

        //取消按钮
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        //创建对应族文件的子项
        private void CreateFamilyItems(DirectoryInfo directoryInfo, Control control)
        {
            //遍历族文件
            foreach (FileInfo fi in directoryInfo.GetFiles("*.rfa"))
            {
                //创建子项
                TreeViewItem item = new TreeViewItem();
                item.Tag = fi;
                item.Header = fi.Name;
                //添加子项
                if (control is TreeView)
                {
                    ((TreeView)control).Items.Add(item);
                    continue;
                }
                if (control is TreeViewItem)
                {
                    ((TreeViewItem)control).Items.Add(item);
                }
            }
        }

        public string FamilyFilePath => familyFilePath;

    }
}
