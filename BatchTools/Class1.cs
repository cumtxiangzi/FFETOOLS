using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.ApplicationServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class GetSelect : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                if (null == commandData)
                {
                    throw new ArgumentNullException(" commandData ");
                }

                UIApplication uiApp = commandData.Application;
                Document doc = uiApp.ActiveUIDocument.Document;
                Selection sel = uiApp.ActiveUIDocument.Selection;

                Reference refelem = null; // 类似C#中的Object基类
                IList<Duct> ducts = new List<Duct>();
                IList<XYZ> xyzs = new List<XYZ>();

               




                for (int i = 1; i < 5; i++)
                {
                    // 没有提示文字
                    refelem = sel.PickObject(ObjectType.Element, " 请选择第  " + i.ToString() + "  个对象 ");
                    if (doc.GetElement(refelem) is Duct)
                    {
                        ducts.Add(doc.GetElement(refelem) as Duct);
                        xyzs.Add(refelem.GlobalPoint);
                        MessageBox.Show(" 选择了一个风管 ");
                    }
                    else
                    {
                        MessageBox.Show(" 请选择风管 ");
                    }
                }
                MessageBox.Show(" 你选择了 " + ducts.Count + " 个风管 ");

            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public void AutoCreatDimension(Document doc, Selection selection)
        {

            //选择需要标注尺寸的图元
            IList<Reference> referenceList = selection.PickObjects(ObjectType.Element, "请选择一组图元");
            if (referenceList.Count == 0)
            {
                TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                return;
            }
            //取得其中一个图元 获取其位置
            // Pipe pipe = doc.GetElement(referenceList.ElementAt(0)) as Pipe;
            Element element = doc.GetElement(referenceList.ElementAt(0));
            Line line = (element.Location as LocationCurve).Curve as Line;

            View view = doc.ActiveView;
            XYZ selectionPoint = selection.PickPoint();
            XYZ projectPoint = line.Project(selectionPoint).XYZPoint;
            Line newLine = Line.CreateBound(selectionPoint, projectPoint);

            ReferenceArray references = new ReferenceArray();
            foreach (Reference reference in referenceList)
            {
                references.Append(reference);
            }
            //调用创建尺寸的方法创建
            Dimension autoDimension = doc.Create.NewDimension(view, newLine, references);
        }
    }
}
