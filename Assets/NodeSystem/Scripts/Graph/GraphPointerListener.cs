using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeNodeEditor
{
    public class GraphPointerListener : MonoBehaviour, IPointerClickHandler, IDragHandler, IScrollHandler
    {
        private RectTransform m_RectTransform;
        private Vector2 m_ZoomCenterPos;
        private float m_MinZoom;
        private float m_MaxZoom;
        private Camera m_UICamera;
        private Canvas m_Canvas;
        private float m_CanvasScale;
        private float m_CurrentZoom = 1;
        public static event Action<PointerEventData> GraphPointerClickEvent;
        public static event Action<PointerEventData> GraphPointerDragEvent;

        public void Init(RectTransform background, float minZoom, float maxZoom, Canvas canvas, Camera uiCamera)
        {
            m_RectTransform = background;
            m_MinZoom = minZoom;
            m_MaxZoom = maxZoom;
            m_CurrentZoom = 1;
            m_Canvas = canvas;
            m_UICamera = uiCamera;

            m_CanvasScale = m_Canvas.transform.localScale.x;
            m_CurrentZoom = m_RectTransform.localScale.x;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GraphPointerClickEvent?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            GraphPointerDragEvent?.Invoke(eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (m_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
                ScaleScreenSpaceCamera(eventData.position, eventData.scrollDelta.y);
            else
                ScaleScreenSpaceOverlay(eventData.position, eventData.scrollDelta.y);
        }

        /// <summary>
        /// 将鼠标的坐标转换到target的pivot坐标，统一在屏幕坐标下进行计算
        /// 1.取得target的屏幕坐标
        /// 2.鼠标相对于target的本地坐标
        /// 3.将鼠标的本地坐标映射到 target的 Pivot坐标
        /// 4.计算中需要考虑target的缩放
        /// </summary>
        /// <param name="screenPos">屏幕坐标系中的点</param>
        private void SetPivot(Vector2 screenPos)
        {
            Vector3 oriPos = m_RectTransform.position; // 原世界空间位置
            Vector2 oriPivot = m_RectTransform.pivot; // 原轴心位置

            Vector3 curPos = new Vector3(); // 当前在世界空间中的位置
            Vector2 curPivot = new Vector2(); // 当前轴心位置（将轴心位置移动到鼠标点击位置） 

            Vector2 targetScreenPos = m_UICamera.WorldToScreenPoint(m_RectTransform.position);
            Vector2 mouseLocalForTarget = screenPos - targetScreenPos;

            float t1 = oriPivot.x * m_RectTransform.rect.width * m_CurrentZoom * -1f;
            float t2 = (1 - oriPivot.x) * m_RectTransform.rect.width * m_CurrentZoom;
            mouseLocalForTarget.x = Mathf.Clamp(mouseLocalForTarget.x, t1, t2);
            curPivot.x = Remap(mouseLocalForTarget.x, t1, t2, 0f, 1f);

            t1 = oriPivot.y * m_RectTransform.rect.height * m_CurrentZoom * -1f;
            t2 = (1 - oriPivot.y) * m_RectTransform.rect.height * m_CurrentZoom;
            mouseLocalForTarget.y = Mathf.Clamp(mouseLocalForTarget.y, t1, t2);
            curPivot.y = Remap(mouseLocalForTarget.y, t1, t2, 0f, 1f);
            m_RectTransform.pivot = curPivot;
            curPos = oriPos + new Vector3(
                (curPivot - oriPivot).x * m_RectTransform.rect.width * m_CanvasScale * m_CurrentZoom,
                (curPivot - oriPivot).y * m_RectTransform.rect.height * m_CanvasScale * m_CurrentZoom, 0);
            m_RectTransform.position = curPos;
        }

        // Canvas RenderModel = ScreenSpaceCamera 模式时，可以使用该模式
        private void ScaleScreenSpaceCamera(Vector2 center, float delta)
        {
            // 过滤掉重复的轴心设置
            if (m_ZoomCenterPos != center)
            {
                SetPivot(center);
                m_ZoomCenterPos = center;
            }

            m_CurrentZoom = m_RectTransform.localScale.x;
            m_CurrentZoom += (delta > 0) ? 0.1f : (-0.1f);
            if (m_CurrentZoom <= m_MaxZoom && m_CurrentZoom >= m_MinZoom)
            {
                m_RectTransform.localScale = Vector3.one * m_CurrentZoom;
            }
            else
            {
                m_CurrentZoom = (m_CurrentZoom > m_MaxZoom) ? m_MaxZoom : m_MinZoom;
            }
        }

        // Canvas RenderModel = ScreenSpaceOverlay 模式时，可以使用该模式
        private void ScaleScreenSpaceOverlay(Vector2 center, float delta)
        {
            float delX = center.x - m_RectTransform.position.x;
            float delY = center.y - m_RectTransform.position.y;
            Vector2 pivot = new Vector2();
            pivot.x = delX / m_RectTransform.rect.width / m_CurrentZoom;
            pivot.y = delY / m_RectTransform.rect.height / m_CurrentZoom;
            m_CurrentZoom = m_RectTransform.localScale.x;
            m_CurrentZoom += (delta > 0) ? 0.1f : (-0.1f);
            if (m_CurrentZoom <= m_MaxZoom && m_CurrentZoom >= m_MinZoom)
            {
                m_RectTransform.localScale = Vector3.one * m_CurrentZoom;
            }
            else
            {
                m_CurrentZoom = (m_CurrentZoom > m_MaxZoom) ? m_MaxZoom : m_MinZoom;
            }

            m_RectTransform.pivot += pivot;

            // 增加画布的缩放系数，当画布大小和screen大小不一致时，画布也是有缩放的
            m_RectTransform.position += new Vector3(delX, delY, 0) * m_CanvasScale;
        }

        // 重映射函数，将x 从t1—t2的范围内映射到s1—s2的范围
        private float Remap(float x, float t1, float t2, float s1, float s2)
        {
            return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
        }
    }
}