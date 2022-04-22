//using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Architecture;
//using Autodesk.Revit.DB.ExtensibleStorage;
//using Autodesk.Revit.DB.Mechanical;
//using Autodesk.Revit.DB.Plumbing;
//using Autodesk.Revit.DB.Structure;
//using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Selection;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Windows;
//using Point = System.Windows.Point;

//namespace FFETOOLS
//{
//    [Transaction(TransactionMode.Manual)]
//    public class MyApp测试2 : IExternalCommand
//    {
//        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
//        {
//            try
//            {
//                UIApplication uiapp = commandData.Application;
//                UIDocument uidoc = uiapp.ActiveUIDocument;
//                Document doc = uidoc.Document;
//                Selection sel = uidoc.Selection;

//                FilteredElementCollector col = new FilteredElementCollector(doc)
//                                              .WhereElementIsNotElementType()
//                                              .OfCategory(BuiltInCategory.INVALID)
//                                              .OfClass(typeof(Wall));

//                foreach (Element e in col)
//                {
//                    Debug.Print(e.Name);
//                }
//                using (Transaction trans = new Transaction(doc, "name"))
//                {
//                    trans.Start();

//                    trans.Commit();
//                }
//            }
//            catch (Exception e)
//            {
//                messages = e.Message;
//                return Result.Failed;
//            }
//            return Result.Succeeded;
//        }
//    }

//    public static class SPEV1Auxiliary
//    {
//        public static bool Pnpoly(Point p, IList<IList<Point>> s)
//        {
//            if (s == null)
//            {
//                return false;
//            }
//            foreach (IList<Point> points in s)
//            {
//                if (Pnpoly(p, points))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        public static bool Pnpoly(Point p, IList<Point> s)
//        {
//            if (s == null || s.Count < 3)
//            {
//                return false;
//            }
//            int nvert = s.Count;
//            bool c = false;
//            for (int i = 0, j = nvert - 1; i < nvert; j = i++)
//            {
//                if (((s[i].Y > p.Y) != (s[j].Y > p.Y)) &&
//                 (p.X < (s[j].X - s[i].X) *
//                 (p.Y - s[i].Y) / (s[j].Y - s[i].Y) + s[i].X))
//                {
//                    c = !c;
//                }
//            }
//            return c;
//        }

//        public static bool Intersection(Point a, Point b, Point c, Point d)
//        {
//            /*
//            快速排斥：
//            两个线段为对角线组成的矩形，如果这两个矩形没有重叠的部分，那么两条线段是不可能出现重叠的
//            */
//            if (!(Math.Min(a.X, b.X) <= Math.Max(c.X, d.X) && Math.Min(c.Y, d.Y) <= Math.Max(a.Y, b.Y) && Math.Min(c.X, d.X) <= Math.Max(a.X, b.X) && Math.Min(a.Y, b.Y) <= Math.Max(c.Y, d.Y)))//这里的确如此，这一步是判定两矩形是否相交
//                                                                                                                                                                                                //1.线段ab的低点低于cd的最高点（可能重合） 2.cd的最左端小于ab的最右端（可能重合）
//                                                                                                                                                                                                //3.cd的最低点低于ab的最高点（加上条件1，两线段在竖直方向上重合） 4.ab的最左端小于cd的最右端（加上条件2，两直线在水平方向上重合）
//                                                                                                                                                                                                //综上4个条件，两条线段组成的矩形是重合的
//                /*特别要注意一个矩形含于另一个矩形之内的情况*/
//                return false;
//            /*
//            跨立实验：
//            如果两条线段相交，那么必须跨立，就是以一条线段为标准，另一条线段的两端点一定在这条线段的两段
//            也就是说a b两点在线段cd的两端，c d两点在线段ab的两端
//            */
//            double u, v, w, z;//分别记录两个向量
//            u = (c.X - a.X) * (b.Y - a.Y) - (b.X - a.X) * (c.Y - a.Y);
//            v = (d.X - a.X) * (b.Y - a.Y) - (b.X - a.X) * (d.Y - a.Y);
//            w = (a.X - c.X) * (d.Y - c.Y) - (d.X - c.X) * (a.Y - c.Y);
//            z = (b.X - c.X) * (d.Y - c.Y) - (d.X - c.X) * (b.Y - c.Y);
//            return (u * v <= 0.00000001 && w * z <= 0.00000001);
//        }

