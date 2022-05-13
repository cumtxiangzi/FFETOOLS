using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Reflection;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using System.Windows.Interop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Windows.Forms;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class ExportDLT : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                ProjectInfo pro = doc.ProjectInformation;
                Parameter proNum = pro.LookupParameter("工程代号");
                Parameter proName = pro.LookupParameter("工程名称");
                Parameter subproNum = pro.LookupParameter("子项代号");
                Parameter subproName = pro.LookupParameter("子项名称");

                string projectName = proNum.AsString() + "-" + proName.AsString();
                string subProjectName = subproNum.AsString() + "-" + subproName.AsString();
                string footerName = proNum.AsString() + "-" + subproNum.AsString() + "-" + "WD-DLT";
                string excelFooterName = "&\"Arial\"" + "&16" + " " + footerName;

                ExportDLTWindow eltwin = new ExportDLTWindow();
                eltwin.ShowDialog();

                string path = "";

                if (eltwin.CH_Button.IsChecked == true)
                {
                    path = "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\ExcelTemplate\\CNTemplate-DLT.xlsx";
                }
                if (eltwin.CH_EN_Button.IsChecked == true)
                {
                    path = "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\ExcelTemplate\\CNENTemplate-DLT.xlsx";
                }
                if (eltwin.EN_Button.IsChecked == true)
                {
                    path = "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\ExcelTemplate\\ENTemplate-DLT.xlsx";
                }

                ExcelHelper helper = new ExcelHelper();
                ExcelPackage package = helper.OpenExcel(path);

                //指定需要写入的sheet名
                ExcelWorksheet excelWorksheet = package.Workbook.Worksheets["2017"];
                string footPage = "&\"Arial\"" + "&16" + " " + "第" + "&P" + "页，" + "共" + "&N" + "页";

                excelWorksheet.HeaderFooter.differentOddEven = false;
                excelWorksheet.HeaderFooter.OddFooter.LeftAlignedText = excelFooterName;
                excelWorksheet.HeaderFooter.OddFooter.RightAlignedText = footPage;

                excelWorksheet.Cells[1, 4].Value = projectName;
                excelWorksheet.Cells[2, 4].Value = subProjectName;
                excelWorksheet.Cells[1, 4].Style.Font.Name = "Arial";
                excelWorksheet.Cells[2, 4].Style.Font.Name = "Arial";
                excelWorksheet.Cells[1, 4].Style.Font.Size = 18;
                excelWorksheet.Cells[2, 4].Style.Font.Size = 18;

                IList<ViewSheet> ALLviewSheets = CollectorHelper.TCollector<ViewSheet>(doc);

                List<ViewSheet> WDviewsheets = new List<ViewSheet>();
                List<ViewSheet> WLviewsheets = new List<ViewSheet>();

                foreach (var vs in ALLviewSheets)
                {
                    if (vs.Title.Contains("WL") && !(vs.Name.Contains("材料表")) && !(vs.Name.Contains("未命名")))
                    {
                        WLviewsheets.Add(vs);
                    }
                }
                WLviewsheets.Sort(new ViewSheetComparer());

                foreach (var vs in ALLviewSheets)
                {
                    if (vs.Title.Contains("WD") && !(vs.Name.Contains("材料表")) && !(vs.Name.Contains("未命名")))
                    {
                        WDviewsheets.Add(vs);
                    }
                }
                WDviewsheets.Sort(new ViewSheetComparer());

                List<DrawingInfoStore> drawingInfoStores = new List<DrawingInfoStore>();
                IList<FamilyInstance> titleBlocks = CollectorHelper.TCollector<FamilyInstance>(doc);
            
                for (int i = 0; i < WLviewsheets.Count; i++)
                {
                    FamilyInstance tbInstance = null;
                    foreach (var tb in titleBlocks)
                    {
                        if (tb.OwnerViewId == WLviewsheets[i].Id)
                        {
                            tbInstance = tb;
                            break;
                        }
                    }
                    string size = GetDrawingSize(tbInstance);

                    drawingInfoStores.Add(new DrawingInfoStore()
                    {
                        Code = (i + 1).ToString(),
                        Title = proNum.AsString() + "-" + subproNum.AsString() + "-" + WLviewsheets[i].SheetNumber,
                        DrawingName = WLviewsheets[i].ViewName,
                        DrawingSize = size
                    });
                }

                for (int i = 0; i < WDviewsheets.Count; i++)
                {
                    FamilyInstance tbInstance = null;
                    foreach (var tb in titleBlocks)
                    {
                        if (tb.OwnerViewId == WDviewsheets[i].Id)
                        {
                            tbInstance = tb;
                            break;
                        }
                    }
                    string size = GetDrawingSize(tbInstance);

                    drawingInfoStores.Add(new DrawingInfoStore()
                    {
                        Code = (i + 1 + WLviewsheets.Count).ToString(),
                        Title = proNum.AsString() + "-" + subproNum.AsString() + "-" + WDviewsheets[i].SheetNumber,
                        DrawingName = WDviewsheets[i].ViewName,
                        DrawingSize = size
                    });
                }

                int totalNum = drawingInfoStores.Count;
                for (int i = 0; i < totalNum; i++)
                {
                    excelWorksheet.Cells[i + 5, 1].Value = drawingInfoStores[i].Code;
                    excelWorksheet.Cells[i + 5, 2].Value = drawingInfoStores[i].Title;

                    excelWorksheet.Cells[i + 5, 1].Style.Font.Size = 16;
                    excelWorksheet.Cells[i + 5, 1].Style.Font.Name = "Arial";
                    excelWorksheet.Cells[i + 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    if (subproNum.AsString().Length > 4)
                    {
                        excelWorksheet.Cells[i + 5, 2].Style.Font.Size = 13;
                        excelWorksheet.Cells[i + 5, 2].Style.Font.Name = "Arial";
                    }
                    else
                    {
                        excelWorksheet.Cells[i + 5, 2].Style.Font.Size = 16;
                        excelWorksheet.Cells[i + 5, 2].Style.Font.Name = "Arial";
                    }

                    excelWorksheet.Cells[i + 5, 4].Value = drawingInfoStores[i].DrawingName;
                    excelWorksheet.Cells[i + 5, 4].Style.Font.Size = 16;
                    excelWorksheet.Cells[i + 5, 4].Style.Font.Name = "Arial";
                    excelWorksheet.Cells[i + 5, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    excelWorksheet.Cells[i + 5, 5].Value = Convert.ToDouble(drawingInfoStores[i].DrawingSize);
                    excelWorksheet.Cells[i + 5, 5].Style.Font.Size = 16;
                    excelWorksheet.Cells[i + 5, 5].Style.Font.Name = "Arial";
                    excelWorksheet.Cells[i + 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excelWorksheet.Cells[i + 5, 5].Style.Numberformat.Format = "0.00";
                }

                excelWorksheet.Cells[totalNum + 5, 1].Value = (totalNum + 1).ToString();
                excelWorksheet.Cells[totalNum + 5, 4].Value = "给排水设备表" + "（" + eltwin.eltNum.ToString() + "页）";
                excelWorksheet.Cells[totalNum + 5, 1].Style.Font.Size = 16;
                excelWorksheet.Cells[totalNum + 5, 1].Style.Font.Name = "Arial";
                excelWorksheet.Cells[totalNum + 5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                excelWorksheet.Cells[totalNum + 5, 4].Style.Font.Size = 16;
                excelWorksheet.Cells[totalNum + 5, 4].Style.Font.Name = "Arial";
                excelWorksheet.Cells[totalNum + 5, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                double eltsize = Convert.ToDouble(eltwin.eltNum) * 0.125;
                int n = eltsize.ToString().Length - eltsize.ToString().IndexOf(".") - 1;
                excelWorksheet.Cells[totalNum + 5, 5].Value = eltsize;
                excelWorksheet.Cells[totalNum + 5, 5].Style.Font.Size = 16;
                excelWorksheet.Cells[totalNum + 5, 5].Style.Font.Name = "Arial";
                excelWorksheet.Cells[totalNum + 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                excelWorksheet.Cells["" + "E" + (totalNum + 8).ToString() + ""].Formula = "=SUM(E5:" + "E" + (totalNum + 5).ToString() + ")";
                excelWorksheet.Cells[totalNum + 8, 4].Value = "总计：";
                excelWorksheet.Cells[totalNum + 8, 4].Style.Font.Size = 16;
                excelWorksheet.Cells[totalNum + 8, 4].Style.Font.Name = "Arial";
                excelWorksheet.Cells[totalNum + 8, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                excelWorksheet.Cells[totalNum + 8, 4].Style.Font.Name = "Arial";
                excelWorksheet.Cells[totalNum + 8, 5].Style.Font.Name = "Arial";
                excelWorksheet.Cells[totalNum + 8, 5].Style.Font.Size = 16;
                excelWorksheet.Cells[totalNum + 8, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                if (n == 3)
                {
                    excelWorksheet.Cells[totalNum + 5, 5].Style.Numberformat.Format = "0.000";
                    excelWorksheet.Cells[totalNum + 8, 5].Style.Numberformat.Format = "0.000";
                }
                else
                {
                    excelWorksheet.Cells[totalNum + 5, 5].Style.Numberformat.Format = "0.00";
                    excelWorksheet.Cells[totalNum + 8, 5].Style.Numberformat.Format = "0.00";
                }

                try
                {
                    if (!(eltwin.Note.Text.Length == 0))
                    {
                        string localFilePath, fileName, newFileName, filePath;
                        string dltName = proNum.AsString() + "-" + subproNum.AsString().Replace("/", " ") + "-" + "WD" + "-" + "DLT" + "." + "xlsx";

                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Title = "图纸目录导出";
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
                            System.Windows.Forms.MessageBox.Show("图纸目录导出成功!", "GPSBIM", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start(localFilePath);//打开设备表
                        }
                    }
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {

                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.ToString();
                return Result.Failed;
            }
        }
        public string GetDrawingSize(FamilyInstance tl)
        {
            string size = "1";

            if (tl.Name.Contains("A0"))
            {
                size = "2";
            }
            else if (tl.Name == "A1")
            {
                size = "1";
            }
            else if (tl.Name == "A1.25")
            {
                size = "1.25";
            }
            else if (tl.Name == "A1.5")
            {
                size = "1.5";
            }
            else if (tl.Name == "A1.75")
            {
                size = "1.75";
            }
            else if (tl.Name.Contains("A2"))
            {
                size = "0.5";
            }
            return size;
        }
    }
    public class DrawingInfoStore
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string DrawingName { get; set; }
        public string DrawingSize { get; set; }

    }
    public class ViewSheetComparer : IComparer<ViewSheet>
    {
        public int Compare(ViewSheet x, ViewSheet y)
        {
            return (x.Title.CompareTo(y.Title));
        }
    }
}
