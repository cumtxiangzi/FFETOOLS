using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;


namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class NotePipes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                using (Transaction ts = new Transaction(doc, "管道系统图标注"))
                {
                    ts.Start();
                    IList<Element> pipetagscollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                    FamilySymbol pipeDNtag = null;
                    foreach (Element tag in pipetagscollect)
                    {
                        FamilySymbol pipetag = tag as FamilySymbol;
                        if (pipetag.Name.Contains("管道公称直径"))
                        {
                            pipeDNtag = pipetag;
                            break;
                        }
                    }

                    FilteredElementCollector pipeCollector = new FilteredElementCollector(doc);
                    pipeCollector.OfClass(typeof(Pipe));
                    IList<Element> pipes = pipeCollector.ToElements();
                    foreach (Element pipe in pipes)
                    {

                        double pipeLength = pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();

                        if ((pipeLength * 304.83) >= 500)
                        {
                            Reference pipeRef = new Reference(pipe);
                            TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                            TagOrientation tagOri = TagOrientation.Horizontal;
                            //Add the tag to the middle of duct
                            LocationCurve locCurve = pipe.Location as LocationCurve;
                            XYZ pipeMid = locCurve.Curve.Evaluate(0.5, true);
                            IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, pipeMid);
                            tag.ChangeTypeId(pipeDNtag.Id);
                        }

                    }
                    ts.Commit();
                }
                return Result.Succeeded;
            }
            catch (Exception)
            {
                TaskDialog.Show("错误", "请锁定三维视图");
                return Result.Failed;
               
            }
        }
    }
}
