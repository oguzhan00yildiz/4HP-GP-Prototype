using PlayerLogic;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Scripts.Global
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private Slider _waveProgressSlider;
        [SerializeField] private GameObject _upgradeScreen;
        private float progressBarTransitionDuration = 0.2f;

        #region Singleton
        private static CanvasManager _instance;
        public static CanvasManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<CanvasManager>();

                if (_instance != null) return _instance;
                var obj = new GameObject("CanvasManager");
                _instance = obj.AddComponent<CanvasManager>();

                return _instance;
            }
        }
        #endregion

        private void OnEnable()
        {
            if (_instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Awake()
        {
        }

        public void UpdateProgress(float newProgress)
        {
            float currentProgress = _waveProgressSlider.value;
            StartCoroutine(UpdateProgressBar(currentProgress, newProgress));
        }

        private IEnumerator UpdateProgressBar(float initialValue, float targetValue)
        {
            var elapsedTime = 0f;

            while (elapsedTime < progressBarTransitionDuration)
            {
                _waveProgressSlider.value = Mathf.Lerp(initialValue, targetValue, elapsedTime / progressBarTransitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _waveProgressSlider.value = targetValue;
        }

        public void ResetProgressBar()
        {
            UpdateProgress(0);
        }

        public void OnWaveCompleted()
        {
            _upgradeScreen.SetActive(true);
           
        }

    }
}