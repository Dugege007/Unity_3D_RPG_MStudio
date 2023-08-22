using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public class OptionUI : MonoBehaviour
{
    public Text optionText;
    private Button thisBtn;
    private DialoguePiece currentPiece;

    private string nextPieceID;
    private bool takeQuest;

    private void Awake()
    {
        thisBtn = GetComponent<Button>();
        thisBtn.onClick.AddListener(OnOptionClicked);
    }

    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetID;
        takeQuest = option.takeQuest;
    }

    public void OnOptionClicked()
    {
        if (currentPiece.quest != null)
        {
            var newTask = new QuestManager.QuestTask
            {
                questData = Instantiate(currentPiece.quest)
            };

            if (takeQuest)
            {
                // 添加到任务列表
                // 判断是否已经有任务
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    // 判断是否完成，给予奖励
                    if (QuestManager.Instance.GetTask(newTask.questData).IsComplete)
                    {
                        newTask.questData.GiveRewards();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    // 没有任务，接受新任务
                    QuestManager.Instance.tasks.Add(newTask);
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;

                    // 检查背包中的物品
                    foreach (var requireItem in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }
            }
        }

        if (nextPieceID == "")
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
        }
        else
        {
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);
        }
    }
}
