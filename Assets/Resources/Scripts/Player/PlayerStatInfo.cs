using System.Collections.Generic;

namespace PlayerLogic
{
    public abstract class PlayerStatInfo : IPlayerStats
    {
        public List<StatUpgrade> Upgrades { get; set; }

        protected int LastKnownListLength;
        protected Dictionary<StatUpgrade.Stat, float> LastKnownStats;
        protected Dictionary<StatUpgrade.Stat, float> InitialStats;

        public float GetInitialStat(StatUpgrade.Stat stat)
        {
            return InitialStats[stat];
        }

        public float GetTotalStat(StatUpgrade.Stat stat)
        {
            // If the list of upgrades is the same length as last time, don't recalculate
            if (LastKnownListLength == Upgrades.Count && LastKnownListLength != 0)
                return LastKnownStats[stat];

            float startingValue = GetInitialStat(stat);

            float total = startingValue;

            foreach (var upgrade in Upgrades)
            {
                var statChanges = upgrade.StatChanges;

                if (statChanges.Count == 0)
                    continue;

                foreach (var change in statChanges)
                {
                    if (change.AffectedStat != stat)
                        continue;

                    total *= change.Multiplier;
                }
            }

            return total;
        }

        public abstract void AddUpgrade(StatUpgrade upgrade);
    }
}