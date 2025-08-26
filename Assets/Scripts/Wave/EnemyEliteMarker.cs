using UnityEngine;

public class EnemyEliteMarker : MonoBehaviour
{
    [Header("Visual")]
    public Color eliteColor = new Color(0.8f, 0.2f, 1f, 1f); // 보라 계열

    // DifficultyDirector에서 계산한 %를 받아 적용
    public void ApplyElite(DifficultyDirector dir)
    {
        if (dir == null)
        {
            return;
        }

        // 1) 색 변경(가능하면)
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = eliteColor;
        }

        DamageFlash flash = GetComponent<DamageFlash>();
        if (flash != null)
        {
            flash.SetBaseColor(eliteColor);
        }

        // 2) 체력 배수 (Health에 함수 패치가 필요)
        Health hp = GetComponent<Health>();
        if (hp != null)
        {
            float hpPercent = dir.GetEliteHpPercent(); // 예: 60 → +60%
            // 아래 MultiplyMaxHpPercent는 Health에 추가할 작은 함수(아래에 제공)
            hp.MultiplyMaxHpPercent(hpPercent);
        }

        // 3) 이동속도 배수 (EnemyCore에 외부 배율 필드 패치 필요)
        EnemyCore ai = GetComponent<EnemyCore>();
        if (ai != null)
        {
            float spdPercent = dir.GetEliteSpeedPercent();
            ai.AddSpeedMultiplierPercent(spdPercent);
        }

        // 4) 드랍 보너스 (있을 때만)
        EnemyDropper xpDrop = GetComponent<EnemyDropper>();
        if (xpDrop != null)
        {
            float xpPercent = dir.GetEliteXpPercent();
            int bonus = Mathf.RoundToInt(xpDrop.xpPerOrb * (xpPercent / 100f));
            xpDrop.xpPerOrb = xpDrop.xpPerOrb + bonus;
        }

        EnemyGoldDropper goldDrop = GetComponent<EnemyGoldDropper>();
        if (goldDrop != null)
        {
            float gPercent = dir.GetEliteGoldPercent();
            int bonus = Mathf.RoundToInt(goldDrop.goldPerCoin * (gPercent / 100f));
            goldDrop.goldPerCoin = goldDrop.goldPerCoin + bonus;
        }
    }
}
