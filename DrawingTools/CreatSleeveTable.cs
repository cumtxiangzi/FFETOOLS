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
                    List<FamilyInstance> sleeveNoteInstances=new List<FamilyInstance>();

                    foreach (FamilyInstance s in sleeveNotes)
                    {
                        if (s.Symbol.FamilyName.Contains("给排水_注释符号_套管字母标注"))
                        {
                            sleeveNoteInstances.Add(s);
                        }
                    }

                    for (int i = 0; i < sleeveNoteInstances.Count+1; i++)
                    {
                       doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(0, -i*500/304.8, 0), new XYZ(7500 / 304.8, -i*500/304.8, 0)));
                    }

                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(0,  0, 0), new XYZ(0, -sleeveNoteInstances.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(500 / 304.8,0, 0), new XYZ(500 / 304.8, -sleeveNoteInstances.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(1500 / 304.8,0 , 0), new XYZ(1500 / 304.8, -sleeveNoteInstances.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(2500 / 304.8, 0, 0), new XYZ(2500 / 304.8, -sleeveNoteInstances.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(3500 / 304.8, 0, 0), new XYZ(3500 / 304.8, -sleeveNoteInstances.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(4500 / 304.8, 0, 0), new XYZ(4500 / 304.8, -sleeveNoteInstances.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(5000 / 304.8, 0, 0), new XYZ(5000 / 304.8, -sleeveNoteInstances.Count * 500 / 304.8, 0)));
                    doc.Create.NewDetailCurve(viewDraft, Line.CreateBound(new XYZ(7500 / 304.8, 0, 0), new XYZ(7500 / 304.8, -sleeveNoteInstances.Count * 500 / 304.8, 0)));

                    List<PipeSleeveInfo> sleeveInfos= new List<PipeSleeveInfo>();
                    foreach (var item in sleeveNoteInstances)
                    {
                        string code = item.LookupParameter("字母编号").AsString();
                        int isSleeve = item.LookupParameter("是否为套管").AsInteger();
                        string pipeSize= item.LookupParameter("管道直径").AsString();
                        string pipeHeight= item.LookupParameter("管中心标高").AsString();
                        string sleeeveSize= item.LookupParameter("套管直径").AsString();
                        string holeSize= item.LookupParameter("预留洞").AsString();
                        string note= item.LookupParameter("备注").AsString();

                        PipeSleeveInfo info=new PipeSleeveInfo() { SleeveCode=code,IsSleeve=isSleeve,PipeSize=pipeSize,PipeHeight=pipeHeight,
                        SleeveSize=sleeeveSize,PipeHole=holeSize,SleeveNote=note};
                        sleeveInfos.Add(info);
                    }

                    sleeveInfos.Sort((a, b) => a.SleeveCode.CompareTo(b.SleeveCode));

                    for(int i = 0; i < sleeveInfos.Count; i++)
                    {
                        TextNoteType textNoteType = null;
                        IList<TextNoteType> noteTypes = CollectorHelper.TCollector<TextNoteType>(doc);
                        textNoteType = noteTypes.FirstOrDefault(x => x.Name=="给排水-字高3.5");
                        
                        XYZ point1=new XYZ(200/304.8,-i*500/304.8-120/304.8,0);
                        XYZ point2 = new XYZ(750 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point3 = new XYZ(1750 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point4 = new XYZ(2700 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point5 = new XYZ(3650 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point6 = new XYZ(4700 / 304.8, -i * 500 / 304.8 - 120 / 304.8, 0);
                        XYZ point7 = new XYZ(5100 / 304.8, -i * 500 / 304.8 - 30 / 304.8, 0);
                        TextNote.Create(doc, viewDraft.Id, point1, sleeveInfos[i].SleeveCode, textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point2, sleeveInfos[i].PipeSize, textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point3, sleeveInfos[i].PipeHeight, textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point4, sleeveInfos[i].SleeveSize, textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point5, sleeveInfos[i].PipeHole, textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point6, "3", textNoteType.Id);
                        TextNote.Create(doc, viewDraft.Id, point7, sleeveInfos[i].SleeveNote, textNoteType.Id);

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
}
