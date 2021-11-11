using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeNodeEditor
{
    public class NodeGraph : MonoBehaviour
    {
        public List<Connection> ConnectionList => m_ConnectionList;
        private List<Connection> m_ConnectionList;
        [SerializeField]
        private BezierCurveDrawer m_BezierCurveDrawer;
        private SocketOutput m_CurrentDraggingSocket;
        private Vector2 m_PointerOffset;
        private Vector2 m_LocalPointerPos;
        private RectTransform m_NodeContainer;
        private RectTransform m_GraphContainer;
        private NodeEditor m_NodeEditor;
        private string m_SaveFilePath = "";
        public RectTransform GraphContainer => m_GraphContainer;


        public void Init(NodeEditor editor, RectTransform nodeContainer, string filePath)
        {
            if (m_BezierCurveDrawer == null)
                m_BezierCurveDrawer = GetComponent<BezierCurveDrawer>();
            m_SaveFilePath = filePath;
            m_NodeEditor = editor;
            m_NodeContainer = nodeContainer;
            if (m_GraphContainer == null)
                m_GraphContainer = this.GetComponent<RectTransform>();
            m_ConnectionList = new List<Connection>();

            SignalSystem.OutputSocketDragStartEvent += OnOutputDragStarted;
            SignalSystem.OutputSocketDragDrop += OnOutputDragDroppedTo;
            SignalSystem.InputSocketClickEvent += OnInputSocketClicked;
            SignalSystem.OutputSocketClickEvent += OnOutputSocketClicked;
            SignalSystem.NodePointerDownEvent += OnNodePointerDown;
            SignalSystem.NodePointerDragEvent += OnNodePointerDrag;

            m_BezierCurveDrawer.Init(m_NodeEditor.UICamera);
        }

        public void Create(string prefabPath, Vector2 pos)
        {
            var node = Utility.CreateNodePrefab<Node>(prefabPath);
            node.Init(pos, CreateId, prefabPath);
            node.Setup();
            m_NodeEditor.NodesList.Add(node);
            HandleSocketRegister(node);
        }

        public void Delete(Node node)
        {
            ClearConnectionsOf(node);
            Destroy(node.gameObject);
            m_NodeEditor.NodesList.Remove(node);
        }

        public void Connect(SocketInput input, SocketOutput output)
        {
            if (input.GetNodeType() == NodeType.Object)
                input.SetNodeType(output.GetNodeType());
            var connection = new Connection()
            {
                ConnectID = CreateId,
                Input = input,
                Output = output
            };
            input.Connect(connection);
            output.Connect(connection);

            ConnectionList.Add(connection);
            input.ParentNode.Connect(input, output);
            m_BezierCurveDrawer.Add(connection.ConnectID, output.Handle, input.Handle);
        }

        public void Disconnect(Connection conn)
        {
            m_BezierCurveDrawer.Remove(conn.ConnectID);
            conn.Input.ParentNode.Disconnect(conn.Input, conn.Output);
            conn.Input.Disconnect();
            conn.Output.Disconnect();
            ConnectionList.Remove(conn);
            conn.Input.Rest();  
        }

        public void Disconnect(IConnection conn)
        {
            var connection = ConnectionList.FirstOrDefault<Connection>(c => c.ConnectID == conn.ConnectID);
            Disconnect(connection);
        }

        public void Disconnect(string id)
        {
            var connection = ConnectionList.FirstOrDefault<Connection>(c => c.ConnectID == id);
            Disconnect(connection);
        }

        public void ClearConnectionsOf(Node node)
        {
            ConnectionList.Where(conn => conn.Output.ParentNode == node || conn.Input.ParentNode == node)
                .ToList()
                .ForEach(conn => Disconnect(conn));
        }

        public void Save()
        {
            var graph = new GraphData();
            var nodeDatas = new List<NodeData>();
            var connDatas = new List<ConnectionData>();

            foreach (var node in m_NodeEditor.NodesList)
            {
                var ser = new Serializer();
                var data = new NodeData();
                node.OnSerialize(ser);

                data.id = node.ID;
                data.values = ser.Serialize();
                data.posX = node.Position.x;
                data.posY = node.Position.y;
                data.path = node.Path;

                var inputIds = new List<string>();
                foreach (var input in node.inputs)
                {
                    inputIds.Add(input.SocketID);
                }

                var outputIds = new List<string>();
                foreach (var output in node.outputs)
                {
                    outputIds.Add(output.SocketID);
                }

                data.inputSocketIds = inputIds.ToArray();
                data.outputSocketIds = outputIds.ToArray();

                nodeDatas.Add(data);
            }

            foreach (var conn in ConnectionList)
            {
                var data = new ConnectionData();
                data.id = conn.ConnectID;
                data.outputSocketId = conn.Output.SocketID;
                data.inputSocketId = conn.Input.SocketID;

                connDatas.Add(data);
            }

            graph.name = "awesome graph";
            graph.nodes = nodeDatas.ToArray();
            graph.connections = connDatas.ToArray();

            ;
            System.IO.File.WriteAllText(m_SaveFilePath, JsonUtility.ToJson(graph, true));
        }

        public void Load()
        {
            if (System.IO.File.Exists(m_SaveFilePath))
            {
                var file = System.IO.File.ReadAllText(m_SaveFilePath);
                var graph = JsonUtility.FromJson<GraphData>(file);

                foreach (var data in graph.nodes)
                {
                    LoadNode(data);
                }

                foreach (var node in m_NodeEditor.NodesList)
                {
                    var nodeData = graph.nodes.FirstOrDefault(data => data.id == node.ID);

                    for (int i = 0; i < nodeData.inputSocketIds.Length; i++)
                    {
                        node.inputs[i].SocketID = nodeData.inputSocketIds[i];
                    }

                    for (int i = 0; i < nodeData.outputSocketIds.Length; i++)
                    {
                        node.outputs[i].SocketID = nodeData.outputSocketIds[i];
                    }
                }

                foreach (var data in graph.connections)
                {
                    LoadConnect(data);
                }
            }
        }

        public void Clear()
        {
            var nodesToClear = new List<Node>(m_NodeEditor.NodesList);
            nodesToClear.ForEach(n => Delete(n));
        }

        public void OnUpdate()
        {
            m_BezierCurveDrawer.UpdateDraw();
        }


        //  event handlers
        private void OnInputSocketClicked(SocketInput input, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ConnectionList.Where(conn => conn.Input == input)
                            .ToList()
                            .ForEach(conn => Disconnect(conn));
            }
        }

        private void OnOutputSocketClicked(SocketOutput output, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ConnectionList.Where(conn => conn.Output == output)
                            .ToList()
                            .ForEach(conn => Disconnect(conn));
            }
        }

        private void OnOutputDragDroppedTo(SocketInput target)
        {

            if (m_CurrentDraggingSocket == null || target == null)
            {
                m_CurrentDraggingSocket = null;
                m_BezierCurveDrawer.CancelDrag();

                return;
            }

            //  if sockets connected already
            //  do nothing
            if (m_CurrentDraggingSocket.HasConnection() && target.HasConnection())
            {
                if (m_CurrentDraggingSocket.Connection == target.Connection)
                {
                    m_CurrentDraggingSocket = null;
                    m_BezierCurveDrawer.CancelDrag();

                    return;
                }
            }

            if (target != null)
            {
                if (target.ParentNode.Equals(m_CurrentDraggingSocket.ParentNode))
                {
                    m_BezierCurveDrawer.CancelDrag();
                    return;
                }

                if ((target.GetNodeType() == NodeType.Object ||
                     target.GetNodeType() == m_CurrentDraggingSocket.GetNodeType() ||
                     (target.GetNodeType() == NodeType.Class &&
                      target.GetInputType() == m_CurrentDraggingSocket.GetOutpuType())))
                {
                    //  check if input allows multiple connection
                    if (target.HasConnection())
                    {
                        //  disconnect old connection
                        if (target.ConnectionType != ConnectionType.Multiple)
                        {
                            Disconnect(target.Connection);
                        }
                    }

                    Connect(target, m_CurrentDraggingSocket);
                }
            }

            m_CurrentDraggingSocket = null;
            m_BezierCurveDrawer.CancelDrag();
        }

        private void OnOutputDragStarted(SocketOutput socketOnDrag)
        {
            m_CurrentDraggingSocket = socketOnDrag;
            m_BezierCurveDrawer.StartDrag(m_CurrentDraggingSocket);

            //  check socket connection type
            if (m_CurrentDraggingSocket.HasConnection())
            {
                //  if single, disconnect
                if (m_CurrentDraggingSocket.ConnectionType == ConnectionType.Single)
                {
                    Disconnect(m_CurrentDraggingSocket.Connection);
                }
            }
        }

        private void OnNodePointerDown(Node node, PointerEventData eventData)
        {
            node.SetAsLastSibling();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(((Node)node).PanelRect, eventData.position,
                eventData.pressEventCamera, out m_PointerOffset))
            {
                m_PointerOffset = eventData.position - (Vector2)((Node)node).transform.localPosition;
            }
            DragNode(node, eventData);
        }

        private void OnNodePointerDrag(Node node, PointerEventData eventData)
        {
            DragNode(node, eventData);
        }


        //  helper methods
        private void DragNode(Node node, PointerEventData eventData)
        {
            if (!node.CanMove())
            {
                return;
            }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Vector2 pointerPos = ClampToWindow(eventData);
                var success = RectTransformUtility.ScreenPointToLocalPointInRectangle(m_NodeContainer, pointerPos,
                                                                                eventData.pressEventCamera, out m_LocalPointerPos);
                if (success)
                {
                    node.SetPosition(eventData.position - m_PointerOffset);
                }
            }
        }

        private Vector2 ClampToWindow(PointerEventData eventData)
        {
            var rawPointerPos = eventData.position;
            var canvasCorners = new Vector3[4];
            m_NodeContainer.GetWorldCorners(canvasCorners);

            var clampedX = Mathf.Clamp(rawPointerPos.x, canvasCorners[0].x, canvasCorners[2].x);
            var clampedY = Mathf.Clamp(rawPointerPos.y, canvasCorners[0].y, canvasCorners[2].y);

            var newPointerPos = new Vector2(clampedX, clampedY);
            return newPointerPos;
        }

        private void HandleSocketRegister(Node node)
        {
            foreach (var i in node.inputs)
            {
                i.SocketID = CreateId;
            }

            foreach (var o in node.outputs)
            {
                o.SocketID = CreateId;
            }
        }

        private void LoadNode(NodeData data)
        {
            var node = Utility.CreateNodePrefab<Node>(data.path);
            node.Init(new Vector2(data.posX, data.posY), data.id, data.path);
            node.Setup();
            m_NodeEditor.NodesList.Add(node);

            var ser = new Serializer();
            ser.Deserialize(data.values);
            node.OnDeserialize(ser);


        }

        private void LoadConnect(ConnectionData data)
        {
            var input = m_NodeEditor.NodesList.SelectMany(n => n.inputs).FirstOrDefault(i => i.SocketID == data.inputSocketId);
            var output = m_NodeEditor.NodesList.SelectMany(n => n.outputs).FirstOrDefault(o => o.SocketID == data.outputSocketId);

            if (input != null && output != null)
            {
                var connection = new Connection()
                {
                    ConnectID = data.id,
                    Input = input,
                    Output = output
                };

                input.Connect(connection);
                output.Connect(connection);

                ConnectionList.Add(connection);
                input.ParentNode.Connect(input, output);

                m_BezierCurveDrawer.Add(connection.ConnectID, output.Handle, input.Handle);
            }
        }

        private string CreateId => Nanoid.Nanoid.Generate(size: 10);
    }
}