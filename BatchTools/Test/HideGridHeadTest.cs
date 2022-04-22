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
    public class MyApp : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            if (commandData.Application.ActiveUIDocument == null)
                return Result.Cancelled;
            UIDocument UIDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UIDoc.Document;

            ViewPlan AcView = UIDoc.ActiveGraphicalView as ViewPlan;
            if (AcView == null)
                return Result.Cancelled;

            Transform Tx = Transform.Identity;
            Tx.BasisX = AcView.RightDirection;
            Tx.BasisY = AcView.UpDirection;
            Tx.BasisZ = XYZ.BasisZ;

            FilteredElementCollector FEC = new FilteredElementCollector(Doc, AcView.Id);
            ElementClassFilter ECF = new ElementClassFilter(typeof(Grid));
            List<Grid> Grids = FEC.WherePasses(ECF).ToElements().Cast<Grid>().ToList();

            const bool ShowGridBubbleLeft = true;
            const bool ShowGridBubbleTop = true;

            using (Transaction Tr = new Transaction(Doc, "Line up those grid bubbles"))
            {

                if (Tr.Start() == TransactionStatus.Started)
                {
                    foreach (Grid Gr in Grids)
                    {
                        Line crv = Gr.Curve as Line;
                        if (crv == null)
                            continue;
                        //Line curves only

                        XYZ[] EP = new XYZ[] {
                Tx.Inverse.OfPoint(crv.GetEndPoint(0)),
                Tx.Inverse.OfPoint(crv.GetEndPoint(1))
            };

                        if (Math.Abs(EP[0].X - EP[1].X) > Math.Abs(EP[0].Y - EP[1].Y))
                        {
                            //More horizontal than vertical

                            if (EP[0].X > EP[1].X)
                            {
                                if (ShowGridBubbleLeft)
                                {
                                    Gr.ShowBubbleInView(DatumEnds.End1, AcView);
                                    Gr.HideBubbleInView(DatumEnds.End0, AcView);
                                }
                                else
                                {
                                    Gr.ShowBubbleInView(DatumEnds.End0, AcView);
                                    Gr.HideBubbleInView(DatumEnds.End1, AcView);
                                }
                            }
                            else
                            {
                                if (ShowGridBubbleLeft)
                                {
                                    Gr.ShowBubbleInView(DatumEnds.End0, AcView);
                                    Gr.HideBubbleInView(DatumEnds.End1, AcView);
                                }
                                else
                                {
                                    Gr.ShowBubbleInView(DatumEnds.End1, AcView);
                                    Gr.HideBubbleInView(DatumEnds.End0, AcView);
                                }
                            }
                        }
                        else
                        {
                            //More vertical than horizontal or perhaps 45 degrees (Dx=Dy)

                            if (EP[0].Y > EP[1].Y)
                            {
                                if (ShowGridBubbleTop)
                                {
                                    Gr.ShowBubbleInView(DatumEnds.End1, AcView);
                                    Gr.HideBubbleInView(DatumEnds.End0, AcView);
                                }
                                else
                                {
                                    Gr.ShowBubbleInView(DatumEnds.End0, AcView);
                                    Gr.HideBubbleInView(DatumEnds.End1, AcView);
                                }
                            }
                            else
                            {
                                if (ShowGridBubbleTop)
                                {
                                    Gr.ShowBubbleInView(DatumEnds.End0, AcView);
                                    Gr.HideBubbleInView(DatumEnds.End1, AcView);
                                }
                                else
                                {
                                    Gr.ShowBubbleInView(DatumEnds.End1, AcView);
                                    Gr.HideBubbleInView(DatumEnds.End0, AcView);
                                }
                            }

                        }
                    }

                    Tr.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
