using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
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
using System.IO;
using Path = System.IO.Path;

namespace FFETOOLS
{
    /// <summary>
    ///  PumpSelectForm.xaml 的交互逻辑
    /// </summary>
    public partial class PumpSelectForm : Window
    {
        List<string> ManufactureList = new List<string>() { "山东双轮泵业", "上海东方泵业", "南方泵业" };
        List<string> ModelList = new List<string>() { "IS", "S", "DL" };

        List<PumpData> PumpDataSource = new List<PumpData>();
        ObservableCollection<PumpDataInfo> PumpInfoList = new ObservableCollection<PumpDataInfo>();//DataGrid的数据源    

        public PumpSelectForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PumpInfoList.Add(new PumpDataInfo() { PumpModel = "IS-100", PumpFlow = "200", PumpLift = "20", PumpPower = "15", PumptWeight = "200" });

            RoomSettingGrid.ItemsSource = PumpInfoList;

            PumpManufacture.ItemsSource = ManufactureList;
            PumpManufacture.SelectedIndex = 0;

            PumpModel.ItemsSource = ModelList;
            PumpModel.SelectedIndex = 0;

            Flow.Focus();
        }

        private void PumpManufacture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PumpManufacture.SelectedItem.ToString().Contains("双轮"))
            {

            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            GetPumpData("双轮");

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void GetPumpData(string manufacture)
        {
            try
            {
                string dataFile = "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\Data\\GPSPumpData.db3";
                
                MessageBox.Show("sss");
                SQLiteConnection conn = new SQLiteConnection();
                Tuple<bool, DataSet, string> tuple = null;
               
                SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder
                {
                    DataSource = dataFile
                };
                conn.ConnectionString = conStr.ToString();
                conn.Open();

               
                string sql = string.Format("SELECT * FROM {0}", "DataIS");
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    //if (paramArr != null)
                    //{
                    //    cmd.Parameters.AddRange(paramArr);
                    //}
                    try
                    {
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
                        DataSet dataSet = new DataSet();
                        dataAdapter.SelectCommand = cmd;
                        dataAdapter.Fill(dataSet);
                        cmd.Parameters.Clear();
                        dataAdapter.Dispose();
                        tuple = Tuple.Create(true, dataSet, string.Empty);
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                DataSet dataSetResult = tuple.Item2;
                if (dataSetResult != null)
                {
                    DataTable resultDate = dataSetResult.Tables[0];
                    foreach (DataRow dataRow in resultDate.Rows)
                    {
                        PumpInfoList.Add(new PumpDataInfo()
                        {
                            PumpModel = dataRow["SPEC"].ToString(),
                            PumpFlow = dataRow["OutPipeDN"].ToString(),
                            PumpLift = dataRow["Length"].ToString(),
                            PumpPower = dataRow["EnginPower"].ToString(),
                            PumptWeight = dataRow["Weight"].ToString()
                        });
                    }
                }

                //SQLiteParameter[] parameter = new SQLiteParameter[]
                //{
                //    new SQLiteParameter("address", "济南")
                //};
                //string sql = string.Format("SELECT * FROM {0} WHERE address = @address", TextBox_DBTable.Text);
                //Tuple<bool, string, DataSet> tuple = SQLiteHelpers.ExecuteDataSet(sql, parameter);
                //if (tuple.Item1)
                //{
                //    dataGridView1.DataSource = tuple.Item3.Tables[0];
                conn.Close();
                conn.Dispose();

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
