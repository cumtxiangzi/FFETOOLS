using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace FFETOOLS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class DimesionOpration : IExternalCommand //族与族之间的标注基本方法 （可批量）
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            Selection selection = commandData.Application.ActiveUIDocument.Selection;
            Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;

            Autodesk.Revit.DB.View view = document.ActiveView;
            int hashcode = 0;
            ReferenceArray referenceArray = new ReferenceArray();
            //Pick one object from Revit.
            List<Wall> mWalls = new List<Wall>();
            List<FamilyInstance> familyInstances = new List<FamilyInstance>();

            IList<Reference> instanceReferences = new List<Reference>();
            IList<Reference> hasPickOne = selection.PickObjects(ObjectType.Element);
            foreach (Reference reference in hasPickOne)
            {
                familyInstances.Add((document.GetElement(reference)) as FamilyInstance);//转换选中的族们
            }
            bool isHorizen = isDimensionHorizen(familyInstances);//判断设备位置横竖大致情况
            foreach (FamilyInstance familyInstance in familyInstances)
            {
                double rotation = (familyInstance.Location as LocationPoint).Rotation;
                bool phare = Math.Round(rotation / (0.5 * Math.PI) % 2, 4) == 0;//是否180旋转  即 与原有族各参照线平行  平行则true
                //MessageBox.Show(isHorizen.ToString() + "\t" + phare.ToString());
                IList<Reference> refs = isHorizen ^ phare == false ? familyInstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight)
                    : familyInstance.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);//将实例中特殊的参照平台拿出来

                IList<Reference> refLRs = familyInstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                IList<Reference> reFBRs = familyInstance.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);



                // IList<Reference> refs = familyInstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);//将实例中特殊的参照平台拿出来
                instanceReferences.Add(refs.Count == 0 ? null : refs[0]); //将获得的中线放入参照平台面内

                referenceArray.Append(refs.Count == 0 ? null : refs[0]);
            }

            //  AutoCreatDimension(document, selection, instanceReferences);  //该函数可用 但条件苛刻

            Element element = document.GetElement(hasPickOne.ElementAt(0));
            XYZ elementXyz = (element.Location as LocationPoint).Point;
            // Line line = (element.Location as LocationCurve).Curve as Line;

            //尺寸线定位
            double distanceNewLine = 2;
            Line line = Line.CreateBound(elementXyz, new XYZ(elementXyz.X + distanceNewLine, elementXyz.Y, elementXyz.Z));
            line = isHorizen == true ?
                  Line.CreateBound(elementXyz, new XYZ(elementXyz.X, elementXyz.Y + distanceNewLine, elementXyz.Z))
                : Line.CreateBound(elementXyz, new XYZ(elementXyz.X + distanceNewLine, elementXyz.Y, elementXyz.Z));
            XYZ selectionPoint = selection.PickPoint();
            selectionPoint = new XYZ(elementXyz.X + distanceNewLine, elementXyz.Y + 50, elementXyz.Z);
            selectionPoint = isHorizen == true ?
                    new XYZ(elementXyz.X + 50, elementXyz.Y + distanceNewLine, elementXyz.Z)
                : new XYZ(elementXyz.X + distanceNewLine, elementXyz.Y + 50, elementXyz.Z);
            XYZ projectPoint = line.Project(selectionPoint).XYZPoint;
            Line newLine = Line.CreateBound(selectionPoint, projectPoint);

            Transaction transaction = new Transaction(document, "添加标注");
            transaction.Start();
            //调用创建尺寸的方法创建
            Dimension autoDimension = document.Create.NewDimension(view, newLine, referenceArray);
            transaction.Commit();


            // AutoCreatDimension(document,selection, hasPickOne);  //该函数可用 但条件苛刻

            //throw new NotImplementedException();

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


    }
}