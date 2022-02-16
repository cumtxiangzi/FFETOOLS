using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using OfficeOpenXml;
using MessageBox = System.Windows.MessageBox;

namespace FFETOOLS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WellPointWindow : Window
    {
        ExecuteEventWellPoint excWellPoint = null;
        ExternalEvent eventHandlerWellPoint = null;
        public List<string> Wellname = new List<string>();
        public List<string> Xpoints = new List<string>();
        public List<string> Ypoints = new List<string>();
        public List<string> Zpoints = new List<string>();
        public List<XYZ> Wellpoints = new List<XYZ>();
        public List<double> wellBottomValue = new List<double>();
        public List<DataTable> Results = new List<DataTable>();
        public WellPointWindow()
        {
            InitializeComponent();
            excWellPoint = new ExecuteEventWellPoint();
            eventHandlerWellPoint = ExternalEvent.Create(excWellPoint);
        }
        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void MainForm_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                Close();
            }
        }
        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            if (PS_List.Items.Count != 0)
            {
                PS_List.Items.Clear();
            }
            string path = SelectPath();

            if (path == "")
            {
                MessageBox.Show("未选择管网数据文件", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                DataTable dt = EPPlusHelper.WorksheetToTable(path);
                List<string> st = new List<string>();
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    PS_List.Items.Add(new WellInfo(dt.Rows[j][0].ToString(), dt.Rows[j][1].ToString(), dt.Rows[j][2].ToString(), dt.Rows[j][3].ToString(), dt.Rows[j][4].ToString()
                    , dt.Rows[j][5].ToString(), dt.Rows[j][7].ToString()));
                }
            }
        }
        private void CreatPipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (PS_List.Items.Count == 0)
            {
                MessageBox.Show("未选择管网数据文件", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                DataTable newTB = new DataTable();
                //获取表头
                for (int i = 0; i < PS_List.Columns.Count; i++)
                {
                    if (PS_List.Columns[i].Visibility == System.Windows.Visibility.Visible)//只导出可见列  
                    {
                        if (PS_List.Columns[i].Header == null)
                        {
                            newTB.Columns.Add(string.Empty);
                            continue;
                        }
                        newTB.Columns.Add(PS_List.Columns[i].Header.ToString());//构建表头  
                    }
                }
                //获取内容
                for (int i = 0; i < PS_List.Items.Count; i++)
                {
                    DataRow row = newTB.NewRow();
                    for (int j = 0; j < PS_List.Columns.Count; j++)
                    {
                        System.Windows.Controls.DataGridCell cell1 = DataGridPlus.GetCell(PS_List, i, j);
                        string str = (cell1.Content as TextBlock).Text.ToString();
                        row[j] = str;
                    }
                    newTB.Rows.Add(row);
                }

                DataTable copyTB = newTB.Copy();
                copyTB.Columns.Remove("分组号");
                DataTable distTable = copyTB.DefaultView.ToTable(true, "排水井编号", "A坐标", "B坐标", "井面标高(m)", "井底标高(m)", "井深(m)"); //去除重复行

                Wellname = RemoveNull(DataGridVaule(distTable, 0));
                Xpoints = RemoveNull(DataGridVaule(distTable, 1));
                Ypoints = RemoveNull(DataGridVaule(distTable, 2));
                Zpoints = RemoveNull(DataGridVaule(distTable, 3));
                List<string> wellBottom = RemoveNull(DataGridVaule(distTable, 5));//井深数据

                for (int i = 0; i < Xpoints.Count; i++)
                {
                    Wellpoints.Add(new XYZ(Convert.ToDouble(Ypoints.ElementAt(i)) * 3.28083989501312, Convert.ToDouble(Xpoints.ElementAt(i)) * 3.28083989501312,
                        Convert.ToDouble(Zpoints.ElementAt(i)) * 3.28083989501312));
                }
                for (int i = 0; i < Xpoints.Count; i++)
                {
                    wellBottomValue.Add((Convert.ToDouble(wellBottom.ElementAt(i))) * 3.28083989501312);
                }

                RemoveEmpty(newTB);//去除空行
                Results = newTB.AsEnumerable().GroupBy(row => row.Field<string>("分组号")).Select(g => g.CopyToDataTable()).ToList();//根据分组号分组         

                eventHandlerWellPoint.Raise();
                Close();
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public static List<string> RemoveName(List<string> list)
        {
            List<string> lt = new List<string>();
            foreach (string s in list)
            {
                if (s.Length > 2)
                {
                    lt.Add(s);
                }

            }
            return lt;
        }
        public static bool InChinese(string StrChineseString)
        {
            int i = 0, m = 0;
            m = StrChineseString.Length;
            if (m < 1)
                return false;
            byte[] ucat = System.Text.Encoding.Default.GetBytes(StrChineseString);
            for (i = 0; i <= ucat.Length - 1; i++)
            {
                m = ucat[i];
                i += 1;
                if (m > 160)
                {
                    return true;
                }
                else
                {
                    if (i < ucat.Length)
                    {
                        m = m * 256 + ucat[i] - 65536;
                        if ((m > -20320) && (m < -10246))
                            return true;
                    }
                }
            }
            return false;
        }
        public static List<string> RemoveNull(List<string> list)
        {
            List<string> lt = new List<string>();
            foreach (string s in list)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    lt.Add(s);
                }
            }
            return lt;
        }
        protected void RemoveEmpty(DataTable dt)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool IsNull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {
                        IsNull = false;
                    }
                }
                if (IsNull)
                {
                    removelist.Add(dt.Rows[i]);
                }
            }
            for (int i = 0; i < removelist.Count; i++)
            {
                dt.Rows.Remove(removelist[i]);
            }
        }
        protected DataTable DataTableRemoveEmptyRow(DataTable dt)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool flg_AllNull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString()) || !string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()) || dt.Rows[i][j].ToString() == "" || dt.Rows[i][j].ToString() == "\"")
                    {
                        flg_AllNull = false;
                        break;
                    }
                }
                if (flg_AllNull)
                {
                    removelist.Add(dt.Rows[i]);
                }
            }
            foreach (var r in removelist)
            {
                dt.Rows.Remove(r);
            }
            return dt;
        }
        public static List<string> DataGridVaule(DataTable data, int j)
        {
            List<string> strings = new List<string>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                strings.Add(data.Rows[i][j].ToString());
            }
            return strings;
        }
        public void ExportDataGridToCSV(DataTable dt, string dltName)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = dltName;
            sfd.Filter = "csv文件 (*.csv)|*.csv|所有文件 (*.*)|*.*";
            sfd.ShowDialog();
            FileStream files = new FileStream(sfd.FileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(files, new System.Text.UnicodeEncoding());

            //Tabel 头
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i].ColumnName);
                sw.Write("\t");
            }
            sw.WriteLine("");
            //Table 数据块
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sw.Write(dt.Rows[i][j].ToString());
                    sw.Write("\t");
                }
                sw.WriteLine("");
            }
            sw.Flush();
            sw.Close();
        }
        public static DataTable ConvertToDataTable(System.Windows.Controls.DataGrid dataGrid)
        {
            DataTable dt = new DataTable();

            //获取表头
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].Visibility == System.Windows.Visibility.Visible)//只导出可见列  
                {
                    if (dataGrid.Columns[i].Header == null)
                    {
                        dt.Columns.Add(string.Empty);
                        continue;
                    }
                    dt.Columns.Add(dataGrid.Columns[i].Header.ToString());//构建表头  
                }
            }
            //获取内容
            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                int columnsIndex = 0;
                DataRow row = dt.NewRow();
                for (int j = 0; j < dataGrid.Columns.Count; j++)
                {
                    if (dataGrid.Columns[j].Visibility == System.Windows.Visibility.Visible)
                    {

                        if (dataGrid.Items[i] != null && (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock) != null)//填充可见列数据  
                        {
                            string str = (dataGrid.Columns[j].GetCellContent(dataGrid.Items[i]) as TextBlock).Text.ToString();
                            row[columnsIndex] = str;
                        }
                        else
                        {
                            row[columnsIndex] = "";
                        }
                        columnsIndex++;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        private string SelectPath()
        {
            string path = string.Empty;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "管网数据 (*.xlsx)|*.xlsx|所有文件 (*.*)|*.*"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                path = openFileDialog.FileName;
            }
            return path;
        }

    }

    /// <summary>
    /// 使用  EPPlus 第三方的组件读取Excel
    /// </summary>
    public class EPPlusHelper
    {
        private static string GetString(object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        ///将指定的Excel的文件转换成DataTable （Excel的第一个sheet）
        /// </summary>
        /// <param name="fullFielPath">文件的绝对路径</param>
        /// <returns></returns>
        public static DataTable WorksheetToTable(string fullFielPath)
        {
            try
            {
                FileInfo existingFile = new FileInfo(fullFielPath);
                ExcelPackage package = new ExcelPackage(existingFile);
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];//选定 指定页

                return WorksheetToTable(worksheet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 将worksheet转成datatable
        /// </summary>
        /// <param name="worksheet">待处理的worksheet</param>
        /// <returns>返回处理后的datatable</returns>
        public static DataTable WorksheetToTable(ExcelWorksheet worksheet)
        {
            //获取worksheet的行数
            int rows = worksheet.Dimension.End.Row;
            //获取worksheet的列数
            int cols = worksheet.Dimension.End.Column;

            DataTable dt = new DataTable(worksheet.Name);
            DataRow dr = null;
            for (int i = 1; i <= rows; i++)
            {
                if (i > 1)
                    dr = dt.Rows.Add();

                for (int j = 1; j <= cols; j++)
                {
                    //默认将第一行设置为datatable的标题
                    if (i == 1)
                        dt.Columns.Add(GetString(worksheet.Cells[i, j].Value));
                    //剩下的写入datatable
                    else
                        dr[j - 1] = GetString(worksheet.Cells[i, j].Value);
                }
            }
            return dt;
        }
    }
    public class WellInfo
    {
        public string Code { set; get; }
        public string Name { set; get; }
        public string Aaxis { set; get; }
        public string Baxis { set; get; }
        public string UpLevel { set; get; }
        public string DownLevel { set; get; }
        public string Depth { set; get; }
        public string Diameter { set; get; }
        public string Slope { set; get; }
        public string Type { set; get; }
        public WellInfo(string name, string aaxis, string baxis, string depth, string diamter, string slope, string type, string code)
        {
            Name = name;
            Aaxis = aaxis;
            Baxis = baxis;
            Depth = depth;
            Diameter = diamter;
            Slope = slope;
            Type = type;
            Code = code;
        }
        public WellInfo(string code, string name, string aaxis, string baxis, string uplevel, string downlevel, string depth)
        {
            Code = code;//分组号
            Name = name; //排水井编号
            Aaxis = aaxis; //A坐标
            Baxis = baxis;//B坐标
            UpLevel = uplevel;//井面标高
            DownLevel = downlevel;//井底标高
            Depth = depth;//井深        
        }
    }
    public static class DataGridPlus
    {
        /// <summary>
        /// 获取DataGrid控件单元格
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">单元格所在的行号</param>
        /// <param name="columnIndex">单元格所在的列号</param>
        /// <returns>指定的单元格</returns>
        public static System.Windows.Controls.DataGridCell GetCell(this System.Windows.Controls.DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            DataGridRow rowContainer = dataGrid.GetRow(rowIndex);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                System.Windows.Controls.DataGridCell cell = (System.Windows.Controls.DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                if (cell == null)
                {
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[columnIndex]);
                    cell = (System.Windows.Controls.DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                }
                return cell;
            }
            return null;
        }

        /// <summary>
        /// 获取DataGrid的行
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">DataGrid行号</param>
        /// <returns>指定的行号</returns>
        public static DataGridRow GetRow(this System.Windows.Controls.DataGrid dataGrid, int rowIndex)
        {
            DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (rowContainer == null)
            {
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                dataGrid.UpdateLayout();
                rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            return rowContainer;
        }

        /// <summary>
        /// 获取父可视对象中第一个指定类型的子可视对象
        /// </summary>
        /// <typeparam name="T">可视对象类型</typeparam>
        /// <param name="parent">父可视对象</param>
        /// <returns>第一个指定类型的子可视对象</returns>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
