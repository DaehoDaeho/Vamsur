using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 배경(바닥/환경) 스프라이트들의 '색'을 밤에 더 어둡게 틴트.
/// - 캐릭터/효과는 손대지 않음 → 밤에도 캐릭터는 선명.
/// - 성능: 초기화 시 자식 SpriteRenderer를 캐시.
/// 규칙: 모든 if 블록, 불리언 비교는 == true/false
/// </summary>
public class WorldTintController1 : MonoBehaviour
{
    public DayNightController dayNight;
    public Color dayColor = Color.white;                 // 낮색(그대로)
    public Color nightColor = new Color(0.6f, 0.65f, 0.8f, 1f); // 밤색(살짝 푸르고 어둡게)

    List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    void Awake()
    {
        renderers.Clear();
        SpriteRenderer[] arr = GetComponentsInChildren<SpriteRenderer>(true);
        int i = 0;
        while (i < arr.Length)
        {
            if (arr[i] != null)
            {
                renderers.Add(arr[i]);
            }
            i = i + 1;
        }
    }

    void Update()
    {
        if (dayNight == null)
        {
            return;
        }
        float nightFactor = dayNight.GetNightFactor();
        Color c = Color.Lerp(dayColor, nightColor, nightFactor);

        int i = 0;
        while (i < renderers.Count)
        {
            SpriteRenderer sr = renderers[i];
            if (sr != null)
            {
                // 알파는 유지하고 RGB만 변경되도록 곱 연산처럼 사용
                Color baseC = sr.color;
                Color newC = new Color(c.r, c.g, c.b, baseC.a);
                sr.color = newC;
            }
            i = i + 1;
        }
    }
}
