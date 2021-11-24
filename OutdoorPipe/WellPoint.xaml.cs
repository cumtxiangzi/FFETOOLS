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

namespace FFETOOLS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WellPointWindow : Window
    {
        public UIDocument _uidoc;
        public ExecuteHander _executeHander;
        public ExternalEvent _externalEvent;
        public List<XYZ> points = new List<XYZ>();
        public int total { set; get; }
        public int totalnum { set; get; }
        public WellPointWindow()
        {

        }
        public WellPointWindow(ExecuteHander executeHander, ExternalEvent externalEvent, UIDocument uidoc)
        {
            InitializeComponent();
            _executeHander = executeHander;
            _externalEvent = externalEvent;
            _uidoc = uidoc;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void OPButton_Click(object sender, RoutedEventArgs e)
        {
            if (_externalEvent != null)
            {
                _executeHander.ExecuteAction = new Action<UIApplication>((app) =>
                  {
                      if (app.ActiveUIDocument == null || app.ActiveUIDocument.Document == null)
                      {
                          return;
                      }
                      UIDocument uidoc = app.ActiveUIDocument;
                      Document doc = uidoc.Document;

                      FilteredElementCollector wellCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_Site);
                      IList<Element> wells = wellCollector.ToElements();

                      View3D view3D;
                      FamilyInstance well;
                      XYZ point = null;
                      List<string> wellNumber = new List<string>();

                      ProjectInfo pro = doc.ProjectInformation;
                      Parameter proNum = pro.LookupParameter("工程代号");
                      string dltName = proNum.AsString() + "W" + "." + "txt";
                      SaveFileDialog sfd = new SaveFileDialog();
                      sfd.FileName = dltName;
                      sfd.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                      sfd.ShowDialog();
                      FileStream files = new FileStream(sfd.FileName, FileMode.Create);
                      StreamWriter sw = new StreamWriter(files);

                      foreach (Element s in wells)
                      {
                          FamilyInstance w = s as FamilyInstance;
                          if (w.Name.Contains("给排水") && w.Name.Contains("排水检查井"))
                          {
                              well = w;
                              string i_type = well.LookupParameter("标记").AsString();
                              wellNumber.Add(i_type);
                          }
                      }
                      wellNumber = wellNumber.OrderBy(s => int.Parse(Regex.Match(s, @"\d+").Value)).ThenBy(x => x.ToUpper()).ToList();
                      total = wellNumber.Count;

                      using (Transaction ts = new Transaction(doc, "导出排水纵断数据"))
                      {
                          ts.Start();
                          FilteredElementCollector collector = new FilteredElementCollector(doc);
                          Func<View3D, bool> isNotTemplate = v3 => !(v3.IsTemplate);
                          view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First<View3D>(isNotTemplate);
                          foreach (string p in wellNumber)
                          {
                              foreach (Element elm in wells)
                              {
                                  FamilyInstance w = elm as FamilyInstance;
                                  if (w.Name.Contains("给排水") && w.Name.Contains("排水检查井"))
                                  {
                                      well = w;
                                      point = (well.Location as LocationPoint).Point;
                                      string s_type = well.Symbol.LookupParameter("类型标记").AsString();
                                      string i_type = well.LookupParameter("标记").AsString();
                                      if (p == i_type)
                                      {
                                          sw.WriteLine("'" + i_type.PadRight(5) + "'" + ", " + (point.X * 0.3048).ToString("0.###") + ", " + (point.Y * 0.3048).ToString("0.###") +
                                           ", " + (point.Z * 0.3048).ToString("0.###") + ",      " + "300" + ", " + "0.003" + ", " + s_type + ", " + "1");
                                      }
                                  }
                              }
                          }
                          ts.Commit();
                      }
                      sw.Flush();
                      sw.Close();
                      files.Close();
                  }
                );
            }
            _externalEvent.Raise();
        }
        private void CreatPipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_externalEvent != null)
            {
                _executeHander.ExecuteAction = new Action<UIApplication>((app) =>
                {
                    if (app.ActiveUIDocument == null || app.ActiveUIDocument.Document == null)
                    {
                        return;
                    }
                    UIDocument uidoc = app.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    FilteredElementCollector pipetype = new FilteredElementCollector(doc);
                    pipetype.OfClass(typeof(PipeType));
                    IList<Element> pipetypes = pipetype.ToElements();
                    PipeType pt = null;
                    foreach (Element pipe in pipetypes)
                    {
                        PipeType ps = pipe as PipeType;
                        if (ps.Name.Contains("给排水") && ps.Name.Contains("HDPE"))
                        {
                            pt = ps;
                            break;
                        }
                    }

                    FilteredElementCollector pipesystem = new FilteredElementCollector(doc);
                    pipesystem.OfClass(typeof(PipingSystemType));
                    IList<Element> pipesystems = pipesystem.ToElements();
                    PipingSystemType pipesys = null;
                    foreach (Element sys in pipesystems)
                    {
                        PipingSystemType ps = sys as PipingSystemType;
                        if (ps.Name.Contains("给排水") && ps.Name.Contains("污水"))
                        {
                            pipesys = ps;
                            break;
                        }
                    }

                    FilteredElementCollector level = new FilteredElementCollector(doc);
                    level.OfClass(typeof(Level));
                    Level pipelevel = level.ToList().First() as Level;

                    using (Transaction ts = new Transaction(doc, "创建排水管道"))
                    {
                        ts.Start();

                        FamilyInstance well;

                        FilteredElementCollector wellCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_Site);
                        IList<Element> wells = wellCollector.ToElements();

                        XYZ point = null;
                        List<string> wellNumber = new List<string>();

                        foreach (Element s in wells)
                        {
                            FamilyInstance w = s as FamilyInstance;
                            if (w.Name.Contains("给排水") && w.Name.Contains("排水检查井"))
                            {
                                well = w;
                                string i_type = well.LookupParameter("标记").AsString();
                                wellNumber.Add(i_type);
                            }
                        }
                        wellNumber = wellNumber.OrderBy(s => int.Parse(Regex.Match(s, @"\d+").Value)).ThenBy(x => x.ToUpper()).ToList();
                        total = wellNumber.Count;

                        foreach (string p in wellNumber)
                        {
                            foreach (Element elm in wells)
                            {
                                FamilyInstance w = elm as FamilyInstance;
                                if (w.Name.Contains("给排水") && w.Name.Contains("排水检查井"))
                                {
                                    well = w;
                                    point = (well.Location as LocationPoint).Point;
                                    string s_type = well.Symbol.LookupParameter("类型标记").AsString();
                                    string i_type = well.LookupParameter("标记").AsString();
                                    if (p == i_type)
                                    {
                                        points.Add(point);
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < points.Count - 1; i++)
                        {
                            Pipe tp = Pipe.Create(doc, pipesys.Id, pt.Id, pipelevel.Id, ChangePoint(points.ElementAt(i), -1800), ChangePoint(points.ElementAt(i + 1), -1800));
                            ChangePipeSize(tp, "300");
                        }
                        points.Clear();
                        ts.Commit();
                    }
                }
                );
            }
            _externalEvent.Raise();
        }
        private void CollectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_externalEvent != null)
            {
                _executeHander.ExecuteAction = new Action<UIApplication>((app) =>
                {
                    if (app.ActiveUIDocument == null || app.ActiveUIDocument.Document == null)
                    {
                        return;
                    }
                    UIDocument uidoc = app.ActiveUIDocument;
                    Document doc = uidoc.Document;
                    using (Transaction ts = new Transaction(doc, "提取排水管道纵断数据"))
                    {
                        ts.Start();

                        FamilyInstance well;

                        FilteredElementCollector wellCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_Site);
                        IList<Element> wells = wellCollector.ToElements();
                        XYZ point = null;
                        List<string> wellNumber = new List<string>();

                        foreach (Element s in wells)
                        {
                            FamilyInstance w = s as FamilyInstance;
                            if (w.Name.Contains("给排水") && w.Name.Contains("排水检查井"))
                            {
                                well = w;
                                string i_type = well.LookupParameter("标记").AsString();
                                wellNumber.Add(i_type);
                            }
                        }
                        wellNumber = wellNumber.OrderBy(s => int.Parse(Regex.Match(s, @"\d+").Value)).ThenBy(x => x.ToUpper()).ToList();
                        total = wellNumber.Count;

                        if (this.PS_List.Items.Count != 0)
                        {
                            this.PS_List.Items.Clear();

                        }

                        foreach (string p in wellNumber)
                        {
                            foreach (Element elm in wells)
                            {
                                FamilyInstance w = elm as FamilyInstance;
                                if (w.Name.Contains("给排水") && w.Name.Contains("排水检查井"))
                                {
                                    well = w;
                                    point = (well.Location as LocationPoint).Point;
                                    string s_type = well.Symbol.LookupParameter("类型标记").AsString();
                                    string i_type = well.LookupParameter("标记").AsString();
                                    if (p == i_type)
                                    {
                                        this.PS_List.Items.Add(new WellInfo("'" + i_type.PadRight(5) + "'", (point.X * 0.3048).ToString("0.###"), (point.Y * 0.3048).ToString("0.###"), (point.Z * 0.3048).ToString("0.###")
                                                                                   , "700", "300", "0.003", s_type, "1"));
                                    }
                                }
                            }
                        }
                        ts.Commit();
                    }
                }
                );
            }
            _externalEvent.Raise();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            
            //this.Close();
        }
        private void WellButton_Click(object sender, RoutedEventArgs e)
        {
            if (_externalEvent != null)
            {
                _executeHander.ExecuteAction = new Action<UIApplication>((app) =>
                {
                    if (app.ActiveUIDocument == null || app.ActiveUIDocument.Document == null)
                    {
                        return;
                    }
                    try
                    {
                        UIDocument uidoc = app.ActiveUIDocument;
                        Document doc = uidoc.Document;

                        FilteredElementCollector wellCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol));
                        IList<Element> wells = wellCollector.ToElements();

                        FilteredElementCollector level = new FilteredElementCollector(doc);
                        level.OfClass(typeof(Level));
                        Level pipelevel = level.ToList().First() as Level;

                        FamilySymbol well = null;
                        XYZ point = null;
                        List<string> wellNumber = new List<string>();

                        foreach (Element s in wells)
                        {
                            FamilySymbol w = s as FamilySymbol;
                            if (w.Name.Contains("给排水") && w.Name.Contains("排水检查井") && w.Name.Contains("直通式"))
                            {
                                well = w;
                                break;
                            }
                        }

                        DataTable newTB = new DataTable();
                        //获取表头
                        for (int i = 0; i < this.PS_List.Columns.Count; i++)
                        {
                            if (this.PS_List.Columns[i].Visibility == System.Windows.Visibility.Visible)//只导出可见列  
                            {
                                if (this.PS_List.Columns[i].Header == null)
                                {
                                    newTB.Columns.Add(string.Empty);
                                    continue;
                                }
                                newTB.Columns.Add(this.PS_List.Columns[i].Header.ToString());//构建表头  
                            }
                        }
                        //获取内容
                        for (int i = 0; i < this.PS_List.Items.Count; i++)
                        {
                            DataRow row = newTB.NewRow();
                            for (int j = 0; j < this.PS_List.Columns.Count; j++)
                            {
                                System.Windows.Controls.DataGridCell cell1 = DataGridPlus.GetCell(this.PS_List, i, j);
                                string str = (cell1.Content as TextBlock).Text.ToString();
                                //TaskDialog.Show("Hello", str);
                                //if (string.IsNullOrEmpty(str) == false && string.IsNullOrWhiteSpace(str) == false && str != "\'")
                                //{
                                row[j] = str;
                                //}
                            }
                            newTB.Rows.Add(row);
                        }

                        //ExportDataGridToCSV(DataTableRemoveEmptyRow(newTB), "test");

                        List<string> Wellname = new List<string>();
                        List<string> Xpoints = new List<string>();
                        List<string> Ypoints = new List<string>();
                        List<string> Zpoints = new List<string>();
                        List<XYZ> wellpoints = new List<XYZ>();

                        Wellname = RemoveName(DataGridVaule(newTB, 0));
                        Xpoints = RemoveNull(DataGridVaule(newTB, 1));
                        Ypoints = RemoveNull(DataGridVaule(newTB, 2));
                        Zpoints = RemoveNull(DataGridVaule(newTB, 3));

                        for (int i = 0; i < Xpoints.Count; i++)
                        {
                            wellpoints.Add(new XYZ(Convert.ToDouble(Xpoints.ElementAt(i)) * 3.2808, Convert.ToDouble(Ypoints.ElementAt(i)) * 3.2808, Convert.ToDouble(Zpoints.ElementAt(i)) * 3.2808));
                        }

                        FamilyInstance wellinstance = null;
                        using (Transaction ts = new Transaction(doc, "创建排水井"))
                        {
                            ts.Start();
                            for (int i = 0; i < Xpoints.Count; i++)
                            {
                                wellinstance = doc.Create.NewFamilyInstance(wellpoints.ElementAt(i), well, pipelevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                IList<Parameter> list = wellinstance.GetParameters("标记");
                                Parameter param = list[0];
                                param.Set(Wellname.ElementAt(i));
                                //TaskDialog.Show("Hello", Wellname.ElementAt(i).ToString());
                                //TaskDialog.Show("Hello", Xpoints.ElementAt(i).ToString());
                                //TaskDialog.Show("Hello", Ypoints.ElementAt(i).ToString());
                                //TaskDialog.Show("Hello", Zpoints.ElementAt(i).ToString());
                            }
                            ts.Commit();
                        }
                        wellpoints.Clear();
                        Wellname.Clear();
                        Xpoints.Clear();
                        Ypoints.Clear();
                        Zpoints.Clear();
                        newTB.Clear();
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("错误", ex.StackTrace);
                    }
                }
                );
            }
            _externalEvent.Raise();
        }

        public static List<string> RemoveName(List<string> list)
        {
            List<string> lt = new List<string>();
            foreach (string s in list)
            {
                if (s.Length>2)
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
        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            if (_externalEvent != null)
            {
                _executeHander.ExecuteAction = new Action<UIApplication>((app) =>
                {
                    if (app.ActiveUIDocument == null || app.ActiveUIDocument.Document == null)
                    {
                        return;
                    }
                    if (this.PS_List.Items.Count != 0)
                    {
                        this.PS_List.Items.Clear();

                    }
                    string path = SelectPath();
                    //TaskDialog.Show("Hello", path);

                    DataTable dt = ImportFromCsv(path);
                    //DataTable dt = CSVFileHelper.OpenCSV(path);                   
                    List<string> st = new List<string>();
                    //TaskDialog.Show("Hello", dt.Rows.Count.ToString());

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        this.PS_List.Items.Add(new WellInfo("'" + dt.Rows[j][0].ToString() + "'", dt.Rows[j][1].ToString(), dt.Rows[j][2].ToString(), dt.Rows[j][3].ToString()
                        , "", dt.Rows[j][4].ToString(), dt.Rows[j][5].ToString(), dt.Rows[j][6].ToString(), dt.Rows[j][7].ToString()));

                    }

                }
                );
            }
            _externalEvent.Raise();
        }
        public static void ChangePipeSize(Pipe pipe, string diameter)
        {
            Parameter pdiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            pdiameter.SetValueString(diameter);
        }
        public static XYZ ChangePoint(XYZ point, double z)
        {
            double zz = z / 304.8;
            XYZ pt = new XYZ(point.X, point.Y, point.Z + zz);
            return pt;
        }
        public static DataTable ImportFromCsv(string filePath)//从csv读取数据返回table  
        {
            //Encoding encoding = System.Data.Common.GetType(filePath); //Encoding.ASCII;//  
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            //StreamReader sr = new StreamReader(fs, Encoding.UTF8);  
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            //记录每次读取的一行记录  
            string strLine = "";
            //记录每行记录中的各字段内容  
            string[] aryLine = null;
            //标示列数  
            int columnCount = 0;
            //标示是否是读取的第一行  
            bool IsFirst = true;
            //逐行读取CSV中的数据  
            while ((strLine = sr.ReadLine()) != null)
            {
                DataRow dr = dt.NewRow();
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列  
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i].ToString());
                        dt.Columns.Add(dc);
                    }
                }
                //else
                //{
                for (int j = 0; j < columnCount; j++)
                {
                    dr[j] = aryLine[j];
                }
                dt.Rows.Add(dr);
                //}
            }
            sr.Close();
            fs.Close();
            return dt;
        }
        private string SelectPath()
        {
            string path = string.Empty;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "纵断数据 (*.csv)|*.csv|所有文件 (*.*)|*.*"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                path = openFileDialog.FileName;
            }
            return path;
        }
    }
    public class ExecuteHander : IExternalEventHandler

    {
        public string Name { get; private set; }
        public Action<UIApplication> ExecuteAction { get; set; }
        public ExecuteHander(string name)

        {
            Name = name;
        }
        public void Execute(UIApplication app)
        {
            if (ExecuteAction != null)
            {
                try
                {
                    ExecuteAction(app);
                }
                catch
                { }
            }
        }
        public string GetName()
        {
            return Name;
        }
    }
    public class CSVFileHelper
    {
        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        public static void SaveCSV(DataTable dt, string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";
            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                    || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
            // DialogResult result = MessageBox.Show("CSV文件保存成功！");
            //if (result == DialogResult.OK)
            //{
            // System.Diagnostics.Process.Start("explorer.exe", Common.PATH_LANG);
            //}
        }
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
            System.IO.FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }
        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(System.IO.FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string filePath)
        {
            Encoding encoding = GetType(filePath); //Encoding.ASCII;//
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            StreamReader sr = new StreamReader(fs, encoding);
            //string fileContent = sr.ReadToEnd();
            //encoding = sr.CurrentEncoding;
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                //strLine = Common.ConvertStringUTF8(strLine, encoding);
                //strLine = Common.ConvertStringUTF8(strLine);

                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        public static string GetAppPath()
        {
            try
            {
                string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8); // 8是 file:// 的长度
                string[] arrSection = _CodeBase.Split(new char[] { '/' });
                string _FolderPath = "";
                for (int i = 0; i < arrSection.Length - 1; i++)
                {
                    _FolderPath += arrSection[i] + "/";
                }
                return _FolderPath.Replace("/", @"\");
            }
            catch
            {
                return null;
            }
        }
    }
    public class WellInfo
    {
        public string Name { set; get; }
        public string Aaxis { set; get; }
        public string Baxis { set; get; }
        public string Level { set; get; }
        public string Depth { set; get; }
        public string Diameter { set; get; }
        public string Slope { set; get; }
        public string Type { set; get; }
        public string Code { set; get; }
        public WellInfo(string name, string aaxis, string baxis, string level, string depth, string diamter, string slope, string type, string code)
        {
            this.Name = name;
            this.Aaxis = aaxis;
            this.Baxis = baxis;
            this.Level = level;
            this.Depth = depth;
            this.Diameter = diamter;
            this.Slope = slope;
            this.Type = type;
            this.Code = code;
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
