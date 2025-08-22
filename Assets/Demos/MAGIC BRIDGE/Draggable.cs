using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 Pos = Camera.main.ScreenToWorldPoint(eventData.position);
        Pos.z = 0;
        transform.position = Pos;
    }

}
