using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeNodeEditor
{
    public class NodeEditor : MonoBehaviour
    {
        private List<Node> m_NodesList;
        public List<Node> NodesList => m_NodesList;
        [SerializeField]
        protected float _MinZoom;
        [SerializeField]
        protected float _MaxZoom;
        [SerializeField]
        protected NodeGraph _Graph;
        [SerializeField]
        protected GraphPointerListener _PointerListener;
        [SerializeField]
        protected RectTransform _ContextMenuContainer;
        [SerializeField]
        protected RectTransform _NodeContainer;

        protected ContextMenu _ContextMenu;
        protected ContextMenuData _NodeMenuData;

        [SerializeField]
        private Canvas m_NodeCanvas;

        private Camera m_UICamera;
        public Camera UICamera => m_UICamera;
        [SerializeField]
        private string m_SaveFilePath = "";

        private void Start()
        {
            Application.targetFrameRate = 60;
            m_NodesList = new List<Node>();

            if (m_NodeCanvas != null && (m_NodeCanvas.renderMode == RenderMode.WorldSpace ||
                                         m_NodeCanvas.renderMode == RenderMode.ScreenSpaceCamera))
                m_UICamera = m_NodeCanvas.worldCamera;
            else if (m_NodeCanvas != null && m_NodeCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                m_UICamera = null;
            else if (m_NodeCanvas == null)
                m_UICamera = null;

            _Graph.Init(this, _NodeContainer, m_SaveFilePath);
            _PointerListener.Init(_Graph.GraphContainer, _MinZoom, _MaxZoom, m_NodeCanvas, m_UICamera);
            Utility.Initialize(_NodeContainer, _ContextMenuContainer);
            if (_ContextMenu == null)
                _ContextMenu = Utility.CreatePrefab<ContextMenu>("Prefabs/Contexts/ContextMenu", _ContextMenuContainer);
            _ContextMenu.Init();
            GraphPointerListener.GraphPointerDragEvent += OnGraphPointerDrag;
            SignalSystem.NodePointerClickEvent += OnNodePointerClick;
            SignalSystem.LineDownEvent += OnLineDown;
        }

        public void SaveFilePath(string path)
        {
            m_SaveFilePath = path;
        }

        private void Update()
        {
            _Graph.OnUpdate();
        }

        private void OnGraphPointerDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                _Graph.GraphContainer.localPosition += new Vector3(eventData.delta.x, eventData.delta.y);
            }
        }

        private void OnNodePointerClick(Node node, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                DisplayNodeContexMenu(node);
            }
        }

        //link
        private void OnLineDown(string connId, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                DisplayLineContexMenu(connId);
            }
        }

        //  context methods
       
        public void ShowMenu(ContextMenuData menuData)
        {
            _ContextMenu.Clear();
            _ContextMenu.Show(menuData, Utility.GetCtxMenuPointerPosition(m_UICamera));
        }

        private void DisplayLineContexMenu(string connId)
        {
            _NodeMenuData = new ContextMenuBuilder()
                .Add("delete that line", () => DisconnectConnection(connId))
                .Build();

            _ContextMenu.Clear();
            _ContextMenu.Show(_NodeMenuData, Utility.GetCtxMenuPointerPosition(m_UICamera));
        }

        private void DisplayNodeContexMenu(Node node)
        {
            _NodeMenuData = new ContextMenuBuilder()
                .Add("clear connections", () => ClearConnections(node))
                .Add("delete", () => DeleteNode(node))
                .Build();

            _ContextMenu.Clear();
            _ContextMenu.Show(_NodeMenuData, Utility.GetCtxMenuPointerPosition(m_UICamera));
        }

        public void CloseContextMenu()
        {
            _ContextMenu.Hide();
            _ContextMenu.Clear();
        }

        public void CreateNode(string prefabsPath)
        {
            var pos = Utility.GetLocalPointIn(_NodeContainer, Input.mousePosition, m_UICamera);
            _Graph.Create(prefabsPath, pos);
            CloseContextMenu();
        }

        private void DeleteNode(Node node)
        {
            node.DeleteNode();
            CloseContextMenu();
            _Graph.Delete(node);
        }

        private void DisconnectConnection(string line_id)
        {
            CloseContextMenu();
            _Graph.Disconnect(line_id);
        }

        private void ClearConnections(Node node)
        {
            CloseContextMenu();
            _Graph.ClearConnectionsOf(node);
        }

        public void SaveGraph()
        {
            CloseContextMenu();
            _Graph.Save();
        }

        public void LoadGraph()
        {
            CloseContextMenu();
            _Graph.Clear();
            _Graph.Load();
        }

    }
}