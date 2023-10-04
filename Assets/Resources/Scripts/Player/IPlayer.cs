namespace PlayerLogic
{
    // Player interface
    public interface IPlayer
    {
        public enum PlayerCharacter
        {
            Tank,
            Archer
        }
        void Attack();
        void TakeDamage(int amount);
        void ReceiveUpgrade(StatUpgrade upgrade);
        void Initialize();
    }
}