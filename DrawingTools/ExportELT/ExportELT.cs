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
                Parameter proNum = pro.LookupParameter("���̴���");
                Parameter proName = pro.LookupParameter("��������");
                Parameter subproNum = pro.LookupParameter("�������");
                Parameter subproName = pro.LookupParameter("��������");

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

                            //ָ����Ҫд���sheet��
                            ExcelWorksheet excelWorksheet = package.Workbook.Worksheets["sheet1"];
                            string footPage = "&\"Arial\"" + "&16" + " " + "��" + "&P" + "ҳ��" + "��" + "&N" + "ҳ";

                            excelWorksheet.HeaderFooter.differentOddEven = false;
                            excelWorksheet.HeaderFooter.OddFooter.LeftAlignedText = excelFooterName;
                            excelWorksheet.HeaderFooter.OddFooter.RightAlignedText = footPage;

                            excelWorksheet.Cells[1, 3].Value = projectName;
                            excelWorksheet.Cells[2, 3].Value = subProjectName;
                            excelWorksheet.Cells[3, 3].Value = "ʩ��ͼ";

                            int index = 7;
                            List<string> pipeSystemList = GetPipeSystemType(uidoc, "����ˮ");
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
                            sfd.Title = "�豸����";
                            sfd.FileName = dltName;
                            sfd.Filter = "Excel ��������*.xlsx��|*.xlsx";
                            //sfd.FilterIndex = 1;//����Ĭ���ļ�������ʾ˳��

                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                localFilePath = sfd.FileName.ToString();//����ļ�·��

                                fileName = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);   //��ȡ�ļ���������·��

                                filePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));//��ȡ�ļ�·���������ļ��� 

                                newFileName = DateTime.Now.ToString("yyyymmdd") + fileName;   //���ļ���ǰ����ʱ��

                                sfd.FileName.Insert(1, "abc");//���ļ���������ַ� 

                                helper.saveExcel(package, localFilePath);
                                System.Windows.Forms.MessageBox.Show("�豸�����ɹ�!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Process.Start(localFilePath);//���豸��
                            }
                        }
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {

                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("����ƽ�����ά��ͼ�н��в���!", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            // ��ȡ��ǰ��ͼ�ܵ�ϵͳ�����б�
            FilteredElementCollector viewCollector = new FilteredElementCollector(uiDoc.Document, uiDoc.ActiveView.Id);
            viewCollector.OfCategory(BuiltInCategory.OST_PipingSystem);
            IList<Element> pipesystems = viewCollector.ToElements();

            List<string> pipesystemname = new List<string>();
            foreach (Element e in pipesystems)
            {
                PipingSystem ps = e as PipingSystem;
                if (ps.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString().Contains(profession))
                {
                    string pipeSystemName = ps.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString().Replace("�ܵ�ϵͳ: ����ˮ_", "").Replace("�ܵ�", "");
                    pipesystemname.Add(pipeSystemName);
                }
            }

            List<string> newList = pipesystemname.Distinct().ToList();
            SortedList sl = new SortedList();
            foreach (string item in newList)
            {
                if (item == "ˮԴ��ˮϵͳ")
                {
                    sl.Add(1, item);
                }
                if (item == "ѭ����ˮϵͳ")
                {
                    sl.Add(2, item);
                }
                if (item == "ѭ����ˮϵͳ")
                {
                    sl.Add(3, item);
                }
                if (item == "������ˮϵͳ")
                {
                    sl.Add(4, item);
                }
                if (item == "�����ˮϵͳ")
                {
                    sl.Add(5, item);
                }
                if (item == "��ˮϵͳ")
                {
                    sl.Add(6, item);
                }
                if (item == "��ˮϵͳ")
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
                if (v.Name.Contains("��·����") && v.Name.Contains(pipeSystemName.Replace("ϵͳ", "")))
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

                        PipeValveInfo valveInfo = new PipeValveInfo(valveTable.ElementAt(0).Replace("����ˮ_", "").Replace("�ܵ�", ""), subProjectNum, "VA" + valveCode.ToString().PadLeft(2, '0'), StringHelper.FilterCH(valveTable.ElementAt(1).Replace("����ˮ_����_", "")),
                            StringHelper.FilterEN(valveTable.ElementAt(1).Replace("����ˮ_����_", "")), "DN" + sArray.FirstOrDefault(), valveTable.ElementAt(2), valveTable.ElementAt(4), valveTable.ElementAt(5));
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
        public string ValveAbb { get; set; }//������д
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
    /// ����EPPlus��excel������,��֧��xlsx��ʽ��excel�ļ�
    /// </summary>
    class ExcelHelper
    {
        /// <summary>
        /// ��excel�ļ�
        /// </summary>
        /// <param name="openPath">excel�ļ�·��</param>
        /// <returns>excel����</returns>
        public ExcelPackage OpenExcel(string openPath)
        {
            FileStream excelFile = new FileStream(openPath, FileMode.Open, FileAccess.Read);
            ExcelPackage package = new ExcelPackage(excelFile);
            return package;
        }
        /// <summary>
        /// ���excel�ļ�
        /// </summary>
        /// <param name="package">excel�ļ�����</param>
        /// <param name="savePath">����·��</param>
        public void saveExcel(ExcelPackage package, string savePath)
        {
            FileStream excelFile = new FileStream(savePath, FileMode.Create);
            package.SaveAs(excelFile);
            excelFile.Dispose();
            package.Dispose();   //�ͷ���Դ��һ��Ҳ�ɲ���using���         
        }
        /// <summary>
        /// �ϲ���
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="mergeRowIndexs">�ϲ��е���������ʼλ�ã���ֹλ��</param>
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
        /// ���������ַ����������ģ���ĸ�����֣���-
        /// </summary>
        /// <param name="inputValue">�����ַ���</param>
        /// <remarks>����-�������ַ�����Ҫ���˵�</remarks>
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
