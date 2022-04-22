using Autodesk.Revit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace FFETOOLS
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class CreateTextNode : IExternalCommand //��������ͷ������
    {
        public Result Execute(ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            UIApplication uiapp = cmdData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;

            //������ͼû�й���ƽ�����½�����ƽ�棨������ο�������������Ҫ�½�����ƽ��ģ�
            if (view.SketchPlane == null)
            {
                Transaction ts1 = new Transaction(doc, "�½�����ƽ��");
                ts1.Start();
                Plane plane = Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, doc.ActiveView.Origin);
                SketchPlane sp = SketchPlane.Create(doc, plane);
                doc.ActiveView.SketchPlane = sp;
                view = doc.ActiveView;
                ts1.Commit();
            }

            Selection S1 = uidoc.Selection;
            XYZ textNodeLocationPt = null;
            try
            {
                textNodeLocationPt = S1.PickPoint("��ѡ������ע�ʹ���λ��");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            if (textNodeLocationPt != null)
            {
                //��������ע��Option
                TextNoteOptions options = new TextNoteOptions();
                options.HorizontalAlignment = HorizontalTextAlignment.Left; //����ˮƽ���뷽ʽ
                options.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
                //options.ve= VerticalTextAlignment.Top;//���ִ�ֱ���뷽ʽ
                double dWidth = 0.07;//���ֿ��
                //��ͷ�˵�
                XYZ leaderEnd = new XYZ(textNodeLocationPt.X - 1500 / 304.8, textNodeLocationPt.Y + 1500 / 304.8, textNodeLocationPt.Z);


                using (Transaction tran = new Transaction(doc, "Create  Textnote"))
                {
                    tran.Start();
                    //��������ע��
                    TextNote note = TextNote.Create(doc, doc.ActiveView.Id, textNodeLocationPt, dWidth, "���߿�����", options);
                    note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L); //���߷���һ�����֣���ֱ����ֱ���󻡣��һ�
                    note.LeaderLeftAttachment = LeaderAtachement.TopLine;//���ߵ�λ�ã�top��������λ���ڵ�һ���ı���λ��

                    IList<Leader> leaderList = note.GetLeaders();
                    foreach (Leader leader in leaderList)
                    {
                        leader.End = leaderEnd;//����ͷ�˵�����ֵ
                        XYZ pointElbow = new XYZ(leaderEnd.X, leader.Anchor.Y, leaderEnd.Z);
                        leader.Elbow = pointElbow;//����ͷ��ͷ������ֵ
                    }

                    //��������ע�͵������ͣ����������ơ�����_2.5mm��,�����жϵ�ǰ�������ǲ��ǣ����Ǿ��ж���û�и������ͣ��о��ã�û�оʹ���������
                    if (note.TextNoteType.Name != "����_2.5mm")
                    {
                        Parameter familyType = (note as Element).get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM);
                        if (familyType != null && familyType.StorageType == StorageType.ElementId && familyType.AsElementId().IntegerValue >= 0)
                        {
                            Element elem_2 = doc.GetElement(familyType.AsElementId());
                            TextNoteType type = elem_2 as TextNoteType;
                            if (type != null)
                            {
                                ElementId elementId = null;
                                bool symbolExist = false;
                                FilteredElementCollector collectorSymbol = new FilteredElementCollector(doc);
                                IList<Element> textSymbols = collectorSymbol.OfClass(typeof(TextNoteType)).ToElements();
                                foreach (var item in collectorSymbol)
                                {
                                    if (item.Name == "����_2.5mm")
                                    {
                                        symbolExist = true;
                                        elementId = item.Id;
                                    }
                                }
                                if (symbolExist)
                                {
                                    (note as TextNote).ChangeTypeId(elementId);//����ID�ı�������
                                }

                                else
                                {
                                    TextNoteType duplicatedtextType = null;
                                    duplicatedtextType = type.Duplicate("����_2.5mm") as TextNoteType;

                                    string textNode = duplicatedtextType.LookupParameter("���ִ�С").AsString() + duplicatedtextType.LookupParameter("���ִ�С").AsValueString();
                                    string dut_gerneral = duplicatedtextType.LookupParameter("���ϵ��").AsString() + duplicatedtextType.LookupParameter("���ϵ��").AsValueString();
                                    string text_Form = duplicatedtextType.LookupParameter("��������").AsString() + duplicatedtextType.LookupParameter("��������").AsValueString();
                                    string text_Elbow = duplicatedtextType.LookupParameter("���߼�ͷ").AsString() + duplicatedtextType.LookupParameter("���߼�ͷ").AsValueString();

                                    if (textNode != null && textNode != "2.5mm")
                                    {
                                        duplicatedtextType.LookupParameter("���ִ�С").SetValueString("2.5mm");
                                    }
                                    if (dut_gerneral != null && dut_gerneral != 0.7.ToString())
                                    {
                                        duplicatedtextType.LookupParameter("���ϵ��").Set(0.70);
                                    }
                                    if (text_Form != null && text_Form != "������")
                                    {
                                        duplicatedtextType.LookupParameter("��������").Set("������");
                                    }

                                    if (text_Elbow != null && text_Elbow != "¥����ͷ_30��ʵ�ļ�ͷ")
                                    {
                                        //���������͡���¥����ͷ_30��ʵ�ļ�ͷ
                                        Element arrowType = doc.GetElement(duplicatedtextType.LookupParameter("���߼�ͷ").AsElementId());
                                        arrowType.LookupParameter("��ͷ��ʽ").Set(8);
                                        arrowType.Name = "¥����ͷ_30��ʵ�ļ�ͷ";
                                        arrowType.LookupParameter("��ͷ��Ƚ�").Set(0.523598775598298);
                                        arrowType.LookupParameter("���Ǻ�").Set(1);
                                        arrowType.LookupParameter("�Ǻųߴ�").Set(0.00984251968503937);
                                    }

                                    note.TextNoteType = duplicatedtextType;
                                }
                            }
                        }
                    }
                    //������ʾ��ɫ��Ҳ���޸�����ע����ɫ�����ԣ�
                    Color color = new Color((byte)255, (byte)128, (byte)128);
                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                    ogs.SetProjectionLineColor(color);//ͶӰ�����ߵ���ɫ
                    view.SetElementOverrides(note.Id, ogs);

                    tran.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}