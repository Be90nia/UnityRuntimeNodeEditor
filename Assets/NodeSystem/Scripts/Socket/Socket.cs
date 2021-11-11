using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeNodeEditor
{
    public class Socket : MonoBehaviour
    {
        public string SocketID;
        public IConnection Connection;
        public SocketHandle Handle;
        public ConnectionType ConnectionType;
        private Node m_ParentNode;
        public Node ParentNode => m_ParentNode;

        public void Init(Node parent)
        {
            if (Handle == null)
                Handle = GetComponent<SocketHandle>();
            this.m_ParentNode = parent;
        }

        public bool HasConnection()
        {
            return Connection != null;
        }

        public void Connect(IConnection conn)
        {
            Connection = conn;
        }

        public void Disconnect()
        {
            Connection = null;
        }
    }
}