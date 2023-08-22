using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

[Serializable]
public class InventoryItem
{
    public ItemData_SO itemData;
    public int amount;
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(ItemData_SO newItemData, int amount)
    {
        bool found = false;

        //判断物品是否可堆叠
        if (newItemData.stackable)
        {
            //遍历背包里每个物品
            foreach (var item in items)
            {
                //找到相同物品
                if (item.itemData == newItemData)
                {
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }

        //遍历背包
        for (int i = 0; i < items.Count; i++)
        {
            //如果有空位
            if (items[i].itemData == null && !found)
            {
                items[i].itemData = newItemData;
                items[i].amount = amount;
                break;
            }
        }
    }
}