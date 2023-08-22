using RPGCharacterAnims.Lookups;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//由于游戏对象无法直接挂载ScriptableObject，需要该脚本链接
public class CharacterStats : MonoBehaviour
{
    //攻击时更新血条
    public event Action<int, int> UpdateHealthBarOnAttack;
    //模板数据
    public CharacterData_SO templateData;
    //使用的数据
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    private AttackData_SO baseAttackData;
    private RuntimeAnimatorController baseAnimator;

    [Header("Weapon")]
    public Transform weaponSlot;

    [HideInInspector]//在Inspector中隐藏
    //是否暴击
    public bool isCritical;

    private void Awake()
    {
        if (templateData)
            characterData = Instantiate(templateData);

        baseAttackData = Instantiate(attackData);
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 1);//保证受到伤害最小值为1
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);//保证血最小值为0

        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        // Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        // 经验Update
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    public void TakeDamage(int damage, CharacterStats defener)
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 1);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }
    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击：" + coreDamage);
        }
        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    public void EquipWeapon(ItemData_SO weapon)
    {
        if (weapon.weaponPrefab != null)
        {
            Instantiate(weapon.weaponPrefab, weaponSlot);
        }

        // 更新属性
        attackData.ApplyWeaponData(weapon.weaponData);
        // 切换动画
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
    }

    public void UnEquipWeapon()
    {
        if (weaponSlot.transform.childCount > 0)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyWeaponData(baseAttackData);

        // 切换动画
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }
    #endregion

    #region Apply Data Change
    public void ApplyHealth(int amount)
    {
        if (CurrentHealth + amount <= MaxHealth)
        {
            CurrentHealth += amount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }
    #endregion
}