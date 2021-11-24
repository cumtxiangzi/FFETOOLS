using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using System.Xml;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Events;


namespace HelloWorld
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Test : IExternalCommand
    {  //定义一个字段通过属性变化来触发空闲事件
        public static Boolean IdleFlag = false;
        //记录DocumentChanged事件发生改变的构件
        IList<ElementId> listId = new List<ElementId>();
        //定义一个全局UIApplication，用来注销指定事件
        UIApplication uiApp = null;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document Doc = uiDoc.Document;
            uiApp.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.Door));//调用“门”命令，用户创建门
            uiApp.Idling += new EventHandler<IdlingEventArgs>(IdlingHandler);//先注册空闲事件
            uiApp.Application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(DocumentChangedForSomething);//再注册DocumentChanged事件

            return Result.Succeeded;
        }


        private void DocumentChangedForSomething(object sender, DocumentChangedEventArgs e)
        {
            listId.Clear();
            ICollection<ElementId> collection = e.GetAddedElementIds();//获取创建的门的ids
            listId.Add(collection.ElementAt(0));
            IdleFlag = true;
            uiApp.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(DocumentChangedForSomething);//注销本事件
        }
        //Revit空闲事件
        public void IdlingHandler(object sender, IdlingEventArgs args)
        {
            UIApplication uiapp = sender as UIApplication;
            if (!IdleFlag)//true
            {
                return;//继续执行
            }
            ExecuteIdlingHandler.Execute(uiapp, listId);//删除创建的门
            IdleFlag = false;
            uiApp.Idling -= new EventHandler<IdlingEventArgs>(IdlingHandler);//注销
        }
    }
    public static class ExecuteIdlingHandler
    {
        public static void Execute(UIApplication uiapp, IList<ElementId> listId)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            if (uidoc != null)
            {
                Transaction ts = new Transaction(uidoc.Document, "delete");
                ts.Start();
                uidoc.Document.Delete(listId);
                ts.Commit();
            }
        }
    }
}