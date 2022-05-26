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
using OfficeOpenXml.Style;
using System.Collections.ObjectModel;

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
                Parameter name = pro.LookupParameter("作者");

                string subproNumOnly = subproNum.AsString();

                string projectName = proNum.AsString() + "-" + proName.AsString();
                string subProjectName = subproNum.AsString() + "-" + subproName.AsString();
                string footerName = proNum.AsString() + "-" + subproNum.AsString() + "-" + "WD-ELT";
                string excelFooterName = "&\"Arial\"" + "&16" + " " + footerName;

                ExportELTWindow mainfrm = new ExportELTWindow();
                mainfrm.ShowDialog();
                ReplaceViewSchedule(doc, name);

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

                            excelWorksheet.Cells[1, 4].Value = projectName;
                            excelWorksheet.Cells[2, 4].Value = subProjectName;
                            excelWorksheet.Cells[3, 4].Value = "施工图";
                            excelWorksheet.Cells[1, 4].Style.Font.Name = "Arial";
                            excelWorksheet.Cells[2, 4].Style.Font.Name = "Arial";
                            excelWorksheet.Cells[3, 4].Style.Font.Name = "Arial";

                            int rowNum = 7;
                            int valveCode = 1;  //设备编号计数
                            List<string> pipeSystemList = GetPipeSystemType(uidoc, "给排水");
                            //string ss = String.Join("\n", pipeSystemList.ToArray());
                            //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            List<PipeValveInfo> valveList = new List<PipeValveInfo>();
                            List<PipeValveInfo> meterList = new List<PipeValveInfo>();
                            List<PipeValveInfo> accessoryWList = new List<PipeValveInfo>();
                            List<PipeValveInfo> accessoryJList = new List<PipeValveInfo>();
                            List<PipeValveInfo> visualGlassList = new List<PipeValveInfo>();
                            List<PipeInfo> pipeList = new List<PipeInfo>();
                            List<FamilyInstance> outDoorHydrantsUp = new List<FamilyInstance>();

                            foreach (string item in pipeSystemList)
                            {
                                if (mainfrm.OnlyPipe.IsChecked == false)
                                {
                                    rowNum++;
                                    int[,] mergeRowIndexs = { { rowNum - 2, 1, 11 }, { rowNum - 2, 1, 11 } };  //合并单元格
                                    ExcelHelper.MergeRowCells(excelWorksheet, 1, mergeRowIndexs);
                                    excelWorksheet.Cells[rowNum - 1, 1].Value = item;
                                    excelWorksheet.Cells[rowNum - 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    excelWorksheet.Cells[rowNum - 1, 1].Style.Font.Bold = true;

                                    //立式管道泵写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_水泵_立式管道泵");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "PU" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "立式管道泵";
                                        excelWorksheet.Cells[rowNum, 6].Value = "1";
                                        excelWorksheet.Cells[rowNum, 7].Value = "23";
                                        excelWorksheet.Cells[rowNum, 9].Value = "配套对应法兰，螺栓，螺母，垫片";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 1, 9].Value = "一用一备";
                                        excelWorksheet.Cells[rowNum + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "ADL1x7";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "流量:" + "1.0m³/h";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "扬程:" + "39m";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "转速:" + "2900r/min";
                                        rowNum++;

                                        excelWorksheet.Cells[rowNum, 3].Value = "MT01";
                                        excelWorksheet.Cells[rowNum, 4].Value = "电机";
                                        excelWorksheet.Cells[rowNum, 6].Value = "1";
                                        excelWorksheet.Cells[rowNum, 8].Value = "0.37";
                                        excelWorksheet.Cells[rowNum, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "功率:" + "0.37kW";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "电压:" + "380V";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "是否变频:" + "非变频";
                                        rowNum++;
                                        valveCode++;
                                    }

                                    //固定式潜水泵写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_水泵_潜水泵(固定式安装)");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "PU" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "潜水泵";
                                        excelWorksheet.Cells[rowNum, 6].Value = "1";
                                        excelWorksheet.Cells[rowNum, 7].Value = "600";
                                        excelWorksheet.Cells[rowNum, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        excelWorksheet.Cells[rowNum, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        excelWorksheet.Cells[rowNum, 9].Value = "一用一备";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 1, 9].Value = "自动耦合安装";
                                        excelWorksheet.Cells[rowNum + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 2, 9].Value = "导轨及导链采用不锈钢";
                                        excelWorksheet.Cells[rowNum + 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "200QW250-15-18.5";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "流量:" + "150m³/h";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "扬程:" + "20m";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "转速:" + "1450r/min";
                                        rowNum++;

                                        excelWorksheet.Cells[rowNum, 3].Value = "MT01";
                                        excelWorksheet.Cells[rowNum, 4].Value = "电机";
                                        excelWorksheet.Cells[rowNum, 6].Value = "1";
                                        excelWorksheet.Cells[rowNum, 8].Value = "18.5";
                                        excelWorksheet.Cells[rowNum, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "功率:" + "18.5kW";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "电压:" + "380V";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "是否变频:" + "非变频";
                                        rowNum++;
                                        valveCode++;
                                    }

                                    //移动式潜水泵写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_水泵_潜水泵(移动式安装)");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "PU" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "潜水泵";
                                        excelWorksheet.Cells[rowNum, 6].Value = "1";
                                        excelWorksheet.Cells[rowNum, 7].Value = "35";
                                        excelWorksheet.Cells[rowNum, 9].Value = "配套附件及安装详见08S305-17";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 1, 9].Value = "配电控箱，接收信号；自动启停";
                                        excelWorksheet.Cells[rowNum + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 2, 9].Value = "高水位工作，低水位停泵。";
                                        excelWorksheet.Cells[rowNum + 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "50QW-10-15-1.5";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "流量:" + "10m³/h";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "扬程:" + "15m";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "转速:" + "2900r/min";
                                        rowNum++;

                                        excelWorksheet.Cells[rowNum, 3].Value = "MT01";
                                        excelWorksheet.Cells[rowNum, 4].Value = "电机";
                                        excelWorksheet.Cells[rowNum, 6].Value = "1";
                                        excelWorksheet.Cells[rowNum, 8].Value = "1.5";
                                        excelWorksheet.Cells[rowNum, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "功率:" + "1.5kW";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "电压:" + "380V";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "是否变频:" + "非变频";
                                        rowNum++;
                                        valveCode++;
                                    }

                                    if (item.Contains("热水"))
                                    {
                                        //壁挂式电热水器写入
                                        outDoorHydrantsUp = GetEquipmentsNoCon(doc, "给排水_加热设备_挂壁式电热水器");
                                        foreach (var hydrant in outDoorHydrantsUp)
                                        {
                                            excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                            excelWorksheet.Cells[rowNum, 2].Value = "HE" + valveCode.ToString().PadLeft(2, '0');
                                            if (outDoorHydrantsUp.Count > 1)
                                            {
                                                excelWorksheet.Cells[rowNum + 1, 2].Value = "~" + (valveCode + outDoorHydrantsUp.Count - 1).ToString().PadLeft(2, '0');
                                            }
                                            excelWorksheet.Cells[rowNum, 4].Value = "壁挂式电热水器";
                                            excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                            excelWorksheet.Cells[rowNum, 8].Value = (3 * outDoorHydrantsUp.Count).ToString() + ".0";
                                            excelWorksheet.Cells[rowNum, 9].Value = "";
                                            excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "型号:";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "储水容积:" + "60L";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "功率:" + "3.0kW";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "电压:" + "220V";
                                            rowNum++;
                                            for (int i = 0; i < outDoorHydrantsUp.Count; i++)
                                            {
                                                valveCode++;
                                            }
                                            break;
                                        }

                                        //DVE商用电热水器写入
                                        outDoorHydrantsUp = GetEquipmentsNoCon(doc, "给排水_加热设备_DVE商用电热水炉");
                                        foreach (var hydrant in outDoorHydrantsUp)
                                        {
                                            excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                            excelWorksheet.Cells[rowNum, 2].Value = "HE" + valveCode.ToString().PadLeft(2, '0');
                                            if (outDoorHydrantsUp.Count > 1)
                                            {
                                                excelWorksheet.Cells[rowNum + 1, 2].Value = "~" + (valveCode + outDoorHydrantsUp.Count - 1).ToString().PadLeft(2, '0');
                                            }
                                            excelWorksheet.Cells[rowNum, 4].Value = "商用电热水器";
                                            excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                            excelWorksheet.Cells[rowNum, 8].Value = (90 * outDoorHydrantsUp.Count).ToString() + ".0";
                                            excelWorksheet.Cells[rowNum, 9].Value = "";
                                            excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "型号:"+ "DVE";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "储水容积:" + "200L";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "功率:" + "90.0kW";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "电压:" + "380V";
                                            rowNum++;
                                            for (int i = 0; i < outDoorHydrantsUp.Count; i++)
                                            {
                                                valveCode++;
                                            }
                                            break;
                                        }

                                        //DSE商用电热水器写入
                                        outDoorHydrantsUp = GetEquipmentsNoCon(doc, "给排水_加热设备_DSE商用电热水炉");
                                        foreach (var hydrant in outDoorHydrantsUp)
                                        {
                                            excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                            excelWorksheet.Cells[rowNum, 2].Value = "HE" + valveCode.ToString().PadLeft(2, '0');
                                            if (outDoorHydrantsUp.Count > 1)
                                            {
                                                excelWorksheet.Cells[rowNum + 1, 2].Value = "~" + (valveCode + outDoorHydrantsUp.Count - 1).ToString().PadLeft(2, '0');
                                            }
                                            excelWorksheet.Cells[rowNum, 4].Value = "商用电热水器";
                                            excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                            excelWorksheet.Cells[rowNum, 8].Value = (90 * outDoorHydrantsUp.Count).ToString() + ".0";
                                            excelWorksheet.Cells[rowNum, 9].Value = "";
                                            excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "DSE";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "储水容积:" + "200L";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "功率:" + "90.0kW";
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "电压:" + "380V";
                                            rowNum++;
                                            for (int i = 0; i < outDoorHydrantsUp.Count; i++)
                                            {
                                                valveCode++;
                                            }
                                            break;
                                        }

                                    }

                                    //电磁流量计写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_仪表_电磁流量计");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "LF" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "电磁流量计";
                                        excelWorksheet.Cells[rowNum, 6].Value = "1";
                                        excelWorksheet.Cells[rowNum, 9].Value = "配对应法兰、螺栓、螺母、垫片等";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 1, 9].Value = "带就地显示并远传";
                                        excelWorksheet.Cells[rowNum + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "LD-" + hydrant.LookupParameter("公称直径DN").AsValueString();
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + "DN" + hydrant.LookupParameter("公称直径DN").AsValueString();
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + "1.0MPa";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "测量范围:" + GetMeasuringRange(hydrant.LookupParameter("尺寸").AsString()) + "m³/h";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "电压:" + "220V";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "输出信号:" + "DC4~20mA";
                                        rowNum++;
                                        valveCode++;
                                    }

                                    //螺翼式与螺翼式水表写入
                                    meterList = GetPipeSystemMeter(doc, item, subproNum.AsString(), valveCode);
                                    foreach (PipeValveInfo valveInfo in meterList)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = valveInfo.ProjectNum;
                                        excelWorksheet.Cells[rowNum, 2].Value = valveInfo.ValveAbb;
                                        excelWorksheet.Cells[rowNum, 4].Value = valveInfo.ValveName;
                                        excelWorksheet.Cells[rowNum, 6].Value = valveInfo.ValveQulity;
                                        excelWorksheet.Cells[rowNum, 9].Value = valveInfo.ValveNote;
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + valveInfo.ValveModel;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + valveInfo.ValvePressure;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + valveInfo.ValveSize;
                                        rowNum++;
                                        valveCode++;
                                    }

                                    //地上式消火栓写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_消防设备_室外地上式消火栓");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "FZ" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "室外地上式消火栓";
                                        excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                        excelWorksheet.Cells[rowNum, 9].Value = "配对应法兰、螺栓、螺母、垫片等";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "SSF100/65-1.6";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "进水口:" + "DN100";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "出水口:" + "DN100, DN65";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + "1.6MPa";
                                        rowNum++;
                                        valveCode++;
                                        break;
                                    }

                                    //室内消火栓写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_消防设备_室内消火栓箱");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "FZ" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "室内消火栓";
                                        excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();

                                        excelWorksheet.Cells[rowNum, 9].Value = "配乙型单栓室内消火栓箱×" + (outDoorHydrantsUp.Count - 1).ToString();
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 1, 9].Value = "箱内配置消防按钮";
                                        excelWorksheet.Cells[rowNum + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 2, 9].Value = "详见15S202-9页";
                                        excelWorksheet.Cells[rowNum + 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 3, 9].Value = "配屋顶试验用消火栓箱×1";
                                        excelWorksheet.Cells[rowNum + 3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 4, 9].Value = "箱内配置压力表";
                                        excelWorksheet.Cells[rowNum + 4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 5, 9].Value = "详见15S202-54页";
                                        excelWorksheet.Cells[rowNum + 5, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "SN65";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + "DN65";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + "1.0MPa";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "接口型式:" + "内口式";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "水枪型号:" + "QZ19";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "水龙带:" + "DN65,25m";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "水龙带接口:" + "KD65型2个";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "消火栓箱:" + "800x650x240mm";
                                        rowNum++;
                                        valveCode++;
                                        break;
                                    }

                                    //地下式水泵接合器写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_消防设备_地下式水泵接合器");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "FZ" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "地下式消防水泵接合器";
                                        excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                        excelWorksheet.Cells[rowNum, 9].Value = "乙型配套止回阀、安全阀、闸阀等";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 1, 9].Value = "配套法兰、垫片、螺栓及螺母";
                                        excelWorksheet.Cells[rowNum + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 2, 9].Value = "详见99S203-17页";
                                        excelWorksheet.Cells[rowNum + 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        rowNum++;
                                        if (hydrant.Name.Contains("DN100"))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "SQX100-A";
                                        }
                                        else if (hydrant.Name.Contains("DN150"))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "SQX150-A";
                                        }
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + hydrant.Name;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + "1.0MPa";
                                        rowNum++;
                                        valveCode++;
                                        break;
                                    }

                                    //墙壁式水泵接合器写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_消防设备_墙壁式水泵接合器");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "FZ" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "墙壁式消防水泵接合器";
                                        excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                        excelWorksheet.Cells[rowNum, 9].Value = "乙型配套止回阀、安全阀、闸阀等";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 1, 9].Value = "配套法兰、垫片、螺栓及螺母";
                                        excelWorksheet.Cells[rowNum + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        excelWorksheet.Cells[rowNum + 2, 9].Value = "详见99S203-5页";
                                        excelWorksheet.Cells[rowNum + 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "SQX" + hydrant.Symbol.LookupParameter("出口公称直径").AsValueString() + "-A";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + "DN" + hydrant.Symbol.LookupParameter("出口公称直径").AsValueString();
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + "1.0MPa";
                                        rowNum++;
                                        valveCode++;
                                        break;
                                    }

                                    //压力表写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_仪表_压力表");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "VG" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "压力表";
                                        excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                        excelWorksheet.Cells[rowNum, 9].Value = "带旋塞阀和表弯";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "Y-100";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "测量范围:" + "0~2.5Mpa";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "测量精度:" + "2.5";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "接头螺纹:" + "M20x1.5";
                                        rowNum++;
                                        valveCode++;
                                        break;
                                    }

                                    //真空表写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_仪表_真空表");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "VG" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "真空表";
                                        excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                        excelWorksheet.Cells[rowNum, 9].Value = "带旋塞阀和表弯";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "YZ-100";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "测量范围:" + "-0.1~0.06MPa";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "测量精度:" + "1.5";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "接头螺纹:" + "M20x1.5";
                                        rowNum++;
                                        valveCode++;
                                        break;
                                    }

                                    //温度计写入
                                    outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "给排水_仪表_温度计");
                                    foreach (var hydrant in outDoorHydrantsUp)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                        excelWorksheet.Cells[rowNum, 2].Value = "XT" + valveCode.ToString().PadLeft(2, '0');
                                        excelWorksheet.Cells[rowNum, 4].Value = "工业内标式有机液体温度计";
                                        excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                        excelWorksheet.Cells[rowNum, 9].Value = "带连接短管";
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "WNY-11 直型(带有金属保护管)";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "测量范围:" + "0~50℃";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "上体长度:" + "150mm";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "下体长度:" + "100mm";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "分度值:" + "0.2℃";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + "1.0MPa";
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "直型接头:" + "M27x2,H=60mm";
                                        rowNum++;
                                        valveCode++;
                                        break;
                                    }

                                    //水流视镜写入
                                    visualGlassList = GetPipeSystemVisualGlass(doc, item, subproNum.AsString(), valveCode);
                                    foreach (PipeValveInfo valveInfo in visualGlassList)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = valveInfo.ProjectNum;
                                        excelWorksheet.Cells[rowNum, 2].Value = valveInfo.ValveAbb;
                                        excelWorksheet.Cells[rowNum, 4].Value = valveInfo.ValveName;
                                        excelWorksheet.Cells[rowNum, 6].Value = valveInfo.ValveQulity;
                                        excelWorksheet.Cells[rowNum, 9].Value = valveInfo.ValveNote;
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + valveInfo.ValvePressure;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + valveInfo.ValveSize;
                                        rowNum++;
                                        valveCode++;
                                    }

                                    //阀门写入
                                    valveList = GetPipeSystemValve(doc, item, subproNum.AsString(), valveCode);
                                    foreach (PipeValveInfo valveInfo in valveList)
                                    {
                                        if (valveInfo.ValveName.Contains("电动"))
                                        {
                                            for (int i = 0; i < int.Parse(valveInfo.ValveQulity); i++)
                                            {
                                                excelWorksheet.Cells[rowNum, 1].Value = valveInfo.ProjectNum;
                                                excelWorksheet.Cells[rowNum, 2].Value = "VA" + valveCode.ToString().PadLeft(2, '0');
                                                excelWorksheet.Cells[rowNum, 4].Value = valveInfo.ValveName;
                                                excelWorksheet.Cells[rowNum, 6].Value = "1";

                                                if (valveInfo.ValveModel.Contains("Z45") || valveInfo.ValveName.Contains("蝶阀") || valveInfo.ValveName.Contains("止回阀")
                                                    || valveInfo.ValveModel.Contains("Z945") || valveInfo.ValveName.Contains("液压水位控制阀") || valveInfo.ValveName.Contains("泄压阀"))
                                                {
                                                    excelWorksheet.Cells[rowNum, 9].Value = "配置对应法兰、螺栓、螺母、垫片";
                                                }
                                                else
                                                {
                                                    excelWorksheet.Cells[rowNum, 9].Value = "";
                                                }

                                                excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "型号:" + valveInfo.ValveModel;
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + valveInfo.ValvePressure;
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + valveInfo.ValveSize;
                                                rowNum++;

                                                excelWorksheet.Cells[rowNum, 3].Value = "MT01";
                                                excelWorksheet.Cells[rowNum, 4].Value = "电动执行机构";
                                                excelWorksheet.Cells[rowNum, 6].Value = "1";
                                                excelWorksheet.Cells[rowNum, 8].Value = "0.18";
                                                excelWorksheet.Cells[rowNum, 9].Value = "普通开关型";
                                                excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "型号:";
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "最大输出转矩:" + "600N.m";
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "90°旋转时间:" + "15s";
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "输入输出信号:" + "开关量信号";
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "电机功率:" + "0.18kW";
                                                rowNum++;
                                                excelWorksheet.Cells[rowNum, 4].Value = "电压:" + "220V";
                                                rowNum++;
                                                valveCode++;
                                            }
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 1].Value = valveInfo.ProjectNum;
                                            excelWorksheet.Cells[rowNum, 2].Value = valveInfo.ValveAbb;
                                            excelWorksheet.Cells[rowNum, 4].Value = valveInfo.ValveName;
                                            excelWorksheet.Cells[rowNum, 6].Value = valveInfo.ValveQulity;

                                            if (valveInfo.ValveModel.Contains("Z45") || valveInfo.ValveName.Contains("蝶阀") || valveInfo.ValveName.Contains("止回阀")
                                                || valveInfo.ValveModel.Contains("Z945") || valveInfo.ValveName.Contains("液压水位控制阀") || valveInfo.ValveName.Contains("泄压阀"))
                                            {
                                                excelWorksheet.Cells[rowNum, 9].Value = "配置对应法兰、螺栓、螺母、垫片";
                                            }
                                            else
                                            {
                                                excelWorksheet.Cells[rowNum, 9].Value = "";
                                            }

                                            excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "型号:" + valveInfo.ValveModel;
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + valveInfo.ValvePressure;
                                            rowNum++;
                                            excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + valveInfo.ValveSize;
                                            rowNum++;
                                            valveCode++;
                                        }

                                    }

                                    //Y型过滤器与挠性橡胶接头写入
                                    accessoryJList = GetPipeSystemAccessoryJ(doc, item, subproNum.AsString(), valveCode);
                                    foreach (PipeValveInfo valveInfo in accessoryJList)
                                    {
                                        excelWorksheet.Cells[rowNum, 1].Value = valveInfo.ProjectNum;
                                        excelWorksheet.Cells[rowNum, 2].Value = valveInfo.ValveAbb;
                                        excelWorksheet.Cells[rowNum, 4].Value = valveInfo.ValveName;
                                        excelWorksheet.Cells[rowNum, 6].Value = valveInfo.ValveQulity;
                                        excelWorksheet.Cells[rowNum, 9].Value = valveInfo.ValveNote;
                                        excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + valveInfo.ValvePressure;
                                        rowNum++;
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + valveInfo.ValveSize;
                                        rowNum++;
                                        valveCode++;
                                    }

                                    if (item.Contains("生活给水"))
                                    {
                                        //坐便器写入
                                        outDoorHydrantsUp = GetEquipmentsNoCon(doc, "坐便器");
                                        foreach (var hydrant in outDoorHydrantsUp)
                                        {
                                            excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                            excelWorksheet.Cells[rowNum, 2].Value = "TA" + valveCode.ToString().PadLeft(2, '0');
                                            excelWorksheet.Cells[rowNum, 4].Value = "坐式大便器";
                                            excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                            excelWorksheet.Cells[rowNum, 9].Value = "附给排水配件,见09S304-68";
                                            excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            rowNum++;                                                                                  
                                            valveCode++;
                                            break;
                                        }
                                    }                               

                                }

                                //管道写入
                                pipeList = GetPipeSystemPipe(doc, item, subproNum.AsString(), 1);
                                foreach (PipeInfo pipeInfo in pipeList)
                                {
                                    excelWorksheet.Cells[rowNum, 1].Value = pipeInfo.ProjectNum;
                                    excelWorksheet.Cells[rowNum, 2].Value = "PP" + valveCode.ToString().PadLeft(2, '0');
                                    excelWorksheet.Cells[rowNum, 4].Value = pipeInfo.PipeSystem + "管道及管件";
                                    rowNum++;
                                    valveCode++;
                                    break;
                                }
                                foreach (PipeInfo pipeInfo in pipeList)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeInfo.PipeAbb;
                                    if (pipeInfo.PipeName.Contains("镀锌"))
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "双面热浸镀锌钢管";
                                    }
                                    else if (pipeInfo.PipeName.Contains("HDPE"))
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "HDPE双壁波纹管";
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = pipeInfo.PipeName;
                                    }
                                    rowNum++;
                                    break;
                                }
                                foreach (PipeInfo pipeInfo in pipeList)
                                {
                                    if (pipeInfo.PipeName.Contains("UPVC"))
                                    {

                                    }
                                    else if (pipeInfo.PipeName.Contains("HDPE"))
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "环刚度:≥8kN/m²";
                                        rowNum++;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeInfo.PipePressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeInfo pipeInfo in pipeList)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + pipeInfo.PipeSize;
                                    if (pipeInfo.PipeQulity == "0 m")
                                    {
                                        if (pipeList.Count == 1)
                                        {
                                            if (pipeInfo.PipeName.Contains("UPVC"))
                                            {
                                                excelWorksheet.Cells[rowNum - 1, 6].Value = "1 m";
                                            }
                                            else
                                            {
                                                excelWorksheet.Cells[rowNum - 2, 6].Value = "1 m";
                                            }
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 6].Value = "1 m";
                                        }
                                    }
                                    else
                                    {
                                        if (pipeList.Count == 1)
                                        {
                                            if (pipeInfo.PipeName.Contains("UPVC"))
                                            {
                                                excelWorksheet.Cells[rowNum - 1, 6].Value = pipeInfo.PipeQulity;
                                            }
                                            else
                                            {
                                                excelWorksheet.Cells[rowNum - 2, 6].Value = pipeInfo.PipeQulity;
                                            }
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 6].Value = pipeInfo.PipeQulity;
                                        }
                                    }
                                    rowNum++;
                                }

                                //90°弯头写入
                                int elbowNum = 2;
                                List<PipeElbowInfo> pipeElbowList90 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "90");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList90)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("镀锌"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽式90°弯头";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "90°镀锌弯头";
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = pipeElbowInfo.PipeElbowName;
                                    }
                                    elbowNum++;
                                    rowNum++;
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList90)
                                {
                                    if (pipeElbowInfo.PipeElbowName.Contains("UPVC"))
                                    {

                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList90)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + pipeElbowInfo.PipeElbowSize;

                                    if (pipeElbowList90.Count == 1)
                                    {
                                        if (pipeElbowInfo.PipeElbowName.Contains("UPVC"))
                                        {
                                            excelWorksheet.Cells[rowNum - 1, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum - 2, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    }
                                    rowNum++;
                                }

                                //60°弯头写入
                                List<PipeElbowInfo> pipeElbowList60 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "60");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList60)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("镀锌"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽式60°弯头";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "60°镀锌弯头";
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = pipeElbowInfo.PipeElbowName;
                                    }
                                    elbowNum++;
                                    rowNum++;
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList60)
                                {
                                    if (pipeElbowInfo.PipeElbowName.Contains("UPVC"))
                                    {

                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList60)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + pipeElbowInfo.PipeElbowSize;
                                    if (pipeElbowList60.Count == 1)
                                    {
                                        excelWorksheet.Cells[rowNum - 2, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    }
                                    rowNum++;
                                }

                                //45°弯头写入
                                List<PipeElbowInfo> pipeElbowList45 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "45");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList45)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("镀锌"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽式45°弯头";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "45°镀锌弯头";
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = pipeElbowInfo.PipeElbowName;
                                    }
                                    elbowNum++;
                                    rowNum++;
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList45)
                                {

                                    if (pipeElbowInfo.PipeElbowName.Contains("UPVC"))
                                    {

                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList45)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + pipeElbowInfo.PipeElbowSize;
                                    if (pipeElbowList45.Count == 1)
                                    {
                                        if (pipeElbowInfo.PipeElbowName.Contains("UPVC"))
                                        {
                                            excelWorksheet.Cells[rowNum - 1, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum - 2, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    }
                                    rowNum++;
                                }

                                //30°弯头写入
                                List<PipeElbowInfo> pipeElbowList30 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "30");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList30)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("镀锌"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽式30°弯头";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "30°镀锌弯头";
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = pipeElbowInfo.PipeElbowName;
                                    }
                                    elbowNum++;
                                    rowNum++;
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList30)
                                {

                                    if (pipeElbowInfo.PipeElbowName.Contains("UPVC"))
                                    {

                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList30)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + pipeElbowInfo.PipeElbowSize;
                                    if (pipeElbowList30.Count == 1)
                                    {
                                        excelWorksheet.Cells[rowNum - 2, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    }
                                    rowNum++;
                                }

                                //22.5°弯头写入
                                List<PipeElbowInfo> pipeElbowList225 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "22");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList225)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("镀锌"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽式22.5°弯头";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "22.5°镀锌弯头";
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = pipeElbowInfo.PipeElbowName;
                                    }
                                    elbowNum++;
                                    rowNum++;
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList225)
                                {

                                    if (pipeElbowInfo.PipeElbowName.Contains("UPVC"))
                                    {

                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }

                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList225)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + pipeElbowInfo.PipeElbowSize;

                                    if (pipeElbowList225.Count == 1)
                                    {
                                        if (pipeElbowInfo.PipeElbowName.Contains("UPVC"))
                                        {
                                            excelWorksheet.Cells[rowNum - 1, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum - 2, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    }

                                    rowNum++;
                                }

                                //三通写入
                                List<PipeTeeInfo> pipeTeeList = GetPipeSystemPipeTee(doc, item, subproNum.AsString(), elbowNum);
                                foreach (PipeTeeInfo pipeTeeInfo in pipeTeeList)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeTeeInfo.PipeTeeAbb;
                                    if (pipeTeeInfo.PipeTeeName.Contains("镀锌"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽式三通";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "镀锌三通";
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = pipeTeeInfo.PipeTeeName;
                                    }
                                    elbowNum++;
                                    rowNum++;
                                    break;
                                }
                                foreach (PipeTeeInfo pipeTeeInfo in pipeTeeList)
                                {
                                    if (pipeTeeInfo.PipeTeeName.Contains("UPVC"))
                                    {

                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeTeeInfo.PipeTeePressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeTeeInfo pipeTeeInfo in pipeTeeList)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "规格:" + pipeTeeInfo.PipeTeeSize;

                                    if (pipeTeeList.Count == 1)
                                    {
                                        if (pipeTeeInfo.PipeTeeName.Contains("UPVC"))
                                        {
                                            excelWorksheet.Cells[rowNum - 1, 6].Value = pipeTeeInfo.PipeTeeQulity;
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum - 2, 6].Value = pipeTeeInfo.PipeTeeQulity;
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = pipeTeeInfo.PipeTeeQulity;
                                    }
                                    rowNum++;
                                }

                                //异径写入
                                List<PipeReduceInfo> pipeReduceList = GetPipeSystemPipeReduce(doc, item, subproNum.AsString(), elbowNum);
                                foreach (PipeReduceInfo pipeReduceInfo in pipeReduceList)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeReduceInfo.PipeReduceAbb;
                                    if (pipeReduceInfo.PipeReduceName.Contains("镀锌"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽式异径";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "镀锌异径";
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = pipeReduceInfo.PipeReduceName;
                                    }
                                    elbowNum++;
                                    rowNum++;
                                    break;
                                }
                                foreach (PipeReduceInfo pipeReduceInfo in pipeReduceList)
                                {
                                    if (pipeReduceInfo.PipeReduceName.Contains("UPVC"))
                                    {

                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeReduceInfo.PipeReducePressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeReduceInfo pipeReduceInfo in pipeReduceList)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "规格:" + pipeReduceInfo.PipeReduceSize;

                                    if (pipeReduceList.Count == 1)
                                    {
                                        if (pipeReduceInfo.PipeReduceName.Contains("UPVC"))
                                        {
                                            excelWorksheet.Cells[rowNum - 1, 6].Value = pipeReduceInfo.PipeReduceQulity;
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum - 2, 6].Value = pipeReduceInfo.PipeReduceQulity;
                                        }
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = pipeReduceInfo.PipeReduceQulity;
                                    }
                                    rowNum++;
                                }

                                //排水设备附件写入
                                accessoryWList = GetPipeSystemAccessoryW(doc, item, subproNum.AsString(), elbowNum);
                                foreach (PipeValveInfo valveInfo in accessoryWList)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = valveInfo.ValveAbb;
                                    excelWorksheet.Cells[rowNum, 4].Value = valveInfo.ValveName;
                                    excelWorksheet.Cells[rowNum, 6].Value = valveInfo.ValveQulity;
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + valveInfo.ValveSize;
                                    rowNum++;
                                    elbowNum++;
                                }

                                //法兰写入只针对沟槽式消防连接
                                if (item.Contains("消防") && !(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = "OX" + elbowNum.ToString().PadLeft(2, '0');
                                    excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽式法兰";
                                    elbowNum++;
                                    rowNum++;

                                    foreach (PipeValveInfo valveInfo in valveList)
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + valveInfo.ValvePressure;
                                        rowNum++;
                                        break;
                                    }

                                    foreach (PipeValveInfo valveInfo in valveList)
                                    {
                                        if (!(valveInfo.ValveSize == "DN20") && !(valveInfo.ValveSize == "DN25") && !(valveInfo.ValveSize == "DN32") &&
                                            !(valveInfo.ValveSize == "DN40") && !(valveInfo.ValveSize == "DN50") && !(valveInfo.ValveSize == "DN15"))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + valveInfo.ValveSize;
                                            if (valveList.Count == 2)
                                            {
                                                excelWorksheet.Cells[rowNum - 2, 6].Value = (int.Parse(valveInfo.ValveQulity) * 2).ToString();
                                            }
                                            else
                                            {
                                                excelWorksheet.Cells[rowNum, 6].Value = (int.Parse(valveInfo.ValveQulity) * 2).ToString();
                                            }
                                            rowNum++;
                                        }
                                    }
                                }

                                //卡箍写入只针对沟槽式消防连接
                                if (item.Contains("消防") && !(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = "OX" + elbowNum.ToString().PadLeft(2, '0');
                                    excelWorksheet.Cells[rowNum, 4].Value = "消防沟槽卡箍";
                                    elbowNum++;
                                    rowNum++;

                                    foreach (PipeInfo pipeInfo in pipeList)
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "公称压力:" + pipeInfo.PipePressure;
                                        rowNum++;
                                        break;
                                    }

                                    foreach (PipeInfo pipeInfo in pipeList)
                                    {
                                        int totalNum = 0;
                                        totalNum += int.Parse(pipeInfo.PipeQulity.Replace(" m", "")) / 6;

                                        foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList90)
                                        {
                                            if (pipeElbowInfo.PipeElbowSize.Contains(pipeInfo.PipeSize))
                                            {
                                                totalNum += int.Parse(pipeElbowInfo.PipeElbowQulity) * 2;
                                            }
                                        }

                                        foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList45)
                                        {
                                            if (pipeElbowInfo.PipeElbowSize.Contains(pipeInfo.PipeSize))
                                            {
                                                totalNum += int.Parse(pipeElbowInfo.PipeElbowQulity) * 2;
                                            }
                                        }

                                        foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList225)
                                        {
                                            if (pipeElbowInfo.PipeElbowSize.Contains(pipeInfo.PipeSize))
                                            {
                                                totalNum += int.Parse(pipeElbowInfo.PipeElbowQulity) * 2;
                                            }
                                        }

                                        foreach (PipeTeeInfo pipeTeeInfo in pipeTeeList)
                                        {
                                            if (pipeTeeInfo.PipeTeeSize.Contains(pipeInfo.PipeSize))
                                            {
                                                totalNum += int.Parse(pipeTeeInfo.PipeTeeQulity) * 2;
                                            }

                                            if (pipeTeeInfo.PipeTeeSize.Replace(pipeInfo.PipeSize, "").Replace("X", "").Contains(pipeInfo.PipeSize.Replace("DN", "")))
                                            {
                                                totalNum += int.Parse(pipeTeeInfo.PipeTeeQulity);
                                            }
                                        }

                                        foreach (PipeReduceInfo pipeReduceInfo in pipeReduceList)
                                        {
                                            if (pipeReduceInfo.PipeReduceSize.Contains(pipeInfo.PipeSize))
                                            {
                                                totalNum += int.Parse(pipeReduceInfo.PipeReduceQulity);
                                            }

                                            if (pipeReduceInfo.PipeReduceSize.Replace(pipeInfo.PipeSize, "").Replace("X", "").Contains(pipeInfo.PipeSize.Replace("DN", "")))
                                            {
                                                totalNum += int.Parse(pipeReduceInfo.PipeReduceQulity);
                                            }
                                        }

                                        if (!(pipeInfo.PipeSize == "DN20") && !(pipeInfo.PipeSize == "DN25") && !(pipeInfo.PipeSize == "DN32") &&
                                           !(pipeInfo.PipeSize == "DN40") && !(pipeInfo.PipeSize == "DN50") && !(pipeInfo.PipeSize == "DN15"))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "公称直径:" + pipeInfo.PipeSize;
                                            excelWorksheet.Cells[rowNum, 6].Value = totalNum.ToString();
                                            rowNum++;
                                        }
                                    }
                                }

                            }

                            if (mainfrm.OnlyPipe.IsChecked == false)
                            {
                                //手提干粉灭火器写入
                                outDoorHydrantsUp = GetEquipmentsNoCon(doc, "给排水_消防设备_手提干粉灭火器");
                                foreach (var item in outDoorHydrantsUp)
                                {
                                    excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                    excelWorksheet.Cells[rowNum, 2].Value = "FN" + valveCode.ToString().PadLeft(2, '0');
                                    excelWorksheet.Cells[rowNum, 4].Value = "手提式灭火器";
                                    excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                    excelWorksheet.Cells[rowNum, 9].Value = "灭火剂为磷酸铵盐";
                                    excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "型号:" + "MF/ABC4";
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "灭火剂量:" + "4Kg";
                                    rowNum++;

                                    if (subproName.AsString().Contains("电力室"))
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "灭火级别:" + "55B";
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "灭火级别:" + "2A";
                                    }
                                    rowNum++;
                                    valveCode++;
                                    break;
                                }
                            }

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
                                System.Windows.Forms.MessageBox.Show("设备表导出成功!", "GPSBIM", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    System.Windows.Forms.MessageBox.Show("请在平面或三维视图中进行操作!", "GPSBIM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Failed;
            }
        }
        public void ReplaceViewSchedule(Document doc, Parameter name)
        {
            //Transaction trans = new Transaction(doc, "传递项目标准实例");
            //trans.Start();
            //实例化复制粘贴选项,这里实例化就行
            //CopyPasteOptions option = new CopyPasteOptions();
            //由于材料信息与位移无关,所以位移为null,如果是族实例或者其他与位置有关的,这个地方就需要思考下设置了
            //ElementTransformUtils.CopyElements(document, copyIds, doc, null, option);
            //trans.Commit();
            //保存关闭修改后的文档
            // doc.Save();
            // doc.Close(false);
            // 在后台打开文件，UI上不会显示，并且把文件中墙的数量显示出来。
            if (name.AsString() != "1")
            {
                using (Transaction trans = new Transaction(doc, "明细表替换"))
                {
                    trans.Start();
                    name.Set("1");

                    FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
                    viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
                    IList<Element> viewScheduleList = viewCollector.ToElements();
                    ICollection<ElementId> deletIds = new Collection<ElementId>();
                    foreach (ViewSchedule v in viewScheduleList)
                    {
                        if (v.Name.Contains("给排水"))
                        {
                            deletIds.Add(v.Id);
                        }
                    }
                    doc.Delete(deletIds);

                    string filepath = @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\给排水项目标准.rvt";
                    Document newdoc = doc.Application.OpenDocumentFile(filepath);
                    FilteredElementCollector fec = new FilteredElementCollector(newdoc);
                    fec.OfCategory(BuiltInCategory.OST_Schedules);
                    IList<Element> fecList = fec.ToElements();
                    ICollection<ElementId> copyIds = new Collection<ElementId>();
                    foreach (ViewSchedule v in fecList)
                    {
                        if (v.Name.Contains("给排水"))
                        {
                            copyIds.Add(v.Id);
                        }
                    }
                    CopyPasteOptions option = new CopyPasteOptions();
                    ElementTransformUtils.CopyElements(newdoc, copyIds, doc, null, option);
                    //newdoc.Save();
                    newdoc.Close(false);

                    trans.Commit();
                }
            }
        }
        public List<FamilyInstance> GetEquipmentsHaveCon(Document doc, string pipeSystemName, string equipmentName)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            IList<FamilyInstance> allInstances = CollectorHelper.TCollector<FamilyInstance>(doc);
            IList<PipingSystem> allPipingSys = CollectorHelper.TCollector<PipingSystem>(doc);

            foreach (var sys in allPipingSys)
            {
                PipingSystemType psType = doc.GetElement(sys.GetTypeId()) as PipingSystemType;

                if (psType.Name.Replace("给排水_", "").Replace("管道", "") == pipeSystemName)
                {
                    ElementSet elements = sys.PipingNetwork;
                    foreach (var ele in elements)
                    {
                        FamilyInstance instance = ele as FamilyInstance;
                        if (instance != null)
                        {
                            if (instance.Symbol.FamilyName.Contains(equipmentName))
                            {
                                list.Add(instance);
                            }
                        }
                    }
                }
            }
            return list;
        }
        public List<FamilyInstance> GetEquipmentsNoCon(Document doc, string equipmentName)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            IList<FamilyInstance> allInstances = CollectorHelper.TCollector<FamilyInstance>(doc);

            foreach (var instance in allInstances)
            {
                if (instance.Symbol.FamilyName.Contains(equipmentName))
                {
                    list.Add(instance);
                }
            }
            return list;
        }
        public List<string> GetPipeSystemType(UIDocument uiDoc, string profession)
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
                if (item == "压力污水系统")
                {
                    sl.Add(8, item);
                }
                if (item == "压力废水系统")
                {
                    sl.Add(9, item);
                }
                if (item == "废水系统")
                {
                    sl.Add(10, item);
                }
                if (item == "热水给水系统")
                {
                    sl.Add(11, item);
                }
                if (item == "排泥系统")
                {
                    sl.Add(12, item);
                }
                if (item == "消毒剂系统")
                {
                    sl.Add(13, item);
                }
                if (item == "混凝剂系统")
                {
                    sl.Add(14, item);
                }
                if (item == "水质稳定剂系统")
                {
                    sl.Add(15, item);
                }
            }

            List<string> orderedList = new List<string>();
            foreach (var item in sl.Values)
            {
                orderedList.Add(item.ToString());
            }
            return orderedList;
        }
        public List<PipeValveInfo> GetPipeSystemAccessoryJ(Document doc, string pipeSystemName, string subProjectNum, int valveCode)//挠性橡胶接头与Y型过滤器列表获取
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeValveInfo> valveNameList = new List<PipeValveInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管路附件") && v.Name.Replace("给排水_", "").Replace("管路附件明细表", "") == pipeSystemName.Replace("系统", ""))
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

                        if (valveTable.ElementAt(1).Contains("Y型过滤器") || valveTable.ElementAt(1).Contains("挠性橡胶接头"))
                        {
                            string abb = "";
                            string model = "";
                            string note = "";
                            string name = "";

                            if (valveTable.ElementAt(1).Contains("Y型过滤器"))
                            {
                                abb = "FR";
                                name = "Y型过滤器";
                                model = "";
                                note = "配置对应法兰、螺栓、螺母、垫片";
                            }
                            else if (valveTable.ElementAt(1).Contains("挠性橡胶接头"))
                            {
                                abb = "JE";
                                name = "可曲挠橡胶接头";
                                model = "K-XT-3";
                                note = "配置对应法兰、螺栓、螺母、垫片";
                            }

                            PipeValveInfo valveInfo = new PipeValveInfo(valveTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, abb + valveCode.ToString().PadLeft(2, '0'), name,
                            model, "DN" + sArray.FirstOrDefault(), valveTable.ElementAt(2), valveTable.ElementAt(4), note);
                            valveNameList.Add(valveInfo);
                            valveCode++;
                        }
                        valveTable.Clear();
                    }
                }
            }
            return valveNameList;
        }
        public List<PipeValveInfo> GetPipeSystemVisualGlass(Document doc, string pipeSystemName, string subProjectNum, int valveCode)//水流视镜列表获取
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeValveInfo> valveNameList = new List<PipeValveInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管路附件") && v.Name.Replace("给排水_", "").Replace("管路附件明细表", "") == pipeSystemName.Replace("系统", ""))
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

                        if (valveTable.ElementAt(1).Contains("水流视镜"))
                        {
                            string model = "LX";
                            string note = "配对应法兰、螺栓、螺母、垫片等";

                            PipeValveInfo valveInfo = new PipeValveInfo(valveTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "LO" + valveCode.ToString().PadLeft(2, '0'), valveTable.ElementAt(1).Replace("给排水_仪表_", ""),
                            model, "DN" + sArray.FirstOrDefault(), valveTable.ElementAt(2), valveTable.ElementAt(4), note);
                            valveNameList.Add(valveInfo);
                            valveCode++;
                        }
                        valveTable.Clear();
                    }
                }
            }
            return valveNameList;
        }
        public List<PipeValveInfo> GetPipeSystemAccessoryW(Document doc, string pipeSystemName, string subProjectNum, int valveCode)//排水设备附件列表获取
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeValveInfo> valveNameList = new List<PipeValveInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管路附件") && v.Name.Replace("给排水_", "").Replace("管路附件明细表", "") == pipeSystemName.Replace("系统", ""))
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

                        if (valveTable.ElementAt(1).Contains("排水设备附件"))
                        {
                            string model = "LX";
                            string note = "";

                            PipeValveInfo valveInfo = new PipeValveInfo(valveTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "OX" + valveCode.ToString().PadLeft(2, '0'), valveTable.ElementAt(1).Replace("给排水_排水设备附件_", ""),
                            model, "DN" + sArray.FirstOrDefault(), valveTable.ElementAt(2), valveTable.ElementAt(4), note);
                            valveNameList.Add(valveInfo);
                            valveCode++;
                        }
                        valveTable.Clear();
                    }
                }
            }
            return valveNameList;
        }
        public List<PipeValveInfo> GetPipeSystemMeter(Document doc, string pipeSystemName, string subProjectNum, int valveCode)//水表列表获取
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeValveInfo> valveNameList = new List<PipeValveInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管路附件") && v.Name.Replace("给排水_", "").Replace("管路附件明细表", "") == pipeSystemName.Replace("系统", ""))
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

                        if ((valveTable.ElementAt(1).Contains("水表")) && valveTable.ElementAt(1).Contains("仪表"))
                        {
                            string model = "LX";
                            string note = "";
                            if (valveTable.ElementAt(1).Contains("旋翼"))
                            {
                                model = "LXS-" + sArray.FirstOrDefault();
                            }
                            else
                            {
                                model = "LXL-" + sArray.FirstOrDefault();
                                note = "配套法兰、垫片、螺栓及螺母";
                            }

                            PipeValveInfo valveInfo = new PipeValveInfo(valveTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "LF" + valveCode.ToString().PadLeft(2, '0'), valveTable.ElementAt(1).Replace("给排水_仪表_", ""),
                            model, "DN" + sArray.FirstOrDefault(), valveTable.ElementAt(2), valveTable.ElementAt(4), note);
                            valveNameList.Add(valveInfo);
                            valveCode++;
                        }
                        valveTable.Clear();
                    }
                }
            }
            return valveNameList;
        }
        public List<PipeValveInfo> GetPipeSystemValve(Document doc, string pipeSystemName, string subProjectNum, int valveCode)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeValveInfo> valveNameList = new List<PipeValveInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管路附件") && v.Name.Replace("给排水_", "").Replace("管路附件明细表", "") == pipeSystemName.Replace("系统", ""))
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

                        if (valveTable.ElementAt(1).Contains("阀门") && !(valveTable.ElementAt(1).Contains("便器")))
                        {
                            PipeValveInfo valveInfo = new PipeValveInfo(valveTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "VA" + valveCode.ToString().PadLeft(2, '0'), StringHelper.FilterCH(valveTable.ElementAt(1).Replace("给排水_阀门_", "")),
                          StringHelper.FilterEN(valveTable.ElementAt(1).Replace("给排水_阀门_", "")), "DN" + sArray.FirstOrDefault(), valveTable.ElementAt(2), valveTable.ElementAt(4), valveTable.ElementAt(5));
                            valveNameList.Add(valveInfo);

                            //string ss = valveInfo.ValvePipeSystem + "\n" + valveInfo.ProjectNum + "\n" + valveInfo.ValveAbb + "\n" + valveInfo.ValveName + "\n" + valveInfo.ValveModel
                            //+ "\n" + valveInfo.ValveSize + "\n" + valveInfo.ValvePressure + "\n" + valveInfo.ValveQulity + "\n" + valveInfo.ValveNote;
                            //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (valveInfo.ValveName.Contains("电动"))
                            {
                                for (int k = 0; k < int.Parse(valveInfo.ValveQulity); k++)
                                {
                                    valveCode++;
                                }
                            }
                            else
                            {
                                valveCode++;
                            }
                        }
                        valveTable.Clear();
                    }
                }
            }
            return valveNameList;
        }
        public List<PipeInfo> GetPipeSystemPipe(Document doc, string pipeSystemName, string subProjectNum, int pipeCode)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeInfo> pipeNameList = new List<PipeInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管道") && v.Name.Replace("给排水_", "").Replace("管道明细表", "") == pipeSystemName.Replace("系统", ""))
                {
                    TableData td = v.GetTableData();
                    TableSectionData tdb = td.GetSectionData(SectionType.Header);
                    string head = v.GetCellText(SectionType.Header, 0, 0);

                    TableSectionData tdd = td.GetSectionData(SectionType.Body);
                    int c = tdd.NumberOfColumns;
                    int r = tdd.NumberOfRows;
                    List<string> pipeTable = new List<string>();

                    for (int i = 1; i < r; i++)
                    {
                        for (int j = 0; j < c; j++)
                        {
                            CellType ctype = tdd.GetCellType(i, j);
                            string str = v.GetCellText(SectionType.Body, i, j);
                            pipeTable.Add(str);
                        }
                        //string[] sArray = pipeTable.ElementAt(3).Split('-');
                        string pipeSize = "";
                        if (subProjectNum.Contains("G19") || subProjectNum.Contains("91"))
                        {
                            if (pipeTable.ElementAt(1).Contains("焊接") || pipeTable.ElementAt(1).Contains("镀锌")
                                || (pipeTable.ElementAt(1).Contains("PE") && !(pipeTable.ElementAt(1).Contains("HD"))) || pipeTable.ElementAt(1).Contains("钢丝网骨架"))
                            {
                                pipeSize = PipeSizeHaveThick(pipeTable.ElementAt(3), pipeTable.ElementAt(1));
                            }
                            else
                            {
                                pipeSize = pipeTable.ElementAt(3);
                            }
                        }
                        else
                        {
                            pipeSize = pipeTable.ElementAt(3);
                        }

                        PipeInfo pipeInfo = new PipeInfo(pipeTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "QX" + pipeCode.ToString().PadLeft(2, '0'), pipeTable.ElementAt(1).Replace("给排水_", "").Replace("_厂区", ""),
                                                                          pipeSize, pipeTable.ElementAt(2), pipeTable.ElementAt(6), pipeTable.ElementAt(7));
                        pipeNameList.Add(pipeInfo);

                        string ss = pipeInfo.PipeSystem + "\n" + pipeInfo.ProjectNum + "\n" + pipeInfo.PipeAbb + "\n" + pipeInfo.PipeName
                                        + "\n" + pipeInfo.PipeSize + "\n" + pipeInfo.PipePressure + "\n" + pipeInfo.PipeQulity + "\n" + pipeInfo.PipeNote;
                        //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        pipeCode++;

                        pipeTable.Clear();
                    }
                }
            }
            return pipeNameList;
        }
        public List<PipeElbowInfo> GetPipeSystemPipeElbow(Document doc, string pipeSystemName, string subProjectNum, int pipeElbowCode, string pipeElbowAngle)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeElbowInfo> pipeElbowNameList = new List<PipeElbowInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管件") && v.Name.Replace("给排水_", "").Replace("管件明细表", "") == pipeSystemName.Replace("系统", ""))
                {
                    TableData td = v.GetTableData();
                    TableSectionData tdb = td.GetSectionData(SectionType.Header);
                    string head = v.GetCellText(SectionType.Header, 0, 0);

                    TableSectionData tdd = td.GetSectionData(SectionType.Body);
                    int c = tdd.NumberOfColumns;
                    int r = tdd.NumberOfRows;
                    List<string> pipeElbowTable = new List<string>();

                    for (int i = 1; i < r; i++)
                    {
                        for (int j = 0; j < c; j++)
                        {
                            CellType ctype = tdd.GetCellType(i, j);
                            string str = v.GetCellText(SectionType.Body, i, j);
                            pipeElbowTable.Add(str);
                        }

                        if (pipeElbowTable.ElementAt(1).Contains("弯头") && pipeElbowTable.ElementAt(2).Contains(pipeElbowAngle))
                        {
                            string[] sArray = pipeElbowTable.ElementAt(4).Split('-');
                            string[] sAngle = pipeElbowTable.ElementAt(2).Split('.');

                            string pipeSize = "";
                            if (subProjectNum.Contains("G19") || subProjectNum.Contains("91"))
                            {
                                if (pipeElbowTable.ElementAt(1).Contains("钢制") || pipeElbowTable.ElementAt(1).Contains("镀锌")
                                    || (pipeElbowTable.ElementAt(1).Contains("PE") && !(pipeElbowTable.ElementAt(1).Contains("HD"))) || pipeElbowTable.ElementAt(1).Contains("钢丝网骨架"))
                                {
                                    pipeSize = PipeSizeHaveThick("DN" + sArray.ElementAt(0), pipeElbowTable.ElementAt(1));
                                }
                                else
                                {
                                    pipeSize = "DN" + sArray.ElementAt(0);
                                }
                            }
                            else
                            {
                                pipeSize = "DN" + sArray.ElementAt(0);
                            }

                            PipeElbowInfo pipeElbowInfo = new PipeElbowInfo(pipeElbowTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "OX" + pipeElbowCode.ToString().PadLeft(2, '0'),
                                                                      sAngle.ElementAt(0) + "°" + pipeElbowTable.ElementAt(1).Replace("给排水_管件_", "").Replace("_厂区", ""), pipeSize, pipeElbowTable.ElementAt(3), pipeElbowTable.ElementAt(5));
                            pipeElbowNameList.Add(pipeElbowInfo);
                            pipeElbowCode++;
                        }

                        //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);                    
                        pipeElbowTable.Clear();
                    }
                }
            }
            return pipeElbowNameList;
        }
        public List<PipeTeeInfo> GetPipeSystemPipeTee(Document doc, string pipeSystemName, string subProjectNum, int pipeTeeCode)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeTeeInfo> pipeTeeNameList = new List<PipeTeeInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管件") && v.Name.Replace("给排水_", "").Replace("管件明细表", "") == pipeSystemName.Replace("系统", ""))
                {
                    TableData td = v.GetTableData();
                    TableSectionData tdb = td.GetSectionData(SectionType.Header);
                    string head = v.GetCellText(SectionType.Header, 0, 0);

                    TableSectionData tdd = td.GetSectionData(SectionType.Body);
                    int c = tdd.NumberOfColumns;
                    int r = tdd.NumberOfRows;
                    List<string> pipeTeeTable = new List<string>();

                    for (int i = 1; i < r; i++)
                    {
                        for (int j = 0; j < c; j++)
                        {
                            CellType ctype = tdd.GetCellType(i, j);
                            string str = v.GetCellText(SectionType.Body, i, j);
                            pipeTeeTable.Add(str);
                        }

                        if (pipeTeeTable.ElementAt(1).Contains("三通"))
                        {
                            string[] sArray = pipeTeeTable.ElementAt(4).Split('-');
                            PipeTeeInfo pipeTeeInfo = new PipeTeeInfo(pipeTeeTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "OX" + pipeTeeCode.ToString().PadLeft(2, '0'),
                                                                   pipeTeeTable.ElementAt(1).Replace("给排水_管件_", "").Replace("_厂区", ""), "DN" + sArray.ElementAt(0) + "X" + sArray.ElementAt(2), pipeTeeTable.ElementAt(3), pipeTeeTable.ElementAt(5));
                            pipeTeeNameList.Add(pipeTeeInfo);
                            pipeTeeCode++;
                        }

                        //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);                    
                        pipeTeeTable.Clear();
                    }
                }
            }
            return pipeTeeNameList;
        }
        public List<PipeReduceInfo> GetPipeSystemPipeReduce(Document doc, string pipeSystemName, string subProjectNum, int pipeReduceCode)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeReduceInfo> pipeReduceNameList = new List<PipeReduceInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("管件") && v.Name.Replace("给排水_", "").Replace("管件明细表", "") == pipeSystemName.Replace("系统", ""))
                {
                    TableData td = v.GetTableData();
                    TableSectionData tdb = td.GetSectionData(SectionType.Header);
                    string head = v.GetCellText(SectionType.Header, 0, 0);

                    TableSectionData tdd = td.GetSectionData(SectionType.Body);
                    int c = tdd.NumberOfColumns;
                    int r = tdd.NumberOfRows;
                    List<string> pipeReduceTable = new List<string>();

                    for (int i = 1; i < r; i++)
                    {
                        for (int j = 0; j < c; j++)
                        {
                            CellType ctype = tdd.GetCellType(i, j);
                            string str = v.GetCellText(SectionType.Body, i, j);
                            pipeReduceTable.Add(str);
                        }

                        if (pipeReduceTable.ElementAt(1).Contains("异径"))
                        {
                            string[] sArray = pipeReduceTable.ElementAt(4).Split('-');
                            PipeReduceInfo pipeReduceInfo = new PipeReduceInfo(pipeReduceTable.ElementAt(0).Replace("给排水_", "").Replace("管道", ""), subProjectNum, "OX" + pipeReduceCode.ToString().PadLeft(2, '0'),
                                                                   pipeReduceTable.ElementAt(1).Replace("给排水_管件_", "").Replace("_厂区", ""), "DN" + sArray.ElementAt(0) + "X" + sArray.ElementAt(1), pipeReduceTable.ElementAt(3), pipeReduceTable.ElementAt(5));
                            pipeReduceNameList.Add(pipeReduceInfo);
                            pipeReduceCode++;
                        }

                        //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);                    
                        pipeReduceTable.Clear();
                    }
                }
            }
            return pipeReduceNameList;
        }
        public string PipeSizeHaveThick(string pipeSize, string pipeMaterial)
        {
            string str = "";

            if (pipeMaterial.Contains("焊接") || pipeMaterial.Contains("钢制"))
            {
                if (pipeSize == "DN15")
                {
                    str = "DN15";
                }
                else if (pipeSize == "DN20")
                {
                    str = "DN20";
                }
                else if (pipeSize == "DN25")
                {
                    str = "DN25";
                }
                else if (pipeSize == "DN32")
                {
                    str = "DN32";
                }
                else if (pipeSize == "DN40")
                {
                    str = "DN40";
                }
                else if (pipeSize == "DN50")
                {
                    str = "DN50(D57X3.5)";
                }
                else if (pipeSize == "DN65")
                {
                    str = "DN65(D76X3.5)";
                }
                else if (pipeSize == "DN80")
                {
                    str = "DN80(D89X4.0)";
                }
                else if (pipeSize == "DN100")
                {
                    str = "DN100(D108X4.0)";
                }
                else if (pipeSize == "DN125")
                {
                    str = "DN125(D133X4.0)";
                }
                else if (pipeSize == "DN150")
                {
                    str = "DN150(D159X4.5)";
                }
                else if (pipeSize == "DN200")
                {
                    str = "DN200(D219X6.0)";
                }
                else if (pipeSize == "DN250")
                {
                    str = "DN250(D273X8.0)";
                }
                else if (pipeSize == "DN300")
                {
                    str = "DN300(D325X8.0)";
                }
                else if (pipeSize == "DN350")
                {
                    str = "DN350(D377X9.0)";
                }
                else if (pipeSize == "DN400")
                {
                    str = "DN400(D426X9.0)";
                }
                else if (pipeSize == "DN450")
                {
                    str = "DN450(D480X9.0)";
                }
                else if (pipeSize == "DN500")
                {
                    str = "DN500(D530X9.0)";
                }
                else if (pipeSize == "DN600")
                {
                    str = "DN600(D630X9.0)";
                }
                else if (pipeSize == "DN700")
                {
                    str = "DN700(D720X9.0)";
                }
            }

            if (pipeMaterial.Contains("镀锌"))
            {
                if (pipeSize == "DN15")
                {
                    str = "DN15";
                }
                else if (pipeSize == "DN20")
                {
                    str = "DN20";
                }
                else if (pipeSize == "DN25")
                {
                    str = "DN25";
                }
                else if (pipeSize == "DN32")
                {
                    str = "DN32";
                }
                else if (pipeSize == "DN40")
                {
                    str = "DN40";
                }
                else if (pipeSize == "DN50")
                {
                    str = "DN50(D60.3X3.8)";
                }
                else if (pipeSize == "DN65")
                {
                    str = "DN65(D76.1X4.0)";
                }
                else if (pipeSize == "DN80")
                {
                    str = "DN80(D88.9X4.0)";
                }
                else if (pipeSize == "DN100")
                {
                    str = "DN100(D114.3X4.0)";
                }
                else if (pipeSize == "DN125")
                {
                    str = "DN125(D139.7X4.0)";
                }
                else if (pipeSize == "DN150")
                {
                    str = "DN150(D168.3X4.5)";
                }
                else if (pipeSize == "DN200")
                {
                    str = "DN200(D219.1X6.5)";
                }
                else if (pipeSize == "DN250")
                {
                    str = "DN250(D273X10.5)";
                }
                else if (pipeSize == "DN300")
                {
                    str = "DN300(D325X10.0)";
                }
                else if (pipeSize == "DN350")
                {
                    str = "DN350(D377X10.0)";
                }
            }

            if (pipeMaterial.Contains("PE") && !(pipeMaterial.Contains("HD")))//此数据来源于亚太管道,压力为PN10，1.0Mpa
            {
                if (pipeSize == "DN15")
                {
                    str = "DN15";
                }
                else if (pipeSize == "DN20")
                {
                    str = "DN20";
                }
                else if (pipeSize == "DN25")
                {
                    str = "DN25";
                }
                else if (pipeSize == "DN32")
                {
                    str = "DN32(De40X2.4)";
                }
                else if (pipeSize == "DN40")
                {
                    str = "DN40(De50X3.0)";
                }
                else if (pipeSize == "DN50")
                {
                    str = "DN50(De63X3.8)";
                }
                else if (pipeSize == "DN65")
                {
                    str = "DN65(De75X4.5)";
                }
                else if (pipeSize == "DN80")
                {
                    str = "DN80(De90X5.4)";
                }
                else if (pipeSize == "DN100")
                {
                    str = "DN100(De110X6.6)";
                }
                else if (pipeSize == "DN125")
                {
                    str = "DN125(De140X8.3)";
                }
                else if (pipeSize == "DN150")
                {
                    str = "DN150(De160X9.5)";
                }
                else if (pipeSize == "DN200")
                {
                    str = "DN200(De225X13.4)";
                }
                else if (pipeSize == "DN250")
                {
                    str = "DN250(De250X14.8)";
                }
                else if (pipeSize == "DN300")
                {
                    str = "DN300(De315X18.7)";
                }
                else if (pipeSize == "DN350")
                {
                    str = "DN350(De355X21.1)";
                }
                else if (pipeSize == "DN400")
                {
                    str = "DN400(De400X23.7)";
                }
                else if (pipeSize == "DN450")
                {
                    str = "DN450(De450X26.7)";
                }
                else if (pipeSize == "DN500")
                {
                    str = "DN500(De500X29.7)";
                }
            }

            if (pipeMaterial.Contains("钢丝网骨架"))//此数据来源于CJT189-2007钢丝网骨架塑料（聚乙烯）复合管材及管件，压力等级为1.6Mpa
            {
                if (pipeSize == "DN15")
                {
                    str = "DN15";
                }
                else if (pipeSize == "DN20")
                {
                    str = "DN20";
                }
                else if (pipeSize == "DN25")
                {
                    str = "DN25";
                }
                else if (pipeSize == "DN32")
                {
                    str = "DN32(De40X2.4)";
                }
                else if (pipeSize == "DN40")
                {
                    str = "DN40(De50X4.5)";
                }
                else if (pipeSize == "DN50")
                {
                    str = "DN50(De63X4.5)";
                }
                else if (pipeSize == "DN65")
                {
                    str = "DN65(De75X5.0)";
                }
                else if (pipeSize == "DN80")
                {
                    str = "DN80(De90X5.5)";
                }
                else if (pipeSize == "DN100")
                {
                    str = "DN100(De110X7.0)";
                }
                else if (pipeSize == "DN125")
                {
                    str = "DN125(De140X8.0)";
                }
                else if (pipeSize == "DN150")
                {
                    str = "DN150(De160X9.0)";
                }
                else if (pipeSize == "DN200")
                {
                    str = "DN200(De225X10.0)";
                }
                else if (pipeSize == "DN250")
                {
                    str = "DN250(De250X12.0)";
                }
                else if (pipeSize == "DN300")
                {
                    str = "DN300(De315X13.0)";
                }
                else if (pipeSize == "DN350")
                {
                    str = "DN350(De355X14.0)";
                }
                else if (pipeSize == "DN400")
                {
                    str = "DN400(De400X15.0)";
                }
                else if (pipeSize == "DN450")
                {
                    str = "DN450(De450X16.0)";
                }
                else if (pipeSize == "DN500")
                {
                    str = "DN500(De500X18.0)";
                }
            }

            return str;
        }
        public string GetMeasuringRange(string pipeSize)
        {
            string str = "0~50";

            if (pipeSize == "15")
            {
                str = "0.4~5";
            }
            else if (pipeSize == "20")
            {
                str = "0.6~8";
            }
            else if (pipeSize == "25")
            {
                str = "0.9~12";
            }
            else if (pipeSize == "32")
            {
                str = "1.5~20";
            }
            else if (pipeSize == "40")
            {
                str = "2.3~30";
            }
            else if (pipeSize == "50")
            {
                str = "3.5~50";
            }
            else if (pipeSize == "65")
            {
                str = "6~80";
            }
            else if (pipeSize == "80")
            {
                str = "9~120";
            }
            else if (pipeSize == "100")
            {
                str = "14~200";
            }
            else if (pipeSize == "125")
            {
                str = "22~310";
            }
            else if (pipeSize == "150")
            {
                str = "32~450";
            }
            else if (pipeSize == "200")
            {
                str = "57~800";
            }
            else if (pipeSize == "250")
            {
                str = "88~1200";
            }
            else if (pipeSize == "300")
            {
                str = "130~1800";
            }
            else if (pipeSize == "350")
            {
                str = "173~2500";
            }
            else if (pipeSize == "400")
            {
                str = "226~3200";
            }
            else if (pipeSize == "450")
            {
                str = "287~4000";
            }
            else if (pipeSize == "500")
            {
                str = "354~5000";
            }

            return str;
        }
        public bool SewageHaveSteelElbow(Document doc)
        {
            bool haveSteel = false;

            IList<PipingSystem> allPipingSys = CollectorHelper.TCollector<PipingSystem>(doc);
            foreach (var sys in allPipingSys)
            {
                PipingSystemType psType = doc.GetElement(sys.GetTypeId()) as PipingSystemType;

                if (psType.Name == "给排水_污水管道系统")
                {
                    ElementSet elements = sys.PipingNetwork;
                    foreach (var ele in elements)
                    {
                        FamilyInstance instance = ele as FamilyInstance;
                        if (instance != null)
                        {
                            if (instance.Symbol.FamilyName.Contains("管路附件"))
                            {
                                haveSteel = true;
                            }
                        }
                    }
                }
            }

            return haveSteel;
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
    public class PipeInfo
    {
        public string ProjectNum { get; set; }
        public string PipeSystem { get; set; }
        public string PipeAbb { get; set; }//管道缩写
        public string PipeName { get; set; }
        public string PipeSize { get; set; }
        public string PipePressure { get; set; }
        public string PipeQulity { get; set; }
        public string PipeNote { get; set; }
        public PipeInfo()
        {

        }
        public PipeInfo(string pipeSystem, string projectNum, string pipeAbb, string pipeName,
                              string pipeSize, string pipePressure, string pipeQulity, string pipeNote)
        {
            ProjectNum = projectNum;
            PipeSystem = pipeSystem;
            PipeAbb = pipeAbb;
            PipeName = pipeName;
            PipeSize = pipeSize;
            PipePressure = pipePressure;
            PipeQulity = pipeQulity;
            PipeNote = pipeNote;
        }
    }
    public class PipeElbowInfo
    {
        public string ProjectNum { get; set; }
        public string PipeElbowSystem { get; set; }
        public string PipeElbowAbb { get; set; }//弯头缩写
        public string PipeElbowName { get; set; }
        public string PipeElbowSize { get; set; }
        public string PipeElbowPressure { get; set; }
        public string PipeElbowQulity { get; set; }
        public PipeElbowInfo()
        {

        }
        public PipeElbowInfo(string pipeElbowSystem, string projectNum, string pipeElbowAbb, string pipeElbowName,
                              string pipeElbowSize, string pipeElbowPressure, string pipeElbowQulity)
        {
            ProjectNum = projectNum;
            PipeElbowSystem = pipeElbowSystem;
            PipeElbowAbb = pipeElbowAbb;
            PipeElbowName = pipeElbowName;
            PipeElbowSize = pipeElbowSize;
            PipeElbowPressure = pipeElbowPressure;
            PipeElbowQulity = pipeElbowQulity;
        }
    }
    public class PipeTeeInfo
    {
        public string ProjectNum { get; set; }
        public string PipeTeeSystem { get; set; }
        public string PipeTeeAbb { get; set; }//三通缩写
        public string PipeTeeName { get; set; }
        public string PipeTeeSize { get; set; }
        public string PipeTeePressure { get; set; }
        public string PipeTeeQulity { get; set; }
        public PipeTeeInfo()
        {

        }
        public PipeTeeInfo(string pipeTeeSystem, string projectNum, string pipeTeeAbb, string pipeTeeName,
                              string pipeTeeSize, string pipeTeePressure, string pipeTeeQulity)
        {
            ProjectNum = projectNum;
            PipeTeeSystem = pipeTeeSystem;
            PipeTeeAbb = pipeTeeAbb;
            PipeTeeName = pipeTeeName;
            PipeTeeSize = pipeTeeSize;
            PipeTeePressure = pipeTeePressure;
            PipeTeeQulity = pipeTeeQulity;
        }
    }
    public class PipeReduceInfo
    {
        public string ProjectNum { get; set; }
        public string PipeReduceSystem { get; set; }
        public string PipeReduceAbb { get; set; }//异径缩写
        public string PipeReduceName { get; set; }
        public string PipeReduceSize { get; set; }
        public string PipeReducePressure { get; set; }
        public string PipeReduceQulity { get; set; }
        public PipeReduceInfo()
        {

        }
        public PipeReduceInfo(string pipeReduceSystem, string projectNum, string pipeReduceAbb, string pipeReduceName,
                              string pipeReduceSize, string pipeReducePressure, string pipeReduceQulity)
        {
            ProjectNum = projectNum;
            PipeReduceSystem = pipeReduceSystem;
            PipeReduceAbb = pipeReduceAbb;
            PipeReduceName = pipeReduceName;
            PipeReduceSize = pipeReduceSize;
            PipeReducePressure = pipeReducePressure;
            PipeReduceQulity = pipeReduceQulity;
        }
    }
    public class PipeAccessoryInfo
    {
        public string ProjectNum { get; set; }
        public string PipeReduceSystem { get; set; }
        public string PipeReduceAbb { get; set; }//异径缩写
        public string PipeReduceName { get; set; }
        public string PipeReduceSize { get; set; }
        public string PipeReducePressure { get; set; }
        public string PipeReduceQulity { get; set; }
        public PipeAccessoryInfo()
        {

        }
        public PipeAccessoryInfo(string pipeReduceSystem, string projectNum, string pipeReduceAbb, string pipeReduceName,
                              string pipeReduceSize, string pipeReducePressure, string pipeReduceQulity)
        {
            ProjectNum = projectNum;
            PipeReduceSystem = pipeReduceSystem;
            PipeReduceAbb = pipeReduceAbb;
            PipeReduceName = pipeReduceName;
            PipeReduceSize = pipeReduceSize;
            PipeReducePressure = pipeReducePressure;
            PipeReduceQulity = pipeReduceQulity;
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
                sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
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
