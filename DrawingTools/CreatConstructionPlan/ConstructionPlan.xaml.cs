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

namespace FFETOOLS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConstructionPlanForm : Window
    {
        public List<string> SelectPlanNameList = new List<string>();
        public List<string> PlanNameList= new List<string>();
        
        int PlanListCount = 0;
        int SectionListCount = 0;
        int SystemListCount = 0;
        int DetailListCount = 0;
        int DraftingListCount = 0;
        int ScheduleListCount = 0;

        ExecuteEventConstructionPlan excConstructionPlan = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerConstructionPlan = null;
     
        private ObservableCollection<ConstructionPlanInfo> plans = new ObservableCollection<ConstructionPlanInfo>();   
        private ObservableCollection<ConstructionDrawingInfo> drawings = new ObservableCollection<ConstructionDrawingInfo>();
       
        public ConstructionPlanForm(List<string> planNameList, List<string>drawingNameList,List<string> sectionNameList, List<string>systemViewNameList,
            List<string>detailNameList, List<string> draftingNameList, List<string> scheduleNameList)
        {
            InitializeComponent();

            PlanNameList.Add("平面图");
            PlanNameList.AddRange(planNameList); 
            PlanNameList.Add("剖面图");
            PlanNameList.AddRange(sectionNameList);
            PlanNameList.Add("系统图");
            PlanNameList.AddRange(systemViewNameList);
            PlanNameList.Add("详图视图");
            PlanNameList.AddRange(detailNameList);
            PlanNameList.Add("绘制视图");
            PlanNameList.AddRange(draftingNameList);
            PlanNameList.Add("明细表");
            PlanNameList.AddRange(scheduleNameList);

            PlanListCount=planNameList.Count;
            SectionListCount=sectionNameList.Count;
            SystemListCount=systemViewNameList.Count;
            DetailListCount=detailNameList.Count;
            DraftingListCount=draftingNameList.Count;
            ScheduleListCount=scheduleNameList.Count;
            ConstructionPlanTreeView.ItemsSource= InitData();

           // MessageBox.Show(PlanListCount.ToString());
            

            foreach (string info in drawingNameList)
            {
              drawings.Add(new ConstructionDrawingInfo(info, false));
            }
            ConstructionDrawingListBox.ItemsSource = drawings;

        }
        
        /// <summary>
         /// 数据初始化
         /// </summary>
         /// <returns></returns>
        private ObservableCollection<ConstructionPlanInfo> InitData()
        {
            ObservableCollection<ConstructionPlanInfo> tcList=new ObservableCollection<ConstructionPlanInfo>();
            
            tcList.Add(new ConstructionPlanInfo() { Id = 1, PlanName = PlanNameList.ElementAt(0), ParentID = 0 });
            for (int i = 1; i < PlanListCount+1; i++)
            {         
                    tcList.Add(new ConstructionPlanInfo() { Id = i+1, PlanName =PlanNameList.ElementAt(i), ParentID =1 });              
            }
          
            //最开始的父节点默认为0,如果不递归,TreeView并不会生成【树】
            plans = FindChild(tcList, 0);
            return plans;
        }
       
        /// <summary>
        /// 遍历子类
        /// </summary>
        /// <param name="tcList"></param>
        /// <param name="id">父类编号</param>
        /// <returns></returns>
        private ObservableCollection<ConstructionPlanInfo> FindChild(ObservableCollection<ConstructionPlanInfo> tcList, int id)
        {
            ObservableCollection<ConstructionPlanInfo> childList = new ObservableCollection<ConstructionPlanInfo>();
            foreach (var item in tcList)
            {
                if (item.ParentID == id)
                {
                    childList.Add(item);
                    //判断是否有子节点
                    int count = tcList.Where(a => a.ParentID == item.Id).Count();
                    if (count > 0)
                    {
                        item.Children = FindChild(tcList, item.Id);
                    }
                }
            }
            return childList;
        }
        private void PlanCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            int id = Convert.ToInt32(checkBox.Tag);
            //寻找被点击的checkbox
            ConstructionPlanInfo trees = FindNode(ConstructionPlanTreeView.Items.Cast<ConstructionPlanInfo>().ToList(), id);
            //如果有子类,则子类状态和被点击的CheckBox状态一致
            if (trees.Children.Count() > 0)
            {
                IsSelected(trees.Children, Convert.ToBoolean(checkBox.IsChecked));
            }
        }

        /// <summary>
        /// 寻找被点击的CheckBox
        /// </summary>
        /// <param name="Nodes"></param>
        /// <param name="id">需要寻找的ID</param>
        /// <returns></returns>
        private ConstructionPlanInfo FindNode(List<ConstructionPlanInfo> Nodes, int id)
        {
            ConstructionPlanInfo tree = new ConstructionPlanInfo();
            foreach (var item in Nodes)
            {
                if (item.Id != id)
                {
                    tree = FindNode(item.Children.ToList(), id);
                }
                else
                {
                    tree = item;
                }
                if (tree.Id == id)
                {
                    return tree;
                }
            }
            return tree;
        }

        /// <summary>
        /// 更改子节点状态
        /// </summary>
        /// <param name="tcList">子节点集合</param>
        /// <param name="flag">true？flag</param>
        private void IsSelected(ObservableCollection<ConstructionPlanInfo> tcList, bool flag)
        {
            foreach (var item in tcList)
            {
                item.IsSelected = flag;
                if (item.Children.Count() > 0)
                {
                    IsSelected(item.Children, flag);
                }
            }
        }


        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            excConstructionPlan = new ExecuteEventConstructionPlan();
            eventHandlerConstructionPlan = Autodesk.Revit.UI.ExternalEvent.Create(excConstructionPlan);
        }

        private void this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            eventHandlerConstructionPlan.Raise();
            SelectPlanNameList = SelecPlanName(plans);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private List<string> SelecPlanName(ObservableCollection<ConstructionPlanInfo> items)
        {
            List<string> selectPlanName = new List<string>();
            foreach (var item in items)
            {
                if (item.IsSelected == true)
                {
                    selectPlanName.Add(item.PlanName);
                }
            }
            return selectPlanName;
        }
       
    }
}
