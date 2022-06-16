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
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class OverrideDimensions : IExternalCommand //可将单个尺寸标注的文字改写
    {
        public static OverrideDimensionsForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new OverrideDimensionsForm();
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();              
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
             
            }
            return Result.Succeeded;
        }
    }

    public class ExecuteEventOverrideDimensions : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = app.ActiveUIDocument.Document;
            Selection sel = app.ActiveUIDocument.Selection;

            try
            {
                HideValue(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
               
            }
        }

        public string GetName()
        {
            return "批量改尺寸标注";
        }
        public void HideValue(Document doc, UIDocument uidoc)
        {
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, new DimensionAndElevationSelectionFilter(), "请选择单个尺寸标注");
            Element ele = doc.GetElement(reference);

            using (Transaction trans = new Transaction(doc, "尺寸标注改写"))
            {
                trans.Start();
                if (ele.Category.Name == "尺寸标注")
                {
                    Dimension dimension = ele as Dimension;
                    if(dimension.Segments.Size==0)
                    {
                        dimension.ValueOverride = "\u200E";
                        Line directionLine = dimension.Curve as Line;
                        string textVlaue = OverrideDimensions.mainfrm.LengthValue.Text;
                        if (directionLine.Direction.X == 1)
                        {
                            XYZ textPosition = new XYZ(dimension.TextPosition.X - doc.ActiveView.Scale * 3.5 / 304.8, dimension.TextPosition.Y + doc.ActiveView.Scale * 5 / 304.8, 0);
                            HorizontalDimensionText(doc, doc.ActiveView, textVlaue, textPosition);
                        }
                        else
                        {
                            XYZ textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                            VerticalDimensionText(doc, doc.ActiveView, textVlaue, textPosition);
                        }
                    }
                    else
                    {
                        MessageBox.Show("请确保选择的尺寸标注只有一个数值", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }                  
                }
                else if (ele.Category.Name == "高程点")
                {
                    SpotDimension elevation = ele as SpotDimension;
                    elevation.ValueOverride = "\u200E";
                }
                trans.Commit();
            }

            HideValue(doc, uidoc);
        }
        public TextNote VerticalDimensionText(Document doc, View view, string text, XYZ point)
        {
            TextNote textValue = null;
            TextNoteType type = null;
            IList<TextNoteType> noteTypes = CollectorHelper.TCollector<TextNoteType>(doc);

            foreach (var item in noteTypes)
            {
                if (item.Name.Contains("给排水-字高3.5"))
                {
                    type = item;
                    break;
                }
            }

            textValue = TextNote.Create(doc, view.Id, point, text, type.Id);
            Line zAxis = Line.CreateBound(new XYZ(point.X, point.Y, 0), new XYZ(point.X, point.Y, 1));
            ElementTransformUtils.RotateElement(doc, textValue.Id, zAxis, -Math.PI / 2);
            return textValue;
        }
        public TextNote HorizontalDimensionText(Document doc, View view, string text, XYZ point)
        {
            TextNote textValue = null;
            TextNoteType type = null;
            IList<TextNoteType> noteTypes = CollectorHelper.TCollector<TextNoteType>(doc);

            foreach (var item in noteTypes)
            {
                if (item.Name.Contains("给排水-字高3.5"))
                {
                    type = item;
                    break;
                }
            }

            textValue = TextNote.Create(doc, view.Id, point, text, type.Id);
            return textValue;
        }
    }
}
