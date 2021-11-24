using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class Class测试: IExternalCommand
    {
        UIApplication uiApp = null; //定义一个全局UIApplication，用来注销指定事件
        IList<ElementId> listId = new List<ElementId>();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiApp = commandData.Application;
            //应用级别的修改
            commandData.Application.Application.DocumentChanged += appChange;

            return Result.Succeeded;
        }

        private void appChange(object sender, DocumentChangedEventArgs e)
        {
            ElementCategoryFilter genericmodelFilter = new ElementCategoryFilter(BuiltInCategory.OST_GenericModel);
            listId.Clear();
            ICollection<ElementId> collection = e.GetAddedElementIds(genericmodelFilter);
            ICollection<ElementId> collection2 = e.GetModifiedElementIds(genericmodelFilter);

            foreach (ElementId elid in collection)
            {
                listId.Add(elid);


            }
            foreach (ElementId elid in collection2)
            {
                listId.Add(elid);

            }

            if (listId.Count() > 0)
            {
                uiApp.Idling += IdlingHandler;//注册空闲事件
            }
        }

        private void IdlingHandler(object sender, IdlingEventArgs e)
        {
            UIDocument uidoc = uiApp.ActiveUIDocument;//获取活动文档
            Document doc = uidoc.Document;
            Transaction trans = new Transaction(doc, "命名");
            trans.Start();
            TaskDialog.Show("1", "1");//这里可以放自己想要执行的代码
            //在空闲事件中建议只放一个事务来完成所有动作，因为我测试过多个事务只会执行一个事务，后面的会被取消
            trans.Commit();
            uiApp.Idling -= IdlingHandler;//取消空闲事件
        }
    }
}
