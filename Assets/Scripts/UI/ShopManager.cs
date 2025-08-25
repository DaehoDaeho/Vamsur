using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject shopPanel;  // 상점 패널(켜고/끄기)
    public TextMeshProUGUI infoText;         // "구매 완료!" 또는 "골드 부족!" 표시

    [Header("Prices")]
    public int priceMoveSpeed = 30;
    public int priceAttackSpeed = 40;
    public int priceHeal = 20;

    [Header("Effects")]
    public float moveSpeedPercent = 10f;   // +10%
    public float attackSpeedPercent = 15f; // +15%
    public int healAmount = 20;            // +20 HP

    [Header("Targets")]
    public GoldSystem gold;
    public PlayerCore player;
    public AutoAttackController autoAttack;

    void Start()
    {
        if (gold == null)
        {
            gold = FindAnyObjectByType<GoldSystem>();
        }

        if (player == null)
        {
            player = FindAnyObjectByType<PlayerCore>();
        }

        if (autoAttack == null)
        {
            autoAttack = FindAnyObjectByType<AutoAttackController>();
        }

        CloseShop();
    }

    void Update()
    {
        // B 키로 열기/닫기
        if (Input.GetKeyDown(KeyCode.B) == true)
        {
            if (shopPanel.activeSelf == true)
            {
                CloseShop();
            }

            else OpenShop();
        }
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f; // 일시정지
        if (infoText != null)
        {
            infoText.text = "원하는 강화를 선택하세요.";
        }
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f; // 재개
    }

    // 버튼용 함수 3개 (인스펙터에서 직접 연결)
    public void BuyMoveSpeed()
    {
        if (gold == null)
        {
            CloseShop();
            return;
        }

        if (gold.CanAfford(priceMoveSpeed) == false)
        {
            if (infoText != null)
            {
                infoText.text = "골드가 부족합니다.";
            }

            CloseShop();
            return;
        }
        if (gold.Spend(priceMoveSpeed) == false)
        {
            if (infoText != null)
            {
                infoText.text = "결제 실패!";
            }

            CloseShop();
            return;
        }
        if (player != null)
        {
            player.AddMoveSpeedMultiplier(moveSpeedPercent);
        }

        if (infoText != null)
        {
            infoText.text = "이동속도 +" + moveSpeedPercent + "% 적용!";
        }

        CloseShop();
    }

    public void BuyAttackSpeed()
    {
        if (gold == null)
        {
            CloseShop();
            return;
        }

        if (gold.CanAfford(priceAttackSpeed) == false)
        {
            if (infoText != null)
            {
                infoText.text = "골드가 부족합니다.";
            }

            CloseShop();
            return;
        }
        if (gold.Spend(priceAttackSpeed) == false)
        {
            if (infoText != null)
            {
                infoText.text = "결제 실패!";
            }

            CloseShop();
            return;
        }

        if (autoAttack != null)
        {
            autoAttack.AddAttackSpeedMultiplier(attackSpeedPercent);
        }

        if (infoText != null)
        {
            infoText.text = "공격속도 +" + attackSpeedPercent + "% 적용!";
        }

        CloseShop();
    }

    public void BuyHeal()
    {
        if (gold == null)
        {
            CloseShop();
            return;
        }

        if (gold.CanAfford(priceHeal) == false)
        {
            if (infoText != null)
            {
                infoText.text = "골드가 부족합니다.";
            }

            CloseShop();
            return;
        }
        if (gold.Spend(priceHeal) == false)
        {
            if (infoText != null)
            {
                infoText.text = "결제 실패!";
            }

            CloseShop();
            return;
        }
        if (player != null)
        {
            player.Heal(healAmount);
        }

        if (infoText != null)
        {
            infoText.text = "체력 +" + healAmount + " 적용!";
        }

        CloseShop();
    }

    // 구매 로직(간단): 가격 확인 → 결제 → 효과 실행 → 안내 → 닫기
    void TryBuy(int price, string label, System.Action apply)
    {
        if (gold == null)
        {
            return;
        }

        if (!gold.CanAfford(price))
        {
            if (infoText != null)
            {
                infoText.text = "골드가 부족합니다.";
            }

            return;
        }

        bool ok = gold.Spend(price);
        if (!ok)
        {
            if (infoText != null)
            {
                infoText.text = "결제 실패!";
            }

            return;
        }

        // 효과 적용
        if (apply != null)
        {
            apply();
        }

        if (infoText != null)
        {
            infoText.text = label + " 적용!";
        }

        CloseShop();
    }
}
