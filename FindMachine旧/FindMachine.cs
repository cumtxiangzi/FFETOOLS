using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class FindMachine : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FilteredElementCollector machineCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            IList<Element> machineelms = machineCollect.ToElements();
            FamilyInstance machine = null;

            FilteredElementCollector modelCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel);
            IList<Element> modelelms = modelCollect.ToElements();
            FamilyInstance model = null;

            FindMachineWindow fw = new FindMachineWindow();
            fw.ShowDialog();

            try
            {
                foreach (Element elm in machineelms)
                {
                    FamilyInstance ins = elm as FamilyInstance;
                    Parameter note = ins.LookupParameter("标记");
                    if (note.AsString() == fw.machineNote)
                    {
                        machine = ins;
                        break;
                    }
                }

                foreach (Element elm in modelelms)
                {
                    FamilyInstance ins = elm as FamilyInstance;
                    Parameter note = ins.LookupParameter("标记");
                    if (note.AsString() == fw.machineNote)
                    {
                        model = ins;
                        break;
                    }
                }

                IList<ElementId> list = new List<ElementId>();
                if (null!=machine)
                {
                    list.Add(machine.Id);
                }
                else if (null!=model)
                {
                    list.Add(model.Id);
                }              
                uidoc.Selection.SetElementIds(list);
                uidoc.ShowElements(list);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                //throw;
                TaskDialog.Show("错误", "请输入正确设备编号");
                return Result.Failed;
            }
        }
    }
}
