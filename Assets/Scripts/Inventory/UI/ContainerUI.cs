using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：Dugege
 * 功能说明：
 * 创建时间：
 */

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slotsHolders;

    public void RefreshUI()
    {
        for (int i = 0; i < slotsHolders.Length; i++)
        {
            slotsHolders[i].itemUI.Index = i;
            slotsHolders[i].UpdateItem();
        }
    }
}