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
using System.Data.SQLite;
using System.Data;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class PumpGroup : IExternalCommand
    {
        public static PumpGroupForm mainfrm;
        public static List<PumpData> ShuangLun_IS_PumpData = new List<PumpData>();
        public static List<PumpData> Liancheng_S_PumpData = new List<PumpData>();
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                string sql = string.Format("SELECT * FROM {0}", "DataIS");
                ShuangLun_IS_PumpData = GetPumpData(sql);//双轮IS型泵数据获取
                sql = string.Format("SELECT * FROM {0}", "DataS");
                Liancheng_S_PumpData = GetPumpData(sql);//连成S型泵数据获取

                mainfrm = new PumpGroupForm(GetAllPipeSize(doc, "给排水_焊接钢管"), GetPipeType(doc, "给排水"), GetPipeSystemType(doc, "给排水")
                    ,ShuangLun_IS_PumpData,Liancheng_S_PumpData);
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();

            }
            catch (Exception)
            {
                throw;
            }
            return Result.Succeeded;
        }
        public List<PumpData> GetPumpData(string sql)
        {
            try
            {
                List<PumpData> list = new List<PumpData>();
                string dataFile = "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2018\\FFETOOLS\\Data\\GPSPumpData.db3";
                SQLiteConnection conn = new SQLiteConnection();
                Tuple<bool, DataSet, string> tuple = null;

                SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder
                {
                    DataSource = dataFile
                };
                conn.ConnectionString = conStr.ToString();
                conn.Open();


                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
                        DataSet dataSet = new DataSet();
                        dataAdapter.SelectCommand = cmd;
                        dataAdapter.Fill(dataSet);
                        cmd.Parameters.Clear();
                        dataAdapter.Dispose();
                        tuple = Tuple.Create(true, dataSet, string.Empty);
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                DataSet dataSetResult = tuple.Item2;
                if (dataSetResult != null)
                {
                    DataTable resultDate = dataSetResult.Tables[0];
                    foreach (DataRow dataRow in resultDate.Rows)
                    {
                        string baseInfo = dataRow["BaseParam"].ToString();
                        string[] sArray = baseInfo.Split(';');

                        list.Add(new PumpData()
                        {
                            Model = dataRow["SPEC"].ToString(),
                            Flow = dataRow["Volume"].ToString(),
                            Lift = dataRow["Head"].ToString(),
                            Power = dataRow["EnginPower"].ToString(),
                            Weight = dataRow["Weight"].ToString(),
                            OutletHeight = dataRow["Height"].ToString(),
                            OutletSize = dataRow["OutPipeDN"].ToString(),
                            InletSize = dataRow["InPipeDN"].ToString(),
                            BaseLength = sArray[0]
                        });
                    }
                }

                conn.Close();
                conn.Dispose();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<string> GetPipeSystemType(Document doc, string profession)
        {
            // 获取管道系统名称列表
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = collector.ToElements();
            List<string> pipesystemname = new List<string>();
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;
                if (ps.Name.Contains(profession))
                {
                    pipesystemname.Add(ps.Name.Replace("给排水_", "").Replace("管道系统", ""));
                }
            }
            return pipesystemname;
        }
        public static List<string> GetPipeType(Document doc, string profession)
        {
            // 获取管道类型列表
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            List<string> pipetypename = new List<string>();
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains(profession))
                {
                    pipetypename.Add(ps.Name.Replace("给排水_", ""));
                }
            }
            return pipetypename;
        }
        public static List<string> GetAllPipeSize(Document doc, string pipetype)
        {
            // 获取管道全部尺寸类型
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeSegment));
            IList<Element> pipesizes = collector.ToElements();
            List<string> pipesizename = new List<string>();
            ICollection<MEPSize> mepsize;
            foreach (Element e in pipesizes)
            {
                PipeSegment ps = e as PipeSegment;
                if (ps.Name.Contains(pipetype))
                {
                    mepsize = ps.GetSizes();
                    foreach (var item in mepsize)
                    {
                        pipesizename.Add("DN" + (item.NominalDiameter * 304.8).ToString());
                    }
                    break;
                }
            }
            return pipesizename;
        }
    }
    public class ExecuteEventPumpGroup : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;
                int clickNumber = PumpGroup.mainfrm.ClickNum;
                PumpSelectForm selectForm = new PumpSelectForm(PumpGroup.mainfrm);

                View view = uidoc.ActiveView;
                if (view is ViewPlan)
                {
                    if (clickNumber == 1)//主界面确定点击
                    {
                        CreatSingleSuctionPump(doc, sel);
                    }

                    if (clickNumber == 2)//主界面水泵选型点击
                    {                     
                        selectForm.PumpDataSource = PumpGroup.ShuangLun_IS_PumpData;
                        selectForm.ShowDialog();                     
                    }
                }
                else
                {
                    TaskDialog.Show("警告", "请在平面视图中进行操作");
                    PumpGroup.mainfrm.Show();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public void CreatSingleSuctionPump(Document doc, Selection sel)
        {

            XYZ pickpoint = sel.PickPoint("请选择插入点");

            FamilyInstance pump = null;
            FamilyInstance inLetPipeJoint = null;
            FamilyInstance inLetPipeValve = null;
            FamilyInstance outLetPipeJoint = null;
            FamilyInstance outLetPipeValve = null;
            FamilyInstance outLetPipeCheckValve = null;
            FamilyInstance pressuerMeter = null;
            FamilyInstance vacuumMeter = null;

            Pipe inLetPipe1 = null;
            Pipe inLetPipe2 = null;
            Pipe inLetPipe3 = null;
            Pipe outLetPipe1 = null;
            Pipe outLetPipe2 = null;
            Pipe outLetPipe3 = null;
            Pipe outLetPipe4 = null;
            Pipe outLetPipe41 = null;

            FamilyInstance reducer1 = null;
            FamilyInstance reducer2 = null;
            FamilyInstance elbow = null;

            PipingSystemType pipeSystem = null;
            PipingSystem newPipeSystem = null;

            Connector outConnector = null;
            Connector inConnector = null;

            XYZ inLetPoint1 = new XYZ();
            XYZ outLetPoint1 = new XYZ();
            XYZ outLetPoint2 = new XYZ();

            Line inletLine = null;
            Line outletLine = null;

            string inLetSize = null;
            string outLetSize = null;
            string pipetype = null;
            string pipeSystemType = null;

            TransactionGroup tg = new TransactionGroup(doc, "布置泵组");
            tg.Start();

            using (Transaction trans = new Transaction(doc, "载入水泵组族"))
            {
                trans.Start();

                PumpFamilyLoad(doc, "卧式单吸泵");
                EquipmentAccessoryFamilyLoad(doc, "挠性橡胶接头");
                ValveFamilyLoad(doc, "蝶形止回阀H77X-10");
                ValveFamilyLoad(doc, "微阻缓闭式止回阀HH44X-10");
                ValveFamilyLoad(doc, "电动蝶阀D97A1X-10");
                MeterFamilyLoad(doc, "压力表");
                MeterFamilyLoad(doc, "真空表");

                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "创建水泵"))
            {
                trans.Start();

                FamilySymbol pumpSymbol = null;
                if (PumpSymbolExist(doc, PumpGroup.mainfrm.PumpModelValue))
                {
                    pumpSymbol = PumpSymbol(doc, PumpGroup.mainfrm.PumpModelValue);
                    pumpSymbol.Activate();
                    ModifyPumpParameter(pumpSymbol);
                    pump = doc.Create.NewFamilyInstance(pickpoint, pumpSymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
                }
                else
                {
                    FamilySymbol newSymbol = PumpSymbol(doc).Duplicate(PumpGroup.mainfrm.PumpModelValue) as FamilySymbol;
                    newSymbol.Activate();
                    ModifyPumpParameter(newSymbol);
                    pump = doc.Create.NewFamilyInstance(pickpoint, newSymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
                }
                pump.LookupParameter("偏移").Set(0);

                trans.Commit();
            }

            ConnectorSet set = pump.MEPModel.ConnectorManager.Connectors;
            List<Connector> conList = new List<Connector>();

            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            if (conList.ElementAt(0).Origin.Z > conList.ElementAt(1).Origin.Z)
            {
                outConnector = conList.ElementAt(0);
                inConnector = conList.ElementAt(1);
            }
            else
            {
                outConnector = conList.ElementAt(1);
                inConnector = conList.ElementAt(0);
            }

            inLetPoint1 = new XYZ(inConnector.Origin.X - 1700 / 304.8, inConnector.Origin.Y, inConnector.Origin.Z);

            outLetPoint1 = new XYZ(outConnector.Origin.X, outConnector.Origin.Y, outConnector.Origin.Z + 1500 / 304.8);
            outLetPoint2 = new XYZ(outLetPoint1.X - 1700 / 304.8, outLetPoint1.Y, outLetPoint1.Z);

            inletLine = Line.CreateBound(inConnector.Origin, inLetPoint1);
            outletLine = Line.CreateBound(outLetPoint1, outLetPoint2);

            string pumpInLetSize = pump.Symbol.LookupParameter("进口直径").AsValueString();
            string pumpOutLetSize = pump.Symbol.LookupParameter("出口直径").AsValueString();

            inLetSize = PumpGroup.mainfrm.InLetPipeSize.SelectedItem.ToString().Replace("DN", "");
            outLetSize = PumpGroup.mainfrm.OutLetPipeSize.SelectedItem.ToString().Replace("DN", "");
            pipetype = PumpGroup.mainfrm.PipeType.SelectedItem.ToString();
            pipeSystemType = PumpGroup.mainfrm.PipeSystemType.SelectedItem.ToString();

            using (Transaction trans = new Transaction(doc, "载入泵组附件"))
            {
                trans.Start();

                FamilySymbol inletJointSymbol = PipeAccessorySymbol(doc, inLetSize, "挠性橡胶接头");
                inletJointSymbol.Activate();
                inLetPipeJoint = doc.Create.NewFamilyInstance(ThreeEqualPoint(inletLine, 1), inletJointSymbol, StructuralType.NonStructural);

                FamilySymbol inLetValveSymbol = PipeAccessorySymbol(doc, inLetSize, "蝶阀D37A1X");
                inLetValveSymbol.Activate();
                inLetPipeValve = doc.Create.NewFamilyInstance(ThreeEqualPoint(inletLine, 2), inLetValveSymbol, StructuralType.NonStructural);

                FamilySymbol outletJointSymbol = PipeAccessorySymbol(doc, outLetSize, "挠性橡胶接头");
                outletJointSymbol.Activate();
                outLetPipeJoint = doc.Create.NewFamilyInstance(ThreeEqualPoint(outletLine, 1), outletJointSymbol, StructuralType.NonStructural);

                FamilySymbol outLetValveSymbol = PipeAccessorySymbol(doc, outLetSize, "蝶阀D37A1X");
                outLetValveSymbol.Activate();
                outLetPipeValve = doc.Create.NewFamilyInstance(ThreeEqualPoint(outletLine, 2), outLetValveSymbol, StructuralType.NonStructural);

                FamilySymbol outLetCheckValveSymbol = PipeAccessorySymbol(doc, outLetSize, "蝶形止回阀H77X-10");
                outLetCheckValveSymbol.Activate();
                outLetPipeCheckValve = doc.Create.NewFamilyInstance(MiddlePoint(outletLine), outLetCheckValveSymbol, StructuralType.NonStructural);

                XYZ point1 = MiddlePoint(outletLine);
                XYZ point2 = new XYZ(point1.X, point1.Y, point1.Z + 1);
                Line axis = Line.CreateBound(point1, point2);
                ElementTransformUtils.RotateElement(doc, outLetPipeCheckValve.Id, axis, -Math.PI);

                outLetPipeJoint.LookupParameter("使用注释比例").Set(1);
                outLetPipeValve.LookupParameter("使用注释比例").Set(1);
                outLetPipeCheckValve.LookupParameter("使用注释比例").Set(1);

                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "布置泵组"))
            {
                trans.Start();

                pipeSystem = GetPipeSystemType(doc, "给排水", pipeSystemType);
                PipeType pipeType = GetPipeType(doc, "给排水", pipetype);
                //Level pipeLevel = GetPipeLevel(doc, "0.000");
                Level pipeLevel = doc.ActiveView.GenLevel;

                Connector jointConnector1 = GetNearConnector(inLetPipeJoint, inConnector.Origin);//进水管橡胶接头

                inLetPipe1 = Pipe.Create(doc, pipeSystem.Id, pipeType.Id, pipeLevel.Id, inConnector.Origin, jointConnector1.Origin);
                ChangePipeSize(inLetPipe1, pumpInLetSize);
                Connector inLetPipe1Con = GetPointConnector(inLetPipe1, inConnector.Origin);
                inConnector.ConnectTo(inLetPipe1Con);
                ChangePipeSize(inLetPipe1, inLetSize);//泵进口第一段管道

                Connector inletPipe1Connector2 = GetFarConnector(inLetPipe1, inConnector.Origin);
                inletPipe1Connector2.ConnectTo(jointConnector1);

                Connector jointConnector2 = GetFarConnector(inLetPipeJoint, inConnector.Origin);
                Connector inLetPipeValveConnector1 = GetNearConnector(inLetPipeValve, inConnector.Origin);

                inLetPipe2 = Pipe.Create(doc, pipeSystem.Id, pipeType.Id, pipeLevel.Id, jointConnector2.Origin, inLetPipeValveConnector1.Origin);
                ChangePipeSize(inLetPipe2, inLetSize);

                Connector inLetPipe2Connector1 = GetNearConnector(inLetPipe2, inConnector.Origin);
                jointConnector2.ConnectTo(inLetPipe2Connector1);

                Connector inLetPipe2Connector2 = GetFarConnector(inLetPipe2, inConnector.Origin);
                inLetPipe2Connector2.ConnectTo(inLetPipeValveConnector1);

                Connector inLetPipeValveConnector2 = GetFarConnector(inLetPipeValve, inConnector.Origin);

                inLetPipe3 = Pipe.Create(doc, pipeSystem.Id, pipeType.Id, pipeLevel.Id, inLetPipeValveConnector2.Origin, inLetPoint1);
                ChangePipeSize(inLetPipe3, inLetSize);

                Connector inLetPipe3Connector1 = GetNearConnector(inLetPipe3, inConnector.Origin);
                inLetPipeValveConnector2.ConnectTo(inLetPipe3Connector1);

                outLetPipe1 = Pipe.Create(doc, pipeSystem.Id, pipeType.Id, pipeLevel.Id, outConnector.Origin, outLetPoint1);
                ChangePipeSize(outLetPipe1, pumpOutLetSize);
                Connector outLetPipeCon1 = GetPointConnector(outLetPipe1, outConnector.Origin);
                outConnector.ConnectTo(outLetPipeCon1);
                ChangePipeSize(outLetPipe1, outLetSize);

                Connector jointConnector3 = GetNearConnector(outLetPipeJoint, outConnector.Origin);

                outLetPipe2 = Pipe.Create(doc, pipeSystem.Id, pipeType.Id, pipeLevel.Id, outLetPoint1, jointConnector3.Origin);
                ChangePipeSize(outLetPipe2, outLetSize);
                elbow = ConnectTwoPipesWithElbow(doc, outLetPipe1, outLetPipe2);

                Connector outLetPipe2Connector2 = GetFarConnector(outLetPipe2, outConnector.Origin);
                jointConnector3.ConnectTo(outLetPipe2Connector2);

                Connector jointConnector4 = GetFarConnector(outLetPipeJoint, outConnector.Origin);
                Connector outLetPipeValveConnector1 = GetNearConnector(outLetPipeValve, outConnector.Origin);

                Connector checkValveConnector1 = GetNearConnector(outLetPipeCheckValve, outConnector.Origin);

                outLetPipe3 = Pipe.Create(doc, pipeSystem.Id, pipeType.Id, pipeLevel.Id, jointConnector4.Origin, checkValveConnector1.Origin);
                ChangePipeSize(outLetPipe3, outLetSize);

                Connector outLetPipe3Connector1 = GetNearConnector(outLetPipe3, outConnector.Origin);
                jointConnector4.ConnectTo(outLetPipe3Connector1);

                Connector outLetPipe3Connector2 = GetFarConnector(outLetPipe3, outConnector.Origin);
                outLetPipe3Connector2.ConnectTo(checkValveConnector1);

                Connector checkValveConnector2 = GetFarConnector(outLetPipeCheckValve, outConnector.Origin);
                outLetPipe41 = Pipe.Create(doc, pipeSystem.Id, pipeType.Id, pipeLevel.Id, checkValveConnector2.Origin, outLetPipeValveConnector1.Origin);
                ChangePipeSize(outLetPipe41, outLetSize);

                Connector outLetPipe41Connector1 = GetNearConnector(outLetPipe41, outConnector.Origin);
                checkValveConnector2.ConnectTo(outLetPipe41Connector1);

                Connector outLetPipe41Connector2 = GetFarConnector(outLetPipe41, outConnector.Origin);
                outLetPipeValveConnector1.ConnectTo(outLetPipe41Connector2);

                Connector outLetPipeValveConnector2 = GetFarConnector(outLetPipeValve, outConnector.Origin);
                outLetPipe4 = Pipe.Create(doc, pipeSystem.Id, pipeType.Id, pipeLevel.Id, outLetPoint2, outLetPipeValveConnector2.Origin);
                ChangePipeSize(outLetPipe4, outLetSize);

                Connector outLetPipe4Connector1 = GetNearConnector(outLetPipe4, outConnector.Origin);
                outLetPipeValveConnector2.ConnectTo(outLetPipe4Connector1);

                doc.Delete(inLetPipe1.MEPSystem.Id);
                doc.Delete(inLetPipe2.MEPSystem.Id);
                doc.Delete(inLetPipe3.MEPSystem.Id);
                doc.Delete(outLetPipe1.MEPSystem.Id);
                doc.Delete(outLetPipe2.MEPSystem.Id);
                doc.Delete(outLetPipe3.MEPSystem.Id);
                doc.Delete(outLetPipe4.MEPSystem.Id);
                doc.Delete(outLetPipe41.MEPSystem.Id);

                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "修改异径及出口高度"))
            {
                trans.Start();
                reducer1 = GetLinkReducer(pump, 1);//泵出口异径
                reducer2 = GetLinkReducer(pump, 2);//泵进口异径                             

                if (reducer2 != null)
                {
                    Parameter height = reducer2.get_Parameter(BuiltInParameter.RBS_FAMILY_CONTENT_OFFSET_HEIGHT);
                    double outdiameter1 = reducer2.LookupParameter("管件外径 1").AsDouble();
                    double outdiameter2 = reducer2.LookupParameter("管件外径 2").AsDouble();
                    double offset = height.AsDouble() - (outdiameter2 - outdiameter1) / 2;
                    height.Set(offset);
                }

                double length = outLetPipe1.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                double pipeHeight = outLetPipe4.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).AsDouble() - length;
                double offSet = (Math.Ceiling((pipeHeight * 304.8 / 100)) * 100) / 304.8;
                outLetPipe4.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).Set(offSet);

                XYZ pressureMeterPoint = MiddlePoint(outLetPipe2.LocationLine());
                FamilySymbol pressureMeterSymbol = PipeMeterSymbol(doc, "压力表");
                pressureMeterSymbol.Activate();
                pressuerMeter = doc.Create.NewFamilyInstance(pressureMeterPoint, pressureMeterSymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
                double outpipeSize = outLetPipe2.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                pressuerMeter.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(offSet + outpipeSize / 2);
                XYZ point1 = pressureMeterPoint;
                XYZ point2 = new XYZ(pressureMeterPoint.X, pressureMeterPoint.Y, pickpoint.Z + 1);
                Line axis1 = Line.CreateBound(point1, point2);
                ElementTransformUtils.RotateElement(doc, pressuerMeter.Id, axis1, -Math.PI);

                XYZ vacuumMeterPoint = MiddlePoint(GetReducerLine(reducer2));
                FamilySymbol vacuumMeterSymbol = PipeMeterSymbol(doc, "真空表");
                vacuumMeterSymbol.Activate();
                vacuumMeter = doc.Create.NewFamilyInstance(vacuumMeterPoint, vacuumMeterSymbol, doc.ActiveView.GenLevel, StructuralType.NonStructural);
                double inpipeSize = inLetPipe1.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                vacuumMeter.LookupParameter("偏移").Set(inLetPipe1.LookupParameter("偏移").AsDouble() + inpipeSize / 2);
                XYZ point3 = vacuumMeterPoint;
                XYZ point4 = new XYZ(vacuumMeterPoint.X, vacuumMeterPoint.Y, vacuumMeterPoint.Z + 1);
                Line axis2 = Line.CreateBound(point3, point4);
                ElementTransformUtils.RotateElement(doc, vacuumMeter.Id, axis2, -Math.PI);

                if (pipeSystemType.Contains("循环给水"))
                {
                    newPipeSystem = PipingSystem.Create(doc, pipeSystem.Id, "XJ 1");

                }
                if (pipeSystemType.Contains("循环回水"))
                {
                    newPipeSystem = PipingSystem.Create(doc, pipeSystem.Id, "XH 1");
                }
                if (pipeSystemType.Contains("消防"))
                {
                    newPipeSystem = PipingSystem.Create(doc, pipeSystem.Id, "XF 1");
                }
                if (pipeSystemType.Contains("生活"))
                {
                    newPipeSystem = PipingSystem.Create(doc, pipeSystem.Id, "J 1");
                }
                if (pipeSystemType.Contains("水源"))
                {
                    newPipeSystem = PipingSystem.Create(doc, pipeSystem.Id, "YJ 1");
                }
                if (pipeSystemType.Contains("中水"))
                {
                    newPipeSystem = PipingSystem.Create(doc, pipeSystem.Id, "ZJ 1");
                }

                newPipeSystem.Add(pump.MEPModel.ConnectorManager.Connectors);

                List<ElementId> hideElements = new List<ElementId>();
                hideElements.Add(inLetPipeJoint.Id);
                hideElements.Add(inLetPipeValve.Id);
                hideElements.Add(inLetPipe1.Id);
                hideElements.Add(inLetPipe2.Id);
                hideElements.Add(inLetPipe3.Id);
                doc.ActiveView.HideElements(hideElements);

                trans.Commit();
            }

            using (Transaction trans = new Transaction(doc, "水泵成组"))
            {
                trans.Start();

                List<ElementId> elementsToGroup = new List<ElementId>();
                elementsToGroup.Add(pump.Id);
                elementsToGroup.Add(inLetPipeJoint.Id);
                elementsToGroup.Add(inLetPipeValve.Id);
                elementsToGroup.Add(outLetPipeJoint.Id);
                elementsToGroup.Add(outLetPipeValve.Id);
                elementsToGroup.Add(outLetPipeCheckValve.Id);
                elementsToGroup.Add(inLetPipe1.Id);
                elementsToGroup.Add(inLetPipe2.Id);
                elementsToGroup.Add(inLetPipe3.Id);
                elementsToGroup.Add(outLetPipe1.Id);
                elementsToGroup.Add(outLetPipe2.Id);
                elementsToGroup.Add(outLetPipe3.Id);
                elementsToGroup.Add(outLetPipe4.Id);
                elementsToGroup.Add(outLetPipe41.Id);
                elementsToGroup.Add(pressuerMeter.Id);
                elementsToGroup.Add(vacuumMeter.Id);

                if (reducer1 != null)
                {
                    elementsToGroup.Add(reducer1.Id);
                }
                if (reducer2 != null)
                {
                    elementsToGroup.Add(reducer2.Id);
                }
                elementsToGroup.Add(elbow.Id);
                Autodesk.Revit.DB.Group group = doc.Create.NewGroup(elementsToGroup);
                group.GroupType.Name = pipeSystemType + "水泵组";

                XYZ point1 = pickpoint;
                XYZ point2 = new XYZ(pickpoint.X, pickpoint.Y, pickpoint.Z + 1);
                Line axis = Line.CreateBound(point1, point2);
                ElementTransformUtils.RotateElement(doc, group.Id, axis, -Math.PI / 2);

                trans.Commit();
            }

            tg.Assimilate();

            PumpGroup.mainfrm.Show();
        }

        public string GetName()
        {
            return "布置泵组";
        }
        public void ModifyPumpParameter(FamilySymbol pump)
        {
            pump.LookupParameter("进口直径").SetValueString(PumpGroup.mainfrm.InletDiameter.SelectedItem.ToString().Replace("DN", ""));
            pump.LookupParameter("出口直径").SetValueString(PumpGroup.mainfrm.OutletDiameter.SelectedItem.ToString().Replace("DN", ""));
            pump.LookupParameter("进口高度").SetValueString(PumpGroup.mainfrm.InletHeightValue.ToString());
            pump.LookupParameter("出口高度").SetValueString(PumpGroup.mainfrm.OutletHeightValue.ToString());
            pump.LookupParameter("基础孔宽").SetValueString(PumpGroup.mainfrm.HoleSizeValue.ToString());
            pump.LookupParameter("孔间距L1").SetValueString(PumpGroup.mainfrm.HoleLengthValue.ToString());
            pump.LookupParameter("孔间距宽B1").SetValueString(PumpGroup.mainfrm.HoleWidthValue.ToString());
            pump.LookupParameter("电机基础边A1").SetValueString((PumpGroup.mainfrm.BaseLengthValue / 2 - PumpGroup.mainfrm.HoleLengthValue / 2).ToString());
            pump.LookupParameter("泵基础边A2").SetValueString((PumpGroup.mainfrm.BaseLengthValue / 2 - PumpGroup.mainfrm.HoleLengthValue / 2).ToString());
            pump.LookupParameter("基础边距孔B1").SetValueString((PumpGroup.mainfrm.BaseWidthValue / 2 - PumpGroup.mainfrm.HoleWidthValue / 2).ToString());
        }
        public Line GetReducerLine(FamilyInstance instance)
        {
            ConnectorManager manager = instance.MEPModel.ConnectorManager;
            ConnectorSet set = manager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }
            Connector con1 = conList.ElementAt(0);
            Connector con2 = conList.ElementAt(1);

            Line line = Line.CreateBound(con1.Origin, con2.Origin);
            return line;
        }

        public Connector GetNearConnector(FamilyInstance instance, XYZ point)
        {
            ConnectorManager manager = instance.MEPModel.ConnectorManager;
            ConnectorSet set = manager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            Connector con = null;
            Connector con1 = conList.ElementAt(0);
            Connector con2 = conList.ElementAt(1);
            if (con1.Origin.DistanceTo(point) > con2.Origin.DistanceTo(point))
            {
                con = con2;
            }
            else
            {
                con = con1;
            }
            return con;
        }
        public Connector GetNearConnector(Pipe pipe, XYZ point)
        {
            ConnectorManager manager = pipe.ConnectorManager;
            ConnectorSet set = manager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            Connector con = null;
            Connector con1 = conList.ElementAt(0);
            Connector con2 = conList.ElementAt(1);
            if (con1.Origin.DistanceTo(point) > con2.Origin.DistanceTo(point))
            {
                con = con2;
            }
            else
            {
                con = con1;
            }
            return con;
        }
        public Connector GetFarConnector(FamilyInstance instance, XYZ point)
        {
            ConnectorManager manager = instance.MEPModel.ConnectorManager;
            ConnectorSet set = manager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            Connector con = null;
            Connector con1 = conList.ElementAt(0);
            Connector con2 = conList.ElementAt(1);
            if (con1.Origin.DistanceTo(point) > con2.Origin.DistanceTo(point))
            {
                con = con1;
            }
            else
            {
                con = con2;
            }
            return con;
        }
        public Connector GetFarConnector(Pipe pipe, XYZ point)
        {
            ConnectorManager manager = pipe.ConnectorManager;
            ConnectorSet set = manager.Connectors;
            List<Connector> conList = new List<Connector>();
            foreach (Connector item in set)
            {
                conList.Add(item);
            }

            Connector con = null;
            Connector con1 = conList.ElementAt(0);
            Connector con2 = conList.ElementAt(1);
            if (con1.Origin.DistanceTo(point) > con2.Origin.DistanceTo(point))
            {
                con = con1;
            }
            else
            {
                con = con2;
            }
            return con;
        }
        public FamilyInstance GetLinkReducer(FamilyInstance instance, int i)
        {
            ConnectorSet csi = instance.MEPModel.ConnectorManager.Connectors;
            FamilyInstance reducer = null;

            Connector con1 = null;
            Connector con2 = null;
            foreach (Connector item in csi)
            {
                if (item.Id == 1)
                {
                    con1 = item;
                }
                if (item.Id == 2)
                {
                    con2 = item;
                }
            }

            if (i == 1)
            {
                ConnectorSet connectorSet1 = con1.AllRefs;
                ConnectorSetIterator csiChild = connectorSet1.ForwardIterator();
                while (csiChild.MoveNext())
                {
                    Connector connected = csiChild.Current as Connector;
                    if (null != connected)
                    {
                        // look for physical connections 
                        if (connected.ConnectorType == ConnectorType.End || connected.ConnectorType == ConnectorType.Curve || connected.ConnectorType == ConnectorType.Physical)
                        {
                            //判断是不是管件
                            if (connected.Owner != null)
                            {
                                //TaskDialog.Show("管道所连接的连接件名称是：", connected.Owner.Name);
                                reducer = connected.Owner as FamilyInstance;
                            }
                        }
                    }
                }

            }
            if (i == 2)
            {
                ConnectorSet connectorSet2 = con2.AllRefs;
                ConnectorSetIterator csiChild = connectorSet2.ForwardIterator();
                while (csiChild.MoveNext())
                {
                    Connector connected = csiChild.Current as Connector;
                    if (null != connected)
                    {
                        // look for physical connections 
                        if (connected.ConnectorType == ConnectorType.End || connected.ConnectorType == ConnectorType.Curve || connected.ConnectorType == ConnectorType.Physical)
                        {
                            //判断是不是管件
                            if (connected.Owner != null)
                            {
                                //TaskDialog.Show("管道所连接的连接件名称是：", connected.Owner.Name);
                                reducer = connected.Owner as FamilyInstance;
                            }
                        }
                    }
                }
            }

            return reducer;
        }
        public Connector GetLinkConnector(Pipe pipe)
        {
            ConnectorSetIterator csi = pipe.ConnectorManager.Connectors.ForwardIterator();
            Connector con = null;
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                if (conn.IsConnected == true)//是否有连接
                {
                    ConnectorSet connectorSet = conn.AllRefs;//找到所有连接器连接的连接器【这句很重要】
                    ConnectorSetIterator csiChild = connectorSet.ForwardIterator();
                    while (csiChild.MoveNext())
                    {
                        Connector connected = csiChild.Current as Connector;
                        if (null != connected && connected.Owner.UniqueId != conn.Owner.UniqueId)
                        {
                            // look for physical connections 
                            if (connected.ConnectorType == ConnectorType.End || connected.ConnectorType == ConnectorType.Curve || connected.ConnectorType == ConnectorType.Physical)
                            {
                                //判断是不是管件
                                if (connected.Owner is FamilyInstance)
                                {
                                    con = connected;
                                }
                            }
                        }
                    }
                }
            }
            return con;
        }
        public Connector GetPointConnector(Pipe pipe, XYZ point)
        {
            Connector con = null;
            ConnectorSet set = pipe.ConnectorManager.Connectors;
            foreach (Connector item in set)
            {
                if (item.Origin.IsAlmostEqualTo(point))
                {
                    con = item;
                    break;
                }
            }
            return con;
        }
        public XYZ ThreeEqualPoint(Line line, int num)
        {
            XYZ point = new XYZ();

            XYZ startPoint = line.StartPoint();
            XYZ endPoint = line.EndPoint();

            XYZ point1 = new XYZ((2 * startPoint.X + endPoint.X) / 3, (2 * startPoint.Y + endPoint.Y) / 3, (2 * startPoint.Z + endPoint.Z) / 3);
            XYZ point2 = new XYZ((startPoint.X + 2 * endPoint.X) / 3, (startPoint.Y + 2 * endPoint.Y) / 3, (startPoint.Z + 2 * endPoint.Z) / 3);

            if (num == 1)
            {
                point = point1;
            }
            if (num == 2)
            {
                point = point2;
            }
            return point;
        }
        public XYZ MiddlePoint(Line line)
        {
            XYZ startPoint = line.StartPoint();
            XYZ endPoint = line.EndPoint();
            XYZ point = new XYZ((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2, (startPoint.Z + endPoint.Z) / 2);
            return point;
        }

        public FamilySymbol PipeAccessorySymbol(Document doc, string dn, string accessoryName)
        {
            FilteredElementCollector valveCollector = new FilteredElementCollector(doc);
            valveCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessory);
            List<FamilySymbol> valveSymbolList = new List<FamilySymbol>();
            FamilySymbol valve = null;

            IList<Element> pumps = valveCollector.ToElements();
            foreach (FamilySymbol item in pumps)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains(accessoryName))
                {
                    valveSymbolList.Add(item);
                }
            }
            foreach (FamilySymbol item in valveSymbolList)
            {
                if (item.Name.Contains(dn))
                {
                    valve = item;
                    break;
                }
            }
            return valve;
        }

        public FamilySymbol PumpSymbol(Document doc, string symbolName)
        {
            FamilySymbol symbol = null;
            FilteredElementCollector pumpCollector = new FilteredElementCollector(doc);
            pumpCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            List<FamilySymbol> pumpSymbolList = new List<FamilySymbol>();

            IList<Element> pumps = pumpCollector.ToElements();
            foreach (FamilySymbol item in pumps)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains("卧式单吸泵"))
                {
                    pumpSymbolList.Add(item);
                }
            }
            foreach (var item in pumpSymbolList)
            {
                if (item.Name.Contains(symbolName))
                {
                    symbol = item;
                    break;
                }
            }
            return symbol;
        }
        public FamilySymbol PumpSymbol(Document doc)
        {
            FilteredElementCollector pumpCollector = new FilteredElementCollector(doc);
            pumpCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            List<FamilySymbol> pumpSymbolList = new List<FamilySymbol>();

            IList<Element> pumps = pumpCollector.ToElements();
            foreach (FamilySymbol item in pumps)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains("卧式单吸泵"))
                {
                    pumpSymbolList.Add(item);
                }
            }
            return pumpSymbolList.FirstOrDefault();
        }
        public bool PumpSymbolExist(Document doc, string symbolName)
        {
            bool exist = false;
            FilteredElementCollector pumpCollector = new FilteredElementCollector(doc);
            pumpCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            List<FamilySymbol> pumpSymbolList = new List<FamilySymbol>();

            IList<Element> pumps = pumpCollector.ToElements();
            foreach (FamilySymbol item in pumps)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains("卧式单吸泵"))
                {
                    pumpSymbolList.Add(item);
                }
            }
            foreach (var item in pumpSymbolList)
            {
                if (item.Name.Contains(symbolName))
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }
        public FamilySymbol PipeMeterSymbol(Document doc, string meterName)
        {
            FilteredElementCollector meterCollector = new FilteredElementCollector(doc);
            meterCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessory);
            List<FamilySymbol> meterSymbolList = new List<FamilySymbol>();
            FamilySymbol meter = null;

            IList<Element> meters = meterCollector.ToElements();
            foreach (FamilySymbol item in meters)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains(meterName))
                {
                    meterSymbolList.Add(item);
                }
            }
            foreach (FamilySymbol item in meterSymbolList)
            {
                if (item.Name.Contains(meterName))
                {
                    meter = item;
                    break;
                }
            }
            return meter;
        }
        public Level GetPipeLevel(Document doc, string Levelname)
        {
            // 获取标高
            Level newlevel = null;
            var levelFilter = new ElementClassFilter(typeof(Level));
            FilteredElementCollector levels = new FilteredElementCollector(doc);
            levels = levels.WherePasses(levelFilter);
            foreach (Level level in levels)
            {
                if (level.Name.Contains(Levelname))
                {
                    newlevel = level;
                    break;
                }
            }
            return newlevel;
        }
        public FamilyInstance ConnectTwoPipesWithElbow(Document doc, Pipe pipe1, Pipe pipe2)
        {
            // 创建弯头
            MEPCurve pipe1curve = pipe1 as MEPCurve;
            MEPCurve pipe2curve = pipe2 as MEPCurve;

            double minDistance = double.MaxValue;
            Connector connector1, connector2;
            connector1 = connector2 = null;
            FamilyInstance elbow = null;

            foreach (Connector con1 in pipe1curve.ConnectorManager.Connectors)
            {
                foreach (Connector con2 in pipe2curve.ConnectorManager.Connectors)
                {
                    var dis = con1.Origin.DistanceTo(con2.Origin);
                    if (dis < minDistance)
                    {
                        minDistance = dis;
                        connector1 = con1;
                        connector2 = con2;
                    }
                }
            }
            if (connector1 != null && connector2 != null)
            {
                elbow = doc.Create.NewElbowFitting(connector1, connector2);
            }
            return elbow;
        }
        public void ChangePipeSize(Pipe pipe, string dn)
        {
            //改变管道尺寸
            Parameter diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            diameter.SetValueString(dn);
        }
        public PipeType GetPipeType(Document doc, string profession, string pipetype)
        {
            // 获取管道类型
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipeType));
            IList<Element> pipetypes = collector.ToElements();
            PipeType pt = null;
            foreach (Element e in pipetypes)
            {
                PipeType ps = e as PipeType;
                if (ps.Name.Contains(profession) && ps.Name.Contains(pipetype))
                {
                    pt = ps;
                    break;
                }
            }
            return pt;
        }
        public PipingSystemType GetPipeSystemType(Document doc, string profession, string pipesystemtype)
        {
            // 获取管道系统
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(PipingSystemType));
            IList<Element> pipesystems = collector.ToElements();
            PipingSystemType pipesys = null;
            foreach (Element e in pipesystems)
            {
                PipingSystemType ps = e as PipingSystemType;
                if (ps.Name.Contains(profession) && ps.Name.Contains(pipesystemtype))
                {
                    pipesys = ps;
                    break;
                }
            }
            return pipesys;
        }

        public bool LocationRotate(Element element)
        {
            bool rotated = false;
            LocationPoint location = element.Location as LocationPoint;
            if (null != location)
            {
                XYZ aa = location.Point;
                XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                Line axis = Line.CreateBound(aa, cc);
                rotated = location.Rotate(axis, Math.PI / 2);
            }
            return rotated;
        }

        public void PumpFamilyLoad(Document doc, string categoryName)
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
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_水泵_" + categoryName + ".rfa");
            }
        }
        public void EquipmentAccessoryFamilyLoad(Document doc, string categoryName)
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
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_给水设备附件_" + categoryName + ".rfa");
            }
        }
        public void ValveFamilyLoad(Document doc, string categoryName)
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
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_阀门_" + categoryName + ".rfa");
            }

        }
        public void MeterFamilyLoad(Document doc, string categoryName)
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
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_仪表_" + categoryName + ".rfa");
            }

        }
    }
}
