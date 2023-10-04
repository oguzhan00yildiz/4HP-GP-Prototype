using Global;
using UnityEngine;

namespace PlayerLogic
{
    public class Player : MonoBehaviour
    {
        public enum PlayerCharacter
        {
            Tank,
            Archer
        }
        public static Player instance { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public PlayerAttackHandler AttackHandler { get; private set; }
        public CameraMouseLook MouseLook { get; private set; }
        public Camera Camera { get; private set; }
        public int Health { get; private set; }
        public Canvas Canvas { get; private set; }

        public PlayerCharacter Character;

        private GameObject _nullObj;

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
            CameraShake shaker = instance.Camera.GetComponent<CameraShake>();
            shaker.Shake(0.25f, 0.1f);

            // Find damage overlay effect
            DamageScreenEffect dmgFx =
                Canvas.transform.Find("DamageOverlay")
                .GetComponent<DamageScreenEffect>();

            // Show flash if not null
            dmgFx?.ShowDamageFlash(false);
        }

        // Start is called before the first frame update
        private void Start()
        {
            Initialize();
        }

        public void AddUpgrade(StatUpgrade upgrade)
        {
            Debug.Log("Added upgrade to player");

            AttackHandler.ReceiveUpgrade(upgrade);
        }

        void Initialize()
        {
            Movement = GetComponent<PlayerMovement>();
            Movement.WalkSpeed = 
                Character == PlayerCharacter.Tank 
                    ? Const.Player.STATS_TANK_SPEED
                    : Const.Player.STATS_ARCHER_SPEED;

            AttackHandler = GetComponent<PlayerAttackHandler>();
            Camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
            MouseLook = Camera.GetComponent<CameraMouseLook>();
            Canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
            
        }
    }
}