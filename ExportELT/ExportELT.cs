using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Reflection;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;


namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class ExportELT : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            ProjectInfo pro = doc.ProjectInformation;
            Parameter proNum = pro.LookupParameter("工程代号");
            Parameter proNam = pro.LookupParameter("工程名称");
            Parameter subproNum = pro.LookupParameter("子项代号");
            Parameter subproNam = pro.LookupParameter("子项名称");




            XSSFWorkbook wk;
            FileStream fs = new FileStream("C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\Template-DLT.xlsx", FileMode.Open, FileAccess.Read);
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
            style7.SetFont(font6);
            style7.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            style7.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            style7.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;

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

            XSSFCell cell = (XSSFCell)sheet.GetRow(0).GetCell(3);
            cell.SetCellValue(proNum.AsString() + "-" + proNam.AsString());
            cell = (XSSFCell)sheet.GetRow(1).GetCell(3);
            cell.SetCellValue(subproNum.AsString() + "-" + subproNam.AsString());

            try
            {
                IList<ViewSheet> wviewsheets = new List<ViewSheet>();
                IList<Element> elems = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).ToElements();
                foreach (Element e in elems)
                {
                    ViewSheet vs = e as ViewSheet;
                    if (vs.Title.Contains("WD"))
                    {
                        wviewsheets.Add(vs);

                    }

                }

                IList<Element> titleblocks = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_TitleBlocks).ToElements();
                List<string> sizelist = new List<string>();
                foreach (Element e in titleblocks)
                {
                    FamilyInstance tl = e as FamilyInstance;
                    Element el = doc.GetElement(tl.OwnerViewId);
                    ViewSheet vs1 = el as ViewSheet;
                    if (vs1.SheetNumber.Contains("WD") && tl.Name.Contains("A0"))
                    {
                        sizelist.Add("2");
                    }
                    if (vs1.SheetNumber.Contains("WD") && tl.Name.Contains("A1"))
                    {
                        sizelist.Add("1");
                    }
                    if (vs1.SheetNumber.Contains("WD") && tl.Name.Contains("A2"))
                    {
                        sizelist.Add("0.5");
                    }
                }

                int num = wviewsheets.Count;
                List<string> strlist = new List<string>();
                foreach (ViewSheet vs in wviewsheets)
                {
                    strlist.Add(vs.Name);
                }

                for (int i = 0; i < num; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(i + 4).GetCell(0);
                    cell.SetCellValue(i + 1);
                    cell.CellStyle = style7;
                    cell = (XSSFCell)sheet.GetRow(i + 4).GetCell(1);
                    cell.SetCellValue(proNum.AsString() + "-" + subproNum.AsString() + "-" + "WD" + "-" + (i + 1).ToString().PadLeft(3, '0'));
                    cell.CellStyle = style7;
                    cell = (XSSFCell)sheet.GetRow(i + 4).GetCell(3);
                    cell.SetCellValue(strlist.ElementAt(i));
                    cell.CellStyle = style6;
                    cell = (XSSFCell)sheet.GetRow(i + 4).GetCell(4);
                    cell.SetCellValue(Convert.ToDouble(sizelist.ElementAt(i)));
                    cell.CellStyle = style8;

                }

                cell = (XSSFCell)sheet.GetRow(num + 3).GetCell(1);
                cell.SetCellValue("");
                cell = (XSSFCell)sheet.GetRow(num + 3).GetCell(3);

                IFont font = wk.CreateFont();
                font.FontName = "Arial";
                font.FontHeightInPoints = 16;

                ExportELTWindow eltwin = new ExportELTWindow();
                eltwin.ShowDialog();
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

                cell = (XSSFCell)sheet.GetRow(num + 3).GetCell(4);
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
                cell = (XSSFCell)sheet.GetRow(num + 6).GetCell(3);
                cell.SetCellValue("总计：");
                cell.CellStyle = style;
                cell = (XSSFCell)sheet.GetRow(num + 6).GetCell(4);
                string sum = "sum(E5:" + "E" + (num + 4).ToString() + ")";
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

                string dltName = proNum.AsString() + "-" + subproNum.AsString() + "-" + "WD" + "-" + "DLT" + "." + "xlsx";
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = dltName;
                sfd.Filter = "Excel 工作薄（*.xlsx）|*.xlsx";
                sfd.ShowDialog();

                FileStream files = new FileStream(sfd.FileName, FileMode.Create);
                wk.Write(files);
                files.Close();

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                //throw;
                TaskDialog.Show("警告", e.ToString());
                return Result.Failed;
            }
        }


    }

}
