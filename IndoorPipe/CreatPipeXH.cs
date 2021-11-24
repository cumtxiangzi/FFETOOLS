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
    class CreatPipeXH : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            CompoundOperation(doc,uiApp,uidoc);
            RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.CreateSimilar);
            //加载命令
            uiApp.PostCommand(cmdId);

            return Result.Succeeded;

        }

        public void CompoundOperation(Document doc, UIApplication uiApp, UIDocument uidoc)
        {
            // 所有TransactionGroup要用“using”来创建来保证它的正确结束 
            using (TransactionGroup transGroup = new TransactionGroup(doc, "创建循环回水"))
            {
                if (transGroup.Start() == TransactionStatus.Started)
                {
                    // 我们打算调用两个函数，每个都有一个独立的事务 
                    // 我们打算这个组合操作要么成功，要么失败 
                    // 只要其中有一个失败，我们就撤销所有操作 

                    if (CreatePipe(doc, uiApp, uidoc) && DeletPipe(doc, uiApp))
                    {
                        // Assimilate函数会将这两个事务合并成一个，并只显示TransactionGroup的名
                        // 在Undo菜单里 
                        transGroup.Assimilate();
                    }
                    else
                    {
                        // 如果有一个操作失败了，我们撤销在这个事务组里的所有操作 
                        transGroup.RollBack();
                    }
                }
            }
        }

        private bool DeletPipe(Document doc, UIApplication uiApp)
        {
            using (Transaction trans = new Transaction(doc, "删除短管"))
            {
                if (TransactionStatus.Started == trans.Start())
                {

                    //查找PostableCommand的API，在枚举中找到对应的方法为创建类似
                   


                    FilteredElementCollector elementcollector = new FilteredElementCollector(doc);
                    var pipelist = elementcollector.OfClass((typeof(Pipe))).Cast<Pipe>().ToList();
                    var pipeslessthan5 = pipelist.Where(m => (m.Location as LocationCurve).Curve.Length < 5 / 304.8);

                    foreach (Pipe item in pipeslessthan5)
                    {
                        //doc.Delete(item.Id);
                       //MessageBox.Show("ss");
                    }

                    if (TransactionStatus.Committed == trans.Commit())
                    {
                        return true;
                    }
                    // 如果失败，撤销这个事务 
                    trans.RollBack();
                }
            }
            return false;

        }

        private bool CreatePipe(Document doc, UIApplication uiApp, UIDocument uidoc)
        {
            using (Transaction trans = new Transaction(doc, "创建管道"))
            {
                if (TransactionStatus.Started == trans.Start())
                {

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
                        if (ps.Name.Contains("给排水") && ps.Name.Contains("循环回水"))
                        {
                            pipesys = ps;
                            break;
                        }
                    }

                    Pipe p = Pipe.Create(doc, pipesys.Id, pt.Id, doc.ActiveView.GenLevel.Id, new XYZ(0, 0, 0), new XYZ(3 / 304.8, 0, 0));

                   
                    if (TransactionStatus.Committed == trans.Commit())
                    {
                        return true;
                    }
                    // 如果失败，撤销这个事务 
                    trans.RollBack();
                }
            }
            return false;
        }
    }
}
