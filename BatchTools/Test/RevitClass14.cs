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
    public class DimesionBetweenEP : IExternalCommand //批量创建设备间尺寸标注
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                using (Transaction trans = new Transaction(doc, "设备间批量标注"))
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
        /// 判断设备位置大致情况 是否为水平 还是垂直
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

        public void DimensionBetweenInstance(Document doc, UIDocument uidoc)//设备之间批量标注
        {
            IList<Element> quipments = uidoc.Selection.PickElementsByRectangle(new WMechanicalSelectionFilter(), "请选择需要批量标注的设备");

            DimensionType dimType = null;
            FilteredElementCollector elems = new FilteredElementCollector(doc);//获取标注类型
            foreach (DimensionType dt in elems.OfClass(typeof(DimensionType)))
            {
                if (dt.Name.Contains("给排水") && dt.StyleType == DimensionStyleType.Linear)
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

            instanceInBoxList.Sort((a, b) => (a.Location as LocationPoint).Point.X.CompareTo((b.Location as LocationPoint).Point.X));//排序         
            bool isHorizen = isDimensionHorizen(instanceInBoxList);//判断设备位置横竖大致情况

            Reference ref1 = null;
            Reference ref2 = null;
            foreach (var item in instanceInBoxList)
            {
                //double rotation = (item.Location as LocationPoint).Rotation;
                //bool phare = Math.Round(rotation / (0.5 * Math.PI) % 2, 4) == 0;//是否180旋转  即 与原有族各参照线平行  平行则true

                //IList<Reference> refs1 = isHorizen ^ phare == false ? item.GetReferences(FamilyInstanceReferenceType.CenterLeftRight)
                //    : item.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);//将实例中特殊的参照平台拿出来

                ref1 = item.GetReferenceByName("尺寸标注参照线1");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                ref2 = item.GetReferenceByName("尺寸标注参照线2");//将实例中弱参照参照平面拿出来
                if (ref1 != null && ref2 != null)
                {
                    instanceReferences.Add(ref1); //将获得的中线放入参照平台面内
                    referenceArray.Append(ref1);
                    instanceReferences.Add(ref2);
                    referenceArray.Append(ref2);
                }
                else
                {
                    MessageBox.Show("请确保族中含有名称为" + "\n" + "尺寸标注参照线1或2的参照平面", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                }

            }

            //计算尺寸标注位置
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
                XYZ selPoint = uidoc.Selection.PickPoint(ObjectSnapTypes.None, "选择尺寸定位位置");
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
            TaskDialog.Show("1", "选择一个轮廓");
            Reference profilReference = uidoc.Selection.PickObject(ObjectType.Element, "请选择一个族实例轮廓");
            FamilyInstance familyInstance = doc.GetElement(profilReference) as FamilyInstance;
            Options option = new Options();//新建一个解析几何的选项
            option.ComputeReferences = true; //打开计算几何引用 
            option.DetailLevel = ViewDetailLevel.Fine; //视图详细程度为最好 
            GeometryElement geomElement = familyInstance.get_Geometry(option); //从族实例中获取到它的几何元素GeometryElement
            foreach (GeometryObject geomObj in geomElement)
            {
                if (geomObj is GeometryInstance)//从几何元素GeometryElement中找到其几何实例GeometryInstance
                {
                    GeometryInstance geomInstance = geomObj as GeometryInstance;
                    //几何实例GeometryInstance的SymbolGeometry属性中获取族类型的几何对象（GeometryObject）
                    foreach (GeometryObject instObj in geomInstance.SymbolGeometry)
                    {
                        if (instObj is Curve)
                        {
                            Curve curve = instObj as Curve;
                            TaskDialog.Show("局部坐标系", "局部坐标系是：" + "\n"
                            + "起点：" + curve.GetEndPoint(0).ToString() + "\n"
                            + "终点：" + curve.GetEndPoint(1).ToString());
                            // 把取到的线变换到实例的坐标系中 
                            curve = curve.CreateTransformed(geomInstance.Transform);
                            TaskDialog.Show("世界坐标系", "世界坐标系是：" + "\n"
                            + "起点：" + curve.GetEndPoint(0).ToString() + "\n"
                            + "终点：" + curve.GetEndPoint(1).ToString());
                            // … 
                        }
                    }
                }
            }
        }//几何待用
    }
}
