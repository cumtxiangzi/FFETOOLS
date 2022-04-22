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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class NotePipes : IExternalCommand
    {
        public static NotePipesForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new NotePipesForm();
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
    public class ExecuteEventNotePipes : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                View view = uidoc.ActiveView;
                if (view is View3D)
                {
                    View3D aView = view as View3D;
                    if (aView.IsLocked == true)
                    {
                        CreatPipeNotes(doc, uidoc);
                    }
                    else
                    {
                        TaskDialog.Show("警告", "请将三维视图锁定后再进行操作");
                    }
                }
                else
                {
                    CreatPipeNotes(doc, uidoc);
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "批量标注管径";
        }      
        public void CreatPipeNotes(Document doc, UIDocument uidoc)
        {
            using (Transaction trans = new Transaction(doc, "批量标注管径"))
            {
                trans.Start();

                IList<Element> pipetagscollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                FamilySymbol pipeDNtag = null;
                foreach (Element tag in pipetagscollect)
                {
                    FamilySymbol pipetag = tag as FamilySymbol;
                    if (pipetag.Name.Contains("管道公称直径"))
                    {
                        pipeDNtag = pipetag;
                        break;
                    }
                }

                FilteredElementCollector pipeCollector = new FilteredElementCollector(doc, uidoc.ActiveView.Id);
                pipeCollector.OfClass(typeof(Pipe)).OfCategory(BuiltInCategory.OST_PipeCurves);
                IList<Element> pipes = pipeCollector.ToElements();

                List<Pipe> allPipes = new List<Pipe>();
                List<Pipe> notePipes = new List<Pipe>();
                List<Pipe> notNotePipes = new List<Pipe>();

                foreach (Pipe item in pipes)
                {
                    allPipes.Add(item);
                }

                FilteredElementCollector pipeNoteCollector = new FilteredElementCollector(doc, uidoc.ActiveView.Id);
                IList<Element> pipeNotes = pipeNoteCollector.OfClass(typeof(IndependentTag)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                foreach (IndependentTag item in pipeNotes)
                {
                    notePipes.Add(item.GetTaggedLocalElement() as Pipe);
                }

                notNotePipes = allPipes.Except(notePipes, new NotePipeComparer()).ToList();

                foreach (Pipe pipe in notNotePipes)
                {

                    double pipeLength = pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                    double noteLength = Convert.ToDouble(NotePipes.mainfrm.LengthValue.Text);

                    if ((pipeLength * 304.83) >= noteLength)
                    {
                        Reference pipeRef = new Reference(pipe);
                        TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                        TagOrientation tagOri = TagOrientation.Horizontal;
                        //Add the tag to the middle of duct
                        LocationCurve locCurve = pipe.Location as LocationCurve;
                        XYZ pipeMid = locCurve.Curve.Evaluate(0.5, true);

                        IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, pipeMid);
                        tag.ChangeTypeId(pipeDNtag.Id);
                    }

                }
                trans.Commit();
            }
        }
    }
    public class NotePipeComparer : IEqualityComparer<Pipe>
    {
        public bool Equals(Pipe x, Pipe y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Pipe obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.ToString().GetHashCode();
            }
        }
    }
}
