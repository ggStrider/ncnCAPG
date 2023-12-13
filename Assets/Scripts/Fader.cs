using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ncn.Mechanics.UI
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeTime;

        [SerializeField] private bool _unFadeOnStart;

        private void Start()
        {
            if (_unFadeOnStart) 
                StartCoroutine(UnFade());
        }

        private IEnumerator Fade()
        {
            var elaspedTime = 0f;
            var fadeTimeDelta = _fadeTime * 100;
            var newColor = _fadeImage.color;

            while (elaspedTime < fadeTimeDelta)
            {
                newColor.a += elaspedTime / fadeTimeDelta;
                _fadeImage.color = newColor;

                elaspedTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator UnFade()
        {
            var elaspedTime = 0f;
            var fadeTimeDelta = _fadeTime * 100;
            var newColor = _fadeImage.color;

            while (elaspedTime < fadeTimeDelta)
            {
                newColor.a -= elaspedTime / fadeTimeDelta;
                _fadeImage.color = newColor;

                elaspedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
