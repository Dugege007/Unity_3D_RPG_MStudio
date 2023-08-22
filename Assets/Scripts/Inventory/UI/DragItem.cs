using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private ItemUI currentItemUI;
    private SlotHolder currentHolder;
    private SlotHolder targetHolder;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //开始拖拽，记录原始信息
        InventoryManager.Instance.currentDrag = new DragData();
        InventoryManager.Instance.currentDrag.originalHolder = currentHolder;
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)currentHolder.transform;
        //设置父级，保持在最上层
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //跟随鼠标位置
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //结束拖拽，放下物品、交换数据
        //是否指向UI物品
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (IsPointerOnContainerUI(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                    Debug.Log(eventData.pointerEnter.gameObject + " GetComponentInParent");
                }

                //如果不是拖拽回同一个格子
                if (targetHolder != InventoryManager.Instance.currentDrag.originalHolder)
                {
                    switch (targetHolder.SlotType)
                    {
                        case SlotType.Bag:
                            SwapItem();
                            break;
                        case SlotType.Weapon:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                            {
                                SwapItem();
                            }
                            break;
                        case SlotType.Armor:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                                SwapItem();
                            break;
                        case SlotType.Action:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                                SwapItem();
                            break;
                        default:
                            break;
                    }
                }

                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);
        RectTransform rectTrans = transform as RectTransform;
        rectTrans.offsetMax = -Vector2.one * 5;
        rectTrans.offsetMin = Vector2.one * 5;
    }

    /// <summary>
    /// 交换物体
    /// </summary>
    public void SwapItem()
    {
        //目标物体
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];
        //临时物体
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];
        //判断是否相同物体
        bool isSameItem = tempItem.itemData == targetItem.itemData;

        if (isSameItem && targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        else
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;
        }
    }

    public bool IsPointerOnContainerUI(Vector3 mousePosition)
    {
        return InventoryManager.Instance.CheckInventoryUI(mousePosition) ||
            InventoryManager.Instance.CheckActionUI(mousePosition) ||
            InventoryManager.Instance.CheckEquipmentUI(mousePosition);
    }
}