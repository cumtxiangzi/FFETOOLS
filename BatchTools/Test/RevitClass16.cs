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
using Quadrant = System.Int32;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatPipeAlongGroundTest : IExternalCommand //射线法两点生成管道
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                using (Transaction trans = new Transaction(doc, "创建跟随地形管道"))
                {
                    trans.Start();

                    XYZ pickPoint1= sel.PickPoint(ObjectSnapTypes.Nearest, "请拾取管道起点");
                    XYZ pickPoint2 = sel.PickPoint(ObjectSnapTypes.Nearest, "请拾取管道终点");

                    XYZ newStartPoint =new XYZ(pickPoint1.X,pickPoint1.Y,0);
                    XYZ newEndPoint = new XYZ(pickPoint2.X, pickPoint2.Y, 0);

                    Line startline = CalculateHeight(doc, newStartPoint);
                    Line endline = CalculateHeight(doc, newEndPoint);

                    XYZ realStartPoint = new XYZ(newStartPoint.X, newStartPoint.Y, newStartPoint.Z + startline.Length - 1500 / 304.8);
                    XYZ realEndPoint = new XYZ(newEndPoint.X, newEndPoint.Y, newEndPoint.Z + endline.Length - 1500 / 304.8);

                    ElementId sys = GetPipeSystemType(doc, "给排水", "消防给水").Id;
                    ElementId typeHDPE = GetPipeType(doc, "给排水", "焊接钢管").Id;
                    ElementId level = GetPipeLevel(doc, "0.000").Id;

                    if (realStartPoint != null && realEndPoint != null)
                    {
                        Pipe p = Pipe.Create(doc, sys, typeHDPE, level, realStartPoint, realEndPoint);
                        ChangePipeSize(p, "250");
                    }
                    else
                    {
                        MessageBox.Show("点为空");
                    }

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                // throw e;
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }        
        public Line CalculateHeight(Document doc, XYZ center)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            Func<View3D, bool> isNotTemplate = v3 => !(v3.IsTemplate);
            View3D view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First<View3D>(isNotTemplate);

            FilteredElementCollector TopographCollector = new FilteredElementCollector(doc).OfClass(typeof(TopographySurface)).OfCategory(BuiltInCategory.OST_Topography);
            IList<Element> topographs = TopographCollector.ToElements();
            TopographySurface targetTypography = topographs.FirstOrDefault(x=>x.Name=="表面") as TopographySurface;

            var typographyList = new List<TopographySurface>();
            typographyList.Add(targetTypography);
            var subRegionIds = targetTypography.GetHostedSubRegionIds();
            if (subRegionIds != null) typographyList.AddRange(subRegionIds.Select(x => doc.GetElement(x) as TopographySurface));


            XYZ intersection = null;
            Line result = null;
            foreach (var topography in typographyList)
            {
                // Project in the negative Z direction down to the floor.特别注意Z值,决定了射线的方向,-1向下,1向上
                XYZ rayDirection = new XYZ(0, 0, 1);

                // Look for references to faces where the element is the floor element id.
                ReferenceIntersector referenceIntersector = new ReferenceIntersector(topography.Id, FindReferenceTarget.Mesh, view3D);
                IList<ReferenceWithContext> references = referenceIntersector.Find(center, rayDirection);

                double distance = Double.PositiveInfinity;               
                foreach (ReferenceWithContext referenceWithContext in references)
                {
                    Reference reference = referenceWithContext.GetReference();
                    // Keep the closest matching reference (using the proximity parameter to determine closeness).
                    double proximity = referenceWithContext.Proximity;
                    if (proximity < distance)
                    {
                        distance = proximity;
                        intersection = reference.GlobalPoint;
                        if (intersection != null)
                        {
                            // Create line segment from the start point and intersection point.
                            result = Line.CreateBound(center, intersection);
                        }
                    }
                }
            }                    
            return result;
        }
        public static PipeType GetPipeType(Document doc, string profession, string pipetype)
        {
            // 获取管道类型
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains(profession) && ps.Name.Contains(pipetype))
                {
                    pt = ps;
                    break;
                }
            }
            return pt;
        }
        public static PipingSystemType GetPipeSystemType(Document doc, string profession, string pipesystemtype)
        {
            // 获取管道系统
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = collector.ToElements();
            PipingSystemType pipesys = null;
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;
                if (ps.Name.Contains(profession) && ps.Name.Contains(pipesystemtype))
                {
                    pipesys = ps;
                    break;
                }
            }
            return pipesys;
        }
        public static void ChangePipeSize(Pipe pipe, string dn)
        {
            //改变管道尺寸
            Parameter diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            diameter.SetValueString(dn);
        }
        public static Level GetPipeLevel(Document doc, string Levelname)
        {
            // 获取标高
            Level newlevel = null;
            var levelFilter = new ElementClassFilter(typeof(Level));
            FilteredElementCollector levels = new FilteredElementCollector(doc);
            levels = levels.WherePasses(levelFilter);
            foreach (Level level in levels)
            {
                if (level.Name == Levelname)
                {
                    newlevel = level;
                    break;
                }
            }
            return newlevel;
        }
    }
}
