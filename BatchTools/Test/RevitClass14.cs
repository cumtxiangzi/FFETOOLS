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
    public class DimesionBetweenEP : IExternalCommand //���������豸��ߴ��ע
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                using (Transaction trans = new Transaction(doc, "�豸��������ע"))
                {
                    trans.Start();

                    DimensionBetweenInstance(doc, uidoc);

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        /// <summary>
        /// �ж��豸λ�ô������ �Ƿ�Ϊˮƽ ���Ǵ�ֱ
        /// </summary>
        /// <param name="familyInstances"></param>
        /// <returns></returns>
        public bool isDimensionHorizen(List<FamilyInstance> familyInstances)
        {
            if (
                Math.Abs
                (
                   familyInstances.OrderBy(f => (f.Location as LocationPoint).Point.X).Select(f => (f.Location as LocationPoint).Point.X).First()
                 - familyInstances.OrderByDescending(f => (f.Location as LocationPoint).Point.X).Select(f => (f.Location as LocationPoint).Point.X).First()
                )
                >
                Math.Abs
                (
                      familyInstances.OrderBy(f => (f.Location as LocationPoint).Point.Y).Select(f => (f.Location as LocationPoint).Point.Y).First()
                    - familyInstances.OrderByDescending(f => (f.Location as LocationPoint).Point.Y).Select(f => (f.Location as LocationPoint).Point.Y).First()
                 )
                )
            {
                return true;
            }
            return false;
        }

        public void DimensionBetweenInstance(Document doc, UIDocument uidoc)//�豸֮��������ע
        {
            IList<Element> quipments = uidoc.Selection.PickElementsByRectangle(new WMechanicalSelectionFilter(), "��ѡ����Ҫ������ע���豸");

            DimensionType dimType = null;
            FilteredElementCollector elems = new FilteredElementCollector(doc);//��ȡ��ע����
            foreach (DimensionType dt in elems.OfClass(typeof(DimensionType)))
            {
                if (dt.Name.Contains("����ˮ") && dt.StyleType == DimensionStyleType.Linear)
                {
                    dimType = dt;
                    break;
                }
            }

            List<FamilyInstance> instanceInBoxList = new List<FamilyInstance>();
            foreach (FamilyInstance item in quipments)
            {
                instanceInBoxList.Add(item);
            }

            View view = doc.ActiveView;
            IList<Reference> instanceReferences = new List<Reference>();
            ReferenceArray referenceArray = new ReferenceArray();

            instanceInBoxList.Sort((a, b) => (a.Location as LocationPoint).Point.X.CompareTo((b.Location as LocationPoint).Point.X));//����         
            bool isHorizen = isDimensionHorizen(instanceInBoxList);//�ж��豸λ�ú����������

            Reference ref1 = null;
            Reference ref2 = null;
            foreach (var item in instanceInBoxList)
            {
                //double rotation = (item.Location as LocationPoint).Rotation;
                //bool phare = Math.Round(rotation / (0.5 * Math.PI) % 2, 4) == 0;//�Ƿ�180��ת  �� ��ԭ�����������ƽ��  ƽ����true

                //IList<Reference> refs1 = isHorizen ^ phare == false ? item.GetReferences(FamilyInstanceReferenceType.CenterLeftRight)
                //    : item.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);//��ʵ��������Ĳ���ƽ̨�ó���

                ref1 = item.GetReferenceByName("�ߴ��ע������1");//��ʵ����ͨ������ƽ�����ƻ�ȡ���գ�ǰ���ǲ���ƽ����벻��Ϊ�ǲ���
                ref2 = item.GetReferenceByName("�ߴ��ע������2");//��ʵ���������ղ���ƽ���ó���
                if (ref1 != null && ref2 != null)
                {
                    instanceReferences.Add(ref1); //����õ����߷������ƽ̨����
                    referenceArray.Append(ref1);
                    instanceReferences.Add(ref2);
                    referenceArray.Append(ref2);
                }
                else
                {
                    MessageBox.Show("��ȷ�����к�������Ϊ" + "\n" + "�ߴ��ע������1��2�Ĳ���ƽ��", "����", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                }

            }

            //����ߴ��עλ��
            Line referenceLine = null;
            if (isHorizen)
            {
                referenceLine = Line.CreateBound(new XYZ(), new XYZ(0, 10, 0));
            }
            else
            {
                referenceLine = Line.CreateBound(new XYZ(), new XYZ(10, 0, 0));
            }

            if (ref1 != null && ref2 != null)
            {
                XYZ selPoint = uidoc.Selection.PickPoint(ObjectSnapTypes.None, "ѡ��ߴ綨λλ��");
                XYZ lineDir = referenceLine.Direction.CrossProduct(new XYZ(0, 0, 1));
                XYZ point_s = referenceLine.GetEndPoint(0);
                XYZ point_e = referenceLine.GetEndPoint(1);
                if (point_s.DistanceTo(selPoint) > point_e.DistanceTo(selPoint))
                {
                    XYZ temPoint = point_s;
                    point_s = point_e;
                    point_e = temPoint;
                }
                XYZ offsetDir = point_e - point_s;
                double lenght = dimType.get_Parameter(BuiltInParameter.TEXT_SIZE).AsDouble();
                Line line_o = Line.CreateUnbound(selPoint, lineDir);

                Dimension autoDimension = doc.Create.NewDimension(view, line_o, referenceArray, dimType);
            }
        }

        public void GetInstanceGeometry_Curve(Document doc, UIDocument uidoc)
        {
            TaskDialog.Show("1", "ѡ��һ������");
            Reference profilReference = uidoc.Selection.PickObject(ObjectType.Element, "��ѡ��һ����ʵ������");
            FamilyInstance familyInstance = doc.GetElement(profilReference) as FamilyInstance;
            Options option = new Options();//�½�һ���������ε�ѡ��
            option.ComputeReferences = true; //�򿪼��㼸������ 
            option.DetailLevel = ViewDetailLevel.Fine; //��ͼ��ϸ�̶�Ϊ��� 
            GeometryElement geomElement = familyInstance.get_Geometry(option); //����ʵ���л�ȡ�����ļ���Ԫ��GeometryElement
            foreach (GeometryObject geomObj in geomElement)
            {
                if (geomObj is GeometryInstance)//�Ӽ���Ԫ��GeometryElement���ҵ��伸��ʵ��GeometryInstance
                {
                    GeometryInstance geomInstance = geomObj as GeometryInstance;
                    //����ʵ��GeometryInstance��SymbolGeometry�����л�ȡ�����͵ļ��ζ���GeometryObject��
                    foreach (GeometryObject instObj in geomInstance.SymbolGeometry)
                    {
                        if (instObj is Curve)
                        {
                            Curve curve = instObj as Curve;
                            TaskDialog.Show("�ֲ�����ϵ", "�ֲ�����ϵ�ǣ�" + "\n"
                            + "��㣺" + curve.GetEndPoint(0).ToString() + "\n"
                            + "�յ㣺" + curve.GetEndPoint(1).ToString());
                            // ��ȡ�����߱任��ʵ��������ϵ�� 
                            curve = curve.CreateTransformed(geomInstance.Transform);
                            TaskDialog.Show("��������ϵ", "��������ϵ�ǣ�" + "\n"
                            + "��㣺" + curve.GetEndPoint(0).ToString() + "\n"
                            + "�յ㣺" + curve.GetEndPoint(1).ToString());
                            // �� 
                        }
                    }
                }
            }
        }//���δ���
    }
}
