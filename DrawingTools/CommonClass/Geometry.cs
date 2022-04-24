using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace FFETOOLS
{
    public class Geometry
    {
        public const double _epsDouble = 1.0e-9;
        public const double _epsPoint = 0.0001;
        public const double _epsAngle = 0.0001;

        static public bool LessThan(double val1,double val2) 
        {
            return (val1 - val2 < -_epsDouble);
        }

        static public bool GreaterThan(double val1,double val2) 
        {
            return (val1 - val2 > _epsDouble);
        }

        static public bool IsEqual(double val1, double val2) 
        {
            return !LessThan(val1, val2) && !LessThan(val2, val1);
        }

        static public bool Lessthan_Or_Equal(double val1,double val2) 
        {
            return !GreaterThan(val1, val2);
        }

        static public bool Greaterthan_Or_Equal(double val1,double val2) 
        {
            return !LessThan(val1, val2);
        }

        static public bool IsParallel(XYZ a, XYZ b)
        {
            double angle = a.AngleTo(b);
            return _epsDouble > angle || IsEqual(angle, Math.PI);
        }

        static public bool IsVertical(XYZ a, XYZ b)
        {
            double angle = a.AngleTo(b);
            return IsEqual(angle, Math.PI / 2.0);
        }

        static public XYZ RotateTo(XYZ List, double angle, XYZ axis)
        {
            /*
            double x = List.X * Math.Cos(angle) + (axis.Y * List.Z - axis.Z * List.Y) * Math.Sin(angle) + 
             * axis.X * (axis.X * List.X + axis.Y * List.Y + axis.Z * List.Z) * (1 - Math.Cos(angle));

            double y = List.Y * Math.Cos(angle)+(axis.X * List.Z- axis.Z * List.X)*Math.Sin(angle) + 
             * axis.Y *(axis.X * List.X + axis.Y * List.Y + axis.Z * List.Z)*(1 - Math.Cos(angle));

            double z = List.Z * Math.Cos(angle) + (axis.X * List.Y - axis.Y * List.X) * Math.Sin(angle) + 
             * axis.Z * (axis.X * List.X + axis.Y * List.Y + axis.Z * List.Z) * (1 - Math.Cos(angle));

            XYZ newVector = new XYZ(x, y, z);
            return newVector;
             */
            Transform transform = Transform.CreateRotationAtPoint(axis, angle, new XYZ(0, 0, 0));
            XYZ newVector = TransformPoint(List, transform);
            return newVector;
        }

        static public XYZ CalculateFootPoint(Line line, XYZ point)
        {
            XYZ ptStart = line.GetEndPoint(0);
            XYZ ptEnd = line.GetEndPoint(1);
            return CalculateFootPoint(ptStart, ptEnd, point);
        }

        static public XYZ CalculateFootPoint(XYZ ptStart, XYZ ptEnd, XYZ point)
        {
            XYZ res = null;
            double a = Math.Sqrt((ptEnd.X - ptStart.X) * (ptEnd.X - ptStart.X) +
                       (ptEnd.Y - ptStart.Y) * (ptEnd.Y - ptStart.Y) +
                       (ptEnd.Z - ptStart.Z) * (ptEnd.Z - ptStart.Z));

            double b = (ptEnd.X - ptStart.X) * (point.X - ptStart.X) +
                       (ptEnd.Y - ptStart.Y) * (point.Y - ptStart.Y) +
                       (ptEnd.Z - ptStart.Z) * (point.Z - ptStart.Z);

            a = b / (a * a);

            double x = ptStart.X + (ptEnd.X - ptStart.X) * a;
            double y = ptStart.Y + (ptEnd.Y - ptStart.Y) * a;
            double z = ptStart.Z + (ptEnd.Z - ptStart.Z) * a;
            res = new XYZ(x, y, z);
            return res;
        }

        static public XYZ CalculateFootPoint(Arc arc, XYZ point)
        {
            XYZ ptCenter = arc.Center;
            XYZ ptStart = arc.GetEndPoint(0);
            XYZ ptEnd = arc.GetEndPoint(1);
            return CalculateFootPoint(ptStart, ptEnd, ptCenter, point);
        }

        static public XYZ CalculateFootPoint(XYZ ptStart, XYZ ptEnd, XYZ ptCenter, XYZ point)
        {
            double radius = ptCenter.DistanceTo(ptStart);
            XYZ newPoint = new XYZ(point.X, point.Y, ptCenter.Z);
            XYZ direction = newPoint - ptCenter;
            direction = direction.Normalize() * radius;
            XYZ res = ptCenter + direction;

            return res;
        }

        static public bool PointAtLineLeft(XYZ pt, XYZ ptStart, XYZ ptEnd)
        {
            double A = ptEnd.Y-ptStart.Y; 
            double B = ptStart.X-ptEnd.X;
            double C = ptEnd.X * ptStart.Y - ptStart.X * ptEnd.Y;

            double D = A * pt.X + B * pt.Y + C;

            if (D < 0.0)
                return true;
            return false;
        }

        static public XYZ TransformPoint(XYZ point, Transform transform)
        {
            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            //transform basis of the old coordinate system in the new coordinate // system
            XYZ b0 = transform.get_Basis(0);
            XYZ b1 = transform.get_Basis(1);
            XYZ b2 = transform.get_Basis(2);
            XYZ origin = transform.Origin;

            //transform the origin of the old coordinate system in the new 
            //coordinate system
            double xTemp = x * b0.X + y * b1.X + z * b2.X + origin.X;
            double yTemp = x * b0.Y + y * b1.Y + z * b2.Y + origin.Y;
            double zTemp = x * b0.Z + y * b1.Z + z * b2.Z + origin.Z;

            return new XYZ(xTemp, yTemp, zTemp);
        }

        static public BoundingBoxUV CalculateBoundingBox2D(Line line)
        {
            XYZ ptStart = line.GetEndPoint(0);
            XYZ ptEnd = line.GetEndPoint(1);
            double minX = ptStart.X, maxX = ptEnd.X, minY = ptStart.Y, maxY = ptEnd.Y;
            if (minX > maxX)
            {
                minX = ptEnd.X;
                maxX = ptStart.X;
            }
            if (minY > maxY)
            {
                minY = ptEnd.Y;
                maxY = ptStart.Y;
            }
            BoundingBoxUV res = new BoundingBoxUV(minX, minY, maxX, maxY);
            return res;
        }

        static public void GetArcAngles(Arc arc, ref double startAngle, ref double endAngle)
        {
            XYZ ptStart = arc.GetEndPoint(0);
            XYZ ptEnd = arc.GetEndPoint(1);
            XYZ ptCenter = arc.Center;
            startAngle = XYZ.BasisX.AngleOnPlaneTo(ptStart - ptCenter, XYZ.BasisZ);
            endAngle = XYZ.BasisX.AngleOnPlaneTo(ptEnd - ptCenter, XYZ.BasisZ);
            if (IsEqual(arc.Normal.Z, -1))
            {
                endAngle = (float)(XYZ.BasisX.AngleOnPlaneTo(ptStart - ptCenter, XYZ.BasisZ));
                startAngle = (float)(XYZ.BasisX.AngleOnPlaneTo(ptEnd - ptCenter, XYZ.BasisZ));
            }
            if (startAngle > endAngle)
                startAngle -= Math.PI * 2;
        }

        static public BoundingBoxUV CalculateBoundingBox2D(Arc arc)
        {
            XYZ ptStart = arc.GetEndPoint(0);
            XYZ ptEnd = arc.GetEndPoint(1);
            XYZ ptCenter = arc.Center;
            double radius = arc.Radius;

            double minX = ptStart.X, maxX = ptEnd.X, minY = ptStart.Y, maxY = ptEnd.Y;
            if (minX > maxX)
            {
                minX = ptEnd.X;
                maxX = ptStart.X;
            }
            if (minY > maxY)
            {
                minY = ptEnd.Y;
                maxY = ptStart.Y;
            }

            Line xAxis = Line.CreateUnbound(ptCenter, XYZ.BasisX);
            IntersectionResultArray resultArray1;
            SetComparisonResult resIntersect1 = arc.Intersect(xAxis, out resultArray1);
            if (resIntersect1 == Autodesk.Revit.DB.SetComparisonResult.Overlap)
            {
                if(resultArray1.Size > 0)
                    maxX = ptCenter.X + radius;
            }

            Line xNagetiveAxis = Line.CreateUnbound(ptCenter, -XYZ.BasisX);
            IntersectionResultArray resultArray2;
            SetComparisonResult resIntersect2 = arc.Intersect(xNagetiveAxis, out resultArray2);
            if (resIntersect2 == Autodesk.Revit.DB.SetComparisonResult.Overlap)
            {
                if(resultArray2.Size > 0)
                    minX = ptCenter.X - radius;
            }

            Line yAxis = Line.CreateUnbound(ptCenter, XYZ.BasisY);
            IntersectionResultArray resultArray3;
            SetComparisonResult resIntersect3 = arc.Intersect(yAxis, out resultArray3);
            if (resIntersect3 == Autodesk.Revit.DB.SetComparisonResult.Overlap)
            {
                if(resultArray3.Size > 0)
                    maxY = ptCenter.Y + radius;
            }

            Line yNagetiveAxis = Line.CreateUnbound(ptCenter, -XYZ.BasisY);
            IntersectionResultArray resultArray4;
            SetComparisonResult resIntersect4 = arc.Intersect(yNagetiveAxis, out resultArray4);
            if (resIntersect4 == Autodesk.Revit.DB.SetComparisonResult.Overlap)
            {
                if (resultArray4.Size > 0)
                    minY = ptCenter.Y - radius;
            }

            BoundingBoxUV res = new BoundingBoxUV(minX, minY, maxX, maxY);
            return res;
        }

        static public BoundingBoxUV BoundingBoxesMerge(BoundingBoxUV box1, BoundingBoxUV box2)
        {
            BoundingBoxUV res = new BoundingBoxUV();

            double minX = box1.Min.U;
            double minY = box1.Min.V;
            double maxX = box1.Max.U;
            double maxY = box1.Max.V;
            UV min2 = box2.Min;
            UV max2 = box2.Max;

            if (min2.U < minX)
            {
                minX = min2.U;
            }
            if (min2.V < minY)
            {
                minY = min2.V;
            }
            res.Min = new UV(minX, minY);
            if (max2.U > maxX)
            {
                maxX = max2.U;
            }
            if (max2.V > maxY)
            {
                maxY = max2.V;
            }
            res.Max = new UV(maxX, maxY);

            return res;
        }

        static public XYZ CalculatMidPoint(XYZ ptStart, XYZ ptEnd)
        {
            XYZ direction = ptEnd - ptStart;
            return ptStart + direction / 2.0;
        }

        static public XYZ CalculatMidPoint(Line line)
        {
            XYZ ptStart = line.GetEndPoint(0);
            XYZ ptEnd = line.GetEndPoint(1);
            return CalculatMidPoint(ptStart, ptEnd);
        }

        static public XYZ CalculatMidPoint(XYZ ptStart, XYZ ptEnd, XYZ ptCenter, XYZ normal)
        {
            if (normal.Z < 0)
            {
                XYZ ptSwap = ptStart;
                ptStart = ptEnd;
                ptEnd = ptSwap;
            }
            XYZ dirStart = (ptStart - ptCenter);
            double radius = dirStart.GetLength();
            dirStart = dirStart.Normalize();

            double startAngle = XYZ.BasisX.AngleOnPlaneTo(ptStart - ptCenter, XYZ.BasisZ);
            double endAngle = XYZ.BasisX.AngleOnPlaneTo(ptEnd - ptCenter, XYZ.BasisZ);
            if (startAngle > endAngle)
                startAngle -= Math.PI * 2;

            double halfAngle = (endAngle - startAngle) / 2.0;

            XYZ dirMid = Geometry.RotateTo(dirStart, halfAngle, XYZ.BasisZ).Normalize();

            return ptCenter + dirMid * radius;
        }

        static public XYZ CalculatMidPoint(Arc arc)
        {
            XYZ ptStart = arc.GetEndPoint(0);
            XYZ ptCenter = arc.Center;
            XYZ dirStart = (ptStart - ptCenter).Normalize();
            double radius = arc.Radius;
            double halfAngle = arc.Length / radius / 2.0;

            XYZ dirMid = Geometry.RotateTo(dirStart, halfAngle, XYZ.BasisZ).Normalize();
            if (Geometry.IsEqual(arc.Normal.Z, -1))
            {
                dirMid = Geometry.RotateTo(dirStart, -halfAngle, XYZ.BasisZ).Normalize();
            }
            return ptCenter + dirMid * radius;
        }

        static public Line OffsetLine(Line line, double offset)
        {
            XYZ ptStart = line.GetEndPoint(0);
            XYZ ptEnd = line.GetEndPoint(1);
            XYZ direction = line.Direction;
            XYZ leftVector = Geometry.RotateTo(direction, Math.PI / 2.0, XYZ.BasisZ) * offset;
            XYZ ptS = ptStart + leftVector;
            XYZ ptE = ptEnd + leftVector;

            return Line.CreateBound(ptS, ptE);
        }

        static public Arc OffsetArc(Autodesk.Revit.ApplicationServices.Application app, Arc arc, double offset)
        {
            if (arc.Normal.Z > 0)
                offset = -offset;
            XYZ ptStart = arc.GetEndPoint(0);
            XYZ ptEnd = arc.GetEndPoint(1);
            XYZ ptCenter = arc.Center;
            XYZ ptMid = CalculatMidPoint(arc);
            double radius = arc.Radius;
            double newRadius = radius + offset;

            XYZ dirStart = (ptStart - ptCenter).Normalize() * newRadius;
            XYZ dirEnd = (ptEnd - ptCenter).Normalize() * newRadius;
            XYZ dirMid = (ptMid - ptCenter).Normalize() * newRadius;

            XYZ ptS = ptCenter + dirStart;
            XYZ ptE = ptCenter + dirEnd;
            XYZ ptM = ptCenter + dirMid; 
            return Arc.Create(ptS, ptE, ptM);
        }

        static public double formatAngle(double angle) 
        {
	        int Count;
	        Count = (int)(angle / (Math.PI * 2));
	        double ResultAngle = angle - Count * (Math.PI * 2);
	        if(ResultAngle<0)
		        ResultAngle += (Math.PI * 2);
	        return ResultAngle;	
        }

        static public double getAngle(XYZ ptCenter, XYZ ptOut) 
        {
	        XYZ ptV = ptOut - ptCenter;
            double Angle = XYZ.BasisX.AngleOnPlaneTo(ptV, XYZ.BasisZ);
	        Angle = formatAngle(Angle);
	        return Angle;
        }

        //求转角(从angle1到angle2的角度)
        static public double BetweenTheAngles(double dSAngle, double dEAngle, bool range2PI) 
        {
	        double dAngle;
	        dSAngle = formatAngle(dSAngle);
	        dEAngle = formatAngle(dEAngle);
	        if(Math.Abs(dEAngle - dSAngle) < 0.00001)
	        {
		        dAngle = 0;
		        return dAngle;
	        }
            if (range2PI && dEAngle < dSAngle)
		        dEAngle += Math.PI * 2;
	        dAngle = dEAngle - dSAngle;
	        return dAngle;
        }

        static public double IntersectionAngle(double dSAngle, double dEAngle) 
        {
	        double angle;
	        dSAngle = formatAngle(dSAngle);
	        dEAngle = formatAngle(dEAngle);
            angle = formatAngle(Math.Abs(dSAngle - dEAngle));
	        if(angle > Math.PI)
		        angle = Math.PI * 2 - angle;
	        return angle;
        }

        static public double GetBulge(XYZ SP, XYZ EP,XYZ CenterPt, bool isAnticlockwise) 
        {
            double bulge = 0.0;
	        XYZ ptStart = SP;
            XYZ ptEnd = EP;
	        if (!isAnticlockwise)
	        {
		        ptStart = EP;
		        ptEnd = SP;
	        }
	        double radius = ptStart.DistanceTo(CenterPt);
	        double dS = ptStart.DistanceTo(ptEnd) / 2.0;
	        double dL = radius - Math.Sqrt(radius*radius - dS*dS);
	        double sAngle = getAngle(CenterPt, ptStart);
	        double eAngle = getAngle(CenterPt, ptEnd);
	        double angle = BetweenTheAngles(sAngle, eAngle, true);
            if (IsEqual(angle, Math.PI))
            {
                dL = dS;
            }
	        else if (LessThan(Math.PI, angle))
	        {
		        dL = radius * 2.0 - dL;
	        }
	        bulge = dL / dS;
	        if (!isAnticlockwise)
	        {
		        bulge *= -1;
	        }
            return bulge;
        }

        /// <summary>
        /// 根据起始终止点和凸度计算弧半径
        /// </summary>
        /// <param name="SP">弧起点</param>
        /// <param name="EP">弧终点</param>
        /// <param name="dBulge">弧凸度</param>
        /// <param name="dRadius">返回半径</param>
        static public void GetRadiusWithBulge(XYZ SP, XYZ EP, double dBulge, ref double dRadius)
        {
            if (IsEqual(dBulge, 1.0))
            {
                dRadius = SP.DistanceTo(EP) / 2.0;
                return;
            }
            //凸度 = 玄的垂直平分线与圆的交点(与玄较近的一个)到玄本身的距离L / 玄长的一半S;弧逆时针为正，顺时针为负
            //求S
            double dS = SP.DistanceTo(EP) / 2.0;
            //求L
            double dL = Math.Abs(dBulge) * dS;
            //求半径(三角形直角边的平方=另两边平方和)
            dRadius = (dS * dS + dL * dL) / (dL * 2); 
        }

        /// <summary>
        /// 根据起始终止点和凸度计算弧圆心
        /// </summary>
        /// <param name="SP">弧起点</param>
        /// <param name="EP">弧终点</param>
        /// <param name="dBulge">弧凸度</param>
        /// <param name="ptCenter">返回圆心</param>
        static public void GetCenterWithBulge(XYZ SP, XYZ EP, double dBulge, ref XYZ ptCenter)
        {
            if (IsEqual(dBulge, 1.0))
            {
                ptCenter = CalculatMidPoint(SP, EP);
                return;
            }
            //凸度 = 玄的垂直平分线与圆的交点(与玄较近的一个)到玄本身的距离L / 玄长的一半S;弧逆时针为正，顺时针为负
            //求S
            double dS = SP.DistanceTo(EP) / 2.0;
            //求L
            double dL = Math.Abs(dBulge) * dS;
            //求半径(三角形直角边的平方=另两边平方和)
            double dRadius = (dS * dS + dL * dL) / (dL * 2);

            XYZ ptMid = CalculatMidPoint(SP, EP);
            XYZ dir = (EP - SP).Normalize();
            if (dBulge > 0)//逆时针
            {
                dir = RotateTo(dir, Math.PI/2.0, XYZ.BasisZ);
                if (Math.Abs(dBulge) > 1)
                {
                    dir *= dL - dRadius;
                }
                else
                {
                    dir *= dRadius - dL;
                }
                ptCenter = ptMid + dir;
            }
            else//顺时针
            {
                dir = RotateTo(dir, -(Math.PI / 2.0), XYZ.BasisZ);
                if (Math.Abs(dBulge) > 1)
                {
                    dir *= dL - dRadius;
                }
                else
                {
                    dir *= dRadius - dL;
                }
                ptCenter = ptMid + dir;
            }
        }

        /// <summary>
        /// 根据起始终止点和凸度计算弧基本信息
        /// </summary>
        /// <param name="SP">弧起点</param>
        /// <param name="EP">弧终点</param>
        /// <param name="dBulge">弧凸度</param>
        /// <param name="ptCenter">返回圆心</param>
        /// <param name="dRadius">返回半径</param>
        /// <param name="dSAngle">返回起始角度（弧度0~2PI）</param>
        /// <param name="dEAngle">返回终止角度（弧度0~2PI）</param>
        static public void GetArcInfoWithBulge(XYZ SP, XYZ EP, double dBulge, ref XYZ ptCenter,
                                               ref double dRadius, ref double dSAngle, ref double dEAngle)
        {
            GetCenterWithBulge(SP, EP, dBulge, ref ptCenter);
            dRadius = SP.DistanceTo(ptCenter);
            //求圆心角的一半
            double dAngle = Math.PI - Math.Atan(1 / Math.Abs(dBulge)) * 2;
            if (dBulge > 0)//逆时针
            {
                //求起始角和终了角
                dSAngle = ptCenter.AngleOnPlaneTo(SP, XYZ.BasisZ);
                dEAngle = ptCenter.AngleOnPlaneTo(EP, XYZ.BasisZ);
            }
            else//顺时针
            {
                //求起始角和终了角
                dSAngle = ptCenter.AngleOnPlaneTo(EP, XYZ.BasisZ);
                dEAngle = ptCenter.AngleOnPlaneTo(SP, XYZ.BasisZ);
            }
        }

        /// <summary>
        /// 三点共线
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        static public bool Is_Points_Collinear(XYZ pt1,XYZ pt2,XYZ pt3) 
        {
            double dx1 = pt2.X - pt1.X;
            double dy1 = pt2.Y - pt1.Y;
            double dz1 = pt2.Z - pt1.Z;
            double dx2 = pt3.X - pt1.X;
            double dy2 = pt3.Y - pt1.Y;
            double dz2 = pt3.Z - pt1.Z;
	        double cx = (dy1 * dz2) - (dy2 * dz1);
	        double cy = (dx2 * dz1) - (dx1 * dz2);
	        double cz = (dx1 * dy2) - (dx2 * dy1);

	        return IsEqual(cx * cx + cy * cy + cz * cz,0.0);
        }

        static public bool Is_Point_OnSegment(XYZ ptSOfLine, XYZ ptEOfLine, XYZ pt) 
        {
	        double x1 = ptSOfLine.X;
	        double y1 = ptSOfLine.Y;
	        double z1 = ptSOfLine.Z;
	        double x2 = ptEOfLine.X;
	        double y2 = ptEOfLine.Y;
	        double z2 = ptEOfLine.Z;

	        if (((Lessthan_Or_Equal(x1,pt.X)		&&
                Lessthan_Or_Equal(pt.X, x2)) ||
                (Lessthan_Or_Equal(x2, pt.X) &&
                Lessthan_Or_Equal(pt.X, x1))) &&
		        ((Lessthan_Or_Equal(y1,pt.Y)		&&
                Lessthan_Or_Equal(pt.Y, y2)) ||
                (Lessthan_Or_Equal(y2, pt.Y) &&
                Lessthan_Or_Equal(pt.Y, y1))) &&
		        ((Lessthan_Or_Equal(z1,pt.Z)		&&
                Lessthan_Or_Equal(pt.Z, z2)) ||
                (Lessthan_Or_Equal(z2, pt.Z) &&
                Lessthan_Or_Equal(pt.Z, z1))))
	        {
		        return Is_Points_Collinear(ptSOfLine,ptEOfLine,pt);
	        }
	        return false;
        }

        static public bool Is_Point_OnSegment(XYZ ptSOfArc,XYZ ptEOfArc,XYZ ptCenterOfArc,XYZ normal, XYZ pt) 
        {
            if (normal.Z < 0)
            {
                XYZ ptSwap = ptSOfArc;
                ptSOfArc = ptEOfArc;
                ptEOfArc = ptSwap;
            }
	        double dSRadius = ptSOfArc.DistanceTo(ptCenterOfArc);
	        double dERadius = ptEOfArc.DistanceTo(ptCenterOfArc);
	        if (!IsEqual(dSRadius, dERadius))
	        {
		        return false;
	        }
            double dRadius = pt.DistanceTo(ptCenterOfArc);
            if (!IsEqual(dSRadius, dRadius))
	        {
		        return false;
	        }

            double angleStart = getAngle(ptCenterOfArc, ptSOfArc);
            double angleMid = getAngle(ptCenterOfArc, pt);
            double angleEnd = getAngle(ptCenterOfArc, pt);

	        double angleStoM = BetweenTheAngles(angleStart, angleMid, true);
            double angleMtoE = BetweenTheAngles(angleMid, angleEnd, true);
            double angleStoE = BetweenTheAngles(angleStart, angleEnd, true);

            if (!(IsEqual(angleStoE, (angleStoM + angleMtoE))))
	        {
		        return false;
	        }

	        return true;
        }

        static protected List<Curve> ExtendEndsWithCurve(Curve curve,
            Autodesk.Revit.ApplicationServices.Application revitApp)
        {
            List<Curve> res = new List<Curve>();
            if (!(curve.IsBound))
            {
                res.Add(curve);
                return res;
            }
            Line line = curve as Line;
            if (null != line)
            {
                XYZ ptStart = line.GetEndPoint(0);
                XYZ ptEnd = line.GetEndPoint(1);
                Line newLine = Line.CreateUnbound(ptStart, ptEnd - ptStart);
                res.Add(newLine);
                return res;
            }
            Arc arc = curve as Arc;
            if (null != arc)
            {
                XYZ ptCenter = arc.Center;
                double radius = arc.Radius;
                XYZ ptX = ptCenter + XYZ.BasisX * radius;
                XYZ ptY = ptCenter + XYZ.BasisY * radius;
                XYZ ptXNagtive = ptCenter + -XYZ.BasisX * radius;
                XYZ ptYNagtive = ptCenter + -XYZ.BasisY * radius;
                Arc arc1 = Arc.Create(ptX, ptXNagtive, ptY);
                res.Add(arc1);
                Arc arc2 = Arc.Create(ptXNagtive, ptX, ptYNagtive);
                res.Add(arc2);
                return res;
            }
            return res;
        }

        static public XYZ IntersectWithTwoCurves(Curve curve1, Curve curve2, XYZ curNode, bool extendCurve1, bool extendCurve2,
                                                 Autodesk.Revit.ApplicationServices.Application revitApp)
        {
            List<Curve> extendCurves1 = new List<Curve>();
            if (extendCurve1)
                extendCurves1 = ExtendEndsWithCurve(curve1, revitApp);
            else
                extendCurves1.Add(curve1);
            List<Curve> extendCurves2 = new List<Curve>();
            if (extendCurve2)
                extendCurves2 = ExtendEndsWithCurve(curve2, revitApp);
            else
                extendCurves2.Add(curve2);

            List<XYZ> intersectPoints = new List<XYZ>();
            foreach (Curve subCurve1 in extendCurves1)
            {
                foreach (Curve subCurve2 in extendCurves2)
                {
                    IntersectionResultArray resultArray;
                    SetComparisonResult resIntersect = subCurve1.Intersect(subCurve2, out resultArray);
                    if (resIntersect == Autodesk.Revit.DB.SetComparisonResult.Overlap)
                    {
                        foreach (IntersectionResult interResult in resultArray)
                        {
                            intersectPoints.Add(interResult.XYZPoint);
                        }
                    }
                }
            }
            double minLength = double.MaxValue;
            XYZ res = null;
            foreach (XYZ pt in intersectPoints)
            {
                double length = curNode.DistanceTo(pt);
                if (length < minLength)
                {
                    minLength = length;
                    res = pt;
                }
            }
            return res;
        }

        static private bool IsOverlap(XYZ ptS1, XYZ ptE1, double bugle1,
							   XYZ ptS2, XYZ ptE2, double bugle2)
        {
	        if (IsEqual(bugle1, 0.0) && IsEqual(bugle2, 0.0))
	        {
                if (ptS1.IsAlmostEqualTo(ptE2, _epsPoint) && ptE1.IsAlmostEqualTo(ptS2, _epsPoint))
		        {
			        return true;
		        }
	        }
	        else if (!IsEqual(bugle1, 0.0) && !IsEqual(bugle2, 0.0))
	        {
                if (ptS1.IsAlmostEqualTo(ptE2, _epsPoint) && ptE1.IsAlmostEqualTo(ptS2, _epsPoint) &&
			        IsEqual(bugle1, -bugle2))
		        {
			        return true;
		        }
	        }
	        return false;
        }

        static public void ConvertToSimpleClosedPolylineAndSimpleCurve(List<XYZ> points, List<double> bulges,
													   ref List<List<XYZ> > simplePolysPoint, 
                                                       ref List<List<double> > simplePolysBulge,
                                                       ref List<EdgeInfo> simpleCurves)
        {
	        if (points.Count != bulges.Count)
	        {
		        return;
	        }
            int i = 0;
	        if (points.Count < 3)
	        {		        
		        for (; i < points.Count; ++i)
		        {
			        XYZ ptStartFirst = points[i], ptEndFirst;
			        if (i == points.Count-1)
				        ptEndFirst = points[0];
			        else
				        ptEndFirst = points[i+1];
			        double bulgeFirst = bulges[i];
                    EdgeInfo overlapLine = new EdgeInfo(ptStartFirst, ptEndFirst, bulgeFirst);
			        simpleCurves.Add(overlapLine);
		        }
		        return;
	        }

	        bool findOverlap = false;
	        i = 0;
	        for (; i < points.Count; ++i)
	        {
		        XYZ ptStartFirst = points[i], ptEndFirst;
		        if (i == points.Count-1)
			        ptEndFirst = points[0];
		        else
			        ptEndFirst = points[i+1];
                if (ptStartFirst.IsAlmostEqualTo(ptEndFirst, _epsPoint))
			        continue;
		        double bulgeFirst = bulges[i];
		        findOverlap = false;
		        int j = i+1;
		        for (; j < points.Count; ++j)
		        {
			        XYZ ptStartSecond = points[j], ptEndSecond;
			        if (j == points.Count-1)
				        ptEndSecond = points[0];
			        else
				        ptEndSecond = points[j+1];
                    if (ptStartSecond.IsAlmostEqualTo(ptEndSecond, _epsPoint))
				        continue;
			        double bulgeSecond = bulges[j];
			        if (IsOverlap(ptStartFirst, ptEndFirst, bulgeFirst,ptStartSecond, ptEndSecond, bulgeSecond))
			        {
                        EdgeInfo overlapLine = new EdgeInfo(ptStartFirst, ptEndFirst, bulgeFirst);
				        simpleCurves.Add(overlapLine);

				        List<XYZ> firsetPoints = new List<XYZ>();
                        List<XYZ> SecondPoints = new List<XYZ>();
				        List<double> firstBulges = new List<double>();
                        List<double> SecondBugles = new List<double>();
				        if (i >= 0)
				        {
                            firsetPoints = points.GetRange(0, i + 1);
                            firstBulges = bulges.GetRange(0, i);
					        if (j < points.Count-2)
					        {
                                List<XYZ> tempSecondPoints = points.GetRange(j+2, points.Count - (j+2));
                                firsetPoints.InsertRange(firsetPoints.Count, tempSecondPoints);
                                List<double> tempSecondBulges = bulges.GetRange(j + 1, bulges.Count - (j + 1));
                                firstBulges.InsertRange(firstBulges.Count, tempSecondBulges);
					        }
					        else
					        {
						        if (j == points.Count-1)
						        {
                                    firsetPoints.RemoveAt(firsetPoints.Count - 1);
						        }
						        else
						        {
							        firstBulges.Add(bulges[bulges.Count-1]);
						        }
					        }
				        }
				        if ((j < points.Count-1) && (j > i+1))
				        {
                            SecondPoints = points.GetRange(i + 1, j-(i+1));
                            SecondBugles = bulges.GetRange(i + 1, j-(i+1));
				        }

				        ConvertToSimpleClosedPolylineAndSimpleCurve(firsetPoints, firstBulges,
                            ref simplePolysPoint, ref simplePolysBulge, ref simpleCurves);
				        ConvertToSimpleClosedPolylineAndSimpleCurve(SecondPoints, SecondBugles,
                            ref simplePolysPoint, ref simplePolysBulge, ref simpleCurves);

				        findOverlap = true;
				        break;
			        }
		        }
		        if (findOverlap)
		        {
			        break;
		        }
	        }
	        if (!findOverlap && points.Count > 1)
	        {
		        simplePolysPoint.Add(points);
		        simplePolysBulge.Add(bulges);
	        }
        }

        static public bool GetFittingPolyline(List<XYZ> polylinePoints, List<double> polylineBulge, 
										      ref List<XYZ> fittingPolyline, double fitSpace) 
        {
	        if(polylinePoints.Count != polylineBulge.Count)
	        {
		        return false;
	        }
	        if(polylinePoints.Count<2)
	        {
		        return false;
	        }
	        //拟合点
	        fittingPolyline.Add(polylinePoints[0]);
	        int i = 0;
	        for(; i < polylinePoints.Count;++i)
	        {
		        XYZ ptStart, ptEnd;
		        if (i == (polylinePoints.Count-1))
		        {
			        ptStart = polylinePoints[i];
			        ptEnd = polylinePoints[0];
		        }
		        else
		        {
			        ptStart = polylinePoints[i];
			        ptEnd = polylinePoints[i+1];
		        }
                if (ptStart.IsAlmostEqualTo(ptEnd, _epsPoint))
		        {
			        continue;
		        }
		        if (IsEqual(polylineBulge[i], 0.0))
		        {
			        if ((i+1)<polylinePoints.Count)
			        {
				        fittingPolyline.Add(polylinePoints[i+1]);
			        }
		        }
		        else
		        {
			        XYZ ptCenter = null;
			        GetCenterWithBulge(ptStart, ptEnd, polylineBulge[i], ref ptCenter);
			        double dRadius, dSAngle, dEAngle;
			        dRadius = ptCenter.DistanceTo(ptStart);
			        dSAngle = getAngle(ptCenter, ptStart);
			        dEAngle = getAngle(ptCenter, ptEnd);

			        double bAngle = BetweenTheAngles(dSAngle, dEAngle, true);
			        double lArc = bAngle * dRadius;
			        int size = (int)(lArc / fitSpace);
			        double range = lArc / size;
			        double rangeAngle = range / dRadius;

			        int j = 0;
			        for (; j <= size; ++j)
			        {
				        double dCurAngle = dSAngle + rangeAngle * j;
				        XYZ vector = new XYZ(1,0, 0);
                        vector *= dRadius;
                        vector = RotateTo(vector, dCurAngle, XYZ.BasisZ);
                        XYZ curPoint = ptCenter + vector;

				        fittingPolyline.Add(curPoint);
				        ptStart = curPoint;
			        }
		        }
	        }
	        return true;
        }

        static public int PointInPloygon(List<XYZ> vecPolypt, XYZ pt) 
        {
	        List<XYZ> ptVector = vecPolypt.GetRange(0, vecPolypt.Count);
	        if (ptVector.Count < 3)
	        {
		        return -1;
	        }
	        XYZ pt1 = ptVector[0];
	        XYZ pt2 = ptVector[ptVector.Count-1];
	        if (pt1 != pt2)
	        {
		        ptVector.Add(pt1);
	        }
	        int nQuadrant1,nQuadrant2;
	        int nSum = 0;
	        double df;
	        List<XYZ> vecNewpt = new List<XYZ>();
	        XYZ ptOrigin = new XYZ(0.0,0.0, 0.0);
	        XYZ VecMove = ptOrigin - pt;
	        int i = 0;
	        for (;i<ptVector.Count;++i)
	        {
		        XYZ ptSel=ptVector[i];
		        ptSel=ptSel+VecMove;
		        vecNewpt.Add(ptSel);
	        }
	        XYZ ptOig=pt;
	        ptOig=ptOig+VecMove;
	        nQuadrant1 = Greaterthan_Or_Equal(vecNewpt[0].X, 0.0) ? 
		        (Greaterthan_Or_Equal(vecNewpt[0].Y, 0.0) ? 0:3) : 
	        (Greaterthan_Or_Equal(vecNewpt[0].Y, 0.0) ? 1:2);
           
	        for (i=1;i<vecNewpt.Count;++i)
	        {
		        XYZ ptFront = vecNewpt[i-1]; 
		        XYZ ptCurrent = vecNewpt[i];

                if (ptOig.IsAlmostEqualTo(ptFront, _epsPoint))
                {
                    return 0;	//点在顶点上
                }
		        XYZ vtFront = new XYZ(ptFront.X, ptFront.Y, ptFront.Z);
                XYZ vtCurrent = new XYZ(ptCurrent.X, ptCurrent.Y, ptCurrent.Z);
		        vtFront = vtFront.Normalize();
		        vtCurrent = vtCurrent.Normalize();
		        df = vtCurrent.Y * vtFront.X - vtCurrent.X * vtFront.Y;  // 计算(i-1叉积i)

                if (Math.Abs(df) < _epsAngle &&
                    vtFront.X * vtCurrent.X <= _epsAngle &&
                    vtFront.Y * vtCurrent.Y <= _epsAngle)
                {
                    return 0;  // 点在边上
                }
		        nQuadrant2 = Greaterthan_Or_Equal(ptCurrent.X, 0.0) ? 
			        (Greaterthan_Or_Equal(ptCurrent.Y, 0.0) ? 0:3 ) : 
		        (Greaterthan_Or_Equal(ptCurrent.Y, 0.0) ? 1:2 );
                if (nQuadrant2 == (nQuadrant1 + 1) % 4)
                {
                    nSum++;
                }
                else if (nQuadrant2 == (nQuadrant1 + 2) % 4)
                {
                    if (df > 0)
                        nSum += 2;
                    else
                        nSum -= 2;
                }
                else if (nQuadrant2 == (nQuadrant1 + 3) % 4)
                {
                    nSum--;
                }
		        nQuadrant1=nQuadrant2;
	        }
	        if (nSum == 0/*&&nClose1>0||nClose2>0*/)
		        return -1;
	        else if(Math.Abs(nSum) == 4/*&&nClose1==0&&nClose2==0*/)
		        return 1;
	        return 0;
        }
    }
}
