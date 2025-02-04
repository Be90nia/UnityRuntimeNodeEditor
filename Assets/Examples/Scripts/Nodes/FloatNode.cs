using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor
{
    public class FloatNode : Node
    {
        public TMP_InputField valueField;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);


            SetType(NodeType.Float);
            SetHeader("float");

            valueField.text = "0";
            HandleInputValue(valueField.text);

            valueField.contentType = TMP_InputField.ContentType.DecimalNumber;
            valueField.onValueChanged.AddListener(HandleInputValue);
        }

        private void HandleInputValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "0";
                valueField.SetTextWithoutNotify(value);
            }
            float floatValue = float.Parse(value);
            outputSocket.SetValue(floatValue);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("floatValue", valueField.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("floatValue");
            valueField.SetTextWithoutNotify(value);

            HandleInputValue(value);
        }
    }
}