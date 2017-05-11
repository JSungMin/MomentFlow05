using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontWall : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private const float deltaTm = 0.015f;
    private float nowTm = 0.0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TransparentSlowly()
    {
        StartCoroutine(TransparentSlowlyCo(0.3f));
    }

    public void UnTransparentSlowly()
    {
        StartCoroutine(UnTransparentSlowlyCo(0.3f));
    }

    private IEnumerator TransparentSlowlyCo(float excuteTm)
    {
        nowTm = 0.0f;
        while (nowTm < excuteTm)
        {
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - nowTm / excuteTm);
            nowTm += deltaTm;
            yield return new WaitForSeconds(deltaTm);
        }
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    private IEnumerator UnTransparentSlowlyCo(float excuteTm)
    {
        nowTm = 0.0f;
        while (nowTm < excuteTm)
        {
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, nowTm / excuteTm);

            nowTm += deltaTm;
            yield return new WaitForSeconds(deltaTm);
        }
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
}
