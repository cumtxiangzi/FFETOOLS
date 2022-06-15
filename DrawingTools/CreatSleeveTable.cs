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

                using (Transaction trans = new Transaction(doc, "生成预留套管及预留洞表"))
                {
                    trans.Start();

                    IList<ViewDrafting> viewDrafts = CollectorHelper.TCollector<ViewDrafting>(doc);
                    foreach (ViewDrafting vf in viewDrafts)
                    {
                        if (vf.Name == "给排水-预埋套管,预留洞表")
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
                    viewDraft.Name = "给排水-预埋套管,预留洞表";
                    viewDraft.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).Set("");//设置图纸上的标题
                    viewDraft.Scale = 50;
                    viewDraft.Discipline = ViewDiscipline.Mechanical;
                    viewDraft.LookupParameter("子规程").Set("给排水");

                    IList<FamilyInstance> sleeveNotes = CollectorHelper.TCollector<FamilyInstance>(doc);
                    List<FamilyInstance> sleeveNoteInstances = new List<FamilyInstance>();

                    foreach (FamilyInstance s in sleeveNotes)
                    {
                        if (s.Symbol.FamilyName.Contains("给排水_注释符号_套管字母标注"))
                        {
                            sleeveNoteInstances.Add(s);
                        }
                    }

                    List<PipeSleeveInfo> sleeveInfos = new List<PipeSleeveInfo>();
                    foreach (var item in sleeveNoteInstances)
                    {
                        string code = item.LookupParameter("字母编号").AsString();
                        int isSleeve = item.LookupParameter("是否为套管").AsInteger();
                        string pipeSize = item.LookupParameter("管道直径").AsString();
                        string pipeHeight = item.LookupParameter("管中心标高").AsString();
                        string sleeeveSize = item.LookupParameter("套管直径").AsString();
                        string holeSize = item.LookupParameter("预留洞").AsString();
                        string note = item.LookupParameter("备注").AsString();

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

                    sleeveInfos.Sort((a, b) => a.SleeveCode.CompareTo(b.SleeveCode));//排序                 

                    List<StoreSleeveInfo> storeInfoList = new List<StoreSleeveInfo>();//根据 SleeveCode分组
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
                    textNoteType = noteTypes.FirstOrDefault(x => x.Name == "给排水-字高3.5");

                    TextNote.Create(doc, viewDraft.Id, point21, "编号", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point22, "管道直径" + "\n" + " DN(mm)", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point23, "管中心标高", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point24, "套管直径" + "\n" + "   (mm)", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point25, "预留洞" + "\n" + " (mm)", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point26, "数量", textNoteType.Id);
                    TextNote.Create(doc, viewDraft.Id, point27, "备注", textNoteType.Id);

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

                        if (storeInfoList[i].List.FirstOrDefault().PipeHeight == "池顶")
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
                if (item.Name.Contains(categoryName) && item.Name.Contains("给排水"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_注释符号_" + categoryName + ".rfa");
            }
        }
        public void CreatTitle(Document doc, View view) //创建图名
        {
            XYZ titlePosition = new XYZ(3750 / 304.8, 1800 / 304.8 , 0);
            DetailDrawingFamilyLoad(doc, "绘制视图标题字高5");
            FamilySymbol typeC_TitleSymbol = null;
            FamilyInstance typeC_Title = null;
            typeC_TitleSymbol = TitleSymbol(doc, "绘制视图标题");
            typeC_TitleSymbol.Activate();
            typeC_Title = doc.Create.NewFamilyInstance(titlePosition, typeC_TitleSymbol, view);
            typeC_Title.LookupParameter("标题名称").Set("预埋套管，预留洞表");
            typeC_Title.LookupParameter("横线长度").SetValueString((9 * 5 + 10).ToString());
        }
        public FamilySymbol TitleSymbol(Document doc, string symbolName)
        {
            FamilySymbol symbol = null;
            FilteredElementCollector sectionCollector = new FilteredElementCollector(doc);
            sectionCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericAnnotation);

            IList<Element> sections = sectionCollector.ToElements();
            foreach (FamilySymbol item in sections)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains(symbolName))
                {
                    symbol = item;
                    break;
                }
            }
            return symbol;
        }
        public bool IsNumber(string str) //提取字符串中的数字字符串
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
        private bool IsNumeric(string str)//判断是否是数字还是自负串
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
