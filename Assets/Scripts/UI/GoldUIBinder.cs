using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoldUIBinder : MonoBehaviour
{
    public GoldSystem gold;
    public TextMeshProUGUI goldText; // "Gold: 123" ���� �ؽ�Ʈ

    void Start()
    {
        if (gold == null)
        {
            gold = FindAnyObjectByType<GoldSystem>();
        }
    }

    void Update()
    {
        if (goldText != null && gold != null)
        {
            goldText.text = "Gold: " + gold.currentGold.ToString();
        }
    }
}
