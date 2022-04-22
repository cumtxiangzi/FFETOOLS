//using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Architecture;
//using Autodesk.Revit.DB.ExtensibleStorage;
//using Autodesk.Revit.DB.Mechanical;
//using Autodesk.Revit.DB.Plumbing;
//using Autodesk.Revit.DB.Structure;
//using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Selection;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Numerics;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Windows;

//namespace FFETOOLS
//{
//    [Transaction(TransactionMode.Manual)]
//    public class DrainageWellCode : IExternalCommand
//    {
//        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
//        {
//            try
//            {
//                UIApplication uiapp = commandData.Application;
//                UIDocument uidoc = uiapp.ActiveUIDocument;
//                Document doc = uidoc.Document;
//                Selection sel = uidoc.Selection;

//                FilteredElementCollector col = new FilteredElementCollector(doc)
//                                              .WhereElementIsNotElementType()
//                                              .OfCategory(BuiltInCategory.INVALID)
//                                              .OfClass(typeof(Wall));

//                foreach (Element e in col)
//                {
//                    Debug.Print(e.Name);
//                }
//                using (Transaction trans = new Transaction(doc, "name"))
//                {
//                    trans.Start();

//                    trans.Commit();
//                }
//            }
//            catch (Exception e)
//            {
//                messages = e.Message;
//                return Result.Failed;
//            }
//            return Result.Succeeded;
//        }
//    }
//    public class Node
//    {
//        //�Ƿ����ͨ��
//        public bool m_CanWalk;
//        //�ڵ�ռ�λ��
//        public Vector3 m_WorldPos;
//        //�ڵ��������λ��
//        public int m_GridX;
//        public int m_GridY;
//        //��ʼ�ڵ㵽��ǰ�ڵ�ľ����ֵ
//        public int m_gCost;
//        //��ǰ�ڵ㵽Ŀ��ڵ�ľ����ֵ
//        public int m_hCost;

//        public int FCost
//        {
//            get { return m_gCost + m_hCost; }
//        }
//        //��ǰ�ڵ�ĸ��ڵ�
//        public Node m_Parent;

//        public Node(bool canWalk, Vector3 position, int gridX, int gridY)
//        {
//            m_CanWalk = canWalk;
//            m_WorldPos = position;
//            m_GridX = gridX;
//            m_GridY = gridY;
//        }
//    }
//    public class GridBase : MonoBehaviour
//    {
//        private Node[,] m_Grid;
//        public Vector2 m_GridSize;
//        public float m_NodeRadius;
//        public LayerMask m_Layer;
//        public Stack<Node> m_Path = new Stack<Node>();
//        private float m_NodeDiameter;
//        private int m_GridCountX;
//        private int m_GridCountY;

//        void Start()
//        {
//            m_NodeDiameter = m_NodeRadius * 2;
//            m_GridCountX = Mathf.RoundToInt(m_GridSize.x / m_NodeDiameter);
//            m_GridCountY = Mathf.RoundToInt(m_GridSize.y / m_NodeDiameter);
//            m_Grid = new Node[m_GridCountX, m_GridCountY];
//            CreateGrid();
//        }

//        /// <summary>
//        /// ��������
//        /// </summary>
//        private void CreateGrid()
//        {
//            Vector3 startPos = transform.position;
//            startPos.x = startPos.x - m_GridSize.x / 2;
//            startPos.z = startPos.z - m_GridSize.y / 2;
//            for (int i = 0; i < m_GridCountX; i++)
//            {
//                for (int j = 0; j < m_GridCountY; j++)
//                {
//                    Vector3 worldPos = startPos;
//                    worldPos.x = worldPos.x + i * m_NodeDiameter + m_NodeRadius;
//                    worldPos.z = worldPos.z + j * m_NodeDiameter + m_NodeRadius;
//                    bool canWalk = !Physics.CheckSphere(worldPos, m_NodeRadius, m_Layer);
//                    m_Grid[i, j] = new Node(canWalk, worldPos, i, j);
//                }
//            }
//        }

//        /// <summary>
//        /// ͨ���ռ�λ�û�ö�Ӧ�Ľڵ�
//        /// </summary>
//        /// <param name="pos"></param>
//        /// <returns></returns>
//        public Node GetFromPosition(Vector3 pos)
//        {
//            float percentX = (pos.x + m_GridSize.x / 2) / m_GridSize.x;
//            float percentZ = (pos.z + m_GridSize.y / 2) / m_GridSize.y;
//            percentX = Mathf.Clamp01(percentX);
//            percentZ = Mathf.Clamp01(percentZ);
//            int x = Mathf.RoundToInt((m_GridCountX - 1) * percentX);
//            int z = Mathf.RoundToInt((m_GridCountY - 1) * percentZ);
//            return m_Grid[x, z];
//        }

