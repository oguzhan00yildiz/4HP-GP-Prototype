using UnityEngine;

[CreateAssetMenu(fileName = "Archer Upgrade", menuName = "Upgrades/Archer Upgrade", order = 2)]
public class ArcherUpgrade : StatUpgrade
{
    [System.Serializable]
    public struct MultiShotUpgrade
    {
        public int ArrowCount;
        public float DisperseAngle;
        public float FireDelay;
    }

    [System.Serializable]
    public struct BurstShotUpgrade
    {
        public int ArrowCount;
        public float FireDelay;
        public float BurstDelay;
    }

    public bool GiveMultiShotUpgrade;
    public MultiShotUpgrade GivenMultiShotUpgrade;

    public bool GiveBurstShotUpgrade;
    public BurstShotUpgrade GivenBurstShotUpgrade;
}
