using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Global
{
    public class CanvasManager : MonoBehaviour
    {
        public Canvas Canvas { get; private set; }
        [SerializeField] private Slider _waveProgressSlider;
        [SerializeField] private GameObject _upgradeScreen;
        private DamageScreenEffect _damageScreenEffect;

        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
            _damageScreenEffect = GetComponentInChildren<DamageScreenEffect>();
            _damageScreenEffect.Initialize();
        }

        public void DisplayDamageOverlay()
        {
            _damageScreenEffect?.ShowDamageFlash(true);
        }

        public void UpdateProgress(float newProgress)
        {
            float currentProgress = _waveProgressSlider.value;
            StartCoroutine(UpdateProgressBar(currentProgress, newProgress));
        }

        private IEnumerator UpdateProgressBar(float initialValue, float targetValue)
        {
            var elapsedTime = 0f;
            var duration = Const.Effects.PROGRESS_BAR_TRANSITION_DURATION;

            while (elapsedTime < duration)
            {
                _waveProgressSlider.value = Mathf.Lerp(initialValue, targetValue, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _waveProgressSlider.value = targetValue;
        }

        public void ResetProgressBar()
        {
            UpdateProgress(0);
        }

        public void ShowWaveCompletionScreen()
        {
            GameManager.Upgrades.ShowUpgradePanel();
        }

    }
}