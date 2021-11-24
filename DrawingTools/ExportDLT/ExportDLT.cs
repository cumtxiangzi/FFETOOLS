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
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Windows.Interop;
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
                Parameter proNam = pro.LookupParameter("工程名称");
                Parameter subproNum = pro.LookupParameter("子项代号");
                Parameter subproNam = pro.LookupParameter("子项名称");

                ExportDLTWindow eltwin = new ExportDLTWindow();
                eltwin.ShowDialog();

                XSSFWorkbook wk;
                FileStream fs = null;

                if (eltwin.CH_Button.IsChecked == true)
                {
                    fs = new FileStream("C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\ExcelTemplate\\CNTemplate-DLT.xlsx", FileMode.Open, FileAccess.Read);
                }
                if (eltwin.CH_EN_Button.IsChecked == true)
                {
                    fs = new FileStream("C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\ExcelTemplate\\CNENTemplate-DLT.xlsx", FileMode.Open, FileAccess.Read);
                }
                if (eltwin.EN_Button.IsChecked == true)
                {
                    fs = new FileStream("C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\ExcelTemplate\\ENTemplate-DLT.xlsx", FileMode.Open, FileAccess.Read);
                }

                wk = new XSSFWorkbook(fs);
                fs.Close();

                XSSFSheet sheet = (XSSFSheet)wk.GetSheetAt(0);
                string eltname = proNum.AsString() + "-" + subproNum.AsString() + "-" + "WD" + "-" + "DLT";
                string eltname2 = "&\"Arial\"" + "&16" + " " + eltname;
                sheet.Footer.Left = eltname2;


                ICellStyle style6 = wk.CreateCellStyle();
                IFont font6 = wk.CreateFont();
                font6.FontName = "Arial";
                font6.FontHeightInPoints = 16;
                style6.SetFont(font6);
                style6.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style6.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                style6.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                ICellStyle style7 = wk.CreateCellStyle();
                IFont font7 = wk.CreateFont();
                font7.FontName = "Arial";
                font7.FontHeightInPoints = 16;
                style7.SetFont(font7);
                style7.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style7.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                style7.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                ICellStyle style71 = wk.CreateCellStyle();
                IFont font71 = wk.CreateFont();
                font71.FontName = "Arial";
                font71.FontHeightInPoints = 14;
                style71.SetFont(font71);
                style71.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style71.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                style71.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                ICellStyle style8 = wk.CreateCellStyle();
                IFont font8 = wk.CreateFont();
                font8.FontName = "Arial";
                font8.FontHeightInPoints = 16;
                style8.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                style8.SetFont(font8);
                style8.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                style8.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style8.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style8.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style8.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style8.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                ICellStyle style10 = wk.CreateCellStyle();
                style10.WrapText = true;//设置换行这个要先设置
                IFont font10 = wk.CreateFont();
                font10.FontName = "Arial";
                font10.FontHeightInPoints = 18;
                font10.Boldweight= (short)FontBoldWeight.Bold;
                style10.SetFont(font10);
                style10.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style10.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                style10.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

                XSSFCell cell = (XSSFCell)sheet.GetRow(0).GetCell(3);
                cell.SetCellValue(proNum.AsString() + "-" + proNam.AsString());              
                cell.CellStyle = style10;

                cell = (XSSFCell)sheet.GetRow(1).GetCell(3);
                cell.SetCellValue(subproNum.AsString() + "-" + subproNam.AsString());

                List<ViewSheet> wviewsheets = new List<ViewSheet>();
                IList<Element> elems = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).ToElements();
                foreach (Element e in elems)
                {
                    ViewSheet vs = e as ViewSheet;
                    if (vs.Title.Contains("WD") && !(vs.Name.Contains("材料表")) && !(vs.Name.Contains("未命名")))
                    {
                        wviewsheets.Add(vs);
                    }

                }
                wviewsheets.Sort(new ViewSheetComparer());

                IList<Element> titleblocks = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_TitleBlocks).ToElements();
                List<string> sizelist = new List<string>();
                List<FamilyInstance> WtitleBlocks = new List<FamilyInstance>();

                foreach (var view in wviewsheets)
                {
                    foreach (var item in titleblocks)
                    {
                        FamilyInstance tl = item as FamilyInstance;
                        Element el = doc.GetElement(tl.OwnerViewId);
                        ViewSheet vs1 = el as ViewSheet;
                        if (vs1.Title == view.Title)
                        {
                            WtitleBlocks.Add(tl);
                        }
                    }
                }

                foreach (FamilyInstance tl in WtitleBlocks)
                {
                    if (tl.Name.Contains("A0"))
                    {
                        sizelist.Add("2");
                    }
                    else if (tl.Name == "A1")
                    {
                        sizelist.Add("1");
                    }
                    else if (tl.Name == "A1.25")
                    {
                        sizelist.Add("1.25");
                    }
                    else if (tl.Name == "A1.5")
                    {
                        sizelist.Add("1.5");
                    }
                    else if (tl.Name == "A1.75")
                    {
                        sizelist.Add("1.75");
                    }
                    else if (tl.Name.Contains("A2"))
                    {
                        sizelist.Add("0.5");
                    }
                    else
                    {
                        //sizelist.Add("1");
                    }
                }

                int num = wviewsheets.Count;
                List<string> strlist = new List<string>();
                foreach (ViewSheet vs in wviewsheets)
                {
                    strlist.Add(vs.Name);
                }

                if (eltwin.MainWorkShop.IsChecked == true)
                {
                    if (subproNum.AsString().Contains("913") && subproNum.AsString().Contains("919"))
                    {
                        strlist.Insert(0, "联合水泵站流程图");
                        strlist.Insert(0, "给水处理流程图");
                        sizelist.Insert(0, "1");
                        sizelist.Insert(0, "1");
                        num = num + 2;
                    }
                    if (!(subproNum.AsString().Contains("913")) && subproNum.AsString().Contains("919"))
                    {
                        strlist.Insert(0, "联合水泵站流程图");
                        sizelist.Insert(0, "1");
                        num = num + 1;
                    }
                    if (!(subproNum.AsString().Contains("919")) && subproNum.AsString().Contains("913"))
                    {
                        strlist.Insert(0, "给水处理流程图");
                        sizelist.Insert(0, "1");
                        num = num + 1;
                    }
                    if (subproNum.AsString().Contains("91N"))
                    {
                        strlist.Insert(0, "污水处理流程图");
                        sizelist.Insert(0, "1");
                        num = num + 1;
                    }
                    if (subproNum.AsString().Contains("92N"))
                    {
                        strlist.Insert(0, "废水处理流程图");
                        sizelist.Insert(0, "1");
                        num = num + 1;
                    }
                    if (subproNum.AsString().Contains("91B"))
                    {
                        strlist.Insert(0, "消防泵站流程图");
                        sizelist.Insert(0, "1");
                        num = num + 1;
                    }
                }

                List<string> drawingCodeList = new List<string>();
                if (eltwin.MainWorkShop.IsChecked == true)
                {
                    if (subproNum.AsString().Contains("913") && subproNum.AsString().Contains("919"))
                    {
                        drawingCodeList.Add(proNum.AsString() + "-" + "913" + "-" + "WL" + "-" + 1.ToString().PadLeft(3, '0'));
                        drawingCodeList.Add(proNum.AsString() + "-" + "919" + "-" + "WL" + "-" + 1.ToString().PadLeft(3, '0'));
                    }
                    if (!(subproNum.AsString().Contains("913")) && subproNum.AsString().Contains("919"))
                    {
                        drawingCodeList.Add(proNum.AsString() + "-" + "919" + "-" + "WL" + "-" + 1.ToString().PadLeft(3, '0'));
                    }
                    if (!(subproNum.AsString().Contains("919")) && subproNum.AsString().Contains("913"))
                    {
                        drawingCodeList.Add(proNum.AsString() + "-" + "913" + "-" + "WL" + "-" + 1.ToString().PadLeft(3, '0'));
                    }
                    if (subproNum.AsString().Contains("91N"))
                    {
                        drawingCodeList.Add(proNum.AsString() + "-" + "91N" + "-" + "WL" + "-" + 1.ToString().PadLeft(3, '0'));
                    }
                    if (subproNum.AsString().Contains("92N"))
                    {
                        drawingCodeList.Add(proNum.AsString() + "-" + "92N" + "-" + "WL" + "-" + 1.ToString().PadLeft(3, '0'));
                    }
                    if (subproNum.AsString().Contains("91B"))
                    {
                        drawingCodeList.Add(proNum.AsString() + "-" + "91B" + "-" + "WL" + "-" + 1.ToString().PadLeft(3, '0'));
                    }

                }

                for (int i = 0; i < num; i++)
                {

                    drawingCodeList.Add(proNum.AsString() + "-" + subproNum.AsString() + "-" + "WD" + "-" + (i + 1).ToString().PadLeft(3, '0'));
                }

                for (int i = 0; i < num; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(i + 4).GetCell(0);
                    cell.SetCellValue(i + 1);
                    cell.CellStyle = style7;

                    cell = (XSSFCell)sheet.GetRow(i + 4).GetCell(1);
                    cell.SetCellValue(drawingCodeList.ElementAt(i));

                    if (subproNum.AsString().Length > 4)
                    {
                        cell.CellStyle = style71;
                    }
                    else
                    {
                        cell.CellStyle = style7;
                    }

                    cell = (XSSFCell)sheet.GetRow(i + 4).GetCell(3);
                    cell.SetCellValue(strlist.ElementAt(i));
                    cell.CellStyle = style6;

                    cell = (XSSFCell)sheet.GetRow(i + 4).GetCell(4);
                    cell.SetCellValue(Convert.ToDouble(sizelist.ElementAt(i)));
                    cell.CellStyle = style8;
                }

                cell = (XSSFCell)sheet.GetRow(num + 4).GetCell(0);
                cell.SetCellValue((num + 1).ToString());
                cell.CellStyle = style7;

                cell = (XSSFCell)sheet.GetRow(num + 4).GetCell(1);
                cell.SetCellValue("");

                cell = (XSSFCell)sheet.GetRow(num + 4).GetCell(3);
                IFont font = wk.CreateFont();
                font.FontName = "Arial";
                font.FontHeightInPoints = 16;


                cell.SetCellValue("给排水设备表" + "（" + eltwin.eltNum.ToString() + "页）");
                ICellStyle style5 = wk.CreateCellStyle();
                style5.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style5.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style5.SetFont(font);
                cell.CellStyle = style5;
                double eltsize = Convert.ToDouble(eltwin.eltNum) * 0.125;
                int n = eltsize.ToString().Length - eltsize.ToString().IndexOf(".") - 1;

                ICellStyle style2 = wk.CreateCellStyle();
                style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                style2.SetFont(font);
                style2.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                style2.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                ICellStyle style3 = wk.CreateCellStyle();
                style3.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                style3.SetFont(font);
                style3.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                style3.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style3.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                cell = (XSSFCell)sheet.GetRow(num + 4).GetCell(4);
                cell.SetCellValue(eltsize);
                if (n == 3)
                {
                    cell.CellStyle = style3;
                }
                else
                {
                    cell.CellStyle = style2;
                }

                ICellStyle style = wk.CreateCellStyle();
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                style.SetFont(font);
                cell = (XSSFCell)sheet.GetRow(num + 7).GetCell(3);
                cell.SetCellValue("总计：");
                cell.CellStyle = style;
                cell = (XSSFCell)sheet.GetRow(num + 7).GetCell(4);
                string sum = "sum(E5:" + "E" + (num + 5).ToString() + ")";
                cell.SetCellFormula(sum);

                ICellStyle style4 = wk.CreateCellStyle();
                style4.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                style4.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style4.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style4.SetFont(font);
                if (n == 3)
                {
                    style4.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
                }
                else
                {
                    style4.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                }
                cell.CellStyle = style4;

                try
                {
                    if (!(eltwin.Note.Text.Length == 0))
                    {
                        string dltName = proNum.AsString() + "-" + subproNum.AsString().Replace("/", " ") + "-" + "WD" + "-" + "DLT" + "." + "xlsx";
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.FileName = dltName;
                        sfd.Filter = "Excel 工作薄（*.xlsx）|*.xlsx";
                        sfd.ShowDialog();

                        FileStream files = new FileStream(sfd.FileName, FileMode.Create);
                        wk.Write(files);
                        files.Close();
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
    }
    public class ViewSheetComparer : IComparer<ViewSheet>
    {
        public int Compare(ViewSheet x, ViewSheet y)
        {
            return (x.Title.CompareTo(y.Title));
        }
    }
}
