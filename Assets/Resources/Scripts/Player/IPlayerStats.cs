namespace PlayerLogic
{
    public interface IPlayerStats
    {
        float GetInitialStat(StatUpgrade.Stat stat);
        float GetTotalStat(StatUpgrade.Stat stat);
        void AddUpgrade(StatUpgrade upgrade);
    }
}