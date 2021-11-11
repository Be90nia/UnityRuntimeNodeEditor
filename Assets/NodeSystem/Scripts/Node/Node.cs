using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class Node : MonoBehaviour
    {
        public string ID { get; private set; }
        public Vector2 Position => m_PanelRectTransform.anchoredPosition;
        public RectTransform PanelRect => m_PanelRectTransform;
        public string Path { get; private set; }

        public List<SocketOutput> outputs;
        public List<SocketInput> inputs;
        protected Dictionary<string, List<SocketOutput>> _ConnectedOutputs;

        public event Action<SocketInput, IOutput> OnConnectionEvent;
        public event Action<SocketInput, IOutput> OnDisconnectEvent;

        public TMP_Text HeaderText;
        public GameObject Body;

        private NodeDraggablePanel m_DragPanel;
        [SerializeField] 
        protected NodeType _nodeType;
        private NodeType m_InitNodeType;
        private RectTransform m_PanelRectTransform;

        public NodeType GetNodeType()
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

            m_PanelRectTransform = Body.transform.parent.GetComponent<RectTransform>();
            m_DragPanel = Body.AddComponent<NodeDraggablePanel>();
            m_DragPanel.Init(this);


            SetPosition(pos);
            outputs = new List<SocketOutput>();
            inputs = new List<SocketInput>();

            _ConnectedOutputs = new Dictionary<string, List<SocketOutput>>();

        }

        public virtual void Setup()
        {
        }


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
            if (!_ConnectedOutputs.ContainsKey(input.SocketID))
            {
                List<SocketOutput> tempOutput = new List<SocketOutput>();
                tempOutput.Add(output);
                _ConnectedOutputs.Add(input.SocketID, tempOutput);
            }
            else
            {
                if (!_ConnectedOutputs[input.SocketID].Contains(output))
                    _ConnectedOutputs[input.SocketID].Add(output);
            }

            OnConnectionEvent?.Invoke(input, output);
        }

        public void Disconnect(SocketInput input, SocketOutput output)
        {
            if (_ConnectedOutputs.ContainsKey(input.SocketID) &&
                _ConnectedOutputs[input.SocketID].Contains(output))
            {
                _ConnectedOutputs[input.SocketID].Remove(output);
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
            HeaderText.SetText(name);
        }

        public void SetType(NodeType type)
        {
            _nodeType = type;
            m_InitNodeType = type;
        }

        public void SetPosition(Vector2 pos)
        {
            m_PanelRectTransform.localPosition = pos;
        }

        public void SetAsLastSibling()
        {
            m_PanelRectTransform.SetAsLastSibling();
        }

        public void SetAsFirstSibling()
        {
            m_PanelRectTransform.SetAsFirstSibling();
        }

        public virtual void Rest()
        {
            _nodeType = m_InitNodeType;
        }

        public virtual void ChangeNodeType(NodeType nodeType)
        {
            if (_nodeType == NodeType.Object && nodeType != NodeType.Object && nodeType != _nodeType)
                SetNodType(nodeType);
        }

        public virtual void DeleteNode()
        {

        }

        public virtual void OnUpdate()
        {

        }
    }
}