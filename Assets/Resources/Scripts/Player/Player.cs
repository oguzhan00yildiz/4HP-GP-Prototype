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
        public Camera Camera;
        public int Health { get; private set; }

        // Make sure singleton is functional to access public variables of
        // this player instance outside this class (such as in PlayerMovement, PlayerAttackHandler...)
        private void OnEnable()
        {
            if (instance != this && instance != null)
                Destroy(this);
            else
                instance = this;
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
        }
    }
}