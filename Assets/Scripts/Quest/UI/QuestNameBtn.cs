using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public class QuestNameBtn : MonoBehaviour
{
    public Text questNameText;
    public QuestData_SO currentData;
    public Text questContentText;

    private Button questNameBtn;

    private void Awake()
    {
        questNameBtn=GetComponent<Button>();
        questNameBtn.onClick.AddListener(UpdateQuestContent);
    }

    private void UpdateQuestContent()
    {
        questContentText.text = currentData.description;
        QuestUI.Instance.SetupRequireList(currentData);

        foreach(Transform item in QuestUI.Instance.rewardTransform)
        {
            Destroy(item.gameObject);
        }

        foreach(var item in currentData.rewards)
        {
            QuestUI.Instance.SetupRewardItem(item.itemData, item.amount);
        }
    }

    public void SetupQuestNameBtn(QuestData_SO questData)
    {
        currentData = questData;

        if (questData.isComplete)
            questNameText.text = questData.questName + "完成";
        else
            questNameText.text = questData.questName;
    }
}
