using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Interop;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Drawing;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Architecture;
using OfficeOpenXml;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class ExportELT : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ProjectInfo pro = doc.ProjectInformation;
                Parameter proNum = pro.LookupParameter("工程代号");
                Parameter proName = pro.LookupParameter("工程名称");
                Parameter subproNum = pro.LookupParameter("子项代号");
                Parameter subproName = pro.LookupParameter("子项名称");

                string projectName = proNum.AsString() + "-" + proName.AsString();
                string subProjectName = subproNum.AsString() + "-" + subproName.AsString();
                string footerName = proNum.AsString() + "-" + subproNum.AsString() + "-" + "WD-ELT";
                string excelFooterName = "&\"Arial\"" + "&16" + " " + footerName;

                ExportELTWindow mainfrm = new ExportELTWindow();
                mainfrm.ShowDialog();

                if (uidoc.ActiveView is ViewPlan || uidoc.ActiveView is View3D)
                {
                    try
                    {
                        if (mainfrm.clicked == 1)
                        {
                            ExcelHelper helper = new ExcelHelper();
                            string path = @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\ExcelTemplate\CNTemplate-ELT.xlsx";
                            ExcelPackage package = helper.OpenExcel(path);

                            //指定需要写入的sheet名
                            ExcelWorksheet excelWorksheet = package.Workbook.Worksheets["sheet1"];
                            string footPage = "&\"Arial\"" + "&16" + " " + "第" + "&P" + "页，" + "共" + "&N" + "页";

                            excelWorksheet.HeaderFooter.differentOddEven = false;
                            excelWorksheet.HeaderFooter.OddFooter.LeftAlignedText = excelFooterName;
                            excelWorksheet.HeaderFooter.OddFooter.RightAlignedText = footPage;

                            excelWorksheet.Cells[1, 3].Value = projectName;
                            excelWorksheet.Cells[2, 3].Value = subProjectName;
                            excelWorksheet.Cells[3, 3].Value = "施工图";

                            int index = 7;
                            List<string> pipeSystemList = GetPipeSystemType(uidoc, "给排水");
                            //string ss = String.Join("\n", pipeSystemList.ToArray());
                            //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            foreach (string item in pipeSystemList)
                            {
                                GetPipeSystemValve(doc, item, subproNum.AsString(), 1);
                            }

                            int[,] mergeRowIndexs = { { 6, 1, 11 }, { 6, 1, 11 } };
                            ExcelHelper.MergeRowCells(excelWorksheet, 1, mergeRowIndexs);

                            string localFilePath, fileName, newFileName, filePath;
                            string dltName = proNum.AsString() + "-" + subproNum.AsString().Replace("/", " ") + "-" + "WD" + "-" + "ELT" + "." + "xlsx";

                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Title = "设备表导出";
                            sfd.FileName = dltName;
                            sfd.Filter = "Excel 工作薄（*.xlsx）|*.xlsx";
                            //sfd.FilterIndex = 1;//设置默认文件类型显示顺序

                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                localFilePath = sfd.FileName.ToString();//获得文件路径

                                fileName = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);   //获取文件名，不带路径

                                filePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));//获取文件路径，不带文件名 

                                newFileName = DateTime.Now.ToString("yyyymmdd") + fileName;   //给文件名前加上时间

                                sfd.FileName.Insert(1, "abc");//在文件名里插入字符 

                                helper.saveExcel(package, localFilePath);
                                System.Windows.Forms.MessageBox.Show("设备表导出成功!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Process.Start(localFilePath);//打开设备表
                            }
                        }
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {

                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("请在平面或三维视图中进行操作!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Failed;
            }
        }
        public static List<string> GetPipeSystemType(UIDocument uiDoc, string profession)
        {
            // 获取当前视图管道系统名称列表
            FilteredElementCollector viewCollector = new FilteredElementCollector(uiDoc.Document, uiDoc.ActiveView.Id);
            viewCollector.OfCategory(BuiltInCategory.OST_PipingSystem);
            IList<Element> pipesystems = viewCollector.ToElements();

            List<string> pipesystemname = new List<string>();
            foreach (Element e in pipesystems)
            {
                PipingSystem ps = e as PipingSystem;
                if (ps.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString().Contains(profession))
                {
                    string pipeSystemName = ps.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString().Replace("管道系统: 给排水_", "").Replace("管道", "");
                    pipesystemname.Add(pipeSystemName);
                }
            }

            List<string> newList = pipesystemname.Distinct().ToList();
            SortedList sl = new SortedList();
            foreach (string item in newList)
            {
                if (item == "水源输水系统")
                {
                    sl.Add(1, item);
                }
                if (item == "循环给水系统")
                {
                    sl.Add(2, item);
                }
                if (item == "循环回水系统")
                {
                    sl.Add(3, item);
                }
                if (item == "消防给水系统")
                {
                    sl.Add(4, item);
                }
                if (item == "生活给水系统")
                {
                    sl.Add(5, item);
                }
                if (item == "中水系统")
                {
                    sl.Add(6, item);
                }
                if (item == "污水系统")
                {
                    sl.Add(7, item);
                }
            }

            List<string> orderedList = new List<string>();
            foreach (var item in sl.Values)
            {
                orderedList.Add(item.ToString());
            }
            return orderedList;
        }
        public static List<PipeValveInfo> GetPipeSystemValve(Document doc, string pipeSystemName, string subProjectNum, int valveCode)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeValveInfo> valveNameList = new List<PipeValveInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管路附件") && v.Name.Contains(pipeSystemName.Replace("系统", "")))
                {
                    TableData td = v.GetTableData();
                    TableSectionData tdb = td.GetSectionData(SectionType.Header);
                    string head = v.GetCellText(SectionType.Header, 0, 0);

                    TableSectionData tdd = td.GetSectionData(SectionType.Body);
                    int c = tdd.NumberOfColumns;
                    int r = tdd.NumberOfRows;
                    List<string> valveTable = new List<string>();

                    for (int i = 1; i < r; i++)
                    {
                        for (int j = 0; j < c; j++)
                        {
                            CellType ctype = tdd.GetCellType(i, j);
                            string str = v.GetCellText(SectionType.Body, i, j);
                            valveTable.Add(str);
                        }
                        string[] sArray = valveTable.ElementAt(3).Split('-');

                        PipeValveInfo valveInfo = new PipeValveInfo(valveTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "VA" + valveCode.ToString().PadLeft(2, '0'), StringHelper.FilterCH(valveTable.ElementAt(1).Replace("给排水_阀门_", "")),
                            StringHelper.FilterEN(valveTable.ElementAt(1).Replace("给排水_阀门_", "")), "DN" + sArray.FirstOrDefault(), valveTable.ElementAt(2), valveTable.ElementAt(4), valveTable.ElementAt(5));
                        valveNameList.Add(valveInfo);
                        valveTable.Clear();
                        string ss = valveInfo.ValvePipeSystem + "\n" + valveInfo.ProjectNum + "\n" + valveInfo.ValveAbb + "\n" + valveInfo.ValveName + "\n" + valveInfo.ValveModel
                            + "\n" + valveInfo.ValveSize + "\n" + valveInfo.ValvePressure + "\n" + valveInfo.ValveQulity + "\n" + valveInfo.ValveNote;
                        //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        valveCode++;
                    }

                }
            }
            return valveNameList;
        }
    }
    public class PipeValveInfo
    {
        public string ProjectNum { get; set; }
        public string ValvePipeSystem { get; set; }
        public string ValveAbb { get; set; }//阀门缩写
        public string ValveName { get; set; }
        public string ValveModel { get; set; }
        public string ValveSize { get; set; }
        public string ValvePressure { get; set; }
        public string ValveQulity { get; set; }
        public string ValveNote { get; set; }
        public PipeValveInfo()
        {

        }
        public PipeValveInfo(string valvePipeSystem, string projectNum, string valveAbb, string valveName,
            string valveModel, string valveSize, string valvePressure, string valveQulity, string valveNote)
        {
            ProjectNum = projectNum;
            ValvePipeSystem = valvePipeSystem;
            ValveAbb = valveAbb;
            ValveName = valveName;
            ValveModel = valveModel;
            ValveSize = valveSize;
            ValvePressure = valvePressure;
            ValveQulity = valveQulity;
            ValveNote = valveNote;
        }
    }

    /// <summary>
    /// 基于EPPlus的excel操作类,仅支持xlsx格式的excel文件
    /// </summary>
    class ExcelHelper
    {
        /// <summary>
        /// 打开excel文件
        /// </summary>
        /// <param name="openPath">excel文件路径</param>
        /// <returns>excel对象</returns>
        public ExcelPackage OpenExcel(string openPath)
        {
            FileStream excelFile = new FileStream(openPath, FileMode.Open, FileAccess.Read);
            ExcelPackage package = new ExcelPackage(excelFile);
            return package;
        }
        /// <summary>
        /// 另存excel文件
        /// </summary>
        /// <param name="package">excel文件对象</param>
        /// <param name="savePath">保存路径</param>
        public void saveExcel(ExcelPackage package, string savePath)
        {
            FileStream excelFile = new FileStream(savePath, FileMode.Create);
            package.SaveAs(excelFile);
            excelFile.Dispose();
            package.Dispose();   //释放资源，一般也可采用using语句         
        }
        /// <summary>
        /// 合并行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="mergeRowIndexs">合并行的行数，起始位置，终止位置</param>
        public static void MergeRowCells(ExcelWorksheet sheet, int startRowIndex, int[,] mergeRowIndexs)
        {
            for (int i = 0; i < mergeRowIndexs.Rank; i++)
            {
                sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Merge = true;
                sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            }
        }
    }
    class StringHelper
    {
        /// <summary>
        /// 过滤特殊字符，保留中文，字母，数字，和-
        /// </summary>
        /// <param name="inputValue">输入字符串</param>
        /// <remarks>带有-的特殊字符不需要过滤掉</remarks>
        /// <returns></returns>
        public static string FilterCH(string inputValue)
        {
            if (Regex.IsMatch(inputValue, "[\u4e00-\u9fa5]+"))
            {
                return Regex.Match(inputValue, "[\u4e00-\u9fa5]+").Value;
            }
            return "";
        }
        public static string FilterEN(string inputValue)
        {
            if (Regex.IsMatch(inputValue, "[A-Za-z0-9\u9fa5-]+"))
            {
                return Regex.Match(inputValue, "[A-Za-z0-9\u9fa5-]+").Value;
            }
            return "";
        }
    }
}
