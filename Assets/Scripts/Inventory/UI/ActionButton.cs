using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;
    private SlotHolder currentSlotHolder;

    private void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(actionKey) && currentSlotHolder.itemUI.GetItem())
        {
            currentSlotHolder.UseItem();
        }
    }
}
