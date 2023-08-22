using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public class ShowToolTip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public ItemUI currentItemUI;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        QuestUI.Instance.toolTip.gameObject.SetActive(true);
        QuestUI.Instance.toolTip.SetupTooltip(currentItemUI.currentItemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        QuestUI.Instance.toolTip.gameObject.SetActive(false);
    }
}
