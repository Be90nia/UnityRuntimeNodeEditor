using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RuntimeNodeEditor
{
    public class Vector3Node : Node
    {
        public TMP_InputField valueXField;
        public TMP_InputField valueYField;
        public TMP_InputField valueZField;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            SetType(NodeType.Vector3);
            SetHeader("Vector3");

            valueXField.text = "0";
            valueYField.text = "0";
            valueZField.text = "0";

            outputSocket.SetValue(Vector3.zero);
            SetInitField(valueXField, HandleInputValueX);
            SetInitField(valueYField, HandleInputValueY);
            SetInitField(valueZField, HandleInputValueZ);
        }

        public void SetInitField(TMP_InputField field, UnityAction<string> callBackAction)
        {
            callBackAction?.Invoke(field.text);
            field.contentType = TMP_InputField.ContentType.DecimalNumber;
            field.onValueChanged.AddListener(callBackAction);
        }

        private void HandleInputValueX(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "0";
                valueXField.SetTextWithoutNotify(value);
            }
            float valueX = float.Parse(value);
            var temp = outputSocket.GetValue<Vector3>();
            temp.x = valueX;
            outputSocket.SetValue(temp);
        }

        private void HandleInputValueY(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "0";
                valueYField.SetTextWithoutNotify(value);
            }
            float valueY = float.Parse(value);
            var temp = outputSocket.GetValue<Vector3>();
            temp.y = valueY;
            outputSocket.SetValue(temp);
        }
        private void HandleInputValueZ(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "0";
                valueZField.SetTextWithoutNotify(value);
            }
            float valueZ = float.Parse(value);
            var temp = outputSocket.GetValue<Vector3>();
            temp.z = valueZ;
            outputSocket.SetValue(temp);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("Vector3ValueX", valueXField.text)
                .Add("Vector3ValueY", valueYField.text)
                .Add("Vector3ValueZ", valueZField.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var valueX = serializer.Get("Vector3ValueX");
            valueXField.SetTextWithoutNotify(valueX);
            HandleInputValueX(valueX);

            var valueY = serializer.Get("Vector3ValueY");
            valueYField.SetTextWithoutNotify(valueY);
            HandleInputValueY(valueY);

            var valueZ = serializer.Get("Vector3ValueZ");
            valueZField.SetTextWithoutNotify(valueZ);
            HandleInputValueZ(valueZ);
        }
    }
}

