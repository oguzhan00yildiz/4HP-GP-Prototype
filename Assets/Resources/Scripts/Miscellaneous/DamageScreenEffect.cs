using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageScreenEffect : MonoBehaviour
{
    [SerializeField]
    private float _startAlpha;

    //private CameraShake _shaker;
    private Image _overlay;

    private bool _displayingEffect;

    public void Initialize()
    {
        _startAlpha = 0.5f;
        _overlay = GetComponent<Image>();
        _overlay.enabled = false;
    }

    public void ShowDamageFlash(bool shakeCamera)
    {
        // Find these here, if in start it fucks up..
        //_shaker = PlayerLogic.Player.instance.MouseLook.Shaker;
        _overlay = GetComponent<Image>();

        if (_displayingEffect)
            return;

        StartCoroutine(DamageEffect());
        if (!shakeCamera)
            return;

        //_shaker?.Shake(0.3f, 0.1f);
    }

    IEnumerator DamageEffect()
    {
        _displayingEffect = true;
        _overlay.enabled = true;
        Color orig = _overlay.color;
        for (float i = _startAlpha; i > 0; i -= Time.deltaTime * 2)
        {
            _overlay.color = new Color()
            {
                r = orig.r,
                g = orig.g,
                b = orig.b,
                a = i
            };
            yield return new WaitForEndOfFrame();
        }
        _overlay.enabled = false;
        _overlay.color = orig;
        _displayingEffect = false;
    }
}
