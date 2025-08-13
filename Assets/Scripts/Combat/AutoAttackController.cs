using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    public WeaponMount weaponMount;
    public Transform attackOrigin;  // 공격의 기준점.

    public float attackRange = 1.5f;
    public LayerMask targetLayers;  // 적 레이어를 타겟팅 하기 위해.

    private float attackTimer;

    private void Reset()
    {
        if(weaponMount == null)
        {
            weaponMount = GetComponent<WeaponMount>();
        }

        if(attackOrigin == null)
        {
            attackOrigin = transform;
        }
    }

    private void Awake()
    {
        if(attackOrigin == null)
        {
            attackOrigin = transform;
        }

        attackTimer = GetCurrentAttackInterval();
    }

    private float GetCurrentAttackInterval()
    {
        if(weaponMount != null && weaponMount.weapon != null)
        {
            return Mathf.Max(0.05f, weaponMount.weapon.attackInterval);
        }

        return 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float interval = GetCurrentAttackInterval();
        if(interval <= 0.0f)
        {
            return;
        }

        attackTimer -= Time.deltaTime;

        if(attackTimer <= 0.0f)
        {
            // 공격 처리.
            PerformAttack();
            attackTimer = interval;
        }
    }

    private void PerformAttack()
    {
        if(attackOrigin == null)
        {
            attackOrigin = transform;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin.position, attackRange, targetLayers);

        int damage = GetCurrentDamage();

        foreach(var col in hits)
        {
            var damageable = col.GetComponent<IDmageable>();
            if(damageable == null || damageable.IsAlive == false)
            {
                continue;
            }

            Vector3 hitPoint = col.ClosestPoint(attackOrigin.position);

            if(damageable != null)
            {
                damageable.TakeDamage(damage, hitPoint);
            }
        }
    }

    private int GetCurrentDamage()
    {
        if(weaponMount != null && weaponMount.weapon != null)
        {
            return Mathf.Max(1, weaponMount.weapon.baseDamage);
        }

        return 1;
    }
}
