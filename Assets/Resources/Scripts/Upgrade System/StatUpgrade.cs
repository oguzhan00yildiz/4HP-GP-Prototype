using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "General Upgrade", menuName = "Upgrades/General Upgrade", order = 0)]
public class StatUpgrade : ScriptableObject
{
    [System.Serializable]
    public struct StatChange
    {
        public Stat AffectedStat;

        [Tooltip("Use this to trigger custom events")]
        public string TriggerName;

        [Tooltip("Percentage increase. 20 = +20%")]
        public float Difference;

        public readonly float Multiplier => 1.0f + (Difference / 100);
    }

    public enum Stat
    {
        AttackSpeed,
        MeleeRange,
        AttackDamage,
        MoveSpeed,
        MaxHealth,
        CritDamage,
        CritChance,
        Knockback,
        Armor,
        Other
    }

    public List<StatChange> StatChanges;

    public Sprite UpgradeIcon;
    public string UpgradeName;

    [Tooltip("Minimum wave for this upgrade to become available.")]
    public int MinimumWave = 1;

    [Tooltip("Optional text to display in the upgrade menu.")]
    public string FlavorText = "This is a general upgrade.";
}