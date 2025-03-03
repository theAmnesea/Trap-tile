using System.Collections;
using UnityEngine;

public class FadeItem : MonoBehaviour
{
    private SpriteRenderer m_sprite;

    public static bool isFading = false;

    public float fadeTime = 1f;

    public bool fadeToSolidOnStart = false;

    private void Start()
    {
        isFading = false;
        m_sprite = GetComponent<SpriteRenderer>();

        if (fadeToSolidOnStart)
        {
            Color col = m_sprite.color;
            col.a = 0f;
            m_sprite.color = col;

            if (isFading)
            {
                return;
            }

            isFading = true;

            StartCoroutine(TransparentToSolid(2f));
        }
    }

    public void MakeTransparent()
    {
        if (isFading)
        {
            return;
        }

        isFading = true;
        StartCoroutine(SolidToTransparent());
    }

    public void MakeSolid()
    {
        if (isFading)
        {
            return;
        }

        isFading = true;
        StartCoroutine(TransparentToSolid());
    }

    private IEnumerator SolidToTransparent(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        Color color = m_sprite.color;
        float startAlpha = 1f;
        float endAlpha = 0f;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            m_sprite.color = color;
            yield return null;
        }

        color.a = endAlpha;
        m_sprite.color = color;

        isFading = false;
    }

    private IEnumerator TransparentToSolid(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        Color color = m_sprite.color;
        float startAlpha = 0f;
        float endAlpha = 1f;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            m_sprite.color = color;
            yield return null;
        }

        color.a = endAlpha;
        m_sprite.color = color;

        isFading = false;
    }
}
