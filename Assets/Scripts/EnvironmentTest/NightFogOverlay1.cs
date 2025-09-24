using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ȭ�� ��ü�� ���� ������ Image�� �� �����⸦ �����(URP ���ʿ�).
/// - ������ ���� 0, �㿡�� ���ĸ� �÷� ��Ӱ�.
/// ��Ģ: ��� if ���, �Ҹ��� �񱳴� == true/false
/// </summary>
public class NightFogOverlay1 : MonoBehaviour
{
    public Image overlayImage;           // Canvas �ֻ���� Ǯ��ũ�� Image
    public DayNightController dayNight;  // �� ������ ����
    public Color nightTint = new Color(0.05f, 0.08f, 0.2f, 0.4f); // ��û��, A=�ִ� ����

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

        // ���ĸ� nightFactor�� �°� ����(������ ����)
        Color c = nightTint;
        c.a = Mathf.Lerp(0f, nightTint.a, nightFactor); // �� 0 �� �� nightTint.a
        overlayImage.color = c;
    }
}
