using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeNodeEditor
{
    public class TimerNode : Node
    {
        public Toggle toggleLoopde;
        public Toggle toggleUsesRealTime;
        public TMP_InputField timersField;
        public SocketInput inputSocket;
        public SocketOutput outputSocket;
        private Timer _timer;
        private List<ITimer> _timers;
        [SerializeField]
        private float _duration = 5f;

        public override void Setup()
        {
            Register(inputSocket);
            Register(outputSocket);

            timersField.contentType = TMP_InputField.ContentType.DecimalNumber;
            timersField.text = _duration.ToString();
            _timers = new List<ITimer>();
            timersField.onValueChanged.AddListener(HandleDurationField);
            toggleLoopde.onValueChanged.AddListener(HandleLoopdeToggle);
            toggleUsesRealTime.onValueChanged.AddListener(HandleUsesRealTimeToggle);
            OnConnectionEvent += OnConnection;
            OnDisconnectEvent += OnDisconnect;
        }
        public void OnConnection(SocketInput input, IOutput output)
        {
//            output.ValueUpdated += OnConnectedValueUpdated;
            OnConnectedValueUpdated();
        }

        public void OnDisconnect(SocketInput input, IOutput output)
        {
            //            output.ValueUpdated -= OnConnectedValueUpdated;
            OnConnectedValueUpdated();
        }

        private void OnConnectedValueUpdated()
        {
            if (connectedOutputs.ContainsKey(inputSocket.socketId)
                && connectedOutputs[inputSocket.socketId].Count != 0)
            {
                _timers.Clear();
                foreach (var output in connectedOutputs[inputSocket.socketId])
                {
                    _timers.Add(output.GetValue<ITimer>());
                }
            }
        }


        public override void OnUpdate()
        {
            _timer.Update();
            _timer.isDone = true;
        }


        public override void Play()
        {
            _timer = Timer.Register(_duration);
            foreach (var timer in _timers)
            {
                _timer.AddEvent(timer.OnComplete);
                _timer.AddEvent(timer.OnUpdate);
            }
        }

        public void HandleDurationField(string value)
        {
            _duration = float.Parse(value);
            _timer.SetDuration(_duration);
        }

        public void HandleLoopdeToggle(bool value)
        {
            if (_timer.isLooped != value)
                _timer.isLooped = value;
        }

        public void HandleUsesRealTimeToggle(bool value)
        {
            if (_timer.usesRealTime != value)
                _timer.SetUsesRealTime(value);
        }
    }
}
