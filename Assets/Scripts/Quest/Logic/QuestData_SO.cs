using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions.Must;

/*
 * 创建人：杜
 * 功能说明：任务数据模板
 * 创建时间：
 */

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name;
        public int requireAmount;
        public int currentAmount;
    }

    // 任务名称
    public string questName;

    // 任务描述
    [TextArea]
    public string description;

    // 任务开始
    public bool isStarted;
    // 任务完成
    public bool isComplete;
    // 任务结束
    public bool isFinished;

    public List<QuestRequire> questRequires = new List<QuestRequire>();

    public List<InventoryItem> rewards = new List<InventoryItem>();

    public void CheckQuestProgress()
    {
        var finishRequires = questRequires.Where(r => r.currentAmount >= r.requireAmount);
        isComplete = finishRequires.Count() == questRequires.Count;

        if (isComplete)
        {
            Debug.Log("任务完成！");
        }
    }

    public void GiveRewards()
    {
        foreach (var r in rewards)
        {
            // 需要上交任务物品的情况
            if (r.amount < 0)   
            {
                int requireCount = Mathf.Abs(r.amount);

                // 背包当中有需要交的物品
                if (InventoryManager.Instance.QuestItemInBag(r.itemData) != null)   
                {
                    // 背包当中需要上交物品的数量刚好够或者不够的情况
                    if (InventoryManager.Instance.QuestItemInBag(r.itemData).amount <= requireCount)
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(r.itemData).amount;
                        InventoryManager.Instance.QuestItemInAction(r.itemData).amount = 0;
                        if (InventoryManager.Instance.QuestItemInAction(r.itemData) != null)
                        {
                            InventoryManager.Instance.QuestItemInAction(r.itemData).amount -= requireCount;
                        }
                    }
                    // 背包当中上交物品的数量充足
                    else
                    {
                        InventoryManager.Instance.QuestItemInBag(r.itemData).amount -= requireCount;
                    }
                }
                // 背包当中没有上交物品代表Action中一定满足了任务物品的数量
                else
                {
                    InventoryManager.Instance.QuestItemInAction(r.itemData).amount -= requireCount;
                }
            }
            // 正常获得的额外物品奖励添加到背包中
            else
            {
                InventoryManager.Instance.inventoryData.AddItem(r.itemData, r.amount);
            }

            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }

    // 当前任务中需要 收集/消灭 的目标名字列表
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();

        foreach (var require in questRequires)
        {
            targetNameList.Add(require.name);
        }

        return targetNameList;
    }
}
