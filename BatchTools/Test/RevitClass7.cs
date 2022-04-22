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
    public class GridQuickDimension : IExternalCommand//选择轴网快速生成标注不是很好
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;
                List<Reference> selRefList = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element).ToList();
                List<Grid> gridList = selRefList.Select(x => doc.GetElement(x)).OfType<Grid>().ToList();
                //其中一条轴网的定位线
                Line oneGridLocaLine = gridList[0].Curve as Line;
                //对轴网进行排列，先定个方向
                XYZ alignmentDirection = oneGridLocaLine.Direction.CrossProduct(XYZ.BasisZ);
                //确定一条排布的线
                Line tempLine = Line.CreateUnbound(oneGridLocaLine.GetEndPoint(0), alignmentDirection);
                //通过点乘去排列
                gridList = gridList.OrderBy(x => x.Curve.Evaluate(0.5, true).DotProduct(alignmentDirection)).ToList();
                Transaction trans = new Transaction(doc, "生成标注");
                trans.Start();
                //然后再遍历生成标注
                for (int i = 0; i < gridList.Count - 1; i++)
                {
                    Reference gridRef1 = new Reference(gridList[i]);
                    Reference gridRef2 = new Reference(gridList[i + 1]);
                    ReferenceArray rerArr = new ReferenceArray();
                    rerArr.Append(gridRef1);
                    rerArr.Append(gridRef2);
                    doc.Create.NewDimension(doc.ActiveView, tempLine, rerArr);
                }
                trans.Commit();



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
