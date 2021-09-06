using TMPro;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class TransformNode : Node
    {
        public TMP_Text resultTextPosition;
        public TMP_Text resultTextRotation;
        public TMP_Text resultTextScale;

        public SocketInput inputSocketPosition;
        public SocketInput inputSocketRotation;
        public SocketInput inputSocketScale;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            Register(inputSocketPosition);
            Register(inputSocketRotation);
            Register(inputSocketScale);
            SetType(NodeType.Class);
            SetHeader("transform");

            outputSocket.SetValue(new RuntimeNodeEditor.TransformRuntimeData());
            
            OnConnectionEvent += OnConnection;
            OnDisconnectEvent += OnDisconnect;
            OnConnectedValueUpdated();

        }

        public override void OnSerialize(Serializer serializer)
        {
            var output = outputSocket.GetValue<TransformRuntimeData>();
            serializer.Add("outputPosition", output.position.ToString())
                .Add("outputRotation", output.rotation.ToString())
                .Add("outputScale", output.scale.ToString());
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var outputPosition = serializer.Get("outputPosition");
            var outputRotation = serializer.Get("outputRotation");
            var outputScale = serializer.Get("outputScale");
            Display(resultTextPosition,outputPosition);
            Display(resultTextRotation,outputRotation);
            Display(resultTextScale,outputScale);
        }

        public void OnConnection(SocketInput input, IOutput output)
        {
            output.ValueUpdated += OnConnectedValueUpdated;
            OnConnectedValueUpdated();
        }

        public void OnDisconnect(SocketInput input, IOutput output)
        {
            output.ValueUpdated -= OnConnectedValueUpdated;
            OnConnectedValueUpdated();

        }

        private void OnConnectedValueUpdated()
        {
            ValueUpdate(inputSocketPosition.socketId, resultTextPosition, outputSocket.GetValue<TransformRuntimeData>().position);
            ValueUpdate(inputSocketRotation.socketId, resultTextRotation, outputSocket.GetValue<TransformRuntimeData>().rotation);
            ValueUpdate(inputSocketScale.socketId, resultTextScale, outputSocket.GetValue<TransformRuntimeData>().scale);
        }

        private void ValueUpdate(string id, TMP_Text text, Vector3 value)
        {
            if (connectedOutputs.ContainsKey(id)
                && connectedOutputs[id].Count != 0)
            {
                Vector3 temp = Vector3.zero;
                foreach (var output in connectedOutputs[id])
                {
                    temp += output.GetValue<Vector3>();
                }

                value += temp;
            }

            Display(text, value.ToString());
        }

        private void Display(TMP_Text text, string value)
        {
            text.text = value;
        }
    }
}

