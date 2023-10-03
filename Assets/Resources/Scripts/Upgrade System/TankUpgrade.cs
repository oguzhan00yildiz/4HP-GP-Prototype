using UnityEngine;

[CreateAssetMenu(fileName = "Tank Upgrade", menuName = "Upgrades/Tank Upgrade", order = 1)]
public class TankUpgrade : SkillUpgrade
{
    [System.Serializable]
    public struct ShieldUpgrade
    {
        public int NumHits;
    }

    [System.Serializable]
    public struct SpearUpgrade
    {
        public float Range;
    }

    public bool GiveShieldUpgrade;
    public ShieldUpgrade GivenShieldUpgrade;
    public bool GiveSpearUpgrade;
    public SpearUpgrade GivenSpearUpgrade;
}
