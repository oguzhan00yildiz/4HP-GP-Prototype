using Assets.Resources.Scripts.Enemies;
using TMPro;
using UnityEngine;

namespace Assets.Resources.Scripts.Global
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _waveText;
        [SerializeField] private TMP_Text enemyCounter;

        private void Update()
        {
            _waveText.text = "Wave " + FindAnyObjectByType<SpawnerManager>().currentWave.ToString();
            enemyCounter.text = "Enemy killed: " + GameManager.Instance.killedEnemies.ToString();
        }
    }
}