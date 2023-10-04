using System.Collections.Generic;
using UnityEngine;

namespace PlayerLogic
{
    public class ArcherStats : PlayerStatInfo
    {
        public ArcherStats()
        {
            _archerUpgrades = new List<ArcherUpgrade>();
            Upgrades = new List<StatUpgrade>();
            InitialStats = new Dictionary<StatUpgrade.Stat, float>
                {
                    { StatUpgrade.Stat.AttackDamage, Const.Player.STATS_ARCHER_ATTACK_DAMAGE },
                    { StatUpgrade.Stat.AttackSpeed, Const.Player.STATS_ARCHER_ATTACK_SPEED },
                    { StatUpgrade.Stat.CritChance, Const.Player.STATS_ARCHER_CRIT_CHANCE },
                    { StatUpgrade.Stat.CritDamage, Const.Player.STATS_ARCHER_CRIT_DAMAGE },
                    { StatUpgrade.Stat.MaxHealth, Const.Player.STATS_ARCHER_HEALTH },
                    { StatUpgrade.Stat.MoveSpeed, Const.Player.STATS_ARCHER_SPEED },
                    { StatUpgrade.Stat.Armor, Const.Player.STATS_ARCHER_ARMOR }
                };
        }

        private List<ArcherUpgrade> _archerUpgrades;
        public override void AddUpgrade(StatUpgrade upgrade)
        {
            switch (upgrade)
            {
                case null:
                    Debug.LogWarning("Tried to add null upgrade to archer upgrade list.");
                    return;
                case ArcherUpgrade archerUpgrade:
                    _archerUpgrades.Add(archerUpgrade);
                    break;
                default:
                    Upgrades.Add(upgrade);
                    break;
            }
        }
    }
}