//        public static bool Intersection(Point x, Point y, IList<IList<Point>> shapes)
//        {
//            if (shapes == null)
//            {
//                return false;
//            }
//            foreach (IList<Point> shape in shapes)
//            {
//                if (Intersection(x, y, shape))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        public static bool Intersection(Point x, Point y, IList<Point> shape)
//        {
//            Point? parent = null;
//            if (shape == null || shape.Count < 3)
//            {
//                return false;
//            }
//            for (int i = shape.Count - 1; i >= 0; i--)
//            {
//                Point current = shape[i];
//                if (i != 0 && current == shape[0] && parent == shape[0])
//                {
//                    continue;
//                }
//                if (parent == null)
//                {
//                    int j = shape.Count - 1;
//                    while ((parent = shape[j]) != shape[0]) j--;
//                }
//                if (Intersection(parent.Value, current, x, y))
//                {
//                    return true;
//                }
//                parent = current;
//            }
//            return false;
//        }

//        public static bool PointIsInLine(PointF p, PointF x, PointF y, double range)
//        {
//            double cross = (y.X - x.X) * (p.X - x.X) + (y.Y - x.Y) * (p.Y - x.Y);
//            if (cross <= 0)
//            {
//                return false;
//            }
//            double d2 = (y.X - x.X) * (y.X - x.X) + (y.Y - x.Y) * (y.Y - x.Y);
//            if (cross >= d2)
//            {
//                return false;
//            }
//            double r = cross / d2;
//            double px = x.X + (y.X - x.X) * r;
//            double py = x.Y + (y.Y - x.Y) * r;
//            return Math.Sqrt((p.X - px) * (p.X - px) + (py - p.Y) * (py - p.Y)) <= range;
//        }

//        private static int FindShortsNodeLine(IList<Point> s, bool[] flags, int from, int to, IList<IList<Point>> shapes, ref double distances)
//        {
//            Tuple<int, double> m = null;
//            Point key = s[from];
//            for (int i = 0; i < s.Count; i++)
//            {
//                if (flags[i])
//                {
//                    continue;
//                }
//                if (Intersection(key, s[i], shapes))
//                {
//                    continue;
//                }
//                double distance = GetDistance(key, s[i]);
//                Tuple<int, double> n = new Tuple<int, double>(i, distance);
//                if (i == to)
//                {
//                    m = n;
//                    break;
//                }
//                if (m == null || m.Item2 > n.Item2) // 预测下一个节点
//                {
//                    m = n;
//                }
//            }
//            if (m == null)
//            {
//                return -1;
//            }
//            flags[m.Item1] = true;
//            return m.Item1;
//        }

//        public static double GetDistance(Point x, Point y)
//        {
//            int x1 = Math.Abs(y.X - x.X);
//            int y1 = Math.Abs(y.Y - x.Y);
//            return Math.Sqrt(x1 * x1 + y1 * y1);
//        }

//        public static Tuple<double, IList<int>> FindLine(int from, int to, IList<Point> s, IList<IList<Point>> shapes)
//        {
//            if (s == null || shapes == null || s.Count <= 0 || shapes.Count <= 0)
//            {
//                return new Tuple<double, IList<int>>(0, new List<int>());
//            }
//            if (from < 0 || to < 0 || from >= s.Count || to >= s.Count || from == to)
//            {
//                return new Tuple<double, IList<int>>(0, new List<int>());
//            }
//            Tuple<double, IList<int>> bof = FindShortsUMLLine(from, to, s, shapes);
//            Tuple<double, IList<int>> eof = FindShortsUMLLine(to, from, s, shapes);
//            if (bof.Item1 <= eof.Item1)
//            {
//                return bof;
//            }
//            else
//            {
//                IList<int> pathways = eof.Item2;
//                IList<int> lines = new List<int>(pathways.Count);
//                for (int i = pathways.Count - 1; i >= 0; i--)
//                {
//                    lines.Add(pathways[i]);
//                }
//                return new Tuple<double, IList<int>>(eof.Item1, lines);
//            }
//        }

//        private static Tuple<double, IList<int>> FindShortsUMLLine(int from, int to, IList<Point> s, IList<IList<Point>> shapes)
//        {
//            Tuple<double, IList<int>> result = FindShortsZHTLine(from, to, s, shapes);
//            IList<int> lines = result.Item2; // 路径三插法
//            bool restatistics = false;
//            for (int cc = 0, kk = 2; cc < kk; cc++) // A*B
//            {
//                for (int i = 1, n = lines.Count - 1; i < n; i++)
//                {
//                    int left = lines[i - 1]; // 左边
//                    int middle = lines[i]; // 中间
//                    int right = lines[i + 1]; // 右边
//                    Tuple<double, int> min = null;
//                    for (int j = 0; j < s.Count; j++)
//                    {
//                        if (!Intersection(s[j], s[right], shapes) &&
//                            !Intersection(s[j], s[left], shapes))
//                        {
//                            double distance = GetDistance(s[j], s[left]) + GetDistance(s[right], s[j]);
//                            if (min == null || min.Item1 > distance)
//                            {
//                                min = new Tuple<double, int>(distance, j);
//                            }
//                        }
//                    }
//                    if (min != null)
//                    {
//                        int nn = min.Item2;
//                        restatistics = true;
//                        lines[i] = nn;
//                    }
//                }
//            }
//            if (restatistics)
//            {
//                IList<int> pathways = new List<int>(lines.Count);
//                ISet<int> set = new HashSet<int>();
//                for (int i = 0; i < lines.Count; i++)
//                {
//                    int nn = lines[i];
//                    if (!set.Add(nn))
//                    {
//                        continue;
//                    }
//                    pathways.Add(nn);
//                }
//                result = new Tuple<double, IList<int>>(GetDistance(s, pathways), pathways);
//            }
//            return result;
//        }

