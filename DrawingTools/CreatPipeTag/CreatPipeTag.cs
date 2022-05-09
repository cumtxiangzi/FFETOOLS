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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatPipeTag : IExternalCommand
    {
        public static CreatPipeTagForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new CreatPipeTagForm();
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
    }
    public class ExecuteEventCreatPipeTag : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                Autodesk.Revit.DB.View view = uidoc.ActiveView;
                if (view is View3D)
                {
                    View3D aView = view as View3D;
                    if (aView.IsLocked == true)
                    {
                        CreatTagMain(doc, uidoc, app);
                    }
                    else
                    {
                        TaskDialog.Show("警告", "请将三维视图锁定后再进行操作");
                    }
                }
                else
                {
                    CreatTagMain(doc, uidoc, app);
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "管道绘制";
        }
        public void CreatTagMain(Document doc, UIDocument uidoc, UIApplication app)
        {
            //TransactionGroup tg = new TransactionGroup(doc, "创建给排水标注");
            //tg.Start();

            using (Transaction trans = new Transaction(doc, "给排水标注"))
            {
                trans.Start();
                if (CreatPipeTag.mainfrm.clicked == 1)
                {
                    CreatEquipmentTagMethod(doc, uidoc);
                }

                if (CreatPipeTag.mainfrm.clicked == 2)
                {
                    CreatPipeTagMethod(doc, uidoc, "管道公称直径");
                }

                if (CreatPipeTag.mainfrm.clicked == 3)
                {
                    CreatPipeTagMethod(doc, uidoc, "管道系统缩写");
                }

                if (CreatPipeTag.mainfrm.clicked == 4)
                {
                    CreatPipeTagMethod(doc, uidoc, "进出户管编号");
                }

                if (CreatPipeTag.mainfrm.clicked == 9)
                {
                    RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.SpotElevation);
                    app.PostCommand(cmdId);
                }

                if (CreatPipeTag.mainfrm.clicked == 10)
                {
                    CreatPipeAccessoryTagMethod(doc, uidoc);
                }


                if (CreatPipeTag.mainfrm.clicked == 19)
                {

                }

                if (CreatPipeTag.mainfrm.clicked == 100)
                {
                    CreatTextWithLineMethod(doc, uidoc);
                }

                FailureHandlingOptions failureOptions = trans.GetFailureHandlingOptions();
                failureOptions.SetFailuresPreprocessor(new HidePasteDuplicateTypesPreprocessor());
                trans.Commit(failureOptions);
            }

            if (CreatPipeTag.mainfrm.clicked == 5)
            {
                CreatPipeTagWithLine(doc, uidoc, "立管编号");
            }

            if (CreatPipeTag.mainfrm.clicked == 6)
            {
                CreatCommonNote(doc, uidoc, "管道基础留洞");
            }

            if (CreatPipeTag.mainfrm.clicked == 7)
            {
                CreatSleeveNote(doc, uidoc, "刚性防水套管");
            }

            if (CreatPipeTag.mainfrm.clicked == 8)
            {
                CreatSleeveNote(doc, uidoc, "柔性防水套管");
            }

            if (CreatPipeTag.mainfrm.clicked == 11)
            {
                CreatCommonNote(doc, uidoc, "管道楼板留洞方形");
            }

            if (CreatPipeTag.mainfrm.clicked == 12)
            {
                CreatCommonNote(doc, uidoc, "管道预留洞");
            }

            if (CreatPipeTag.mainfrm.clicked == 13)
            {
                CreatCommonNote(doc, uidoc, "爬梯标注");
            }

            if (CreatPipeTag.mainfrm.clicked == 14)
            {
                CreatManHoleNote(doc, uidoc);
            }

            if (CreatPipeTag.mainfrm.clicked == 15)
            {
                CreatVentPipeNote(doc, uidoc);
            }

            if (CreatPipeTag.mainfrm.clicked == 16)
            {
                CreatCommonNote(doc, uidoc, "吊环标注");
            }

            if (CreatPipeTag.mainfrm.clicked == 17)
            {
                CreatCommonNote(doc, uidoc, "电控箱标注");
            }

            if (CreatPipeTag.mainfrm.clicked == 18)
            {
                CreatCommonNote(doc, uidoc, "消火栓箱留洞标注");
            }

            if (CreatPipeTag.mainfrm.clicked == 20)
            {
                CreatCommonNote(doc, uidoc, "侧壁预埋板标注");
            }

            if (CreatPipeTag.mainfrm.clicked == 23)
            {
                CreatCommonNote(doc, uidoc, "管道楼板留洞圆形");
            }

            //tg.Assimilate();
        }
        public bool CreatCommonNote(Document doc, UIDocument uidoc, string name)//创建通用标注
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "创建通用标注"))
                {
                    trans.Start();
                    string LanguageVer = CreatPipeTag.mainfrm.LanguageCmb.SelectedItem.ToString();

                    if (name == "爬梯标注")
                    {
                        TagFamilyLoad(doc, "梯子标注");
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new GenericModelSelectionFilter(), "请选择爬梯");
                        FamilyInstance ladder = doc.GetElement(reference) as FamilyInstance;
                        LocationPoint ladderLocation = ladder.Location as LocationPoint;

                        XYZ pt1 = ladderLocation.Point;
                        XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                        FamilySymbol ladderSymbol = null;
                        AnnotationSymbol ladderNote = null;
                        ladderSymbol = NoteSymbol(doc, "梯子标注");
                        ladderSymbol.Activate();
                        ladderNote = doc.Create.NewFamilyInstance(pt2, ladderSymbol, doc.ActiveView) as AnnotationSymbol;
                        ladderNote.addLeader();
                        IList<Leader> leadList = ladderNote.GetLeaders();
                        Leader lead = leadList[0];
                        lead.End = pt1;
                    }

                    if (name == "电控箱标注")
                    {                     
                        Selection sel = uidoc.Selection;
                        XYZ pt1 = sel.PickPoint("请选择电控箱位置");
                        XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                        FamilySymbol boxSymbol = null;
                        AnnotationSymbol boxNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "潜水泵电控箱标注");
                            boxSymbol = NoteSymbol(doc, "潜水泵电控箱标注");
                            boxSymbol.Activate();
                            boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                            boxNote.addLeader();
                            IList<Leader> leadList = boxNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "潜水泵电控箱英文注释");
                            boxSymbol = NoteSymbol(doc, "潜水泵电控箱英文注释");
                            boxSymbol.Activate();
                            boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                            boxNote.addLeader();
                            IList<Leader> leadList = boxNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                        }
                    }

                    if (name == "吊环标注")
                    {                                            
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new GenericSignSelectionFilter(), "请选择吊环十字符号");
                        AnnotationSymbol ring = doc.GetElement(reference) as AnnotationSymbol;
                        LocationPoint ringLocation = ring.Location as LocationPoint;

                        Reference reference1 = sel.PickObject(ObjectType.Element, new GenericSignSelectionFilter(), "请选择吊环十字符号");
                        AnnotationSymbol ring1 = doc.GetElement(reference1) as AnnotationSymbol;
                        LocationPoint ringLocation1 = ring1.Location as LocationPoint;

                        XYZ pt1 = ringLocation.Point;
                        XYZ pt2 = ringLocation1.Point;
                        XYZ pt3 = sel.PickPoint("请选择标注创建位置");

                        FamilySymbol ringSymbol = null;
                        AnnotationSymbol ringNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "吊环标注");
                            ringSymbol = NoteSymbol(doc, "吊环标注");
                            ringSymbol.Activate();
                            ringNote = doc.Create.NewFamilyInstance(pt3, ringSymbol, doc.ActiveView) as AnnotationSymbol;
                            ringNote.addLeader();
                            ringNote.addLeader();
                            IList<Leader> leadList = ringNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                            Leader lead1 = leadList[1];
                            lead1.End = pt2;
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "吊环英文注释");
                            ringSymbol = NoteSymbol(doc, "吊环英文注释");
                            ringSymbol.Activate();
                            ringNote = doc.Create.NewFamilyInstance(pt3, ringSymbol, doc.ActiveView) as AnnotationSymbol;
                            ringNote.addLeader();
                            ringNote.addLeader();
                            IList<Leader> leadList = ringNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                            Leader lead1 = leadList[1];
                            lead1.End = pt2;
                        }
                    }

                    if (name == "侧壁预埋板标注")
                    {                                           
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new DetailineSelectionFilter(), "请选择预埋钢板线");
                        DetailLine steel = doc.GetElement(reference) as DetailLine;
                        LocationCurve steelLocationCurve = steel.Location as LocationCurve;
                        string steelLength = steel.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = steelLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                        FamilySymbol steelSymbol = null;
                        AnnotationSymbol steelNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "侧壁预埋钢板标注");
                            steelSymbol = NoteSymbol(doc, "侧壁预埋钢板标注");
                            steelSymbol.Activate();
                            steelNote = doc.Create.NewFamilyInstance(pt2, steelSymbol, doc.ActiveView) as AnnotationSymbol;
                            steelNote.addLeader();
                            IList<Leader> leadList = steelNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            steelNote.LookupParameter("下行文字").Set(steelLength.ToString() + "X100X10mm钢板");
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "侧壁预埋钢板英文注释");
                            steelSymbol = NoteSymbol(doc, "侧壁预埋钢板英文注释");
                            steelSymbol.Activate();
                            steelNote = doc.Create.NewFamilyInstance(pt2, steelSymbol, doc.ActiveView) as AnnotationSymbol;
                            steelNote.addLeader();
                            IList<Leader> leadList = steelNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            steelNote.LookupParameter("下行文字").Set(steelLength.ToString() + "X100X10mm");
                        }

                    }

                    if (name == "消火栓箱留洞标注")
                    {                                           
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new MechanicalSelectionFilter(), "请选择消火栓箱");
                        FamilyInstance box = doc.GetElement(reference) as FamilyInstance;
                        LocationPoint boxLocation = box.Location as LocationPoint;

                        XYZ pt1 = boxLocation.Point;
                        XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                        FamilySymbol boxSymbol = null;
                        AnnotationSymbol boxNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "消火栓预留洞标注");
                            boxSymbol = NoteSymbol(doc, "消火栓预留洞标注");
                            boxSymbol.Activate();
                            boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                            boxNote.addLeader();
                            IList<Leader> leadList = boxNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "消火栓预留洞英文注释");
                            boxSymbol = NoteSymbol(doc, "消火栓预留洞英文注释");
                            boxSymbol.Activate();
                            boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                            boxNote.addLeader();
                            IList<Leader> leadList = boxNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                        }
                    }

                    if (name == "管道基础留洞")
                    {                     
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择管道");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol baseHoleSymbol = null;
                        AnnotationSymbol baseHoleNote = null;
                       
                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "管道基础留洞标注");
                            baseHoleSymbol = NoteSymbol(doc, "管道基础留洞标注");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("留洞尺寸").Set(BaseHoleSize(pipeSize));
                            baseHoleNote.LookupParameter("洞中心标高").Set(d.ToString("0.000"));
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "管道基础留洞英文注释");
                            baseHoleSymbol = NoteSymbol(doc, "管道基础留洞英文注释");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("留洞尺寸").Set(BaseHoleSize(pipeSize));
                            baseHoleNote.LookupParameter("洞中心标高").Set(d.ToString("0.000"));
                        }
                    }

                    if (name == "管道预留洞")
                    {                      
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择管道");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol baseHoleSymbol = null;
                        AnnotationSymbol baseHoleNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "管道预留洞标注");
                            baseHoleSymbol = NoteSymbol(doc, "管道预留洞标注");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("留洞尺寸").Set(BaseHoleSize(pipeSize));
                            baseHoleNote.LookupParameter("洞中心标高").Set(d.ToString("0.000"));
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "管道预留洞英文注释");
                            baseHoleSymbol = NoteSymbol(doc, "管道预留洞英文注释");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("留洞尺寸").Set(BaseHoleSize(pipeSize));
                            baseHoleNote.LookupParameter("洞中心标高").Set(d.ToString("0.000"));
                        }
                    }

                    if (name == "管道楼板留洞方形")
                    {                    
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择管道");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol baseHoleSymbol = null;
                        AnnotationSymbol baseHoleNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "管道楼板留洞标注");
                            baseHoleSymbol = NoteSymbol(doc, "管道楼板留洞标注");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("留洞尺寸").Set(BaseHoleSize(pipeSize));
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "管道楼板留洞英文注释");
                            baseHoleSymbol = NoteSymbol(doc, "管道楼板留洞英文注释");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("留洞尺寸").Set(BaseHoleSize(pipeSize));
                        }
                    }

                    if (name == "管道楼板留洞圆形")
                    {                        
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择管道");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol baseHoleSymbol = null;
                        AnnotationSymbol baseHoleNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "管道楼板留洞标注");
                            baseHoleSymbol = NoteSymbol(doc, "管道楼板留洞标注");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("留洞尺寸").Set(FloorHoleSize(pipeSize));
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "管道楼板留洞英文注释");
                            baseHoleSymbol = NoteSymbol(doc, "管道楼板留洞英文注释");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("留洞尺寸").Set(FloorHoleSize(pipeSize));
                        }

                    }

                    trans.Commit();
                }

                CreatCommonNote(doc, uidoc, name);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;
        }
        public bool CreatManHoleNote(Document doc, UIDocument uidoc)//创建人孔标注
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "创建人孔标注"))
                {
                    trans.Start();

                    TagFamilyLoad(doc, "检修孔标注");
                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new GenericModelSelectionFilter(), "请选择人孔");
                    FamilyInstance manHole = doc.GetElement(reference) as FamilyInstance;
                    LocationPoint manHoleLocation = manHole.Location as LocationPoint;
                    string manHoleSize = manHole.LookupParameter("人孔半径").AsValueString();
                    double manHoleDN = double.Parse(manHoleSize) * 2;

                    XYZ pt1 = manHoleLocation.Point;
                    XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                    FamilySymbol manHoleSymbol = null;
                    AnnotationSymbol manHoleNote = null;
                    manHoleSymbol = NoteSymbol(doc, "检修孔标注");
                    manHoleSymbol.Activate();
                    manHoleNote = doc.Create.NewFamilyInstance(pt2, manHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                    manHoleNote.addLeader();
                    IList<Leader> leadList = manHoleNote.GetLeaders();
                    Leader lead = leadList[0];
                    lead.End = pt1;
                    manHoleNote.LookupParameter("上行文字").Set("Φ" + manHoleDN.ToString() + " 检修孔");

                    trans.Commit();
                }

                CreatManHoleNote(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;
        }
        public bool CreatVentPipeNote(Document doc, UIDocument uidoc)
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "创建通气管标注"))
                {
                    trans.Start();
                    string LanguageVer = CreatPipeTag.mainfrm.LanguageCmb.SelectedItem.ToString();                   
                    
                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new MechanicalSelectionFilter(), "请选择通气管");
                    FamilyInstance ventPipe = doc.GetElement(reference) as FamilyInstance;
                    LocationPoint ventPipeLocation = ventPipe.Location as LocationPoint;
                    string ventPipeHeight = ventPipe.LookupParameter("通气管顶部高度").AsValueString();
                    double ventHeight = double.Parse(ventPipeHeight);

                    XYZ pt1 = ventPipeLocation.Point;
                    XYZ pt2 = sel.PickPoint("请选择标注创建位置");

                    if (ventHeight == 900 || ventHeight == 1400)
                    {
                        FamilySymbol ventSymbol = null;
                        AnnotationSymbol ventNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "通气管标注");
                            ventSymbol = NoteSymbol(doc, "通气管标注");
                            ventSymbol.Activate();
                            ventNote = doc.Create.NewFamilyInstance(pt2, ventSymbol, doc.ActiveView) as AnnotationSymbol;
                            ventNote.addLeader();
                            IList<Leader> leadList = ventNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                            ventNote.LookupParameter("上行文字").Set("通气管DN200 高出池顶" + ventPipeHeight + "mm");
                            ventNote.LookupParameter("下行文字").Set("详见02S403,98");
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "通气管英文注释");
                            ventSymbol = NoteSymbol(doc, "通气管英文注释");
                            ventSymbol.Activate();
                            ventNote = doc.Create.NewFamilyInstance(pt2, ventSymbol, doc.ActiveView) as AnnotationSymbol;
                            ventNote.addLeader();
                            IList<Leader> leadList = ventNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                            ventNote.LookupParameter("上行文字").Set("VENT PIPE " + ventPipeHeight + "mm "+ "HIGHER");
                            ventNote.LookupParameter("下行文字").Set("THAN TOP OF TANK");
                        }

                    }

                    if (ventHeight != 900 && ventHeight != 1400)
                    {
                        double d = UnitUtils.Convert(pt1.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);

                        FamilySymbol ventSymbol = null;
                        AnnotationSymbol ventNote = null;

                        if (LanguageVer=="中文")
                        {
                            TagFamilyLoad(doc, "通气管标注");
                            ventSymbol = NoteSymbol(doc, "通气管标注");
                            ventSymbol.Activate();
                            ventNote = doc.Create.NewFamilyInstance(pt2, ventSymbol, doc.ActiveView) as AnnotationSymbol;
                            ventNote.addLeader();
                            IList<Leader> leadList = ventNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;

                            double ventRealHeight = ventHeight - (d + 200) * (-1);
                            if (TwoValueEqual(ventRealHeight, 900))
                            {
                                ventNote.LookupParameter("上行文字").Set("通气管DN200 高出覆土900mm");
                                ventNote.LookupParameter("下行文字").Set("详见02S403,98");
                            }

                            if (TwoValueEqual(ventRealHeight, 1400))
                            {
                                ventNote.LookupParameter("上行文字").Set("通气管DN200 高出覆土1400mm");
                                ventNote.LookupParameter("下行文字").Set("详见02S403,98");
                            }
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "通气管英文注释");
                            ventSymbol = NoteSymbol(doc, "通气管英文注释");
                            ventSymbol.Activate();
                            ventNote = doc.Create.NewFamilyInstance(pt2, ventSymbol, doc.ActiveView) as AnnotationSymbol;
                            ventNote.addLeader();
                            IList<Leader> leadList = ventNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;

                            double ventRealHeight = ventHeight - (d + 200) * (-1);
                            if (TwoValueEqual(ventRealHeight, 900))
                            {
                                ventNote.LookupParameter("上行文字").Set("VENT PIPE 900mm HIGHER");
                                ventNote.LookupParameter("下行文字").Set("THAN TOP OF COVER SOIL");
                            }

                            if (TwoValueEqual(ventRealHeight, 1400))
                            {
                                ventNote.LookupParameter("上行文字").Set("VENT PIPE 1400mm HIGHER");
                                ventNote.LookupParameter("下行文字").Set("THAN TOP OF COVER SOIL");
                            }
                        }
                    }

                    trans.Commit();
                }

                CreatVentPipeNote(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;
        }
        public bool CreatSleeveNote(Document doc, UIDocument uidoc, string sleeveName)
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "创建套管标注"))
                {
                    trans.Start();

                    string LanguageVer = CreatPipeTag.mainfrm.LanguageCmb.SelectedItem.ToString();                                                     

                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "请选择管道");
                    Pipe pipe = doc.GetElement(reference) as Pipe;
                    LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                    string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                    XYZ pickPoint = reference.GlobalPoint;
                    XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                    XYZ pt2 = sel.PickPoint("请选择标注创建位置");
                    double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                    if (sleeveName == "刚性防水套管")
                    {
                        FamilySymbol sleeveSymbol = null;
                        AnnotationSymbol sleeveNote = null;

                        if (LanguageVer == "中文")
                        {
                            TagFamilyLoad(doc, "刚性防水套管标注");
                            sleeveSymbol = NoteSymbol(doc, "刚性防水套管标注");
                            sleeveSymbol.Activate();
                            sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                            sleeveNote.addLeader();
                            IList<Leader> leadList = sleeveNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            sleeveNote.LookupParameter("上行文字").Set("预埋刚性防水套管(A)型D3=" + SleeveSize(pipeSize, 0) + "mm");
                            sleeveNote.LookupParameter("池壁套管下行文字").Set("套管中心标高" + d.ToString("0.000"));
                        }

                        if (LanguageVer == "英文")
                        {
                            
                            sleeveSymbol = NoteSymbol(doc, "防水套管英文注释");
                            sleeveSymbol.Activate();
                            sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                            sleeveNote.addLeader();
                            IList<Leader> leadList = sleeveNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            sleeveNote.LookupParameter("上行文字").Set("EMBEDDED WATER-PROOF SLEEVE D3=" + SleeveSize(pipeSize, 0) + "mm");
                            sleeveNote.LookupParameter("池壁套管文字").Set("CENTER ELEVATION " + d.ToString("0.000"));
                        }
                    }

                    if (sleeveName == "柔性防水套管")
                    {
                        FamilySymbol sleeveSymbol = null;
                        AnnotationSymbol sleeveNote = null;

                        if (LanguageVer == "中文")
                        {
                            TagFamilyLoad(doc, "柔性防水套管标注");
                            sleeveSymbol = NoteSymbol(doc, "柔性防水套管标注");
                            sleeveSymbol.Activate();
                            sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                            sleeveNote.addLeader();
                            IList<Leader> leadList = sleeveNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            sleeveNote.LookupParameter("上行文字").Set("预埋柔性防水套管(A)型D2=" + SleeveSize(pipeSize, 1) + "mm");
                            sleeveNote.LookupParameter("池壁套管下行文字").Set("套管中心标高" + d.ToString("0.000"));
                        }

                        if (LanguageVer == "英文")
                        {
                            TagFamilyLoad(doc, "防水套管英文注释");
                            sleeveSymbol = NoteSymbol(doc, "防水套管英文注释");
                            sleeveSymbol.Activate();
                            sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                            sleeveNote.addLeader();
                            IList<Leader> leadList = sleeveNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            sleeveNote.LookupParameter("上行文字").Set("EMBEDDED WATER-PROOF SLEEVE D2=" + SleeveSize(pipeSize, 1) + "mm");
                            sleeveNote.LookupParameter("池壁套管文字").Set("CENTER ELEVATION " + d.ToString("0.000"));
                        }
                    }

                    trans.Commit();
                }

                CreatSleeveNote(doc, uidoc, sleeveName);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;
        }
        public bool CreatPipeTagWithLine(Document doc, UIDocument uidoc, string tagName)
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "创建立管标注"))
                {
                    trans.Start();
                    TagFamilyLoad(doc, "立管编号");
                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter());
                    Pipe pipe = doc.GetElement(reference) as Pipe;
                    LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;

                    XYZ pickPoint = reference.GlobalPoint;
                    XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;

                    IList<Element> pipetagscollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                    FamilySymbol pipeTag = null;
                    Family pipeCodeTag = null;
                    foreach (var item in pipetagscollect)
                    {
                        FamilySymbol pipetag = item as FamilySymbol;
                        Family ftag = pipetag.Family;
                        if (ftag.Name.Contains(tagName) && ftag.Name.Contains("给排水"))
                        {
                            pipeTag = pipetag;
                            pipeCodeTag = ftag;
                            break;
                        }
                    }

                    Reference pipeRef = new Reference(pipe);
                    TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                    TagOrientation tagOri = TagOrientation.Horizontal;
                    IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, true, tageMode, tagOri, projectPickPoint);
                    tag.ChangeTypeId(pipeTag.Id);
                    tag.LeaderEndCondition = LeaderEndCondition.Free;

                    if (uidoc.ActiveView is View3D)
                    {
                        tag.LeaderEnd = projectPickPoint;
                        tag.LeaderElbow = projectPickPoint + new XYZ(100 / 304.8, 300 / 304.8, 200 / 304.8);
                        tag.TagHeadPosition = projectPickPoint + new XYZ(100 / 304.8, 469 / 304.8, 250 / 304.8);
                    }
                    else
                    {
                        tag.LeaderEnd = projectPickPoint;
                        tag.LeaderElbow = projectPickPoint + new XYZ(800 / 304.8, 800 / 304.8, 0);
                        tag.TagHeadPosition = projectPickPoint + new XYZ(1019 / 304.8, 1019 / 304.8, 0);
                    }
                    trans.Commit();
                }

                CreatPipeTagWithLine(doc, uidoc, tagName);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            //CreatPipeTagWithLine(doc, uidoc, tagName);
            return true;
        }
        public bool CreatPipeTagMethod(Document doc, UIDocument uidoc, string tagName)
        {
            try
            {
                TagFamilyLoad(doc, "管道公称直径");
                TagFamilyLoad(doc, "管道系统缩写");
                TagFamilyLoad(doc, "进出户管编号");

                Selection sel = uidoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter());
                Pipe pipe = doc.GetElement(reference) as Pipe;
                LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;

                XYZ pickPoint = reference.GlobalPoint;
                XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;

                IList<Element> pipetagscollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                FamilySymbol pipeTag = null;
                Family pipeCodeTag = null;
                foreach (var item in pipetagscollect)
                {
                    FamilySymbol pipetag = item as FamilySymbol;
                    Family ftag = pipetag.Family;
                    if (ftag.Name.Contains(tagName) && ftag.Name.Contains("给排水"))
                    {
                        pipeTag = pipetag;
                        pipeCodeTag = ftag;
                        break;
                    }
                }

                Reference pipeRef = new Reference(pipe);
                TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                TagOrientation tagOri = TagOrientation.Horizontal;
                IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, projectPickPoint);
                tag.ChangeTypeId(pipeTag.Id);

                if (tagName == "进出户管编号")
                {
                    ISet<ElementId> pipeCodeTagSet = pipeCodeTag.GetFamilySymbolIds();
                    List<FamilySymbol> pipeCodeTagList = new List<FamilySymbol>();
                    foreach (var item in pipeCodeTagSet)
                    {
                        pipeCodeTagList.Add(doc.GetElement(item) as FamilySymbol);
                    }

                    string pipeSystemName = (doc.GetElement(pipe.MEPSystem.GetTypeId()) as PipingSystemType).Name;
                    if (pipeSystemName.Contains("循环回水") || pipeSystemName.Contains("污水") || pipeSystemName.Contains("废水") || pipeSystemName.Contains("排泥"))
                    {
                        if ((ConnectorDirection(pipe) == "left") || (ConnectorDirection(pipe) == "down"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("左侧排水"))
                                {
                                    pipeTag = item;
                                    break;
                                }
                            }
                            tag.ChangeTypeId(pipeTag.Id);
                        }

                        if ((ConnectorDirection(pipe) == "right") || (ConnectorDirection(pipe) == "up"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("右侧排水"))
                                {
                                    pipeTag = item;
                                    break;
                                }
                            }
                            tag.ChangeTypeId(pipeTag.Id);
                            Location position = tag.Location;

                            if ((ConnectorDirection(pipe) == "right"))
                            {
                                XYZ direction = new XYZ(1200 / 304.8, 0, 0);
                                position.Move(direction);
                            }
                            if ((ConnectorDirection(pipe) == "up"))
                            {
                                XYZ direction = new XYZ(0, 1200 / 304.8, 0);
                                position.Move(direction);
                            }
                        }
                    }
                    if (pipeSystemName.Contains("循环给水") || pipeSystemName.Contains("消防给水") || pipeSystemName.Contains("生活给水") || pipeSystemName.Contains("中水") || pipeSystemName.Contains("水源输水"))
                    {
                        if ((ConnectorDirection(pipe) == "left") || (ConnectorDirection(pipe) == "down"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("左侧给水"))
                                {
                                    pipeTag = item;
                                    break;
                                }
                            }
                            tag.ChangeTypeId(pipeTag.Id);

                        }

                        if ((ConnectorDirection(pipe) == "right") || (ConnectorDirection(pipe) == "up"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("右侧给水"))
                                {
                                    pipeTag = item;
                                    break;
                                }
                            }
                            tag.ChangeTypeId(pipeTag.Id);
                            Location position = tag.Location;

                            if ((ConnectorDirection(pipe) == "right"))
                            {
                                XYZ direction = new XYZ(1200 / 304.8, 0, 0);
                                position.Move(direction);
                            }
                            if ((ConnectorDirection(pipe) == "up"))
                            {
                                XYZ direction = new XYZ(0, 1200 / 304.8, 0);
                                position.Move(direction);
                            }

                        }
                    }
                }

                CreatPipeTagMethod(doc, uidoc, tagName);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;

        }
        public bool CreatEquipmentTagMethod(Document doc, UIDocument uidoc)
        {
            try
            {
                TagFamilyLoad(doc, "设备编号");
                Selection sel = uidoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element, new MechanicalSelectionFilter());
                FamilyInstance equipment = doc.GetElement(reference) as FamilyInstance;
                if (!equipment.IsTaged(doc))
                {
                    SetEquipmentCode(doc, equipment);
                }             

                LocationPoint equipmentLocation = equipment.Location as LocationPoint;
                XYZ projectPickPoint = equipmentLocation.Point;

                IList<Element> equipmentTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipmentTags).ToElements();
                FamilySymbol equipmentTag = null;
                foreach (var item in equipmentTagsCollect)
                {
                    FamilySymbol etag = item as FamilySymbol;
                    Family ftag = etag.Family;
                    if (ftag.Name.Contains("设备编号") && ftag.Name.Contains("给排水"))
                    {
                        equipmentTag = etag;
                        break;
                    }
                }

                Reference pipeRef = new Reference(equipment);
                TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                TagOrientation tagOri = TagOrientation.Horizontal;
                IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, projectPickPoint);
                tag.ChangeTypeId(equipmentTag.Id);              

                CreatEquipmentTagMethod(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;

        }
        public void SetEquipmentCode(Document doc,FamilyInstance equipment)
        {
            string code = "919PU01";
            string name = equipment.Symbol.FamilyName;
            ProjectInfo pro = doc.ProjectInformation;
            Parameter subproNum = pro.LookupParameter("子项代号");

            if (name.Contains("给排水_水泵"))
            {
                code = subproNum.AsString() + "PU01";
            }

            if (name.Contains("给排水_给水设备"))
            {
                code = subproNum.AsString() + "WS01";
            }

            if (name.Contains("给排水_冷却设备"))
            {
                code = subproNum.AsString() + "TW01";
            }

            if (name.Contains("给排水_冷却设备"))
            {
                code = subproNum.AsString() + "TW01";
            }

            if (name.Contains("给排水")&& name.Contains("加药"))
            {
                code = subproNum.AsString() + "TS01";
            }

            if (name.Contains("给排水") && name.Contains("发生器"))
            {
                code = subproNum.AsString() + "TS01";
            }

            if (name.Contains("给排水") && name.Contains("过滤"))
            {
                code = subproNum.AsString() + "FR01";
            }

            if (name.Contains("给排水") && name.Contains("葫芦"))
            {
                code = subproNum.AsString() + "EH01";
            }

            if (name.Contains("给排水") && name.Contains("稳压装置"))
            {
                code = subproNum.AsString() + "FF01";
            }

            if (name.Contains("给排水") && name.Contains("水箱"))
            {
                code = subproNum.AsString() + "TN01";
            }

            if (name.Contains("给排水") && name.Contains("储气罐"))
            {
                code = subproNum.AsString() + "AT01";
            }

            if (name.Contains("给排水") && name.Contains("气压给水罐"))
            {
                code = subproNum.AsString() + "RN01";
            }

            if (name.Contains("给排水") && name.Contains("管道混合器"))
            {
                code = subproNum.AsString() + "MX01";
            }

            if (name.Contains("给排水") && name.Contains("给水处理设备"))
            {
                if (name.Contains("反渗透"))
                {
                    code = subproNum.AsString() + "RO01";
                }
                else
                {
                    code = subproNum.AsString() + "WT01";
                }         
            }

            if (name.Contains("给排水") && name.Contains("污水处理设备"))
            {
                code = subproNum.AsString() + "SW01";
            }

            if (name.Contains("给排水") && name.Contains("格栅"))
            {
                code = subproNum.AsString() + "GI01";
            }

            if (name.Contains("给排水") && name.Contains("罗茨风机"))
            {
                code = subproNum.AsString() + "BL01";
            }

            if (name.Contains("给排水") && name.Contains("阀门"))
            {
                code = subproNum.AsString() + "VA01";
            }

            equipment.get_Parameter(BuiltInParameter.DOOR_NUMBER).Set(code);
        }
        public bool CreatPipeAccessoryTagMethod(Document doc, UIDocument uidoc)
        {
            try
            {
                TagFamilyLoad(doc, "管道附件编号");
                Selection sel = uidoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element, new PipeAccessorySelectionFilter());
                FamilyInstance accessory = doc.GetElement(reference) as FamilyInstance;
                if (!accessory.IsTaged(doc))
                {
                    SetEquipmentCode(doc,accessory);
                }

                LocationPoint accessoryLocation = accessory.Location as LocationPoint;
                XYZ projectPickPoint = accessoryLocation.Point;

                IList<Element> accessoryTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessoryTags).ToElements();

                FamilySymbol accessoryTag = null;
                foreach (var item in accessoryTagsCollect)
                {
                    FamilySymbol etag = item as FamilySymbol;
                    Family ftag = etag.Family;
                    if (ftag.Name.Contains("管道附件") && ftag.Name.Contains("给排水"))
                    {
                        accessoryTag = etag;
                        break;
                    }
                }

                Reference pipeRef = new Reference(accessory);
                TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                TagOrientation tagOri = TagOrientation.Horizontal;
                IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, projectPickPoint);
                tag.ChangeTypeId(accessoryTag.Id);

                CreatPipeAccessoryTagMethod(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;

        }
        public bool CreatTextWithLineMethod(Document doc, UIDocument uidoc)
        {
            try
            {
                TextNoteType textNoteType = null;
                IList<TextNoteType> noteTypes = CollectorHelper.TCollector<TextNoteType>(doc);
                textNoteType = noteTypes.FirstOrDefault(x => x.Name.Contains("给排水") && x.Name.Contains("带线"));

                if (textNoteType == null)
                {
                    textNoteType = CreatTextWithLineType(doc);
                }


                Selection sel = uidoc.Selection;
                XYZ pt1 = sel.PickPoint("请选择带线文字箭头位置");
                XYZ pt2 = sel.PickPoint("请选择带线文字创建位置");

                //创建文字注释
                TextNote note = TextNote.Create(doc, doc.ActiveView.Id, pt2, CreatPipeTag.mainfrm.TextInputCmb.Text, textNoteType.Id);
                note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L); //引线方向，一共四种，左直，右直，左弧，右弧
                note.LeaderLeftAttachment = LeaderAtachement.TopLine;//引线的位置，top代表引线位置在第一行文本的位置

                //设置文字为上标
                FormattedText formatText = note.GetFormattedText();
                formatText.SetSuperscriptStatus(true);
                note.SetFormattedText(formatText);

                //设置箭头终点
                IList<Leader> leaderList = note.GetLeaders();
                foreach (Leader leader in leaderList)
                {
                    leader.End = pt1;
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;

        }
        public TextNoteType CreatTextWithLineType(Document doc)
        {
            IList<TextNoteType> noteTypes = CollectorHelper.TCollector<TextNoteType>(doc);
            TextNoteType existType = noteTypes.FirstOrDefault(x => x.Name.Contains("给排水"));

            TextNoteType duplicatedtextType = null;
            duplicatedtextType = existType.Duplicate("给排水-带线文字") as TextNoteType;

            duplicatedtextType.LookupParameter("文字大小").SetValueString("5.5mm");
            duplicatedtextType.LookupParameter("宽度系数").Set(0.70);
            duplicatedtextType.LookupParameter("文字字体").Set("Bahnschrift SemiLight");
            duplicatedtextType.LookupParameter("背景").Set(1);
            duplicatedtextType.LookupParameter("线宽").Set(1);
            duplicatedtextType.LookupParameter("下划线").Set(1);
            duplicatedtextType.LookupParameter("粗体").Set(0);
            duplicatedtextType.LookupParameter("斜体").Set(0);
            duplicatedtextType.LookupParameter("引线/边界偏移量").Set(0);
            duplicatedtextType.LookupParameter("引线箭头").Set(new ElementId(-1));//箭头设置为无的办法

            return duplicatedtextType;
        }
        public string ConnectorDirection(Pipe pipe)
        {
            string direction = null;
            ConnectorManager manager = pipe.ConnectorManager;
            ConnectorSet set = manager.UnusedConnectors;
            foreach (Connector item in set)
            {
                XYZ point = item.CoordinateSystem.BasisZ;
                if (point.X == -1)
                {
                    direction = "left";
                }
                if (point.X == 1)
                {
                    direction = "right";
                }
                if (point.Y == 1)
                {
                    direction = "up";
                }
                if (point.Y == -1)
                {
                    direction = "down";
                }
                break;
            }
            return direction;
        }
        public void TagFamilyLoad(Document doc, string categoryName)
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
        public string SleeveSize(string nominal_Diameter, int num) //套管D3和D2值
        {
            string sleeveSize = "114";

            if (num == 0)
            {
                if (nominal_Diameter == "15")
                {
                    sleeveSize = "114";
                }
                else if (nominal_Diameter == "20")
                {
                    sleeveSize = "114";
                }
                else if (nominal_Diameter == "25")
                {
                    sleeveSize = "114";
                }
                else if (nominal_Diameter == "32")
                {
                    sleeveSize = "114";
                }
                else if (nominal_Diameter == "40")
                {
                    sleeveSize = "114";
                }
                else if (nominal_Diameter == "50")
                {
                    sleeveSize = "114";
                }
                else if (nominal_Diameter == "65")
                {
                    sleeveSize = "121";
                }
                else if (nominal_Diameter == "80")
                {
                    sleeveSize = "140";
                }
                else if (nominal_Diameter == "100")
                {
                    sleeveSize = "159";
                }
                else if (nominal_Diameter == "125")
                {
                    sleeveSize = "180";
                }
                else if (nominal_Diameter == "150")
                {
                    sleeveSize = "219";
                }
                else if (nominal_Diameter == "200")
                {
                    sleeveSize = "273";
                }
                else if (nominal_Diameter == "250")
                {
                    sleeveSize = "325";
                }
                else if (nominal_Diameter == "300")
                {
                    sleeveSize = "377";
                }
                else if (nominal_Diameter == "350")
                {
                    sleeveSize = "426";
                }
                else if (nominal_Diameter == "400")
                {
                    sleeveSize = "480";
                }
                else if (nominal_Diameter == "450")
                {
                    sleeveSize = "530";
                }
                else if (nominal_Diameter == "500")
                {
                    sleeveSize = "590";
                }
                else if (nominal_Diameter == "600")
                {
                    sleeveSize = "690";
                }
                else if (nominal_Diameter == "700")
                {
                    sleeveSize = "790";
                }
                else if (nominal_Diameter == "800")
                {
                    sleeveSize = "880";
                }
                else if (nominal_Diameter == "900")
                {
                    sleeveSize = "980";
                }
                else if (nominal_Diameter == "1000")
                {
                    sleeveSize = "1080";
                }
            }

            if (num == 1)
            {
                if (nominal_Diameter == "15")
                {
                    sleeveSize = "95";
                }
                else if (nominal_Diameter == "20")
                {
                    sleeveSize = "95";
                }
                else if (nominal_Diameter == "25")
                {
                    sleeveSize = "95";
                }
                else if (nominal_Diameter == "32")
                {
                    sleeveSize = "95";
                }
                else if (nominal_Diameter == "40")
                {
                    sleeveSize = "95";
                }
                else if (nominal_Diameter == "50")
                {
                    sleeveSize = "95";
                }
                else if (nominal_Diameter == "65")
                {
                    sleeveSize = "114";
                }
                else if (nominal_Diameter == "80")
                {
                    sleeveSize = "127";
                }
                else if (nominal_Diameter == "100")
                {
                    sleeveSize = "146";
                }
                else if (nominal_Diameter == "125")
                {
                    sleeveSize = "180";
                }
                else if (nominal_Diameter == "150")
                {
                    sleeveSize = "203";
                }
                else if (nominal_Diameter == "200")
                {
                    sleeveSize = "265";
                }
                else if (nominal_Diameter == "250")
                {
                    sleeveSize = "325";
                }
                else if (nominal_Diameter == "300")
                {
                    sleeveSize = "377";
                }
                else if (nominal_Diameter == "350")
                {
                    sleeveSize = "426";
                }
                else if (nominal_Diameter == "400")
                {
                    sleeveSize = "480";
                }
                else if (nominal_Diameter == "450")
                {
                    sleeveSize = "530";
                }
                else if (nominal_Diameter == "500")
                {
                    sleeveSize = "585";
                }
                else if (nominal_Diameter == "600")
                {
                    sleeveSize = "690";
                }
                else if (nominal_Diameter == "700")
                {
                    sleeveSize = "780";
                }
                else if (nominal_Diameter == "800")
                {
                    sleeveSize = "880";
                }
                else if (nominal_Diameter == "900")
                {
                    sleeveSize = "980";
                }
                else if (nominal_Diameter == "1000")
                {
                    sleeveSize = "1080";
                }
            }

            return sleeveSize;
        }
        public string BaseHoleSize(string nominal_Diameter) //基础留洞尺寸
        {
            string baseHoleSize = "100X100";

            if (nominal_Diameter == "15")
            {
                baseHoleSize = "50X50";
            }
            else if (nominal_Diameter == "20")
            {
                baseHoleSize = "50X50";
            }
            else if (nominal_Diameter == "25")
            {
                baseHoleSize = "50X50";
            }
            else if (nominal_Diameter == "32")
            {
                baseHoleSize = "50X50";
            }
            else if (nominal_Diameter == "40")
            {
                baseHoleSize = "100X100";
            }
            else if (nominal_Diameter == "50")
            {
                baseHoleSize = "100X100";
            }
            else if (nominal_Diameter == "65")
            {
                baseHoleSize = "150X150";
            }
            else if (nominal_Diameter == "80")
            {
                baseHoleSize = "150X150";
            }
            else if (nominal_Diameter == "100")
            {
                baseHoleSize = "200X200";
            }
            else if (nominal_Diameter == "125")
            {
                baseHoleSize = "200X200";
            }
            else if (nominal_Diameter == "150")
            {
                baseHoleSize = "250X250";
            }
            else if (nominal_Diameter == "200")
            {
                baseHoleSize = "300X300";
            }
            else if (nominal_Diameter == "250")
            {
                baseHoleSize = "350X350";
            }
            else if (nominal_Diameter == "300")
            {
                baseHoleSize = "400X400";
            }
            else if (nominal_Diameter == "350")
            {
                baseHoleSize = "450X450";
            }
            else if (nominal_Diameter == "400")
            {
                baseHoleSize = "500X500";
            }
            else if (nominal_Diameter == "450")
            {
                baseHoleSize = "550X550";
            }
            else if (nominal_Diameter == "500")
            {
                baseHoleSize = "600X600";
            }
            else if (nominal_Diameter == "600")
            {
                baseHoleSize = "700X700";
            }
            else if (nominal_Diameter == "700")
            {
                baseHoleSize = "800X800";
            }

            return baseHoleSize;
        }
        public string FloorHoleSize(string nominal_Diameter) //楼板圆形留洞尺寸
        {
            string floorHoleSize = "Φ100";

            if (nominal_Diameter == "15")
            {
                floorHoleSize = "Φ50";
            }
            else if (nominal_Diameter == "20")
            {
                floorHoleSize = "Φ50";
            }
            else if (nominal_Diameter == "25")
            {
                floorHoleSize = "Φ50";
            }
            else if (nominal_Diameter == "32")
            {
                floorHoleSize = "Φ50";
            }
            else if (nominal_Diameter == "40")
            {
                floorHoleSize = "Φ100";
            }
            else if (nominal_Diameter == "50")
            {
                floorHoleSize = "Φ100";
            }
            else if (nominal_Diameter == "65")
            {
                floorHoleSize = "Φ150";
            }
            else if (nominal_Diameter == "80")
            {
                floorHoleSize = "Φ150";
            }
            else if (nominal_Diameter == "100")
            {
                floorHoleSize = "Φ200";
            }
            else if (nominal_Diameter == "125")
            {
                floorHoleSize = "Φ200";
            }
            else if (nominal_Diameter == "150")
            {
                floorHoleSize = "Φ250";
            }
            else if (nominal_Diameter == "200")
            {
                floorHoleSize = "Φ300";
            }
            else if (nominal_Diameter == "250")
            {
                floorHoleSize = "Φ350";
            }
            else if (nominal_Diameter == "300")
            {
                floorHoleSize = "Φ400";
            }
            else if (nominal_Diameter == "350")
            {
                floorHoleSize = "Φ450";
            }
            else if (nominal_Diameter == "400")
            {
                floorHoleSize = "Φ500";
            }
            else if (nominal_Diameter == "450")
            {
                floorHoleSize = "Φ550";
            }
            else if (nominal_Diameter == "500")
            {
                floorHoleSize = "Φ600";
            }
            else if (nominal_Diameter == "600")
            {
                floorHoleSize = "Φ700";
            }
            else if (nominal_Diameter == "700")
            {
                floorHoleSize = "Φ800";
            }

            return floorHoleSize;
        }
        public FamilySymbol NoteSymbol(Document doc, string symbolName)//获取注释类型
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
        public bool TwoValueEqual(double value1, double value2)
        {
            bool equal = false;
            if (Math.Abs(value2 - value1) < 0.1)
            {
                equal = true;
            }
            return equal;
        }
    }
    public static class Helper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        /// <summary>
        /// 发送键盘消息
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="key"></param>
        public static void SendKeys(IntPtr proc, Keys key)
        {
            SetActiveWindow(proc);
            SetForegroundWindow(proc);
            keybd_event(key, 0, 0, 0);
            keybd_event(key, 0, 2, 0);
            keybd_event(key, 0, 0, 0);
            keybd_event(key, 0, 2, 0);
        }
    }
}

