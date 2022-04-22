using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit;
using Autodesk.Revit.DB;

namespace FFETOOLS
{
    public class EdgeInfo
    {
        private XYZ m_StartPoint;
        private XYZ m_EndPoint;
        private double m_Bulge;

        #region property
        public XYZ StartPoint
        {
            get
            {
                return m_StartPoint;
            }
            set 
            {
                m_StartPoint = value;
            }
        }

        public XYZ EndPoint
        {
            get
            {
                return m_EndPoint;
            }
            set
            {
                m_EndPoint = value;
            }
        }

        public double Bulge
        {
            get
            {
                return m_Bulge;
            }
            set
            {
                m_Bulge = value;
            }
        }

        public bool IsArc
        {
            get
            {
                return !Geometry.IsEqual(m_Bulge, 0.0);
            }
        }
        #endregion

        public EdgeInfo(EdgeInfo rhs)
        {
            m_StartPoint = rhs.m_StartPoint;
            m_EndPoint = rhs.m_EndPoint;
            m_Bulge = rhs.m_Bulge;
        }

        public EdgeInfo(XYZ startPoint, XYZ endPoint, double bulge)
        {
            m_StartPoint = startPoint;
            m_EndPoint = endPoint;
            m_Bulge = bulge;
        }
    }
}
