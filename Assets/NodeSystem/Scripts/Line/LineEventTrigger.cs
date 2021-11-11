using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
namespace RuntimeNodeEditor
{
    public class LinePointerListener : MonoBehaviour, IPointerDownHandler
    {
        public string ConnectID;

        public void OnPointerDown(PointerEventData eventData)
        {
            SignalSystem.InvokeLineDown(ConnectID, eventData);
        }

    }

}