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


namespace ExportDLT
{
    [Transaction(TransactionMode.Manual)]
    public class ExportDLT : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                ViewSchedule v = doc.ActiveView as ViewSchedule;
                TableData td = v.GetTableData();
                TableSectionData tdb = td.GetSectionData(SectionType.Header);
                string head = v.GetCellText(SectionType.Header, 0, 0);

                TableSectionData tdd = td.GetSectionData(SectionType.Body);

                int c = tdd.NumberOfColumns;
                int r = tdd.NumberOfRows;

                HSSFWorkbook work = new HSSFWorkbook();
                ISheet sheet = work.CreateSheet("mysheet");
                for (int i = 0; i < r; i++)
                {
                    IRow row = sheet.CreateRow(i);
                    for (int j = 0; j < c; j++)
                    {
                        Autodesk.Revit.DB.CellType ctype = tdd.GetCellType(i, j);
                        ICell cell = row.CreateCell(j);
                        string str = v.GetCellText(SectionType.Body, i, j);
                        cell.SetCellValue(str);
                    }
                }
                using (FileStream fs = File.Create("d:\\excel.xls"))
                {
                    work.Write(fs);
                    fs.Close();
                }
                return Result.Succeeded;

            }
            catch (Exception)
            {
                TaskDialog.Show("错误","请在明细表下操作");
                return Result.Failed;
            }

        }
    }
}
