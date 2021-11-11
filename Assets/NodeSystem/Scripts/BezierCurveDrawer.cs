using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace RuntimeNodeEditor
{
    /// <summary>
    /// 贝塞尔曲线绘制类
    /// </summary>
    public class BezierCurveDrawer : MonoBehaviour
    {
        public RectTransform PointerLocator;
        public RectTransform LineContainer;
        [Header("Bezier settings")] public float VertexCount = 10;

        private static UILineRendererWithCollider m_LineRenderer;
        private static bool m_IsRequest;
        private static Socket m_DraggingSocket;
        private static Dictionary<string, ConnectionDrawData> m_Connections;
        private static RectTransform m_LineContainer;
        private static RectTransform m_PointerLocator;
        private static Camera m_UICamera;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(Camera camera)
        {
            m_Connections = new Dictionary<string, ConnectionDrawData>();
            m_LineContainer = LineContainer;
            m_PointerLocator = PointerLocator;
            m_LineRenderer = CreateLine();
            m_IsRequest = false;
            m_UICamera = camera;
            CancelDrag();
        }

        /// <summary>
        /// 更新绘制
        /// </summary>
        public void UpdateDraw()
        {
            if (m_Connections.Count > 0)
            {
                foreach (var conn in m_Connections.Values)
                {
                    DrawConnection(conn.Output, conn.Input, conn.LineRenderer);
                }
            }

            if (m_IsRequest)
            {
                DrawDragging(m_DraggingSocket.Handle);
            }
        }

        /// <summary>
        /// 添加连线数据
        /// </summary>
        /// <param name="connId"> </param>
        /// <param name="from"></param>
        /// <param name="target"></param>
        public void Add(string connId, SocketHandle from, SocketHandle target)
        {
            var line = CreateLine();
            var trigger = line.gameObject.AddComponent<LinePointerListener>();
            trigger.ConnectID = connId;
            m_Connections.Add(connId, new ConnectionDrawData(connId, from, target, line));
        }

        /// <summary>
        /// 移除连线
        /// </summary>
        /// <param name="connId"></param>
        public void Remove(string connId)
        {
            Destroy(m_Connections[connId].LineRenderer.gameObject);
            m_Connections.Remove(connId);
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="from"></param>
        public void StartDrag(Socket from)
        {
            m_DraggingSocket = from;
            m_IsRequest = true;
            m_LineRenderer.gameObject.SetActive(m_IsRequest);
        }

        /// <summary>
        /// 取消拖拽
        /// </summary>
        public void CancelDrag()
        {
            m_IsRequest = false;
            m_LineRenderer.gameObject.SetActive(m_IsRequest);
        }

        /// <summary>
        /// 绘制蓝图与蓝图的连线
        /// </summary>
        /// <param name="port1"></param>
        /// <param name="port2"></param>
        /// <param name="lineRenderer"></param>
        private void DrawConnection(SocketHandle port1, SocketHandle port2, UILineRendererWithCollider lineRenderer)
        {
            var pointList = new List<Vector2>();
            for (float i = 0; i < VertexCount; i++)
            {
                var t = i / VertexCount;
                pointList.Add(Utility.CubicCurve(GetLocalPoint(port1.Handle1.position),
                    GetLocalPoint(port1.Handle2.position),
                    GetLocalPoint(port2.Handle1.position),
                    GetLocalPoint(port2.Handle2.position),
                    t));
            }

            lineRenderer.m_points = pointList.ToArray();
            lineRenderer.SetVerticesDirty();
        }

        /// <summary>
        /// 绘制拖拽的线
        /// </summary>
        /// <param name="port"></param>
        private static void DrawDragging(SocketHandle port)
        {
            Vector2 localPointerPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_LineContainer, Input.mousePosition, m_UICamera,
                out localPointerPos);
            m_PointerLocator.localPosition = localPointerPos;

            var pointList = new List<Vector2>();

            for (float i = 0; i < 120; i++)
            {
                var t = i / 120;
                pointList.Add(Utility.QuadraticCurve(GetLocalPoint(port.Handle1.position),
                    GetLocalPoint(port.Handle2.position),
                    GetLocalPoint(m_PointerLocator.position),
                    t));
            }

            m_LineRenderer.m_points = pointList.ToArray();
            m_LineRenderer.SetVerticesDirty();
        }

        /// <summary>
        /// 创建贝塞尔曲线
        /// </summary>
        /// <returns></returns>
        private static UILineRendererWithCollider CreateLine()
        {
            var lineGO = new GameObject("BezierLine");
            var linerenderer = lineGO.AddComponent<UILineRendererWithCollider>();
            var lineRect = lineGO.GetComponent<RectTransform>();

            lineGO.transform.SetParent(m_LineContainer);

            lineRect.localPosition = Vector3.zero;
            lineRect.localScale = Vector3.one;
            lineRect.anchorMin = Vector2.zero;
            lineRect.anchorMax = Vector2.one;
            lineRect.Left(0);
            lineRect.Right(0);
            lineRect.Top(0);
            lineRect.Bottom(0);

            linerenderer.lineThickness = 4f;
            linerenderer.color = Color.yellow;
            linerenderer.raycastTarget = false;

            return linerenderer;
        }

        /// <summary>
        /// 获取局部点坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static Vector2 GetLocalPoint(Vector3 pos)
        {
            return Utility.GetLocalPointIn(m_LineContainer, pos);
        }

        /// <summary>
        /// 连接线的绘制数据
        /// </summary>
        private class ConnectionDrawData
        {
            public readonly string ID;
            public readonly SocketHandle Output;
            public readonly SocketHandle Input;
            public readonly UILineRendererWithCollider LineRenderer;

            public ConnectionDrawData(string id, SocketHandle port1, SocketHandle port2,
                UILineRendererWithCollider lineRenderer)
            {
                this.ID = id;
                this.Output = port1;
                this.Input = port2;
                this.LineRenderer = lineRenderer;
            }
        }
    }
}