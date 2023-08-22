using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

[System.Serializable]
public class LootItem
{
    public GameObject item;

    [Range(0, 1)]
    public float weight;
}

public class LootSpawner : MonoBehaviour
{
    public LootItem[] lootItems;

    public void Spawnloot()
    {
        float currentValue = Random.value;
        foreach (var item in lootItems)
        {
            if (currentValue <= item.weight)
            {
                GameObject obj = Instantiate(item.item);
                obj.transform.position = transform.position + Vector3.up * 2;
                break;
            }
        }
    }
}
