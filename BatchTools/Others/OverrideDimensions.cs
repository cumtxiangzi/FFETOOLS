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
    public class OverrideDimensions : IExternalCommand //可将单个尺寸标注的文字隐藏
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
                MessageBox.Show("请确保选择的尺寸标注只有一个数值","警告",MessageBoxButton.OK,MessageBoxImage.Warning);
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        public void HideValue(Document doc, UIDocument uidoc)
        {
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, new DimensionAndElevationSelectionFilter(), "请选择单个尺寸标注或高程点");
            Element ele = doc.GetElement(reference);

            using (Transaction trans = new Transaction(doc, "尺寸标注或标高不显示数值"))
            {
                trans.Start();
                if (ele.Category.Name == "尺寸标注")
                {
                    Dimension dimension = ele as Dimension;
                    dimension.ValueOverride = "\u200E";
                }
                else if(ele.Category.Name =="高程点")
                {
                    SpotDimension elevation = ele as SpotDimension;
                    elevation.ValueOverride = "\u200E";
                }
                trans.Commit();
            }
           
        }
    }
}
