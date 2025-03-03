using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkItem : MonoBehaviour
{
    private SpriteRenderer m_sprite;

    public static bool isBlinking = false;

    public bool startBlinking = false;

    private void Start()
    {
        isBlinking = false;
        m_sprite = GetComponent<SpriteRenderer>();
        if (startBlinking)
        {
            StartBlink();
        }
    }

    public void StartBlink()
    {
        if (isBlinking)
        {
            return;
        }


        isBlinking = true;
        StartCoroutine(BlinkItemCoroutine());
    }

    public void StopBlink()
    {
        isBlinking = false;
    }

    private IEnumerator BlinkItemCoroutine()
    {
        while (isBlinking)
        {
            m_sprite.enabled = !m_sprite.enabled;
            yield return new WaitForSeconds(0.2f);
        }

        m_sprite.enabled = true;
    }
}
