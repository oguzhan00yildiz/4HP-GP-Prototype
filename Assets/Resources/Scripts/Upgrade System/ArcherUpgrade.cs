using UnityEngine;

[CreateAssetMenu(fileName = "Archer Upgrade", menuName = "Upgrades/Archer Upgrade", order = 2)]
public class ArcherUpgrade : SkillUpgrade
{
    [System.Serializable]
    public struct MultiShotUpgrade
    {
        public int Arrows;
        public float DisperseAngle;
    }

    [System.Serializable]
    public struct BurstShotUpgrade
    {
        public int ArrowCount;
        public float BurstDelay;
    }

    public bool GiveMultiShotUpgrade;
    public MultiShotUpgrade GivenMultiShotUpgrade;

    public bool GiveBurstShotUpgrade;
    public BurstShotUpgrade GivenBurstShotUpgrade;
}
