using System.Collections;
using System.Collections.Generic;
using RuntimeNodeEditor;
using UnityEngine;


public class Example : MonoBehaviour
{
    private RuntimeNodeEditor.ContextMenu _contextMenu;
    private ContextMenuData _graphCtx;
    // Start is called before the first frame update
    void Start()
    {
        _contextMenu = Utility.CreatePrefab<RuntimeNodeEditor.ContextMenu>("Prefabs/ContextMenu", contextMenuContainer);
        _contextMenu.Init();
        CloseContextMenu();
    }



    // Update is called once per frame
    void Update()
    {
        
    }


}
