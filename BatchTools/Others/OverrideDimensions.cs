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
    public class OverrideDimensions : IExternalCommand //�ɽ������ߴ��ע����������
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                HideValue(doc, uidoc);
            }
            catch (Exception)
            {
                //messages = e.Message;
                MessageBox.Show("��ȷ��ѡ��ĳߴ��עֻ��һ����ֵ","����",MessageBoxButton.OK,MessageBoxImage.Warning);
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        public void HideValue(Document doc, UIDocument uidoc)
        {
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, new DimensionAndElevationSelectionFilter(), "��ѡ�񵥸��ߴ��ע��̵߳�");
            Element ele = doc.GetElement(reference);

            using (Transaction trans = new Transaction(doc, "�ߴ��ע���߲���ʾ��ֵ"))
            {
                trans.Start();
                if (ele.Category.Name == "�ߴ��ע")
                {
                    Dimension dimension = ele as Dimension;
                    dimension.ValueOverride = "\u200E";
                }
                else if(ele.Category.Name =="�̵߳�")
                {
                    SpotDimension elevation = ele as SpotDimension;
                    elevation.ValueOverride = "\u200E";
                }
                trans.Commit();
            }
           
        }
    }
}
