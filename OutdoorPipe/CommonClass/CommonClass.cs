using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using System.IO;

namespace FFETOOLS
{
    #region
    public static class LineUtil
    {
        /// <summary>
        /// 计算两条线的交点
        /// </summary>
        /// <param name="curve1"></param>
        /// <param name="curve2"></param>
        /// <returns></returns>
        /// 
        public static XYZ IntersectionPoint(Curve curve1, Curve curve2)
        {
            IntersectionResultArray intersectionR = new IntersectionResultArray();
            SetComparisonResult comparisonR;
            comparisonR = curve1.Intersect(curve2, out intersectionR);
            XYZ intersectionResult = null;
            if (SetComparisonResult.Disjoint != comparisonR) // Disjoint无交点 
            {
                try
                {
                    if (!intersectionR.IsEmpty)
                    {
                        intersectionResult = intersectionR.get_Item(0).XYZPoint;
                    }
                }
                catch
                {

                }
            }
            return intersectionResult;
        }
    }
    public static class ElementIdExtension
    {
        public static Element GetElement(this ElementId eleid, Document doc)
        {
            return doc.GetElement(eleid);
        }
    }
    public static class SelectionFilterHelper
    {
        public static MultiSelectionFilter GetSelectionFilter(this Document doc, Func<Element, bool> func1, Func<Reference, bool> func2 = null)
        {
            return new MultiSelectionFilter(func1, func2);
        }
    }
    public class MultiSelectionFilter : ISelectionFilter
    {
        private Func<Element, bool> eleFunc;
        private Func<Reference, bool> refFunc;
        public MultiSelectionFilter(Func<Element, bool> func, Func<Reference, bool> func1)
        {
            eleFunc = func;
            refFunc = func1;
        }
        public bool AllowElement(Element elem)
        {
            return refFunc != null ? true : eleFunc(elem);
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return refFunc == null ? false : refFunc(reference);
        }
    }
    public static class ReferenceExtension
    {
        public static Element GetElement(this Reference thisref, Document doc)
        {
            return doc.GetElement(thisref);
        }
    }
    public static class ConnectorExtension
    {
        public static Connector GetConnectedCon(this Connector connector)
        {
            var result = default(Connector);
            var conectors = connector.AllRefs;
            var connectordir = connector.CoordinateSystem.BasisZ;
            var connectorOrigin = connector.Origin;

            foreach (Connector con in conectors)
            {

                if (con.ConnectorType == ConnectorType.End || con.ConnectorType == ConnectorType.Curve)
                {
                    var conOrigin = con.Origin;
                    var condir = con.CoordinateSystem.BasisZ;
                    if (connectorOrigin.IsAlmostEqualTo(conOrigin) && connectordir.IsOppositeDirection(condir))
                    {
                        result = con;
                    }
                }
            }
            return result;
        }
    }
    public static class VectorExtension
    {
        private static double precision = 0.000001;
        public static bool IsParallel(this XYZ vector1, XYZ vector2)
        {
            return vector1.IsSameDirection(vector2) || vector1.IsOppositeDirection(vector2);
        }
        /// <summary>
        /// 判断同向
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        public static bool IsSameDirection(this XYZ dir1, XYZ dir2)
        {
            bool result = false;

            double dotproduct = dir1.Normalize().DotProduct(dir2.Normalize());

            if (Math.Abs(dotproduct - 1) < precision)
            {
                result = true;
            }

            return result;
        }
        /// <summary>
        /// 判断反向
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        public static bool IsOppositeDirection(this XYZ dir1, XYZ dir2)
        {
            bool result = false;

            double dotproduct = dir1.Normalize().DotProduct(dir2.Normalize());

            if (Math.Abs(dotproduct + 1) < precision)
            {
                result = true;
            }

            return result;
        }
    }
    public static class MepcurveExtension
    {
        public static Line LocationLine(this MEPCurve mep)
        {
            Line result = null;

            result = (mep.Location as LocationCurve).Curve as Line;

            return result;
        }
    }
    public static class PipeExtension
    {
        public static FamilyInstance ElbowConnect(this Pipe pipe1, Pipe pipe2)
        {
            var line1 = pipe1.LocationLine();
            var line2 = pipe2.LocationLine();

            var line1copy = line1.Clone() as Line;
            var line2copy = line2.Clone() as Line;
            line2copy.MakeUnbound();
            line1copy.MakeUnbound();

            var intersection = line1copy.Intersect_cus(line2copy);

            if (intersection == null) return null;

            var newline1 = default(Line);
            if (line1.StartPoint().DistanceTo(intersection) <= line1.EndPoint().DistanceTo(intersection))
            {
                newline1 = Line.CreateBound(intersection, line1.EndPoint());
            }
            else
            {
                newline1 = Line.CreateBound(line1.StartPoint(), intersection);
            }

            var newline2 = default(Line);
            if (line2.StartPoint().DistanceTo(intersection) <= line2.EndPoint().DistanceTo(intersection))
            {
                newline2 = Line.CreateBound(intersection, line2.EndPoint());
            }
            else
            {
                newline2 = Line.CreateBound(line2.StartPoint(), intersection);
            }

            (pipe1.Location as LocationCurve).Curve = newline1;
            (pipe2.Location as LocationCurve).Curve = newline2;

            var doc = pipe1.Document;
            doc.Regenerate();

            var con1 = pipe1.ConnectorManager.Connectors.Cast<Connector>()
                .Where(m => m.ConnectorType == ConnectorType.Curve || m.ConnectorType == ConnectorType.End)
                .Where(m => m.Origin.IsAlmostEqualTo(intersection)).FirstOrDefault();

            var con2 = pipe2.ConnectorManager.Connectors.Cast<Connector>()
                .Where(m => m.ConnectorType == ConnectorType.Curve || m.ConnectorType == ConnectorType.End)
                .Where(m => m.Origin.IsAlmostEqualTo(intersection)).FirstOrDefault();

            var result = doc.Create.NewElbowFitting(con1, con2);

            return result;
        }

