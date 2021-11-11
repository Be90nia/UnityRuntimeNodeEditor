using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class Utility
    {
        private static RectTransform m_NodeContainer;
        private static RectTransform m_ContextMenuContainer;

        public static List<string> GetInterfaceChildrens<T>()
        {
            List<string> tempTypeName = new List<string>();
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in types)
            {
                if (type.GetInterfaces().Contains(typeof(T)) && type.IsInterface)
                {
                    tempTypeName.Add(type.Name);
                }
            }

            return tempTypeName;
        }

        public static List<string> GetClassChildrens<T>()
        {
            List<string> tempTypeName = new List<string>();
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(T)) && type.IsClass)
                {
                    tempTypeName.Add(type.Name);
                }
            }

            return tempTypeName;
        }
        public static void Initialize(RectTransform nodeContainer, RectTransform contextMenuContainer)
        {
            m_NodeContainer = nodeContainer;
            m_ContextMenuContainer = contextMenuContainer;
        }
        /// <summary>
        /// 获取点在Rect的局部点位
        /// </summary>
        /// <param name="container"></param>
        /// <param name="pos"></param>
        /// <param name="eventCamera"></param>
        /// <returns></returns>
        public static Vector2 GetLocalPointIn(RectTransform container, Vector3 pos, Camera eventCamera = null)
        {
            var point = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pos, eventCamera, out point);
            return point;
        }

        /// <summary>
        /// 获取菜单生成的位置
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetCtxMenuPointerPosition(Camera uiCamera = null)
        {
            Vector2 localPointerPos;
            var success = RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ContextMenuContainer,
                                                                                  Input.mousePosition,
                                                                                  uiCamera,
                                                                                  out localPointerPos);
            return localPointerPos;
        }

        /// <summary>
        /// 创建实例化预制体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T CreatePrefab<T>(string path, Transform parent)
        {
            var prefab = Resources.Load<GameObject>(path);
            var instance = GameObject.Instantiate(prefab, parent);
            var component = instance.GetComponent<T>();

            return component;
        }

        /// <summary>
        /// 创建蓝图的实例化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T CreateNodePrefab<T>(string path) where T : Node
        {
            return CreatePrefab<T>(path, m_NodeContainer);
        }

        public static Vector3 QuadraticCurve(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            var p0 = Vector3.Lerp(a, b, t);
            var p1 = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(p0, p1, t);
        }

        public static Vector3 CubicCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            var p0 = QuadraticCurve(a, b, c, t);
            var p1 = QuadraticCurve(b, c, d, t);
            return Vector3.Lerp(p0, p1, t);
        }


        public static Transform FindTopmostCanvas(Transform currentObject)
        {
            var canvases = currentObject.GetComponentsInParent<Canvas>();
            if (canvases.Length == 0)
            {
                return null;
            }

            return canvases[canvases.Length - 1].transform;
        }

        public static void UpdateLayout(UnityEngine.UI.LayoutGroup layout)
        {
            if (layout == null)
            {
                return;
            }

            layout.CalculateLayoutInputHorizontal();
            layout.SetLayoutHorizontal();
            layout.CalculateLayoutInputVertical();
            layout.SetLayoutVertical();
        }

        private static string GetPrefabPath(string path, string name)
        {
            return path + name;
        }

        //public static string GetContextPrefabPath(string name)
        //{
        //    return GetPrefabPath(Const.CONTEXT_PREFAB_PATH, name);
        //}

        //public static string GetNodePrefabPath(string name)
        //{
        //    return GetPrefabPath(Const.NODES_PREFAB_PATH, name);
        //}

        public static T GetOrAddComponent<T>(Component obj)
            where T : Component
        {
            var component = obj.GetComponent<T>();
            if (component == null)
            {
                component = obj.gameObject.AddComponent<T>();
            }

            return component;
        }
    }

    public static class RectTransformExtensions
    {
        public static RectTransform Left(this RectTransform rt, float x)
        {
            rt.offsetMin = new Vector2(x, rt.offsetMin.y);
            return rt;
        }

        public static RectTransform Right(this RectTransform rt, float x)
        {
            rt.offsetMax = new Vector2(-x, rt.offsetMax.y);
            return rt;
        }

        public static RectTransform Bottom(this RectTransform rt, float y)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, y);
            return rt;
        }

        public static RectTransform Top(this RectTransform rt, float y)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -y);
            return rt;
        }

        public static bool IsRectTransformOverlap(this RectTransform rect1, RectTransform rect2)
        {
            float rect1MinX = rect1.position.x - rect1.rect.width / 2;
            float rect1MaxX = rect1.position.x + rect1.rect.width / 2;
            float rect1MinY = rect1.position.y - rect1.rect.height / 2;
            float rect1MaxY = rect1.position.y + rect1.rect.height / 2;
            float rect2MinX = rect2.position.x - rect2.rect.width / 2;
            float rect2MaxX = rect2.position.x + rect2.rect.width / 2;
            float rect2MinY = rect2.position.y - rect2.rect.height / 2;
            float rect2MaxY = rect2.position.y + rect2.rect.height / 2;

            bool notOverlap = rect1MaxX <= rect2MinX || rect2MaxX <= rect1MinX || rect1MaxY <= rect2MinY || rect2MaxY <= rect1MinY;

            return !notOverlap;
        }
    }
}