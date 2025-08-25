using UnityEngine;
using UnityEngine.UI;

public class GoldUIBinder : MonoBehaviour
{
    public GoldSystem gold;
    public Text goldText; // "Gold: 123" ���� �ؽ�Ʈ

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
