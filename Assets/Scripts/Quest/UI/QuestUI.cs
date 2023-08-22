using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    public GameObject questPanel;

    public ItemToolTip toolTip;

    private bool isOpen;


    [Header("Quest Name")]
    public RectTransform questListTransform;
    public QuestNameBtn questNameBtn;

    [Header("Text Content")]
    public Text questContentText;

    [Header("Requirement")]
    public RectTransform requireListTransform;
    public QuestRequirement requirement;

    [Header("Reward Panel")]
    public RectTransform rewardTransform;
    public ItemUI rewardUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;

            questPanel.SetActive(isOpen);
            if (isOpen == false)
                toolTip.gameObject.SetActive(false);

            questContentText.text = "";
            // 显示面板内容
            SetupQuestList();
        }
    }

    public void SetupQuestList()
    {
        ClearQuestList();

        foreach (var task in QuestManager.Instance.tasks)
        {
            var newTask = Instantiate(questNameBtn, questListTransform);
            newTask.SetupQuestNameBtn(task.questData);
            newTask.questContentText = questContentText;
        }
    }

    public void ClearQuestList()
    {
        foreach (Transform item in questListTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in requireListTransform)
        {
            Destroy(item.gameObject);
        }
    }

    public void SetupRequireList(QuestData_SO questData)
    {
        foreach (Transform item in requireListTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var require in questData.questRequires)
        {
            var q = Instantiate(requirement, requireListTransform);
            if (questData.isFinished)
                q.SetupRequirement(require.name, true);
            else
                q.SetupRequirement(require.name, require.requireAmount, require.currentAmount);
        }
    }

    public void SetupRewardItem(ItemData_SO itemData, int amount)
    {
        var item = Instantiate(rewardUI, rewardTransform);
        item.SetupItemUI(itemData, amount);
    }
}
