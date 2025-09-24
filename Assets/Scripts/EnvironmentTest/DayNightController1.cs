using UnityEngine;

/// <summary>
/// 낮/밤 사이클 컨트롤러(URP 없이도 동작)
/// - time01(0~1)을 돌려 하루를 표현하고, 밤 정도(nightFactor)를 계산한다.
/// - 밤 시작/종료 시 스폰러 등 외부 매니저의 '메서드'를 직접 호출해 연동.
/// 규칙: 모든 if 블록, 불리언 비교는 == true/false, 언더스코어 미사용
/// </summary>
public class DayNightController1 : MonoBehaviour
{
    [Header("Cycle")]
    public float cycleSeconds = 90f;     // 낮→밤→낮 한 바퀴 시간(초). 테스트는 20으로 줄여도 됨.
    [Range(0f, 1f)]
    public float time01 = 0f;            // 하루 진행도(0=자정, 0.25=아침, 0.5=정오, 0.75=저녁, 1≈자정)
    public bool runAutomatically = true; // true면 자동으로 시간이 흐름

    [Header("Night Curve")]
    public float nightCurveSharpness = 1.0f; // 1=기본, >1이면 밤을 좀 더 길게/강하게 느끼게

    public bool isNight = false;         // 현재 밤 상태(내부에서 자동 갱신)

    /// <summary>
    /// 현재 시간에서 '밤의 정도(0~1)'를 구한다.
    /// 0.5(정오) 부근은 0, 0 또는 1(자정) 근처는 1.
    /// </summary>
    public float GetNightFactor()
    {
        float raw = 0f;

        // time01 0~0.5: 새벽→정오(밤에서 낮으로), 0.5~1: 정오→자정(낮에서 밤으로)
        if (time01 >= 0.5f)
        {
            // 0.5~1 → 0~1
            raw = (time01 - 0.5f) * 2f;
        }
        else
        {
            // 0~0.5 → 1~0
            raw = 1f - (time01 * 2f);
        }

        // 부드러운 곡선 적용
        float smooth = Mathf.SmoothStep(0f, 1f, raw);
        if (nightCurveSharpness > 1.0f)
        {
            smooth = Mathf.Pow(smooth, nightCurveSharpness);
        }
        return smooth;
    }

    void Update()
    {
        // 1) 시간 진행
        if (runAutomatically == true)
        {
            float dt = Time.deltaTime;
            if (cycleSeconds > 0f)
            {
                time01 = time01 + (dt / cycleSeconds);
                if (time01 >= 1f)
                {
                    time01 = time01 - 1f; // 래핑
                }
                if (time01 < 0f)
                {
                    time01 = 0f;
                }
            }
        }

        // 2) 밤/낮 전환 감지
        float nightFactor = GetNightFactor();
        bool nowNight = (nightFactor > 0.5f);
        if (nowNight != isNight)
        {
            isNight = nowNight;

            // 이벤트 대신: 필요한 매니저 '직접 호출'
            //EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
            //if (spawner != null)
            //{
            //    if (isNight == true)
            //    {
            //        spawner.OnNightStarted(); // 난이도/확률 살짝 올리기 등
            //    }
            //    else
            //    {
            //        spawner.OnDayStarted();
            //    }
            //}
        }
    }
}
