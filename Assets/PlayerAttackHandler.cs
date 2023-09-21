using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAttackHandler : MonoBehaviour
    {
        [HideInInspector]
        public Transform AttackOrigin;

        // To be used in tandem with movement script
        // so it doesn't flip the player around when trying to attack
        [HideInInspector]
        public bool Attacking { get; private set; }

        private Animator _attackAnimator;
        private static GameObject _meleeEffect;
        [SerializeField]
        private float _attackRate = 0.15f;
        private float _timeAtLastAttack = 0;

        private void Start()
        {
            AttackOrigin = transform.Find("AttackOrigin");
            _attackAnimator = AttackOrigin.GetComponent<Animator>();
            _meleeEffect = Resources.Load<GameObject>("Prefabs/Effects/Attacks/MeleeSwing");
        }

        private void Update()
        {
            if (Player.instance.Movement.AttackHeld)
            {
                TryAttack();
            }
        }
        void TryAttack()
        {
            // Calculate whether we can attack now
            float timeNow = Time.time;
            float timeSinceLastAttack = timeNow - _timeAtLastAttack;

            // If we attacked too short a duration ago, return (do not proceed)
            if (timeSinceLastAttack < _attackRate)
                return;

            _timeAtLastAttack = Time.time;

            // Turn player in the direction he is attacking and set flag to true
            // (when mouse position x is less than the screen's width split in half, the mouse is on the left side)
            if (Input.mousePosition.x < Screen.width / 2)
            {
                Player.instance.Movement.SetFacing(left: true);
                CreateMeleeEffect(flipX: true);
            }
            else
            {
                // Only change it if we're facing left currently; otherwise leave it as is
                if (Player.instance.Movement.FacingLeft)
                    Player.instance.Movement.SetFacing(left: false);

                CreateMeleeEffect(flipX: false);
            }
            Attacking = true;
        }

        void CreateMeleeEffect(bool flipX)
        {
            GameObject fx = Instantiate(_meleeEffect, AttackOrigin.position, Quaternion.identity);
            Vector3 scale = fx.transform.localScale;
            if (flipX)
            {
                scale.x *= -1;
                fx.transform.localScale = scale;
            }
        }
    }
}