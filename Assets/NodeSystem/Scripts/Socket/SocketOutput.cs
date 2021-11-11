using System;
using UnityEngine.EventSystems;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class SocketOutput : Socket, IOutput, IPointerClickHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private NodeType m_OutpuType = NodeType.Object;
        [SerializeField]
        private bool m_SsCreateNewValue = true;
        private object m_Value;

        public void SetIsCreateNewValue(bool isNewValue)
        {
            m_SsCreateNewValue = isNewValue;
        }

        public void SetOutputType(NodeType nodeType)
        {
            m_OutpuType = nodeType;
        }

        public NodeType GetOutpuType()
        {
            return m_OutpuType;
        }

        public void SetValue(object value)
        {
            if (m_Value != value && m_SsCreateNewValue)
            {
                m_Value = value;
                ValueUpdated?.Invoke();
            }
            else if(!m_SsCreateNewValue)
            {
                m_Value = value;
                ValueUpdated?.Invoke();
            }
        }

        public event Action ValueUpdated;

        public NodeType GetNodeType()
        {
            return ParentNode.GetNodeType();
        }

        public void SetNodeType(NodeType nodeType)
        {
            ParentNode.SetNodType(nodeType);
        }

        public T GetValue<T>()
        {
            return (T)m_Value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SignalSystem.InvokeOutputSocketClick(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            SignalSystem.InvokeSocketDragFrom(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (var item in eventData.hovered)
            {
                var input = item.GetComponent<SocketInput>();
                
                if (input != null)
                {
                    SignalSystem.InvokeOutputSocketDragDropTo(input);
                    return;
                }
            }

            SignalSystem.InvokeOutputSocketDragDropTo(null);
        }
    }
}