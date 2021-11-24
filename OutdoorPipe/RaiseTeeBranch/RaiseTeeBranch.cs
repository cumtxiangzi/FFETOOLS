using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows.Interop;
using System.Diagnostics;

namespace FFETOOLS
{
    /// <summary>
    /// 管道三通支管提升
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    class RaiseTeeBranch : IExternalCommand
    {
        public static RaiseTeeBranchForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new RaiseTeeBranchForm();
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
    public class ExecuteEventRaiseTeeBranch : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
                View acview = doc.ActiveView;

                using (Transaction trans = new Transaction(doc, "管道三通支管提升"))
                {
                    trans.Start();
                    RaiseTeeBranchMain(doc,uidoc);
                    trans.Commit();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "管道三通支管提升";
        }

        public void RaiseTeeBranchMain(Document doc, UIDocument uidoc)
        {
            if (RaiseTeeBranch.mainfrm.SingleSelect.IsChecked == true)
            {
                Selection sel = uidoc.Selection;
                var eleref = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Pipe));
                var pipe = eleref.GetElement(doc) as Pipe;
                RaiseTeeBranchMethod(doc, pipe);
            }
            else
            {
                Selection sel = uidoc.Selection;
                IList<Reference> refList = sel.PickObjects(ObjectType.Element, new PipeSelectionFilter(), "请选支管");
                List<Pipe> pipeList = new List<Pipe>();

                if (refList.Count == 0)
                {
                    TaskDialog.Show("警告", "您没有选择任何元素，请重新选择");
                    RaiseTeeBranchMain(doc, uidoc);
                }
                else
                {
                    foreach (var item in refList)
                    {
                        Pipe p = item.GetElement(doc) as Pipe;
                        pipeList.Add(p);
                    }
                    foreach (Pipe item in pipeList)
                    {
                        RaiseTeeBranchMethod(doc, item);
                    }
                }
            }
        }
        public void RaiseTeeBranchMethod(Document doc, Pipe pipe)
        {
            int value = RaiseTeeBranch.mainfrm.Height;

            var pipecons = pipe.ConnectorManager.Connectors.Cast<Connector>();

            var validcons = pipecons.Where(m =>
                    m.IsConnected && (m.ConnectorType == ConnectorType.End || m.ConnectorType == ConnectorType.Curve))
                .ToList();

            //if (validcons.Count < 1) return Result.Cancelled;
            var connectedPipeFittings = validcons.Select(m => m.GetConnectedCon().Owner).Cast<FamilyInstance>().Where(m => m != null);

            var teeFitting = default(IEnumerable<FamilyInstance>);
            teeFitting = connectedPipeFittings.Where(m => m.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE).AsValueString().Contains("三通") && m.FacingOrientation.IsParallel(pipe.LocationLine().Direction));

            if (teeFitting.Count() == 1)
            {
                var tee = teeFitting.FirstOrDefault();
                var location = (tee.Location as LocationPoint).Point;
                var facingdir = tee.FacingOrientation;
                var handdir = tee.HandOrientation;
                var anxisline = Line.CreateUnbound(location, handdir);

                var updir = -facingdir.CrossProduct(handdir);
                var newupdir = default(XYZ);
                if (updir.AngleTo(XYZ.BasisZ) <= Math.PI / 2)
                {
                    newupdir = updir;
                }
                else
                {
                    newupdir = -updir;
                }

                var consOfTee = tee.MEPModel.ConnectorManager.Connectors.Cast<Connector>();
                var branchCon = consOfTee.Where(m => m.CoordinateSystem.BasisZ.IsSameDirection(-facingdir)).FirstOrDefault();

                var connectedconOfBranchCon = branchCon.GetConnectedCon();

                branchCon.DisconnectFrom(connectedconOfBranchCon);
                //改变支管高度
                ElementTransformUtils.MoveElement(doc, pipe.Id, newupdir * value / 304.8);

                //旋转Tee
                ElementTransformUtils.RotateElement(doc, tee.Id, anxisline, facingdir.AngleOnPlaneTo(newupdir * (value) / Math.Abs(value), -handdir));
                doc.Regenerate();

                var branchConPosition = branchCon.Origin;
                var distance = branchConPosition.DistanceTo(pipe.LocationLine());

                //新创建管道
                var startpo = branchConPosition;
                var endpo = startpo + newupdir * (value) / Math.Abs(value) * distance;

                var newline = Line.CreateBound(startpo, endpo);

                var newpipeid = ElementTransformUtils.CopyElement(doc, pipe.Id, new XYZ()).FirstOrDefault();

                var newpipe = newpipeid.GetElement(doc) as Pipe;

                (newpipe.Location as LocationCurve).Curve = newline;

                foreach (Connector con in newpipe.ConnectorManager.Connectors)
                {
                    var conorigin = con?.Origin;
                    if (conorigin == null) continue;
                    if (conorigin.IsAlmostEqualTo(branchConPosition))
                    {
                        con.ConnectTo(branchCon);
                    }
                }
                pipe.ElbowConnect(newpipe);
            }
            else if (teeFitting.Count() == 2)
            {
                //两端都是三通的情况 暂未处理
            }
        }
    }
}
