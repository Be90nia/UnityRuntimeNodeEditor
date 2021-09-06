using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class IntNode : Node
    {
        public TMP_InputField valueField;
        public SocketOutput outputSocket;   

        public override void Setup()
        {
            Register(outputSocket);


            SetType(NodeType.Int);
            SetHeader("Int");

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
            int intValue = int.Parse(value);
            outputSocket.SetValue(intValue);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("IntValue", valueField.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("IntValue");
            valueField.SetTextWithoutNotify(value);

            HandleInputValue(value);
        }
    }
}
