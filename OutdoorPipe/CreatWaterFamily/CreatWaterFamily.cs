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
    public class CreatWaterFamily : IExternalCommand
    {
        public static CreatWaterFamilyForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new CreatWaterFamilyForm();
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
    public class ExecuteCreatWaterFamily : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                FamilySymbol symbol = null;
                GroupType groupType = null;
                int index = CreatWaterFamily.mainfrm.index;

                if (index == 58)
                {
                    using (Transaction trans = new Transaction(doc, "布置给排水组"))
                    {
                        trans.Start();
                        groupType = GetGroupTypeMethod(doc);
                        trans.Commit();
                    }
                    uidoc.PostRequestForElementTypePlacement(groupType);
                }
                else
                {
                    using (Transaction trans = new Transaction(doc, "布置给排水族"))
                    {
                        trans.Start();
                        symbol = CreatWaterFamilyMethod(doc);
                        symbol.Activate();
                        trans.Commit();
                    }
                    uidoc.PostRequestForElementTypePlacement(symbol);
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
        }
        public string GetName()
        {
            return "布置给排水族";
        }
        public FamilySymbol CreatWaterFamilyMethod(Document doc)
        {
            int index = CreatWaterFamily.mainfrm.index;
            string familyName = CreatWaterFamily.mainfrm.FamilyNameText;
            FamilySymbol familySymbol = null;
            switch (index)
            {
                case 1:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 2:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 3:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 4:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 5:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 6:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 7:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 8:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 9:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 10:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 11:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 12:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 13:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 14:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 15:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 16:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 17:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 18:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 19:
                    FamilyLoad(doc, "阀门", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "阀门", familyName);
                    break;
                case 20:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 21:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 22:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 23:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 24:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 25:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 26:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 27:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 28:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 29:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 30:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 31:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 111:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 112:
                    FamilyLoad(doc, "仪表", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "仪表", familyName);
                    break;
                case 32:
                    FamilyLoad(doc, "给水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "给水设备附件", familyName);
                    break;
                case 33:
                    FamilyLoad(doc, "给水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "给水设备附件", familyName);
                    break;
                case 34:
                    FamilyLoad(doc, "给水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "给水设备附件", familyName);
                    break;
                case 35:
                    FamilyLoad(doc, "给水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "给水设备附件", familyName);
                    break;
                case 36:
                    FamilyLoad(doc, "排水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "排水设备附件", familyName);
                    break;
                case 37:
                    FamilyLoad(doc, "排水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "排水设备附件", familyName);
                    break;
                case 38:
                    FamilyLoad(doc, "排水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "排水设备附件", familyName);
                    break;
                case 39:
                    FamilyLoad(doc, "排水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "排水设备附件", familyName);
                    break;
                case 40:
                    FamilyLoad(doc, "排水设备附件", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "排水设备附件", familyName);
                    break;
                case 41:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 42:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 43:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 44:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 45:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 46:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 47:
                    FamilyLoad(doc, "给水设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水设备", familyName);
                    break;
                case 48:
                    FamilyLoad(doc, "给水设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水设备", familyName);
                    break;
                case 49:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 50:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 51:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 52:
                    FamilyLoad(doc, "水泵", familyName);
                    familySymbol = EquipmentSymbol(doc, "水泵", familyName);
                    break;
                case 53:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 54:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 55:
                    FamilyLoad(doc, "给水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水处理设备", familyName);
                    break;
                case 56:
                    FamilyLoad(doc, "给水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水处理设备", familyName);
                    break;
                case 57:
                    FamilyLoad(doc, "给水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水处理设备", familyName);
                    break;
                case 59:
                    FamilyLoad(doc, "给水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水处理设备", familyName);
                    break;
                case 60:
                    FamilyLoad(doc, "给水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水处理设备", familyName);
                    break;
                case 61:
                    FamilyLoad(doc, "给水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水处理设备", familyName);
                    break;
                case 62:
                    FamilyLoad(doc, "给水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水处理设备", familyName);
                    break;
                case 63:
                    FamilyLoad(doc, "给水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水处理设备", familyName);
                    break;
                case 64:
                    FamilyLoad(doc, "污水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "污水处理设备", familyName);
                    break;
                case 65:
                    FamilyLoad(doc, "污水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "污水处理设备", familyName);
                    break;
                case 66:
                    FamilyLoad(doc, "污水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "污水处理设备", familyName);
                    break;
                case 67:
                    FamilyLoad(doc, "污水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "污水处理设备", familyName);
                    break;
                case 68:
                    FamilyLoad(doc, "污水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "污水处理设备", familyName);
                    break;
                case 69:
                    FamilyLoad(doc, "卫生设备", familyName);
                    familySymbol = PlumbingFixtureSymbol(doc, "卫生设备", familyName);
                    break;
                case 70:
                    FamilyLoad(doc, "给水构件", familyName);
                    familySymbol = EquipmentSymbol(doc, "给水构件", familyName);
                    break;
                case 71:
                    StructureFamilyLoad(doc, "排水构筑物", familyName);
                    familySymbol = WaterStructureSymbol(doc, "排水构筑物", familyName);
                    break;
                case 72:
                    StructureFamilyLoad(doc, "给水构筑物", familyName);
                    familySymbol = WaterStructureSymbol(doc, "给水构筑物", familyName);
                    break;
                case 73:
                    StructureFamilyLoad(doc, "排水构筑物", familyName);
                    familySymbol = WaterStructureSymbol(doc, "排水构筑物", familyName);
                    break;
                case 74:
                    StructureFamilyLoad(doc, "排水构筑物", familyName);
                    familySymbol = WaterStructureSymbol(doc, "排水构筑物", familyName);
                    break;
                case 75:
                    StructureFamilyLoad(doc, "排水构筑物", familyName);
                    familySymbol = WaterStructureSymbol(doc, "排水构筑物", familyName);
                    break;
                case 76:
                    FamilyLoad(doc, "排水构件", familyName);
                    familySymbol = SiteSymbol(doc, "排水构件", familyName);
                    break;
                case 77:
                    FamilyLoad(doc, "排水构件", familyName);
                    familySymbol = SiteSymbol(doc, "排水构件", familyName);
                    break;
                case 78:
                    FamilyLoad(doc, "排水构件", familyName);
                    familySymbol = SiteSymbol(doc, "排水构件", familyName);
                    break;
                case 79:
                    FamilyLoad(doc, "排水构件", familyName);
                    familySymbol = SiteSymbol(doc, "排水构件", familyName);
                    break;
                case 80:
                    FamilyLoad(doc, "排水构件", familyName);
                    familySymbol = SiteSymbol(doc, "排水构件", familyName);
                    break;
                case 81:
                    FamilyLoad(doc, "排水构件", familyName);
                    familySymbol = SiteSymbol(doc, "排水构件", familyName);
                    break;
                case 82:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 83:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 84:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 85:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 86:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 87:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 88:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 89:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 90:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = SprinklerSymbol(doc, "消防设备", familyName);
                    break;
                case 91:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = SprinklerSymbol(doc, "消防设备", familyName);
                    break;
                case 92:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 93:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 94:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "消防设备", familyName);
                    break;
                case 95:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = SiteSymbol(doc, "消防设备", familyName);
                    break;
                case 96:
                    FamilyLoad(doc, "消防设备", familyName);
                    familySymbol = SiteSymbol(doc, "消防设备", familyName);
                    break;
                case 97:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 98:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 99:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 100:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 101:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 102:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 103:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 104:
                    FamilyLoad(doc, "详图项目", familyName);
                    familySymbol = DetailComponentSymbol(doc, "详图项目", familyName);
                    break;
                case 105:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 106:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 107:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 108:
                    FamilyLoad(doc, "详图项目", familyName);
                    familySymbol = DetailComponentSymbol(doc, "详图项目", familyName);
                    break;
                case 109:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 110:
                    FamilyLoad(doc, "注释符号", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "注释符号", familyName);
                    break;
                case 113:
                    FamilyLoad(doc, "污水处理设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "污水处理设备", familyName);
                    break;
                case 114:
                    FamilyLoad(doc, "管件", familyName);
                    familySymbol = PipeFittingSymbol(doc, "管件", familyName);
                    break;
                case 115:
                    FamilyLoad(doc, "卫生设备", familyName);
                    familySymbol = PlumbingFixtureSymbol(doc, "卫生设备", familyName);
                    break;
                case 116:
                    FamilyLoad(doc, "加热设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "加热设备", familyName);
                    break;
                case 117:
                    FamilyLoad(doc, "加热设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "加热设备", familyName);
                    break;
                case 118:
                    FamilyLoad(doc, "加热设备", familyName);
                    familySymbol = EquipmentSymbol(doc, "加热设备", familyName);
                    break;
                case 119:
                    FamilyLoad(doc, "管件", familyName);
                    familySymbol = PipeFittingSymbol(doc, "管件", familyName);
                    break;
                case 120:
                    FamilyLoad(doc, "管件", familyName);
                    familySymbol = PipeFittingSymbol(doc, "管件", familyName);
                    break;
                default:
                    break;
            }
            return familySymbol;
        }
        public GroupType GetGroupTypeMethod(Document doc) //获取模型组类型
        {
            string groupName = CreatWaterFamily.mainfrm.FamilyName.Text;
            string fullname = "给排水" + "_" + "模型组" + "_" + groupName;

            FilteredElementCollector groupCollector = new FilteredElementCollector(doc);
            groupCollector.OfClass(typeof(GroupType)).OfCategory(BuiltInCategory.OST_IOSModelGroups);
            GroupType groupType = null;
            IList<Element> groups = groupCollector.ToElements();
            foreach (GroupType item in groups)
            {
                if (item.Name == fullname)
                {
                    groupType = item;
                    break;
                }
            }
            return groupType;
        }
        public FamilySymbol PipeAccessorySymbol(Document doc, string categoryName, string familyName) //管道附件
        {
            FilteredElementCollector accessoryCollector = new FilteredElementCollector(doc);
            accessoryCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessory);
            List<FamilySymbol> accessorySymbolList = new List<FamilySymbol>();
            FamilySymbol accessory = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;

            IList<Element> accessorys = accessoryCollector.ToElements();
            foreach (FamilySymbol item in accessorys)
            {
                if (item.Family.Name == fullname)
                {
                    accessorySymbolList.Add(item);
                }
            }
            if (familyName.Contains("电动蝶阀"))
            {
                foreach (FamilySymbol item in accessorySymbolList)
                {
                    if (item.Name.Contains("DN100"))
                    {
                        accessory = item;
                        break;
                    }
                }
            }
            else
            {
                accessory = accessorySymbolList.FirstOrDefault();
            }
            return accessory;
        }
        public FamilySymbol PipeFittingSymbol(Document doc, string categoryName, string familyName) //管件
        {
            FilteredElementCollector pipeFittingCollector = new FilteredElementCollector(doc);
            pipeFittingCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeFitting);
            List<FamilySymbol> pipeFittingSymbolList = new List<FamilySymbol>();
            FamilySymbol pipeFitting = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;

            IList<Element> pipeFittings = pipeFittingCollector.ToElements();
            foreach (FamilySymbol item in pipeFittings)
            {
                if (item.Family.Name == fullname)
                {
                    pipeFittingSymbolList.Add(item);
                }
            }
            pipeFitting = pipeFittingSymbolList.FirstOrDefault();
            return pipeFitting;
        }
        public FamilySymbol EquipmentSymbol(Document doc, string categoryName, string familyName) //机械设备
        {
            FilteredElementCollector equipmentCollector = new FilteredElementCollector(doc);
            equipmentCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            List<FamilySymbol> equipmentSymbolList = new List<FamilySymbol>();
            FamilySymbol equipment = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;

            IList<Element> equipments = equipmentCollector.ToElements();
            foreach (FamilySymbol item in equipments)
            {
                if (item.Family.Name == fullname)
                {
                    equipmentSymbolList.Add(item);
                }
            }
            equipment = equipmentSymbolList.FirstOrDefault();
            return equipment;
        }
        public FamilySymbol WaterStructureSymbol(Document doc, string categoryName, string familyName) //给排水构筑物
        {
            FilteredElementCollector waterStructureCollector = new FilteredElementCollector(doc);
            waterStructureCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel);
            List<FamilySymbol> waterStructureSymbolList = new List<FamilySymbol>();
            FamilySymbol waterStructure = null;
            string fullname = "结构" + "_" + categoryName + "_" + familyName;

            IList<Element> waterStructures = waterStructureCollector.ToElements();
            foreach (FamilySymbol item in waterStructures)
            {
                if (item.Family.Name == fullname)
                {
                    waterStructureSymbolList.Add(item);
                }
            }
            waterStructure = waterStructureSymbolList.FirstOrDefault();
            return waterStructure;
        }
        public FamilySymbol SiteSymbol(Document doc, string categoryName, string familyName) //室外场地构件(室外地上式消火栓,塑料排水检查井)
        {
            FilteredElementCollector siteCollector = new FilteredElementCollector(doc);
            siteCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_Site);
            List<FamilySymbol> siteSymbolList = new List<FamilySymbol>();
            FamilySymbol site = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;

            IList<Element> sites = siteCollector.ToElements();
            foreach (FamilySymbol item in sites)
            {
                if (item.Family.Name == fullname)
                {
                    siteSymbolList.Add(item);
                }
            }
            site = siteSymbolList.FirstOrDefault();
            return site;
        }
        public FamilySymbol PlumbingFixtureSymbol(Document doc, string categoryName, string familyName) //卫浴设施
        {
            FilteredElementCollector plumbingFixtureCollector = new FilteredElementCollector(doc);
            plumbingFixtureCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PlumbingFixtures);
            List<FamilySymbol> plumbingFixtureSymbolList = new List<FamilySymbol>();
            FamilySymbol plumbingFixture = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;

            IList<Element> plumbingFixtures = plumbingFixtureCollector.ToElements();
            foreach (FamilySymbol item in plumbingFixtures)
            {
                if (item.Family.Name == fullname)
                {
                    plumbingFixtureSymbolList.Add(item);
                }
            }
            plumbingFixture = plumbingFixtureSymbolList.FirstOrDefault();
            return plumbingFixture;
        }
        public FamilySymbol SprinklerSymbol(Document doc, string categoryName, string familyName) //喷头
        {
            FilteredElementCollector sprinklerCollector = new FilteredElementCollector(doc);
            sprinklerCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_Sprinklers);
            List<FamilySymbol> sprinklerSymbolList = new List<FamilySymbol>();
            FamilySymbol sprinkler = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;

            IList<Element> sprinklers = sprinklerCollector.ToElements();
            foreach (FamilySymbol item in sprinklers)
            {
                if (item.Family.Name == fullname)
                {
                    sprinklerSymbolList.Add(item);
                }
            }
            sprinkler = sprinklerSymbolList.FirstOrDefault();
            return sprinkler;
        }
        public FamilySymbol GenericAnnotationSymbol(Document doc, string categoryName, string familyName) //注释符号
        {
            FilteredElementCollector genericAnnotationCollector = new FilteredElementCollector(doc);
            genericAnnotationCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericAnnotation);
            List<FamilySymbol> genericAnnotationSymbolList = new List<FamilySymbol>();
            FamilySymbol genericAnnotation = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;

            IList<Element> genericAnnotations = genericAnnotationCollector.ToElements();
            foreach (FamilySymbol item in genericAnnotations)
            {
                if (item.Family.Name == fullname)
                {
                    genericAnnotationSymbolList.Add(item);
                }
            }
            genericAnnotation = genericAnnotationSymbolList.FirstOrDefault();
            return genericAnnotation;
        }
        public FamilySymbol DetailComponentSymbol(Document doc, string categoryName, string familyName) //详图项目
        {
            FilteredElementCollector detailComponentCollector = new FilteredElementCollector(doc);
            detailComponentCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_DetailComponents);
            List<FamilySymbol> detailComponentSymbolList = new List<FamilySymbol>();
            FamilySymbol detailComponent = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;

            IList<Element> detailComponents = detailComponentCollector.ToElements();
            foreach (FamilySymbol item in detailComponents)
            {
                if (item.Family.Name == fullname)
                {
                    detailComponentSymbolList.Add(item);
                }
            }
            detailComponent = detailComponentSymbolList.FirstOrDefault();
            return detailComponent;
        }
        public void FamilyLoad(Document doc, string categoryName, string familyName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            string fullname = "给排水" + "_" + categoryName + "_" + familyName;
            foreach (Family item in familyCollect)
            {
                if (item.Name == fullname)
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水" + "_" + categoryName + "_" + familyName + ".rfa");
            }
        }
        public void StructureFamilyLoad(Document doc, string categoryName, string familyName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(familyName) && item.Name.Contains("结构"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "结构" + "_" + categoryName + "_" + familyName + ".rfa");
            }
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
                    if (ps.Name.Contains("给排水") && ps.Name == "PVC")
                    {
                        pt = ps;
                        break;
                    }
                }

                if (ps.Name.Contains("给排水") && ps.Name.Contains(pipeType))
                {
                    pt = ps;
                    break;
                }
            }
            return pt;
        }
    }
    public class FamLoadOption : IFamilyLoadOptions //载入并覆盖现有族
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
    }
}
