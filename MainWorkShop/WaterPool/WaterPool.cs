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
    public class WaterPool : IExternalCommand
    {
        public static WaterPoolForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new WaterPoolForm();
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
    public class ExecuteEventWaterPool : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                View view = uidoc.ActiveView;
                if (view is ViewPlan)
                {
                    CreatWaterPool(doc, uidoc, sel);
                }
                else
                {
                    TaskDialog.Show("警告", "请在平面视图中进行操作");
                    WaterPool.mainfrm.Show();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public void CreatWaterPool(Document doc, UIDocument uidoc, Selection sel)
        {
            string poolBottomElevation = WaterPool.mainfrm.PoolBottomElevation.Text;
            double poolBottomElevationValue = WaterPool.mainfrm.PoolBottomElevationValue;
            double poolHeightValue = WaterPool.mainfrm.PoolHeightValue;
            double poolLehgthValue = WaterPool.mainfrm.PoolLengthValue;
            double poolWidthValue = WaterPool.mainfrm.PoolWidthValue;
            double manHoleSize = WaterPool.mainfrm.ManHoleSizeValue;
            double poolWallThick = 250 / 304.8;
            string coolTowerFlow = WaterPool.mainfrm.CoolTowerFlow.SelectedValue.ToString();
            int coolTowerFlowValue = Convert.ToInt32(coolTowerFlow);
            bool haveCoolTower = (bool)WaterPool.mainfrm.CoolTower.IsChecked;

            Level poolBottomlevel = null;
            Level poolToplevel = null;

            ViewPlan poolBottomPlan = null;
            ViewPlan viewPlan = null;
            ElementId newViewId = ElementId.InvalidElementId;

            Floor poolBottomFloor = null;
            Floor poolTopFloor = null;

            FamilySymbol manHoleSymbol = null;
            FamilySymbol poolColumnSymbol = null;
            FamilySymbol sumpSymbol = null;
            FamilySymbol coolTowerSymbol = null;
            FamilySymbol ladderSymbol = null;

            FamilyInstance manHole1 = null;
            FamilyInstance manHole2 = null;
            FamilyInstance ventPipe1 = null;
            FamilyInstance ventPipe2 = null;
            FamilyInstance ventPipe3 = null;
            FamilyInstance ventPipe4 = null;
            FamilyInstance sump = null;
            FamilyInstance coolTower = null;
            FamilyInstance ladder1 = null;
            FamilyInstance ladder2 = null;

            XYZ pickpoint = sel.PickPoint("请选择插入点");
            XYZ middlePoint = new XYZ(pickpoint.X + poolLehgthValue / 2 / 304.8, pickpoint.Y + poolWidthValue / 2 / 304.8, pickpoint.Z);

            TransactionGroup tg = new TransactionGroup(doc, "创建水池");
            tg.Start();

            using (Transaction trans = new Transaction(doc, "创建标高"))
            {
                trans.Start();

                if (ElevationExist(doc, (poolBottomElevationValue * 1000).ToString()))
                {
                    poolBottomlevel = GetLevel(doc, (poolBottomElevationValue * 1000).ToString());
                }
                else
                {
                    poolBottomlevel = Level.Create(doc, poolBottomElevationValue * 1000 / 304.8);
                    poolBottomlevel.Name = Convert.ToDouble(poolBottomElevation).ToString("0.000");
                    viewPlan = ViewPlan.Create(doc, GetViewFamilyType(doc).Id, poolBottomlevel.Id);//为新建的标高创建对应的视图                    
                }

                if (ElevationExist(doc, (poolBottomElevationValue * 1000 + poolHeightValue).ToString()))
                {
                    poolToplevel = GetLevel(doc, (poolBottomElevationValue * 1000 + poolHeightValue).ToString());
                }
                else
                {
                    poolToplevel = Level.Create(doc, (poolBottomElevationValue * 1000 + poolHeightValue) / 304.8);
                    poolToplevel.Name = (poolBottomElevationValue + poolHeightValue / 1000).ToString("0.000");
                    ViewPlan.Create(doc, GetViewFamilyType(doc).Id, poolToplevel.Id);
                }

                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "创建水池主体"))
            {
                trans.Start();

                IList<Curve> curves = new List<Curve>();
                XYZ point1 = new XYZ(pickpoint.X - poolWallThick / 2, pickpoint.Y - poolWallThick / 2, 0);
                XYZ point2 = new XYZ(pickpoint.X - poolWallThick / 2, pickpoint.Y + poolWallThick / 2 + poolWidthValue / 304.8, 0);
                XYZ point3 = new XYZ(pickpoint.X + poolWallThick / 2 + poolLehgthValue / 304.8, pickpoint.Y + poolWallThick / 2 + poolWidthValue / 304.8, 0);
                XYZ point4 = new XYZ(pickpoint.X + poolWallThick / 2 + poolLehgthValue / 304.8, pickpoint.Y - poolWallThick / 2, 0);
                curves.Add(Line.CreateBound(point1, point2));
                curves.Add(Line.CreateBound(point2, point3));
                curves.Add(Line.CreateBound(point3, point4));
                curves.Add(Line.CreateBound(point4, point1));

                foreach (Curve item in curves)
                {
                    Wall wall = Wall.Create(doc, item, PoolWallType(doc, "250").Id, poolBottomlevel.Id, poolHeightValue / 304.8, 0, false, true);
                    wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(poolToplevel.Id);
                    poolWallThick = wall.WallType.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble();
                }

                CurveArray array = new CurveArray();
                XYZ point11 = new XYZ(pickpoint.X - poolWallThick, pickpoint.Y - poolWallThick, 0);
                XYZ point21 = new XYZ(pickpoint.X - poolWallThick, pickpoint.Y + poolWallThick + poolWidthValue / 304.8, 0);
                XYZ point31 = new XYZ(pickpoint.X + poolWallThick + poolLehgthValue / 304.8, pickpoint.Y + poolWallThick + poolWidthValue / 304.8, 0);
                XYZ point41 = new XYZ(pickpoint.X + poolWallThick + poolLehgthValue / 304.8, pickpoint.Y - poolWallThick, 0);
                array.Append(Line.CreateBound(point11, point21));
                array.Append(Line.CreateBound(point21, point31));
                array.Append(Line.CreateBound(point31, point41));
                array.Append(Line.CreateBound(point41, point11));

                poolBottomFloor = doc.Create.NewFloor(array, PoolFloorType(doc, "250"), poolBottomlevel, true, XYZ.BasisZ);
                poolBottomFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0);

                poolTopFloor = doc.Create.NewFloor(array, PoolFloorType(doc, "200"), poolToplevel, true, XYZ.BasisZ);
                double topFloorThick = poolTopFloor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();
                poolTopFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(topFloorThick);

                ManHoleFamilyLoad(doc, "水池人孔");
                manHoleSymbol = ManHoleSymbol(doc);
                manHoleSymbol.Activate();
                XYZ manHolePoint1 = new XYZ(pickpoint.X + manHoleSize / 2 / 304.8, pickpoint.Y + manHoleSize / 2 / 304.8, pickpoint.Z);
                XYZ manHolePoint2 = new XYZ(pickpoint.X + poolLehgthValue / 304.8 - manHoleSize / 2 / 304.8, pickpoint.Y + poolWidthValue / 304.8 - manHoleSize / 2 / 304.8, pickpoint.Z);
                manHole1 = doc.Create.NewFamilyInstance(manHolePoint1, manHoleSymbol, poolTopFloor, poolToplevel, StructuralType.NonStructural);
                manHole1.LookupParameter("人孔半径").SetValueString((manHoleSize / 2).ToString());
                manHole2 = doc.Create.NewFamilyInstance(manHolePoint2, manHoleSymbol, poolTopFloor, poolToplevel, StructuralType.NonStructural);
                manHole2.LookupParameter("人孔半径").SetValueString((manHoleSize / 2).ToString());           

                double poolToplevelOffset = poolToplevel.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
                if (poolToplevelOffset * 304.8 > -400)
                {
                    manHole1.LookupParameter("人孔高度").SetValueString("200");
                    manHole2.LookupParameter("人孔高度").SetValueString("200");
                }
                else
                {
                    manHole1.LookupParameter("人孔高度").SetValueString(((-poolToplevelOffset * 304.8) - 200).ToString());
                    manHole2.LookupParameter("人孔高度").SetValueString(((-poolToplevelOffset * 304.8) - 200).ToString());
                }

                double ladderHeight = (poolHeightValue-1000)/304.8;
                ladderSymbol = LadderSymbol(doc);
                ladderSymbol.Activate();
                XYZ ladderPoint1 = new XYZ(pickpoint.X + manHoleSize / 2 / 304.8, pickpoint.Y+250/304.8, pickpoint.Z);
                XYZ ladderPoint2 = new XYZ(pickpoint.X + poolLehgthValue / 304.8 - manHoleSize / 2 / 304.8, pickpoint.Y + poolWidthValue / 304.8 - 250/ 304.8, pickpoint.Z);
                ladder1 = doc.Create.NewFamilyInstance(ladderPoint1, ladderSymbol, poolBottomlevel, StructuralType.NonStructural);
                ladder1.LookupParameter("护笼可见").Set(0);
                ladder1.LookupParameter("建筑爬梯_梯高").Set(ladderHeight);
                ladder2 = doc.Create.NewFamilyInstance(ladderPoint2, ladderSymbol, poolBottomlevel, StructuralType.NonStructural);
                ladder2.LookupParameter("护笼可见").Set(0);
                ladder2.LookupParameter("建筑爬梯_梯高").Set(ladderHeight);
                Line line = Line.CreateBound(ladderPoint2, ladderPoint2 + XYZ.BasisZ * 1);
                ElementTransformUtils.RotateElement(doc, ladder2.Id, line, Math.PI);

                PoolSumpFamilyLoad(doc);
                sumpSymbol = SumpSymbol(doc);
                sumpSymbol.Activate();
                XYZ sumpPoint = new XYZ(pickpoint.X + (poolLehgthValue - 300 - WaterPool.mainfrm.SumpLengthValue) / 304.8, pickpoint.Y + 300 / 304.8, pickpoint.Z);
                sump = doc.Create.NewFamilyInstance(sumpPoint, sumpSymbol, poolBottomFloor, poolBottomlevel, StructuralType.NonStructural);
                sump.LookupParameter("坑壁厚度").SetValueString("300");
                sump.LookupParameter("地坑高度").SetValueString((WaterPool.mainfrm.SumpHeightValue).ToString());
                sump.LookupParameter("地坑长度").SetValueString((WaterPool.mainfrm.SumpLengthValue).ToString());
                sump.LookupParameter("地坑宽度").SetValueString((WaterPool.mainfrm.SumpWidthValue).ToString());

                if (haveCoolTower == true)
                {
                    if (coolTowerFlow == "400" || coolTowerFlow == "500" || coolTowerFlow == "600" || coolTowerFlow == "700")
                    {
                        CoolTowerFamilyLoad(doc, "圆形逆流式冷却塔10BNG400-700");
                        coolTowerSymbol = CoolTowerSymbol(doc, "圆形逆流式冷却塔10BNG400-700");
                        coolTowerSymbol.Activate();
                        coolTower = doc.Create.NewFamilyInstance(middlePoint, coolTowerSymbol, poolToplevel, StructuralType.NonStructural);
                        coolTower.LookupParameter("偏移").SetValueString("200");
                        coolTower.LookupParameter("流量").SetValueString(coolTowerFlow);
                        if (poolToplevelOffset * 304.8 > -400)
                        {
                            coolTower.LookupParameter("基础高度").SetValueString("300");
                        }
                        else
                        {
                            coolTower.LookupParameter("基础高度").SetValueString(((-poolToplevelOffset * 304.8) - 100).ToString());
                        }
                    }
                    else if (coolTowerFlow == "8" || coolTowerFlow == "15" || coolTowerFlow == "30" || coolTowerFlow == "50")
                    {
                        CoolTowerFamilyLoad(doc, "圆形逆流式冷却塔10BNG8-50");
                        coolTowerSymbol = CoolTowerSymbol(doc, "圆形逆流式冷却塔10BNG8-50");
                        coolTowerSymbol.Activate();
                        coolTower = doc.Create.NewFamilyInstance(middlePoint, coolTowerSymbol, poolToplevel, StructuralType.NonStructural);
                        coolTower.LookupParameter("偏移").SetValueString("200");
                        coolTower.LookupParameter("流量").SetValueString(coolTowerFlow);
                        if (poolToplevelOffset * 304.8 > -400)
                        {
                            coolTower.LookupParameter("基础高度").SetValueString("300");
                        }
                        else
                        {
                            coolTower.LookupParameter("基础高度").SetValueString(((-poolToplevelOffset * 304.8) - 100).ToString());
                        }
                    }
                    else if (coolTowerFlow == "75" || coolTowerFlow == "100")
                    {
                        CoolTowerFamilyLoad(doc, "圆形逆流式冷却塔10BNG75-100");
                        coolTowerSymbol = CoolTowerSymbol(doc, "圆形逆流式冷却塔10BNG75-100");
                        coolTowerSymbol.Activate();
                        coolTower = doc.Create.NewFamilyInstance(middlePoint, coolTowerSymbol, poolToplevel, StructuralType.NonStructural);
                        coolTower.LookupParameter("偏移").SetValueString("200");
                        coolTower.LookupParameter("流量").SetValueString(coolTowerFlow);
                        if (poolToplevelOffset * 304.8 > -400)
                        {
                            coolTower.LookupParameter("基础高度").SetValueString("300");
                        }
                        else
                        {
                            coolTower.LookupParameter("基础高度").SetValueString(((-poolToplevelOffset * 304.8) - 100).ToString());
                        }
                    }
                    else if (coolTowerFlow == "150" || coolTowerFlow == "200")
                    {
                        CoolTowerFamilyLoad(doc, "圆形逆流式冷却塔10BNG150-200");
                        coolTowerSymbol = CoolTowerSymbol(doc, "圆形逆流式冷却塔10BNG150-200");
                        coolTowerSymbol.Activate();
                        coolTower = doc.Create.NewFamilyInstance(middlePoint, coolTowerSymbol, poolToplevel, StructuralType.NonStructural);
                        coolTower.LookupParameter("偏移").SetValueString("200");
                        coolTower.LookupParameter("流量").SetValueString(coolTowerFlow);
                        if (poolToplevelOffset * 304.8 > -400)
                        {
                            coolTower.LookupParameter("基础高度").SetValueString("300");
                        }
                        else
                        {
                            coolTower.LookupParameter("基础高度").SetValueString(((-poolToplevelOffset * 304.8) - 100).ToString());
                        }
                    }
                    else if (coolTowerFlow == "300")
                    {
                        CoolTowerFamilyLoad(doc, "圆形逆流式冷却塔10BNG300");
                        coolTowerSymbol = CoolTowerSymbol(doc, "圆形逆流式冷却塔10BNG300");
                        coolTowerSymbol.Activate();
                        coolTower = doc.Create.NewFamilyInstance(middlePoint, coolTowerSymbol, poolToplevel, StructuralType.NonStructural);
                        coolTower.LookupParameter("偏移").SetValueString("200");
                        coolTower.LookupParameter("流量").SetValueString(coolTowerFlow);
                        if (poolToplevelOffset * 304.8 > -400)
                        {
                            coolTower.LookupParameter("基础高度").SetValueString("300");
                        }
                        else
                        {
                            coolTower.LookupParameter("基础高度").SetValueString(((-poolToplevelOffset * 304.8) - 100).ToString());
                        }
                    }
                    else if (coolTowerFlow == "1000")
                    {
                        CoolTowerFamilyLoad(doc, "圆形逆流式冷却塔10BNG1000");
                        coolTowerSymbol = CoolTowerSymbol(doc, "圆形逆流式冷却塔10BNG1000");
                        coolTowerSymbol.Activate();
                        coolTower = doc.Create.NewFamilyInstance(middlePoint, coolTowerSymbol, poolToplevel, StructuralType.NonStructural);
                        coolTower.LookupParameter("偏移").SetValueString("200");
                        coolTower.LookupParameter("流量").SetValueString(coolTowerFlow);
                        if (poolToplevelOffset * 304.8 > -400)
                        {
                            coolTower.LookupParameter("基础高度").SetValueString("300");
                        }
                        else
                        {
                            coolTower.LookupParameter("基础高度").SetValueString(((-poolToplevelOffset * 304.8) - 100).ToString());
                        }
                    }
                }

                if (WaterPool.mainfrm.PoolColumn.IsChecked == true)
                {
                    PoolColumnFamilyLoad(doc);
                    poolColumnSymbol = PoolColumnSymbol(doc);
                    poolColumnSymbol.Activate();

                    string standSize = WaterPool.mainfrm.StandardSize.SelectedItem.ToString();
                    bool isSquare = (WaterPool.mainfrm.PoolShape.IsChecked == true);

                    if (standSize == "150")
                    {
                        if (isSquare && poolLehgthValue == 6800 && poolWidthValue == 6800 && poolHeightValue == 3500)
                        {
                            XYZ locationPoint = new XYZ(pickpoint.X + 3400 / 304.8, pickpoint.Y + 3400 / 304.8, pickpoint.Z);
                            FamilyInstance poolColumn = doc.Create.NewFamilyInstance(locationPoint, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                        }
                        else if (!isSquare && poolLehgthValue == 9450 && poolWidthValue == 5000 && poolHeightValue == 3500)
                        {
                            XYZ locationPoint1 = new XYZ(pickpoint.X + 3150 / 304.8, pickpoint.Y + 2500 / 304.8, pickpoint.Z);
                            XYZ locationPoint2 = new XYZ(pickpoint.X + 3150 * 2 / 304.8, pickpoint.Y + 2500 / 304.8, pickpoint.Z);

                            FamilyInstance poolColumn1 = doc.Create.NewFamilyInstance(locationPoint1, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn1.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn2 = doc.Create.NewFamilyInstance(locationPoint2, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn2.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);

                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "200")
                    {
                        if (isSquare && poolLehgthValue == 7800 && poolWidthValue == 7800 && poolHeightValue == 3500)
                        {
                            XYZ locationPoint = new XYZ(pickpoint.X + 3900 / 304.8, pickpoint.Y + 3900 / 304.8, pickpoint.Z);
                            FamilyInstance poolColumn = doc.Create.NewFamilyInstance(locationPoint, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                        }
                        else if (!isSquare && poolLehgthValue == 9600 && poolWidthValue == 6300 && poolHeightValue == 3500)
                        {
                            XYZ locationPoint1 = new XYZ(pickpoint.X + 3150 / 304.8, pickpoint.Y + 3150 / 304.8, pickpoint.Z);
                            XYZ locationPoint2 = new XYZ(pickpoint.X + (3150 + 3300) / 304.8, pickpoint.Y + 3150 / 304.8, pickpoint.Z);

                            FamilyInstance poolColumn1 = doc.Create.NewFamilyInstance(locationPoint1, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn1.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn2 = doc.Create.NewFamilyInstance(locationPoint2, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn2.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);

                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "300")
                    {
                        if (isSquare && poolLehgthValue == 9900 && poolWidthValue == 9900 && poolHeightValue == 3500)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 9900 / 304.8, 3);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else if (!isSquare && poolLehgthValue == 13900 && poolWidthValue == 6900 && poolHeightValue == 3500)
                        {
                            XYZ locationPoint1 = new XYZ(pickpoint.X + 3450 / 304.8, pickpoint.Y + 3450 / 304.8, pickpoint.Z);
                            XYZ locationPoint2 = new XYZ(pickpoint.X + (3450 + 3500) / 304.8, pickpoint.Y + 3450 / 304.8, pickpoint.Z);
                            XYZ locationPoint3 = new XYZ(pickpoint.X + (3450 + 3500 * 2) / 304.8, pickpoint.Y + 3450 / 304.8, pickpoint.Z);

                            FamilyInstance poolColumn1 = doc.Create.NewFamilyInstance(locationPoint1, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn1.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn2 = doc.Create.NewFamilyInstance(locationPoint2, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn2.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn3 = doc.Create.NewFamilyInstance(locationPoint3, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn3.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "400")
                    {
                        if (isSquare && poolLehgthValue == 11400 && poolWidthValue == 11400 && poolHeightValue == 3500)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 11400 / 304.8, 3);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else if (!isSquare && poolLehgthValue == 16000 && poolWidthValue == 8000 && poolHeightValue == 3500)
                        {
                            XYZ locationPoint1 = new XYZ(pickpoint.X + 4000 / 304.8, pickpoint.Y + 4000 / 304.8, pickpoint.Z);
                            XYZ locationPoint2 = new XYZ(pickpoint.X + (4000 + 4000) / 304.8, pickpoint.Y + 4000 / 304.8, pickpoint.Z);
                            XYZ locationPoint3 = new XYZ(pickpoint.X + (4000 + 4000 * 2) / 304.8, pickpoint.Y + 4000 / 304.8, pickpoint.Z);

                            FamilyInstance poolColumn1 = doc.Create.NewFamilyInstance(locationPoint1, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn1.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn2 = doc.Create.NewFamilyInstance(locationPoint2, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn2.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn3 = doc.Create.NewFamilyInstance(locationPoint3, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn3.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "500")
                    {
                        if (isSquare && poolLehgthValue == 11700 && poolWidthValue == 11700 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 11700 / 304.8, 3);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else if (!isSquare && poolLehgthValue == 16400 && poolWidthValue == 8200 && poolHeightValue == 4000)
                        {
                            XYZ locationPoint1 = new XYZ(pickpoint.X + 4100 / 304.8, pickpoint.Y + 4100 / 304.8, pickpoint.Z);
                            XYZ locationPoint2 = new XYZ(pickpoint.X + (4100 + 4100) / 304.8, pickpoint.Y + 4100 / 304.8, pickpoint.Z);
                            XYZ locationPoint3 = new XYZ(pickpoint.X + (4100 + 4100 * 2) / 304.8, pickpoint.Y + 4100 / 304.8, pickpoint.Z);

                            FamilyInstance poolColumn1 = doc.Create.NewFamilyInstance(locationPoint1, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn1.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn2 = doc.Create.NewFamilyInstance(locationPoint2, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn2.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn3 = doc.Create.NewFamilyInstance(locationPoint3, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn3.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "600")
                    {
                        if (isSquare && poolLehgthValue == 12900 && poolWidthValue == 12900 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 12900 / 304.8, 3);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else if (!isSquare && poolLehgthValue == 20000 && poolWidthValue == 8000 && poolHeightValue == 4000)
                        {
                            XYZ locationPoint1 = new XYZ(pickpoint.X + 4000 / 304.8, pickpoint.Y + 4000 / 304.8, pickpoint.Z);
                            XYZ locationPoint2 = new XYZ(pickpoint.X + (4000 + 4000) / 304.8, pickpoint.Y + 4000 / 304.8, pickpoint.Z);
                            XYZ locationPoint3 = new XYZ(pickpoint.X + (4000 + 4000 * 2) / 304.8, pickpoint.Y + 4000 / 304.8, pickpoint.Z);
                            XYZ locationPoint4 = new XYZ(pickpoint.X + (4000 + 4000 * 3) / 304.8, pickpoint.Y + 4000 / 304.8, pickpoint.Z);

                            FamilyInstance poolColumn1 = doc.Create.NewFamilyInstance(locationPoint1, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn1.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn2 = doc.Create.NewFamilyInstance(locationPoint2, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn2.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn3 = doc.Create.NewFamilyInstance(locationPoint3, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn3.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            FamilyInstance poolColumn4 = doc.Create.NewFamilyInstance(locationPoint4, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                            poolColumn4.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "800")
                    {
                        if (isSquare && poolLehgthValue == 14800 && poolWidthValue == 14800 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 14800 / 304.8, 4);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else if (!isSquare && poolLehgthValue == 18800 && poolWidthValue == 11200 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 3700 / 304.8, 18800 / 304.8, 11200 / 304.8, 3, 1);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "1000")
                    {
                        if (isSquare && poolLehgthValue == 15900 && poolWidthValue == 15900 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 3950 / 304.8, 15900 / 304.8, 2);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else if (!isSquare && poolLehgthValue == 22800 && poolWidthValue == 11400 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 3800 / 304.8, 22800 / 304.8, 11400 / 304.8, 4, 1);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "1500")
                    {
                        if (isSquare && poolLehgthValue == 19800 && poolWidthValue == 19800 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 3900 / 304.8, 19800 / 304.8, 3);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else if (!isSquare && poolLehgthValue == 26400 && poolWidthValue == 15000 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 3700 / 304.8, 26400 / 304.8, 15000 / 304.8, 5, 2);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                    else if (standSize == "2000")
                    {
                        if (isSquare && poolLehgthValue == 23400 && poolWidthValue == 23400 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 23400 / 304.8, 6);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else if (!isSquare && poolLehgthValue == 27300 && poolWidthValue == 19500 && poolHeightValue == 4000)
                        {

                            List<XYZ> locationPoints = GetColumnLocationPoints(pickpoint, 27300 / 304.8, 19500 / 304.8, 7, 5);
                            foreach (XYZ point in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(point, poolColumnSymbol, poolBottomlevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(poolToplevel.Id);
                            }
                        }
                        else
                        {
                            UnStandPoolColumn(doc, pickpoint, poolColumnSymbol, poolToplevel, poolBottomlevel, poolLehgthValue, poolWidthValue);
                        }
                    }
                }
                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "创建通气管"))
            {
                trans.Start();
                List<FamilyInstance> ventPipes = CreatVentPipe(doc, pickpoint, poolTopFloor, poolToplevel, poolLehgthValue, poolWidthValue);
                double poolToplevelOffset = poolToplevel.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();

                if (poolLehgthValue == poolWidthValue && poolLehgthValue >= 9000)
                {
                    if (poolToplevelOffset * 304.8 > -400)
                    {
                        ventPipe1 = ventPipes.ElementAt(0);
                        ventPipe1.LookupParameter("通气管顶部高度").SetValueString("900");
                        ventPipe2 = ventPipes.ElementAt(1);
                        ventPipe2.LookupParameter("通气管顶部高度").SetValueString("1400");
                        ventPipe3 = ventPipes.ElementAt(2);
                        ventPipe3.LookupParameter("通气管顶部高度").SetValueString("900");
                        ventPipe4 = ventPipes.ElementAt(3);
                        ventPipe4.LookupParameter("通气管顶部高度").SetValueString("1400");
                    }
                    else
                    {
                        ventPipe1 = ventPipes.ElementAt(0);
                        ventPipe1.LookupParameter("通气管顶部高度").SetValueString(((-poolToplevelOffset * 304.8) + 500).ToString());
                        ventPipe2 = ventPipes.ElementAt(1);
                        ventPipe2.LookupParameter("通气管顶部高度").SetValueString(((-poolToplevelOffset * 304.8) + 1000).ToString());
                        ventPipe3 = ventPipes.ElementAt(2);
                        ventPipe3.LookupParameter("通气管顶部高度").SetValueString(((-poolToplevelOffset * 304.8) + 500).ToString());
                        ventPipe4 = ventPipes.ElementAt(3);
                        ventPipe4.LookupParameter("通气管顶部高度").SetValueString(((-poolToplevelOffset * 304.8) + 1000).ToString());
                    }
                }
                else if (poolLehgthValue == poolWidthValue && poolLehgthValue < 9000)
                {
                    if (poolToplevelOffset * 304.8 > -400)
                    {
                        ventPipe1 = ventPipes.ElementAt(0);
                        ventPipe1.LookupParameter("通气管顶部高度").SetValueString("900");
                        ventPipe2 = ventPipes.ElementAt(1);
                        ventPipe2.LookupParameter("通气管顶部高度").SetValueString("1400");
                    }
                    else
                    {
                        ventPipe1 = ventPipes.ElementAt(0);
                        ventPipe1.LookupParameter("通气管顶部高度").SetValueString(((-poolToplevelOffset * 304.8) + 500).ToString());
                        ventPipe2 = ventPipes.ElementAt(1);
                        ventPipe2.LookupParameter("通气管顶部高度").SetValueString(((-poolToplevelOffset * 304.8) + 1000).ToString());
                    }
                }
                else if (!(poolLehgthValue == poolWidthValue))
                {
                    if (poolToplevelOffset * 304.8 > -400)
                    {
                        ventPipe1 = ventPipes.ElementAt(0);
                        ventPipe1.LookupParameter("通气管顶部高度").SetValueString("900");
                        ventPipe2 = ventPipes.ElementAt(1);
                        ventPipe2.LookupParameter("通气管顶部高度").SetValueString("1400");
                    }
                    else
                    {
                        ventPipe1 = ventPipes.ElementAt(0);
                        ventPipe1.LookupParameter("通气管顶部高度").SetValueString(((-poolToplevelOffset * 304.8) + 500).ToString());
                        ventPipe2 = ventPipes.ElementAt(1);
                        ventPipe2.LookupParameter("通气管顶部高度").SetValueString(((-poolToplevelOffset * 304.8) + 1000).ToString());
                    }
                }

                trans.Commit();
            }

            tg.Assimilate();
            WaterPool.mainfrm.Show();
        }
        public bool IsIntegerForDouble(double obj)
        {
            double eps = 1e-10;  // 精度范围
            return obj - Math.Floor(obj) < eps;
        }
        public List<FamilyInstance> CreatVentPipe(Document doc, XYZ point, Floor poolTopFloor, Level topLevel, double length, double width)
        {
            List<FamilyInstance> ventPipes = new List<FamilyInstance>();

            PoolVentPipeFamilyLoad(doc);
            FamilySymbol ventPipeSymbol = VentPipeSymbol(doc);
            ventPipeSymbol.Activate();

            List<Face> faceList = GetGeoFaces(poolTopFloor);
            Face poolTopFace = null;
            foreach (Face item in faceList)
            {
                if ((item.GetSurface() as Plane).Normal.Z == 1)
                {
                    poolTopFace = item;
                    break;
                }
            }

            if ((length == width) && length >= 9000)
            {
                XYZ ventPipePoint1 = new XYZ(point.X + 1000 / 304.8, point.Y + width / 2 / 304.8, point.Z);
                XYZ ventPipePoint2 = new XYZ(point.X + length / 2 / 304.8, point.Y + width / 304.8 - 1000 / 304.8, point.Z);
                XYZ ventPipePoint3 = new XYZ(point.X + length / 2 / 304.8, point.Y + 1000 / 304.8, point.Z);
                XYZ ventPipePoint4 = new XYZ(point.X + length / 304.8 - 1000 / 304.8, point.Y + width / 2 / 304.8, point.Z);

                FamilyInstance ventPipe1 = doc.Create.NewFamilyInstance(poolTopFace, ventPipePoint1, new XYZ(), ventPipeSymbol);
                FamilyInstance ventPipe2 = doc.Create.NewFamilyInstance(poolTopFace, ventPipePoint2, new XYZ(), ventPipeSymbol);
                FamilyInstance ventPipe3 = doc.Create.NewFamilyInstance(poolTopFace, ventPipePoint3, new XYZ(), ventPipeSymbol);
                FamilyInstance ventPipe4 = doc.Create.NewFamilyInstance(poolTopFace, ventPipePoint4, new XYZ(), ventPipeSymbol);

                ventPipes.Add(ventPipe1);
                ventPipes.Add(ventPipe2);
                ventPipes.Add(ventPipe3);
                ventPipes.Add(ventPipe4);
            }
            else if ((length == width) && length < 9000)
            {

                XYZ ventPipePoint2 = new XYZ(point.X + 1000 / 304.8, point.Y + width / 304.8 - 1000 / 304.8, point.Z);
                XYZ ventPipePoint4 = new XYZ(point.X + length / 304.8 - 1000 / 304.8, point.Y + 1000 / 304.8, point.Z);

                FamilyInstance ventPipe2 = doc.Create.NewFamilyInstance(poolTopFace, ventPipePoint2, new XYZ(), ventPipeSymbol);
                FamilyInstance ventPipe4 = doc.Create.NewFamilyInstance(poolTopFace, ventPipePoint4, new XYZ(), ventPipeSymbol);

                ventPipes.Add(ventPipe2);
                ventPipes.Add(ventPipe4);
            }
            else if (!(length == width))
            {
                XYZ ventPipePoint2 = new XYZ(point.X + 1000 / 304.8, point.Y + width / 304.8 - 1000 / 304.8, point.Z);
                XYZ ventPipePoint4 = new XYZ(point.X + length / 304.8 - 1000 / 304.8, point.Y + 1000 / 304.8, point.Z);

                FamilyInstance ventPipe2 = doc.Create.NewFamilyInstance(poolTopFace, ventPipePoint2, new XYZ(), ventPipeSymbol);
                FamilyInstance ventPipe4 = doc.Create.NewFamilyInstance(poolTopFace, ventPipePoint4, new XYZ(), ventPipeSymbol);

                ventPipes.Add(ventPipe2);
                ventPipes.Add(ventPipe4);
            }

            return ventPipes;
        }
        #region GetSolidsOfElement：从element里面获取实体的方法
        public List<Solid> GetSolidsOfElement(Element ele)
        {
            //生成事件，指定返回数据的特征
            Options options = new Options();
            options.DetailLevel = ViewDetailLevel.Fine;
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;
            //取得构件元素
            GeometryElement geoElement = ele.get_Geometry(options);
            List<GeometryObject> geoObj = new List<GeometryObject>();
            //递归获取集合元素的所有geometryobject
            GetAllObj(geoElement, ref geoObj);
            //转为solid的集合
            List<Solid> solids = geoObj.ConvertAll(m => m as Solid);
            return solids;
        }
        #endregion
        #region GetAllObj获得geometry的方法
        //获得geometryobject的递归算法
        public void GetAllObj(GeometryElement gele, ref List<GeometryObject> gobjs)
        {
            if (gele == null)
            {
                return;
            }
            //遍历geometryelement里面的geometryobject
            IEnumerator<GeometryObject> enumerator = gele.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GeometryObject geoObject = enumerator.Current;
                Type type = geoObject.GetType();
                //如果是嵌套的GeometryElement 
                if (type.Equals(typeof(GeometryElement)))
                {
                    //则递归
                    GetAllObj(geoObject as GeometryElement, ref gobjs);
                }
                //如果嵌套的geometryinstance
                else if (type.Equals(typeof(GeometryInstance)))
                {
                    //则用getinstancegeometry取得其中的geometryelement再递归
                    GetAllObj((geoObject as GeometryInstance).GetInstanceGeometry(), ref gobjs);
                }
                //如果是solid，则存入集合，递归结束
                else
                {
                    if (type.Equals(typeof(Solid)))
                    {
                        Solid solid = geoObject as Solid;
                        //去掉可能存在的空Solid
                        if (solid.Faces.Size > 0 || solid.Edges.Size > 0)
                        {
                            gobjs.Add(geoObject);
                        }
                    }
                }
            }
        }
        #endregion
        #region 获得元素的所有面
        public List<Face> GetGeoFaces(Element ele)
        {
            //存放集合元素的所有面
            List<Face> geoFaces = new List<Face>();
            //用上一节的方法取得所有几何体solid
            List<Solid> solids = GetSolidsOfElement(ele);
            //从集合体重提取所有face，存进集合
            foreach (Solid solid in solids)
            {
                foreach (Face face in solid.Faces)
                {
                    if (face.Area > 0 && (face.GetType().ToString() == "Autodesk.Revit.DB.PlanarFace"))

                        geoFaces.Add(face);

                }
            }
            return geoFaces;
        }
        #endregion

        public void UnStandPoolColumn(Document doc, XYZ point, FamilySymbol columnSymbol, Level topLevel, Level bottomLevel, double length, double width)
        {
            double divide;
            bool isDivide = false;
            if (length == width)
            {
                for (int i = 0; i < 11; i++)
                {
                    divide = length / (3100 + i * 100);
                    if (IsIntegerForDouble(divide))
                    {
                        List<XYZ> locationPoints = GetColumnLocationPoints(point, length / 304.8, Convert.ToInt32(divide));
                        foreach (XYZ p in locationPoints)
                        {
                            FamilyInstance poolColumn = doc.Create.NewFamilyInstance(p, columnSymbol, bottomLevel, StructuralType.NonStructural);
                            poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(topLevel.Id);
                        }
                        isDivide = true;
                        break;
                    }
                }
                if (isDivide == false)
                {
                    for (int a = 1; a < 100; a++)
                    {
                        if (((length / a) > 3300 && (length / a) < 4650))
                        {
                            List<XYZ> locationPoints = GetColumnLocationPoints(point, length / 304.8, a);
                            foreach (XYZ p in locationPoints)
                            {
                                FamilyInstance poolColumn = doc.Create.NewFamilyInstance(p, columnSymbol, bottomLevel, StructuralType.NonStructural);
                                poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(topLevel.Id);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                if (width >= 5000 && width <= 9000)
                {
                    List<XYZ> locationPoints = GetColumnLocationPoints(point, length, width);

                    foreach (XYZ p in locationPoints)
                    {
                        FamilyInstance poolColumn = doc.Create.NewFamilyInstance(p, columnSymbol, bottomLevel, StructuralType.NonStructural);
                        poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(topLevel.Id);
                    }
                }
                else
                {
                    List<XYZ> locationPoints = GetColumnLocationPointsUnStand(point, length, width);

                    foreach (XYZ p in locationPoints)
                    {
                        FamilyInstance poolColumn = doc.Create.NewFamilyInstance(p, columnSymbol, bottomLevel, StructuralType.NonStructural);
                        poolColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(topLevel.Id);
                    }
                }
            }
        }
        public List<XYZ> GetColumnLocationPoints(XYZ basePoint, double length, int divide)
        {
            //用于方形水池等分，适用于800立方水池及以下,适用于完全等分
            List<XYZ> points = new List<XYZ>();
            double len = length / divide;
            for (int x = 1; x < divide; x++)
            {
                for (int y = 1; y < divide; y++)
                {
                    points.Add(new XYZ(basePoint.X + x * len, basePoint.Y + y * len, basePoint.Z));
                }
            }
            return points;
        }
        public List<XYZ> GetColumnLocationPoints(XYZ basePoint, double edgeDistance, double length, int divide)
        {
            //用于方形水池等分，适用于800立方水池以上,适用于不完全等分
            List<XYZ> points = new List<XYZ>();
            double len = (length - edgeDistance * 2) / divide;

            for (int x = 0; x < divide + 1; x++)
            {
                for (int y = 0; y < divide + 1; y++)
                {
                    points.Add(new XYZ(basePoint.X + x * len + edgeDistance, basePoint.Y + y * len + edgeDistance, basePoint.Z));
                }
            }
            return points;
        }
        public List<XYZ> GetColumnLocationPoints(XYZ basePoint, double edgeDistance, double length, double width, int divideX, int divideY)
        {
            //用于矩形水池等分,适用于不完全等分
            List<XYZ> points = new List<XYZ>();
            double len = (length - edgeDistance * 2) / divideX;
            double wid = (width - edgeDistance * 2) / divideY;

            for (int x = 0; x < divideX + 1; x++)
            {
                for (int y = 0; y < divideY + 1; y++)
                {
                    points.Add(new XYZ(basePoint.X + x * len + edgeDistance, basePoint.Y + y * wid + edgeDistance, basePoint.Z));
                }
            }
            return points;
        }
        public List<XYZ> GetColumnLocationPoints(XYZ basePoint, double length, double width, int divideX, int divideY)
        {
            //用于矩形水池等分,适用于完全等分
            List<XYZ> points = new List<XYZ>();
            double len = length / divideX;
            double wid = width / divideY;

            for (int x = 1; x < divideX; x++)
            {
                for (int y = 1; y < divideY; y++)
                {
                    points.Add(new XYZ(basePoint.X + x * len, basePoint.Y + y * wid, basePoint.Z));
                }
            }
            return points;
        }
        public List<XYZ> GetColumnLocationPoints(XYZ basePoint, double length, double width)
        {
            //用于矩形非标准水池等分,单排
            int divide = 1;
            List<XYZ> points = new List<XYZ>();

            for (int a = 1; a < 100; a++)
            {
                if ((length / a) > 3300 && (length / a) < 4500)
                {
                    divide = a;
                    break;
                }

            }
            double len = length / 304.8 / divide;
            for (int x = 1; x < divide; x++)
            {
                points.Add(new XYZ(basePoint.X + x * len, basePoint.Y + width / 2 / 304.8, basePoint.Z));
            }
            return points;
        }
        public List<XYZ> GetColumnLocationPointsUnStand(XYZ basePoint, double length, double width)
        {
            //用于矩形非标准水池等分,多排           
            List<XYZ> points = new List<XYZ>();
            int divideX = 1;
            int divideY = 1;

            for (int a = 1; a < 100; a++)
            {
                if ((length / a) > 3300 && (length / a) < 4500)
                {
                    divideX = a;
                    break;
                }

            }
            double lenX = length / 304.8 / divideX;

            for (int b = 1; b < 100; b++)
            {
                if ((width / b) > 3000 && (width / b) < 4500)
                {
                    divideY = b;
                    break;
                }

            }
            double lenY = width / 304.8 / divideY;

            for (int x = 1; x < divideX; x++)
            {
                for (int y = 1; y < divideY; y++)
                {
                    points.Add(new XYZ(basePoint.X + x * lenX, basePoint.Y + y * lenY, basePoint.Z));
                }
            }
            return points;
        }
        public FamilySymbol ManHoleSymbol(Document doc)
        {
            FilteredElementCollector manHoleCollector = new FilteredElementCollector(doc);
            manHoleCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel);
            List<FamilySymbol> manHoleSymbolList = new List<FamilySymbol>();

            IList<Element> manHoles = manHoleCollector.ToElements();
            foreach (FamilySymbol item in manHoles)
            {
                if (item.Family.Name.Contains("结构") && item.Family.Name.Contains("水池人孔"))
                {
                    manHoleSymbolList.Add(item);
                }
            }
            return manHoleSymbolList.FirstOrDefault();
        }
        public FamilySymbol LadderSymbol(Document doc)
        {
            FilteredElementCollector manHoleCollector = new FilteredElementCollector(doc);
            manHoleCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel);
            List<FamilySymbol> manHoleSymbolList = new List<FamilySymbol>();

            IList<Element> manHoles = manHoleCollector.ToElements();
            foreach (FamilySymbol item in manHoles)
            {
                if (item.Family.Name.Contains("建筑_爬梯_default"))
                {
                    manHoleSymbolList.Add(item);
                }
            }
            return manHoleSymbolList.FirstOrDefault();
        }
        public FamilySymbol SumpSymbol(Document doc)
        {
            FilteredElementCollector manHoleCollector = new FilteredElementCollector(doc);
            manHoleCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel);
            List<FamilySymbol> manHoleSymbolList = new List<FamilySymbol>();

            IList<Element> manHoles = manHoleCollector.ToElements();
            foreach (FamilySymbol item in manHoles)
            {
                if (item.Family.Name.Contains("结构") && item.Family.Name.Contains("集水坑"))
                {
                    manHoleSymbolList.Add(item);
                }
            }
            return manHoleSymbolList.FirstOrDefault();
        }
        public FamilySymbol VentPipeSymbol(Document doc)
        {
            FilteredElementCollector pumpCollector = new FilteredElementCollector(doc);
            pumpCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            List<FamilySymbol> pumpSymbolList = new List<FamilySymbol>();

            IList<Element> pumps = pumpCollector.ToElements();
            foreach (FamilySymbol item in pumps)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains("弯管型通气管"))
                {
                    pumpSymbolList.Add(item);
                }
            }
            return pumpSymbolList.FirstOrDefault();
        }
        public FamilySymbol PoolColumnSymbol(Document doc)
        {
            FilteredElementCollector poolColumnCollector = new FilteredElementCollector(doc);
            poolColumnCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_Columns);
            List<FamilySymbol> poolHoleSymbolList = new List<FamilySymbol>();

            IList<Element> poolcolumns = poolColumnCollector.ToElements();
            foreach (FamilySymbol item in poolcolumns)
            {
                if (item.Family.Name.Contains("结构_柱_矩形混凝土柱_01"))
                {
                    poolHoleSymbolList.Add(item);
                }
            }
            return poolHoleSymbolList.FirstOrDefault();
        }
        public FamilySymbol CoolTowerSymbol(Document doc, string categoryName)
        {
            FilteredElementCollector poolColumnCollector = new FilteredElementCollector(doc);
            poolColumnCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            List<FamilySymbol> poolHoleSymbolList = new List<FamilySymbol>();

            IList<Element> poolcolumns = poolColumnCollector.ToElements();
            foreach (FamilySymbol item in poolcolumns)
            {
                if (item.Family.Name.Contains(categoryName))
                {
                    poolHoleSymbolList.Add(item);
                }
            }
            return poolHoleSymbolList.FirstOrDefault();
        }
        public void ManHoleFamilyLoad(Document doc, string categoryName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("结构"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "结构_楼面设备基础_" + categoryName + ".rfa");
            }
        }
        public void CoolTowerFamilyLoad(Document doc, string categoryName)
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
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_冷却设备_" + categoryName + ".rfa");
            }
        }
        public void PoolColumnFamilyLoad(Document doc)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains("结构_柱_矩形混凝土柱_01"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "结构_柱_矩形混凝土柱_01" + ".rfa");
            }
        }
        public void PoolSumpFamilyLoad(Document doc)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains("结构_地坑_集水坑01_1"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "结构_地坑_集水坑01_1" + ".rfa");
            }
        }
        public void PoolVentPipeFamilyLoad(Document doc)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains("给排水_给水构件_弯管型通气管"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_给水构件_弯管型通气管" + ".rfa");
            }
        }
        public WallType PoolWallType(Document doc, string wallThick)
        {
            WallType poolWallType = null;
            FilteredElementCollector collectorWallType = new FilteredElementCollector(doc);
            IList<Element> wallTypes = collectorWallType.OfClass(typeof(WallType)).ToElements();
            foreach (Element elem in wallTypes)
            {
                WallType wall = elem as WallType;
                if (wall.Name.Contains("结构_钢筋混凝土墙") && wall.Name.Contains(wallThick))
                {
                    poolWallType = wall;
                    break;
                }
            }
            return poolWallType;
        }
        public FloorType PoolFloorType(Document doc, string floorThick)
        {
            FloorType poolFloorType = null;

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(FloorType));
            IList<Element> levelTypeList = collector.ToElements();
            foreach (Element elem in collector)
            {
                FloorType floorType = elem as FloorType;
                if (floorType.Name.Contains("结构_混凝土楼板") && floorType.Name.Contains(floorThick))
                {
                    poolFloorType = floorType;
                    break;
                }
            }
            return poolFloorType;
        }
        public Level GetLevel(Document doc, string levelOffsetValue)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            IList<Element> levelList = collector.ToElements();
            Level level = null;

            foreach (Element e in levelList)
            {
                Level lev = e as Level;
                if (lev.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsValueString() == levelOffsetValue)
                {
                    level = lev;
                }
            }
            return level;
        }
        public ViewFamilyType GetViewFamilyType(Document doc)
        {
            ViewFamilyType view = null;
            FilteredElementCollector collectorViewFamilyType = new FilteredElementCollector(doc);
            IList<Element> viewFamilyTypes = collectorViewFamilyType.OfClass(typeof(ViewFamilyType)).ToElements();
            foreach (Element elem in viewFamilyTypes)
            {
                ViewFamilyType viewFamilyType = elem as ViewFamilyType;
                if (viewFamilyType.ViewFamily == ViewFamily.FloorPlan)
                {
                    view = viewFamilyType;
                    break;
                }
            }
            return view;
        }
        public bool ElevationExist(Document doc, string levelOffset)
        {
            bool levelExist = false;
            List<string> levelList = GetAllElevation(doc);
            foreach (string item in levelList)
            {
                if (item == levelOffset)
                {
                    levelExist = true;
                    break;
                }
            }
            return levelExist;
        }
        public List<string> GetAllElevation(Document doc)
        {
            // 获取全部标高
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            IList<Element> levelList = collector.ToElements();
            List<string> levelOffset = new List<string>();

            foreach (Element e in levelList)
            {
                Level level = e as Level;
                levelOffset.Add(level.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsValueString());
            }
            return levelOffset;
        }
        public string GetName()
        {
            return "创建水池";
        }
    }
    public class projectFamLoadOption : IFamilyLoadOptions
    {
        bool IFamilyLoadOptions.OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }
        bool IFamilyLoadOptions.OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Project;
            overwriteParameterValues = true;
            return true;
        }
    };
}
