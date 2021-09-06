using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class Node : MonoBehaviour
    {
        public string ID { get; private set; }
        public Vector2 Position => _panelRectTransform.anchoredPosition;
        public RectTransform PanelRect => _panelRectTransform;
        public string Path { get; private set; }

        public List<SocketOutput> outputs;
        public List<SocketInput> inputs;
        public Dictionary<string, List<SocketOutput>> connectedOutputs;

        public event Action<SocketInput, IOutput> OnConnectionEvent;
        public event Action<SocketInput, IOutput> OnDisconnectEvent;

        public TMP_Text headerText;
        public GameObject body;

        private NodeDraggablePanel _dragPanel;
        [SerializeField]
        protected NodeType _nodeType;
        private NodeType _initNodeType;
        private RectTransform _panelRectTransform;

        internal NodeType GetNodeType()
        {
            return _nodeType;
        }

        public virtual void SetNodType(NodeType nodeType)
        {
            _nodeType = nodeType;
        }

        public void Init(Vector2 pos, string id, string path)
        {
            ID = id;
            Path = path;

            _panelRectTransform = body.transform.parent.GetComponent<RectTransform>();
            _dragPanel = body.AddComponent<NodeDraggablePanel>();
            _dragPanel.Init(this);


            SetPosition(pos);
            outputs = new List<SocketOutput>();
            inputs = new List<SocketInput>();

            connectedOutputs = new Dictionary<string, List<SocketOutput>>();

        }

        public virtual void Setup() { }


        public virtual bool CanMove()
        {
            return true;
        }
        public void Register(SocketOutput output)
        {
            output.Init(this);
            outputs.Add(output);
        }

        public void Register(SocketInput input)
        {
            input.Init(this);
            inputs.Add(input);
        }

        public void Connect(SocketInput input, SocketOutput output)
        {
            if (!connectedOutputs.ContainsKey(input.socketId))
            {
                List<SocketOutput> tempOutput = new List<SocketOutput>();
                tempOutput.Add(output);
                connectedOutputs.Add(input.socketId, tempOutput);
            }
            else
            {
                if (!connectedOutputs[input.socketId].Contains(output))
                    connectedOutputs[input.socketId].Add(output);
            }

            OnConnectionEvent?.Invoke(input, output);
        }

        public void Disconnect(SocketInput input, SocketOutput output)
        {
            if (connectedOutputs.ContainsKey(input.socketId) &&
                connectedOutputs[input.socketId].Contains(output))
            {
                connectedOutputs[input.socketId].Remove(output);
            }

            OnDisconnectEvent?.Invoke(input, output);
        }

        public virtual void OnSerialize(Serializer serializer)
        {

        }

        public virtual void OnDeserialize(Serializer serializer)
        {

        }

        public void SetHeader(string name)
        {
            headerText.SetText(name);
        }

        public void SetType(NodeType type)
        {
            _nodeType = type;
            _initNodeType = type;
        }

        public void SetPosition(Vector2 pos)
        {
            _panelRectTransform.localPosition = pos;
        }

        public void SetAsLastSibling()
        {
            _panelRectTransform.SetAsLastSibling();
        }
        public void SetAsFirstSibling()
        {
            _panelRectTransform.SetAsFirstSibling();
        }

        public virtual void Rest()
        {
            _nodeType = _initNodeType;
        }

        public virtual void ChangeNodeType(NodeType nodeType)
        {
            if (_nodeType == NodeType.Object && nodeType != NodeType.Object && nodeType != _nodeType)
                SetNodType(nodeType);
        }
    }
}