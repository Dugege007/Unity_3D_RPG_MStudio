using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    //消耗品、武器、装备
    Useable, Weapon, Armor
}

[CreateAssetMenu (fileName ="New Item",menuName ="Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;

    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;

    [TextArea]
    public string description = "";
    public bool stackable;

    [Header("Useable Item")]
    public UseableItemData_SO useableItemData;

    [Header("Weapon")]
    public GameObject weaponPrefab;

    public AttackData_SO weaponData;
    public AnimatorOverrideController weaponAnimator;
}