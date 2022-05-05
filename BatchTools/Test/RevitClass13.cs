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
    public class CreatStairsTest : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                TransactionGroup tg = new TransactionGroup(doc,"创建楼梯测试");

                tg.Start();

                Level bottomLevel = null;
                 Level topLevel = null;

                using (Transaction trans = new Transaction(doc, "name"))
                {
                    trans.Start();
                   
                    bottomLevel = GetLevel(doc, (-2.5 * 1000).ToString());
                    topLevel = GetLevel(doc, (0 * 1000).ToString());

                    trans.Commit();
               }

               ElementId stairsID= CreateStairs(doc, new XYZ(),bottomLevel, topLevel);

                using (Transaction trans = new Transaction(doc, "name"))
                {
                    trans.Start();

                    Stairs newStairs=doc.GetElement(stairsID) as Stairs;
                    newStairs.ActualTreadDepth = 200 / 304.8;

                    trans.Commit();
                }

                tg.Assimilate();
            }
            catch (Exception e)
            {
                //throw e;
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        private ElementId CreateStairs(Document document, XYZ stairPoint, Level levelBottom, Level levelTop)//创建楼梯
        {
            ElementId newStairsId = null;
            using (StairsEditScope newStairsScope = new StairsEditScope(document, "创建楼梯"))
            {
                newStairsId = newStairsScope.Start(levelBottom.Id, levelTop.Id);
                using (Transaction stairsTrans = new Transaction(document, "创建梯段和栏杆"))
                {
                    stairsTrans.Start();

                    // 为楼梯创建一个草图梯段
                    IList<Curve> bdryCurves = new List<Curve>();
                    IList<Curve> riserCurves = new List<Curve>();
                    IList<Curve> pathCurves = new List<Curve>();

                    double height = levelTop.Elevation - levelBottom.Elevation;
                    double length = height - 200 / 304.8;
                    //MessageBox.Show("ssss");
                    XYZ pnt2 = new XYZ(stairPoint.X, stairPoint.Y, 0);
                    XYZ pnt1 = new XYZ(stairPoint.X - length, stairPoint.Y, 0);
                    XYZ pnt4 = new XYZ(stairPoint.X, stairPoint.Y + 900 / 304.8, 0);
                    XYZ pnt3 = new XYZ(stairPoint.X - length, stairPoint.Y + 900 / 304.8, 0);

                    //XYZ pnt1 = new XYZ(0, 0, 0);
                    //XYZ pnt2 = new XYZ(15, 0, 0);
                    //XYZ pnt3 = new XYZ(0, 10, 0);
                    //XYZ pnt4 = new XYZ(15, 10, 0);



                    //边界
                    bdryCurves.Add(Line.CreateBound(pnt1, pnt2));
                    bdryCurves.Add(Line.CreateBound(pnt3, pnt4));
                    // 踏步的线
                    double riserNum = Math.Floor(height * 304.8 / 200);
                    //double riserNum = 20;

                    for (int ii = 0; ii <= riserNum; ii++)
                    {
                        XYZ end0 = (pnt1 + pnt2) * ii / riserNum;
                        XYZ end1 = (pnt3 + pnt4) * ii / riserNum;
                        XYZ end2 = new XYZ(end1.X, 900 / 304.8, 0);
                        riserCurves.Add(Line.CreateBound(end0, end2));
                    }

                    //楼梯的路径
                    XYZ pathEnd0 = (pnt1 + pnt3) / 2.0;
                    XYZ pathEnd1 = (pnt2 + pnt4) / 2.0;
                    pathCurves.Add(Line.CreateBound(pathEnd0, pathEnd1));


                    // 创建一个草图梯段
                    //StairsRun newRun1 = StairsRun.CreateSketchedRun(document, newStairsId, levelBottom.Elevation, bdryCurves, riserCurves, pathCurves);

                    //MessageBox.Show(newRun1.ToString());

                    // 创建一个直跑梯段
                    Line locationLine = Line.CreateBound(new XYZ(0, 0, -2500/304.8), new XYZ(length, 0, -2500/304.8));
                    StairsRun newRun2 = StairsRun.CreateStraightRun(document, newStairsId, locationLine, StairsRunJustification.Center);
                    newRun2.ActualRunWidth = 900/304.8;

                    stairsTrans.Commit();
                }
                // 错误信息处理.
                newStairsScope.Commit(new FailuresPreprocessor());
            }
            return newStairsId;
        }
        public Level GetLevel(Document doc, string levelOffsetValue)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            IList<Element> levelList = collector.ToElements();
            Level level = null;

            foreach (Element e in levelList)
            {
                Level lev = e as Level;
                if (lev.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsValueString() == levelOffsetValue)
                {
                    level = lev;
                }
            }
            return level;
        }
    }
}
