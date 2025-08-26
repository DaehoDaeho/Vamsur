using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;

    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine routine;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void PlayFlash()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(CoFlash());
    }

    IEnumerator CoFlash()
    {
        //sr.color = flashColor;
        //yield return new WaitForSeconds(flashDuration);
        //sr.color = originalColor;
        //routine = null;

        Color before = sr.color;            // 플래시 전 색을 기억
        sr.color = flashColor;              // 번쩍
        yield return new WaitForSeconds(flashDuration);
        sr.color = before;                  // 바로 직전 색으로 복구(엘리트 색 유지됨)
        routine = null;
    }

    // DamageFlash.cs 에 추가
    public void SetBaseColor(Color c)
    {
        if (sr == null)
        {
            return;
        }
        sr.color = c;
        originalColor = c; // 기존 코드가 originalColor로 복구했다면, 이 기준을 갱신
    }

}
