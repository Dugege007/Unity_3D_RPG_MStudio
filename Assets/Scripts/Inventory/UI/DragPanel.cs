using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public class DragPanel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTrans;
    private Canvas canvas;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        canvas = InventoryManager.Instance.GetComponent<Canvas>();
    }


    public void OnDrag(PointerEventData eventData)
    {
        rectTrans.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //Debug.Log(rectTrans.GetSiblingIndex());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTrans.SetSiblingIndex(2);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
