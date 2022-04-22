using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Xml;
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
using System.Windows.Forms;

namespace FFETOOLS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class AddAxis : IExternalCommand
    {
        public Result Execute(Autodesk.Revit.UI.ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            Transaction newTran = null;
            try
            {
                if (null == cmdData)
                {
                    throw new ArgumentNullException("commandData");
                }

                Document doc = cmdData.Application.ActiveUIDocument.Document;

                newTran = new Transaction(doc);
                newTran.Start("add axis");

                Autodesk.Revit.UI.Selection.Selection sel = cmdData.Application.ActiveUIDocument.Selection;
                Element ele = doc.GetElement(sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "select a axis"));

                Grid grid = ele as Grid;

                if (null == grid)
                {
                    throw new ArgumentNullException("not aixs");
                }

                XYZ ptDirection = cmdData.Application.ActiveUIDocument.Selection.PickPoint("please pick point with offset direction");

                AxisNameForm dlgAxisName = new AxisNameForm(doc);

                if (dlgAxisName.ShowDialog() != DialogResult.OK)
                {
                    newTran.Commit();
                    return Autodesk.Revit.UI.Result.Cancelled;
                }

                OffsetLengthForm dlgOffsetLength = new OffsetLengthForm();

                if (dlgOffsetLength.ShowDialog() != DialogResult.OK)
                {
                    newTran.Commit();
                    return Autodesk.Revit.UI.Result.Cancelled;
                }

                OffsetAxis offsetAxis = new OffsetAxis(cmdData);
                offsetAxis.Offset(grid, dlgAxisName.NewName, dlgOffsetLength.OffsetLength, ptDirection);

                newTran.Commit();

                return Autodesk.Revit.UI.Result.Succeeded;
            }
            catch (Exception e)
            {
                msg = e.Message;
                if ((newTran != null) && newTran.HasStarted() && !newTran.HasEnded())
                    newTran.RollBack();
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
    }

    class OffsetAxis
    {
        protected Autodesk.Revit.UI.ExternalCommandData m_Revit;
        public const double PI = Math.PI;
       

        public OffsetAxis(Autodesk.Revit.UI.ExternalCommandData cmdData)
        {
            m_Revit = cmdData;
        }

        public void Offset(Grid axis, string name, double offsetLength, XYZ ptDirection)
        {
            offsetLength = Common.MMtoIntch(offsetLength);
            ElementId typeId = axis.GetTypeId();
            ElementType type = m_Revit.Application.ActiveUIDocument.Document.GetElement(typeId) as ElementType;
            GridType gridTpe = type as GridType;

            Curve axisCurve = axis.Curve;
            Line axisLine = axisCurve as Line;
            if (null != axisLine)
            {
                OffsetLineAxis(gridTpe, axisLine, name, offsetLength, ptDirection);
                return;
            }
            Arc axisArc = axisCurve as Arc;
            if (null != axisArc)
            {
                OffsetArcAxis(gridTpe, axisArc, name, offsetLength, ptDirection);
            }
        }

        protected void OffsetLineAxis(GridType type, Line axisLine, string name, double offsetLength, XYZ ptDirection)
        {
            XYZ ptStart = axisLine.GetEndPoint(0);
            XYZ ptEnd = axisLine.GetEndPoint(1);
            XYZ vector1 = ptDirection - ptStart;
            XYZ vector2 = ptEnd - ptStart;
            XYZ offsetDir = vector2;
            //double angle = vector2.AngleTo(vector1);
            //if (angle > 0.0 && angle < PI)
            if (Geometry.PointAtLineLeft(ptDirection, ptStart, ptEnd))
            {
                offsetDir = Geometry.RotateTo(offsetDir, PI / 2.0, XYZ.BasisZ);
            }
            else
            {
                offsetDir = Geometry.RotateTo(offsetDir, -PI / 2.0, XYZ.BasisZ);
            }
            offsetDir = offsetDir.Normalize().Multiply(offsetLength);

            ptStart = ptStart.Add(offsetDir);
            ptEnd = ptEnd.Add(offsetDir);

            Line geomLine = Line.CreateBound(ptStart, ptEnd);
            Grid lineGrid = m_Revit.Application.ActiveUIDocument.Document.Create.newg(geomLine);

            if (null == lineGrid)
            {
                throw new Exception("Create a new straight grid failed.");
            }
            lineGrid.Name = name;

            if (null != type)
            {
                lineGrid.GridType = type;
            }

        }

        protected void OffsetArcAxis(GridType type, Arc axisArc, string name, double offsetLength, XYZ ptDirection)
        {
            double radius = axisArc.Radius;
            XYZ ptCenter = axisArc.Center;
            double space1 = axisArc.Distance(ptDirection);
            double space2 = ptCenter.DistanceTo(ptDirection);

            XYZ ptStart = axisArc.GetEndPoint(0);
            XYZ ptEnd = axisArc.GetEndPoint(1);
            double startAngle = 0, endAngle = 0;
            Geometry.GetArcAngles(axisArc, ref startAngle, ref endAngle);

            if (Math.Abs((space1 + space2) - radius) < 0.001)
            {
                radius -= offsetLength;
            }
            else
            {
                radius += offsetLength;
            }

            Arc geomArc = Arc.Create(ptCenter, radius, startAngle, endAngle, XYZ.BasisX, XYZ.BasisY);
            Grid lineGrid = DOC.NewGrid(geomArc);

            if (null == lineGrid)
            {
                throw new Exception("Create a new straight grid failed.");
            }
            lineGrid.Name = name;

            if (null != type)
            {
                lineGrid.GridType = type;
            }
        }
    }
}
