using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class TestTimerNode : Node ,ITimer
    {
        public SocketOutput outputSocket;
        private ITimer _timer;

        public override void Setup()
        {
            Register(outputSocket);
            SetType(NodeType.ITimer);
            SetHeader("Debug");
            _timer = this;
            outputSocket.SetValue(_timer);
        }

        public void OnUpdate(float time)
        {
            Debug.Log("OnUpdate: " + time);
        }

        public void OnComplete()
        {
            Debug.Log("OnComplete");
        }
    }

}

