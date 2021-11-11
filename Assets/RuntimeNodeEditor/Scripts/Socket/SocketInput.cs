using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeNodeEditor
{
    public class SocketInput : Socket, IInput, IPointerClickHandler
    {
        [SerializeField]
        private NodeType _inpuType = NodeType.Object;

        public NodeType GetInputType()
        {
            return _inpuType;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            SignalSystem.InvokeInputSocketClick(this, eventData);
        }

        public NodeType GetNodeType()
        {
            return GetComponentInParent<Node>().GetNodeType();
        }

        public void SetNodeType(NodeType nodeType)
        {
            parentNode.SetNodType(nodeType);
        }

        public void Rest()
        {
            parentNode.Rest();
        }
    }
}