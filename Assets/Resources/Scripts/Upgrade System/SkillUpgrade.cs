using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "General Upgrade", menuName = "Upgrades/General Upgrade", order = 0)]
public class SkillUpgrade : ScriptableObject
{
    public enum UpgradeType
    {
        General,
        Melee,
        Ranged,
        Magic
    }

    public UpgradeType TypeOfUpgrade;

    // NOTE: FACTOR IS MULTIPLIER, SO 1.5f = 50% INCREASE
    // AND 0.9f = 10% DECREASE

    public Sprite UpgradeIcon;
    public string UpgradeName;

    public bool AffectAttackSpeed;
    [Tooltip("Attack speed factor percentage. 20 = 20%")]
    public float AttackSpeedPercentageIncrease = 1;
    public float AttackSpeedMultiplier
    {
        get { return 1 + (AttackSpeedPercentageIncrease / 100); }
    }
    
    public bool AffectDamage;
    [Tooltip("Attack damage increase percentage. 20 = 20%")]
    public float AttackDamagePercentageIncrease = 1;

    public float AttackDamageMultiplier
    {
        get { return 1 + (AttackDamagePercentageIncrease / 100); }
    }

    public bool AffectMoveSpeed;

    public float MoveSpeedPercentageIncrease = 1;

    public float MoveSpeedMultiplier
    {
        get { return 1 + (MoveSpeedPercentageIncrease / 100); }
    }

    public bool AffectMaxHealth;
    public float MaxHealthPercentageIncrease = 1;

    public float MaxHealthMultiplier
    {
        get { return 1 + (MaxHealthPercentageIncrease / 100); }
    }

    public bool AffectCritDmg;
    public float CritDamagePercentageIncrease = 1;

    public float CritDmgMultiplier
    {
        get { return 1 + (CritDamagePercentageIncrease / 100); }
    }

    public bool AffectCritChance;
    public float CritDmgChancePercentageIncrease = 1;

    public float CritChanceMultiplier
    {
        get { return 1 + (CritDmgChancePercentageIncrease / 100); }
    }

    public bool AffectArmor;
    public float ArmorPercentageIncrease = 1;

    public float ArmorMultiplier
    {
        get { return 1 + (ArmorPercentageIncrease / 100); }
    }

    [Tooltip("Minimum wave for this upgrade to become available.")]
    public int MinimumWave = 1;

    [Tooltip("Optional text to display in the upgrade menu.")]
    public string FlavorText = "This is a general upgrade.";
}