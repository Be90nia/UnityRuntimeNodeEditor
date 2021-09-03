using System;
using UnityEngine.EventSystems;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class SocketOutput : Socket, IOutput, IPointerClickHandler, IDragHandler, IEndDragHandler
    {
        private object _value;
        public void SetValue(object value)
        {
            if (_value != value)
            {
                _value = value;
                ValueUpdated?.Invoke();
            }
        }

        public event Action ValueUpdated;

        public NodeType GetNodeType()
        {
            return parentNode.NodeType;
        }

        public void SetNodeType(NodeType nodeType)
        {
            parentNode.NodeType = nodeType;
        }

        public T GetValue<T>()
        {
            return (T)_value;
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

        public void Rest()
        {
            parentNode.Rest();
        }
    }
}