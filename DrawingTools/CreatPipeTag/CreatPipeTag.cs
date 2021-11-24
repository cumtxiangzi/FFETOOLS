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
    public class CreatPipeTag : IExternalCommand
    {
        public static CreatPipeTagForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new CreatPipeTagForm();
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
    public class ExecuteEventCreatPipeTag : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                using (Transaction trans = new Transaction(doc, "给排水标注"))
                {
                    trans.Start();
                    if (CreatPipeTag.mainfrm.clicked == 1)
                    {
                        CreatEquipmentTagMethod(doc, uidoc);
                        CreatPipeTag.mainfrm.clicked = 0;
                    }

                    if (CreatPipeTag.mainfrm.clicked == 2)
                    {
                        CreatPipeTagMethod(doc, uidoc, "管道公称直径");
                        CreatPipeTag.mainfrm.clicked = 0;
                    }

                    if (CreatPipeTag.mainfrm.clicked == 3)
                    {
                        CreatPipeTagMethod(doc, uidoc, "管道系统缩写");
                        CreatPipeTag.mainfrm.clicked = 0;
                    }

                    if (CreatPipeTag.mainfrm.clicked == 4)
                    {
                        CreatPipeTagMethod(doc, uidoc, "进出户管编号");
                        CreatPipeTag.mainfrm.clicked = 0;
                    }

                    if (CreatPipeTag.mainfrm.clicked == 5)
                    {
                        CreatPipeTagWithLine(doc, uidoc, "立管编号");
                        CreatPipeTag.mainfrm.clicked = 0;
                    }

                    //if (CreatPipeTag.mainfrm.clicked == 6)
                    //{
                    //    CreatPipeTagMethod(doc, uidoc, app, "污水");
                    //    CreatPipeTag.mainfrm.clicked = 0;
                    //}

                    //if (CreatPipeTag.mainfrm.clicked == 7)
                    //{
                    //    CreatPipeTagMethod(doc, uidoc, app, CreatPipeTag.mainfrm.Button7.Content.ToString());
                    //    CreatPipeTag.mainfrm.clicked = 0;
                    //}

                    //if (CreatPipeTag.mainfrm.clicked == 8)
                    //{
                    //    CreatPipeTagMethod(doc, uidoc, app, CreatPipeTag.mainfrm.Button9.Content.ToString());
                    //    CreatPipeTag.mainfrm.clicked = 0;
                    //}

                    if (CreatPipeTag.mainfrm.clicked == 9)
                    {

                        RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.SpotElevation);
                        app.PostCommand(cmdId);
                        CreatPipeTag.mainfrm.clicked = 0;
                    }

                    if (CreatPipeTag.mainfrm.clicked == 10)
                    {
                        CreatPipeAccessoryTagMethod(doc, uidoc);
                        CreatPipeTag.mainfrm.clicked = 0;
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
            return "管道绘制";
        }
        public bool CreatPipeTagWithLine(Document doc, UIDocument uidoc, string tagName)
        {
            try
            {
                TagFamilyLoad(doc,"立管编号");
                Selection sel = uidoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element);
                Pipe pipe = doc.GetElement(reference) as Pipe;
                LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;

                XYZ pickPoint = reference.GlobalPoint;
                XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;

                IList<Element> pipetagscollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                FamilySymbol pipeTag = null;
                Family pipeCodeTag = null;
                foreach (var item in pipetagscollect)
                {
                    FamilySymbol pipetag = item as FamilySymbol;
                    Family ftag = pipetag.Family;
                    if (ftag.Name.Contains(tagName) && ftag.Name.Contains("给排水"))
                    {
                        pipeTag = pipetag;
                        pipeCodeTag = ftag;
                        break;
                    }
                }

                Reference pipeRef = new Reference(pipe);
                TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                TagOrientation tagOri = TagOrientation.Horizontal;
                IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, true, tageMode, tagOri, projectPickPoint);
                tag.ChangeTypeId(pipeTag.Id);
                tag.LeaderEndCondition = LeaderEndCondition.Free;

                if (uidoc.ActiveView is View3D)
                {
                    tag.LeaderEnd = projectPickPoint;
                    tag.LeaderElbow = projectPickPoint + new XYZ(100 / 304.8, 300 / 304.8, 200 / 304.8);
                    tag.TagHeadPosition = projectPickPoint + new XYZ(100 / 304.8, 469 / 304.8, 250 / 304.8);
                }
                else
                {
                    tag.LeaderEnd = projectPickPoint;
                    tag.LeaderElbow = projectPickPoint + new XYZ(800 / 304.8, 800 / 304.8, 0);
                    tag.TagHeadPosition = projectPickPoint + new XYZ(1019 / 304.8, 1019 / 304.8, 0);
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            //CreatPipeTagWithLine(doc, uidoc, tagName);
            return true;
        }
        public bool CreatPipeTagMethod(Document doc, UIDocument uidoc, string tagName)
        {
            try
            {
                TagFamilyLoad(doc, "管道公称直径");
                TagFamilyLoad(doc, "管道系统缩写");
                TagFamilyLoad(doc, "进出户管编号");

                Selection sel = uidoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element);
                Pipe pipe = doc.GetElement(reference) as Pipe;
                LocationCurve pipeLocationCurve = pipe.Location as LocationCurve;

                XYZ pickPoint = reference.GlobalPoint;
                XYZ projectPickPoint = pipeLocationCurve.Curve.Project(pickPoint).XYZPoint;

                IList<Element> pipetagscollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeTags).ToElements();
                FamilySymbol pipeTag = null;
                Family pipeCodeTag = null;
                foreach (var item in pipetagscollect)
                {
                    FamilySymbol pipetag = item as FamilySymbol;
                    Family ftag = pipetag.Family;
                    if (ftag.Name.Contains(tagName) && ftag.Name.Contains("给排水"))
                    {
                        pipeTag = pipetag;
                        pipeCodeTag = ftag;
                        break;
                    }
                }

                Reference pipeRef = new Reference(pipe);
                TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                TagOrientation tagOri = TagOrientation.Horizontal;
                IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, projectPickPoint);
                tag.ChangeTypeId(pipeTag.Id);

                if (tagName == "进出户管编号")
                {
                    ISet<ElementId> pipeCodeTagSet = pipeCodeTag.GetFamilySymbolIds();
                    List<FamilySymbol> pipeCodeTagList = new List<FamilySymbol>();
                    foreach (var item in pipeCodeTagSet)
                    {
                        pipeCodeTagList.Add(doc.GetElement(item) as FamilySymbol);
                    }

                    string pipeSystemName = (doc.GetElement(pipe.MEPSystem.GetTypeId()) as PipingSystemType).Name;
                    if (pipeSystemName.Contains("循环回水") || pipeSystemName.Contains("污水") || pipeSystemName.Contains("废水") || pipeSystemName.Contains("排泥"))
                    {
                        if ((ConnectorDirection(pipe) == "left") || (ConnectorDirection(pipe) == "down"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("左侧排水"))
                                {
                                    pipeTag = item;
                                    break;
                                }
                            }
                            tag.ChangeTypeId(pipeTag.Id);
                        }

                        if ((ConnectorDirection(pipe) == "right") || (ConnectorDirection(pipe) == "up"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("右侧排水"))
                                {
                                    pipeTag = item;
                                    break;
                                }
                            }
                            tag.ChangeTypeId(pipeTag.Id);
                            Location position = tag.Location;

                            if ((ConnectorDirection(pipe) == "right"))
                            {
                                XYZ direction = new XYZ(1200 / 304.8, 0, 0);
                                position.Move(direction);
                            }
                            if ((ConnectorDirection(pipe) == "up"))
                            {
                                XYZ direction = new XYZ(0, 1200 / 304.8, 0);
                                position.Move(direction);
                            }
                        }
                    }
                    if (pipeSystemName.Contains("循环给水") || pipeSystemName.Contains("消防给水") || pipeSystemName.Contains("生活给水") || pipeSystemName.Contains("中水") || pipeSystemName.Contains("水源输水"))
                    {
                        if ((ConnectorDirection(pipe) == "left") || (ConnectorDirection(pipe) == "down"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("左侧给水"))
                                {
                                    pipeTag = item;
                                    break;
                                }
                            }
                            tag.ChangeTypeId(pipeTag.Id);

                        }

                        if ((ConnectorDirection(pipe) == "right") || (ConnectorDirection(pipe) == "up"))
                        {
                            foreach (FamilySymbol item in pipeCodeTagList)
                            {
                                if (item.Name.Contains("右侧给水"))
                                {
                                    pipeTag = item;
                                    break;
                                }
                            }
                            tag.ChangeTypeId(pipeTag.Id);
                            Location position = tag.Location;

                            if ((ConnectorDirection(pipe) == "right"))
                            {
                                XYZ direction = new XYZ(1200 / 304.8, 0, 0);
                                position.Move(direction);
                            }
                            if ((ConnectorDirection(pipe) == "up"))
                            {
                                XYZ direction = new XYZ(0, 1200 / 304.8, 0);
                                position.Move(direction);
                            }

                        }
                    }
                }

                CreatPipeTagMethod(doc, uidoc, tagName);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;

        }
        public bool CreatEquipmentTagMethod(Document doc, UIDocument uidoc)
        {
            try
            {
                TagFamilyLoad(doc,"设备编号");
                Selection sel = uidoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element);
                FamilyInstance equipment = doc.GetElement(reference) as FamilyInstance;
                LocationPoint equipmentLocation = equipment.Location as LocationPoint;
                XYZ projectPickPoint = equipmentLocation.Point;

                IList<Element> equipmentTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipmentTags).ToElements();
                FamilySymbol equipmentTag = null;
                foreach (var item in equipmentTagsCollect)
                {
                    FamilySymbol etag = item as FamilySymbol;
                    Family ftag = etag.Family;
                    if (ftag.Name.Contains("设备编号") && ftag.Name.Contains("给排水"))
                    {
                        equipmentTag = etag;
                        break;
                    }
                }

                Reference pipeRef = new Reference(equipment);
                TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                TagOrientation tagOri = TagOrientation.Horizontal;
                IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, projectPickPoint);
                tag.ChangeTypeId(equipmentTag.Id);

                CreatEquipmentTagMethod(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;

        }
        public bool CreatPipeAccessoryTagMethod(Document doc, UIDocument uidoc)
        {
            try
            {
                TagFamilyLoad(doc, "管道附件编号");
                Selection sel = uidoc.Selection;
                Reference reference = sel.PickObject(ObjectType.Element);
                FamilyInstance accessory = doc.GetElement(reference) as FamilyInstance;
                LocationPoint accessoryLocation = accessory.Location as LocationPoint;
                XYZ projectPickPoint = accessoryLocation.Point;

                IList<Element> accessoryTagsCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessoryTags).ToElements();

                FamilySymbol accessoryTag = null;
                foreach (var item in accessoryTagsCollect)
                {
                    FamilySymbol etag = item as FamilySymbol;
                    Family ftag = etag.Family;
                    if (ftag.Name.Contains("管道附件") && ftag.Name.Contains("给排水"))
                    {
                        accessoryTag = etag;
                        break;
                    }
                }

                Reference pipeRef = new Reference(accessory);
                TagMode tageMode = TagMode.TM_ADDBY_CATEGORY;
                TagOrientation tagOri = TagOrientation.Horizontal;
                IndependentTag tag = IndependentTag.Create(doc, uidoc.ActiveView.Id, pipeRef, false, tageMode, tagOri, projectPickPoint);
                tag.ChangeTypeId(accessoryTag.Id);

                CreatPipeAccessoryTagMethod(doc, uidoc);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            return true;

        }
        public string ConnectorDirection(Pipe pipe)
        {
            string direction = null;
            ConnectorManager manager = pipe.ConnectorManager;
            ConnectorSet set = manager.UnusedConnectors;
            foreach (Connector item in set)
            {
                XYZ point = item.CoordinateSystem.BasisZ;
                if (point.X == -1)
                {
                    direction = "left";
                }
                if (point.X == 1)
                {
                    direction = "right";
                }
                if (point.Y == 1)
                {
                    direction = "up";
                }
                if (point.Y == -1)
                {
                    direction = "down";
                }
                break;
            }
            return direction;
        }
        public void TagFamilyLoad(Document doc, string categoryName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("给排水"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_注释符号_" + categoryName + ".rfa");              
            }
        }
    }
}

