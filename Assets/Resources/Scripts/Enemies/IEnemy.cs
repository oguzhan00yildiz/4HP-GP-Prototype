using UnityEngine;

namespace Enemies
{
    public interface IEnemy
    {
        void TryAttack();
        void TakeDamage(int amount, Vector2? damageOrigin = null, float knockbackAmount = 0);
        void Die();
        void Initialize();
    }
}