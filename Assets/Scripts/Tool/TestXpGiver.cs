using UnityEngine;

/// <summary>
/// 테스트용: K 키를 누르면 XP +10.
/// </summary>
public class TestXpGiver : MonoBehaviour
{
    public XpSystem xpSystem;
    public int amountPerPress = 10;

    private void Reset()
    {
        if (!xpSystem)
        {
            xpSystem = FindAnyObjectByType<XpSystem>();
        }   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            xpSystem?.AddExp(amountPerPress);
        }
    }
}
