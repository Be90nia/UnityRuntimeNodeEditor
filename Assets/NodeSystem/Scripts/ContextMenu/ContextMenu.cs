using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class ContextMenu : MonoBehaviour
    {
        private RectTransform m_Rect;
        private ContextContainer m_Root;

        private List<ContextContainer> m_SubContainers;

        public void Init()
        {
            m_Rect = this.GetComponent<RectTransform>();
            m_SubContainers = new List<ContextContainer>();

            SignalSystem.OnMenuItemClicked += OnMenuItemClicked;
        }

        private void OnMenuItemClicked(ContextMenuData data, ContextContainer container)
        {
            List<ContextContainer> toRemove = new List<ContextContainer>();
            foreach (var item in m_SubContainers)
            {
                if (item.depthLevel > data.Level)
                {
                    toRemove.Add(item);
                }
            }

            foreach (var item in toRemove)
            {
                Destroy(item.gameObject);
                m_SubContainers.Remove(item);
            }

            m_SubContainers.Add(container);
        }

        public void Clear()
        {
            if (m_Root != null)
            {
                Destroy(m_Root.gameObject);
                m_SubContainers = new List<ContextContainer>();
            }
        }

        public void Show(ContextMenuData context, Vector2 pos)
        {
            m_Root = Utility.CreatePrefab<ContextContainer>("Prefabs/Contexts/ContextContainer", m_Rect);
            m_Root.Init(context.Children.ToArray());
            m_Rect.localPosition = pos;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}