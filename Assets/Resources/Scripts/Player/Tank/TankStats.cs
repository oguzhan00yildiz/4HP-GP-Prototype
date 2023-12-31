﻿using System.Collections.Generic;
using UnityEngine;

namespace PlayerLogic
{
    public class TankStats : PlayerStatInfo
    {
        public TankStats()
        {
            _tankUpgrades = new List<TankUpgrade>();
            Upgrades = new List<StatUpgrade>();
            InitialStats = new Dictionary<StatUpgrade.Stat, float>
                {
                    { StatUpgrade.Stat.AttackDamage, Const.Player.STATS_TANK_ATTACK_DAMAGE },
                    { StatUpgrade.Stat.AttackSpeed, Const.Player.STATS_TANK_ATTACK_SPEED },
                    { StatUpgrade.Stat.CritChance, Const.Player.STATS_TANK_CRIT_CHANCE },
                    { StatUpgrade.Stat.CritDamage, Const.Player.STATS_TANK_CRIT_DAMAGE },
                    { StatUpgrade.Stat.MaxHealth, Const.Player.STATS_TANK_HEALTH },
                    { StatUpgrade.Stat.MoveSpeed, Const.Player.STATS_TANK_SPEED },
                    { StatUpgrade.Stat.Armor, Const.Player.STATS_TANK_ARMOR },
                    { StatUpgrade.Stat.MeleeRange, Const.Player.MELEE_INITIAL_RANGE },
                    { StatUpgrade.Stat.Knockback, Const.Player.STATS_TANK_KNOCKBACK }
                };
        }

        private List<TankUpgrade> _tankUpgrades;

        public bool TryGetShieldStats(out TankUpgrade.ShieldUpgrade? upgrade)
        {
            upgrade = null;
            foreach (var tankUpgrade in _tankUpgrades)
            {
                if (tankUpgrade.GiveShieldUpgrade)
                {
                    upgrade = tankUpgrade.GivenShieldUpgrade;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetSpearStats(out TankUpgrade.SpearUpgrade? upgrade)
        {
            upgrade = null;
            foreach (var tankUpgrade in _tankUpgrades)
            {
                if (tankUpgrade.GiveSpearUpgrade)
                {
                    upgrade = tankUpgrade.GivenSpearUpgrade;
                    return true;
                }
            }
            return false;
        }

        public override void AddUpgrade(StatUpgrade upgrade)
        {
            switch (upgrade)
            {
                case null:
                    Debug.LogWarning("Tried to add null upgrade to tank upgrade list.");
                    return;
                case TankUpgrade tankUpgrade:
                    _tankUpgrades.Add(tankUpgrade);
                    break;
                default:
                    Upgrades.Add(upgrade);
                    break;
            }
        }
    }
}