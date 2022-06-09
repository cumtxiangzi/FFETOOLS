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
    public class CreatPipeAlongGround : IExternalCommand
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

                    XYZ realStartPoint = sel.PickPoint(ObjectSnapTypes.Points, "请拾取管道起点");
                    XYZ realEndPoint = sel.PickPoint(ObjectSnapTypes.Points, "请拾取管道终点");

                    IList<TopographySurface> topographys = CollectorHelper.TCollector<TopographySurface>(doc);
                    TopographySurface topography = topographys.FirstOrDefault(x => x.Name == "表面");

                    XYZ crossPoint1 = GetIntersectPointOnTopographySurface(topography, realStartPoint);
                    XYZ crossPoint2 = GetIntersectPointOnTopographySurface(topography, realEndPoint);
                   // MessageBox.Show(crossPoint1.ToString()+"\n"+realStartPoint.ToString());

                    ElementId sys = GetPipeSystemType(doc, "给排水", "污水管道").Id;
                    ElementId typeHDPE = GetPipeType(doc, "给排水", "HDPE管").Id;
                    ElementId typeUPVC = GetPipeType(doc, "给排水", "UPVC管").Id;
                    ElementId level = GetPipeLevel(doc, "0.000").Id;

                    Pipe p = Pipe.Create(doc, sys, typeHDPE, level, crossPoint1, crossPoint2);
                    ChangePipeSize(p, "300");

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                //throw e;
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        public XYZ GetIntersectPointOnTopographySurface(TopographySurface typography, XYZ inputPoint)
        {
            XYZ resultPoint=null;
            var document = typography.Document;
            var targetTypography = !typography.IsSiteSubRegion ? typography : document.GetElement(typography.AsSiteSubRegion().HostId) as TopographySurface;
            var typographyList = new List<TopographySurface>();
            var meshs = new List<Mesh>();
            var meshTriangles = new List<MeshTriangle>();
            var points = new List<XYZ>();
            typographyList.Add(targetTypography);
            var subRegionIds = targetTypography.GetHostedSubRegionIds();
            if (subRegionIds != null) typographyList.AddRange(subRegionIds.Select(x => document.GetElement(x) as TopographySurface));
            typographyList.ForEach(x => meshs.AddRange(GetMeshs(x)));
            meshs.ForEach(x => { points.AddRange(x.Vertices); });
            points.Sort((x, y) => (x - XYZ.BasisZ * y.Z).DistanceTo(inputPoint) <= (y - XYZ.BasisZ * y.Z).DistanceTo(inputPoint) ? -1 : 1);
            var firstThreePoints = points.GetRange(0, 3);
            Parallel.ForEach(meshs, (mesh) =>
            {
                for (var i = 0; i < mesh.NumTriangles; i++)
                {
                    var meshTriangle = mesh.get_Triangle(i);
                    for (var j = 0; j < 3; j++)
                    {
                        if (firstThreePoints.Any(y => meshTriangle.get_Vertex(j).IsAlmostEqualTo(y)))
                        {
                            meshTriangles.Add(meshTriangle);
                            break;
                        }
                    }
                }
            });

            foreach (var meshTriangle in meshTriangles)
            {
                try
                {
                    var profile = new CurveLoop();
                    for (var j = 0; j < 3; j++)
                    {
                        profile.Append(Line.CreateBound(meshTriangle.get_Vertex(j), meshTriangle.get_Vertex((j + 1) % 3)));
                    }
                    var intersectPoint = GetIntersectPointOnCurveloop(new List<CurveLoop> { profile }, inputPoint);
                    if (intersectPoint != null)
                    {
                        resultPoint = intersectPoint;
                    }
                    //continue;
                    //return intersectPoint;
                    //MessageBox.Show("ss");
                }
                catch { }
            }
            return resultPoint;
        }

        public List<Mesh> GetMeshs(TopographySurface surface)
        {
            Mesh groundMesh = null;
            List<Mesh> meshList = new List<Mesh>();
            GeometryElement geometry = surface.get_Geometry(new Options { DetailLevel = ViewDetailLevel.Fine });

            foreach (var item in geometry)
            {
                groundMesh = item as Mesh;
                if (groundMesh != null)
                {
                    meshList.Add(groundMesh);
                }
            }
            return meshList;
        }
        public XYZ GetIntersectPointOnCurveloop(List<CurveLoop> curveList, XYZ point)
        {
            XYZ resultPoint = null;
            XYZ tempPoint=new XYZ();
            List<Curve> faceCur = new List<Curve>();
            List<XYZ> points = new List<XYZ>();
            List<XYZ> notSamePoints = new List<XYZ>();
            //MessageBox.Show(curveList.FirstOrDefault()..ToString());
            //Solid newSolid = GeometryCreationUtilities.CreateExtrusionGeometry(curveList, XYZ.BasisZ, 1000 / 304.8);
            //Face solidFace = newSolid.Getupfaces().FirstOrDefault();
            //bool pointInFace = solidFace.IsInside(new UV(point.X, point.Y));
            //if (solidFace.Project(point).XYZPoint != null)
            //{
            //    resultPoint = solidFace.Project(point).XYZPoint;
            //}
            //MessageBox.Show(solidFace.ToString());

            CurveLoopIterator iteraor = curveList.FirstOrDefault().GetCurveLoopIterator();
            while (iteraor.MoveNext())
            {
                Curve cur = iteraor.Current;
                faceCur.Add(cur);
            }
            foreach (var item in faceCur)
            {
                XYZ p1 = item.GetEndPoint(0);
                XYZ p2 = item.GetEndPoint(1);
                points.Add(p1);
                points.Add(p2);
            }
            notSamePoints = points.Distinct((a, b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z).ToList();
            tempPoint = GetIntersectWithLineAndPlane(point, XYZ.BasisZ, notSamePoints);
            if(IsPosPlane(notSamePoints, tempPoint))
            {
                resultPoint = tempPoint;
            }
            //MessageBox.Show(IsPosPlane(notSamePoints,resultPoint).ToString());

            return resultPoint;
        }

        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="point">直线上某一点</param>
        /// <param name="direct">直线的方向</param>
        /// <param name="planePoints">平面的顶点(最少需传入平面内三个点)</param>
        /// <returns></returns>
        private XYZ GetIntersectWithLineAndPlane(XYZ point, XYZ direct, List<XYZ> planePoints)
        {
            //根据3点求平面法向量
            XYZ p1 = planePoints[0]; XYZ p2 = planePoints[1]; XYZ p3 = planePoints[2];
            ///v1(n1,n2,n3); 平面方程: na * (x – n1) + nb * (y – n2) + nc * (z – n3) = 0 ;
            double na = (p2.Y - p1.Y) * (p3.Z - p1.Z) - (p2.Z - p1.Z) * (p3.Y - p1.Y);
            double nb = (p2.Z - p1.Z) * (p3.X - p1.X) - (p2.X - p1.X) * (p3.Z - p1.Z);
            double nc = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
            XYZ planeNormal = new XYZ(na, nb, nc);
            //平面上任意一点
            XYZ planePoint = planePoints.Last();
            //使用点积计算交点距直线上点point的距离
            double d = (planePoint - point).DotProduct(planeNormal) / direct.Normalize().DotProduct(planeNormal);
            XYZ pointResult = d * direct.Normalize() + point;
            return pointResult;
        }

        /// <summary>
        /// 确定坐标是否在平面内
        /// </summary>
        /// <param name="points">平面顶点坐标</param>
        /// <param name="pos">已知坐标(传入上述方法计算得来的交点)</param>
        /// <returns></returns>
        private bool IsPosPlane(List<XYZ> points, XYZ pos)
        {
            double RadianValue = 0;
            XYZ potOld = XYZ.Zero;
            XYZ potNew = XYZ.Zero;
            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0)
                {
                    potOld = points[i] - pos;
                }
                if (i == points.Count - 1)
                {
                    potNew = points[0] - pos;
                }
                else
                {
                    potNew = points[i + 1] - pos;
                }
                //夹角和
                RadianValue += Math.Acos(potOld.Normalize().DotProduct(potNew.Normalize())) * (180 / Math.PI);
                potOld = potNew;
            }
            //夹角的和等于360度表示在平面内
            if (Math.Abs(RadianValue - 360) < 0.1f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public Line CalculateHeight(Document doc, XYZ center)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            Func<View3D, bool> isNotTemplate = v3 => !(v3.IsTemplate);
            View3D view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First<View3D>(isNotTemplate);

            FilteredElementCollector TopographCollector = new FilteredElementCollector(doc).OfClass(typeof(TopographySurface)).OfCategory(BuiltInCategory.OST_Topography);
            IList<Element> topographs = TopographCollector.ToElements();
            TopographySurface topography = topographs.ElementAt(0) as TopographySurface;

            // Project in the negative Z direction down to the floor.特别注意Z值,决定了射线的方向,-1向下,1向上
            XYZ rayDirection = new XYZ(0, 0, 1);

            // Look for references to faces where the element is the floor element id.
            ReferenceIntersector referenceIntersector = new ReferenceIntersector(topography.Id, FindReferenceTarget.Mesh, view3D);
            IList<ReferenceWithContext> references = referenceIntersector.Find(center, rayDirection);

            double distance = Double.PositiveInfinity;
            XYZ intersection = null;
            foreach (ReferenceWithContext referenceWithContext in references)
            {
                Reference reference = referenceWithContext.GetReference();
                // Keep the closest matching reference (using the proximity parameter to determine closeness).
                double proximity = referenceWithContext.Proximity;
                if (proximity < distance)
                {
                    distance = proximity;
                    intersection = reference.GlobalPoint;
                }
            }
            // Create line segment from the start point and intersection point.
            Line result = Line.CreateBound(center, intersection);
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
    public static class PointInPolyExtensions
    {
        /// <summary>
        /// Add new point to list, unless already present.
        /// </summary>
        private static void AddToPunten(
          List<XYZ> XYZarray,
          XYZ p1)
        {
            var p = XYZarray.Where(
              c => Math.Abs(c.X - p1.X) < 0.001
                && Math.Abs(c.Y - p1.Y) < 0.001)
              .FirstOrDefault();

            if (p == null)
            {
                XYZarray.Add(p1);
            }
        }

        /// <summary>
        /// Return a list of boundary 
        /// points for the given room.
        /// </summary>
        private static List<XYZ> MaakPuntArray(
          Room room)
        {
            SpatialElementBoundaryOptions opt
              = new SpatialElementBoundaryOptions();

            opt.SpatialElementBoundaryLocation
              = SpatialElementBoundaryLocation.Center;

            var boundaries = room.GetBoundarySegments(
              opt);

            return MaakPuntArray(boundaries);
        }

        /// <summary>
        /// Return a list of boundary points 
        /// for the given boundary segments.
        /// </summary>
        private static List<XYZ> MaakPuntArray(
          IList<IList<BoundarySegment>> boundaries)
        {
            List<XYZ> puntArray = new List<XYZ>();
            foreach (var bl in boundaries)
            {
                foreach (var s in bl)
                {
                    Curve c = s.GetCurve();
                    AddToPunten(puntArray, c.GetEndPoint(0));
                    AddToPunten(puntArray, c.GetEndPoint(1));
                }
            }
            puntArray.Add(puntArray.First());
            return puntArray;
        }

        /// <summary>
        /// Return a list of boundary 
        /// points for the given area.
        /// </summary>
        private static List<XYZ> MaakPuntArray(
          Area area)
        {
            SpatialElementBoundaryOptions opt
              = new SpatialElementBoundaryOptions();

            opt.SpatialElementBoundaryLocation
              = SpatialElementBoundaryLocation.Center;

            var boundaries = area.GetBoundarySegments(
              opt);

            return MaakPuntArray(boundaries);
        }

        /// <summary>
        /// Check whether this area contains a given point.
        /// </summary>
        public static bool AreaContains(this Area a, XYZ p1)
        {
            bool ret = false;
            var p = MaakPuntArray(a);
            PointInPoly pp = new PointInPoly();
            ret = pp.PolyGonContains(p, p1);
            return ret;
        }

        /// <summary>
        /// Check whether this room contains a given point.
        /// </summary>
        public static bool RoomContains(this Room r, XYZ p1)
        {
            bool ret = false;
            var p = MaakPuntArray(r);
            PointInPoly pp = new PointInPoly();
            ret = pp.PolyGonContains(p, p1);
            return ret;
        }

        /// <summary>
        /// Project an XYZ point to a UV one in the 
        /// XY plane by simply dropping the Z coordinate.
        /// </summary>
        public static UV TOUV(this XYZ point)
        {
            UV ret = new UV(point.X, point.Y);
            return ret;
        }
    }
    public class UVArray
    {
        List<UV> arrayPoints;
        public UVArray(List<XYZ> XYZArray)
        {
            arrayPoints = new List<UV>();
            foreach (var p in XYZArray)
            {
                arrayPoints.Add(p.TOUV());
            }
        }

        public UV get_Item(int i)
        {
            return arrayPoints[i];
        }

        public int Size
        {
            get
            {
                return arrayPoints.Count;
            }
        }
    }
    public class PointInPoly
    {
        /// <summary>
        /// Determine the quadrant of a polygon vertex 
        /// relative to the test point.
        /// </summary>
        Quadrant GetQuadrant(UV vertex, UV p)
        {
            return (vertex.U > p.U)
              ? ((vertex.V > p.V) ? 0 : 3)
              : ((vertex.V > p.V) ? 1 : 2);
        }

        /// <summary>
        /// Determine the X intercept of a polygon edge 
        /// with a horizontal line at the Y value of the 
        /// test point.
        /// </summary>
        double X_intercept(UV p, UV q, double y)
        {
            Debug.Assert(0 != (p.V - q.V),
              "unexpected horizontal segment");

            return q.U
              - ((q.V - y)
                * ((p.U - q.U) / (p.V - q.V)));
        }

        void AdjustDelta(
          ref int delta,
          UV vertex,
          UV next_vertex,
          UV p)
        {
            switch (delta)
            {
                // make quadrant deltas wrap around:
                case 3: delta = -1; break;
                case -3: delta = 1; break;
                // check if went around point cw or ccw:
                case 2:
                case -2:
                    if (X_intercept(vertex, next_vertex, p.V)
                      > p.U)
                    {
                        delta = -delta;
                    }
                    break;
            }
        }

        public bool PolyGonContains(List<XYZ> xyZArray, XYZ p1)
        {
            UVArray uva = new UVArray(xyZArray);
            return PolygonContains(uva, p1.TOUV());
        }

        /// <summary>
        /// Determine whether given 2D point lies within 
        /// the polygon.
        /// 
        /// Written by Jeremy Tammik, Autodesk, 2009-09-23, 
        /// based on code that I wrote back in 1996 in C++, 
        /// which in turn was based on C code from the 
        /// article "An Incremental Angle Point in Polygon 
        /// Test" by Kevin Weiler, Autodesk, in "Graphics 
        /// Gems IV", Academic Press, 1994.
        /// 
        /// Copyright (C) 2009 by Jeremy Tammik. All 
        /// rights reserved.
        /// 
        /// This code may be freely used. Please preserve 
        /// this comment.
        /// </summary>
        public bool PolygonContains(
          UVArray polygon,
          UV point)
        {
            // initialize
            Quadrant quad = GetQuadrant(
              polygon.get_Item(0), point);

            Quadrant angle = 0;

            // loop on all vertices of polygon
            Quadrant next_quad, delta;
            int n = polygon.Size;
            for (int i = 0; i < n; ++i)
            {
                UV vertex = polygon.get_Item(i);

                UV next_vertex = polygon.get_Item(
                  (i + 1 < n) ? i + 1 : 0);

                // calculate quadrant and delta from last quadrant

                next_quad = GetQuadrant(next_vertex, point);
                delta = next_quad - quad;

                AdjustDelta(
                  ref delta, vertex, next_vertex, point);

                // add delta to total angle sum
                angle = angle + delta;

                // increment for next step
                quad = next_quad;
            }

            // complete 360 degrees (angle of + 4 or -4 ) 
            // means inside

            return (angle == +4) || (angle == -4);

            // odd number of windings rule:
            // if (angle & 4) return INSIDE; else return OUTSIDE;
            // non-zero winding rule:
            // if (angle != 0) return INSIDE; else return OUTSIDE;
        }
    }
}
