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
    public class GridHeadHide : IExternalCommand //���߱�ͷ����δ���
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                double min_x = 0; double min_y = 0; double max_x = 0; double max_y = 0;
                try
                {
                    PickedBox pickedbox = uidoc.Selection.PickBox(PickBoxStyle.Crossing, "��ѡ��Ҫ���ɾֲ���ά��ƽ������");
                    min_x = BackMin(pickedbox.Min.X, pickedbox.Max.X);
                    min_y = BackMin(pickedbox.Min.Y, pickedbox.Max.Y);
                    max_x = BackMax(pickedbox.Min.X, pickedbox.Max.X);
                    max_y = BackMax(pickedbox.Min.Y, pickedbox.Max.Y);
                }
                catch
                {
                    return Result.Cancelled;
                }

                XYZ minPoint = new XYZ(min_x, min_y, 0);
                XYZ maxPoint = new XYZ(max_x, max_y, 0);
                Outline outLine = new Outline(minPoint, maxPoint);
                //Outline outLine = new Outline(new XYZ(0, 0, 0), new XYZ(100, 100, 100));
                BoundingBoxIntersectsFilter boundFilter = new BoundingBoxIntersectsFilter(outLine);//�˹�����ֻ����άԪ����Ч���Զ�άԪ�������������ֵ���Ч

                FilteredElementCollector collector = new FilteredElementCollector(doc);
                IList<Element> crossElements = collector.OfClass(typeof(Pipe)).WherePasses(boundFilter).ToElements();

                
                using (Transaction trans = new Transaction(doc, "�������"))
                {
                    trans.Start();

                    MessageBox.Show(crossElements.Count.ToString());

                    foreach (Element item in crossElements)
                    {
                        //MessageBox.Show(item.Name);
                    }

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

        //������Сֵ
        private Double BackMin(Double a, Double b)
        {
            if (a > b)
            {
                return b;
            }
            else
            {
                return a;
            }

        }

        //�������ֵ
        private Double BackMax(Double a, Double b)
        {
            if (a > b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
    }
}
