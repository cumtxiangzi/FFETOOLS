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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatPipe : IExternalCommand
    {
        public static CreatPipeForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new CreatPipeForm();
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
    }
    public class ExecuteEventCreatPipe : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                using (Transaction trans = new Transaction(doc, "�ܵ�����"))
                {
                    trans.Start();
                    if (CreatPipe.mainfrm.clicked == 1)
                    {
                        CreatPipeMethod(doc, uidoc, app, CreatPipe.mainfrm.Button1.Content.ToString());
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 2)
                    {
                        CreatPipeMethod(doc, uidoc, app, CreatPipe.mainfrm.Button2.Content.ToString());
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 3)
                    {
                        CreatPipeMethod(doc, uidoc, app, CreatPipe.mainfrm.Button3.Content.ToString());
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 4)
                    {
                        CreatPipeMethod(doc, uidoc, app, "������ˮ");
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 5)
                    {
                        CreatPipeMethod(doc, uidoc, app, CreatPipe.mainfrm.Button5.Content.ToString());
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 6)
                    {
                        CreatPipeMethod(doc, uidoc, app, "��ˮ");
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 7)
                    {
                        CreatPipeMethod(doc, uidoc, app, CreatPipe.mainfrm.Button7.Content.ToString());
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 8)
                    {
                        CreatPipeMethod(doc, uidoc, app, CreatPipe.mainfrm.Button8.Content.ToString());
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 9)
                    {
                        CreatPipeMethod(doc, uidoc, app, CreatPipe.mainfrm.Button9.Content.ToString());
                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 10)
                    {
                        CreatPipeMethod(doc, uidoc, app, CreatPipe.mainfrm.Button10.Content.ToString());

                        CreatPipe.mainfrm.clicked = 0;
                    }

                    if (CreatPipe.mainfrm.clicked == 20)
                    {
                        FilteredElementCollector elementcollector = new FilteredElementCollector(doc);
                        var pipelist = elementcollector.OfClass((typeof(Pipe))).Cast<Pipe>().ToList();
                        var pipesLessThan3 = pipelist.Where(m => (m.Location as LocationCurve).Curve.Length == 3 / 304.8);

                        if (!(pipesLessThan3 == null))
                        {
                            foreach (Pipe item in pipesLessThan3)
                            {
                                doc.Delete(item.Id);
                            }
                            CreatPipe.mainfrm.clicked = 0;
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("���Ȼ��ƹܵ���", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    trans.Commit();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "�ܵ�����";
        }
        public void CreatPipeMethod(Document doc, UIDocument uidoc, UIApplication app, string pipeSystem)
        {
            PipeType pt = GetPipeType(doc, uidoc, "���Ӹֹ�");

            if (pipeSystem.Contains("ѭ����ˮ") || pipeSystem.Contains("ѭ����ˮ") || pipeSystem.Contains("ˮԴ��ˮ"))
            {
                pt = GetPipeType(doc, uidoc, "���Ӹֹ�");
            }

            if (pipeSystem.Contains("�����ˮ") || pipeSystem.Contains("��ˮ"))
            {
                pt = GetPipeType(doc, uidoc, "PPR");
            }

            if (pipeSystem.Contains("������ˮ"))
            {
                pt = GetPipeType(doc, uidoc, "UPVC");
            }

            if (pipeSystem.Contains("����"))
            {
                pt = GetPipeType(doc, uidoc, "��п�ֹ�");
            }

            if (pipeSystem.Contains("������") || pipeSystem.Contains("������") || pipeSystem.Contains("ˮ���ȶ�"))
            {
                pt = GetPvcType(doc, uidoc);
            }

            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = col.ToElements();
            PipingSystemType pipesys = null;
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;

                if (pipeSystem.Contains("������ˮ"))
                {
                    if (ps.Name.Contains("����ˮ") && ps.Name.Contains("��ˮ"))
                    {
                        pipesys = ps;
                        break;
                    }
                }

                if (ps.Name.Contains("����ˮ") && ps.Name.Contains(pipeSystem))
                {
                    pipesys = ps;
                    break;
                }
            }

            Pipe p = Pipe.Create(doc, pipesys.Id, pt.Id, doc.ActiveView.GenLevel.Id, new XYZ(0, 0, 0), new XYZ(3 / 304.8, 0, 0));
            IList<ElementId> list = new List<ElementId>();
            list.Add(p.Id);
            uidoc.Selection.SetElementIds(list);

            RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.CreateSimilar);
            app.PostCommand(cmdId);
        }
        public PipeType GetPipeType(Document doc, UIDocument uidoc, string pipeType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;

                if (pipeType == "PVC")
                {
                    if (ps.Name.Contains("����ˮ") && ps.Name == "PVC")
                    {
                        pt = ps;
                        break;
                    }
                }

                if (ps.Name.Contains("����ˮ") && ps.Name.Contains(pipeType))
                {
                    pt = ps;
                    break;
                }
            }
            return pt;
        }
        public PipeType GetPvcType(Document doc, UIDocument uidoc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains("����ˮ") && ps.Name.Contains("PVC") && !(ps.Name.Contains("U")))
                {
                    pt = ps;
                    break;
                }
            }
            return pt;
        }
    }
    public static class Helper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        /// <summary>
        /// ���ͼ�����Ϣ
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="key"></param>
        public static void SendKeys(IntPtr proc, Keys key)
        {
            SetActiveWindow(proc);
            SetForegroundWindow(proc);
            keybd_event(key, 0, 0, 0);
            keybd_event(key, 0, 2, 0);
            keybd_event(key, 0, 0, 0);
            keybd_event(key, 0, 2, 0);
        }
    }
}


