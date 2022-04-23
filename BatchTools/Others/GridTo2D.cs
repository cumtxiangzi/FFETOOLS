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
    public class GridTo2D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                ViewPlan AcView = uidoc.ActiveGraphicalView as ViewPlan;
                if (AcView == null)
                    return Result.Cancelled;
           
                FilteredElementCollector FEC = new FilteredElementCollector(doc, AcView.Id);
                ElementClassFilter ECF = new ElementClassFilter(typeof(Grid));
                List<Grid> Grids = FEC.WherePasses(ECF).ToElements().Cast<Grid>().ToList();
              
                using (Transaction trans = new Transaction(doc, "轴网2D"))
                {
                    trans.Start();
                    
                    int number=0;
                    foreach (var item in Grids)
                    {
                        item.SetDatumExtentType(DatumEnds.End0,AcView,DatumExtentType.ViewSpecific);
                        item.SetDatumExtentType(DatumEnds.End1, AcView, DatumExtentType.ViewSpecific);
                        number++;
                    }

                    if (number!=0)
                    {
                        MessageBox.Show("当前视图的轴网全部转为2D轴网","提示",MessageBoxButton.OK,MessageBoxImage.Information);
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
    }
}
