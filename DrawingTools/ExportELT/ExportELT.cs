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
                Parameter proNum = pro.LookupParameter("���̴���");
                Parameter proName = pro.LookupParameter("��������");
                Parameter subproNum = pro.LookupParameter("�������");
                Parameter subproName = pro.LookupParameter("��������");
                Parameter name = pro.LookupParameter("����");

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

                            //ָ����Ҫд���sheet��
                            ExcelWorksheet excelWorksheet = package.Workbook.Worksheets["sheet1"];
                            string footPage = "&\"Arial\"" + "&16" + " " + "��" + "&P" + "ҳ��" + "��" + "&N" + "ҳ";

                            excelWorksheet.HeaderFooter.differentOddEven = false;
                            excelWorksheet.HeaderFooter.OddFooter.LeftAlignedText = excelFooterName;
                            excelWorksheet.HeaderFooter.OddFooter.RightAlignedText = footPage;

                            excelWorksheet.Cells[1, 4].Value = projectName;
                            excelWorksheet.Cells[2, 4].Value = subProjectName;
                            excelWorksheet.Cells[3, 4].Value = "ʩ��ͼ";
                            excelWorksheet.Cells[1, 4].Style.Font.Name = "Arial";
                            excelWorksheet.Cells[2, 4].Style.Font.Name = "Arial";
                            excelWorksheet.Cells[3, 4].Style.Font.Name = "Arial";

                            int rowNum = 7;
                            int valveCode = 1;  //�豸��ż���
                            List<string> pipeSystemList = GetPipeSystemType(uidoc, "����ˮ");
                            //string ss = String.Join("\n", pipeSystemList.ToArray());
                            //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            List<PipeValveInfo> valveList = new List<PipeValveInfo>();
                            List<PipeInfo> pipeList = new List<PipeInfo>();
                            List<FamilyInstance> outDoorHydrantsUp = new List<FamilyInstance>();

                            foreach (string item in pipeSystemList)
                            {
                                rowNum++;
                                int[,] mergeRowIndexs = { { rowNum - 2, 1, 11 }, { rowNum - 2, 1, 11 } };  //�ϲ���Ԫ��
                                ExcelHelper.MergeRowCells(excelWorksheet, 1, mergeRowIndexs);
                                excelWorksheet.Cells[rowNum - 1, 1].Value = item;
                                excelWorksheet.Cells[rowNum - 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                excelWorksheet.Cells[rowNum - 1, 1].Style.Font.Bold = true;

                                //����ʽ����˨д��
                                outDoorHydrantsUp = GetEquipmentsHaveCon(doc, item, "����ˮ_�����豸_�������ʽ����˨");
                                foreach (var hydrant in outDoorHydrantsUp)
                                {
                                    excelWorksheet.Cells[rowNum, 1].Value = subproNum.AsString();
                                    excelWorksheet.Cells[rowNum, 2].Value = "FZ" + valveCode.ToString().PadLeft(2, '0');
                                    excelWorksheet.Cells[rowNum, 4].Value = "�������ʽ����˨";
                                    excelWorksheet.Cells[rowNum, 6].Value = outDoorHydrantsUp.Count.ToString();
                                    excelWorksheet.Cells[rowNum, 9].Value = "���Ӧ��������˨����ĸ����Ƭ��";
                                    excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "�ͺ�:" + "SSF100/65-1.6";
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "��ˮ��:" + "DN100";
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "��ˮ��:" + "DN100, DN65";
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + "1.6MPa";
                                    rowNum++;
                                    valveCode++;
                                    break;
                                }

                                valveList = GetPipeSystemValve(doc, item, subproNum.AsString(), valveCode);
                                pipeList = GetPipeSystemPipe(doc, item, subproNum.AsString(), 1);

                                //����д��
                                foreach (PipeValveInfo valveInfo in valveList)
                                {
                                    excelWorksheet.Cells[rowNum, 1].Value = valveInfo.ProjectNum;
                                    excelWorksheet.Cells[rowNum, 2].Value = valveInfo.ValveAbb;
                                    excelWorksheet.Cells[rowNum, 4].Value = valveInfo.ValveName;
                                    excelWorksheet.Cells[rowNum, 6].Value = valveInfo.ValveQulity;
                                    excelWorksheet.Cells[rowNum, 9].Value = valveInfo.ValveNote;
                                    excelWorksheet.Cells[rowNum, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "�ͺ�:" + valveInfo.ValveModel;
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + valveInfo.ValvePressure;
                                    rowNum++;
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + valveInfo.ValveSize;
                                    rowNum++;
                                    valveCode++;
                                }

                                //�ܵ�д��
                                foreach (PipeInfo pipeInfo in pipeList)
                                {
                                    excelWorksheet.Cells[rowNum, 1].Value = pipeInfo.ProjectNum;
                                    excelWorksheet.Cells[rowNum, 2].Value = "PP" + valveCode.ToString().PadLeft(2, '0');
                                    excelWorksheet.Cells[rowNum, 4].Value = pipeInfo.PipeSystem + "�ܵ����ܼ�";
                                    rowNum++;
                                    valveCode++;
                                    break;
                                }
                                foreach (PipeInfo pipeInfo in pipeList)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeInfo.PipeAbb;
                                    if (pipeInfo.PipeName.Contains("��п"))
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "˫���Ƚ���п�ֹ�";
                                    }
                                    else if (pipeInfo.PipeName.Contains("HDPE"))
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "HDPE˫�ڲ��ƹ�";
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
                                        excelWorksheet.Cells[rowNum, 4].Value = "���ն�:��8kN/m2";
                                        rowNum++;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeInfo.PipePressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeInfo pipeInfo in pipeList)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + pipeInfo.PipeSize;
                                    if (pipeInfo.PipeQulity == "0 m")
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = "1 m";
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[rowNum, 6].Value = pipeInfo.PipeQulity;
                                    }
                                    rowNum++;
                                }

                                //90����ͷд��
                                int elbowNum = 2;
                                List<PipeElbowInfo> pipeElbowList90 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "90");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList90)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("��п"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��������ʽ90����ͷ";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "90���п��ͷ";
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
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList90)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + pipeElbowInfo.PipeElbowSize;
                                    excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    rowNum++;
                                }

                                //60����ͷд��
                                List<PipeElbowInfo> pipeElbowList60 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "60");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList60)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("��п"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��������ʽ60����ͷ";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "60���п��ͷ";
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
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList60)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + pipeElbowInfo.PipeElbowSize;
                                    excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    rowNum++;
                                }

                                //45����ͷд��
                                List<PipeElbowInfo> pipeElbowList45 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "45");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList45)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("��п"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��������ʽ45����ͷ";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "45���п��ͷ";
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
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList45)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + pipeElbowInfo.PipeElbowSize;
                                    excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    rowNum++;
                                }

                                //30����ͷд��
                                List<PipeElbowInfo> pipeElbowList30 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "30");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList30)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("��п"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��������ʽ30����ͷ";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "30���п��ͷ";
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
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList30)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + pipeElbowInfo.PipeElbowSize;
                                    excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    rowNum++;
                                }

                                //22.5����ͷд��
                                List<PipeElbowInfo> pipeElbowList225 = GetPipeSystemPipeElbow(doc, item, subproNum.AsString(), elbowNum, "22");
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList225)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeElbowInfo.PipeElbowAbb;
                                    if (pipeElbowInfo.PipeElbowName.Contains("��п"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��������ʽ22.5����ͷ";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "22.5���п��ͷ";
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
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeElbowInfo.PipeElbowPressure;
                                        rowNum++;
                                    }

                                    break;
                                }
                                foreach (PipeElbowInfo pipeElbowInfo in pipeElbowList225)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + pipeElbowInfo.PipeElbowSize;
                                    excelWorksheet.Cells[rowNum, 6].Value = pipeElbowInfo.PipeElbowQulity;
                                    rowNum++;
                                }

                                //��ͨд��
                                List<PipeTeeInfo> pipeTeeList = GetPipeSystemPipeTee(doc, item, subproNum.AsString(), elbowNum);
                                foreach (PipeTeeInfo pipeTeeInfo in pipeTeeList)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeTeeInfo.PipeTeeAbb;
                                    if (pipeTeeInfo.PipeTeeName.Contains("��п"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��������ʽ��ͨ";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��п��ͨ";
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
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeTeeInfo.PipeTeePressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeTeeInfo pipeTeeInfo in pipeTeeList)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "���:" + pipeTeeInfo.PipeTeeSize;
                                    excelWorksheet.Cells[rowNum, 6].Value = pipeTeeInfo.PipeTeeQulity;
                                    rowNum++;
                                }

                                //�쾶д��
                                List<PipeReduceInfo> pipeReduceList = GetPipeSystemPipeReduce(doc, item, subproNum.AsString(), elbowNum);
                                foreach (PipeReduceInfo pipeReduceInfo in pipeReduceList)
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = pipeReduceInfo.PipeReduceAbb;
                                    if (pipeReduceInfo.PipeReduceName.Contains("��п"))
                                    {
                                        if (!(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��������ʽ�쾶";
                                        }
                                        else
                                        {
                                            excelWorksheet.Cells[rowNum, 4].Value = "��п�쾶";
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
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeReduceInfo.PipeReducePressure;
                                        rowNum++;
                                    }
                                    break;
                                }
                                foreach (PipeReduceInfo pipeReduceInfo in pipeReduceList)
                                {
                                    excelWorksheet.Cells[rowNum, 4].Value = "���:" + pipeReduceInfo.PipeReduceSize;
                                    excelWorksheet.Cells[rowNum, 6].Value = pipeReduceInfo.PipeReduceQulity;
                                    rowNum++;
                                }

                                //����д��ֻ��Թ���ʽ��������
                                if (item.Contains("����") && !(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = "OX" + elbowNum.ToString().PadLeft(2, '0');
                                    excelWorksheet.Cells[rowNum, 4].Value = "��������ʽ����";
                                    elbowNum++;
                                    rowNum++;

                                    foreach (PipeValveInfo valveInfo in valveList)
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + valveInfo.ValvePressure;
                                        rowNum++;
                                        break;
                                    }

                                    foreach (PipeValveInfo valveInfo in valveList)
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + valveInfo.ValveSize;
                                        excelWorksheet.Cells[rowNum, 6].Value = (int.Parse(valveInfo.ValveQulity) * 2).ToString();
                                        rowNum++;
                                    }
                                }

                                //����д��ֻ��Թ���ʽ��������
                                if (item.Contains("����") && !(subproNumOnly.Contains("G19") || subproNumOnly.Contains("91")))
                                {
                                    excelWorksheet.Cells[rowNum, 3].Value = "OX" + elbowNum.ToString().PadLeft(2, '0');
                                    excelWorksheet.Cells[rowNum, 4].Value = "�������ۿ���";
                                    elbowNum++;
                                    rowNum++;

                                    foreach (PipeInfo pipeInfo in pipeList)
                                    {
                                        excelWorksheet.Cells[rowNum, 4].Value = "����ѹ��:" + pipeInfo.PipePressure;
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

                                        excelWorksheet.Cells[rowNum, 4].Value = "����ֱ��:" + pipeInfo.PipeSize;
                                        excelWorksheet.Cells[rowNum, 6].Value = totalNum.ToString();
                                        rowNum++;
                                    }
                                }

                            }

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
                                System.Windows.Forms.MessageBox.Show("�豸�����ɹ�!", "GPSBIM", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    System.Windows.Forms.MessageBox.Show("����ƽ�����ά��ͼ�н��в���!", "GPSBIM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            //Transaction trans = new Transaction(doc, "������Ŀ��׼ʵ��");
            //trans.Start();
            //ʵ��������ճ��ѡ��,����ʵ��������
            //CopyPasteOptions option = new CopyPasteOptions();
            //���ڲ�����Ϣ��λ���޹�,����λ��Ϊnull,�������ʵ������������λ���йص�,����ط�����Ҫ˼����������
            //ElementTransformUtils.CopyElements(document, copyIds, doc, null, option);
            //trans.Commit();
            //����ر��޸ĺ���ĵ�
            // doc.Save();
            // doc.Close(false);
            // �ں�̨���ļ���UI�ϲ�����ʾ�����Ұ��ļ���ǽ��������ʾ������
            if (name.AsString() != "1")
            {
                using (Transaction trans = new Transaction(doc, "��ϸ���滻"))
                {
                    trans.Start();
                    name.Set("1");

                    FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
                    viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
                    IList<Element> viewScheduleList = viewCollector.ToElements();
                    ICollection<ElementId> deletIds = new Collection<ElementId>();
                    foreach (ViewSchedule v in viewScheduleList)
                    {
                        if (v.Name.Contains("����ˮ"))
                        {
                            deletIds.Add(v.Id);
                        }
                    }
                    doc.Delete(deletIds);

                    string filepath = @"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\����ˮ��Ŀ��׼.rvt";
                    Document newdoc = doc.Application.OpenDocumentFile(filepath);
                    FilteredElementCollector fec = new FilteredElementCollector(newdoc);
                    fec.OfCategory(BuiltInCategory.OST_Schedules);
                    IList<Element> fecList = fec.ToElements();
                    ICollection<ElementId> copyIds = new Collection<ElementId>();
                    foreach (ViewSchedule v in fecList)
                    {
                        if (v.Name.Contains("����ˮ"))
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

                if (psType.Name.Replace("�ܵ�", "").Contains(pipeSystemName))
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
        public List<string> GetPipeSystemType(UIDocument uiDoc, string profession)
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
                if (item == "ѹ����ˮϵͳ")
                {
                    sl.Add(8, item);
                }
                if (item == "ѹ����ˮϵͳ")
                {
                    sl.Add(9, item);
                }
                if (item == "��ˮϵͳ")
                {
                    sl.Add(10, item);
                }
                if (item == "��ˮ��ˮϵͳ")
                {
                    sl.Add(11, item);
                }
                if (item == "����ϵͳ")
                {
                    sl.Add(12, item);
                }
                if (item == "������ϵͳ")
                {
                    sl.Add(13, item);
                }
                if (item == "������ϵͳ")
                {
                    sl.Add(14, item);
                }
                if (item == "ˮ���ȶ���ϵͳ")
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
        public List<PipeValveInfo> GetPipeSystemValve(Document doc, string pipeSystemName, string subProjectNum, int valveCode)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfCategory(BuiltInCategory.OST_Schedules);
            IList<Element> viewScheduleList = viewCollector.ToElements();
            List<PipeValveInfo> valveNameList = new List<PipeValveInfo>();

            foreach (ViewSchedule v in viewScheduleList)
            {
                if (v.Name.Contains("��·����") && v.Name.Contains(pipeSystemName.Replace("ϵͳ", "")) && !(v.Name.Contains("��ˮ")))
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

                        if (valveTable.ElementAt(1).Contains("����") && !(valveTable.ElementAt(1).Contains("����")))
                        {
                            PipeValveInfo valveInfo = new PipeValveInfo(valveTable.ElementAt(0).Replace("����ˮ_", "").Replace("�ܵ�", ""), subProjectNum, "VA" + valveCode.ToString().PadLeft(2, '0'), StringHelper.FilterCH(valveTable.ElementAt(1).Replace("����ˮ_����_", "")),
                          StringHelper.FilterEN(valveTable.ElementAt(1).Replace("����ˮ_����_", "")), "DN" + sArray.FirstOrDefault(), valveTable.ElementAt(2), valveTable.ElementAt(4), valveTable.ElementAt(5));
                            valveNameList.Add(valveInfo);

                            //string ss = valveInfo.ValvePipeSystem + "\n" + valveInfo.ProjectNum + "\n" + valveInfo.ValveAbb + "\n" + valveInfo.ValveName + "\n" + valveInfo.ValveModel
                            //+ "\n" + valveInfo.ValveSize + "\n" + valveInfo.ValvePressure + "\n" + valveInfo.ValveQulity + "\n" + valveInfo.ValveNote;
                            //System.Windows.Forms.MessageBox.Show(ss, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            valveCode++;
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
                if (v.Name.Contains("�ܵ�") && v.Name.Contains(pipeSystemName.Replace("ϵͳ", "")))
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
                            if (pipeTable.ElementAt(1).Contains("����") || pipeTable.ElementAt(1).Contains("��п")
                                || (pipeTable.ElementAt(1).Contains("PE") && !(pipeTable.ElementAt(1).Contains("HD"))) || pipeTable.ElementAt(1).Contains("��˿���Ǽ�"))
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

                        PipeInfo pipeInfo = new PipeInfo(pipeTable.ElementAt(0).Replace("����ˮ_", "").Replace("�ܵ�", ""), subProjectNum, "QX" + pipeCode.ToString().PadLeft(2, '0'), pipeTable.ElementAt(1).Replace("����ˮ_", "").Replace("_����", ""),
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
                if (v.Name.Contains("�ܼ�") && v.Name.Contains(pipeSystemName.Replace("ϵͳ", "")))
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

                        if (pipeElbowTable.ElementAt(1).Contains("��ͷ") && pipeElbowTable.ElementAt(2).Contains(pipeElbowAngle))
                        {
                            string[] sArray = pipeElbowTable.ElementAt(4).Split('-');
                            string[] sAngle = pipeElbowTable.ElementAt(2).Split('.');

                            string pipeSize = "";
                            if (subProjectNum.Contains("G19") || subProjectNum.Contains("91"))
                            {
                                if (pipeElbowTable.ElementAt(1).Contains("����") || pipeElbowTable.ElementAt(1).Contains("��п")
                                    || (pipeElbowTable.ElementAt(1).Contains("PE") && !(pipeElbowTable.ElementAt(1).Contains("HD"))) || pipeElbowTable.ElementAt(1).Contains("��˿���Ǽ�"))
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

                            PipeElbowInfo pipeElbowInfo = new PipeElbowInfo(pipeElbowTable.ElementAt(0).Replace("����ˮ_", "").Replace("�ܵ�", ""), subProjectNum, "OX" + pipeElbowCode.ToString().PadLeft(2, '0'),
                                                                      sAngle.ElementAt(0) + "��" + pipeElbowTable.ElementAt(1).Replace("����ˮ_�ܼ�_", "").Replace("_����", ""), pipeSize, pipeElbowTable.ElementAt(3), pipeElbowTable.ElementAt(5));
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
                if (v.Name.Contains("�ܼ�") && v.Name.Contains(pipeSystemName.Replace("ϵͳ", "")))
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

                        if (pipeTeeTable.ElementAt(1).Contains("��ͨ"))
                        {
                            string[] sArray = pipeTeeTable.ElementAt(4).Split('-');
                            PipeTeeInfo pipeTeeInfo = new PipeTeeInfo(pipeTeeTable.ElementAt(0).Replace("����ˮ_", "").Replace("�ܵ�", ""), subProjectNum, "OX" + pipeTeeCode.ToString().PadLeft(2, '0'),
                                                                   pipeTeeTable.ElementAt(1).Replace("����ˮ_�ܼ�_", "").Replace("_����", ""), "DN" + sArray.ElementAt(0) + "X" + sArray.ElementAt(2), pipeTeeTable.ElementAt(3), pipeTeeTable.ElementAt(5));
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
                if (v.Name.Contains("�ܼ�") && v.Name.Contains(pipeSystemName.Replace("ϵͳ", "")))
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

                        if (pipeReduceTable.ElementAt(1).Contains("�쾶"))
                        {
                            string[] sArray = pipeReduceTable.ElementAt(4).Split('-');
                            PipeReduceInfo pipeReduceInfo = new PipeReduceInfo(pipeReduceTable.ElementAt(0).Replace("����ˮ_", "").Replace("�ܵ�", ""), subProjectNum, "OX" + pipeReduceCode.ToString().PadLeft(2, '0'),
                                                                   pipeReduceTable.ElementAt(1).Replace("����ˮ_�ܼ�_", "").Replace("_����", ""), "DN" + sArray.ElementAt(0) + "X" + sArray.ElementAt(1), pipeReduceTable.ElementAt(3), pipeReduceTable.ElementAt(5));
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

            if (pipeMaterial.Contains("����"))
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

            if (pipeMaterial.Contains("��п"))
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

            if (pipeMaterial.Contains("PE") && !(pipeMaterial.Contains("HD")))//��������Դ����̫�ܵ�,ѹ��ΪPN10��1.0Mpa
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

            if (pipeMaterial.Contains("��˿���Ǽ�"))//��������Դ��CJT189-2007��˿���Ǽ����ϣ�����ϩ�����Ϲܲļ��ܼ���ѹ���ȼ�Ϊ1.6Mpa
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
    public class PipeInfo
    {
        public string ProjectNum { get; set; }
        public string PipeSystem { get; set; }
        public string PipeAbb { get; set; }//�ܵ���д
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
        public string PipeElbowAbb { get; set; }//��ͷ��д
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
        public string PipeTeeAbb { get; set; }//��ͨ��д
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
        public string PipeReduceAbb { get; set; }//�쾶��д
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
        public string PipeReduceAbb { get; set; }//�쾶��д
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
                sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 1], mergeRowIndexs[i, 0] + startRowIndex, mergeRowIndexs[i, 2]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
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
