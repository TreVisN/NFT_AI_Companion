using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIToolkitDemo
{
    public class FadeOutAndDestroy : MonoBehaviour
    {
        public float fadeDelay = 10f;
        public float alphaVal = 0;

        private Image _image;

        void Start()
        {
            _image = GetComponent<Image>();

            StartCoroutine(FadeTo(alphaVal, fadeDelay));
        }
    
        private IEnumerator FadeTo(float alphaV, float fadeDelay)
        {
            float alpha = _image.color.a;

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / this.fadeDelay)
            {
                Color newColor = new Color(_image.color.r, _image.color.g, _image.color.b,
                    Mathf.Lerp(alpha, alphaVal, t));
                _image.color = newColor;
                yield return null;
            }
            
            Destroy(gameObject);
        }
    }
}
