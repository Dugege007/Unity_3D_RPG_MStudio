using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public enum SlotType
{
    Bag, Weapon, Armor, Action
}

public class SlotHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotType SlotType;
    public ItemUI itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            UseItem();
        }
    }

    /// <summary>
    /// 使用消耗品
    /// </summary>
    public void UseItem()
    {
        if (!itemUI.GetItem())
            return;

        if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.GetInventoryItemAmount() > 0)
        {
            GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableItemData.healthPoint);
            itemUI.Bag.items[itemUI.Index].amount--;

            // 检查任务物品更新进度
            QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName, -1);
        }
        UpdateItem();
    }

    /// <summary>
    /// 刷新物品
    /// </summary>
    public void UpdateItem()
    {
        switch (SlotType)
        {
            case SlotType.Bag:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.Weapon:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                //装备武器
                if (itemUI.Bag.items[itemUI.Index].itemData != null)
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                else
                    GameManager.Instance.playerStats.UnEquipWeapon();
                break;
            case SlotType.Armor:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;
            case SlotType.Action:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
            default:
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetupItemUI(item.itemData, item.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())
        {
            InventoryManager.Instance.tooltip.SetupTooltip(itemUI.GetItem());
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }
}
