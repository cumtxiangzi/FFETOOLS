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

            // 得到视图的名称
            message += "\nView name: " + view.Name;

            // 视图裁剪区域包围盒
            BoundingBoxXYZ cropBox = view.CropBox;
            XYZ max = cropBox.Max; // 最大值，包围盒右上角
            XYZ min = cropBox.Min; // 最小值，包围盒左下角
            message += "\nCrop Box: ";
            message += "\nMaximum coordinates: (" + max.X + ", " + max.Y + ", " + max.Z + ")";
            message += "\nMinimum coordinates: (" + min.X + ", " + min.Y + ", " + min.Z + ")";
            view.CropBox = bound;


            // 视图的起始点（想象一下透视视图）
            XYZ origin = view.Origin;
            message += "\nOrigin: (" + origin.X + ", " + origin.Y + ", " + origin.Z + ")";


            //视图投影到平面的范围框
            BoundingBoxUV outline = view.Outline;
            UV maxUv = outline.Max; // 最大值，包围盒右上角
            UV minUv = outline.Min; // 最小值，包围盒左下角
            message += "\nOutline: ";
            message += "\nMaximum coordinates: (" + maxUv.U + ", " + maxUv.V + ")";
            message += "\nMinimum coordinates: (" + minUv.U + ", " + minUv.V + ")";


           


            // 视图往右的方向
            XYZ rightDirection = view.RightDirection;
            message += "\nRight direction: (" + rightDirection.X + ", " +
                           rightDirection.Y + ", " + rightDirection.Z + ")";

            // 视图往上的方向
            XYZ upDirection = view.UpDirection;
            message += "\nUp direction: (" + upDirection.X + ", " +
                           upDirection.Y + ", " + upDirection.Z + ")";

            // 视图指向观察者（即我）的方向
            XYZ viewDirection = view.ViewDirection;
            message += "\nView direction: (" + viewDirection.X + ", " +
                           viewDirection.Y + ", " + viewDirection.Z + ")";

            // 视图的缩放比例
            message += "\nScale: " + view.Scale;
            // Scale is meaningless for Schedules
            if (view.ViewType != ViewType.Schedule)
            {
                //int testScale = 5;
                //设置视图比例，需要 Transaction
                //view.Scale = testScale;
                //message += "\nScale after set: " + view.Scale;
            }

            TaskDialog.Show("Revit", message);
        }

    }
}