        public static Connector StartCon(this Pipe pipe1)
        {
            Connector result = null;

            var locationline = pipe1.LocationLine();


            var connectors = pipe1.ConnectorManager.Connectors.Cast<Connector>()
                .Where(m => m.ConnectorType == ConnectorType.End).ToList();

            var startcon = connectors.First(m => m.Origin.IsAlmostEqualTo(locationline.StartPoint()));
            result = startcon;

            return result;
        }

        public static Connector EndCon(this Pipe pipe1)
        {
            Connector result = null;

            var locationline = pipe1.LocationLine();


            var connectors = pipe1.ConnectorManager.Connectors.Cast<Connector>()
                .Where(m => m.ConnectorType == ConnectorType.End).ToList();

            var startcon = connectors.First(m => m.Origin.IsAlmostEqualTo(locationline.EndPoint()));
            result = startcon;

            return result;
        }
    }
    public static class LineExtension
    {
        public static XYZ StartPoint(this Line line)
        {
            if (line.IsBound)
                return line.GetEndPoint(0);
            return null;
        }
        public static XYZ EndPoint(this Line line)
        {
            if (line.IsBound)
                return line.GetEndPoint(1);
            return null;
        }

        public static XYZ Intersect_cus(this Line line, Plane p)
        {
            var lineorigin = line.Origin;
            var linedir = line.Direction;

            var pointOnLine = lineorigin + linedir;

            var trans = Transform.Identity;
            trans.Origin = p.Origin;
            trans.BasisX = p.XVec;
            trans.BasisY = p.YVec;
            trans.BasisZ = p.Normal;

            var point1 = lineorigin;
            var point2 = pointOnLine;

            var point1Intrans = trans.Inverse.OfPoint(point1);
            var point2Intrans = trans.Inverse.OfPoint(point2);

            point1Intrans = new XYZ(point1Intrans.X, point1Intrans.Y, 0);
            point2Intrans = new XYZ(point2Intrans.X, point2Intrans.Y, 0);

            var point1Inworld = trans.OfPoint(point1Intrans);
            var point2Inworld = trans.OfPoint(point2Intrans);

            var newlineInPlan = Line.CreateBound(point1Inworld, point2Inworld);

            var unboundnewLine = newlineInPlan.Clone() as Line;
            unboundnewLine.MakeUnbound();

            var unboundOriginalLine = line.Clone() as Line;
            unboundOriginalLine.MakeUnbound();

            return unboundnewLine.Intersect_cus(unboundOriginalLine);
        }

