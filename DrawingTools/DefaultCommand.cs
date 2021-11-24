using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Create3DView
{
    /// <summary>
    /// 创建任意视角的3d视图
    /// </summary>
    /// <creator>marc</creator>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class DefaultCommand : IExternalCommand
    {
        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var activeDocument = commandData.Application.ActiveUIDocument;
            var document = activeDocument.Document;

            View3D view3D;
            using (Transaction tran = new Transaction(document, Guid.NewGuid().ToString()))
            {
                tran.Start();

                //创建3d视角
                XYZ eye = XYZ.Zero;
                XYZ up = VectorFromAngles(45, 45 + 90);
                XYZ forward = VectorFromAngles(45, 45);

                IList<ViewFamilyType> types = null;
                using (var collector = new FilteredElementCollector(document).OfClass(typeof(ViewFamilyType)))
                {
                    types = collector.Cast<ViewFamilyType>().ToList();
                }
                var viewFamilyType = types.FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

                view3D = View3D.CreateIsometric(document, viewFamilyType.Id);
                view3D.SetOrientation(new ViewOrientation3D(eye, up, forward));

                tran.Commit();
            }

            //激活刚创建的3d视图。请注意，激活视图必须在事务外进行。
            activeDocument.ActiveView = view3D;

            return Result.Succeeded;
        }

        /// <summary>
        /// 创建指定方向的单位向量
        /// </summary>
        /// <param name="angleHorizD">XY平面的角度</param>
        /// <param name="angleVertD">垂直角度，值在-90到90度之间</param>
        /// <returns>返回某方向的单位向量</returns>
        private XYZ VectorFromAngles(double angleHorizD, double angleVertD)
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
