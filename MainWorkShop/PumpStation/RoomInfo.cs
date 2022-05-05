using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFETOOLS
{
    public class RoomSetInfo
    {
        public int RoomCode { get; set; }
        public string RoomName { get; set; }
        public double RoomLength { get; set; }
        public double RoomHeight { get; set; }
        public double RoomBottom { get; set; }
        public double RoomWidth { get; set; }
    }
    class RoomInfo : INotifyPropertyChanged //泵房信息
    {
        private string roomCode;
        private string roomLength;
        private string roomNameList;//房间名称      
        //private string roomHeightList;//房间顶部标高
        private string roomBottomList;//房间底部标高

        /// <summary>
        /// 房间编号
        /// </summary>
        public string RoomCode { get { return roomCode; } set { roomCode = value; OnPropertyChanged("RoomCode"); } }
        /// <summary>
        ///房间长度
        /// </summary>
        public string RoomLength { get { return roomLength; } set { roomLength = value; OnPropertyChanged("RoomLength"); } }
        /// <summary>
        /// 房间名称
        /// </summary>
        public string RoomNameList { get { return roomNameList; } set { roomNameList = value; OnPropertyChanged("RoomNameList"); } }
        /// <summary>
        /// 房间顶部标高
        /// </summary>
        //public string RoomHeightList { get { return roomHeightList; } set { roomHeightList = value; OnPropertyChanged("RoomHeightList"); } }
        /// <summary>
        /// 房间底部标高
        /// </summary>
        public string RoomBottomList { get { return roomBottomList; } set { roomBottomList = value; OnPropertyChanged("RoomBottomList"); } }

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
