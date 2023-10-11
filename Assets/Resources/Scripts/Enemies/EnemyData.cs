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
        private ulong _enemyId;
        private bool _hasId = false;
        public void SetId(ulong id)
        {
            if (_hasId)
                throw new Exception("Enemy already has an ID!");

            _hasId = true;
            _enemyId = id;
        }

        public ulong GetId()
        {
            if (!_hasId)
                throw new Exception("Enemy doesn't have an ID!");
            return _enemyId;
        }
    }
}