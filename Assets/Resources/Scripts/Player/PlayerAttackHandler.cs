using Assets.Resources.Scripts.Enemies;
using Assets.Resources.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PlayerLogic
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

        // This is a cached reference to a prefab
        private static GameObject _meleeEffect;
        [SerializeField]
        private float _attackRate = 0.15f;
        private float _timeAtLastAttack = 0;

        [SerializeField] private Vector3 _meleeOverlapExtents;

        [SerializeField] private int damageToEnemy;
        [SerializeField] private GameObject popUpTextPrefab;
        [SerializeField] private float minX,maxX;

        private void Awake()
        {
            AttackOrigin = transform.Find("AttackOrigin");
        }
        private void Start()
        {
            
            _attackAnimator = AttackOrigin.GetComponent<Animator>();
            _meleeEffect = Resources.Load<GameObject>("Prefabs/Effects/Attacks/MeleeSwing");
        }

        private void Update()
        {
            if (Player.instance.Movement.AttackHeld)
            {
                TryAttack();
                Attacking = true;
            }
            else
                Attacking = false;

            if(Input.GetKeyDown(KeyCode.K))
            {
                Player.instance.TakeDamage(0);
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

            // Check for enemies hit
            Vector2 overlapCenter = AttackOrigin.position;

            // Move center depending on player facing
            overlapCenter += Player.instance.Movement.FacingLeft ? Vector2.left : Vector2.right;
            var hitCols = Physics2D.OverlapBoxAll(overlapCenter, _meleeOverlapExtents, 0);

            for(int i = 0;  i < hitCols.Length; i++)
            {
                var col = hitCols[i];

                if (!col.CompareTag("Enemy"))
                    continue;

                var enemyData = col.GetComponent<EnemyData>();

                if (enemyData == null)
                    continue;

                // TODO: Actually change the damage given depending on attack type
                GameManager.Instance.EnemyHit(enemyData, damageToEnemy);

                // This is for Ossi to do
                DisplayDamageNumber(col,damageToEnemy);
            }
        }

        private void DisplayDamageNumber(Collider2D collider,int damageAmount)
        { 
            float rnd =Random.Range(minX,maxX);
            Vector3 newPos= new Vector3(rnd,0,0);
            GameObject popUpObject= Instantiate(popUpTextPrefab,collider.transform.position+newPos,collider.transform.rotation);
            popUpObject.transform.GetComponentInChildren<TextMeshPro>().text = damageAmount.ToString();
            Destroy(popUpObject.gameObject,.5f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(AttackOrigin.position, _meleeOverlapExtents);
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