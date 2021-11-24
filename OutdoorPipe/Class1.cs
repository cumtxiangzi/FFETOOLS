using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.InteropServices;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class 测试1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            Reference ref1 = sel.PickObject(ObjectType.Element, "点选一根管道");
            Element elm = doc.GetElement(ref1);
            Pipe p = elm as Pipe;
            TaskDialog.Show("ss", GetPipeBottom(p));
            return Result.Succeeded;
        }
        public static string GetPipeBottom(Pipe p)
        {
            // 获取管底相对于地面高度
            Parameter outDiameter = p.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
            string outDiameterSize = outDiameter.AsValueString();
            //string pipeBottom = (int.Parse(height) + int.Parse(outDiameterSize) / 2).ToString();       
            //return pipeBottom;
            return outDiameterSize;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class 明细表导出1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("生活给水管件"))
                {
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
                }
            }

            //ViewSchedule v = doc.ActiveView as ViewSchedule;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class WellPoint2 : IExternalCommand
    {
        private View3D view3D;
        private FamilyInstance well;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;
            Document doc = uidoc.Document;

            ProjectInfo pro = doc.ProjectInformation;
            Parameter proNum = pro.LookupParameter("工程代号");
            string dltName = proNum.AsString() + "W" + "." + "txt";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = dltName;
            sfd.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
            sfd.ShowDialog();
            FileStream files = new FileStream(sfd.FileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(files);

            FilteredElementCollector wellCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_Site);
            IList<Element> wells = wellCollector.ToElements();
            XYZ point = null;
            List<string> wellNumber = new List<string>();

            foreach (Element e in wells)
            {
                FamilyInstance w = e as FamilyInstance;
                if (w.Name.Contains("给排水") && w.Name.Contains("排水检查井"))
                {
                    well = w;
                    string i_type = well.LookupParameter("标记").AsString();
                    wellNumber.Add(i_type);
                }
            }
            wellNumber = wellNumber.OrderBy(s => int.Parse(Regex.Match(s, @"\d+").Value)).ThenBy(x => x.ToUpper()).ToList();

            using (Transaction trans = new Transaction(doc, "导出排水井坐标"))
            {
                trans.Start();
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
                trans.Commit();
            }
            sw.Flush();
            sw.Close();
            files.Close();
            return Result.Succeeded;
        }
    }
}