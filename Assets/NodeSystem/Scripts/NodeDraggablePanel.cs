using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeNodeEditor
{
    public class NodeDraggablePanel : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler
    {
        private Node m_ParentNode;

        public void Init(Node parent)
        {
            m_ParentNode = parent;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SignalSystem.InvokeNodePointerClick(m_ParentNode, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SignalSystem.InvokeNodePointerDown(m_ParentNode, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            SignalSystem.InvokeNodePointerDrag(m_ParentNode, eventData);
        }

    }
}