//        private static Tuple<double, IList<int>> FindShortsZHTLine(int from, int to, IList<Point> s, IList<IList<Point>> shapes)
//        {
//            Tuple<double, IList<int>> result = FindShortsRANLine(from, to, s, shapes);
//            IList<int> lines = result.Item2;
//            for (int cc = 0, kk = 2; cc < kk; cc++)
//            {
//                for (int i = 0; i < lines.Count; i++)
//                {
//                    int key = lines[i]; // 假定关键点
//                    for (int j = 0; j < lines.Count; j++)
//                    {
//                        int current = lines[j];
//                        if (key == current)
//                        {
//                            continue;
//                        }
//                        if (!Intersection(s[current], s[to], shapes))
//                        {
//                            for (int ii = j + 1; ii < lines.Count;)
//                            {
//                                int pp = lines[ii];
//                                if (pp == to || pp == from)
//                                {
//                                    break;
//                                }
//                                lines.RemoveAt(ii);
//                            }
//                            continue;
//                        }
//                    }
//                    for (int j = 0; j < lines.Count; j++)
//                    {
//                        int current = lines[j];
//                        if (key == current)
//                        {
//                            continue;
//                        }
//                        int depth = Math.Abs(j - i);
//                        if (!Intersection(s[key], s[current], shapes) && depth >= 3)
//                        {
//                            for (int ii = i, ll = i; ii < j; ii++)
//                            {
//                                int pp = lines[ii];
//                                if (pp == from || pp == to)
//                                {
//                                    ll++;
//                                    continue;
//                                }
//                                lines.RemoveAt(ll);
//                            }
//                            if (lines.Count >= 3 && j + 2 < lines.Count && !Intersection(s[key], s[lines[j + 1]], shapes) &&
//                                !Intersection(s[key], s[lines[j + 2]], shapes))
//                            {
//                                lines.RemoveAt(j + 1);
//                            }
//                        }
//                    }
//                }
//            }
//            int n = -1;
//            do
//            {
//                n = lines.IndexOf(from);
//                if (n < 0)
//                {
//                    continue;
//                }
//                for (int i = n; i <= n; i++)
//                {
//                    lines.RemoveAt(0);
//                }
//            } while (n > -1);
//            if (lines.Count > 0 && lines[0] != from)
//            {
//                lines.Insert(0, from);
//            }
//            if (lines.Count > 0 && lines[lines.Count - 1] != to)
//            {
//                lines.Add(to);
//            }
//            IList<int> pathways = new List<int>();
//            ISet<int> set = new HashSet<int>();
//            for (int i = 0; i < lines.Count; i++)
//            {
//                n = lines[i];
//                if (!set.Add(n))
//                {
//                    continue;
//                }
//                pathways.Add(n);
//            }
//            return new Tuple<double, IList<int>>(GetDistance(s, pathways), pathways);
//        }

//        private static Tuple<double, IList<int>> FindShortsRANLine(int from, int to, IList<Point> s, IList<IList<Point>> shapes)
//        {
//            Tuple<double, IList<int>> linetrace = FindShortsNAPLine(from, to, s, shapes);
//            IList<int> lines = linetrace.Item2;
//            bool unintersection = true;
//            int? parent = null;
//            for (int cc = 0; cc < 2; cc++)
//            {
//                parent = null;
//                for (int i = 0; i < lines.Count; i++)
//                {
//                    if (lines.Count > s.Count)
//                    {
//                        break;
//                    }
//                    int current = lines[i];
//                    if (parent != null &&
//                        Intersection(s[parent.Value], s[current], shapes))
//                    {
//                        IList<int> node = FindShortsNAPLine(parent.Value, lines[i], s, shapes).Item2;
//                        for (int jj = 1, kk = i, nn = (node.Count - 1); jj < nn; jj++, kk++) // 三插法
//                        {
//                            unintersection = false;
//                            lines.Insert(kk, node[jj]);
//                        }
//                    }
//                    parent = lines[i];
//                }
//            }
//            for (int c = 0; c < 1; c++)
//            {
//                parent = null;
//                for (int i = 0; i < lines.Count; i++)
//                {
//                    if (parent == null)
//                    {
//                        parent = lines[i];
//                    }
//                    else
//                    {
//                        if (!Intersection(s[parent.Value], s[lines[i]], shapes))
//                        {
//                            parent = lines[i];
//                        }
//                        else
//                        {
//                            parent = lines[i];
//                            lines.RemoveAt(i);
//                        }
//                    }
//                }
//            }
//            if (unintersection)
//            {
//                return linetrace;
//            }
//            double distances = GetDistance(s, lines);
//            return new Tuple<double, IList<int>>(distances, lines);
//        }

