using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.InteropServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatPipeXJ : IExternalCommand
    {
        //定义一个字段通过属性变化来触发空闲事件
        public static Boolean IdleFlag = false;
        //记录DocumentChanged事件发生改变的构件
        IList<ElementId> listId = new List<ElementId>();
        //定义一个全局UIApplication，用来注销指定事件
        UIApplication uiApp = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains("给排水") && ps.Name.Contains("焊接钢管"))
                {
                    pt = ps;
                    break;
                }
            }

            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = col.ToElements();
            PipingSystemType pipesys = null;
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;
                if (ps.Name.Contains("给排水") && ps.Name.Contains("循环给水"))
                {
                    pipesys = ps;
                    break;
                }
            }

            using (Transaction ts=new Transaction(doc,"创建管道"))
            {
                ts.Start();
                Pipe p = Pipe.Create(doc, pipesys.Id, pt.Id, doc.ActiveView.GenLevel.Id, new XYZ(0, 0, 0), new XYZ(0.1, 0, 0));

                IList<ElementId> list = new List<ElementId>();
                list.Add(p.Id);
                uidoc.Selection.SetElementIds(list);

                RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.CreateSimilar);
                uiApp.PostCommand(cmdId);

                //uiApp.Idling += new EventHandler<IdlingEventArgs>(IdlingHandler);//先注册空闲事件
                //uiApp.Application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(DocumentChangedForSomething);//再注册DocumentChanged事件
                ts.Commit();
                return Result.Succeeded;
            }
            

        }

        private void DocumentChangedForSomething(object sender, DocumentChangedEventArgs e)
        {
            listId.Clear();
            ICollection<ElementId> collection = e.GetAddedElementIds();//获取创建元素的ids
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
            ExecuteIdlingHandler.Execute(uiapp, listId);//删除创建元素
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
