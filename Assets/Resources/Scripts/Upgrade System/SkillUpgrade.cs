using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "General Upgrade", menuName = "Upgrades/General Upgrade", order = 0)]
public class SkillUpgrade : ScriptableObject
{
    public Sprite UpgradeIcon;
    public string UpgradeName;

    public bool AffectAttackSpeed;
    [Tooltip("Attack speed factor percentage. 1.15 = 115%")]
    [Range(-1, 3)]
    public float AttackSpeedFactor = 1;
    
    public bool AffectDamage;
    [Tooltip("Attack damage increase percentage. 1.15 = 115%")]
    [Range(-1, 3)]
    public float AttackDamageFactor = 1;

    public bool AffectMoveSpeed;
    [Tooltip("Attack damage increase percentage. 1.15 = 115%")]
    [Range(-1, 3)]
    public float MoveSpeedFactor = 1;

    [Tooltip("Minimum wave for this upgrade to become available.")]
    public int MinimumWave = 1;

    [Tooltip("Optional text to display in the upgrade menu.")]
    public string FlavorText = "This is a general upgrade.";
}