using UnityEngine;
using UnityEngine.UI;

public class NightFogOverlay : MonoBehaviour
{
    public Image overlayImage;           // 풀스크린 Image
    public DayNightController dayNight;  // 밤 정도를 참조
    public Color nightTint = new Color(0.05f, 0.08f, 0.2f, 0.4f);

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

        Color c = nightTint;
        c.a = Mathf.Lerp(0.0f, nightTint.a, nightFactor);
        overlayImage.color = c;
    }
}
