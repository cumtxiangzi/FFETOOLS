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
    public class CreateTextNode : IExternalCommand //创建带箭头的文字
    {
        public Result Execute(ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            UIApplication uiapp = cmdData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;

            //若此视图没有工作平面则新建工作平面（比如二次开发在剖面是需要新建工作平面的）
            if (view.SketchPlane == null)
            {
                Transaction ts1 = new Transaction(doc, "新建工作平面");
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
                textNodeLocationPt = S1.PickPoint("请选择文字注释创建位置");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            if (textNodeLocationPt != null)
            {
                //创建文字注释Option
                TextNoteOptions options = new TextNoteOptions();
                options.HorizontalAlignment = HorizontalTextAlignment.Left; //文字水平对齐方式
                options.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
                //options.ve= VerticalTextAlignment.Top;//文字垂直对齐方式
                double dWidth = 0.07;//文字宽度
                //箭头端点
                XYZ leaderEnd = new XYZ(textNodeLocationPt.X - 1500 / 304.8, textNodeLocationPt.Y + 1500 / 304.8, textNodeLocationPt.Z);


                using (Transaction tran = new Transaction(doc, "Create  Textnote"))
                {
                    tran.Start();
                    //创建文字注释
                    TextNote note = TextNote.Create(doc, doc.ActiveView.Id, textNodeLocationPt, dWidth, "净高控制线", options);
                    note.AddLeader(TextNoteLeaderTypes.TNLT_STRAIGHT_L); //引线方向，一共四种，左直，右直，左弧，右弧
                    note.LeaderLeftAttachment = LeaderAtachement.TopLine;//引线的位置，top代表引线位置在第一行文本的位置

                    IList<Leader> leaderList = note.GetLeaders();
                    foreach (Leader leader in leaderList)
                    {
                        leader.End = leaderEnd;//给箭头端点设置值
                        XYZ pointElbow = new XYZ(leaderEnd.X, leader.Anchor.Y, leaderEnd.Z);
                        leader.Elbow = pointElbow;//给箭头弯头点设置值
                    }

                    //创建文字注释的族类型，族类型名称“宋体_2.5mm”,首先判断当前族类型是不是，不是就判断有没有该族类型，有就用，没有就创建了再用
                    if (note.TextNoteType.Name != "宋体_2.5mm")
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
                                    if (item.Name == "宋体_2.5mm")
                                    {
                                        symbolExist = true;
                                        elementId = item.Id;
                                    }
                                }
                                if (symbolExist)
                                {
                                    (note as TextNote).ChangeTypeId(elementId);//依据ID改变族类型
                                }

                                else
                                {
                                    TextNoteType duplicatedtextType = null;
                                    duplicatedtextType = type.Duplicate("宋体_2.5mm") as TextNoteType;

                                    string textNode = duplicatedtextType.LookupParameter("文字大小").AsString() + duplicatedtextType.LookupParameter("文字大小").AsValueString();
                                    string dut_gerneral = duplicatedtextType.LookupParameter("宽度系数").AsString() + duplicatedtextType.LookupParameter("宽度系数").AsValueString();
                                    string text_Form = duplicatedtextType.LookupParameter("文字字体").AsString() + duplicatedtextType.LookupParameter("文字字体").AsValueString();
                                    string text_Elbow = duplicatedtextType.LookupParameter("引线箭头").AsString() + duplicatedtextType.LookupParameter("引线箭头").AsValueString();

                                    if (textNode != null && textNode != "2.5mm")
                                    {
                                        duplicatedtextType.LookupParameter("文字大小").SetValueString("2.5mm");
                                    }
                                    if (dut_gerneral != null && dut_gerneral != 0.7.ToString())
                                    {
                                        duplicatedtextType.LookupParameter("宽度系数").Set(0.70);
                                    }
                                    if (text_Form != null && text_Form != "新宋体")
                                    {
                                        duplicatedtextType.LookupParameter("文字字体").Set("新宋体");
                                    }

                                    if (text_Elbow != null && text_Elbow != "楼梯碰头_30度实心箭头")
                                    {
                                        //创建族类型——楼梯碰头_30度实心箭头
                                        Element arrowType = doc.GetElement(duplicatedtextType.LookupParameter("引线箭头").AsElementId());
                                        arrowType.LookupParameter("箭头样式").Set(8);
                                        arrowType.Name = "楼梯碰头_30度实心箭头";
                                        arrowType.LookupParameter("箭头宽度角").Set(0.523598775598298);
                                        arrowType.LookupParameter("填充记号").Set(1);
                                        arrowType.LookupParameter("记号尺寸").Set(0.00984251968503937);
                                    }

                                    note.TextNoteType = duplicatedtextType;
                                }
                            }
                        }
                    }
                    //设置显示颜色（也可修改文字注释颜色的属性）
                    Color color = new Color((byte)255, (byte)128, (byte)128);
                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                    ogs.SetProjectionLineColor(color);//投影表面线的颜色
                    view.SetElementOverrides(note.Id, ogs);

                    tran.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}