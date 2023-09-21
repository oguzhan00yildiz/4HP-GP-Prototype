using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;

    private void Start()
    {

    }

    private void Update()
    {
        waveText.text = "Wave " + FindAnyObjectByType<SpawnerManager>().currentWave.ToString();
    }
}
