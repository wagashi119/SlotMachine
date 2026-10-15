using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JackpotEffect : MonoBehaviour
{
    [SerializeField] private Image effectImage;

    public void PlayEffect()
    {
        StopAllCoroutines();
        StartCoroutine(ShowEffect());
    }

    private IEnumerator ShowEffect()
    {
        effectImage.gameObject.SetActive(true);

        Color color = effectImage.color;
        color.a = 1f;
        effectImage.color = color;

        effectImage.transform.localScale = Vector3.one * 0.5f;

        float duration = 1.2f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            effectImage.transform.localScale =
                Vector3.one * Mathf.Lerp(0.5f, 2f, t);

            color.a = 1f - t;
            effectImage.color = color;

            yield return null;
        }

        effectImage.gameObject.SetActive(false);
    }
}