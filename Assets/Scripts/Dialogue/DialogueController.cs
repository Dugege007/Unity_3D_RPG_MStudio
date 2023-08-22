using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;
    bool canTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    private void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.F))
        {
            OpenDialogue();
        }
    }

    private void OpenDialogue()
    {
        // 打开UI面板
        // 传输对话内容信息
        DialogueUI.Instance.UpdateDialogueData(currentData);
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            canTalk = false;
        }
    }
}
