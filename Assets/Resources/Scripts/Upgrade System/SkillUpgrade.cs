using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "General Upgrade", menuName = "Upgrades/General Upgrade", order = 0)]
public class SkillUpgrade : ScriptableObject
{
    [System.Serializable]
    public struct Effect
    {
        public enum EffectType
        {
            AttackSpeed,
            AttackDamage,
            MoveSpeed,
            MaxHealth,
            CritDamage,
            CritChance,
            Armor
        }
        public EffectType Type;
        [Tooltip("Percentage increase. 20 = 20%")]
        public float Difference;
        public readonly float Multiplier => 1 + Mathf.Abs(Difference) / 100;
    }

    public List<Effect> Effects;

    public Sprite UpgradeIcon;
    public string UpgradeName;

    [Tooltip("Minimum wave for this upgrade to become available.")]
    public int MinimumWave = 1;

    [Tooltip("Optional text to display in the upgrade menu.")]
    public string FlavorText = "This is a general upgrade.";
}