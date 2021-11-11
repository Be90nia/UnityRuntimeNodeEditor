using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeNodeEditor
{
    public class ContextItem : MonoBehaviour
    {
        public TMP_Text NameText;
        public Button Button;
        public Transform SubContextIcon;
        public Transform SubContextTransform;

        public void Init(ContextMenuData node)
        {
            NameText.text = node.Name;

            bool hasSubMenu = node.Children.Count > 0;
            SubContextIcon.gameObject.SetActive(hasSubMenu);

            if (hasSubMenu)
            {
                Button.onClick.AddListener(
                    () =>
                    {
                        var container = Utility.CreatePrefab<ContextContainer>("Prefabs/Contexts/ContextContainer", SubContextTransform);
                        container.Init(node.Children.ToArray());
                        SignalSystem.InvokeMenuItemClicked(node, container);
                    }
                );
            }
            else
            {
                Button.onClick.AddListener(
                    () =>
                    {
                        node.ContextMenuCallBack?.Invoke();
                    }
                );
            }
        }
    }
}