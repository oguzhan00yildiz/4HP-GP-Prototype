using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public static Player instance { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public PlayerAttackHandler AttackHandler { get; private set; }
        public CameraMouseLook MouseLook { get; private set; }
        public Camera Camera { get; private set; }
        public int Health { get; private set; }
        public Canvas Canvas { get; private set; }

        // Make sure singleton is functional to access public variables of
        // this player instance outside this class (such as in PlayerMovement, PlayerAttackHandler...)
        private void OnEnable()
        {
            if (instance != this && instance != null)
                Destroy(this);
            else
                instance = this;
        }

        public void TakeDamage(int amount)
        {
            // Shake camera
            CameraShake shaker = Player.instance.Camera.GetComponent<CameraShake>();
            shaker.Shake(0.25f, 0.1f);

            // Find damage overlay effect
            DamageScreenEffect dmgFx =
                Canvas.transform.Find("GameView/DamageOverlay")
                .GetComponent<DamageScreenEffect>();

            // Show flash if not null
            dmgFx?.ShowDamageFlash(true);
        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            Movement = GetComponent<PlayerMovement>();
            AttackHandler = GetComponent<PlayerAttackHandler>();
            Camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
            MouseLook = Camera.GetComponent<CameraMouseLook>();
            Canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        }
    }
}