        public static XYZ Intersect_cus(this Line line1, Line line2)
        {
            var compareResulst = line1.Intersect(line2, out IntersectionResultArray intersectResult);

            if (compareResulst != SetComparisonResult.Disjoint)
            {
                var result = intersectResult.get_Item(0).XYZPoint;
                return result;
            }

            return null;
        }
    }
    public static class PointExtension
    {
        /// <summary>
        /// 投影到射线上
        /// </summary>
        /// <param name="po"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static XYZ ProjectToXLine(this XYZ po, Line l/*有限长度线段*/)
        {
            Line l1 = l.Clone() as Line;
            if (l1.IsBound)
            {
                l1.MakeUnbound();
            }
            return l1.Project(po).XYZPoint;
        }
        /// <summary>
        /// 浮点数相等时的精度
        /// </summary>
        private static double precision = 0.000001;

        /// <summary>
        /// 判断两double数值是否相等
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool IsEqual(double d1, double d2)
        {
            double diff = Math.Abs(d1 - d2);
            return diff < precision;

        }
        /// <summary>
        /// 判断点是否在线段上
        /// </summary>
        /// <param name="p"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static bool IsOnLine(this XYZ p, Line l)
        {
            XYZ end1 = l.GetEndPoint(0);
            XYZ end2 = l.GetEndPoint(1);

            XYZ vec_pToEnd1 = end1 - p;
            XYZ vec_pToEnd2 = end2 - p;

            double precision = 0.0000001d;

            if (p.DistanceTo(end1) < precision || p.DistanceTo(end2) < precision)
            {
                return true;
            }
            if (vec_pToEnd1.IsOppositeDirection(vec_pToEnd2))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断点是否在线 或线的延长线上
        /// </summary>
        /// <param name="p"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static bool IsXOnLine(this XYZ p, Line l)
        {
            double precision = 0.0000001d;
            var l1 = l.Clone() as Line;
            l1.MakeUnbound();
            if (p.DistanceTo(l1) < precision)
            {
                return true;
            }
            return false;
        }

        public static double DistanceTo(this XYZ p1, Line xline)
        {
            double result = double.NegativeInfinity;

            XYZ p1_onLine = p1.ProjectToXLine(xline);

            result = p1.DistanceTo(p1_onLine);

            return result;
        }
    }
    public static class CollectorHelper
    {
        public static IList<T> TCollector<T>(this Document doc)
        {
            Type type = typeof(T);
            return new FilteredElementCollector(doc).OfClass(type).Cast<T>().ToList();
        }
    }
    public static class TransactionHelper
    {
        public static void Invoke(this Document doc, Action<Transaction> action, string name = "Invoke")
        {
#if DEBUG
            LogHelper.LogException(delegate
            {
#endif
                using (Transaction transaction = new Transaction(doc, name))
                {
                    transaction.Start();
                    action(transaction);
                    bool flag = transaction.GetStatus() == (TransactionStatus)1;
                    if (flag)
                    {
                        transaction.Commit();
                    }
                }
#if DEBUG
            }, "c:\\revitExceptionlog.txt");
#endif

        }
        public static void Invoke(this Document doc, Action<Transaction> action, string name = "Invoke", bool ignorefailure = true)
        {
            LogHelper.LogException(delegate
            {
                using (Transaction transaction = new Transaction(doc, name))
                {
                    transaction.Start();

                    if (ignorefailure)
                        transaction.IgnoreFailure();

                    action(transaction);
                    bool flag = transaction.GetStatus() == TransactionStatus.Started;
                    if (flag)
                    {
                        transaction.Commit();
                    }
                }
            }, "c:\\revitExceptionlog.txt");
        }

        public static void SubtranInvoke(this Document doc, Action<SubTransaction> action)
        {
            using (SubTransaction subTransaction = new SubTransaction(doc))
            {
                subTransaction.Start();
                action(subTransaction);
                bool flag = subTransaction.GetStatus() == (TransactionStatus)1;
                if (flag)
                {
                    subTransaction.Commit();
                }


            }
        }
    }
    public static class LogHelper
    {
        public static void LogException(Action action, string path)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                LogWrite(e.ToString(), path);
            }
        }

