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
                    using (Transaction trans = new Transaction(doc, "���ø���ˮ��"))
                    {
                        trans.Start();
                        groupType = GetGroupTypeMethod(doc);
                        trans.Commit();
                    }
                    uidoc.PostRequestForElementTypePlacement(groupType);
                }
                else
                {
                    using (Transaction trans = new Transaction(doc, "���ø���ˮ��"))
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
            return "���ø���ˮ��";
        }
        public FamilySymbol CreatWaterFamilyMethod(Document doc)
        {
            int index = CreatWaterFamily.mainfrm.index;
            string familyName = CreatWaterFamily.mainfrm.FamilyNameText;
            FamilySymbol familySymbol = null;
            switch (index)
            {
                case 1:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 2:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 3:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 4:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 5:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 6:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 7:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 8:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 9:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 10:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 11:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 12:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 13:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 14:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 15:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 16:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 17:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 18:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 19:
                    FamilyLoad(doc, "����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "����", familyName);
                    break;
                case 20:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 21:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 22:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 23:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 24:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 25:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 26:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 27:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 28:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 29:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 30:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 31:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 111:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 112:
                    FamilyLoad(doc, "�Ǳ�", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "�Ǳ�", familyName);
                    break;
                case 32:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 33:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 34:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 35:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 36:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 37:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 38:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 39:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 40:
                    FamilyLoad(doc, "��ˮ�豸����", familyName);
                    familySymbol = PipeAccessorySymbol(doc, "��ˮ�豸����", familyName);
                    break;
                case 41:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 42:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 43:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 44:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 45:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 46:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 47:
                    FamilyLoad(doc, "��ˮ�豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�豸", familyName);
                    break;
                case 48:
                    FamilyLoad(doc, "��ˮ�豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�豸", familyName);
                    break;
                case 49:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 50:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 51:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 52:
                    FamilyLoad(doc, "ˮ��", familyName);
                    familySymbol = EquipmentSymbol(doc, "ˮ��", familyName);
                    break;
                case 53:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 54:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 55:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 56:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 57:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 59:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 60:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 61:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 62:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 63:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 64:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 65:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 66:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 67:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 68:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 69:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = PlumbingFixtureSymbol(doc, "�����豸", familyName);
                    break;
                case 70:
                    FamilyLoad(doc, "��ˮ����", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ����", familyName);
                    break;
                case 71:
                    StructureFamilyLoad(doc, "��ˮ������", familyName);
                    familySymbol = WaterStructureSymbol(doc, "��ˮ������", familyName);
                    break;
                case 72:
                    StructureFamilyLoad(doc, "��ˮ������", familyName);
                    familySymbol = WaterStructureSymbol(doc, "��ˮ������", familyName);
                    break;
                case 73:
                    StructureFamilyLoad(doc, "��ˮ������", familyName);
                    familySymbol = WaterStructureSymbol(doc, "��ˮ������", familyName);
                    break;
                case 74:
                    StructureFamilyLoad(doc, "��ˮ������", familyName);
                    familySymbol = WaterStructureSymbol(doc, "��ˮ������", familyName);
                    break;
                case 75:
                    StructureFamilyLoad(doc, "��ˮ������", familyName);
                    familySymbol = WaterStructureSymbol(doc, "��ˮ������", familyName);
                    break;
                case 76:
                    FamilyLoad(doc, "��ˮ����", familyName);
                    familySymbol = SiteSymbol(doc, "��ˮ����", familyName);
                    break;
                case 77:
                    FamilyLoad(doc, "��ˮ����", familyName);
                    familySymbol = SiteSymbol(doc, "��ˮ����", familyName);
                    break;
                case 78:
                    FamilyLoad(doc, "��ˮ����", familyName);
                    familySymbol = SiteSymbol(doc, "��ˮ����", familyName);
                    break;
                case 79:
                    FamilyLoad(doc, "��ˮ����", familyName);
                    familySymbol = SiteSymbol(doc, "��ˮ����", familyName);
                    break;
                case 80:
                    FamilyLoad(doc, "��ˮ����", familyName);
                    familySymbol = SiteSymbol(doc, "��ˮ����", familyName);
                    break;
                case 81:
                    FamilyLoad(doc, "��ˮ����", familyName);
                    familySymbol = SiteSymbol(doc, "��ˮ����", familyName);
                    break;
                case 82:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 83:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 84:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 85:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 86:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 87:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 88:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 89:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 90:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = SprinklerSymbol(doc, "�����豸", familyName);
                    break;
                case 91:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = SprinklerSymbol(doc, "�����豸", familyName);
                    break;
                case 92:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 93:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 94:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 95:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = SiteSymbol(doc, "�����豸", familyName);
                    break;
                case 96:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = SiteSymbol(doc, "�����豸", familyName);
                    break;
                case 97:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 98:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 99:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 100:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 101:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 102:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 103:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 104:
                    FamilyLoad(doc, "��ͼ��Ŀ", familyName);
                    familySymbol = DetailComponentSymbol(doc, "��ͼ��Ŀ", familyName);
                    break;
                case 105:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 106:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 107:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 108:
                    FamilyLoad(doc, "��ͼ��Ŀ", familyName);
                    familySymbol = DetailComponentSymbol(doc, "��ͼ��Ŀ", familyName);
                    break;
                case 109:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 110:
                    FamilyLoad(doc, "ע�ͷ���", familyName);
                    familySymbol = GenericAnnotationSymbol(doc, "ע�ͷ���", familyName);
                    break;
                case 113:
                    FamilyLoad(doc, "��ˮ�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "��ˮ�����豸", familyName);
                    break;
                case 114:
                    FamilyLoad(doc, "�ܼ�", familyName);
                    familySymbol = PipeFittingSymbol(doc, "�ܼ�", familyName);
                    break;
                case 115:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = PlumbingFixtureSymbol(doc, "�����豸", familyName);
                    break;
                case 116:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 117:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 118:
                    FamilyLoad(doc, "�����豸", familyName);
                    familySymbol = EquipmentSymbol(doc, "�����豸", familyName);
                    break;
                case 119:
                    FamilyLoad(doc, "�ܼ�", familyName);
                    familySymbol = PipeFittingSymbol(doc, "�ܼ�", familyName);
                    break;
                case 120:
                    FamilyLoad(doc, "�ܼ�", familyName);
                    familySymbol = PipeFittingSymbol(doc, "�ܼ�", familyName);
                    break;
                default:
                    break;
            }
            return familySymbol;
        }
        public GroupType GetGroupTypeMethod(Document doc) //��ȡģ��������
        {
            string groupName = CreatWaterFamily.mainfrm.FamilyName.Text;
            string fullname = "����ˮ" + "_" + "ģ����" + "_" + groupName;

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
        public FamilySymbol PipeAccessorySymbol(Document doc, string categoryName, string familyName) //�ܵ�����
        {
            FilteredElementCollector accessoryCollector = new FilteredElementCollector(doc);
            accessoryCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeAccessory);
            List<FamilySymbol> accessorySymbolList = new List<FamilySymbol>();
            FamilySymbol accessory = null;
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;

            IList<Element> accessorys = accessoryCollector.ToElements();
            foreach (FamilySymbol item in accessorys)
            {
                if (item.Family.Name == fullname)
                {
                    accessorySymbolList.Add(item);
                }
            }
            if (familyName.Contains("�綯����"))
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
        public FamilySymbol PipeFittingSymbol(Document doc, string categoryName, string familyName) //�ܼ�
        {
            FilteredElementCollector pipeFittingCollector = new FilteredElementCollector(doc);
            pipeFittingCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PipeFitting);
            List<FamilySymbol> pipeFittingSymbolList = new List<FamilySymbol>();
            FamilySymbol pipeFitting = null;
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;

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
        public FamilySymbol EquipmentSymbol(Document doc, string categoryName, string familyName) //��е�豸
        {
            FilteredElementCollector equipmentCollector = new FilteredElementCollector(doc);
            equipmentCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            List<FamilySymbol> equipmentSymbolList = new List<FamilySymbol>();
            FamilySymbol equipment = null;
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;

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
        public FamilySymbol WaterStructureSymbol(Document doc, string categoryName, string familyName) //����ˮ������
        {
            FilteredElementCollector waterStructureCollector = new FilteredElementCollector(doc);
            waterStructureCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel);
            List<FamilySymbol> waterStructureSymbolList = new List<FamilySymbol>();
            FamilySymbol waterStructure = null;
            string fullname = "�ṹ" + "_" + categoryName + "_" + familyName;

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
        public FamilySymbol SiteSymbol(Document doc, string categoryName, string familyName) //���ⳡ�ع���(�������ʽ����˨,������ˮ��龮)
        {
            FilteredElementCollector siteCollector = new FilteredElementCollector(doc);
            siteCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_Site);
            List<FamilySymbol> siteSymbolList = new List<FamilySymbol>();
            FamilySymbol site = null;
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;

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
        public FamilySymbol PlumbingFixtureSymbol(Document doc, string categoryName, string familyName) //��ԡ��ʩ
        {
            FilteredElementCollector plumbingFixtureCollector = new FilteredElementCollector(doc);
            plumbingFixtureCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_PlumbingFixtures);
            List<FamilySymbol> plumbingFixtureSymbolList = new List<FamilySymbol>();
            FamilySymbol plumbingFixture = null;
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;

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
        public FamilySymbol SprinklerSymbol(Document doc, string categoryName, string familyName) //��ͷ
        {
            FilteredElementCollector sprinklerCollector = new FilteredElementCollector(doc);
            sprinklerCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_Sprinklers);
            List<FamilySymbol> sprinklerSymbolList = new List<FamilySymbol>();
            FamilySymbol sprinkler = null;
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;

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
        public FamilySymbol GenericAnnotationSymbol(Document doc, string categoryName, string familyName) //ע�ͷ���
        {
            FilteredElementCollector genericAnnotationCollector = new FilteredElementCollector(doc);
            genericAnnotationCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericAnnotation);
            List<FamilySymbol> genericAnnotationSymbolList = new List<FamilySymbol>();
            FamilySymbol genericAnnotation = null;
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;

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
        public FamilySymbol DetailComponentSymbol(Document doc, string categoryName, string familyName) //��ͼ��Ŀ
        {
            FilteredElementCollector detailComponentCollector = new FilteredElementCollector(doc);
            detailComponentCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_DetailComponents);
            List<FamilySymbol> detailComponentSymbolList = new List<FamilySymbol>();
            FamilySymbol detailComponent = null;
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;

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
            string fullname = "����ˮ" + "_" + categoryName + "_" + familyName;
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
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "����ˮ" + "_" + categoryName + "_" + familyName + ".rfa");
            }
        }
        public void StructureFamilyLoad(Document doc, string categoryName, string familyName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(familyName) && item.Name.Contains("�ṹ"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "�ṹ" + "_" + categoryName + "_" + familyName + ".rfa");
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
    }
    public class FamLoadOption : IFamilyLoadOptions //���벢����������
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
