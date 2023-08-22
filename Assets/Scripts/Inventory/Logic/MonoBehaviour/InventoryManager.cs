using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * 创建人：Dugege
 * 功能说明：
 * 创建时间：
 */
public class DragData
{
    public SlotHolder originalHolder;
    public RectTransform originalParent;
}

public class InventoryManager : Singleton<InventoryManager>
{
    //TODO 最后添加模板用于保存数据
    [Header("Inventory Data")]
    public InventoryData_SO inventoryTempData;
    public InventoryData_SO inventoryData;
    public InventoryData_SO actionTempData;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentTempData;
    public InventoryData_SO equipmentData;

    [Header("Container")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;

    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;
    private bool isBagPanelOpen = false;
    private bool isStatsPanelOpen = false;

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;

    [Header("Tooltip")]
    public ItemToolTip tooltip;

    protected override void Awake()
    {
        base.Awake();

        //初始化背包数据
        if (inventoryTempData != null)
            inventoryData = Instantiate(inventoryTempData);
        if (actionTempData != null)
            actionData = Instantiate(actionTempData);
        if (equipmentTempData != null)
            equipmentData = Instantiate(equipmentTempData);
    }

    private void Start()
    {
        LoadData();

        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    private void Update()
    {
        //按下B键打开或关闭背包
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBagPanelOpen = !isBagPanelOpen;
            bagPanel.gameObject.SetActive(isBagPanelOpen);
        }
        //按下I键打开或关闭人物信息栏
        if (Input.GetKeyDown(KeyCode.I))
        {
            isStatsPanelOpen = !isStatsPanelOpen;
            statsPanel.gameObject.SetActive(isStatsPanelOpen);
        }

        UpdateStatsText(GameManager.Instance.playerStats.MaxHealth, GameManager.Instance.playerStats.attackData.minDamage, GameManager.Instance.playerStats.attackData.maxDamage);
    }

    public void SaveData()
    {
        SaveManager.Instance.Save(inventoryData,inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }

    public void UpdateStatsText(int health, int min, int max)
    {
        healthText.text = health.ToString();
        attackText.text = min + " - " + max;
    }

    #region 检查拖拽物品是否在每一个Slot范围内
    public bool CheckInventoryUI(Vector3 mousePosition)
    {
        for (int i = 0; i < inventoryUI.slotsHolders.Length; i++)
        {
            RectTransform rectTrans = inventoryUI.slotsHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTrans, mousePosition))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckActionUI(Vector3 mousePosition)
    {
        for (int i = 0; i < actionUI.slotsHolders.Length; i++)
        {
            RectTransform rectTrans = actionUI.slotsHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTrans, mousePosition))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckEquipmentUI(Vector3 mousePosition)
    {
        for (int i = 0; i < equipmentUI.slotsHolders.Length; i++)
        {
            RectTransform rectTrans = equipmentUI.slotsHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTrans, mousePosition))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    public void CheckQuestItemInBag(string questItemName)
    {
        foreach(var item in inventoryData.items)
        {
            if (item.itemData!=null)
            {
                if (item.itemData.itemName==questItemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
                }
            }
        }

        foreach(var item in actionData.items)
        {
            if (item.itemData!=null)
            {
                if (item.itemData.itemName==questItemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
                }
            }
        }
    }

    // 检测背包和快捷栏的物品
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i => i.itemData == questItem);
    }

    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i => i.itemData == questItem);
    }
}