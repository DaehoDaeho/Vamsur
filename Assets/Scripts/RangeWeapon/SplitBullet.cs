using UnityEngine;

/// <summary>
/// 기존 총알 프리팹 전용 분열탄 스크립트
/// - 직진하다가 '충돌' 또는 '수명 종료' 시 현재 진행 방향을 중심으로 N갈래 생성.
/// - 각도 배치: start~end 범위를 '균등 분배' → 예측 가능한 패턴.
/// 규칙: 모든 if 블록 사용, 불리언 비교는 == true/false
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class SplitBullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 9f;
    public float lifeTime = 2.0f;

    [Header("Hit")]
    public LayerMask targetMask;
    public int damage = 8;
    public bool destroyOnHit = true;

    [Header("Split")]
    public GameObject childBulletPrefab; // 생성할 자식 총알(동일 프리팹 or 유도탄 프리팹)
    public int splitCount = 7;           // 생성 개수
    public float spreadDeg = 70f;        // 전체 확산 각도
    public bool includeCenter = true;    // 중앙 방향 포함 여부

    Rigidbody2D rb;
    float lifeLeft = 0f;

    public void Fire(Vector2 position, Vector2 initialDir)
    {
        transform.position = position;

        Vector2 dir = Vector2.right;
        if (initialDir.sqrMagnitude > 0.0001f)
        {
            dir = initialDir.normalized;
        }

        // 전방을 진행 방향으로 (up 기준 보정 -90도)
        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        lifeLeft = lifeTime;
        //gameObject.SetActive(true);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // 직진 이동
        Vector2 move = (Vector2)transform.up * speed * dt;
        rb.MovePosition(rb.position + move);

        // 수명 체크
        if (lifeLeft > 0f)
        {
            lifeLeft -= dt;
            if (lifeLeft <= 0f)
            {
                lifeLeft = 0f;
                //DoSplit();
                //gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") == true)
        {
            return;
        }

        // 적 레이어만 처리
        if (((1 << other.gameObject.layer) & targetMask.value) != 0)
        {
            Health hp = other.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage, hp.transform.position);
            }

            DoSplit();
            if (destroyOnHit == true)
            {
                //gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }

    void DoSplit()
    {
        if (childBulletPrefab == null)
        {
            return;
        }
        if (splitCount <= 0)
        {
            return;
        }

        // 중심 각도: 현재 전방(up)
        Vector2 forward = transform.up;
        float centerDeg = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;

        if (includeCenter == true)
        {
            // 시작~끝을 count로 등분 → 중앙 방향 포함
            float start = centerDeg - spreadDeg * 0.5f;
            float end = centerDeg + spreadDeg * 0.5f;

            float step = 0f;
            if (splitCount > 1)
            {
                step = (end - start) / (float)(splitCount - 1);
            }

            int i = 0;
            while (i < splitCount)
            {
                float deg = start + step * i;
                SpawnChild(deg);
                i = i + 1;
            }
        }
        else
        {
            // 중앙을 비우고 좌우로 동일 분배
            int half = splitCount / 2;
            float halfSpread = spreadDeg * 0.5f;
            float step = 0f;
            if (half > 0)
            {
                step = halfSpread / (float)half;
            }

            // 왼쪽
            int i = 1;
            while (i <= half)
            {
                float deg = centerDeg - step * i;
                SpawnChild(deg);
                i = i + 1;
            }
            // 오른쪽
            i = 1;
            while (i <= half)
            {
                float deg = centerDeg + step * i;
                SpawnChild(deg);
                i = i + 1;
            }

            // 홀수면 중앙 하나 추가
            if ((splitCount % 2) == 1)
            {
                SpawnChild(centerDeg);
            }
        }
    }

    void SpawnChild(float deg)
    {
        // 각도 → 방향
        float rx = Mathf.Cos(deg * Mathf.Deg2Rad);
        float ry = Mathf.Sin(deg * Mathf.Deg2Rad);
        Vector2 dir = new Vector2(rx, ry).normalized;

        GameObject go = GameObject.Instantiate(childBulletPrefab);
        go.transform.position = transform.position;

        // 자식 총알의 타입에 따라 Fire 호출
        SplitBullet sb = go.GetComponent<SplitBullet>();
        if (sb != null)
        {
            sb.Fire(transform.position, dir);
            return;
        }

        HomingBullet hb = go.GetComponent<HomingBullet>();
        if (hb != null)
        {
            hb.Fire(transform.position, dir);
            return;
        }

        // 프로젝트의 기존 직선탄 스크립트가 따로 있다면 여기에 분기 추가
        // ExampleBasicBullet ex = go.GetComponent<ExampleBasicBullet>();
        // if (ex != null) { ex.Fire(transform.position, dir); return; }
    }
}
