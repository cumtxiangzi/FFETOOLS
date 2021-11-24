using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFETOOLS
{  
    class WorkSetInfo : INotifyPropertyChanged //工作集信息
    {
        private string workSetName;//工作集名称
        private bool isSelected;

        public event PropertyChangedEventHandler PropertyChanged;
        public string WorkSetName
        {
            get { return workSetName; }
            set
            {
                workSetName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("WorkSetName"));
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

        public WorkSetInfo(string workSetName, bool isSelected)//构造函数
        {
            this.workSetName = workSetName;
            this.isSelected = isSelected;
        }
        public WorkSetInfo(string workSetName)//构造函数
        {
            this.workSetName = workSetName;
        }

    }
}