//        /// <summary>
//        /// ��õ�ǰ�ڵ�����ڽڵ�
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        public List<Node> GetNeighor(Node node)
//        {
//            List<Node> neighborList = new List<Node>();
//            for (int i = -1; i <= 1; i++)
//            {
//                for (int j = -1; j <= 1; j++)
//                {
//                    if (i == 0 && j == 0)
//                    {
//                        continue;
//                    }
//                    int tempX = node.m_GridX + i;
//                    int tempY = node.m_GridY + j;
//                    if (tempX < m_GridCountX && tempX > 0 && tempY > 0 && tempY < m_GridCountY)
//                    {
//                        neighborList.Add(m_Grid[tempX, tempY]);
//                    }
//                }
//            }
//            return neighborList;
//        }

//        private void OnDrawGizmos()
//        {
//            Gizmos.DrawWireCube(transform.position, new Vector3(m_GridSize.x, 1, m_GridSize.y));
//            if (m_Grid == null)
//            {
//                return;
//            }
//            foreach (var node in m_Grid)
//            {
//                Gizmos.color = node.m_CanWalk ? Color.white : Color.red;
//                Gizmos.DrawCube(node.m_WorldPos, Vector3.one * (m_NodeDiameter - 0.1f));
//            }
//            if (m_Path != null)
//            {
//                foreach (var node in m_Path)
//                {
//                    Gizmos.color = Color.green;
//                    Gizmos.DrawCube(node.m_WorldPos, Vector3.one * (m_NodeDiameter - 0.1f));
//                }
//            }
//        }
//    }
//    public class FindPath : MonoBehaviour
//    {
//        public Transform m_StartNode;
//        public Transform m_EndNode;
//        private GridBase m_Grid;
//        private List<Node> openList = new List<Node>();
//        private HashSet<Node> closeSet = new HashSet<Node>();

//        void Start()
//        {
//            m_Grid = GetComponent<GridBase>();
//        }

//        void Update()
//        {
//            FindingPath(m_StartNode.position, m_EndNode.position);
//        }

//        /// <summary>
//        /// A*�㷨��Ѱ�����·��
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        private void FindingPath(Vector3 start, Vector3 end)
//        {
//            Node startNode = m_Grid.GetFromPosition(start);
//            Node endNode = m_Grid.GetFromPosition(end);
//            openList.Clear();
//            closeSet.Clear();
//            openList.Add(startNode);
//            while (openList.Count > 0)
//            {
//                // Ѱ�ҿ����б��е�F��С�Ľڵ㣬���F��ͬ��ѡȡH��С��
//                Node currentNode = openList[0];
//                for (int i = 0; i < openList.Count; i++)
//                {
//                    Node node = openList[i];
//                    if (node.FCost < currentNode.FCost || node.FCost == currentNode.FCost && node.m_hCost < currentNode.m_hCost)
//                    {
//                        currentNode = node;
//                    }
//                }
//                // �ѵ�ǰ�ڵ�ӿ����б����Ƴ��������뵽�ر��б���
//                openList.Remove(currentNode);
//                closeSet.Add(currentNode);
//                // �����Ŀ�Ľڵ㣬����
//                if (currentNode == endNode)
//                {
//                    GeneratePath(startNode, endNode);
//                    return;
//                }
//                // ������ǰ�ڵ���������ڽڵ�
//                foreach (var node in m_Grid.GetNeighor(currentNode))
//                {
//                    // ����ڵ㲻��ͨ���������ڹر��б��У�����
//                    if (!node.m_CanWalk || closeSet.Contains(node))
//                    {
//                        continue;
//                    }
//                    int gCost = currentNode.m_gCost + GetDistanceNodes(currentNode, node);
//                    // �����·�������ڵ�ľ������ ���߲��ڿ����б���
//                    if (gCost < node.m_gCost || !openList.Contains(node))
//                    {
//                        // �������ڵ��F��G��H
//                        node.m_gCost = gCost;
//                        node.m_hCost = GetDistanceNodes(node, endNode);
//                        // �������ڵ�ĸ��ڵ�Ϊ��ǰ�ڵ�
//                        node.m_Parent = currentNode;
//                        // ������ڿ����б��У����뵽�����б���
//                        if (!openList.Contains(node))
//                        {
//                            openList.Add(node);
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// ����·��
//        /// </summary>
//        /// <param name="startNode"></param>
//        /// <param name="endNode"></param>
//        private void GeneratePath(Node startNode, Node endNode)
//        {
//            Stack<Node> path = new Stack<Node>();
//            Node node = endNode;
//            while (node.m_Parent != startNode)
//            {
//                path.Push(node);
//                node = node.m_Parent;
//            }
//            m_Grid.m_Path = path;
//        }

//        /// <summary>
//        /// ��������ڵ�ľ���
//        /// </summary>
//        /// <param name="node1"></param>
//        /// <param name="node2"></param>
//        /// <returns></returns>
//        private int GetDistanceNodes(Node node1, Node node2)
//        {
//            int deltaX = Math.Abs(node1.m_GridX - node2.m_GridX);
//            int deltaY = Math.Abs(node1.m_GridY - node2.m_GridY);
//            if (deltaX > deltaY)
//            {
//                return deltaY * 14 + 10 * (deltaX - deltaY);
//            }
//            else
//            {
//                return deltaX * 14 + 10 * (deltaY - deltaX);
//            }
//        }
//    }
//}
