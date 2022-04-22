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
    public class GridQuickDimension : IExternalCommand//ѡ�������������ɱ�ע���Ǻܺ�
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;
                List<Reference> selRefList = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element).ToList();
                List<Grid> gridList = selRefList.Select(x => doc.GetElement(x)).OfType<Grid>().ToList();
                //����һ�������Ķ�λ��
                Line oneGridLocaLine = gridList[0].Curve as Line;
                //�������������У��ȶ�������
                XYZ alignmentDirection = oneGridLocaLine.Direction.CrossProduct(XYZ.BasisZ);
                //ȷ��һ���Ų�����
                Line tempLine = Line.CreateUnbound(oneGridLocaLine.GetEndPoint(0), alignmentDirection);
                //ͨ�����ȥ����
                gridList = gridList.OrderBy(x => x.Curve.Evaluate(0.5, true).DotProduct(alignmentDirection)).ToList();
                Transaction trans = new Transaction(doc, "���ɱ�ע");
                trans.Start();
                //Ȼ���ٱ������ɱ�ע
                for (int i = 0; i < gridList.Count - 1; i++)
                {
                    Reference gridRef1 = new Reference(gridList[i]);
                    Reference gridRef2 = new Reference(gridList[i + 1]);
                    ReferenceArray rerArr = new ReferenceArray();
                    rerArr.Append(gridRef1);
                    rerArr.Append(gridRef2);
                    doc.Create.NewDimension(doc.ActiveView, tempLine, rerArr);
                }
                trans.Commit();



            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
