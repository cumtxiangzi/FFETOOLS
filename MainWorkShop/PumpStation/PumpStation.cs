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
    public class PumpStation : IExternalCommand
    {
        public static PumpStationForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new PumpStationForm();
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
    public class ExecuteEventPumpStation : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
                View activeView = doc.ActiveView;

                List<Element> allElements = new List<Element>();
                ArrayList labels = GetAllLabelsOfGrids(doc);
                DisplayUnitType dut = GetLengthUnitType(doc);
                CreateOrthogonalGridsData m_data;
                List<Grid> XGrids = new List<Grid>();
                List<Grid> YGrids = new List<Grid>();
                List<Grid> XYGrids = new List<Grid>();

                ViewPlan roomBottomPlan = null;
                ViewPlan viewPlan = null;
                ElementId newViewId = ElementId.InvalidElementId;
                Level roomBottomlevel = null;
                Level roomToplevel = null;

                XYZ stairsPosition = null;

                List<RoomSetInfo> RoomSetInfoList = new List<RoomSetInfo>();
                for (int i = 0; i < PumpStation.mainfrm.RoomSettingGrid.Items.Count; i++)
                {
                    string roomName = PumpStation.mainfrm.GetComBoxValue(i, 1, "RoomName");
                    double roomLength = double.Parse(PumpStation.mainfrm.GetTextBlockValue(i, 2));
                    double roomBottom = double.Parse(PumpStation.mainfrm.GetTextBlockValue(i, 3));
                    double roomHeigth = double.Parse(PumpStation.mainfrm.RoomHeight.Text);
                    double roomWidth = double.Parse(PumpStation.mainfrm.RoomWidth.Text);

                    RoomSetInfoList.Add(new RoomSetInfo()
                    {
                        RoomCode = i,
                        RoomName = roomName,
                        RoomLength = roomLength,
                        RoomWidth = roomWidth,
                        RoomBottom = roomBottom,
                        RoomHeight = roomHeigth
                    });
                }

                if (activeView.IsTemplate != true && activeView.ViewType == ViewType.FloorPlan)
                {
                    TransactionGroup tg = new TransactionGroup(doc, "������վ");
                    tg.Start();

                    using (Transaction trans = new Transaction(doc, "�������"))
                    {
                        trans.Start();
                        activeView.Scale = 50;

                        foreach (var item in RoomSetInfoList)
                        {
                            double roomTopElevationValue = item.RoomHeight;
                            if (ElevationExist(doc, (roomTopElevationValue * 1000).ToString()))
                            {
                                roomToplevel = GetLevel(doc, (roomTopElevationValue * 1000).ToString());
                            }
                            else
                            {
                                roomToplevel = Level.Create(doc, roomTopElevationValue * 1000 / 304.8);
                                roomToplevel.Name = roomTopElevationValue.ToString("0.000");
                                viewPlan = ViewPlan.Create(doc, GetViewFamilyType(doc).Id, roomToplevel.Id);//Ϊ�½��ı�ߴ�����Ӧ����ͼ                    
                            }
                            break;
                        }

                        foreach (var item in RoomSetInfoList)
                        {
                            double roomBottomElevationValue = item.RoomBottom;
                            if (ElevationExist(doc, (roomBottomElevationValue * 1000).ToString()))
                            {
                                roomBottomlevel = GetLevel(doc, (roomBottomElevationValue * 1000).ToString());
                            }
                            else
                            {
                                roomBottomlevel = Level.Create(doc, roomBottomElevationValue * 1000 / 304.8);
                                roomBottomlevel.Name = roomBottomElevationValue.ToString("0.000");
                                viewPlan = ViewPlan.Create(doc, GetViewFamilyType(doc).Id, roomBottomlevel.Id);//Ϊ�½��ı�ߴ�����Ӧ����ͼ                    
                            }
                        }

                        trans.Commit();
                    }

                    double roomLehgthValue = 10000;
                    double roomWidthValue = 6000;

                    using (Transaction trans = new Transaction(doc, "��������"))
                    {
                        trans.Start();

                        List<double> roomLengthList = new List<double>();
                        CreateOrthogonalGridsData orthogonalData = new CreateOrthogonalGridsData(app, dut, labels);

                        m_data = orthogonalData;
                        m_data.XOrigin = Unit.CovertToAPI(0, m_data.Dut);
                        m_data.YOrigin = Unit.CovertToAPI(0, m_data.Dut);
                        m_data.XNumber = 2;
                        m_data.YNumber = (uint)(RoomSetInfoList.Count + 1);

                        int roomWidth = (int)RoomSetInfoList.FirstOrDefault().RoomWidth;
                        m_data.XSpacing = Unit.CovertToAPI(roomWidth, m_data.Dut);//��ֱ����
                        m_data.XBubbleLoc = BubbleLocation.StartPoint;
                        m_data.XFirstLabel = "A";

                        m_data.YSpacing = Unit.CovertToAPI(1000, m_data.Dut);//ˮƽ����
                        m_data.YBubbleLoc = BubbleLocation.StartPoint;
                        m_data.YFirstLabel = "1";

                        double roomTotalLength = 0;
                        foreach (var item in RoomSetInfoList)
                        {
                            roomTotalLength += item.RoomLength;
                        }

                        roomLehgthValue = roomTotalLength;
                        roomWidthValue = RoomSetInfoList.FirstOrDefault().RoomWidth;

                        orthogonalData.CreateGrids(Unit.CovertToAPI(roomTotalLength, m_data.Dut));
                        XGrids.AddRange(orthogonalData.XGrids);
                        YGrids.AddRange(orthogonalData.YGrids);
                        HideGridBubble(XGrids, activeView);
                        HideGridBubble(YGrids, activeView);

                        foreach (var item in RoomSetInfoList)
                        {
                            roomLengthList.Add(item.RoomLength);
                        }

                        double offset = 0;
                        for (int i = 0; i < roomLengthList.Count; i++)
                        {
                            offset += roomLengthList[i] - 1000 * (i + 1);
                            ElementTransformUtils.MoveElement(doc, YGrids[i + 1].Id, new XYZ(offset / 304.8, 0, 0));
                            offset += 1000 * (i + 1);
                        }

                        XYGrids.AddRange(orthogonalData.XGrids);
                        XYGrids.AddRange(orthogonalData.YGrids);
                        allElements.AddRange(XYGrids);

                        trans.Commit();
                    }

                    List<Wall> newWalls = new List<Wall>();
                    List<Wall> allNewBottomWalls = new List<Wall>();
                    List<Wall> newBottomWalls = new List<Wall>();
                    Floor roomBottomUnderFloor = null;
                    double roomBottomWallThick = 200 / 304.8;
                    List<CurveArray> holeOnFloorAry = new List<CurveArray>();
                    double roomBottomValue = 0;

                    using (Transaction trans = new Transaction(doc, "������ע����ǽ"))
                    {
                        trans.Start();

                        newWalls = AddWallByCrossGrids(doc, app, XYGrids, roomToplevel, GetLevel(doc, 0.ToString()), 0, false, false);//�ȴ�����0.000ƽ��ǽ
                        allElements.AddRange(newWalls);

                        FamilySymbol pitSymbol = null;
                        FamilyInstance pitInstance = null;
                        IList<FamilySymbol> pitSymbolList = CollectorHelper.TCollector<FamilySymbol>(doc);
                        pitSymbol = pitSymbolList.FirstOrDefault(x => x.FamilyName.Contains("�ṹ_�ؿ�_��ˮ��01_1"));

                        List<Grid> newGrids = new List<Grid>();
                        foreach (var item in RoomSetInfoList)
                        {
                            newGrids.AddRange(XGrids);
                            if (item.RoomBottom < 0)
                            {
                                roomBottomValue = item.RoomBottom;
                                string gridNum1 = (item.RoomCode + 1).ToString();
                                string gridNum2 = (item.RoomCode + 2).ToString();

                                foreach (var grid in YGrids)
                                {
                                    if (grid.Name == gridNum1 || grid.Name == gridNum2)
                                    {
                                        newGrids.Add(grid);
                                    }
                                }
                                newBottomWalls = AddWallByCrossGrids(doc, app, newGrids, GetLevel(doc, 0.ToString()),
                                    GetLevel(doc, (item.RoomBottom * 1000).ToString()), -100, false, false);//������0.000����ƽ��ǽ
                                allElements.AddRange(newBottomWalls);
                                allNewBottomWalls.AddRange(newBottomWalls);

                                List<XYZ> newBottomFloorPoints = GridCrossPoints(newGrids);
                                CurveArray curveArray = new CurveArray();
                                newBottomFloorPoints.Sort((a, b) => a.X.CompareTo(b.X));
                                newBottomFloorPoints.Sort((a, b) => a.Y.CompareTo(b.Y));

                                XYZ point1 = newBottomFloorPoints[0];
                                XYZ point2 = newBottomFloorPoints[1];
                                XYZ point3 = newBottomFloorPoints[2];
                                XYZ point4 = newBottomFloorPoints[3];

                                XYZ point11 = new XYZ(point1.X + roomBottomWallThick / 2, point1.Y - roomBottomWallThick / 2, 0);
                                XYZ point21 = new XYZ(point2.X - roomBottomWallThick / 2, point2.Y - roomBottomWallThick / 2, 0);
                                XYZ point31 = new XYZ(point3.X - roomBottomWallThick / 2, point3.Y + roomBottomWallThick / 2, 0);
                                XYZ point41 = new XYZ(point4.X + roomBottomWallThick / 2, point4.Y + roomBottomWallThick / 2, 0);

                                XYZ point12 = new XYZ(point1.X - roomBottomWallThick / 2, point1.Y + roomBottomWallThick / 2, 0);
                                XYZ point22 = new XYZ(point2.X + roomBottomWallThick / 2, point2.Y + roomBottomWallThick / 2, 0);
                                XYZ point32 = new XYZ(point3.X + roomBottomWallThick / 2, point3.Y - roomBottomWallThick / 2, 0);
                                XYZ point42 = new XYZ(point4.X - roomBottomWallThick / 2, point4.Y - roomBottomWallThick / 2, 0);

                                curveArray.Append(Line.CreateBound(point11, point21));
                                curveArray.Append(Line.CreateBound(point21, point31));
                                curveArray.Append(Line.CreateBound(point31, point41));
                                curveArray.Append(Line.CreateBound(point41, point11));

                                CurveArray holeCurveArray = new CurveArray();
                                holeCurveArray.Append(Line.CreateBound(point12, point22));
                                holeCurveArray.Append(Line.CreateBound(point22, point32));
                                holeCurveArray.Append(Line.CreateBound(point32, point42));
                                holeCurveArray.Append(Line.CreateBound(point42, point12));

                                if (curveArray.Size != 0)
                                {
                                    roomBottomUnderFloor = doc.Create.NewFloor(curveArray, PoolFloorType(doc, "250"),
                                        GetLevel(doc, (item.RoomBottom * 1000).ToString()), true, XYZ.BasisZ);//������0.000����¥��                           
                                    roomBottomUnderFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0);
                                    allElements.Add(roomBottomUnderFloor);

                                    XYZ pitPoint = new XYZ(point2.X + roomBottomWallThick / 2, point2.Y + roomBottomWallThick / 2+(roomWidthValue-1000)/304.8, 0);
                                    pitSymbol.Activate();
                                    pitInstance = doc.Create.NewFamilyInstance(pitPoint, pitSymbol, roomBottomUnderFloor,
                                        GetLevel(doc, (item.RoomBottom * 1000).ToString()), StructuralType.NonStructural);
                                    pitInstance.LookupParameter("�ӱں��").SetValueString("200");
                                    pitInstance.LookupParameter("�ؿӸ߶�").SetValueString("800");
                                    pitInstance.LookupParameter("�ؿӳ���").SetValueString("800");
                                    pitInstance.LookupParameter("�ؿӿ��").SetValueString("800");
                                    allElements.Add(pitInstance);
                                }

                                if (holeCurveArray.Size != 0)
                                {
                                    holeOnFloorAry.Add(holeCurveArray);
                                }
                            }
                            newGrids.Clear();
                        }

                        List<Dimension> dmList1 = CreatGridDemesion(doc, activeView, XGrids);
                        List<Dimension> dmList2 = CreatGridDemesion(doc, activeView, YGrids);
                        allElements.AddRange(dmList1);
                        allElements.AddRange(dmList2);

                        trans.Commit();
                    }

                    Floor roomBottomFloor = null;
                    Floor roomTopFloor = null;
                    XYZ pickpoint = new XYZ();
                    double roomWallThick = 100 / 304.8;

                    using (Transaction trans = new Transaction(doc, "���ɵ��漰�ݶ�"))
                    {
                        trans.Start();

                        CurveArray array = new CurveArray();
                        XYZ point11 = new XYZ(pickpoint.X - roomWallThick, pickpoint.Y - roomWallThick, 0);
                        XYZ point21 = new XYZ(pickpoint.X - roomWallThick, pickpoint.Y + roomWallThick + roomWidthValue / 304.8, 0);
                        XYZ point31 = new XYZ(pickpoint.X + roomWallThick + roomLehgthValue / 304.8, pickpoint.Y + roomWallThick + roomWidthValue / 304.8, 0);
                        XYZ point41 = new XYZ(pickpoint.X + roomWallThick + roomLehgthValue / 304.8, pickpoint.Y - roomWallThick, 0);
                        array.Append(Line.CreateBound(point11, point21));
                        array.Append(Line.CreateBound(point21, point31));
                        array.Append(Line.CreateBound(point31, point41));
                        array.Append(Line.CreateBound(point41, point11));

                        CurveArray array2 = new CurveArray();
                        XYZ point12 = new XYZ(pickpoint.X - roomWallThick - 800 / 304.8, pickpoint.Y - roomWallThick - 800 / 304.8, 0);
                        XYZ point22 = new XYZ(pickpoint.X - roomWallThick - 800 / 304.8, pickpoint.Y + roomWallThick + roomWidthValue / 304.8 + 800 / 304.8, 0);
                        XYZ point32 = new XYZ(pickpoint.X + roomWallThick + roomLehgthValue / 304.8 + 800 / 304.8, pickpoint.Y + roomWallThick + roomWidthValue / 304.8 + 800 / 304.8, 0);
                        XYZ point42 = new XYZ(pickpoint.X + roomWallThick + roomLehgthValue / 304.8 + 800 / 304.8, pickpoint.Y - roomWallThick - 800 / 304.8, 0);
                        array2.Append(Line.CreateBound(point12, point22));
                        array2.Append(Line.CreateBound(point22, point32));
                        array2.Append(Line.CreateBound(point32, point42));
                        array2.Append(Line.CreateBound(point42, point12));

                        roomBottomFloor = doc.Create.NewFloor(array, PoolFloorType(doc, "100"), GetLevel(doc, 0.ToString()), true, XYZ.BasisZ);//������0.000ƽ��¥��
                        roomBottomFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0);
                        allElements.Add(roomBottomFloor);

                        roomTopFloor = doc.Create.NewFloor(array2, PoolFloorType(doc, "100"), roomToplevel, true, XYZ.BasisZ);//�����ݶ�¥��
                        double topFloorThick = roomTopFloor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();
                        roomTopFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(100 / 304.8);
                        allElements.Add(roomTopFloor);

                        trans.Commit();
                    }

                    using (Transaction trans = new Transaction(doc, "¥�忪���������Ŵ�"))
                    {
                        trans.Start();

                        foreach (var item in RoomSetInfoList)
                        {
                            if (item.RoomBottom < 0)
                            {
                                foreach (var arry in holeOnFloorAry)
                                {
                                    Opening hole = doc.Create.NewOpening(roomBottomFloor, arry, true);
                                    allElements.Add(hole);
                                }
                                break;
                            }
                        }

                        trans.Commit();
                    }

                    using (Transaction trans = new Transaction(doc, "�����Ŵ������ƽ̨"))
                    {
                        trans.Start();

                        List<Element> instances = CreatDoorAndWindow(doc, newWalls, roomWidthValue);//�����źʹ�
                        allElements.AddRange(instances);

                        List<Element> floors = CreatFloorEnter(doc, RoomSetInfoList, allNewBottomWalls);//�������ƽ̨������
                        allElements.AddRange(floors);

                        List<FamilyInstance> steelAndHoists = CreatSteelAndHoist(doc, newWalls);//�������ָֺ��ֶ���«
                        allElements.AddRange(steelAndHoists);

                        trans.Commit();
                    }

                    List<TextNote> roomNameList = new List<TextNote>();
                    using (Transaction trans = new Transaction(doc, "����ɢˮ������"))
                    {
                        trans.Start();

                        GetSlabEdge(doc, roomBottomFloor);//����ɢˮ

                        List<string> names = new List<string>();
                        foreach (var item in RoomSetInfoList)
                        {
                            names.Add(item.RoomName);
                        }

                        List<XYZ> points = TextPoints(newWalls, roomWidthValue);
                        roomNameList = RoomNameText(doc, activeView, names, points);//��������                       

                        trans.Commit();
                    }

                    List<Stairs> allStairs = CreatStairsMain(doc, allNewBottomWalls, RoomSetInfoList);//����ֱ��¥��
                    allElements.AddRange(allStairs);

                    using (Transaction trans = new Transaction(doc, "�����վ"))
                    {
                        trans.Start();

                        MoveText(doc, activeView, roomNameList);//�ƶ�����
                        allElements.AddRange(roomNameList);

                        List<ElementId> mirroElementsID = new List<ElementId>();
                        foreach (var item in allElements)
                        {
                            if (ElementTransformUtils.CanMirrorElement(doc, item.Id))
                            {
                                mirroElementsID.Add(item.Id);
                            }
                        }

                        if (PumpStation.mainfrm.RoomMirro.IsChecked == true)
                        {
                            ElementTransformUtils.MirrorElements(doc, mirroElementsID, Plane.CreateByNormalAndOrigin(new XYZ(0, 1, 0), new XYZ(0, 0, 0)), false);
                        }

                        trans.Commit();
                    }

                    tg.Assimilate();
                }
                else
                {
                    MessageBox.Show("����ƽ����ͼ�в���", "����", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public string GetName()
        {
            return "������վ";
        }
        public List<FamilyInstance> CreatSteelAndHoist(Document doc, List<Wall> walls)
        {
            List<FamilyInstance> instances = new List<FamilyInstance>();

            FamilySymbol steelBeamSymbol = null;
            FamilyInstance steelBeamInstance = null;
            IList<FamilySymbol> steelSymbolList = CollectorHelper.TCollector<FamilySymbol>(doc);
            steelBeamSymbol = steelSymbolList.FirstOrDefault(x => x.FamilyName.Contains("�ṹ_���ָ�"));

            FamilySymbol hoistSymbol = null;
            FamilyInstance hoistInstance = null;
            IList<FamilySymbol> hoistSymbolList = CollectorHelper.TCollector<FamilySymbol>(doc);
            hoistSymbol = hoistSymbolList.FirstOrDefault(x => x.FamilyName.Contains("����ˮ_���������豸_�ֶ���«"));

            foreach (var item in walls)
            {
                Line line = null;
                LocationCurve locationCurve = item.Location as LocationCurve;
                if (locationCurve != null)
                {
                    line = locationCurve.Curve as Line;
                }

                double wallLength = double.Parse(item.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString());
                if (wallLength > 6700)
                {
                    if (line.Direction.X == 1 && line.Direction.Y == 0)
                    {
                        if (line.GetEndPoint(0).Y == 0)
                        {
                            double roomWidth = Convert.ToDouble(PumpStation.mainfrm.RoomWidth.Text);
                            double roomHeight = Convert.ToDouble(PumpStation.mainfrm.RoomHeight.Text);

                            XYZ steelPoint = new XYZ(line.GetEndPoint(0).X + (wallLength - 3100 + 3100) / 304.8 / 2, line.GetEndPoint(1).Y + roomWidth / 2 / 304.8, line.GetEndPoint(1).Z);
                            steelBeamSymbol.Activate();
                            steelBeamInstance = doc.Create.NewFamilyInstance(steelPoint, steelBeamSymbol, GetLevel(doc, 0.ToString()), StructuralType.NonStructural);
                            steelBeamInstance.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(roomHeight * 1000 / 304.8 - 500 / 304.8);
                            Line rotateLine = Line.CreateBound(steelPoint, steelPoint + XYZ.BasisZ * 1);
                            ElementTransformUtils.RotateElement(doc, steelBeamInstance.Id, rotateLine, Math.PI / 2);
                            steelBeamInstance.LookupParameter("��").Set((wallLength - 3100 + 3100) / 304.8);

                            XYZ hoistPoint = new XYZ(line.GetEndPoint(0).X + (wallLength - 3100 + 3100) / 304.8 / 2, line.GetEndPoint(1).Y + roomWidth / 2 / 304.8, line.GetEndPoint(1).Z);
                            hoistSymbol.Activate();
                            hoistInstance = doc.Create.NewFamilyInstance(hoistPoint, hoistSymbol, GetLevel(doc, 0.ToString()), StructuralType.NonStructural);
                            hoistInstance.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(roomHeight * 1000 / 304.8 + 100 / 304.8);
                            Line rotateLineHoist = Line.CreateBound(hoistPoint, hoistPoint + XYZ.BasisZ * 1);
                            ElementTransformUtils.RotateElement(doc, hoistInstance.Id, rotateLineHoist, Math.PI / 2);

                            instances.Add(steelBeamInstance);
                            instances.Add(hoistInstance);
                        }
                    }
                }
            }
            return instances;
        }
        public List<Element> CreatFloorEnter(Document RevitDoc, List<RoomSetInfo> roomSetInfos, List<Wall> walls)
        {
            List<Element> floors = new List<Element>();
            double bottomVlaue = 0;
            RailingType railType = null;         

            IList<RailingType> rails = CollectorHelper.TCollector<RailingType>(RevitDoc);
            foreach (var item in rails)
            {
                if (item.Name.Contains("����_ƽ̨����_1"))
                {
                    railType = item;
                    break;
                }
            }

            foreach (var roomInfo in roomSetInfos)
            {
                if (roomInfo.RoomBottom < 0 && roomInfo.RoomLength > 6700)
                {
                    bottomVlaue = -1;
                    break;
                }
            }

            if (bottomVlaue < 0)
            {
                foreach (var item in walls)
                {
                    Line line = null;
                    LocationCurve locationCurve = item.Location as LocationCurve;
                    if (locationCurve != null)
                    {
                        line = locationCurve.Curve as Line;
                    }

                    double wallLength = double.Parse(item.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString());
                    if (wallLength > 6700)
                    {
                        if (line.Direction.X == 1 && line.Direction.Y == 0)
                        {
                            if (line.GetEndPoint(0).Y == 0)
                            {
                                XYZ point1 = new XYZ(line.GetEndPoint(1).X - 100 / 304.8, line.GetEndPoint(1).Y + 100 / 304.8, line.GetEndPoint(1).Z);
                                XYZ point2 = new XYZ(line.GetEndPoint(1).X - 3000 / 304.8, line.GetEndPoint(1).Y + 100 / 304.8, line.GetEndPoint(1).Z);
                                XYZ point3 = new XYZ(line.GetEndPoint(1).X - 3000 / 304.8, line.GetEndPoint(1).Y + 2000 / 304.8, line.GetEndPoint(1).Z);
                                XYZ point4 = new XYZ(line.GetEndPoint(1).X - 100 / 304.8, line.GetEndPoint(1).Y + 2000 / 304.8, line.GetEndPoint(1).Z);

                                CurveArray curveArray = new CurveArray();
                                curveArray.Append(Line.CreateBound(point1, point2));
                                curveArray.Append(Line.CreateBound(point2, point3));
                                curveArray.Append(Line.CreateBound(point3, point4));
                                curveArray.Append(Line.CreateBound(point4, point1));

                                Floor roomPlatformFloor = RevitDoc.Create.NewFloor(curveArray, PoolFloorType(RevitDoc, "100"), GetLevel(RevitDoc, 0.ToString()), true, XYZ.BasisZ);//������0.000���ƽ̨
                                roomPlatformFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(0);
                                floors.Add(roomPlatformFloor);

                                XYZ point22 = new XYZ(line.GetEndPoint(1).X - 3000 / 304.8, line.GetEndPoint(1).Y + 1150 / 304.8, line.GetEndPoint(1).Z);
                                IList<Curve> lines = new List<Curve>();
                                lines.Add(Line.CreateBound(point22, point3));
                                lines.Add(Line.CreateBound(point3, point4));
                                CurveLoop railLoop = CurveLoop.Create(lines);
                                Railing floorRail = Railing.Create(RevitDoc, railLoop, railType.Id, GetLevel(RevitDoc, 0.ToString()).Id);//����ƽ̨����
                                floors.Add(floorRail);

                            }
                        }
                    }
                }
            }

            return floors;
        }
        public List<Stairs> CreatStairsMain(Document RevitDoc, List<Wall> walls, List<RoomSetInfo> roomSetInfos) //����ֱ��¥�ݣ�������ͼ¥��������
        {
            List<Stairs> stairsList = new List<Stairs>();
            double bottomVlaue = 0;

            foreach (var roomInfo in roomSetInfos)
            {
                if (roomInfo.RoomBottom < 0 && roomInfo.RoomLength > 6700)
                {
                    bottomVlaue = -1;
                    break;
                }
            }

            if (bottomVlaue < 0)
            {
                foreach (var item in walls)
                {
                    Line line = null;
                    LocationCurve locationCurve = item.Location as LocationCurve;
                    if (locationCurve != null)
                    {
                        line = locationCurve.Curve as Line;
                    }

                    double wallLength = double.Parse(item.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString());
                    if (wallLength > 6700)
                    {
                        if (line.Direction.X == 1 && line.Direction.Y == 0)
                        {
                            if (line.GetEndPoint(0).Y == 0)
                            {
                                XYZ point = new XYZ(line.GetEndPoint(1).X - 3000 / 304.8, 0, 0);//���ƽ̨¥�����

                                ElementId stairsID = CreateStairs(RevitDoc, point, RevitDoc.GetElement((item.LevelId)) as Level, GetLevel(RevitDoc, 0.ToString()));//����¥�ݱȽ������һ������
                                Stairs newStairs = RevitDoc.GetElement(stairsID) as Stairs;

                                using (Transaction trans = new Transaction(RevitDoc, "�޸�¥��̤����"))
                                {
                                    trans.Start();

                                    int stairsNum = newStairs.get_Parameter(BuiltInParameter.STAIRS_ACTUAL_NUM_RISERS).AsInteger();
                                    newStairs.get_Parameter(BuiltInParameter.STAIRS_DESIRED_NUMBER_OF_RISERS).Set(stairsNum);

                                    trans.Commit();
                                }
                                stairsList.Add(newStairs);
                            }
                        }
                    }
                }
            }

            return stairsList;
        }
        private ElementId CreateStairs(Document document, XYZ stairPoint, Level levelBottom, Level levelTop)//����¥��
        {
            ElementId newStairsId = null;
            double length = 10;

            using (StairsEditScope newStairsScope = new StairsEditScope(document, "����¥��"))
            {
                newStairsId = newStairsScope.Start(levelBottom.Id, levelTop.Id);
                using (Transaction stairsTrans = new Transaction(document, "�����ݶκ�����"))
                {
                    stairsTrans.Start();

                    double height = levelTop.Elevation - levelBottom.Elevation;
                    double heightStringValue = Convert.ToDouble((levelBottom.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsValueString())) / 1000;

                    if (IsIntegerForDouble(heightStringValue))
                    {
                        length = height - 200 / 304.8;
                    }
                    else
                    {
                        length = height - 100 / 304.8;
                    }

                    // ����һ��ֱ���ݶ�
                    Line locationLine = Line.CreateBound(new XYZ(stairPoint.X - length, 700 / 304.8, levelBottom.Elevation), new XYZ(stairPoint.X, 700 / 304.8, levelBottom.Elevation));
                    StairsRun newRun2 = StairsRun.CreateStraightRun(document, newStairsId, locationLine, StairsRunJustification.Center);
                    newRun2.ActualRunWidth = 900 / 304.8;

                    stairsTrans.Commit();
                }
                // ������Ϣ����.
                newStairsScope.Commit(new FailuresPreprocessor());
            }
            return newStairsId;
        }
        public bool IsIntegerForDouble(double obj)//�ж�double�Ƿ�������
        {
            double eps = 1e-10;  // ���ȷ�Χ
            return obj - Math.Floor(obj) < eps;
        }
        public void MoveText(Document doc, View view, List<TextNote> textList)
        {
            foreach (var item in textList)
            {
                BoundingBoxXYZ box = item.get_BoundingBox(view);
                double length = box.Max.X - box.Min.X;
                ElementTransformUtils.MoveElement(doc, item.Id, new XYZ(-length / 2, 0, 0));
            }
        }
        public List<XYZ> TextPoints(List<Wall> walls, double roomWidth)
        {
            List<XYZ> textPoints = new List<XYZ>();
            foreach (var item in walls)
            {
                Line line = null;
                LocationCurve locationCurve = item.Location as LocationCurve;
                if (locationCurve != null)
                {
                    line = locationCurve.Curve as Line;
                }

                if (line.Direction.X == 1 && line.Direction.Y == 0)
                {
                    if (line.GetEndPoint(0).Y == 0)
                    {
                        XYZ midPoint = (line.GetEndPoint(0) + line.GetEndPoint(1)) / 2;
                        XYZ textPoint = new XYZ(midPoint.X, midPoint.Y + roomWidth / 2 / 304.8, midPoint.Z);
                        textPoints.Add(textPoint);
                    }
                }
            }

            return textPoints;
        }
        public List<TextNote> RoomNameText(Document doc, View view, List<string> roomNames, List<XYZ> points)
        {
            List<TextNote> list = new List<TextNote>();
            TextNoteType type = null;
            IList<TextNoteType> noteTypes = CollectorHelper.TCollector<TextNoteType>(doc);

            foreach (var item in noteTypes)
            {
                if (item.Name.Contains("����ˮ-�ָ�5"))
                {
                    type = item;
                    break;
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                TextNote note = TextNote.Create(doc, view.Id, points[i], roomNames[i], type.Id);
                list.Add(note);
            }

            return list;
        }
        public List<XYZ> GridCrossPoints(List<Grid> allGrids)
        {
            //��ȡ���������н���
            List<XYZ> Points = new List<XYZ>();
            foreach (Grid grid in allGrids)
            {
                Grid currentGrid = grid;
                foreach (Grid grd in allGrids)
                {
                    IntersectionResultArray ira = null;
                    SetComparisonResult scr = currentGrid.Curve.Intersect(grd.Curve, out ira);
                    if (ira != null)
                    {
                        IntersectionResult ir = ira.get_Item(0);

                        // �жϵ��Ƿ��ظ�
                        if (!CheckPoint(Points, ir.XYZPoint))
                        {
                            Points.Add(ir.XYZPoint);
                        }
                    }
                }
            }
            return Points;
        }
        private bool CheckPoint(List<XYZ> points, XYZ point) //�ж����߽����Ƿ��ظ�
        {
            bool flag = false;
            foreach (XYZ p in points)
            {
                if (p.IsAlmostEqualTo(point))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        public List<Element> CreatDoorAndWindow(Document RevitDoc, List<Wall> walls, double roomWidth)
        {
            List<Element> instanceLists = new List<Element>();
            FamilySymbol doorType1800 = null;
            FamilySymbol doorType1500 = null;
            FamilySymbol windowType = null;
            FamilySymbol rampType = null;//�µ�   

            IList<FamilySymbol> symbols = CollectorHelper.TCollector<FamilySymbol>(RevitDoc);
            foreach (FamilySymbol element in symbols)
            {
                if (element.FamilyName.Contains("����_��_default˫��") && element.Name.Contains("1800x2400"))
                {
                    doorType1800 = element;
                }

                if (element.FamilyName.Contains("����_��_default˫��") && element.Name.Contains("1500x2400"))
                {
                    doorType1500 = element;
                }
            }

            foreach (FamilySymbol element in symbols)
            {
                if (element.FamilyName.Contains("���Ͻ�") && element.Name.Contains("C-1"))
                {
                    windowType = element;
                    break;
                }
            }

            foreach (FamilySymbol element in symbols)
            {
                if (element.FamilyName.Contains("����_����_�µ�") && element.Name.Contains("�޵��"))
                {
                    rampType = element;
                    break;
                }
            }

            // ʹ�������ʹ����Ŵ��µ�
            foreach (var item in walls)
            {
                Line line = null;
                LocationCurve locationCurve = item.Location as LocationCurve;
                if (locationCurve != null)
                {
                    line = locationCurve.Curve as Line;
                }

                XYZ midPoint = (line.GetEndPoint(0) + line.GetEndPoint(1)) / 2;
                Level wallLevel = RevitDoc.GetElement(item.LevelId) as Level;

                double wallLength = double.Parse(item.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString());
                if (wallLength > 6700)
                {
                    if (line.Direction.X == 1 && line.Direction.Y == 0)
                    {
                        if (line.GetEndPoint(0).Y == 0)
                        {
                            // ��ǽ��һ��λ�ô���һ���� 
                            double doorwidth = doorType1800.get_Parameter(BuiltInParameter.DOOR_WIDTH).AsDouble();

                            XYZ doorPoint = new XYZ(line.GetEndPoint(1).X - doorwidth / 2 - 600 / 304.8, line.GetEndPoint(1).Y, line.GetEndPoint(1).Z);
                            FamilyInstance door = RevitDoc.Create.NewFamilyInstance(doorPoint, doorType1800, item, wallLevel, StructuralType.NonStructural);
                            if (door.CanRotate)
                            {
                                door.rotate();
                            }
                            instanceLists.Add(door);

                            XYZ point1 = new XYZ(line.GetEndPoint(1).X - 100 / 304.8, line.GetEndPoint(1).Y + 100 / 304.8, line.GetEndPoint(1).Z);
                            XYZ point2 = new XYZ(line.GetEndPoint(1).X - 3000 / 304.8, line.GetEndPoint(1).Y + 100 / 304.8, line.GetEndPoint(1).Z);
                            XYZ point3 = new XYZ(line.GetEndPoint(1).X - 3000 / 304.8, line.GetEndPoint(1).Y + 2000 / 304.8, line.GetEndPoint(1).Z);
                            XYZ point4 = new XYZ(line.GetEndPoint(1).X - 100 / 304.8, line.GetEndPoint(1).Y + 2000 / 304.8, line.GetEndPoint(1).Z);

                            CurveArray curveArray = new CurveArray();
                            curveArray.Append(Line.CreateBound(point1, point2));
                            curveArray.Append(Line.CreateBound(point2, point3));
                            curveArray.Append(Line.CreateBound(point3, point4));
                            curveArray.Append(Line.CreateBound(point4, point1));

                            FamilyInstance ramp = RevitDoc.Create.NewFamilyInstance(doorPoint, rampType, item, wallLevel, StructuralType.NonStructural);
                            ramp.LookupParameter("�Ŷ���").Set(doorwidth);
                            instanceLists.Add(ramp);

                            XYZ newPoint = new XYZ(line.GetEndPoint(1).X - doorwidth - 500 / 304.8, line.GetEndPoint(1).Y, line.GetEndPoint(1).Z);
                            Line windowLine = Line.CreateBound(line.GetEndPoint(0), newPoint);
                            List<XYZ> windowPoints = GetWindowLocationPoints(windowLine.StartPoint(), windowLine.Length * 304.8, 0);
                            foreach (var point in windowPoints)
                            {
                                FamilyInstance window = RevitDoc.Create.NewFamilyInstance(point, windowType, item, wallLevel, StructuralType.NonStructural);
                                window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(900 / 304.8);
                                instanceLists.Add(window);
                            }
                        }
                        else
                        {
                            if (HasMinimalDifference(line.GetEndPoint(0).Y, roomWidth / 304.8, 1))
                            {
                                // ��ǽ�ϴ�������� 
                                List<XYZ> windowPoints = GetWindowLocationPoints(line.StartPoint(), line.Length * 304.8, 0);
                                foreach (var point in windowPoints)
                                {
                                    FamilyInstance window = RevitDoc.Create.NewFamilyInstance(point, windowType, item, wallLevel, StructuralType.NonStructural);
                                    window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(900 / 304.8);
                                    instanceLists.Add(window);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (line.Direction.X == 1 && line.Direction.Y == 0)
                    {
                        if (line.GetEndPoint(0).Y == 0)
                        {
                            // ��ǽ������λ�ô���һ���� 
                            FamilyInstance door = RevitDoc.Create.NewFamilyInstance(midPoint, doorType1500, item, wallLevel, StructuralType.NonStructural);
                            if (door.CanRotate)
                            {
                                door.rotate();
                            }
                            instanceLists.Add(door);

                            double doorwidth = doorType1500.get_Parameter(BuiltInParameter.DOOR_WIDTH).AsDouble();
                            FamilyInstance ramp = RevitDoc.Create.NewFamilyInstance(midPoint, rampType, item, wallLevel, StructuralType.NonStructural);
                            ramp.LookupParameter("�Ŷ���").Set(doorwidth);
                            instanceLists.Add(ramp);
                        }
                        else
                        {
                            if (HasMinimalDifference(line.GetEndPoint(0).Y, roomWidth / 304.8, 1))
                            {
                                // ��ǽ������λ�ô���һ���� 
                                FamilyInstance window = RevitDoc.Create.NewFamilyInstance(midPoint, windowType, item, wallLevel, StructuralType.NonStructural);
                                window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(900 / 304.8);
                                instanceLists.Add(window);
                            }
                        }
                    }
                }
            }

            return instanceLists;
        }
        public List<XYZ> GetWindowLocationPoints(XYZ basePoint, double length, double width)
        {
            //���ڴ��ȷ�,����
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
        public bool HasMinimalDifference(double value1, double value2, int units)
        {
            long lValue1 = BitConverter.DoubleToInt64Bits(value1);
            long lValue2 = BitConverter.DoubleToInt64Bits(value2);

            // If the signs are different, return false except for +0 and -0.
            if ((lValue1 >> 63) != (lValue2 >> 63))
            {
                if (value1 == value2)
                    return true;

                return false;
            }

            long diff = Math.Abs(lValue1 - lValue2);

            if (diff <= (long)units)
                return true;

            return false;
        }
        public void GetSlabEdge(Document doc, Floor bottomFloor)//����ɢˮ
        {
            SlabEdgeType footSlab = null;
            IList<SlabEdgeType> edges = CollectorHelper.TCollector<SlabEdgeType>(doc);
            foreach (var item in edges)
            {
                if (item.Name.Contains("����_ɢˮ�޵��_1000"))
                {
                    footSlab = item;
                    break;
                }
            }

            Face normalFace = null;
            Options options = new Options();
            options.ComputeReferences = true;
            GeometryElement geometryElement = bottomFloor.get_Geometry(options);
            foreach (GeometryObject item in geometryElement)
            {
                if (item is Solid solid)
                {
                    List<Face> list = new List<Face>();
                    foreach (Face face in Getupfaces(solid))
                    {
                        list.Add(face);
                    }
                    double AreaMax = list.Max(t => t.Area);
                    normalFace = list.FirstOrDefault(p => p.Area == AreaMax);
                }
            }

            EdgeArrayArray eaa = normalFace.EdgeLoops;
            EdgeArray ea = eaa.get_Item(0);

            ReferenceArray refArr = new ReferenceArray();
            foreach (Edge item in ea)
            {
                refArr.Append(item.Reference);
            }

            SlabEdge newEdge = doc.Create.NewSlabEdge(footSlab, refArr);
            newEdge.get_Parameter(BuiltInParameter.SWEEP_BASE_OFFSET).Set(200 / 304.8);
            newEdge.get_Parameter(BuiltInParameter.SWEEP_BASE_VERT_OFFSET).Set(20 / 304.8);
        }
        public List<Face> Getupfaces(Solid solid)
        {
            var upfaces = new List<Face>();
            var faces = solid.Faces;
            foreach (Face face in faces)
            {
                var normal = face.ComputeNormal(new UV());
                if (normal.IsSameDirection(XYZ.BasisZ))
                {
                    upfaces.Add(face);
                }
            }
            return upfaces;
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
        public FloorType PoolFloorType(Document doc, string floorThick)
        {
            FloorType poolFloorType = null;

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(FloorType));
            IList<Element> levelTypeList = collector.ToElements();
            foreach (Element elem in collector)
            {
                FloorType floorType = elem as FloorType;
                if (floorType.Name.Contains("�ṹ_������¥��") && floorType.Name.Contains(floorThick))
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
            // ��ȡȫ�����
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
        public List<Dimension> CreatGridDemesion(Document doc, View activeView, List<Grid> grids)
        {
            List<Dimension> dimensions = new List<Dimension>();
            //�ߴ��ע��λ��
            XYZ locationPoint = new XYZ();
            Line gridLine = grids.FirstOrDefault().Curve as Line;
            if (gridLine != null)
            {
                XYZ newPoint = gridLine.StartPoint();
                locationPoint = new XYZ(newPoint.X + 400 / 304.8, newPoint.Y + 400 / 304.8, newPoint.Z);
            }

            //��ȡ��������
            DimensionType dimType = null;
            FilteredElementCollector elems = new FilteredElementCollector(doc);
            foreach (DimensionType dt in elems.OfClass(typeof(DimensionType)))
            {
                if (dt.Name.Contains("����ˮ") && dt.StyleType == DimensionStyleType.Linear)
                {
                    dimType = dt;
                    break;
                }
            }

            if (dimType != null)
            {
                if (grids.Count > 1)
                {
                    ReferenceArray referenceArray1 = new ReferenceArray();
                    ReferenceArray referenceArray2 = new ReferenceArray();
                    //������ѡ��������Ϊ���ջ�׼
                    List<Grid> lineGrid = new List<Grid>();
                    Line referenceLine = null;
                    double dis = double.MaxValue;
                    foreach (Grid g in grids)
                    {
                        double d = g.Curve.Distance(locationPoint);
                        Line line = g.Curve as Line;
                        if (line != null)
                        {
                            lineGrid.Add(g);
                            if (d < dis)
                            {
                                referenceLine = line;
                                dis = d;
                            }
                        }
                    }
                    //����ڲ�ߴ��ע������
                    foreach (Grid g in lineGrid)
                    {
                        Line line = g.Curve as Line;
                        if (line.Direction.IsAlmostEqualTo(referenceLine.Direction) || line.Direction.IsAlmostEqualTo(referenceLine.Direction.Multiply(-1)))
                        {
                            referenceArray1.Append(new Reference(g));
                        }
                    }
                    //��ȡ���ߴ��ע������
                    foreach (Reference refGrid in referenceArray1)
                    {
                        Grid g = doc.GetElement(refGrid) as Grid;
                        Line line = g.Curve as Line;
                        XYZ point1 = line.GetEndPoint(0);
                        XYZ point2 = line.GetEndPoint(1);
                        int i = 0;
                        foreach (Reference _refGrid in referenceArray1)
                        {
                            Grid _g = doc.GetElement(_refGrid) as Grid;
                            Line _line = _g.Curve as Line;
                            XYZ point = _line.GetEndPoint(0);
                            if (PointOnTheLeft(point1, point2, point))
                            {
                                i += 1;
                            }
                        }
                        if (i == 0 || i == referenceArray1.Size - 1)
                        {
                            referenceArray2.Append(new Reference(g));
                        }
                    }
                    //����ߴ��עλ��
                    XYZ lineDir = referenceLine.Direction.CrossProduct(new XYZ(0, 0, 1));
                    XYZ point_s = referenceLine.GetEndPoint(0);
                    XYZ point_e = referenceLine.GetEndPoint(1);
                    if (point_s.DistanceTo(locationPoint) > point_e.DistanceTo(locationPoint))
                    {
                        XYZ temPoint = point_s;
                        point_s = point_e;
                        point_e = temPoint;
                    }
                    XYZ offsetDir = point_e - point_s;
                    double lenght = dimType.get_Parameter(BuiltInParameter.TEXT_SIZE).AsDouble();
                    Line line_o = Line.CreateUnbound(locationPoint, lineDir);
                    Line line_i = Line.CreateUnbound(locationPoint + offsetDir.Normalize() * lenght * activeView.Scale * 1.9, lineDir);
                    //�����ߴ��ע
                    if (grids.Count > 2)
                    {
                        Dimension dm1 = doc.Create.NewDimension(activeView, line_o, referenceArray2, dimType);
                        Dimension dm2 = doc.Create.NewDimension(activeView, line_i, referenceArray1, dimType);
                        dimensions.Add(dm1);
                        dimensions.Add(dm2);
                    }
                    else
                    {
                        Dimension dm = doc.Create.NewDimension(activeView, line_o, referenceArray2, dimType);
                        dimensions.Add(dm);
                    }
                }
            }
            return dimensions;
        }
        public bool PointOnTheLeft(XYZ point1, XYZ point2, XYZ point)
        {
            double r = (point1.X - point2.X) / (point1.Y - point2.Y) * (point.Y - point2.Y) + point2.X;
            if (r > point.X)
            {
                return true;
            }
            return false;
        }
        public List<Wall> AddWallByCrossGrids(Document doc, UIApplication uiapp, List<Grid> wallGrids, Level topLevel,
            Level bottomLevel, double offset, bool isSegemention, bool isStructure)
        {
            List<Wall> res = new List<Wall>();

            WallType wallType = GetWallTypeByName(doc, "default_����ǽ");
            if (null == wallType)
            {
                return res;
            }

            foreach (Grid grid in wallGrids)
            {
                List<Curve> curves = Common.GetSegementsWithGrid(uiapp, grid, wallGrids);

                foreach (Curve curve in curves)
                {
                    List<Wall> newWall = CreateSegementionWall(doc, curve, topLevel, bottomLevel, wallType, isSegemention, isStructure, offset);
                    res.AddRange(newWall);
                }
            }

            return res;
        }
        protected List<Wall> CreateSegementionWall(Document doc, Curve curve, Level topLevel, Level bottomLevel, WallType wallType,
                                                  bool isSegemention, bool isStructural, double offset)
        {
            List<Wall> res = new List<Wall>();
            if (isSegemention)
            {
                double bottom = bottomLevel.Elevation;
                double top = topLevel.Elevation;
                List<Level> Levels = Common.GetSortLevels(doc, bottom, top);
                if (Levels.Count < 2)
                {
                    return res;
                }
                int i = 0;
                for (; i < Levels.Count - 1; ++i)
                {
                    Level curBottomLevel = Levels[i];
                    Level curTopLevel = Levels[i + 1];
                    Wall wall = CreateOneWall(doc, curve, curBottomLevel, curTopLevel, wallType, offset, isStructural);
                    wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(0);
                    wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(offset / 304.8);
                    res.Add(wall);
                }
            }
            else
            {
                Wall wall = CreateOneWall(doc, curve, bottomLevel, topLevel, wallType, offset, isStructural);
                wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(0);
                wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(offset / 304.8);
                res.Add(wall);
            }

            return res;
        }
        protected Wall CreateOneWall(Document doc, Curve curve, Level bottomLevel, Level topLevel, WallType wallType,
                                    double offset, bool isStructural)
        {
            double height = topLevel.Elevation - bottomLevel.Elevation;
            Wall wall = Wall.Create(doc, curve, wallType.Id, bottomLevel.Id, height, offset, false, isStructural);
            Parameter p = wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE);
            if (null != p)
            {
                p.Set(topLevel.Id);
            }
            wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(0);
            wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(0);
            return wall;
        }
        protected WallType GetWallTypeByName(Document doc, string walltypename)
        {
            WallType wallType = null;
            IList<WallType> wallTypes = CollectorHelper.TCollector<WallType>(doc);

            foreach (WallType item in wallTypes)
            {
                if (item.Name.Contains(walltypename))
                {
                    wallType = item;
                    break;
                }
            }

            return wallType;
        }
        public void HideGridBubble(List<Grid> grids, View view)
        {
            foreach (var item in grids)
            {
                item.HideBubbleInView(DatumEnds.End0, view);
            }
        }
        /// <summary>
        /// Get current length display unit type
        /// </summary>
        /// <param name="document">Revit's document</param>
        /// <returns>Current length display unit type</returns>
        private static DisplayUnitType GetLengthUnitType(Document document)
        {
            UnitType unittype = UnitType.UT_Length;
            Units projectUnit = document.GetUnits();
            try
            {
                Autodesk.Revit.DB.FormatOptions formatOption = projectUnit.GetFormatOptions(unittype);
                return formatOption.DisplayUnits;
            }
            catch (System.Exception /*e*/)
            {
                return DisplayUnitType.DUT_DECIMAL_FEET;
            }
        }

        /// <summary>
        /// Get all grid labels in current document
        /// </summary>
        /// <param name="document">Revit's document</param>
        /// <returns>ArrayList contains all grid labels in current document</returns>
        private static ArrayList GetAllLabelsOfGrids(Document document)
        {
            ArrayList labels = new ArrayList();
            FilteredElementIterator itor = new FilteredElementCollector(document).OfClass(typeof(Grid)).GetElementIterator();
            itor.Reset();
            for (; itor.MoveNext();)
            {
                Grid grid = itor.Current as Grid;
                if (null != grid)
                {
                    labels.Add(grid.Name);
                }
            }

            return labels;
        }
    }
}
