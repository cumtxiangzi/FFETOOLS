using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Architecture;


namespace CreateStairs
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Transaction trans = new Transaction(doc, "new level");
            trans.Start();
            Level blvl = Level.Create(doc, 0);
            Level tlvl = Level.Create(doc, 2);
            trans.Commit();
            CreateStairs(doc, blvl, tlvl);
            return Result.Succeeded;
        }
        private ElementId CreateStairs(Document document, Level levelBottom, Level levelTop)
        {
            ElementId newStairsId = null;
            using (StairsEditScope newStairsScope = new StairsEditScope(document, "New Stairs"))
            {
                newStairsId = newStairsScope.Start(levelBottom.Id, levelTop.Id);
                using (Transaction stairsTrans = new Transaction(document, "Add Runs and Landings to Stairs"))
                {
                    stairsTrans.Start();

                    // Create a sketched run for the stairs
                    IList<Curve> bdryCurves = new List<Curve>();
                    IList<Curve> riserCurves = new List<Curve>();
                    IList<Curve> pathCurves = new List<Curve>();
                    XYZ pnt1 = new XYZ(0, 0, 0);
                    XYZ pnt2 = new XYZ(15, 0, 0);
                    XYZ pnt3 = new XYZ(0, 10, 0);
                    XYZ pnt4 = new XYZ(15, 10, 0);
                    // boundaries
                    bdryCurves.Add(Line.CreateBound(pnt1, pnt2));
                    bdryCurves.Add(Line.CreateBound(pnt3, pnt4));
                    // riser curves
                    const int riserNum = 20;
                    for (int ii = 0; ii <= riserNum; ii++)
                    {
                        XYZ end0 = (pnt1 + pnt2) * ii / (double)riserNum;
                        XYZ end1 = (pnt3 + pnt4) * ii / (double)riserNum;
                        XYZ end2 = new XYZ(end1.X, 10, 0);
                        riserCurves.Add(Line.CreateBound(end0, end2));
                    }

                    //stairs path curves
                    XYZ pathEnd0 = (pnt1 + pnt3) / 2.0;
                    XYZ pathEnd1 = (pnt2 + pnt4) / 2.0;
                    pathCurves.Add(Line.CreateBound(pathEnd0, pathEnd1));
                    StairsRun newRun1 = StairsRun.CreateSketchedRun(document, newStairsId, levelBottom.Elevation, bdryCurves, riserCurves, pathCurves);
                    // Add a straight run
                    Line locationLine = Line.CreateBound(new XYZ(20, -5, newRun1.TopElevation), new XYZ(35, -5, newRun1.TopElevation));
                    StairsRun newRun2 = StairsRun.CreateStraightRun(document, newStairsId, locationLine, StairsRunJustification.Center);
                    newRun2.ActualRunWidth = 10;
                    // Add a landing between the runs
                    CurveLoop landingLoop = new CurveLoop();
                    XYZ p1 = new XYZ(15, 10, 0);
                    XYZ p2 = new XYZ(20, 10, 0);
                    XYZ p3 = new XYZ(20, -10, 0);
                    XYZ p4 = new XYZ(15, -10, 0);
                    Line curve_1 = Line.CreateBound(p1, p2);
                    Line curve_2 = Line.CreateBound(p2, p3);
                    Line curve_3 = Line.CreateBound(p3, p4);
                    Line curve_4 = Line.CreateBound(p4, p1);
                    landingLoop.Append(curve_1);
                    landingLoop.Append(curve_2);
                    landingLoop.Append(curve_3);
                    landingLoop.Append(curve_4);
                    //StairsLanding newLanding = StairsLanding.CreateSketchedLanding(document, newStairsId, landingLoop, newRun1.TopElevation);
                    stairsTrans.Commit();
                }
                // A failure preprocessor is to handle possible failures during the edit mode commitment process.
               // newStairsScope.Commit(new FailuresPreprocessor());//new StairsFailurePreprocessor());
                newStairsScope.Commit(new FailuresPreprocessor());//new StairsFailurePreprocessor());

            }
            return newStairsId;
        }

    }
    public class FailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> listFma = failuresAccessor.GetFailureMessages();
            if (listFma.Count == 0)
                return FailureProcessingResult.Continue;
            foreach (FailureMessageAccessor fma in listFma)
            {
                if (fma.GetSeverity() == FailureSeverity.Error)
                {
                    if (fma.HasResolutions())
                        failuresAccessor.ResolveFailure(fma);
                }
                if (fma.GetSeverity() == FailureSeverity.Warning)
                {
                    failuresAccessor.DeleteWarning(fma);
                }
            }
            return FailureProcessingResult.ProceedWithCommit;
        }
    }
}