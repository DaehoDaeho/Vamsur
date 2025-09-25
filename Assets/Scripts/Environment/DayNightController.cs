using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [Header("Cycle")]
    public float cycleSeconds = 90f;     // 낮->밤->낮 한 바퀴 시간(초).
    [Range(0f, 1f)]
    public float time01 = 0f;            // 하루 진행도(0=자정, 0.25=아침, 0.5=정오, 0.75=저녁, 1≈자정)
    public bool runAutomatically = true; // true면 자동으로 시간이 흐름.

    [Header("Night Curve")]
    public float nightCurveSharpness = 1.0f; // 1=기본, >이면 밤을 좀 더 길게/강하게 느끼게.

    public bool isNight = false;

    public WeatherController weatherController;

    /// <summary>
    /// 현재 시간에서 '밤의 정도(0~1)'를 구한다.
    /// 0.5(정오) 부근은 0, 0 또는 1(자정) 근처는 1.
    /// </summary>
    public float GetNightFactor()
    {
        float raw = 0f;

        if (time01 >= 0.5f)
        {
            raw = (time01 - 0.5f) * 2f;
        }
        else
        {
            raw = 1f - (time01 * 2f);
        }

        float smooth = Mathf.SmoothStep(0f, 1f, raw);
        if (nightCurveSharpness > 1.0f)
        {
            smooth = Mathf.Pow(smooth, nightCurveSharpness);
        }
        return smooth;
    }

    void Update()
    {
        if (runAutomatically == true)
        {
            float dt = Time.deltaTime;
            if (cycleSeconds > 0f)
            {
                time01 = time01 + (dt / cycleSeconds);
                if (time01 >= 1f)
                {
                    time01 = time01 - 1f;
                }
                if (time01 < 0f)
                {
                    time01 = 0f;
                }
            }
        }

        float nightFactor = GetNightFactor();
        bool nowNight = (nightFactor > 0.5f);
        if (nowNight != isNight)
        {
            isNight = nowNight;

            if (isNight == true)
            {
                StatModifierAtNight.addctiveDamage = 10;
            }
            else
            {
                StatModifierAtNight.addctiveDamage = 0;
            }

            if(isNight == true)
            {
                weatherController.enableRain = false;
                weatherController.enableSnow = false;
            }
            else
            {
                bool enableRain = Random.Range(0, 2) == 0 ? true : false;
                weatherController.enableRain = enableRain;
                weatherController.enableSnow = !enableRain;
            }
        }
    }
}
