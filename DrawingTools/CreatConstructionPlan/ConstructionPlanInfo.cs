using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFETOOLS
{ 
    class ConstructionPlanInfo : INotifyPropertyChanged //视图信息
    {       
        private int id;
        private string planName;//图名
        private bool isSelected;
        private int parentID;
        private ObservableCollection<ConstructionPlanInfo> children;
        private string icon;
        private string fontweight;
        private string visualSetting;
        /// <summary>
        /// 节点编号
        /// </summary>
        public int Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        /// <summary>
        /// 名称
        /// </summary>
        public string PlanName { get { return planName; } set { planName = value; OnPropertyChanged("PlanName"); } }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get { return isSelected; } set { isSelected = value; OnPropertyChanged("IsSelected"); } }
        /// <summary>
        ///父节点
        /// </summary>
        public int ParentID { get { return parentID; } set { parentID = value; OnPropertyChanged("ParentID"); } }
        /// <summary>
        /// 子节点
        /// </summary>
        public ObservableCollection<ConstructionPlanInfo> Children
        {
            get
            {
                if (children == null)
                {
                    children = new ObservableCollection<ConstructionPlanInfo>();
                }
                return children;
            }
            set
            {
                if (children == null)
                {
                    children = new ObservableCollection<ConstructionPlanInfo>();
                }
                children = value; OnPropertyChanged("Children");
            }
        }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get { return icon; } set { icon = value; OnPropertyChanged("Icon"); } }
        /// <summary>
        /// 字体粗显
        /// </summary>
        public string Fontweight { get { return fontweight; } set { fontweight = value; OnPropertyChanged("Fontweight"); } }
        /// <summary>
        /// 显隐设置
        /// </summary>
        public string VisualSetting { get { return visualSetting; } set { visualSetting = value; OnPropertyChanged("VisualSetting"); } }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
