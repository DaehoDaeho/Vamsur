using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 전체를 덮는 반투명 Image로 밤 분위기를 만든다(URP 불필요).
/// - 낮에는 알파 0, 밤에는 알파를 올려 어둡게.
/// 규칙: 모든 if 블록, 불리언 비교는 == true/false
/// </summary>
public class NightFogOverlay1 : MonoBehaviour
{
    public Image overlayImage;           // Canvas 최상단의 풀스크린 Image
    public DayNightController dayNight;  // 밤 정도를 참조
    public Color nightTint = new Color(0.05f, 0.08f, 0.2f, 0.4f); // 남청색, A=최대 알파

    void Update()
    {
        if (overlayImage == null)
        {
            return;
        }
        if (dayNight == null)
        {
            return;
        }

        float nightFactor = dayNight.GetNightFactor();

        // 알파만 nightFactor에 맞게 보간(색상은 고정)
        Color c = nightTint;
        c.a = Mathf.Lerp(0f, nightTint.a, nightFactor); // 낮 0 → 밤 nightTint.a
        overlayImage.color = c;
    }
}
