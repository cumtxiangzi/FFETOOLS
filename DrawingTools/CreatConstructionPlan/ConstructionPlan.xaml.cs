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
        public List<string> PlanNameList = new List<string>();
        public List<string> DrawingNameList = new List<string>();

        public List<string> AllDrawings = new List<string>();
        public List<List<string>> AllPlans = new List<List<string>>();

        int PlanListCount = 0;
        int SectionListCount = 0;
        int SystemListCount = 0;
        int DetailListCount = 0;
        int DraftingListCount = 0;
        int ScheduleListCount = 0;
        int DrawingListCount = 0;

        ExecuteEventConstructionPlan excConstructionPlan = null;
        Autodesk.Revit.UI.ExternalEvent eventHandlerConstructionPlan = null;

        private ObservableCollection<ConstructionPlanInfo> tcPlanList = new ObservableCollection<ConstructionPlanInfo>();
        private ObservableCollection<ConstructionDrawingInfo> tcDrawingList = new ObservableCollection<ConstructionDrawingInfo>();

        private ObservableCollection<ConstructionPlanInfo> plans = new ObservableCollection<ConstructionPlanInfo>();
        private ObservableCollection<ConstructionDrawingInfo> drawings = new ObservableCollection<ConstructionDrawingInfo>();

        //private ObservableCollection<ConstructionPlanInfo> tcNewPlanList = new ObservableCollection<ConstructionPlanInfo>();
        //private ObservableCollection<ConstructionDrawingInfo> tcNewDrawingList = new ObservableCollection<ConstructionDrawingInfo>();
        public ConstructionPlanForm(List<string> planNameList, List<string> drawingNameList, List<string> sectionNameList, List<string> systemViewNameList,
            List<string> detailNameList, List<string> draftingNameList, List<string> scheduleNameList)
        {
            InitializeComponent();

            PlanNameList.AddRange(planNameList);
            PlanNameList.AddRange(sectionNameList);
            PlanNameList.AddRange(systemViewNameList);
            PlanNameList.AddRange(detailNameList);
            PlanNameList.AddRange(draftingNameList);
            PlanNameList.AddRange(scheduleNameList);

            PlanListCount = planNameList.Count;
            SectionListCount = sectionNameList.Count;
            SystemListCount = systemViewNameList.Count;
            DetailListCount = detailNameList.Count;
            DraftingListCount = draftingNameList.Count;
            ScheduleListCount = scheduleNameList.Count;
            ConstructionPlanTreeView.ItemsSource = InitData();

            DrawingNameList.AddRange(drawingNameList);
            DrawingListCount = drawingNameList.Count;
            ConstructionDrawingTreeView.ItemsSource = DrawingInitData();

        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<ConstructionPlanInfo> InitData()
        {
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 1, PlanName = "平面图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 2, PlanName = "剖面图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 3, PlanName = "系统图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 4, PlanName = "详图视图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 5, PlanName = "绘制视图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 6, PlanName = "明细表", ParentID = 0, Fontweight = "Bold" });

            for (int i = 7; i < PlanListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 1 });
            }

            for (int i = PlanListCount + 7; i < PlanListCount + SectionListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 2 });
            }

            for (int i = SectionListCount + PlanListCount + 7; i < PlanListCount + SectionListCount + SystemListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 3 });
            }

            for (int i = SectionListCount + SystemListCount + PlanListCount + 7; i < PlanListCount + SectionListCount + SystemListCount + DetailListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 4 });
            }

            for (int i = SectionListCount + SystemListCount + PlanListCount + DetailListCount + 7; i < PlanListCount + SectionListCount + SystemListCount + DetailListCount + DraftingListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 5 });
            }

            for (int i = SectionListCount + SystemListCount + PlanListCount + DetailListCount + DraftingListCount + 7; i < PlanListCount + SectionListCount + SystemListCount + DetailListCount + DraftingListCount + ScheduleListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 6 });
            }
            //最开始的父节点默认为0,如果不递归,TreeView并不会生成【树】           
            plans = FindChild(tcPlanList, 0);
            return plans;
        }
        private ObservableCollection<ConstructionPlanInfo> NewInitData(List<List<string>> selectPlan)
        {
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 1, PlanName = "平面图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 2, PlanName = "剖面图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 3, PlanName = "系统图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 4, PlanName = "详图视图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 5, PlanName = "绘制视图", ParentID = 0, Fontweight = "Bold" });
            tcPlanList.Add(new ConstructionPlanInfo() { Id = 6, PlanName = "明细表", ParentID = 0, Fontweight = "Bold" });

            for (int i = 7; i < PlanListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 1 });
            }

            for (int i = PlanListCount + 7; i < PlanListCount + SectionListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 2 });
            }

            for (int i = SectionListCount + PlanListCount + 7; i < PlanListCount + SectionListCount + SystemListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 3 });
            }

            for (int i = SectionListCount + SystemListCount + PlanListCount + 7; i < PlanListCount + SectionListCount + SystemListCount + DetailListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 4 });
            }

            for (int i = SectionListCount + SystemListCount + PlanListCount + DetailListCount + 7; i < PlanListCount + SectionListCount + SystemListCount + DetailListCount + DraftingListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 5 });
            }

            for (int i = SectionListCount + SystemListCount + PlanListCount + DetailListCount + DraftingListCount + 7; i < PlanListCount + SectionListCount + SystemListCount + DetailListCount + DraftingListCount + ScheduleListCount + 7; i++)
            {
                tcPlanList.Add(new ConstructionPlanInfo() { Id = i, PlanName = PlanNameList.ElementAt(i - 7), ParentID = 6 });
            }

            foreach (var item in selectPlan)
            {
                foreach (var plan in item)
                {
                    for (int i = tcPlanList.Count - 1; i >= 0; i--)
                    {
                        if (tcPlanList[i].PlanName == plan)
                        {
                            tcPlanList.Remove(tcPlanList[i]);
                        }
                    }
                }
            }
            //最开始的父节点默认为0,如果不递归,TreeView并不会生成【树】           
            plans = FindChild(tcPlanList, 0);
            return plans;
        }
        private ObservableCollection<ConstructionDrawingInfo> DrawingInitData()
        {
            for (int i = 1; i < DrawingListCount + 1; i++)
            {
                tcDrawingList.Add(new ConstructionDrawingInfo() { Id = i, DrawingName = DrawingNameList.ElementAt(i - 1), ParentID = 0, Fontweight = "Bold" });
            }

            //最开始的父节点默认为0,如果不递归,TreeView并不会生成【树】
            drawings = FindChild(tcDrawingList, 0);
            return drawings;
        }

        private ObservableCollection<ConstructionDrawingInfo> NewDrawingInitData(List<List<string>> selectPlan, List<int> selectDrawingID)//增加平面后数据
        {
            for (int i = 1; i < DrawingListCount + 1; i++)
            {
                tcDrawingList.Add(new ConstructionDrawingInfo() { Id = i, DrawingName = DrawingNameList.ElementAt(i - 1), ParentID = 0, Fontweight = "Bold" });
            }

            int num1 = DrawingListCount + 1;
            for (int i = 0; i < selectPlan.Count; i++)
            {
                int num2 = selectPlan.ElementAt(i).Count;

                for (int j = num1; j < num2 + num1; j++)
                {
                    tcDrawingList.Add(new ConstructionDrawingInfo() { Id = j, DrawingName = selectPlan.ElementAt(i).ElementAt(j - num1), ParentID = selectDrawingID.ElementAt(i) });
                }
                num1 += num2;
            }

            //最开始的父节点默认为0,如果不递归,TreeView并不会生成【树】
            drawings = FindChild(tcDrawingList, 0);
            return drawings;
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
        private ObservableCollection<ConstructionDrawingInfo> FindChild(ObservableCollection<ConstructionDrawingInfo> tcList, int id)
        {
            ObservableCollection<ConstructionDrawingInfo> childList = new ObservableCollection<ConstructionDrawingInfo>();
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
        private void DrawingCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            int id = Convert.ToInt32(checkBox.Tag);
            //寻找被点击的checkbox
            //ConstructionDrawingInfo trees = FindNode(ConstructionPlanTreeView.Items.Cast<ConstructionDrawingInfo>().ToList(), id);
            //如果有子类,则子类状态和被点击的CheckBox状态一致
            //if (trees.Children.Count() > 0)
            //{
            //    IsSelected(trees.Children, Convert.ToBoolean(checkBox.IsChecked));
            //}

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
        private ConstructionDrawingInfo FindNode(List<ConstructionDrawingInfo> Nodes, int id)
        {
            ConstructionDrawingInfo tree = new ConstructionDrawingInfo();
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
        private void IsSelected(ObservableCollection<ConstructionDrawingInfo> tcList, bool flag)
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

            foreach (var item in tcDrawingList)
            {
                if (item.ParentID == 0)
                {
                    AllDrawings.Add(item.DrawingName);
                    List<string> planUnderDrawing = new List<string>();
                    foreach (var item1 in tcDrawingList)
                    {
                        if (item1.ParentID == item.Id)
                        {                         
                            planUnderDrawing.Add(item1.DrawingName);                         
                        }
                    }
                    AllPlans.Add(planUnderDrawing);
                }
            }
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private List<List<string>> SelectPlan = new List<List<string>>();
        private List<int> SelectDrawingID = new List<int>();
        private void AddButton_Click(object sender, RoutedEventArgs e) //增加视图操作
        {
            List<string> selectPlan = SelectPlanName(tcPlanList);
            List<string> selectPlanNotHeader = new List<string>();
            string selectDrawing = SelectDrawingName(tcDrawingList).FirstOrDefault();

            if (SelectDrawingName(tcDrawingList).Count > 1)
            {
                MessageBox.Show("只能选择一张图纸", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                foreach (ConstructionDrawingInfo item in tcDrawingList)
                {
                    item.IsSelected = false;
                }
            }
            else if (selectPlan.Count == 0)
            {
                MessageBox.Show("请从左侧选择一张给排水视图", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (SelectDrawingName(tcDrawingList).Count == 0)
            {
                MessageBox.Show("请从右侧选择一张给排水图纸", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (!(selectDrawing.Contains("W")))
            {
                MessageBox.Show("请确保右侧选择项为图纸", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                foreach (string item in selectPlan)
                {
                    foreach (ConstructionPlanInfo plan in tcPlanList)
                    {
                        if (plan.PlanName == item)
                        {
                            if (!(IsHeader(plan.PlanName)))
                            {
                                //plan.VisualSetting = "Collapsed";
                                selectPlanNotHeader.Add(plan.PlanName);
                            }
                            else
                            {
                                plan.IsSelected = false;
                            }
                        }
                    }
                }

                int selectDrawingID = 1;
                foreach (ConstructionDrawingInfo item in tcDrawingList)
                {
                    if (item.DrawingName == selectDrawing)
                    {
                        selectDrawingID = item.Id;

                    }
                }

                SelectPlan.Add(selectPlanNotHeader);
                SelectDrawingID.Add(selectDrawingID);

                tcDrawingList.Clear();
                ConstructionDrawingTreeView.ItemsSource = NewDrawingInitData(SelectPlan, SelectDrawingID);

                tcPlanList.Clear();
                ConstructionPlanTreeView.ItemsSource = NewInitData(SelectPlan);
            }
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e) //移除选择图纸操作 未解决暂时不做
        {
            //List<string> selectPlan = SelectPlanName(tcPlanList);
            List<string> selectDrawingPlan = SelectDrawingName(tcDrawingList);
            List<ConstructionDrawingInfo> selectDrawingInfo = new List<ConstructionDrawingInfo>();

            //foreach (string item in selectPlan)
            //{
            //    foreach (ConstructionPlanInfo plan in tcPlanList)
            //    {
            //        if (plan.PlanName == item)
            //        {
            //            //tcPlanList.Remove(plan);
            //            //MessageBox.Show(plan.PlanName);
            //            plan.VisualSetting = "Visible";
            //        }
            //    }
            //}

            foreach (string item in selectDrawingPlan)
            {
                foreach (ConstructionDrawingInfo drawing in tcDrawingList)
                {
                    if (drawing.DrawingName == item && !(item.Contains("WD")))
                    {
                        selectDrawingInfo.Add(drawing);
                    }
                }
            }

            foreach (ConstructionDrawingInfo item in selectDrawingInfo)
            {
                MessageBox.Show("sss");
                tcDrawingList.Remove(item);
            }

            ConstructionDrawingTreeView.ItemsSource = FindChild(tcDrawingList, 0);
        }
        private List<string> SelectPlanName(ObservableCollection<ConstructionPlanInfo> items)
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
        private List<string> SelectDrawingName(ObservableCollection<ConstructionDrawingInfo> items)
        {
            List<string> selectDrawingName = new List<string>();
            foreach (var item in items)
            {
                if (item.IsSelected == true)
                {
                    selectDrawingName.Add(item.DrawingName);
                }
            }
            return selectDrawingName;
        }
        public bool IsHeader(string name)
        {
            bool isHeader = false;
            if (name == "平面图" || name == "剖面图" || name == "系统图" || name == "详图视图" || name == "绘制视图" || name == "明细表")
            {
                isHeader = true;
            }
            return isHeader;
        }

    }
}
