using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFETOOLS
{
    public class PumpDataInfo : INotifyPropertyChanged //水泵信息
    {
        private string model;
        private string flow;
        private string lift;
        private string power;
        private string weight;

        /// <summary>
        /// 水泵型号
        /// </summary>
        public string PumpModel { get { return model; } set { model = value; OnPropertyChanged("PumpModel"); } }
        /// <summary>
        /// 水泵流量
        /// </summary>
        public string PumpFlow { get { return flow; } set { flow = value; OnPropertyChanged("PumpFlow"); } }
        /// <summary>
        /// 水泵扬程
        /// </summary>
        public string PumpLift { get { return lift; } set { lift = value; OnPropertyChanged("PumpLift"); } }
        /// <summary>
        /// 水泵功率
        /// </summary>
        public string PumpPower { get { return power; } set { power = value; OnPropertyChanged("PumpPower"); } }
        /// <summary>
        /// 水泵重量
        /// </summary>
        public string PumptWeight{ get { return weight; } set { weight = value; OnPropertyChanged("PumptWeight"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class PumpData
    {
        public int Id
        { get; set; }

        public string Model//型号
        { get; set; }

        public string Flow//流量
        { get; set; }

        public string Lift//扬程
        { get; set; }

        public string NPSH//汽蚀余量
        { get; set; }

        public string RotateSpeed//转速
        { get; set; }

        public string Power//功率
        { get; set; }

        public string Weight//水泵重量
        { get; set; }

        public string InletHeight//水泵进口高度
        { get; set; }

        public string OutletHeight//水泵出口高度
        { get; set; }

        public string InletSize//水泵进口尺寸
        { get; set; }

        public string OutletSize//水泵出口尺寸
        { get; set; }

        public string BaseLength//基础长L
        { get; set; }

        public string BaseWidth//基础宽B
        { get; set; }

        public string BoltHoleLength//螺栓孔间距L3
        { get; set; }

        public string BoltHoleWidth//螺栓孔间距B1
        { get; set; }

        public string BoltHoleSize//基础孔宽
        { get; set; }

        //public int? Age
        //{ get; set; }
    }
    public class TblPumpData
    {
        //查询的字段名称
        public string FieldName
        { get; set; }

        //查询的运算符
        public Operater Op
        { get; set; }

        //查询的值
        public string Value
        { get; set; }

    }
    //运算符的枚举
    public enum Operater
    {
        Qt,
        Lt,
        Eq,
        like
    }
}
