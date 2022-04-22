using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class MyApp : IExternalCommand
    {
        List<ElementId> listId = new List<ElementId>();
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                Reference refer = uidoc.Selection.PickObject(ObjectType.Element, new PipeSelectionFilter(), "");
                Element elem = doc.GetElement(refer);
                listId.Add(elem.Id);
                Pipe pipe = elem as Pipe;
                XYZ xyz = refer.GlobalPoint;
                Curve curve = (pipe.Location as LocationCurve).Curve;
                XYZ project = curve.Project(xyz).XYZPoint;
                Connector connector = BackPipeNearConnectors(elem, xyz);
                BackAlrefsConnectors(connector);
                uidoc.Selection.SetElementIds(listId);
                //return Result.Succeeded;
            }

            catch (Exception e)
            {
                messages = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        ///
        /// 返回管道连接件原点与鼠标拾取点在管道上的投影点最近的那个
        ///
        ///
        ///
        ///
        private Connector BackPipeNearConnectors(Element elem, XYZ xyz)
        {
            Connector conn = null;
            MEPCurve mep = elem as MEPCurve;
            SortedDictionary<double, Connector> dictionary = new SortedDictionary<double, Connector>();
            ConnectorSetIterator connectorSetIterator = mep.ConnectorManager.Connectors.ForwardIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector connector = connectorSetIterator.Current as Connector;
                if (connector.AllRefs.Size > 0)
                {
                    dictionary.Add(connector.Origin.DistanceTo(xyz), connector);
                }
            }

            return dictionary.Values.ElementAt(0);
        }
        ///
        /// 获得连接件上的连接的所有连接件 根据连接件获得图元 根据图元获得连接件 实现递归查询
        ///
        ///
        private void BackAlrefsConnectors(Connector connector)
        {
            Element elem = null;
            ConnectorSetIterator connectorSetIterator = connector.AllRefs.ForwardIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector connref = connectorSetIterator.Current as Connector;
                if (connref.Origin.IsAlmostEqualTo(connector.Origin))
                {
                    elem = connref.Owner;
                    break;
                }
            }

            if (elem.GetType().ToString() == "Autodesk.Revit.DB.Plumbing.Pipe")
            {
                listId.Add(elem.Id);

                Connector connector1 = BackPipeConnectors(elem, connector);
                if (connector1.IsConnected)
                {
                    BackAlrefsConnectors(connector1);
                }
            }
            else if (elem.GetType().ToString() == "Autodesk.Revit.DB.FamilyInstance")
            {
                listId.Add(elem.Id);
                Connector connector1 = BackInstanceConnectors(elem as FamilyInstance, connector);
                if (connector1.IsConnected)
                {
                    BackAlrefsConnectors(connector1);
                }
            }

        }
        ///
        /// 返回管道上指定连接件
        ///
        ///
        ///
        ///
        private Connector BackPipeConnectors(Element elem, Connector conn)
        {
            Connector connector = null;
            MEPCurve mep = elem as MEPCurve;
            ConnectorSetIterator connectorSetIterator = mep.ConnectorManager.Connectors.ForwardIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector connectorset = connectorSetIterator.Current as Connector;
                if (connectorset.AllRefs.Size > 0)
                {
                    if (!connectorset.Origin.IsAlmostEqualTo(conn.Origin))
                    {
                        connector = connectorset;
                        break;
                    }
                }
            }

            return connector;
        }
        ///
        /// 返回弯头 闸阀等实例的指定连接件
        ///
        ///
        ///
        ///
        private Connector BackInstanceConnectors(FamilyInstance instance, Connector conn)
        {
            Connector connector = null;
            MEPModel model = instance.MEPModel as MEPModel;
            ConnectorSetIterator connectorSetIterator = model.ConnectorManager.Connectors.ForwardIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector connectorset = connectorSetIterator.Current as Connector;
                if (connectorset.AllRefs.Size > 0)
                {
                    if (!connectorset.Origin.IsAlmostEqualTo(conn.Origin))
                    {
                        connector = connectorset;
                        break;
                    }
                }
            }
            return connector;
        }
    }
}
