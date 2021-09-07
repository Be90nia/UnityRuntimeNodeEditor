using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeNodeEditor
{
    public class MathOperationNode : Node
    {
        public TMP_Text resultText;
        public TMP_Dropdown dropdown;
        public SocketInput inputSocket;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            Register(inputSocket);

            SetType(NodeType.Object);
            SetHeader("operation");
            outputSocket.SetValue(0f);
            
            dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData(MathOperations.Multiply.ToString()),
                new TMP_Dropdown.OptionData(MathOperations.Divide.ToString()),
                new TMP_Dropdown.OptionData(MathOperations.Add.ToString()),
                new TMP_Dropdown.OptionData(MathOperations.Substract.ToString())
            });

            dropdown.onValueChanged.AddListener(selected => { OnConnectedValueUpdated(); });

            OnConnectionEvent += OnConnection;
            OnDisconnectEvent += OnDisconnect;
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

        public override void OnSerialize(Serializer serializer)
        {
            var output = outputSocket.GetValue<object>();
            serializer.Add("outputValue", output.ToString())
                .Add("opType", dropdown.value.ToString());
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var opType = int.Parse(serializer.Get("opType"));
            dropdown.SetValueWithoutNotify(opType);
            var outputValue = serializer.Get("outputValue");
            Display(outputValue);
        }

        private void OnConnectedValueUpdated()
        {
            if (connectedOutputs[inputSocket.socketId].Count != 0)
            {
                List<object> incomingValues = new List<object>();
                foreach (var c in connectedOutputs[inputSocket.socketId])
                {
                    ChangeNodeType(c.GetOutpuType());
                    incomingValues.Add(c.GetValue<object>());
                }
                switch (GetNodeType())
                {
                    case NodeType.Float:
                        ValueUpdate<float>(incomingValues);
                        break;
                    case NodeType.Int:
                        ValueUpdate<int>(incomingValues);
                        break;
                    case NodeType.Vector3:
                        ValueUpdate<Vector3>(incomingValues);
                        break;
                }
            }
            else
            {
                Display("result");
            }
            
        }

        private void ValueUpdate<T>(List<object> values)
        {
            T result = (T)Calculate(values);
            outputSocket.SetValue(result);
            Display(((T)result).ToString());
        }

        private void Display(string text)
        {
            resultText.text = text;
        }

        private object Calculate(List<object> values)
        {
            if (values.Count > 0)
            {
                var operation = (MathOperations)dropdown.value;
                switch (operation)
                {
                    case MathOperations.Multiply:
                        return values.Aggregate((x, y) =>
                        {
                            object temp = null;
                            if (GetNodeType() == NodeType.Float)
                                temp = (float)x * (float)y;
                            if (GetNodeType() == NodeType.Int)
                                temp = (int)x * (int)y;
                            if (GetNodeType() == NodeType.Vector3)
                            {
                                var tempX = (Vector3) x;
                                var tempY = (Vector3)y;
                                temp = new Vector3(tempX.x*tempY.x, tempX.y * tempY.y, tempX.z * tempY.z);
                            }
                            return temp;
                        });
                    case MathOperations.Divide:
                        return values.Aggregate((x, y) =>
                        {
                            object temp = null;
                            if (GetNodeType() == NodeType.Float)
                                temp = (float)x / (float)y;
                            if (GetNodeType() == NodeType.Int)
                                temp = (int)x / (int)y;
                            if (GetNodeType() == NodeType.Vector3)
                            {
                                var tempX = (Vector3)x;
                                var tempY = (Vector3)y;
                                temp = new Vector3(tempX.x / tempY.x, tempX.y / tempY.y, tempX.z / tempY.z);
                            }
                            return temp;
                        });
                    case MathOperations.Add:
                        return values.Aggregate((x, y) =>
                        {
                            object temp = null;
                            if (GetNodeType() == NodeType.Float)
                                temp = (float)x + (float)y;
                            if (GetNodeType() == NodeType.Int)
                                temp = (int)x + (int)y;
                            if (GetNodeType() == NodeType.Vector3)
                                temp = (Vector3) x + (Vector3) y;
                            return temp;
                        });
                    case MathOperations.Substract:
                        return values.Aggregate((x, y) =>
                        {
                            object temp = null;
                            if (GetNodeType() == NodeType.Float)
                                temp = (float)x - (float)y;
                            if (GetNodeType() == NodeType.Int)
                                temp = (int)x - (int)y;
                            if (GetNodeType() == NodeType.Vector3)
                                temp = (Vector3)x - (Vector3)y;
                            return temp;
                        });
                    default:
                        return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public override void SetNodType(NodeType nodeType)
        {
            base.SetNodType(nodeType);
            outputSocket.SetOutputType(_nodeType);
        }

        public override void Rest()
        {
            base.Rest();
            outputSocket.SetOutputType(_nodeType);
        }
    }
}