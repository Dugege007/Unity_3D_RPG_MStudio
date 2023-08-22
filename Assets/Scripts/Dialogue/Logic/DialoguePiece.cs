using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

[System.Serializable]
public class DialoguePiece
{
    // 创建ID，可以识别语句的序号
    public string ID;
    public Sprite image;
    [TextArea]
    public string text;

    public QuestData_SO quest;

    [HideInInspector]
    public bool canExpand;

    public List<DialogueOption> options = new List<DialogueOption>();
}
