using System;
using UnityEngine.EventSystems;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class SocketOutput : Socket, IOutput, IPointerClickHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private NodeType _outpuType = NodeType.Object;
        [SerializeField]
        private bool _isCreateNewValue = true;
        private object _value;

        public void SetIsCreateNewValue(bool isNewValue)
        {
            _isCreateNewValue = isNewValue;
        }

        public void SetOutputType(NodeType nodeType)
        {
            _outpuType = nodeType;
        }

        public NodeType GetOutpuType()
        {
            return _outpuType;
        }

        public void SetValue(object value)
        {
            if (_value != value && _isCreateNewValue)
            {
                _value = value;
                ValueUpdated?.Invoke();
            }
            else if(!_isCreateNewValue)
            {
                _value = value;
                ValueUpdated?.Invoke();
            }
        }

        public event Action ValueUpdated;

        public NodeType GetNodeType()
        {
            return parentNode.GetNodeType();
        }

        public void SetNodeType(NodeType nodeType)
        {
            parentNode.SetNodType(nodeType);
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
    }
}