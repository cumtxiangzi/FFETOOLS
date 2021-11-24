using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace New3dView
{
    /// <summary>
    /// Document扩展
    /// </summary>
    /// <creator>marc</creator>
    public static class DocumentExtensions
    {
        /// <summary>
        /// 筛选视图
        /// </summary>
        /// <param name="currentDocument">当前文档</param>
        /// <returns></returns>
        public static FilteredElementCollector ClassOfView(this Document currentDocument)
        {
            return new FilteredElementCollector(currentDocument).OfClass(typeof(View));
        }

        /// <summary>
        /// 筛选视图类型
        /// </summary>
        /// <param name="currentDocument">当前文档</param>
        /// <returns></returns>
        public static FilteredElementCollector ClassOfViewFamilyType(this Document currentDocument)
        {
            return new FilteredElementCollector(currentDocument).OfClass(typeof(ViewFamilyType));
        }

        /// <summary>
        /// 取视图
        /// </summary>
        /// <param name="currentDocument"></param>
        /// <returns></returns>
        public static IList<View> GetViewList(this Document currentDocument)
        {
            using (var source = currentDocument.ClassOfView())
            {
                return source.Cast<View>().ToList();
            }
        }

        /// <summary>
        /// 筛选视图类型
        /// </summary>
        /// <param name="currentDocument">当前文档</param>
        /// <returns></returns>
        public static IList<ViewFamilyType> GetViewFamilyTypeList(this Document currentDocument)
        {
            using (var source = currentDocument.ClassOfViewFamilyType())
            {
                return source.Cast<ViewFamilyType>().ToList();
            }
        }

        /// <summary>
        /// 生成默认3d视图，3d视角与Revit原生是一样的
        /// </summary>
        public static Document Create3DView(this Document currentDocument)
        {
            //正前方
            //XYZ eye = new XYZ(0, 0, 0);
            //XYZ up = new XYZ(0, 0, 1);
            //XYZ forward = new XYZ(0, 1, 0);

            //左，前
            //XYZ eye = new XYZ(0, 0, 0);
            //XYZ up = new XYZ(0, 0, 1);
            //XYZ forward = new XYZ(1, 1, 0);

            //上，前
            //XYZ eye = new XYZ(0, 0, 0);
            //XYZ up = new XYZ(0, 1, 1);
            //XYZ forward = new XYZ(0, 1, -1);

            //左
            //XYZ eye = new XYZ(0, 0, 0);
            //XYZ up = new XYZ(0, 0, 1);
            //XYZ forward = new XYZ(1, 0, 0);

            //前
            //XYZ eye = new XYZ(0, 0, 0);
            //XYZ up = new XYZ(0, 0, 1);
            //XYZ forward = new XYZ(0, 1, 0);

            //45,-35度。这个与Revit原生的视角几乎是一样的
            return Create3DView(currentDocument, 45, -35);
        }

        /// <summary>
        /// 生成指定文档的3d视图
        /// </summary>
        public static Document Create3DView(this Document currentDocument, XYZ eye, XYZ up, XYZ forward)
        {
            var v3 = currentDocument.GetViewList().FirstOrDefault(q => q.ViewType == ViewType.ThreeD);
            if (v3 != null)
            {
                return currentDocument;
            }

            var viewFamilyType = currentDocument.GetViewFamilyTypeList().FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

            var view3D = View3D.CreateIsometric(currentDocument, viewFamilyType.Id);
            view3D.SetOrientation(new ViewOrientation3D(eye, up, forward));

            return currentDocument;
        }

        /// <summary>
        /// 生成指定文档的3d视图
        /// </summary>
        /// <param name="currentDocument">当前文档</param>
        /// <param name="angleHorizD">水平视角的角度。侧面则是45度</param>
        /// <param name="angleVertD">垂直角度，俯视传入负值，仰视传入正值。值的范围在-90度到+90之间</param>
        public static Document Create3DView(this Document currentDocument, double angleHorizD, double angleVertD)
        {
            XYZ eye = XYZ.Zero;
            XYZ up = VectorFromAngles(angleHorizD, angleVertD + 90);
            XYZ forward = VectorFromAngles(angleHorizD, angleVertD);

            return Create3DView(currentDocument, eye, up, forward);
        }

        /// <summary>
        /// 创建指定方向的单位向量
        /// </summary>
        /// <param name="angleHorizD">XY平面的角度</param>
        /// <param name="angleVertD">垂直角度，值在-90到90度之间</param>
        /// <returns>返回某方向的单位向量</returns>
        private static XYZ VectorFromAngles(double angleHorizD, double angleVertD)
        {
            // 转弧度
            double degToRadian = Math.PI * 2 / 360;
            double angleHorizR = angleHorizD * degToRadian;
            double angleVertR = angleVertD * degToRadian;

            // 生成3d视图中的单位向量
            double a = Math.Cos(angleVertR);
            double b = Math.Cos(angleHorizR);
            double c = Math.Sin(angleHorizR);
            double d = Math.Sin(angleVertR);

            return new XYZ(a * b, a * c, d);
        }
    }
}
