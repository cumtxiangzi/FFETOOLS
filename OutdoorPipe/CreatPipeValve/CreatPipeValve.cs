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

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatPipeValve : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                //TransactionGroup tg = new TransactionGroup(doc, "��ӹܵ�����");
                //tg.Start();δ���

                FamilySymbol pipeAccessory = null;
                Pipe p = null;

                using (Transaction trans = new Transaction(doc, "�ܵ�����"))
                {
                    trans.Start();
                    ValveFamilyLoad(doc, "����ֹ�ط�H77X-10");
                    ValveFamilyLoad(doc, "΢�軺��ʽֹ�ط�HH44X-10");
                    ValveFamilyLoad(doc, "�綯����D97A1X-10");
                    ValveFamilyLoad(doc, "����D97A1X-10");

                    pipeAccessory = PipeAccessorySymbol(doc, "DN200", "����D37A1X");
                    pipeAccessory.Activate();

                    trans.Commit();
                }
                uidoc.PostRequestForElementTypePlacement(pipeAccessory);
            
                //tg.Assimilate();
            }
            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
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
                if (item.Family.Name.Contains("����ˮ") && item.Family.Name.Contains(accessoryName))
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

        public void ValveFamilyLoad(Document doc, string categoryName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("����ˮ"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "����ˮ_����_" + categoryName + ".rfa");
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
}
