using System;
using UnityEngine;

namespace Enemies
{
    [System.Serializable]
    public class EnemyData
    {
        [Header("Attributes")]
        public int enemyCost;
        public int unlockingWave;
        public int lastSpawningWave;
        public float maxPercentageInWave;
    }
}