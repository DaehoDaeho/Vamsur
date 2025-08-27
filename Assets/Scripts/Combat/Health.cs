using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDmageable
{
    public int maxHP = 100;
    public int currentHP;

    public Action<int, int> OnHPChanged;
    public Action<int, Vector3> OnDamaged;
    public Action OnDied;

    public bool IsAlive => currentHP > 0;

    void Awake()
    {
        currentHP = maxHP;
        if (OnHPChanged != null) // 이벤트 함수가 등록이 되어 있으면.
        {
            OnHPChanged.Invoke(currentHP, maxHP);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (amount <= 0 || IsAlive == false)
        {
            return;
        }

        currentHP = Mathf.Max(currentHP - amount, 0);
        Debug.Log("공격 받음!!!!!");
        if(OnHPChanged != null)
        {
            OnHPChanged.Invoke(currentHP, maxHP);
        }

        if(OnDamaged != null)
        {
            OnDamaged.Invoke(amount, hitPoint);
        }

        if(currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(OnDied != null)
        {
            OnDied.Invoke();
        }

        EnemyDropper enemyDropper = GetComponent<EnemyDropper>();
        if (enemyDropper != null)
        {
            enemyDropper.CreateOrb();
        }

        EnemyTracker enemyTracker = GetComponent<EnemyTracker>();
        if(enemyTracker != null)
        {
            enemyTracker.ProcessDestroy();
        }

        EnemyGoldDropper enemyGoldDropper = GetComponent<EnemyGoldDropper>();
        if (enemyGoldDropper != null)
        {
            enemyGoldDropper.CreateGold();
        }

        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || !IsAlive)
        {
            return;
        }

        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP); // 3일차 UI바인더가 이 이벤트를 듣고 있음
    }

    public void MultiplyMaxHpPercent(float percent)
    {
        // percent가 60이면 최댓값을 1.6배로 만든다.
        float mul = 1f + (percent / 100f);
        if (mul < 0.1f)
        {
            mul = 0.1f;
        }

        int oldMax = maxHP;
        int newMax = Mathf.RoundToInt(oldMax * mul);
        if (newMax < 1)
        {
            newMax = 1;
        }

        int delta = newMax - oldMax;
        maxHP = newMax;

        // 현재 HP도 같은 비율로 늘려서 "강화 느낌"을 준다.
        int newCurrent = Mathf.RoundToInt(currentHP * mul);
        if (newCurrent > maxHP)
        {
            newCurrent = maxHP;
        }
        if (newCurrent < 1)
        {
            newCurrent = 1;
        }
        currentHP = newCurrent;

        // UI 갱신
        if (OnHPChanged != null)
        {
            OnHPChanged(currentHP, maxHP);
        }
    }

    public bool IsAliveNow()
    {
        if (currentHP > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
