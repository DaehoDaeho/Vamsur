using UnityEngine;

/// <summary>
/// [12일차] 플레이어 애니메이션 동기화 어댑터
/// Animator 파라미터:
///  - Speed(float), IsMoving(bool), IsAttacking(bool), HitBool(bool), IsDead(bool)
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimatorSync : MonoBehaviour
{
    [Header("Refs")]
    public Animator anim;
    public Rigidbody2D rb;
    public Health health; // 선택

    [Header("Config")]
    public float moveThreshold = 0.05f;
    public float attackFlagTime = 0.15f;
    public float hitFlagTime = 0.12f;

    private float attackTimer = 0f;
    private float hitTimer = 0f;

    void Awake()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    void Update()
    {
        // 이동 상태 갱신
        float speed = 0f;
        if (rb != null)
        {
            speed = rb.velocity.magnitude;
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", speed);
            if (speed > moveThreshold)
            {
                anim.SetBool("IsMoving", true);
            }
            else
            {
                anim.SetBool("IsMoving", false);
            }
        }

        // 공격 플래그 타이머
        if (attackTimer > 0f)
        {
            attackTimer = attackTimer - Time.deltaTime;
            if (attackTimer <= 0f)
            {
                if (anim != null)
                {
                    anim.SetBool("IsAttacking", false);
                }
            }
        }

        // 피격 플래그 타이머
        if (hitTimer > 0f)
        {
            hitTimer = hitTimer - Time.deltaTime;
            if (hitTimer <= 0f)
            {
                if (anim != null)
                {
                    anim.SetBool("HitBool", false);
                }
            }
        }

        // 사망 감지(선택)
        if (health != null && anim != null)
        {
            bool alive = health.IsAliveNow();
            if (alive == false)
            {
                anim.SetBool("IsDead", true);
            }
        }
    }

    public void NotifyAttack()
    {
        if (anim != null)
        {
            anim.SetBool("IsAttacking", true);
        }
        attackTimer = attackFlagTime;
    }

    public void NotifyHit()
    {
        if (anim != null)
        {
            anim.SetBool("HitBool", true);
        }
        hitTimer = hitFlagTime;
    }

    public void NotifyDead()
    {
        if (anim != null)
        {
            anim.SetBool("IsDead", true);
        }
    }
}