//        private static Tuple<double, IList<int>> FindShortsNAPLine(int from, int to, IList<Point> s, IList<IList<Point>> shapes)
//        {
//            Tuple<double, IList<int>> bof = FindShortsNEGLine(from, to, s, shapes);
//            Tuple<double, IList<int>> eof = FindShortsNEGLine(to, from, s, shapes);
//            if (bof.Item1 > 0 || bof.Item2.Count > eof.Item2.Count)
//            {
//                return bof;
//            }
//            else
//            {
//                IList<int> pathways = eof.Item2;
//                IList<int> lines = new List<int>(pathways.Count);
//                for (int i = pathways.Count - 1; i >= 0; i--)
//                {
//                    lines.Add(pathways[i]);
//                }
//                return new Tuple<double, IList<int>>(eof.Item1, lines);
//            }
//        }

//        private static double GetDistance(IList<Point> s, IList<int> pathways)
//        {
//            double distances = 0;
//            int? parent = null;
//            if (pathways != null && s != null)
//            {
//                for (int i = 0; i < pathways.Count; i++)
//                {
//                    int current = pathways[i];
//                    if (parent != null)
//                    {
//                        distances += GetDistance(s[current], s[parent.Value]);
//                    }
//                    parent = current;
//                }
//            }
//            return distances;
//        }

//        private static Tuple<double, IList<int>> FindShortsNEGLine(int from, int to, IList<Point> s, IList<IList<Point>> shapes)
//        {
//            bool[] flags = new bool[s.Count];
//            flags[from] = true;
//            int r = from;
//            double distances = 0;
//            List<int> pathways = new List<int>();
//            if (from == to)
//            {
//                pathways.Add(from);
//            }
//            else
//            {
//                do
//                {
//                    r = FindShortsNodeLine(s, flags, r, to, shapes, ref distances);
//                    if (r == -1)
//                    {
//                        int i = pathways.Count - 1;
//                        if (i >= 0 && i < pathways.Count)
//                        {
//                            pathways.RemoveAt(i);
//                        }
//                        i = pathways.Count - 1;
//                        if (i >= 0 && i < pathways.Count)
//                        {
//                            r = pathways[i];
//                        }
//                        continue;
//                    }
//                    Point key = s[r];
//                    int? shortspathnode = null;
//                    double? shortsnodedistance = null;
//                    for (int i = 0; i < pathways.Count; i++)
//                    {
//                        if (Intersection(key, s[pathways[i]], shapes))
//                        {
//                            continue;
//                        }
//                        double distance = GetDistance(key, s[i]);
//                        if (shortsnodedistance == null)
//                        {
//                            shortspathnode = pathways[i];
//                            shortsnodedistance = distance;
//                        }
//                        else
//                        {
//                            if (shortsnodedistance > distance)
//                            {
//                                int lastshortspathnode = shortspathnode.Value;
//                                shortspathnode = pathways[i];
//                                shortsnodedistance = distance;
//                                pathways.Remove(lastshortspathnode);
//                            }
//                        }
//                    }
//                    pathways.Add(r);
//                } while (r != -1 && r != to);
//                if (pathways.Count > 0)
//                {
//                    pathways.Insert(0, from);
//                }
//            }
//            distances = GetDistance(s, pathways);
//            return new Tuple<double, IList<int>>(distances, pathways);
//        }

//        public static double GetDistance(int from, int to, IList<Point> s, IList<IList<Point>> shapes)
//        {
//            return FindLine(from, to, s, shapes).Item1;
//        }

//        public static IList<int> FindLine(int from, int to, IList<Point> s, IList<IList<Point>> shapes, out double distances)
//        {
//            Tuple<double, IList<int>> trace = SPEV1Auxiliary.FindLine(from, to, s, shapes);
//            distances = trace.Item1;
//            return trace.Item2;
//        }

//        public static IList<int> GetLine(int from, int to, IList<Point> s, IList<IList<Point>> shapes) // P*
//        {
//            Tuple<double, IList<int>> way = FindLine(from, to, s, shapes);
//            IList<int> pathways = way.Item2;
//            if (pathways == null)
//            {
//                pathways = new List<int>();
//            }
//            return pathways;
//        }
//    }
//}
