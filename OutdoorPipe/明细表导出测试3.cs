using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //1.
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            ViewSchedule schedule = doc.ActiveView as ViewSchedule;

            //2、取得table数据
            TableData tableData = schedule.GetTableData();

            //BodyTextTypeId

            //3、分别取得数据
            //string header = tableData.GetSectionData(SectionType.Header).ToString();取得的为Autodesk.Revit.DB.TableSelctionData
            string header = schedule.GetCellText(SectionType.Header, 0, 0);//要想取得数据使用schedule.GetCellTexk();

            //4、得到body数据部分
            TableSectionData sectionBody = tableData.GetSectionData(SectionType.Body);
            int rs = sectionBody.NumberOfRows;
            int cs = sectionBody.NumberOfColumns;
            //TaskDialog.Show("columns:",cs.ToString());

            //创建workbook
            HSSFWorkbook workBook = new HSSFWorkbook();

            //设置格式
            ICellStyle cellStyle = workBook.CreateCellStyle();
            cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            if (workBook == null)
            {
                message = "workBook is null";
                return Result.Failed;
            }
            //sheet的命名为明细表的表头
            ISheet sheet = workBook.CreateSheet(header);

            //添加表头
            IRow rowH = sheet.CreateRow(0);
            ICell cellH = rowH.CreateCell(0);
            cellH.SetCellValue(header);
            cellH.CellStyle = cellStyle;
            //合并表头
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, cs + 6));

            //标题信息
            CreateTitle(cs, schedule, sheet, cellStyle);

            for (int r = 3; r < rs + 2; r++)
            {
                IRow row = sheet.CreateRow(r);
                for (int c = 0; c < cs; c++)
                {
                    ICell cel = row.CreateCell(c);
                    string cellText = schedule.GetCellText(SectionType.Body, r - 2, c);
                    //如何读取Revit明细表中数据格式
                    cel.SetCellValue(cellText);
                }
            }
            using (FileStream fs = File.Create(@"D:\\" + header + ".xls"))
            {
                workBook.Write(fs);
            }
            return Result.Succeeded;
        }

        public void CreateTitle(int cs, ViewSchedule schedule, ISheet sheet, ICellStyle cellStyle)
        {
            IRow rowTitle = sheet.CreateRow(1);
            for (int c1 = 0; c1 < cs; c1++)
            {
                ICell cellLeft = rowTitle.CreateCell(c1);
                string cellText = schedule.GetCellText(SectionType.Body, 0, c1);
                cellLeft.SetCellValue(cellText);
                cellLeft.CellStyle = cellStyle;
                sheet.AddMergedRegion(new CellRangeAddress(1, 2, c1, c1));
            }
            ICell cellSingle = rowTitle.CreateCell(cs);
            cellSingle.SetCellValue("单价");
            cellSingle.CellStyle = cellStyle;
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, cs, cs + 2));


            ICell cellTotal = rowTitle.CreateCell(cs + 3);
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, cs + 3, cs + 5));
            cellTotal.SetCellValue("合价");
            cellTotal.CellStyle = cellStyle;

            IRow row2 = sheet.CreateRow(2);
            ICell cellCs = row2.CreateCell(cs);
            cellCs.SetCellValue("小计");

            ICell cellCs1 = row2.CreateCell(cs + 1);
            cellCs1.SetCellValue("主材费");

            ICell cellCs2 = row2.CreateCell(cs + 2);
            cellCs2.SetCellValue("安装费");

            ICell cellCs3 = row2.CreateCell(cs + 3);
            cellCs3.SetCellValue("合计");

            ICell cellCs4 = row2.CreateCell(cs + 4);
            cellCs4.SetCellValue("主材费");

            ICell cellCs5 = row2.CreateCell(cs + 5);
            cellCs5.SetCellValue("安装费");

            ICell cellCs6 = rowTitle.CreateCell(cs + 6);
            sheet.AddMergedRegion(new CellRangeAddress(1, 2, cs + 6, cs + 6));
            cellCs6.SetCellValue("备注");
            cellCs6.CellStyle = cellStyle;
        }
    }

}
