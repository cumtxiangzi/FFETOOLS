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
                        TaskDialog.Show("����", "�뽫��ά��ͼ�������ٽ��в���");
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
            return "�ܵ�����";
        }
        public void CreatTagMain(Document doc, UIDocument uidoc, UIApplication app)
        {
            //TransactionGroup tg = new TransactionGroup(doc, "��������ˮ��ע");
            //tg.Start();

            using (Transaction trans = new Transaction(doc, "����ˮ��ע"))
            {
                trans.Start();
                if (CreatPipeTag.mainfrm.clicked == 1)
                {
                    CreatEquipmentTagMethod(doc, uidoc);
                }

                if (CreatPipeTag.mainfrm.clicked == 2)
                {
                    CreatPipeTagMethod(doc, uidoc, "�ܵ�����ֱ��");
                }

                if (CreatPipeTag.mainfrm.clicked == 3)
                {
                    CreatPipeTagMethod(doc, uidoc, "�ܵ�ϵͳ��д");
                }

                if (CreatPipeTag.mainfrm.clicked == 4)
                {
                    CreatPipeTagMethod(doc, uidoc, "�������ܱ��");
                }

                if (CreatPipeTag.mainfrm.clicked == 9)
                {
                    BatchNote(doc, uidoc);
                }

                if (CreatPipeTag.mainfrm.clicked == 10)
                {
                    CreatPipeAccessoryTagMethod(doc, uidoc);
                }

                if (CreatPipeTag.mainfrm.clicked == 24)
                {
                    AlignNote(doc, uidoc);
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
                CreatPipeTagWithLine(doc, uidoc, "���ܱ��");
            }

            if (CreatPipeTag.mainfrm.clicked == 6)
            {
                CreatCommonNote(doc, uidoc, "�ܵ���������");
            }

            if (CreatPipeTag.mainfrm.clicked == 7)
            {
                CreatSleeveNote(doc, uidoc, "���Է�ˮ�׹�");
            }

            if (CreatPipeTag.mainfrm.clicked == 8)
            {
                CreatSleeveNote(doc, uidoc, "���Է�ˮ�׹�");
            }

            if (CreatPipeTag.mainfrm.clicked == 11)
            {
                CreatCommonNote(doc, uidoc, "�ܵ�¥����������");
            }

            if (CreatPipeTag.mainfrm.clicked == 12)
            {
                CreatCommonNote(doc, uidoc, "�ܵ�Ԥ����");
            }

            if (CreatPipeTag.mainfrm.clicked == 13)
            {
                CreatCommonNote(doc, uidoc, "���ݱ�ע");
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
                CreatCommonNote(doc, uidoc, "������ע");
            }

            if (CreatPipeTag.mainfrm.clicked == 17)
            {
                CreatCommonNote(doc, uidoc, "������ע");
            }

            if (CreatPipeTag.mainfrm.clicked == 18)
            {
                CreatCommonNote(doc, uidoc, "����˨��������ע");
            }

            if (CreatPipeTag.mainfrm.clicked == 19)
            {
                CreatSupportSectionNote(doc, uidoc);
            }

            if (CreatPipeTag.mainfrm.clicked == 20)
            {
                CreatCommonNote(doc, uidoc, "���Ԥ����ע");
            }

            if (CreatPipeTag.mainfrm.clicked == 21)
            {
                CreatCommonNote(doc, uidoc, "�����׹���ĸ��ע");
            }

            if (CreatPipeTag.mainfrm.clicked == 22)
            {
                CreatCommonNote(doc, uidoc, "�����׹���ĸ��ע");
            }

            if (CreatPipeTag.mainfrm.clicked == 23)
            {
                CreatCommonNote(doc, uidoc, "�ܵ�¥������Բ��");
            }

            if (CreatPipeTag.mainfrm.clicked == 25)
            {
                CreatElevationNote(doc, uidoc);
            }

            if (CreatPipeTag.mainfrm.clicked == 26)
            {
                CreatCommonNote(doc, uidoc, "Ԥ������ĸ��ע");
            }
            //tg.Assimilate();
        }
        public bool CreatElevationNote(Document doc, UIDocument uidoc)//�������
        {
            try
            {
                SpotDimensionType type = null;
                using (Transaction trans = new Transaction(doc, "����ˮ���"))
                {
                    trans.Start();

                    type = CollectorHelper.TCollector<SpotDimensionType>(doc).FirstOrDefault(x => x.Name == "����ˮ�߳�");

                    if (type.get_Parameter(BuiltInParameter.SPOT_TEXT_FROM_LEADER).AsDouble() == 3.0 / 304.8)
                    {
                        type.get_Parameter(BuiltInParameter.SPOT_TEXT_FROM_LEADER).Set(3.5 / 304.8);
                    }

                    trans.Commit();
                }
                uidoc.PostRequestForElementTypePlacement(type);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;
        }
        public bool CreatSupportSectionNote(Document doc, UIDocument uidoc)
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "�����ܵ�֧�������ע"))
                {
                    trans.Start();

                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new DetailSignSelectionFilter(), "��ѡ������ܵ�");
                    FamilyInstance ladder = doc.GetElement(reference) as FamilyInstance;
                    LocationPoint ladderLocation = ladder.Location as LocationPoint;
                    string dn = (Convert.ToDouble(ladder.LookupParameter("�ܵ��뾶").AsValueString()) * 2).ToString();
                    int hasInsulation = ladder.LookupParameter("�ܵ�����").AsInteger();

                    XYZ pt1 = ladderLocation.Point;
                    XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                    FamilySymbol boxSymbol = null;
                    AnnotationSymbol boxNote = null;

                    TagFamilyLoad(doc, "֧������ܵ���ע");
                    boxSymbol = NoteSymbol(doc, "֧������ܵ���ע");
                    boxSymbol.Activate();
                    boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                    boxNote.addLeader();
                    IList<Leader> leadList = boxNote.GetLeaders();
                    Leader lead = leadList[0];
                    lead.End = pt1;

                    boxNote.LookupParameter("�ܵ����ͼ��ߴ�").Set("XJ-DN" + dn);
                    boxNote.LookupParameter("�ܵ�����").Set(PipeWeight("DN" + dn, hasInsulation));

                    trans.Commit();
                }

                CreatSupportSectionNote(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;
        }
        public void BatchNote(Document doc, UIDocument uidoc)//������ע��ʵ��
        {
            IList<FamilyInstance> waterInstance = CollectorHelper.TCollector<FamilyInstance>(doc, uidoc);

            foreach (var item in waterInstance)
            {
                if (item.Category.Name == "��е�豸" && item.Symbol.FamilyName.Contains("����ˮ") && !item.Symbol.FamilyName.Contains("�����豸")
                    && !item.Symbol.FamilyName.Contains("ͨ����") && !item.Symbol.FamilyName.Contains("ͨ���"))
                {
                    if (!item.IsTaged(doc))
                    {
                        SetEquipmentCode(doc, item);
                    }
                    LocationPoint equipmentLocation = item.Location as LocationPoint;
                    XYZ projectPickPoint = equipmentLocation.Point;

                    IList<Element> equipmentTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipmentTags).ToElements();
                    FamilySymbol equipmentTag = null;
                    foreach (var tagItem in equipmentTagsCollect)
                    {
                        FamilySymbol etag = tagItem as FamilySymbol;
                        Family ftag = etag.Family;
                        if (ftag.Name.Contains("�豸���") && ftag.Name.Contains("����ˮ"))
                        {
                            equipmentTag = etag;
                            break;
                        }
                    }

                    Reference pipeRef = new Reference(item);
                    TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                    TagOrientation tagOri = TagOrientation.Horizontal;
                    IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, projectPickPoint);
                    tag.ChangeTypeId(equipmentTag.Id);
                }

                if (item.Category.Name == "�ܵ�����" && item.Symbol.FamilyName.Contains("����ˮ") && !item.Symbol.FamilyName.Contains("��ˮ�豸����")
                    && !item.Symbol.FamilyName.Contains("��ˮ�豸����") && !item.Symbol.FamilyName.Contains("�����豸"))
                {
                    if (!item.IsTaged(doc))
                    {
                        SetEquipmentCode(doc, item);
                    }

                    LocationPoint accessoryLocation = item.Location as LocationPoint;
                    XYZ projectPickPoint = accessoryLocation.Point;

                    IList<Element> accessoryTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessoryTags).ToElements();

                    FamilySymbol accessoryTag = null;
                    foreach (var tagItem in accessoryTagsCollect)
                    {
                        FamilySymbol etag = tagItem as FamilySymbol;
                        Family ftag = etag.Family;
                        if (ftag.Name.Contains("�ܵ�����") && ftag.Name.Contains("����ˮ"))
                        {
                            accessoryTag = etag;
                            break;
                        }
                    }

                    Reference pipeRef = new Reference(item);
                    TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                    TagOrientation tagOri = TagOrientation.Horizontal;
                    IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, projectPickPoint);
                    tag.ChangeTypeId(accessoryTag.Id);
                }
            }

        }
        #region
        public void AlignNote(Document doc, UIDocument uidoc)//�豸��Ŷ����ע
        {
            PickedBox pickBox = null;
            pickBox = uidoc.Selection.PickBox(PickBoxStyle.Crossing, "��ѡ����Ҫ������豸���");

            XYZ maxPoint = pickBox.Max;
            XYZ minPoint = pickBox.Min;
            //��ֵ
            double minX = Math.Min(minPoint.X, maxPoint.X);
            double maxX = Math.Max(minPoint.X, maxPoint.X);
            double minY = Math.Min(minPoint.Y, maxPoint.Y);
            double maxY = Math.Max(minPoint.Y, maxPoint.Y);
            List<XYZ> pointList = TwoPointGetPointList(new XYZ(minX, minY, 0), new XYZ(maxX, maxY, 0));
            List<IndependentTag> tagList = new FilteredElementCollector(doc, uidoc.ActiveGraphicalView.Id).OfClass(typeof(IndependentTag)).OfType<IndependentTag>().ToList();

            List<IndependentTag> tagInBoxList = new List<IndependentTag>();
            foreach (var tag in tagList)
            {
                XYZ point = tag.TagHeadPosition;
                if (IsInPolygon(point.SetZ(), pointList))
                {
                    tagInBoxList.Add(tag);
                }
            }

            tagInBoxList.Sort((a, b) => a.TagHeadPosition.X.CompareTo(b.TagHeadPosition.X));//����         
            foreach (var item in tagInBoxList)
            {
                XYZ newpoint = new XYZ();
                XYZ instancePoint = ((item.GetTaggedLocalElement() as FamilyInstance).Location as LocationPoint).Point;

                if (item.TagHeadPosition.IsSameDirection(tagInBoxList[0].TagHeadPosition))
                {
                    newpoint = tagInBoxList[0].TagHeadPosition;
                }
                else
                {
                    newpoint = new XYZ(instancePoint.X-270/304.8, tagInBoxList[0].TagHeadPosition.Y, item.TagHeadPosition.Z);
                }
                item.TagHeadPosition = newpoint;
            }
        }
        public List<XYZ> TwoPointGetPointList(XYZ minPoint, XYZ maxPoint)
        {
            List<XYZ> result = new List<XYZ>();
            try
            {
                XYZ p1 = minPoint;
                XYZ p3 = maxPoint;
                XYZ p2 = new XYZ(maxPoint.X, minPoint.Y, 0);
                XYZ p4 = new XYZ(minPoint.X, maxPoint.Y, 0);
                result.Add(p1);
                result.Add(p2);
                result.Add(p3);
                result.Add(p4);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("��ѡ������С���޷��б�");
                result = new List<XYZ>();
            }
            return result;
        }
        public bool IsInPolygon(XYZ checkPoint, List<XYZ> polygonPoints)
        {
            bool inSide = false;
            int pointCount = polygonPoints.Count;
            XYZ p1, p2;
            for (int i = 0, j = pointCount - 1;
                i < pointCount;
                j = i, i++)
            {
                p1 = polygonPoints[i];
                p2 = polygonPoints[j];
                if (checkPoint.Y < p2.Y)
                {
                    if (p1.Y <= checkPoint.Y)
                    {
                        if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) > (checkPoint.X - p1.X) * (p2.Y - p1.Y)
                        )
                        {
                            inSide = (!inSide);
                        }
                    }
                }
                else if (checkPoint.Y < p1.Y)
                {
                    if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) < (checkPoint.X - p1.X) * (p2.Y - p1.Y)
                    )
                    {
                        inSide = (!inSide);
                    }
                }
            }

            return inSide;
        }
        #endregion
        public bool CreatCommonNote(Document doc, UIDocument uidoc, string name)//����ͨ�ñ�ע
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "����ͨ�ñ�ע"))
                {
                    trans.Start();
                    string LanguageVer = CreatPipeTag.mainfrm.LanguageCmb.SelectedItem.ToString();

                    if (name == "���ݱ�ע")
                    {
                        TagFamilyLoad(doc, "���ӱ�ע");
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new GenericModelSelectionFilter(), "��ѡ������");
                        FamilyInstance ladder = doc.GetElement(reference) as FamilyInstance;
                        LocationPoint ladderLocation = ladder.Location as LocationPoint;

                        XYZ pt1 = ladderLocation.Point;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                        FamilySymbol ladderSymbol = null;
                        AnnotationSymbol ladderNote = null;
                        ladderSymbol = NoteSymbol(doc, "���ӱ�ע");
                        ladderSymbol.Activate();
                        ladderNote = doc.Create.NewFamilyInstance(pt2, ladderSymbol, doc.ActiveView) as AnnotationSymbol;
                        ladderNote.addLeader();
                        IList<Leader> leadList = ladderNote.GetLeaders();
                        Leader lead = leadList[0];
                        lead.End = pt1;
                    }

                    if (name == "������ע")
                    {
                        Selection sel = uidoc.Selection;
                        XYZ pt1 = sel.PickPoint("��ѡ������λ��");
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                        FamilySymbol boxSymbol = null;
                        AnnotationSymbol boxNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "Ǳˮ�õ�����ע");
                            boxSymbol = NoteSymbol(doc, "Ǳˮ�õ�����ע");
                            boxSymbol.Activate();
                            boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                            boxNote.addLeader();
                            IList<Leader> leadList = boxNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "Ǳˮ�õ����Ӣ��ע��");
                            boxSymbol = NoteSymbol(doc, "Ǳˮ�õ����Ӣ��ע��");
                            boxSymbol.Activate();
                            boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                            boxNote.addLeader();
                            IList<Leader> leadList = boxNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                        }
                    }

                    if (name == "������ע")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new GenericSignSelectionFilter(), "��ѡ�����ʮ�ַ���");
                        AnnotationSymbol ring = doc.GetElement(reference) as AnnotationSymbol;
                        LocationPoint ringLocation = ring.Location as LocationPoint;

                        Reference reference1 = sel.PickObject(ObjectType.Element, new GenericSignSelectionFilter(), "��ѡ�����ʮ�ַ���");
                        AnnotationSymbol ring1 = doc.GetElement(reference1) as AnnotationSymbol;
                        LocationPoint ringLocation1 = ring1.Location as LocationPoint;

                        XYZ pt1 = ringLocation.Point;
                        XYZ pt2 = ringLocation1.Point;
                        XYZ pt3 = sel.PickPoint("��ѡ���ע����λ��");

                        FamilySymbol ringSymbol = null;
                        AnnotationSymbol ringNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "������ע");
                            ringSymbol = NoteSymbol(doc, "������ע");
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

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "����Ӣ��ע��");
                            ringSymbol = NoteSymbol(doc, "����Ӣ��ע��");
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

                    if (name == "���Ԥ����ע")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new DetailineSelectionFilter(), "��ѡ��Ԥ��ְ���");
                        DetailLine steel = doc.GetElement(reference) as DetailLine;
                        LocationCurve steelLocationCurve = steel.Location as LocationCurve;
                        string steelLength = steel.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = steelLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                        FamilySymbol steelSymbol = null;
                        AnnotationSymbol steelNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "���Ԥ��ְ��ע");
                            steelSymbol = NoteSymbol(doc, "���Ԥ��ְ��ע");
                            steelSymbol.Activate();
                            steelNote = doc.Create.NewFamilyInstance(pt2, steelSymbol, doc.ActiveView) as AnnotationSymbol;
                            steelNote.addLeader();
                            IList<Leader> leadList = steelNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            steelNote.LookupParameter("��������").Set(steelLength.ToString() + "X100X10mm�ְ�");
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "���Ԥ��ְ�Ӣ��ע��");
                            steelSymbol = NoteSymbol(doc, "���Ԥ��ְ�Ӣ��ע��");
                            steelSymbol.Activate();
                            steelNote = doc.Create.NewFamilyInstance(pt2, steelSymbol, doc.ActiveView) as AnnotationSymbol;
                            steelNote.addLeader();
                            IList<Leader> leadList = steelNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            steelNote.LookupParameter("��������").Set(steelLength.ToString() + "X100X10mm");
                        }

                    }

                    if (name == "����˨��������ע")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new MechanicalSelectionFilter(), "��ѡ������˨��");
                        FamilyInstance box = doc.GetElement(reference) as FamilyInstance;
                        LocationPoint boxLocation = box.Location as LocationPoint;

                        XYZ pt1 = boxLocation.Point;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                        FamilySymbol boxSymbol = null;
                        AnnotationSymbol boxNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "����˨Ԥ������ע");
                            boxSymbol = NoteSymbol(doc, "����˨Ԥ������ע");
                            boxSymbol.Activate();
                            boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                            boxNote.addLeader();
                            IList<Leader> leadList = boxNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "����˨Ԥ����Ӣ��ע��");
                            boxSymbol = NoteSymbol(doc, "����˨Ԥ����Ӣ��ע��");
                            boxSymbol.Activate();
                            boxNote = doc.Create.NewFamilyInstance(pt2, boxSymbol, doc.ActiveView) as AnnotationSymbol;
                            boxNote.addLeader();
                            IList<Leader> leadList = boxNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                        }
                    }

                    if (name == "�ܵ���������")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "��ѡ��ܵ�");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol baseHoleSymbol = null;
                        AnnotationSymbol baseHoleNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "�ܵ�����������ע");
                            baseHoleSymbol = NoteSymbol(doc, "�ܵ�����������ע");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("�����ߴ�").Set(BaseHoleSize(pipeSize));
                            baseHoleNote.LookupParameter("�����ı��").Set(d.ToString("0.000"));
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "�ܵ���������Ӣ��ע��");
                            baseHoleSymbol = NoteSymbol(doc, "�ܵ���������Ӣ��ע��");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("�����ߴ�").Set(BaseHoleSize(pipeSize));
                            baseHoleNote.LookupParameter("�����ı��").Set(d.ToString("0.000"));
                        }
                    }

                    if (name == "�ܵ�Ԥ����")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "��ѡ��ܵ�");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol baseHoleSymbol = null;
                        AnnotationSymbol baseHoleNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "�ܵ�Ԥ������ע");
                            baseHoleSymbol = NoteSymbol(doc, "�ܵ�Ԥ������ע");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("�����ߴ�").Set(BaseHoleSize(pipeSize));
                            baseHoleNote.LookupParameter("�����ı��").Set(d.ToString("0.000"));
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "�ܵ�Ԥ����Ӣ��ע��");
                            baseHoleSymbol = NoteSymbol(doc, "�ܵ�Ԥ����Ӣ��ע��");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("�����ߴ�").Set(BaseHoleSize(pipeSize));
                            baseHoleNote.LookupParameter("�����ı��").Set(d.ToString("0.000"));
                        }
                    }

                    if (name == "�ܵ�¥����������")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "��ѡ��ܵ�");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol baseHoleSymbol = null;
                        AnnotationSymbol baseHoleNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "�ܵ�¥��������ע");
                            baseHoleSymbol = NoteSymbol(doc, "�ܵ�¥��������ע");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("�����ߴ�").Set(BaseHoleSize(pipeSize));
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "�ܵ�¥������Ӣ��ע��");
                            baseHoleSymbol = NoteSymbol(doc, "�ܵ�¥������Ӣ��ע��");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("�����ߴ�").Set(BaseHoleSize(pipeSize));
                        }
                    }

                    if (name == "�ܵ�¥������Բ��")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "��ѡ��ܵ�");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol baseHoleSymbol = null;
                        AnnotationSymbol baseHoleNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "�ܵ�¥��������ע");
                            baseHoleSymbol = NoteSymbol(doc, "�ܵ�¥��������ע");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("�����ߴ�").Set(FloorHoleSize(pipeSize));
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "�ܵ�¥������Ӣ��ע��");
                            baseHoleSymbol = NoteSymbol(doc, "�ܵ�¥������Ӣ��ע��");
                            baseHoleSymbol.Activate();
                            baseHoleNote = doc.Create.NewFamilyInstance(pt2, baseHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                            baseHoleNote.addLeader();
                            IList<Leader> leadList = baseHoleNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            baseHoleNote.LookupParameter("�����ߴ�").Set(FloorHoleSize(pipeSize));
                        }

                    }

                    if (name == "�����׹���ĸ��ע")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "��ѡ��ܵ�");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();
                        double verticalPipeZ = (pipeLocationCurve.Curve as Line).Direction.Z;

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");
                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol sleeveSymbol = null;
                        AnnotationSymbol sleeveNote = null;

                        TagFamilyLoad(doc, "�׹���ĸ��ע");
                        sleeveSymbol = NoteSymbol(doc, "�׹���ĸ��ע");
                        sleeveSymbol.Activate();
                        sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                        sleeveNote.addLeader();
                        IList<Leader> leadList = sleeveNote.GetLeaders();
                        Leader lead = leadList[0];
                        lead.End = projectPickPoint;
                        sleeveNote.LookupParameter("�ܵ�ֱ��").Set("DN" + pipeSize);
                        sleeveNote.LookupParameter("�����ı��").Set(d.ToString("0.000"));
                        sleeveNote.LookupParameter("�׹�ֱ��").Set("D3=" + SleeveSize(pipeSize, 0));
                        sleeveNote.LookupParameter("Ԥ����").Set("");
                        sleeveNote.LookupParameter("��ע").Set("���Է�ˮ�׹�,���02S404-15");
                        sleeveNote.LookupParameter("�Ƿ�Ϊ�׹�").Set(1);

                        if (verticalPipeZ == 1 || verticalPipeZ == -1)
                        {
                            sleeveNote.LookupParameter("�����ı��").Set("�ض�");
                        }
                    }

                    if (name == "�����׹���ĸ��ע")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "��ѡ��ܵ�");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();
                        double verticalPipeZ = (pipeLocationCurve.Curve as Line).Direction.Z;

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");
                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol sleeveSymbol = null;
                        AnnotationSymbol sleeveNote = null;

                        TagFamilyLoad(doc, "�׹���ĸ��ע");
                        sleeveSymbol = NoteSymbol(doc, "�׹���ĸ��ע");
                        sleeveSymbol.Activate();
                        sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                        sleeveNote.addLeader();
                        IList<Leader> leadList = sleeveNote.GetLeaders();
                        Leader lead = leadList[0];
                        lead.End = projectPickPoint;
                        sleeveNote.LookupParameter("�ܵ�ֱ��").Set("DN" + pipeSize);
                        sleeveNote.LookupParameter("�����ı��").Set(d.ToString("0.000"));
                        sleeveNote.LookupParameter("�׹�ֱ��").Set("D2=" + SleeveSize(pipeSize, 1));
                        sleeveNote.LookupParameter("Ԥ����").Set("");
                        sleeveNote.LookupParameter("��ע").Set("���Է�ˮ�׹�,���02S404-5");
                        sleeveNote.LookupParameter("�Ƿ�Ϊ�׹�").Set(1);

                        if (verticalPipeZ == 1 || verticalPipeZ == -1)
                        {
                            sleeveNote.LookupParameter("�����ı��").Set("�ض�");
                        }
                    }

                    if (name == "Ԥ������ĸ��ע")
                    {
                        Selection sel = uidoc.Selection;
                        Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "��ѡ��ܵ�");
                        Pipe pipe = doc.GetElement(reference) as Pipe;
                        LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                        string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();
                        double verticalPipeZ = (pipeLocationCurve.Curve as Line).Direction.Z;

                        XYZ pickPoint = reference.GlobalPoint;
                        XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                        XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");
                        double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                        FamilySymbol sleeveSymbol = null;
                        AnnotationSymbol sleeveNote = null;

                        TagFamilyLoad(doc, "�׹���ĸ��ע");
                        sleeveSymbol = NoteSymbol(doc, "�׹���ĸ��ע");
                        sleeveSymbol.Activate();
                        sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                        sleeveNote.addLeader();
                        IList<Leader> leadList = sleeveNote.GetLeaders();
                        Leader lead = leadList[0];
                        lead.End = projectPickPoint;
                        sleeveNote.LookupParameter("�ܵ�ֱ��").Set("DN" + pipeSize);
                        sleeveNote.LookupParameter("�����ı��").Set(d.ToString("0.000"));
                        sleeveNote.LookupParameter("�׹�ֱ��").Set("");
                        sleeveNote.LookupParameter("Ԥ����").Set(BaseHoleSize(pipeSize));
                        sleeveNote.LookupParameter("��ע").Set("");
                        sleeveNote.LookupParameter("�Ƿ�Ϊ�׹�").Set(0);

                        if (verticalPipeZ == 1 || verticalPipeZ == -1)
                        {
                            sleeveNote.LookupParameter("�����ı��").Set("");
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
        public bool CreatManHoleNote(Document doc, UIDocument uidoc)//�����˿ױ�ע
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "�����˿ױ�ע"))
                {
                    trans.Start();

                    TagFamilyLoad(doc, "���޿ױ�ע");
                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new GenericModelSelectionFilter(), "��ѡ���˿�");
                    FamilyInstance manHole = doc.GetElement(reference) as FamilyInstance;
                    LocationPoint manHoleLocation = manHole.Location as LocationPoint;
                    string manHoleSize = manHole.LookupParameter("�˿װ뾶").AsValueString();
                    double manHoleDN = double.Parse(manHoleSize) * 2;

                    XYZ pt1 = manHoleLocation.Point;
                    XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                    FamilySymbol manHoleSymbol = null;
                    AnnotationSymbol manHoleNote = null;
                    manHoleSymbol = NoteSymbol(doc, "���޿ױ�ע");
                    manHoleSymbol.Activate();
                    manHoleNote = doc.Create.NewFamilyInstance(pt2, manHoleSymbol, doc.ActiveView) as AnnotationSymbol;
                    manHoleNote.addLeader();
                    IList<Leader> leadList = manHoleNote.GetLeaders();
                    Leader lead = leadList[0];
                    lead.End = pt1;
                    manHoleNote.LookupParameter("��������").Set("��" + manHoleDN.ToString() + " ���޿�");

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
                using (Transaction trans = new Transaction(doc, "����ͨ���ܱ�ע"))
                {
                    trans.Start();
                    string LanguageVer = CreatPipeTag.mainfrm.LanguageCmb.SelectedItem.ToString();

                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new MechanicalSelectionFilter(), "��ѡ��ͨ����");
                    FamilyInstance ventPipe = doc.GetElement(reference) as FamilyInstance;
                    LocationPoint ventPipeLocation = ventPipe.Location as LocationPoint;
                    string ventPipeHeight = ventPipe.LookupParameter("ͨ���ܶ����߶�").AsValueString();
                    double ventHeight = double.Parse(ventPipeHeight);

                    XYZ pt1 = ventPipeLocation.Point;
                    XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");

                    if (ventHeight == 900 || ventHeight == 1400)
                    {
                        FamilySymbol ventSymbol = null;
                        AnnotationSymbol ventNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "ͨ���ܱ�ע");
                            ventSymbol = NoteSymbol(doc, "ͨ���ܱ�ע");
                            ventSymbol.Activate();
                            ventNote = doc.Create.NewFamilyInstance(pt2, ventSymbol, doc.ActiveView) as AnnotationSymbol;
                            ventNote.addLeader();
                            IList<Leader> leadList = ventNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                            ventNote.LookupParameter("��������").Set("ͨ����DN200 �߳��ض�" + ventPipeHeight + "mm");
                            ventNote.LookupParameter("��������").Set("���02S403,98");
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "ͨ����Ӣ��ע��");
                            ventSymbol = NoteSymbol(doc, "ͨ����Ӣ��ע��");
                            ventSymbol.Activate();
                            ventNote = doc.Create.NewFamilyInstance(pt2, ventSymbol, doc.ActiveView) as AnnotationSymbol;
                            ventNote.addLeader();
                            IList<Leader> leadList = ventNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;
                            ventNote.LookupParameter("��������").Set("VENT PIPE " + ventPipeHeight + "mm " + "HIGHER");
                            ventNote.LookupParameter("��������").Set("THAN TOP OF TANK");
                        }

                    }

                    if (ventHeight != 900 && ventHeight != 1400)
                    {
                        double d = UnitUtils.Convert(pt1.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);

                        FamilySymbol ventSymbol = null;
                        AnnotationSymbol ventNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "ͨ���ܱ�ע");
                            ventSymbol = NoteSymbol(doc, "ͨ���ܱ�ע");
                            ventSymbol.Activate();
                            ventNote = doc.Create.NewFamilyInstance(pt2, ventSymbol, doc.ActiveView) as AnnotationSymbol;
                            ventNote.addLeader();
                            IList<Leader> leadList = ventNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;

                            double ventRealHeight = ventHeight - (d + 200) * (-1);
                            if (TwoValueEqual(ventRealHeight, 900))
                            {
                                ventNote.LookupParameter("��������").Set("ͨ����DN200 �߳�����900mm");
                                ventNote.LookupParameter("��������").Set("���02S403,98");
                            }

                            if (TwoValueEqual(ventRealHeight, 1400))
                            {
                                ventNote.LookupParameter("��������").Set("ͨ����DN200 �߳�����1400mm");
                                ventNote.LookupParameter("��������").Set("���02S403,98");
                            }
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "ͨ����Ӣ��ע��");
                            ventSymbol = NoteSymbol(doc, "ͨ����Ӣ��ע��");
                            ventSymbol.Activate();
                            ventNote = doc.Create.NewFamilyInstance(pt2, ventSymbol, doc.ActiveView) as AnnotationSymbol;
                            ventNote.addLeader();
                            IList<Leader> leadList = ventNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = pt1;

                            double ventRealHeight = ventHeight - (d + 200) * (-1);
                            if (TwoValueEqual(ventRealHeight, 900))
                            {
                                ventNote.LookupParameter("��������").Set("VENT PIPE 900mm HIGHER");
                                ventNote.LookupParameter("��������").Set("THAN TOP OF COVER SOIL");
                            }

                            if (TwoValueEqual(ventRealHeight, 1400))
                            {
                                ventNote.LookupParameter("��������").Set("VENT PIPE 1400mm HIGHER");
                                ventNote.LookupParameter("��������").Set("THAN TOP OF COVER SOIL");
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
                using (Transaction trans = new Transaction(doc, "�����׹ܱ�ע"))
                {
                    trans.Start();

                    string LanguageVer = CreatPipeTag.mainfrm.LanguageCmb.SelectedItem.ToString();

                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter(), "��ѡ��ܵ�");
                    Pipe pipe = doc.GetElement(reference) as Pipe;
                    LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;
                    string pipeSize = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsValueString();
                    double verticalPipeZ = (pipeLocationCurve.Curve as Line).Direction.Z;

                    XYZ pickPoint = reference.GlobalPoint;
                    XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;
                    XYZ pt2 = sel.PickPoint("��ѡ���ע����λ��");
                    double d = UnitUtils.Convert(projectPickPoint.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);

                    if (sleeveName == "���Է�ˮ�׹�")
                    {
                        FamilySymbol sleeveSymbol = null;
                        AnnotationSymbol sleeveNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "���Է�ˮ�׹ܱ�ע");
                            sleeveSymbol = NoteSymbol(doc, "���Է�ˮ�׹ܱ�ע");
                            sleeveSymbol.Activate();
                            sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                            sleeveNote.addLeader();
                            IList<Leader> leadList = sleeveNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            sleeveNote.LookupParameter("��������").Set("Ԥ����Է�ˮ�׹�(A)��D3=" + SleeveSize(pipeSize, 0) + "mm");
                            sleeveNote.LookupParameter("�ر��׹���������").Set("�׹����ı��" + d.ToString("0.000"));

                            if (verticalPipeZ == 1 || verticalPipeZ == -1)
                            {
                                sleeveNote.LookupParameter("�ر��׹�").Set(0);
                            }
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            sleeveSymbol = NoteSymbol(doc, "��ˮ�׹�Ӣ��ע��");
                            sleeveSymbol.Activate();
                            sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                            sleeveNote.addLeader();
                            IList<Leader> leadList = sleeveNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            sleeveNote.LookupParameter("��������").Set("EMBEDDED WATER-PROOF SLEEVE D3=" + SleeveSize(pipeSize, 0) + "mm");
                            sleeveNote.LookupParameter("�ر��׹�����").Set("CENTER ELEVATION " + d.ToString("0.000"));

                            if (verticalPipeZ == 1 || verticalPipeZ == -1)
                            {
                                sleeveNote.LookupParameter("�ر��׹�").Set(0);
                            }
                        }
                    }

                    if (sleeveName == "���Է�ˮ�׹�")
                    {
                        FamilySymbol sleeveSymbol = null;
                        AnnotationSymbol sleeveNote = null;

                        if (LanguageVer == "����")
                        {
                            TagFamilyLoad(doc, "���Է�ˮ�׹ܱ�ע");
                            sleeveSymbol = NoteSymbol(doc, "���Է�ˮ�׹ܱ�ע");
                            sleeveSymbol.Activate();
                            sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                            sleeveNote.addLeader();
                            IList<Leader> leadList = sleeveNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            sleeveNote.LookupParameter("��������").Set("Ԥ�����Է�ˮ�׹�(A)��D2=" + SleeveSize(pipeSize, 1) + "mm");
                            sleeveNote.LookupParameter("�ر��׹���������").Set("�׹����ı��" + d.ToString("0.000"));

                            if (verticalPipeZ == 1 || verticalPipeZ == -1)
                            {
                                sleeveNote.LookupParameter("�ر��׹�").Set(0);
                            }
                        }

                        if (LanguageVer == "Ӣ��")
                        {
                            TagFamilyLoad(doc, "��ˮ�׹�Ӣ��ע��");
                            sleeveSymbol = NoteSymbol(doc, "��ˮ�׹�Ӣ��ע��");
                            sleeveSymbol.Activate();
                            sleeveNote = doc.Create.NewFamilyInstance(pt2, sleeveSymbol, doc.ActiveView) as AnnotationSymbol;
                            sleeveNote.addLeader();
                            IList<Leader> leadList = sleeveNote.GetLeaders();
                            Leader lead = leadList[0];
                            lead.End = projectPickPoint;
                            sleeveNote.LookupParameter("��������").Set("EMBEDDED WATER-PROOF SLEEVE D2=" + SleeveSize(pipeSize, 1) + "mm");
                            sleeveNote.LookupParameter("�ر��׹�����").Set("CENTER ELEVATION " + d.ToString("0.000"));

                            if (verticalPipeZ == 1 || verticalPipeZ == -1)
                            {
                                sleeveNote.LookupParameter("�ر��׹�").Set(0);
                            }
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
                using (Transaction trans = new Transaction(doc, "�������ܱ�ע"))
                {
                    trans.Start();
                    TagFamilyLoad(doc, "���ܱ��");
                    Selection sel = uidoc.Selection;
                    Reference reference = sel.PickObject(ObjectType.Element, new PipeSelectionFilter());
                    Pipe pipe = doc.GetElement(reference) as Pipe;
                    LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;

                    if (!pipe.IsTaged(doc))
                    {
                        string systemName = pipe.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM).AsString();
                        pipe.get_Parameter(BuiltInParameter.DOOR_NUMBER).Set(systemName + "L1");
                    }

                    XYZ pickPoint = reference.GlobalPoint;
                    XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;

                    IList<Element> pipetagscollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                    FamilySymbol pipeTag = null;
                    Family pipeCodeTag = null;
                    foreach (var item in pipetagscollect)
                    {
                        FamilySymbol pipetag = item as FamilySymbol;
                        Family ftag = pipetag.Family;
                        if (ftag.Name.Contains(tagName) && ftag.Name.Contains("����ˮ"))
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

                    FailureHandlingOptions failureOptions = trans.GetFailureHandlingOptions();
                    failureOptions.SetFailuresPreprocessor(new HidePasteDuplicateTypesPreprocessor());
                    trans.Commit(failureOptions);
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
                TagFamilyLoad(doc, "�ܵ�����ֱ��");
                TagFamilyLoad(doc, "�ܵ�ϵͳ��д");
                TagFamilyLoad(doc, "�������ܱ��");

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
                    if (ftag.Name.Contains(tagName) && ftag.Name.Contains("����ˮ"))
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

                if (tagName == "�������ܱ��")
                {
                    ISet<ElementId> pipeCodeTagSet = pipeCodeTag.GetFamilySymbolIds();
                    List<FamilySymbol> pipeCodeTagList = new List<FamilySymbol>();
                    foreach (var item in pipeCodeTagSet)
                    {
                        pipeCodeTagList.Add(doc.GetElement(item) as FamilySymbol);
                    }

                    string pipeSystemName = (doc.GetElement(pipe.MEPSystem.GetTypeId()) as PipingSystemType).Name;
                    if (pipeSystemName.Contains("ѭ����ˮ") || pipeSystemName.Contains("��ˮ") || pipeSystemName.Contains("��ˮ") || pipeSystemName.Contains("����"))
                    {
                        if ((ConnectorDirection(pipe) == "left") || (ConnectorDirection(pipe) == "down"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("�����ˮ"))
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
                                if (item.Name.Contains("�Ҳ���ˮ"))
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
                    if (pipeSystemName.Contains("ѭ����ˮ") || pipeSystemName.Contains("������ˮ") || pipeSystemName.Contains("�����ˮ") || pipeSystemName.Contains("��ˮ") || pipeSystemName.Contains("ˮԴ��ˮ"))
                    {
                        if ((ConnectorDirection(pipe) == "left") || (ConnectorDirection(pipe) == "down"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("����ˮ"))
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
                                if (item.Name.Contains("�Ҳ��ˮ"))
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
                TagFamilyLoad(doc, "�豸���");
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
                    if (ftag.Name.Contains("�豸���") && ftag.Name.Contains("����ˮ"))
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
        public void SetEquipmentCode(Document doc, FamilyInstance equipment)
        {
            string code = "919PU01";
            string name = equipment.Symbol.FamilyName;
            ProjectInfo pro = doc.ProjectInformation;
            Parameter subproNum = pro.LookupParameter("�������");

            if (name.Contains("����ˮ_ˮ��"))
            {
                code = subproNum.AsString() + "PU01";
            }

            if (name.Contains("����ˮ_��ˮ�豸"))
            {
                code = subproNum.AsString() + "WS01";
            }

            if (name.Contains("����ˮ_��ȴ�豸"))
            {
                code = subproNum.AsString() + "TW01";
            }

            if (name.Contains("����ˮ_��ȴ�豸"))
            {
                code = subproNum.AsString() + "TW01";
            }

            if (name.Contains("����ˮ") && name.Contains("��ҩ"))
            {
                code = subproNum.AsString() + "TS01";
            }

            if (name.Contains("����ˮ") && name.Contains("������"))
            {
                code = subproNum.AsString() + "TS01";
            }

            if (name.Contains("����ˮ") && name.Contains("����"))
            {
                code = subproNum.AsString() + "FR01";
            }

            if (name.Contains("����ˮ") && name.Contains("��«"))
            {
                code = subproNum.AsString() + "EH01";
            }

            if (name.Contains("����ˮ") && name.Contains("��ѹװ��"))
            {
                code = subproNum.AsString() + "FF01";
            }

            if (name.Contains("����ˮ") && name.Contains("ˮ��"))
            {
                code = subproNum.AsString() + "TN01";
            }

            if (name.Contains("����ˮ") && name.Contains("������"))
            {
                code = subproNum.AsString() + "AT01";
            }

            if (name.Contains("����ˮ") && name.Contains("��ѹ��ˮ��"))
            {
                code = subproNum.AsString() + "RN01";
            }

            if (name.Contains("����ˮ") && name.Contains("�ܵ������"))
            {
                code = subproNum.AsString() + "MX01";
            }

            if (name.Contains("����ˮ") && name.Contains("��ˮ�����豸"))
            {
                if (name.Contains("����͸"))
                {
                    code = subproNum.AsString() + "RO01";
                }
                else
                {
                    code = subproNum.AsString() + "WT01";
                }
            }

            if (name.Contains("����ˮ") && name.Contains("��ˮ�����豸"))
            {
                code = subproNum.AsString() + "SW01";
            }

            if (name.Contains("����ˮ") && name.Contains("��դ"))
            {
                code = subproNum.AsString() + "GI01";
            }

            if (name.Contains("����ˮ") && name.Contains("�޴ķ��"))
            {
                code = subproNum.AsString() + "BL01";
            }

            if (name.Contains("����ˮ") && name.Contains("����"))
            {
                code = subproNum.AsString() + "VA01";
            }

            if (name.Contains("����ˮ") && name.Contains("�𽺽�ͷ"))
            {
                code = subproNum.AsString() + "JE01";
            }

            if (name.Contains("����ˮ") && name.Contains("��ձ�"))
            {
                code = subproNum.AsString() + "VG01";
            }

            if (name.Contains("����ˮ") && name.Contains("ѹ����"))
            {
                code = subproNum.AsString() + "VG01";
            }

            if (name.Contains("����ˮ") && name.Contains("������"))
            {
                code = subproNum.AsString() + "LF01";
            }

            if (name.Contains("����ˮ") && name.Contains("ˮ��"))
            {
                code = subproNum.AsString() + "LF01";
            }

            if (name.Contains("����ˮ") && name.Contains("Һλ��"))
            {
                code = subproNum.AsString() + "LQ01";
            }

            if (name.Contains("����ˮ") && name.Contains("������"))
            {
                code = subproNum.AsString() + "AG01";
            }

            if (name.Contains("����ˮ") && name.Contains("�Ƕ�"))
            {
                code = subproNum.AsString() + "AG01";
            }

            if (name.Contains("����ˮ") && name.Contains("���屨��"))
            {
                code = subproNum.AsString() + "AL01";
            }

            equipment.get_Parameter(BuiltInParameter.DOOR_NUMBER).Set(code);
        }
        public bool CreatPipeAccessoryTagMethod(Document doc, UIDocument uidoc)
        {
            try
            {
                TagFamilyLoad(doc, "�ܵ��������");
                Selection sel = uidoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element, new PipeAccessorySelectionFilter());
                FamilyInstance accessory = doc.GetElement(reference) as FamilyInstance;
                if (!accessory.IsTaged(doc))
                {
                    SetEquipmentCode(doc, accessory);
                }

                LocationPoint accessoryLocation = accessory.Location as LocationPoint;
                XYZ projectPickPoint = accessoryLocation.Point;

                IList<Element> accessoryTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessoryTags).ToElements();

                FamilySymbol accessoryTag = null;
                foreach (var item in accessoryTagsCollect)
                {
                    FamilySymbol etag = item as FamilySymbol;
                    Family ftag = etag.Family;
                    if (ftag.Name.Contains("�ܵ�����") && ftag.Name.Contains("����ˮ"))
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
                textNoteType = noteTypes.FirstOrDefault(x => x.Name.Contains("����ˮ") && x.Name.Contains("����"));

                if (textNoteType == null)
                {
                    textNoteType = CreatTextWithLineType(doc);
                }


                Selection sel = uidoc.Selection;
                XYZ pt1 = sel.PickPoint("��ѡ��������ּ�ͷλ��");
                XYZ pt2 = sel.PickPoint("��ѡ��������ִ���λ��");

                //��������ע��
                TextNote note = TextNote.Create(doc, doc.ActiveView.Id, pt2, CreatPipeTag.mainfrm.TextInputCmb.Text, textNoteType.Id);
                note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L); //���߷���һ�����֣���ֱ����ֱ���󻡣��һ�
                note.LeaderLeftAttachment = LeaderAtachement.TopLine;//���ߵ�λ�ã�top��������λ���ڵ�һ���ı���λ��

                //��������Ϊ�ϱ�
                FormattedText formatText = note.GetFormattedText();
                formatText.SetSuperscriptStatus(true);
                note.SetFormattedText(formatText);

                //���ü�ͷ�յ�
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
            TextNoteType existType = noteTypes.FirstOrDefault(x => x.Name.Contains("����ˮ"));

            TextNoteType duplicatedtextType = null;
            duplicatedtextType = existType.Duplicate("����ˮ-��������") as TextNoteType;

            duplicatedtextType.LookupParameter("���ִ�С").SetValueString("5.5mm");
            duplicatedtextType.LookupParameter("���ϵ��").Set(0.70);
            duplicatedtextType.LookupParameter("��������").Set("Bahnschrift SemiLight");
            duplicatedtextType.LookupParameter("����").Set(1);
            duplicatedtextType.LookupParameter("�߿�").Set(1);
            duplicatedtextType.LookupParameter("�»���").Set(1);
            duplicatedtextType.LookupParameter("����").Set(0);
            duplicatedtextType.LookupParameter("б��").Set(0);
            duplicatedtextType.LookupParameter("����/�߽�ƫ����").Set(0);
            duplicatedtextType.LookupParameter("���߼�ͷ").Set(new ElementId(-1));//��ͷ����Ϊ�޵İ취

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
        public string SleeveSize(string nominal_Diameter, int num) //�׹�D3��D2ֵ
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
        public string BaseHoleSize(string nominal_Diameter) //���������ߴ�
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
        public string FloorHoleSize(string nominal_Diameter) //¥��Բ�������ߴ�
        {
            string floorHoleSize = "��100";

            if (nominal_Diameter == "15")
            {
                floorHoleSize = "��50";
            }
            else if (nominal_Diameter == "20")
            {
                floorHoleSize = "��50";
            }
            else if (nominal_Diameter == "25")
            {
                floorHoleSize = "��50";
            }
            else if (nominal_Diameter == "32")
            {
                floorHoleSize = "��50";
            }
            else if (nominal_Diameter == "40")
            {
                floorHoleSize = "��100";
            }
            else if (nominal_Diameter == "50")
            {
                floorHoleSize = "��100";
            }
            else if (nominal_Diameter == "65")
            {
                floorHoleSize = "��150";
            }
            else if (nominal_Diameter == "80")
            {
                floorHoleSize = "��150";
            }
            else if (nominal_Diameter == "100")
            {
                floorHoleSize = "��200";
            }
            else if (nominal_Diameter == "125")
            {
                floorHoleSize = "��200";
            }
            else if (nominal_Diameter == "150")
            {
                floorHoleSize = "��250";
            }
            else if (nominal_Diameter == "200")
            {
                floorHoleSize = "��300";
            }
            else if (nominal_Diameter == "250")
            {
                floorHoleSize = "��350";
            }
            else if (nominal_Diameter == "300")
            {
                floorHoleSize = "��400";
            }
            else if (nominal_Diameter == "350")
            {
                floorHoleSize = "��450";
            }
            else if (nominal_Diameter == "400")
            {
                floorHoleSize = "��500";
            }
            else if (nominal_Diameter == "450")
            {
                floorHoleSize = "��550";
            }
            else if (nominal_Diameter == "500")
            {
                floorHoleSize = "��600";
            }
            else if (nominal_Diameter == "600")
            {
                floorHoleSize = "��700";
            }
            else if (nominal_Diameter == "700")
            {
                floorHoleSize = "��800";
            }

            return floorHoleSize;
        }
        public string PipeWeight(string nominal_Diameter, int haveInsulation) //�ܵ�����
        {
            string weight = null;
            if (haveInsulation == 1)
            {
                if (nominal_Diameter == "DN15")
                {
                    weight = "3.3";
                }
                else if (nominal_Diameter == "DN20")
                {
                    weight = "4.0";
                }
                else if (nominal_Diameter == "DN25")
                {
                    weight = "5.3";
                }
                else if (nominal_Diameter == "DN32")
                {
                    weight = "6.7";
                }
                else if (nominal_Diameter == "DN40")
                {
                    weight = "7.9";
                }
                else if (nominal_Diameter == "DN50")
                {
                    weight = "10.3";
                }
                else if (nominal_Diameter == "DN65")
                {
                    weight = "14.2";
                }
                else if (nominal_Diameter == "DN80")
                {
                    weight = "17.8";
                }
                else if (nominal_Diameter == "DN100")
                {
                    weight = "25.3";
                }
                else if (nominal_Diameter == "DN125")
                {
                    weight = "34.0";
                }
                else if (nominal_Diameter == "DN150")
                {
                    weight = "45.3";
                }
                else if (nominal_Diameter == "DN200")
                {
                    weight = "77.6";
                }
                else if (nominal_Diameter == "DN250")
                {
                    weight = "112.4";
                }
                else if (nominal_Diameter == "DN300")
                {
                    weight = "155.7";
                }
                else if (nominal_Diameter == "DN350")
                {
                    weight = "211.0";
                }
                else if (nominal_Diameter == "DN400")
                {
                    weight = "255.8";
                }
                else if (nominal_Diameter == "DN450")
                {
                    weight = "295.7";
                }
            }
            else
            {
                if (nominal_Diameter == "DN15")
                {
                    weight = "1.7";
                }
                else if (nominal_Diameter == "DN20")
                {
                    weight = "2.2";
                }
                else if (nominal_Diameter == "DN25")
                {
                    weight = "3.3";
                }
                else if (nominal_Diameter == "DN32")
                {
                    weight = "4.6";
                }
                else if (nominal_Diameter == "DN40")
                {
                    weight = "5.7";
                }
                else if (nominal_Diameter == "DN50")
                {
                    weight = "7.8";
                }
                else if (nominal_Diameter == "DN65")
                {
                    weight = "11.3";
                }
                else if (nominal_Diameter == "DN80")
                {
                    weight = "14.8";
                }
                else if (nominal_Diameter == "DN100")
                {
                    weight = "21.7";
                }
                else if (nominal_Diameter == "DN125")
                {
                    weight = "29.9";
                }
                else if (nominal_Diameter == "DN150")
                {
                    weight = "40.7";
                }
                else if (nominal_Diameter == "DN200")
                {
                    weight = "71.8";
                }
                else if (nominal_Diameter == "DN250")
                {
                    weight = "105.5";
                }
                else if (nominal_Diameter == "DN300")
                {
                    weight = "147.7";
                }
                else if (nominal_Diameter == "DN350")
                {
                    weight = "201.9";
                }
                else if (nominal_Diameter == "DN400")
                {
                    weight = "245.7";
                }
                else if (nominal_Diameter == "DN450")
                {
                    weight = "284.8";
                }
            }
            return weight;
        }
        public FamilySymbol NoteSymbol(Document doc, string symbolName)//��ȡע������
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
        /// ���ͼ�����Ϣ
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

