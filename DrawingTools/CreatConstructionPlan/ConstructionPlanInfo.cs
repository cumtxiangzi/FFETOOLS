using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFETOOLS
{ 
    class ConstructionPlanInfo : INotifyPropertyChanged //图名信息
    {
        private string drawingName;//图名
        private bool isSelected;

        public event PropertyChangedEventHandler PropertyChanged;
        public string DrawingName
        {
            get { return drawingName; }
            set
            {
                drawingName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("DrawingName"));
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
        
        public ConstructionPlanInfo(string drawingName, bool isSelected)//构造函数
        {
            this.drawingName = drawingName;
            this.isSelected = isSelected;       
        }
        public ConstructionPlanInfo(string drawingName)//构造函数
        {
            this.drawingName = drawingName;
        }

    }
}
