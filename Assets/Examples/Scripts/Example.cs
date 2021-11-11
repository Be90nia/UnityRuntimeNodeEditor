using System.Collections;
using System.Collections.Generic;
using RuntimeNodeEditor;
using UnityEngine;
using UnityEngine.EventSystems;


public class Example : MonoBehaviour
{
    public NodeEditor NodeEditor;
    private ContextMenuData m_GraphMenuData;
    public List<NodeCreateData> CreateDatas;
    public string FeildPath = "/NodeSystem/Resources/Graphs/graph.json";

    void Start()
    {
        FeildPath = Application.dataPath + FeildPath;
        NodeEditor.SaveFilePath(FeildPath);
        GraphPointerListener.GraphPointerClickEvent += OnGraphPointerClick;
    }
    //  event handlers
    private void OnGraphPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Right:
                DisplayGraphContextMenu();
                break;
            case PointerEventData.InputButton.Left:
                NodeEditor.CloseContextMenu();
                break;
        }
    }
    private void DisplayGraphContextMenu()
    {
        var contextMenuBuilder = new ContextMenuBuilder();
        foreach (var data in CreateDatas)
            contextMenuBuilder.Add(data.NodeMenuName,
                () => { NodeEditor.CreateNode(data.NodePrefabsPath); });

        m_GraphMenuData = contextMenuBuilder
            .Add("graph/load", LoadGraph)
            .Add("graph/save", SaveGraph)
            .Build();

        NodeEditor.ShowMenu(m_GraphMenuData);
    }



    public void SaveGraph()
    {
        NodeEditor.SaveGraph();
    }

    public void LoadGraph()
    {
        NodeEditor.LoadGraph();
    }
}
