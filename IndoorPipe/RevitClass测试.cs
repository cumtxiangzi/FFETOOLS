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
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class Class����: IExternalCommand
    {
        UIApplication uiApp = null; //����һ��ȫ��UIApplication������ע��ָ���¼�
        IList<ElementId> listId = new List<ElementId>();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uiApp = commandData.Application;
            //Ӧ�ü�����޸�
            commandData.Application.Application.DocumentChanged += appChange;

            return Result.Succeeded;
        }

        private void appChange(object sender, DocumentChangedEventArgs e)
        {
            ElementCategoryFilter genericmodelFilter = new ElementCategoryFilter(BuiltInCategory.OST_GenericModel);
            listId.Clear();
            ICollection<ElementId> collection = e.GetAddedElementIds(genericmodelFilter);
            ICollection<ElementId> collection2 = e.GetModifiedElementIds(genericmodelFilter);

            foreach (ElementId elid in collection)
            {
                listId.Add(elid);


            }
            foreach (ElementId elid in collection2)
            {
                listId.Add(elid);

            }

            if (listId.Count() > 0)
            {
                uiApp.Idling += IdlingHandler;//ע������¼�
            }
        }

        private void IdlingHandler(object sender, IdlingEventArgs e)
        {
            UIDocument uidoc = uiApp.ActiveUIDocument;//��ȡ��ĵ�
            Document doc = uidoc.Document;
            Transaction trans = new Transaction(doc, "����");
            trans.Start();
            TaskDialog.Show("1", "1");//������Է��Լ���Ҫִ�еĴ���
            //�ڿ����¼��н���ֻ��һ��������������ж�������Ϊ�Ҳ��Թ��������ֻ��ִ��һ�����񣬺���Ļᱻȡ��
            trans.Commit();
            uiApp.Idling -= IdlingHandler;//ȡ�������¼�
        }
    }
}
