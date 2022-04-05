using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFETOOLS
{
    class ConstructionDrawingInfo : INotifyPropertyChanged //图纸信息
    {
        private int id;
        private string drawingName;//图名
        private bool isSelected;
        private int parentID;
        private ObservableCollection<ConstructionDrawingInfo> children;
        private string icon;

        /// <summary>
        /// 节点编号
        /// </summary>
        public int Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }
        /// <summary>
        /// 名称
        /// </summary>
        public string DrawingName { get { return drawingName; } set { drawingName = value; OnPropertyChanged("DrawingName"); } }
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
        public ObservableCollection<ConstructionDrawingInfo> Children
        {
            get
            {
                if (children == null)
                {
                    children = new ObservableCollection<ConstructionDrawingInfo>();
                }
                return children;
            }
            set
            {
                if (children == null)
                {
                    children = new ObservableCollection<ConstructionDrawingInfo>();
                }
                children = value; OnPropertyChanged("Children");
            }
        }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get { return icon; } set { icon = value; OnPropertyChanged("Icon"); } }

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
