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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class MyApp : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                Selection sel = uiDoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element);
                Pipe pipe = doc.GetElement(reference) as Pipe;
               

                using (Transaction transaction = new Transaction(doc, "View"))
                {
                    transaction.Start();
                    View view = doc.ActiveView;
                    Getinfo_View(view,pipe.get_BoundingBox(view));
                    transaction.Commit();
                }
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }          
        }
        private void Getinfo_View(View view,BoundingBoxXYZ bound)
        {
            string message = "View: ";

            // �õ���ͼ������
            message += "\nView name: " + view.Name;

            // ��ͼ�ü������Χ��
            BoundingBoxXYZ cropBox = view.CropBox;
            XYZ max = cropBox.Max; // ���ֵ����Χ�����Ͻ�
            XYZ min = cropBox.Min; // ��Сֵ����Χ�����½�
            message += "\nCrop Box: ";
            message += "\nMaximum coordinates: (" + max.X + ", " + max.Y + ", " + max.Z + ")";
            message += "\nMinimum coordinates: (" + min.X + ", " + min.Y + ", " + min.Z + ")";
            view.CropBox = bound;


            // ��ͼ����ʼ�㣨����һ��͸����ͼ��
            XYZ origin = view.Origin;
            message += "\nOrigin: (" + origin.X + ", " + origin.Y + ", " + origin.Z + ")";


            //��ͼͶӰ��ƽ��ķ�Χ��
            BoundingBoxUV outline = view.Outline;
            UV maxUv = outline.Max; // ���ֵ����Χ�����Ͻ�
            UV minUv = outline.Min; // ��Сֵ����Χ�����½�
            message += "\nOutline: ";
            message += "\nMaximum coordinates: (" + maxUv.U + ", " + maxUv.V + ")";
            message += "\nMinimum coordinates: (" + minUv.U + ", " + minUv.V + ")";


           


            // ��ͼ���ҵķ���
            XYZ rightDirection = view.RightDirection;
            message += "\nRight direction: (" + rightDirection.X + ", " +
                           rightDirection.Y + ", " + rightDirection.Z + ")";

            // ��ͼ���ϵķ���
            XYZ upDirection = view.UpDirection;
            message += "\nUp direction: (" + upDirection.X + ", " +
                           upDirection.Y + ", " + upDirection.Z + ")";

            // ��ͼָ��۲��ߣ����ң��ķ���
            XYZ viewDirection = view.ViewDirection;
            message += "\nView direction: (" + viewDirection.X + ", " +
                           viewDirection.Y + ", " + viewDirection.Z + ")";

            // ��ͼ�����ű���
            message += "\nScale: " + view.Scale;
            // Scale is meaningless for Schedules
            if (view.ViewType != ViewType.Schedule)
            {
                //int testScale = 5;
                //������ͼ��������Ҫ Transaction
                //view.Scale = testScale;
                //message += "\nScale after set: " + view.Scale;
            }

            TaskDialog.Show("Revit", message);
        }

    }
}
