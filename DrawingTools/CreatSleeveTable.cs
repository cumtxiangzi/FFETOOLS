using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatSleeveTable : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;
                ViewDrafting viewDraft = null;

                using (Transaction trans = new Transaction(doc, "����Ԥ���׹ܼ�Ԥ������"))
                {
                    trans.Start();

                    IList<ViewDrafting> viewDrafts = CollectorHelper.TCollector<ViewDrafting>(doc);
                    foreach (ViewDrafting vf in viewDrafts)
                    {
                        if (vf.Name == "����ˮ-Ԥ���׹�,Ԥ������")
                        {
                            doc.Delete(vf.Id);
                            break;
                        }
                    }

                    ElementId eid = new ElementId(-1);
                    IList<Element> elems = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).ToElements();
                    foreach (Element e in elems)
                    {
                        ViewFamilyType v = e as ViewFamilyType;
                        if (v != null && v.ViewFamily == ViewFamily.Drafting)
                        {
                            eid = e.Id;
                            break;
                        }
                    }

                    viewDraft = ViewDrafting.Create(doc, eid);
                    viewDraft.Name = "����ˮ-Ԥ���׹�,Ԥ������";
                    viewDraft.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).Set("");//����ͼֽ�ϵı���
                    viewDraft.Scale = 50;
                    viewDraft.Discipline = ViewDiscipline.Mechanical;
                    viewDraft.LookupParameter("�ӹ��").Set("����ˮ");

                    IList<FamilyInstance> sleeveNotes = CollectorHelper.TCollector<FamilyInstance>(doc);
                    List<FamilyInstance> sleeveNoteInstances = new List<FamilyInstance>();

                    foreach (FamilyInstance s in sleeveNotes)
                    {
                        if (s.Symbol.FamilyName.Contains("����ˮ_ע�ͷ���_�׹���ĸ��ע"))
                        {
                            sleeveNoteInstances.Add(s);
                        }
                    }

                    List<PipeSleeveInfo> sleeveInfos = new List<PipeSleeveInfo>();
                    foreach (var item in sleeveNoteInstances)
                    {
                        string code = item.LookupParameter("��ĸ���").AsString();
                        int isSleeve = item.LookupParameter("�Ƿ�Ϊ�׹�").AsInteger();
                        string pipeSize = item.LookupParameter("�ܵ�ֱ��").AsString();
                        string pipeHeight = item.LookupParameter("�����ı��").AsString();
                        string sleeeveSize = item.LookupParameter("�׹�ֱ��").AsString();
                        string holeSize = item.LookupParameter("Ԥ����").AsString();
                        string note = item.LookupParameter("��ע").AsString();

                        PipeSleeveInfo info = new PipeSleeveInfo()
                        {
                            SleeveCode = code,
                            IsSleeve = isSleeve,
                            PipeSize = pipeSize,
                            PipeHeight = pipeHeight,
                            SleeveSize = sleeeveSize,
                            PipeHole = holeSize,
                            SleeveNote = note
                        };
                        sleeveInfos.Add(info);
                    }

                    sleeveInfos.Sort((a, b) => a.SleeveCode.CompareTo(b.SleeveCode));//����                 

                    List<StoreSleeveInfo> storeInfoList = new List<StoreSleeveInfo>();//���� SleeveCode����
                    storeInfoList = sleeveInfos.GroupBy(x => x.SleeveCode)
                        .Select(group => new StoreSleeveInfo
                        {
                            SleeveCode = group.Key,
                            List = group.ToList()
                        }).ToList();

                    for (int i = 0; i < storeInfoList.Count + 1; i++)
                    {
                        doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(0, -i * 500 / 304.8, 0), new XYZ(7500 / 304.8, -i * 500 / 304.8, 0)));
                    }

                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(0, 1000 / 304.8, 0), new XYZ(7500 / 304.8, 1000 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(0, 1000 / 304.8, 0), new XYZ(0, -storeInfoList.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(500 / 304.8, 1000 / 304.8, 0), new XYZ(500 / 304.8, -storeInfoList.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(1500 / 304.8, 1000 / 304.8, 0), new XYZ(1500 / 304.8, -storeInfoList.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(2500 / 304.8, 1000 / 304.8, 0), new XYZ(2500 / 304.8, -storeInfoList.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(3500 / 304.8, 1000 / 304.8, 0), new XYZ(3500 / 304.8, -storeInfoList.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(4500 / 304.8, 1000 / 304.8, 0), new XYZ(4500 / 304.8, -storeInfoList.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(5000 / 304.8, 1000 / 304.8, 0), new XYZ(5000 / 304.8, -storeInfoList.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(7500 / 304.8, 1000 / 304.8, 0), new XYZ(7500 / 304.8, -storeInfoList.Count * 500 / 304.8, 0)));

                    XYZ point21 = new XYZ(100 / 304.8, 1000 / 304.8 - 350 / 304.8, 0);
                    XYZ point22 = new XYZ(650 / 304.8, 1000 / 304.8 - 120 / 304.8, 0);
                    XYZ point23 = new XYZ(1570 / 304.8, 1000 / 304.8 - 350 / 304.8, 0);
                    XYZ point24 = new XYZ(2650 / 304.8, 1000 / 304.8 - 120 / 304.8, 0);
                    XYZ point25 = new XYZ(3720 / 304.8, 1000 / 304.8 - 120 / 304.8, 0);
                    XYZ point26 = new XYZ(4580 / 304.8, 1000 / 304.8 - 350 / 304.8, 0);
                    XYZ point27 = new XYZ(6000 / 304.8, 1000 / 304.8 - 350 / 304.8, 0);

                    TextNoteType textNoteType = null;
                    IList<TextNoteType> noteTypes = CollectorHelper.TCollector<TextNoteType>(doc);
                    textNoteType = noteTypes.FirstOrDefault(x => x.Name == "����ˮ-�ָ�3.5");

                    TextNote.Create(doc, viewDraft.Id, point21, "���", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point22, "�ܵ�ֱ��" + "\n" + " DN(mm)", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point23, "�����ı��", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point24, "�׹�ֱ��" + "\n" + "   (mm)", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point25, "Ԥ����" + "\n" + " (mm)", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point26, "����", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point27, "��ע", textNoteType.Id);

                    CreatTitle(doc,viewDraft);

                    for (int i = 0; i < storeInfoList.Count; i++)
                    {
                        XYZ point1 = new XYZ(200 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point2 = new XYZ(750 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point3 = new XYZ(1750 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point4 = new XYZ(2700 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point5 = new XYZ(3650 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point6 = new XYZ(4700 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point7 = new XYZ(5100 / 304.8, -i * 500 / 304.8 - 30 / 304.8, 0);
                        XYZ point11 = new XYZ(160 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point31 = new XYZ(1800 / 304.8, -i * 500 / 304.8 - 30 / 304.8, 0);

                        if (!IsNumber(storeInfoList[i].List.FirstOrDefault().SleeveCode))
                        {
                            TextNote.Create(doc, viewDraft.Id, point11, storeInfoList[i].List.FirstOrDefault().SleeveCode, textNoteType.Id);
                        }
                        else
                        {
                            TextNote.Create(doc, viewDraft.Id, point1, storeInfoList[i].List.FirstOrDefault().SleeveCode, textNoteType.Id);
                        }

                        TextNote.Create(doc, viewDraft.Id, point2, storeInfoList[i].List.FirstOrDefault().PipeSize, textNoteType.Id);

                        if (storeInfoList[i].List.FirstOrDefault().PipeHeight == "�ض�")
                        {
                            TextNote.Create(doc, viewDraft.Id, point31, storeInfoList[i].List.FirstOrDefault().PipeHeight, textNoteType.Id);
                        }
                        else
                        {
                            TextNote.Create(doc, viewDraft.Id, point3, storeInfoList[i].List.FirstOrDefault().PipeHeight, textNoteType.Id);
                        }

                        TextNote.Create(doc, viewDraft.Id, point4, storeInfoList[i].List.FirstOrDefault().SleeveSize, textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point5, storeInfoList[i].List.FirstOrDefault().PipeHole, textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point6, storeInfoList[i].List.Count.ToString(), textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point7, storeInfoList[i].List.FirstOrDefault().SleeveNote, textNoteType.Id);
                    }

                    trans.Commit();
                }

                uidoc.ActiveView = viewDraft;
            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        public void DetailDrawingFamilyLoad(Document doc, string categoryName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("����ˮ"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "����ˮ_ע�ͷ���_" + categoryName + ".rfa");
            }
        }
        public void CreatTitle(Document doc, View view) //����ͼ��
        {
            XYZ titlePosition = new XYZ(3750 / 304.8, 1800 / 304.8 , 0);
            DetailDrawingFamilyLoad(doc, "������ͼ�����ָ�5");
            FamilySymbol typeC_TitleSymbol = null;
            FamilyInstance typeC_Title = null;
            typeC_TitleSymbol = TitleSymbol(doc, "������ͼ����");
            typeC_TitleSymbol.Activate();
            typeC_Title = doc.Create.NewFamilyInstance(titlePosition, typeC_TitleSymbol, view);
            typeC_Title.LookupParameter("��������").Set("Ԥ���׹ܣ�Ԥ������");
            typeC_Title.LookupParameter("���߳���").SetValueString((9 * 5 + 10).ToString());
        }
        public FamilySymbol TitleSymbol(Document doc, string symbolName)
        {
            FamilySymbol symbol = null;
            FilteredElementCollector sectionCollector = new FilteredElementCollector(doc);
            sectionCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericAnnotation);

            IList<Element> sections = sectionCollector.ToElements();
            foreach (FamilySymbol item in sections)
            {
                if (item.Family.Name.Contains("����ˮ") && item.Family.Name.Contains(symbolName))
                {
                    symbol = item;
                    break;
                }
            }
            return symbol;
        }
        public bool IsNumber(string str) //��ȡ�ַ����е������ַ���
        {
            bool result = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsNumber(str, i))
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }
        private bool IsNumeric(string str)//�ж��Ƿ������ֻ����Ը���
        {

            foreach (char c in str)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
    public class PipeSleeveInfo
    {
        public string SleeveCode { get; set; }
        public int IsSleeve { get; set; }
        public string PipeSize { get; set; }
        public string PipeHeight { get; set; }
        public string SleeveSize { get; set; }
        public string PipeHole { get; set; }
        public string SleeveNote { get; set; }
    }
    public class StoreSleeveInfo
    {
        public string SleeveCode { get; set; }
        public List<PipeSleeveInfo> List { get; set; }
    }
}