        public static void LogWrite(string msg, string path, bool append = false)
        {
            StreamWriter sw = new StreamWriter(path, append);
            sw.WriteLine(msg);
            sw.Close();
        }
    }
    public static class TransactionExtension
    {
        public static void IgnoreFailure(this Transaction trans)
        {
            var options = trans.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new failure_ignore());
        }

        //public static void Invoke(this Document doc, Action<Transaction> action, string transactionName = "aaa")
        //{
        //    Transaction ts = new Transaction(doc, transactionName);
        //    LogHelper.LogException(delegate
        //    {
        //        ts.Start();
        //        action(ts);
        //        ts.Commit();
        //    }, @"c:\transactionException.txt");
        //}
    }
    public class failure_ignore : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            failuresAccessor.DeleteAllWarnings();
            //failuresAccessor.DeleteElements(failuresAccessor.el);
            return FailureProcessingResult.Continue;
        }
    }
    public static class GeometryElementExtension
    {
        public static List<GeometryObject> GetGeometries(this GeometryElement geoele)
        {
            List<GeometryObject> result = new List<GeometryObject>();
            var enu = geoele.GetEnumerator();
            while (enu.MoveNext())
            {
                var geoobj = enu.Current as GeometryObject;
                if (geoobj != null)
                {
                    result.Add(geoobj);
                }
            }
            return result;
        }
        public static List<Face> GetFaces(this GeometryElement geoele)
        {
            List<Face> result = new List<Face>();
            var geoobjs = geoele.GetGeometries();
            foreach (GeometryObject geoobbj in geoobjs)
            {
                result.AddRange(geoobbj.GetFacesOfGeometryObject());
            }
            return result;
        }
        public static List<Edge> GetEdges(this GeometryElement geoele)
        {
            List<Edge> result = new List<Edge>();
            var geoobjs = geoele.GetGeometries();
            foreach (GeometryObject geoobj in geoobjs)
            {
                result.AddRange(geoobj.GetEdgesofGeometryObject());
            }
            return result;
        }
        public static List<XYZ> GetPoints(this GeometryElement geoele)
        {
            List<XYZ> result = new List<XYZ>();

            //var geoobjs = geoele.GetGeometries();
            var geoedges = geoele.GetEdges();

            var points = new List<XYZ>();

            foreach (var edge in geoedges)
            {
                var curve = edge.AsCurve();

                var startpoint = curve.GetEndPoint(0);
                var endpoint = curve.GetEndPoint(1);

                //判断点是否位置上重合 如果不重合 则添加进结果列表

                var startflag = false;
                var endflag = false;
                points.ForEach(m =>
                {
                    if (m.DistanceTo(startpoint) < 1e-6) startflag = true;
                    if (m.DistanceTo(endpoint) < 1e-6) endflag = true;
                });

                //MessageBox.Show(startflag.ToString() + Environment.NewLine +
                //                endflag.ToString());

                if (!startflag)
                    points.Add(startpoint);
                if (!endflag)
                    points.Add(endpoint);
            }

            result = points;
            return result;
        }

    }
    public static class GeometryObjectExtension
    {
        public static IList<Face> GetFacesOfGeometryObject(this GeometryObject geoobj)
        {
            IList<Face> result = new List<Face>();
            List<Face> temresult = new List<Face>();
            if (geoobj is GeometryElement)
            {
                GeometryElement geoele = geoobj as GeometryElement;
                foreach (GeometryObject geoitem in geoele)
                {
                    temresult.AddRange(GetFacesOfGeometryObject(geoitem));
                }
            }
            else if (geoobj is GeometryInstance)
            {
                GeometryElement geoele = (geoobj as GeometryInstance).SymbolGeometry;
                foreach (GeometryObject obj in geoele)
                {
                    if (obj is Solid)
                    {
                        //result.Add(obj as Face);
                        temresult.AddRange(GetFacesOfGeometryObject(obj));
                    }
                }
            }
            else if (geoobj is Solid)
            {
                Solid solid = geoobj as Solid;
                foreach (Face face in solid.Faces)
                {
                    temresult.Add(face);
                }
            }
            else if (geoobj is Face)
            {
                temresult.Add(geoobj as Face);
            }
            result = temresult;
            return result;
        }


        //public static IList<Face> GetFacesOfGeometryObject(this GeometryObject geoobj)
        //{
        //    IList<Face> result = new List<Face>();
        //    List<Face> temresult = new List<Face>();

        //    if (geoobj is GeometryInstance)
        //    {
        //        GeometryElement geoele = (geoobj as GeometryInstance).SymbolGeometry;
        //        foreach (GeometryObject obj in geoele)
        //        {
        //            if (obj is Solid)
        //            {
        //                //result.Add(obj as Face);
        //                temresult.AddRange(GetFacesOfGeometryObject(obj));
        //            }
        //        }
        //    }

        //    else if (geoobj is Solid)
        //    {
        //        Solid solid = geoobj as Solid;
        //        foreach (Face face in solid.Faces)
        //        {
        //            temresult.Add(face);
        //        }
        //    }
        //    result = temresult;
        //    return result;
        //}

        public static IList<Solid> GetSolidOfGeometryObject(this GeometryObject geoobj)
        {
            IList<Solid> result = new List<Solid>();
            List<Solid> temresult = new List<Solid>();
            if (geoobj is GeometryElement)
            {
                GeometryElement geoele = geoobj as GeometryElement;
                foreach (GeometryObject geoitem in geoele)
                {
                    temresult.AddRange(GetSolidOfGeometryObject(geoitem));
                }
            }
            else if (geoobj is GeometryInstance)
            {
                GeometryElement geoele = (geoobj as GeometryInstance).SymbolGeometry;
                foreach (GeometryObject obj in geoele)
                {
                    if (obj is Solid)
                    {
                        //result.Add(obj as Face);
                        temresult.AddRange(GetSolidOfGeometryObject(obj));
                    }
                }
            }
            else if (geoobj is Solid)
            {
                Solid solid = geoobj as Solid;
                temresult.Add(solid);
                //foreach (Face face in solid.Faces)
                //{
                //    temresult.Add(face);
                //}
            }
            //else if (geoobj is Face)
            //{
            //    temresult.Add(geoobj as Face);
            //}
            result = temresult;
            return result;
        }
        public static IList<Edge> GetEdgesofGeometryObject(this GeometryObject geoobj)
        {
            IList<Edge> result = new List<Edge>();
            List<Edge> temresult = new List<Edge>();
            if (geoobj is GeometryElement)
            {
                GeometryElement geoele = geoobj as GeometryElement;
                foreach (GeometryObject geoitem in geoele)
                {
                    temresult.AddRange(GetEdgesofGeometryObject(geoitem));
                }
            }
            else if (geoobj is GeometryInstance)
            {
                GeometryElement geoele = (geoobj as GeometryInstance).SymbolGeometry;
                foreach (GeometryObject obj in geoele)
                {
                    if (obj is Solid)
                    {
                        //result.Add(obj as Face);
                        temresult.AddRange(GetEdgesofGeometryObject(obj));
                    }
                }
            }
            else if (geoobj is Solid)
            {
                Solid solid = geoobj as Solid;
                //temresult.Add(solid);
                foreach (Face face in solid.Faces)
                {
                    temresult.AddRange(GetEdgesofGeometryObject(face));
                }
            }
            else if (geoobj is Face)
            {
                Face face = geoobj as Face;
                //foreach (EdgeArrayArray edgearrayarray in face.EdgeLoops)
                {
                    foreach (EdgeArray edgeArray in face.EdgeLoops)
                    {
                        var enu = edgeArray.GetEnumerator();
                        while (enu.MoveNext())
                        {
                            var edge = enu.Current as Edge;
                            if (edge != null)
                                temresult.Add(edge);
                        }
                    }
                }
            }
            else if (geoobj is Edge)
            {
                temresult.Add(geoobj as Edge);
            }
            result = temresult;
            return result;
        }



    }
    #endregion

    #region

    //详图线的过滤条件
    public class DetailineSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return e is DetailLine;
        }
        public bool AllowReference(Reference r, XYZ p)
        {
            return true;
        }
    }
    //墙的过滤条件
    public class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return e is Wall;
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            return true;
        }
    }
    //风管的过滤条件
    public class DuctSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return e is Duct;
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            return true;
        }
    }
    //水管的过滤条件
    public class PipeSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return e is Pipe;
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            return true;
        }
    }
    //排水管的过滤条件
    public class DrainagePipeSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            if (e.Category.Name == "管道占位符")
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            return true;
        }
    }
    #endregion

}
