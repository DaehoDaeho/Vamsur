using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDmageable
{
    public int maxHP = 30;
    public int currentHP;

    public Action<int, int> OnHPChanged;
    public Action OnDied;

    public bool IsAlive => currentHP > 0;

    void Awake()
    {
        currentHP = maxHP;
        if (OnHPChanged != null) // �̺�Ʈ �Լ��� ����� �Ǿ� ������.
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
        Debug.Log("���� ����!!!!!");
        if(OnHPChanged != null)
        {
            OnHPChanged.Invoke(currentHP, maxHP);
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

        Destroy(gameObject);
    }
}
