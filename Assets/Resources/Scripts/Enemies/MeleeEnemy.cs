using Global;
using UnityEngine;

namespace Enemies
{
    public sealed class MeleeEnemy : Enemy
    {
        public override void TryAttack()
        {
            float timeNow = Time.time;
            if(timeNow - TimeAtLastAttack < AttackSpeed)
                return;

            TimeAtLastAttack = timeNow;

            if (Vector2.Distance(transform.position, GameManager.Player.Position) <= AttackRange)
            {
                GameManager.Instance.PlayerHit(1);
            }
        }
    }
}