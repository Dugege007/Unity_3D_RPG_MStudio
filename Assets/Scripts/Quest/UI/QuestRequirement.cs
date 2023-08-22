using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 创建人：杜
 * 功能说明：任务详细信息
 * 
 * 用于更新任务目标和进度
 * 
 * 创建时间：
 */

public class QuestRequirement : MonoBehaviour
{
    private Text requireNameText;
    private Text progressNumber;

    private void Awake()
    {
        requireNameText = GetComponent<Text>();
        progressNumber = transform.GetChild(0).GetComponent<Text>();
    }

    public void SetupRequirement(string name, int amount, int currentAmount)
    {
        requireNameText.text = name;
        progressNumber.text = currentAmount + "/" + amount;
    }

    public void SetupRequirement(string name, bool isFinished)
    {
        if (isFinished)
        {
            requireNameText.text = name;
            progressNumber.text = "完成";
            requireNameText.color = Color.gray;
        }
    }
}
