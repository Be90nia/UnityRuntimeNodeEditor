    using TMPro;
using UnityEngine;

namespace RuntimeNodeEditor
{

    public class CubeNode : Node
    {
        [HideInInspector]
        public GameObject Model;
        [SerializeField]
        private GameObject _cubePrefab;
        public TMP_Text resultTextPosition;
        public TMP_Text resultTextRotation;
        public TMP_Text resultTextScale;

        public SocketInput inputSocketTransform;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            Register(inputSocketTransform);
            SetType(NodeType.Class);
            SetHeader("Cube");
            CreateCube(Vector3.zero, Quaternion.identity, Vector3.one);
            outputSocket.SetValue(Model);
            OnConnectionEvent += OnConnection;
            OnDisconnectEvent += OnDisconnect;
            OnConnectedValueUpdated();
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

        public void CreateCube(Vector3 pos,Quaternion rot, Vector3 scale)
        {
            Model = Instantiate(_cubePrefab);
            Model.transform.position = pos;
            Model.transform.rotation = rot;
            Model.transform.localScale = scale;
        }

        private void OnConnectedValueUpdated()
        {
            ValueUpdate(inputSocketTransform.socketId, resultTextPosition, resultTextRotation, resultTextScale);
        }

        private void ValueUpdate(string id, TMP_Text posText, TMP_Text rotText, TMP_Text scaleText)
        {
            if (connectedOutputs.ContainsKey(id)
                && connectedOutputs[id].Count != 0)
            {
                Model.transform.position = connectedOutputs[id][0].GetValue<TransformRuntimeData>().position;
                Model.transform.localEulerAngles = connectedOutputs[id][0].GetValue<TransformRuntimeData>().rotation;
                Model.transform.localScale = connectedOutputs[id][0].GetValue<TransformRuntimeData>().scale;
            }
            Display(posText, Model.transform.position.ToString());
            Display(rotText, Model.transform.localEulerAngles.ToString());
            Display(scaleText, Model.transform.localScale.ToString());
        }

        private void Display(TMP_Text text, string value)
        {
            text.text = value;
        }

        public override void DeleteNode()
        {
            Destroy(Model);
        }
    }
}

