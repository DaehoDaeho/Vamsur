using UnityEngine;

public class GoldSystem : MonoBehaviour
{
    [Header("State")]
    public int currentGold = 0;   // 현재 골드

    // 골드 더하기(코인 먹을 때 호출)
    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentGold = currentGold + amount;
    }

    // 살 수 있는지 확인
    public bool CanAfford(int price)
    {
        if (price < 0)
        {
            return false;
        }

        return currentGold >= price;
    }

    // 결제(성공하면 true)
    public bool Spend(int price)
    {
        if (CanAfford(price) == false)
        {
            return false;
        }

        currentGold = currentGold - price;
        return true;
    }
}
