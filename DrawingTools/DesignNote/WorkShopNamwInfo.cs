using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFETOOLS
{
    class WorkShopNameInfo : INotifyPropertyChanged //车间信息
    {
        private string workShopName;//车间名
        private bool isSelected;

        public event PropertyChangedEventHandler PropertyChanged;
        public string WorkShopName
        {
            get { return workShopName; }
            set
            {
                workShopName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("WorkShopName"));
            }
        }
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public WorkShopNameInfo(string drawingName, bool isSelected)//构造函数
        {
            this.workShopName = drawingName;
            this.isSelected = isSelected;
        }
        public WorkShopNameInfo(string drawingName)//构造函数
        {
            this.workShopName = drawingName;
        }

    }
}

