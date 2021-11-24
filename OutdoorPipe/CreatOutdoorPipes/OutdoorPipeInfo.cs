using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFETOOLS
{
    public class OutdoorPipeSizeInfo
    {
        public List<string> PipeSizeList { get; set; }
        public string PipeTypeName { get; set; }
        public string PipeSystemName { get; set; }
        public OutdoorPipeSizeInfo(string pipeSystemName, string pipeTypeName, List<string> pipeSizeList)
        {
            PipeSystemName = pipeSystemName;
            PipeTypeName = pipeTypeName;
            PipeSizeList = pipeSizeList;
        }       
    }
    class OutdoorPipeInfo : INotifyPropertyChanged //管道信息
    {
        private string pipeSystem;//管道系统
        private string pipeType;//管道类型
        private string pipeSize;//管径   
        private string pipeHeight;//管道高度  
        public string PipeDistance { get; set; }      
        public string PipeCode { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string PipeSystem
        {
            get { return pipeSystem; }
            set
            {
                pipeSystem = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PipeSystem"));
            }
        }
        public string PipeType
        {
            get { return pipeType; }
            set
            {
                pipeType = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PipeType"));
            }
        }
        public string PipeSize
        {
            get { return pipeSize; }
            set
            {
                pipeSize = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PipeSize"));
            }
        }
        public string PipeHeight
        {
            get { return pipeHeight; }
            set
            {
                pipeHeight = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PipeHeight"));
            }
        }
        public OutdoorPipeInfo(string pipeSystem, string pipeType, string pipeSize)//构造函数
        {
            this.pipeSystem = pipeSystem;
            this.pipeType = pipeType;
            this.pipeSize = pipeSize;
            
        }
        public OutdoorPipeInfo(string pipeSystem, string pipeType, string pipeSize, string pipeCode, string pipeDistance, string pipeHeight)//构造函数
        {
            this.pipeSystem = pipeSystem;
            this.pipeType = pipeType;
            this.pipeSize = pipeSize;           
            this.pipeHeight = pipeHeight;
            PipeDistance = pipeDistance;
            PipeCode = pipeCode;
        }    
    }
}
