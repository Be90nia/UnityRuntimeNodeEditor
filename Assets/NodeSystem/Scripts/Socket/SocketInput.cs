using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeNodeEditor
{
    public class SocketInput : Socket, IInput, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            SignalSystem.InvokeInputSocketClick(this, eventData);
        }

        public NodeType GetNodeType()
        {
            return GetComponentInParent<Node>().NodeType;
        }

        public void SetNodeType(NodeType nodeType)
        {
            parentNode.NodeType = nodeType;
        }
    }
}