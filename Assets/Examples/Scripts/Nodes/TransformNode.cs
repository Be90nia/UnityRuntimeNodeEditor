using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
            outputSocket.SetIsCreateNewValue(false);
            OnConnectionEvent += OnConnection;
            OnDisconnectEvent += OnDisconnect;
            OnConnectedValueUpdated();

        }

        private void Update()
        {

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
            ValueUpdate(inputSocketPosition.socketId, resultTextPosition,ref outputSocket.GetValue<TransformRuntimeData>().position);
            ValueUpdate(inputSocketRotation.socketId, resultTextRotation,ref outputSocket.GetValue<TransformRuntimeData>().rotation);
            ValueUpdate(inputSocketScale.socketId, resultTextScale,ref outputSocket.GetValue<TransformRuntimeData>().scale);
            outputSocket.SetValue(outputSocket.GetValue<TransformRuntimeData>());
        }

        private void ValueUpdate(string id, TMP_Text text,ref Vector3 value)
        {
            if (connectedOutputs.ContainsKey(id)
                && connectedOutputs[id].Count != 0)
            {
                Vector3 temp = Vector3.zero;
                foreach (var output in connectedOutputs[id])
                {
                    temp += output.GetValue<Vector3>();
                }
                value = temp;
            }

            Display(text, value.ToString());
        }

        private void Display(TMP_Text text, string value)
        {
            text.text = value;
        }
    }
}

