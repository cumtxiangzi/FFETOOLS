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
    class DimesionOpration : IExternalCommand //������֮��ı�ע�������� ����������
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
                familyInstances.Add((document.GetElement(reference)) as FamilyInstance);//ת��ѡ�е�����
            }
            bool isHorizen = isDimensionHorizen(familyInstances);//�ж��豸λ�ú����������
            foreach (FamilyInstance familyInstance in familyInstances)
            {
                double rotation = (familyInstance.Location as LocationPoint).Rotation;
                bool phare = Math.Round(rotation / (0.5 * Math.PI) % 2, 4) == 0;//�Ƿ�180��ת  �� ��ԭ�����������ƽ��  ƽ����true
                //MessageBox.Show(isHorizen.ToString() + "\t" + phare.ToString());
                IList<Reference> refs = isHorizen ^ phare == false ? familyInstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight)
                    : familyInstance.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);//��ʵ��������Ĳ���ƽ̨�ó���

                IList<Reference> refLRs = familyInstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
                IList<Reference> reFBRs = familyInstance.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);



                // IList<Reference> refs = familyInstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);//��ʵ��������Ĳ���ƽ̨�ó���
                instanceReferences.Add(refs.Count == 0 ? null : refs[0]); //����õ����߷������ƽ̨����

                referenceArray.Append(refs.Count == 0 ? null : refs[0]);
            }

            //  AutoCreatDimension(document, selection, instanceReferences);  //�ú������� ����������

            Element element = document.GetElement(hasPickOne.ElementAt(0));
            XYZ elementXyz = (element.Location as LocationPoint).Point;
            // Line line = (element.Location as LocationCurve).Curve as Line;

            //�ߴ��߶�λ
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

            Transaction transaction = new Transaction(document, "��ӱ�ע");
            transaction.Start();
            //���ô����ߴ�ķ�������
            Dimension autoDimension = document.Create.NewDimension(view, newLine, referenceArray);
            transaction.Commit();


            // AutoCreatDimension(document,selection, hasPickOne);  //�ú������� ����������

            //throw new NotImplementedException();

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


    }
}