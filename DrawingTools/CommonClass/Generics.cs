using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace FFETOOLS
{
    #region 过滤重复数据
    public delegate bool EqualsComparer<T>(T x, T y);
    /// <summary>
    /// 过滤重复数据
    /// </summary>
    public class Comparint<T> : IEqualityComparer<T>
    {
        private EqualsComparer<T> ec;
        public Comparint() { }
        public Comparint(EqualsComparer<T> e)
        {
            this.ec = e;
        }
        #region IEqualityComparer<BudgetBE> 成员

        public bool Equals(T x, T y)
        {
            if (null != this.ec)
                return this.ec(x, y);
            else
                return false;
        }

        public int GetHashCode(T obj)
        {
            return obj.ToString().GetHashCode();
        }

        #endregion
    }
    #endregion

    #region LevelComparer
    public class LevelComparer : IComparer<Level>
    {
        public LevelComparer() { }
        #region IComparer<Student> 成员
        public int Compare(Level x, Level y)
        {
            return x.Elevation.CompareTo(y.Elevation);
        }

        #endregion
    }
    #endregion

    #region DistanceComparer
    public class DistanceComparer : IComparer<XYZ>
    {
        XYZ m_ReferencePoint = null;
        public DistanceComparer(XYZ refPoint)
        {
            m_ReferencePoint = refPoint;
        }
        #region IComparer<Student> 成员
        public int Compare(XYZ pt1, XYZ pt2)
        {
            return pt1.DistanceTo(m_ReferencePoint).CompareTo(pt2.DistanceTo(m_ReferencePoint));
        }

        #endregion
    }
    #endregion

    #region AngleComparer
    public class AngleComparer : IComparer<XYZ>
    {
        XYZ m_ReferencePoint = null;
        XYZ m_Center;
        public AngleComparer(XYZ refPoint, XYZ center)
        {
            m_ReferencePoint = refPoint;
            m_Center = center;
        }
        #region IComparer<Student> 成员
        public int Compare(XYZ pt1, XYZ pt2)
        {
            XYZ vecterStart = m_ReferencePoint - m_Center;
            XYZ vecter1 = pt1 - m_Center;
            XYZ vecter2 = pt2 - m_Center;
            double angle1 = vecterStart.AngleOnPlaneTo(vecter1, XYZ.BasisZ);
            if (Math.Abs(angle1 - Math.PI * 2) < 0.000001)
                angle1 = 0.0;
            double angle2 = vecterStart.AngleOnPlaneTo(vecter2, XYZ.BasisZ);
            if (Math.Abs(angle2 - Math.PI * 2) < 0.000001)
                angle2 = 0.0;
            return angle1.CompareTo(angle2);
        }

        #endregion
    }
    #endregion

    #region AngleComparer
    class XYZComparer : IEqualityComparer<XYZ>
    {
        public bool Equals(XYZ p, XYZ q)
        {
            return p.IsAlmostEqualTo(q, Geometry._epsPoint);
        }

        public int GetHashCode(XYZ p)
        {
            string strX = p.X.ToString("#0.0000");
            string strY = p.Y.ToString("#0.0000");
            string strZ = p.Z.ToString("#0.0000");
            string strPoint = "(" + strX + "," + strY + "," + strZ + ")";
            return strPoint.GetHashCode();
        }
    }
    #endregion
}

