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
using System.Runtime.InteropServices;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace FFETOOLS
{
    public static class WindowsHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, CallBack lpEnumFunc, IntPtr lParam);
        public delegate bool CallBack(IntPtr hwnd, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpText, int nCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);

    }

    [Transaction(TransactionMode.Manual)]
    public class BatchBreakPipes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                using (Transaction trans = new Transaction(doc, "管道批量打断"))
                {
                    trans.Start();

                    BatchBreakPipeMain(doc, uidoc);

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                messages = e.Message;
            }
            return Result.Succeeded;
        }

        public void BatchBreakPipeMain(Document doc, UIDocument uidoc)
        {
            Selection sel = uidoc.Selection;

            var eleref = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is MEPCurve), "拾取管线打断点");
            var pickpoint = eleref.GlobalPoint;

            Subscribe();
            IList<Reference> refList = sel.PickObjects(ObjectType.Element, new PipeSelectionFilter(), "请选要要批量打断的管道");         
            CompleteMultiSelection();         
            Unsubscribe();

            if (refList.Count == 0)
            {
                TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                BatchBreakPipeMain(doc, uidoc);
            }
            else
            {
                BatchBreakPipeMethod(doc, refList, pickpoint);
            }
        }
        public void BatchBreakPipeMethod(Document doc, IList<Reference> refList, XYZ point)
        {
            List<Pipe> pipeList = new List<Pipe>();
            foreach (var item in refList)
            {
                Pipe p = item.GetElement(doc) as Pipe;
                pipeList.Add(p);
            }
            foreach (Pipe item in pipeList)
            {
                Line line = ((item as MEPCurve).Location as LocationCurve).Curve as Line;
                line.MakeUnbound();
                IntersectionResult result = line.Project(point);
                XYZ crossPoint = result.XYZPoint;
                BreakPipeMethod(doc, item, crossPoint);
            }
        }
        public void BreakPipeMethod(Document doc, Pipe pipe, XYZ point)
        {
            var mep = pipe as MEPCurve;
            var locationline = (mep.Location as LocationCurve).Curve as Line;
            PlumbingUtils.BreakCurve(doc, mep.Id, point);
        }
        
        /// <summary>
        /// 完成按钮右键触发
        /// </summary>
        private IKeyboardMouseEvents m_GlobalHook;
        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyUp += GlobalHookKeyUpExt;
            m_GlobalHook.MouseUpExt += GlobalHookMouseUpExt;
        }
        private void GlobalHookMouseUpExt(object sender, MouseEventExtArgs e)
        {
            //if (e.Button == MouseButtons.Right) { CompleteMultiSelection(); }
        }
        private void GlobalHookKeyUpExt(object sender, KeyEventArgs e)
        {
            // 32 represent Space
            if (e.KeyValue == 32) { CompleteMultiSelection(); }
        }
        private void CompleteMultiSelection()
        {
            var rvtwindow = Autodesk.Windows.ComponentManager.ApplicationWindow;
            var list = new List<IntPtr>();
            var flag = WindowsHelper.EnumChildWindows(rvtwindow,
                       (hwnd, l) =>
                       {
                           StringBuilder windowText = new StringBuilder(200);
                           WindowsHelper.GetWindowText(hwnd, windowText, windowText.Capacity);
                           StringBuilder className = new StringBuilder(200);
                           WindowsHelper.GetClassName(hwnd, className, className.Capacity);
                           if ((windowText.ToString().Equals("完成", StringComparison.Ordinal) ||
                          windowText.ToString().Equals("Finish", StringComparison.Ordinal)) &&
                          className.ToString().Contains("Button"))
                           {
                               list.Add(hwnd);
                               return false;
                           }
                           return true;
                       }, new IntPtr(0));

            var complete = list.FirstOrDefault();
            WindowsHelper.SendMessage(complete, 245, 0, 0);
        }
        public void Unsubscribe()
        {
            m_GlobalHook.MouseUpExt -= GlobalHookMouseUpExt;
            m_GlobalHook.KeyUp -= GlobalHookKeyUpExt;
            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }
    }
   
}
