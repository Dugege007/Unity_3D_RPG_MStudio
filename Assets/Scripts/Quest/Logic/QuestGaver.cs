using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：杜
 * 功能说明：任务提供者
 * 创建时间：
 */

[RequireComponent(typeof(DialogueController))]
public class QuestGaver : MonoBehaviour
{
    DialogueController controller;
    QuestData_SO currentQuest;

    // 第一次对话
    public DialogueData_SO startDialogue;
    // 任务进行中对话
    public DialogueData_SO progressDialogue;
    // 完成任务时的对话
    public DialogueData_SO completeDialogue;
    // 任务结束后的对话
    public DialogueData_SO finishDialogue;

    #region 获得任务状态
    public bool IsStarted
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
                return QuestManager.Instance.GetTask(currentQuest).IsStarted;

            return false;
        }
    }

    public bool IsComplete
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
                return QuestManager.Instance.GetTask(currentQuest).IsComplete;

            return false;
        }
    }

    public bool IsFinished
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuest))
                return QuestManager.Instance.GetTask(currentQuest).IsFinished;

            return false;
        }
    }
    #endregion

    private void Awake()
    {
        controller = GetComponent<DialogueController>();
    }

    private void Start()
    {
        controller.currentData = startDialogue;
        currentQuest = controller.currentData.GetQuest();
    }

    private void Update()
    {
        if (IsStarted)
        {
            if (IsComplete)
                controller.currentData = completeDialogue;
            else
                controller.currentData = progressDialogue;
        }

        if (IsFinished)
            controller.currentData = finishDialogue;
    }
}
