using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemyCounter;

    private void Start()
    {

    }

    private void Update()
    {
        waveText.text = "Wave " + FindAnyObjectByType<SpawnerManager>().currentWave.ToString();
        enemyCounter.text = "Enemy killed: " + GameManager.Instance.killedEnemies.ToString();
    }
}
