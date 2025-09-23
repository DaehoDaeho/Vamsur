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
    private float attackSpeedMultiplier = 1.0f;

    [Header("Projectile Mode")]
    public bool useProjectile = true;                 // true면 총알 발사, false면 기존 근접
    public GameObject projectilePrefab;               // Bullet_Standard 프리팹
    public Transform firePoint;                       // 탄이 나갈 위치(없으면 플레이어 위치)
    public LayerMask projectileHitMask;               // 맞힐 대상(Enemy 레이어)
    public int projectileDamage = 10;
    public float projectileSpeed = 8f;
    public float projectileRange = 8f;
    public int projectilePierce = 1;

    // 플레이어 참조(애니메이션 알림용)
    public PlayerAnimatorSync animSync;               // 인스펙터로 연결(없으면 런타임에 찾기)

    // (선택) 내부 상태: 마지막 이동 방향(적이 없을 때 방향 대체)
    private Vector2 lastMoveDir = Vector2.right;
    public Rigidbody2D playerRb;                      // 플레이어의 Rigidbody2D(이동 벡터 읽기)
    public WeaponShooter weaponShooter;
    public ChainLaserController clc;

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

    void Start()
    {
        if (animSync == null)
        {
            animSync = FindAnyObjectByType<PlayerAnimatorSync>();
        }
        if (playerRb == null)
        {
            playerRb = GetComponent<Rigidbody2D>();
            if (playerRb == null)
            {
                playerRb = FindAnyObjectByType<Rigidbody2D>();
            }
        }
    }

    private float GetCurrentAttackInterval()
    {
        if(weaponMount != null && weaponMount.weapon != null)
        {
            float baseInterval = Mathf.Max(0.05f, weaponMount.weapon.attackInterval);
            float speedMul = Mathf.Max(0.1f, attackSpeedMultiplier);
            return baseInterval / speedMul;
        }

        return 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float interval = GetCurrentAttackInterval();
        if(interval <= 0.0f)
        {
            return;
        }

        if (playerRb != null)
        {
            Vector2 v = playerRb.velocity;
            if (v.sqrMagnitude > 0.0001f)
            {
                lastMoveDir = v.normalized;
            }
        }

        attackTimer -= Time.deltaTime;

        if(attackTimer <= 0.0f)
        {
            // 공격 처리.
            //PerformAttack();
            attackTimer = interval;
        }
    }

    private void PerformAttack()
    {
        if (useProjectile == true)
        {
            //FireProjectile();
            weaponShooter.TryFire();
        }
        else
        {
            if (attackOrigin == null)
            {
                attackOrigin = transform;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin.position, attackRange, targetLayers);

            int damage = GetCurrentDamage();

            foreach (var col in hits)
            {
                var damageable = col.GetComponent<IDmageable>();
                if (damageable == null || damageable.IsAlive == false)
                {
                    continue;
                }

                Vector3 hitPoint = col.ClosestPoint(attackOrigin.position);

                if (damageable != null)
                {
                    damageable.TakeDamage(damage, hitPoint);
                }
            }
        }

        // 공격 애니메이션 알림(플레이어 애니메이터에 Attack 표시)
        if (animSync != null)
        {
            //animSync.NotifyAttack();
        }
    }

    // 총알 발사 함수 — 방향 선택 → 탄 생성/초기화
    void FireProjectile()
    {
        Transform t = FindNearestEnemy();
        if(t == null)
        {
            return;
        }

        // Laset ========================================
        clc.FireChain(t);
        return;
        // Laset ========================================

        // 1) 목표 방향 계산
        Vector2 dir = GetAimDirection();

        // 2) 발사 위치
        Vector3 spawnPos;
        if (firePoint != null)
        {
            spawnPos = firePoint.position;
        }
        else
        {
            spawnPos = transform.position;
        }

        // 3) 탄 생성(풀 우선)
        GameObject go;
        if (PoolManager.Instance != null)
        {
            go = PoolManager.Instance.Spawn(projectilePrefab, spawnPos, Quaternion.identity, null);
        }
        else
        {
            go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        }

        // 4) 초기화 값 넣기
        ProjectileBullet b = go.GetComponent<ProjectileBullet>();
        if (b == null)
        {
            b = go.AddComponent<ProjectileBullet>(); // 혹시 프리팹에 빠졌다면 안전하게 붙이기
        }

        b.Init(spawnPos, dir, projectileDamage, projectileSpeed, projectileRange, projectilePierce, projectileHitMask);
        AudioManager.Instance.PlaySFX("attack");
    }

    // 간단한 조준 함수: 1) 가까운 적 있으면 그쪽, 2) 없으면 마지막 이동 방향, 3) 그래도 없으면 오른쪽
    Vector2 GetAimDirection()
    {
        Transform t = FindNearestEnemy();
        if (t != null)
        {
            Vector2 d = (t.position - transform.position);
            if (d.sqrMagnitude > 0.0001f)
            {
                return d.normalized;
            }
        }

        if (lastMoveDir.sqrMagnitude > 0.0001f)
        {
            return lastMoveDir;
        }

        return Vector2.right;
    }

    // 범위 내에서 가장 가까운 적 한 명 찾기(아주 단순 버전)
    public float aimSearchRadius = 10f;

    Transform FindNearestEnemy()
    {
        // 비싼 연산이라 실제 게임에서는 캐싱/스폰 목록을 쓰는 게 좋아.
        // 오늘은 이해를 위해 단순 Physics2D.OverlapCircle 사용
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aimSearchRadius);
        Transform best = null;
        float bestDist = 999999f;

        int i = 0;
        while (i < hits.Length)
        {
            Collider2D c = hits[i];

            // Tag로 구분(Enemy)
            if (c.CompareTag("Enemy") == true)
            {
                float d = Vector2.SqrMagnitude(c.transform.position - transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = c.transform;
                }
            }
            i = i + 1;
        }

        return best;
    }

    private int GetCurrentDamage()
    {
        if(weaponMount != null && weaponMount.weapon != null)
        {
            return Mathf.Max(1, weaponMount.weapon.baseDamage);
        }

        return 1;
    }

    public void AddAttackSpeedMultiplier(float addPercent)
    {
        float m = 1.0f + Mathf.Max(-0.9f, addPercent / 100.0f);
        attackSpeedMultiplier *= m;
    }
}
