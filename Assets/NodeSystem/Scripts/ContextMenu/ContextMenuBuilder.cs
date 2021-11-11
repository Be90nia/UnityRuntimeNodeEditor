using System;
using System.Collections.Generic;
namespace RuntimeNodeEditor
{
    public class ContextMenuBuilder
    {
        private ContextMenuData m_Root;

        public ContextMenuBuilder()
        {
            m_Root = new ContextMenuData("");
        }

        public ContextMenuBuilder Add(string name)
        {
            BuildHierarchy(name);
            return this;
        }

        public ContextMenuBuilder Add(string name, Action callback)
        {
            BuildHierarchy(name).ContextMenuCallBack = callback;
            return this;
        }
        public ContextMenuData BuildHierarchy(string path)
        {
            path = path.Replace("\\", "/");
            string[] split = path.Split('/');
            ContextMenuData menu_item = m_Root;
            int index = 0;

            while (index < split.Length)
            {
                bool found = false;


                for (int i = 0; i < menu_item.Children.Count; ++i)
                {
                    if (menu_item.Children[i].Name == split[index])
                    {
                        menu_item = menu_item.Children[i];
                        ++index;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    var new_menu_item = new ContextMenuData(split[index]) { Parent = menu_item };
                    menu_item.Children.Add(new_menu_item);
                    menu_item = new_menu_item;
                    ++index;
                    found = true;
                }
            }
            return menu_item;
        }

        public ContextMenuBuilder AddChild(ContextMenuData child)
        {
            int lastIndex = m_Root.Children.Count - 1;
            var targetNode = m_Root.Children[lastIndex];
            child.Parent = targetNode;
            targetNode.Children.Add(child);
            return this;
        }

        public ContextMenuData Build()
        {
            return m_Root;
        }
    }


    public class ContextMenuData
    {
        public string Name;
        public ContextMenuData Parent;
        public List<ContextMenuData> Children;
        public Action ContextMenuCallBack;

        public bool IsRoot => Parent == null;
        public int Level => IsRoot ? 0 : Parent.Level + 1;

        public ContextMenuData(string name)
        {
            this.Name = name;
            Children = new List<ContextMenuData>();
        }
    }
}