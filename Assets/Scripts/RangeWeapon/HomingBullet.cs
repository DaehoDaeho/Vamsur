using UnityEngine;

/// <summary>
/// 기존 총알 프리팹 전용 유도탄 스크립트(라인/트레일 미사용)
/// 규칙: 모든 if 블록 사용, 불리언 비교는 == true/false, 언더스코어 미사용
/// 핵심 아이디어:
///  - retargetInterval마다 반경 seekRadius 안에서 가장 가까운 적을 찾는다.
///  - 현재 각 a → 목표 각 b로 '프레임당 회전 한도'만큼만 회전(부드러운 조향).
///  - transform.up을 전방으로 사용하므로, 스프라이트는 '위쪽'이 총알의 앞이 되도록 정렬.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class HomingBullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;                   // 초당 이동 속도
    public float turnSpeedDegPerSec = 240f;    // 초당 회전 가능한 최대 각도(도). 너무 크면 급회전.
    public float lifeTime = 3f;                // 총알 수명(초)

    [Header("Targeting")]
    public LayerMask targetMask;               // 적 레이어
    public float seekRadius = 6f;              // 목표 탐색 반경(월드 단위)
    public float retargetInterval = 0.25f;     // 목표 재검색 주기(초) - 성능을 위한 스로틀링

    [Header("Damage")]
    public int damage = 10;                    // 적에 닿았을 때 줄 대미지

    Rigidbody2D rb;
    float lifeLeft = 0f;
    float retargetLeft = 0f;
    Transform currentTarget = null;

    /// <summary>
    /// 총알 발사(외부에서 한 줄로 호출)
    /// </summary>
    public void Fire(Vector2 position, Vector2 initialDir)
    {
        transform.position = position;

        // 초기 방향이 0에 가깝다면 오른쪽을 기본값으로 사용
        Vector2 dir = Vector2.right;

        if (initialDir.sqrMagnitude > 0.0001f)
        {
            dir = initialDir.normalized;
        }

        // 스프라이트의 'up(위쪽)'을 진행 방향으로 맞춘다.
        // 2D에서 전방을 transform.up으로 쓰기로 했으므로 -90도 보정.
        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        // 수명/타깃 타이머 초기화
        lifeLeft = lifeTime;
        retargetLeft = 0f;
        currentTarget = null;

        //gameObject.SetActive(true);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 에디터에서 Rigidbody2D의 GravityScale=0, Interpolate, Continuous 설정 권장
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // (1) 수명 감소
        if (lifeLeft > 0f)
        {
            lifeLeft -= dt;
            if (lifeLeft <= 0f)
            {
                lifeLeft = 0f;
                //gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }
        }

        // (2) 목표 재검색(너무 자주 하지 않도록 주기 사용)
        if (retargetLeft > 0f)
        {
            retargetLeft -= dt;
            if (retargetLeft < 0f)
            {
                retargetLeft = 0f;
            }
        }
        if (retargetLeft == 0f)
        {
            currentTarget = FindNearestTarget((Vector2)transform.position, seekRadius, targetMask);
            retargetLeft = retargetInterval;
        }

        // (3) 조향: 현재 전방→목표 방향으로 '서서히' 회전
        Vector2 forward = transform.up; // 현재 전방(단위 벡터)
        float a = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg; // 현재 각도(도)

        if (currentTarget != null)
        {
            Vector2 toTarget = (Vector2)currentTarget.position - (Vector2)transform.position;

            // 목표가 너무 가까워서 방향이 애매하면 회전하지 않음
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                toTarget.Normalize();
                float b = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg; // 목표 각도(도)

                // 각도 차이를 -180~+180으로 정규화(짧은 방향으로 돌기)
                float d = DeltaAngleDeg(a, b);

                // 이번 프레임 허용 회전량 = turnSpeedDegPerSec * dt
                float maxTurn = turnSpeedDegPerSec * dt;

                // 실제 회전량을 -maxTurn ~ +maxTurn으로 제한
                float turn = Mathf.Clamp(d, -maxTurn, +maxTurn);

                // 새 각도 적용 (up 기준 보정 -90도)
                float newA = a + turn;
                transform.rotation = Quaternion.Euler(0f, 0f, newA - 90f);
            }
        }

        // (4) 이동: 전방으로 speed*dt만큼 이동 (물리 보간을 위해 MovePosition 사용)
        Vector2 move = (Vector2)transform.up * speed * dt;
        rb.MovePosition(rb.position + move);
    }

    // 충돌(Trigger 권장)
    void OnTriggerEnter2D(Collider2D other)
    {
        // 대상 레이어에만 반응
        if (((1 << other.gameObject.layer) & targetMask.value) != 0)
        {
            Health hp = other.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage, hp.transform.position);
            }
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    Transform FindNearestTarget(Vector2 from, float radius, LayerMask mask)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(from, radius, mask);

        float best = float.MaxValue;
        Transform bestTf = null;

        int i = 0;
        while (i < hits.Length)
        {
            Collider2D c = hits[i];
            if (c != null)
            {
                Vector2 diff = (Vector2)c.transform.position - from;
                // 제곱거리: sqrt 연산 없이 거리 비교 가능 → 빠름
                float d2 = diff.x * diff.x + diff.y * diff.y;
                if (d2 < best)
                {
                    best = d2;
                    bestTf = c.transform;
                }
            }
            i = i + 1;
        }

        return bestTf;
    }

    float DeltaAngleDeg(float fromDeg, float toDeg)
    {
        // 각도 차이를 -180~+180 범위로 맞추는 함수(짧은 회전 방향 선택)
        float delta = toDeg - fromDeg;
        while (delta > 180f)
        {
            delta = delta - 360f;
        }
        while (delta < -180f)
        {
            delta = delta + 360f;
        }
        return delta;
    }
}
