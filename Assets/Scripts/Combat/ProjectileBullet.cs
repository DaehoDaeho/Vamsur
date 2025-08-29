using UnityEngine;

/// <summary>
/// 표준 투사체(총알) 스크립트
/// - 쉬운 문법만 사용(이벤트/람다 없음)
/// - 이동: 지정된 방향 * 속도로 전진
/// - 수명: 누적 이동 거리 >= maxDistance 이면 소멸
/// - 충돌: Enemy에 닿으면 데미지 주고 pierce(관통) 1 감소
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ProjectileBullet : MonoBehaviour
{
    [Header("Runtime State (초기화로 세팅)")]
    public int damage = 10;           // 데미지
    public float speed = 8f;          // 이동 속도(유닛/초)
    public float maxDistance = 8f;    // 최대 사거리
    public int pierce = 1;            // 몇 번 관통 가능한지(1이면 1명 맞추고 소멸)
    public Vector2 dir = Vector2.right;// 진행 방향(정규화 권장)
    public LayerMask hitMask;         // 맞힐 레이어(Enemy)

    private Vector3 _startPos;        // 시작 위치(거리 계산용)
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 발사 직후에 설정값을 채워주기 위한 초기화 함수
    /// </summary>
    public void Init(Vector3 startPos, Vector2 direction, int dmg, float spd, float range, int pierceCount, LayerMask targetMask)
    {
        _startPos = startPos;
        transform.position = startPos;

        dir = direction;
        damage = dmg;
        speed = spd;
        maxDistance = range;
        pierce = pierceCount;
        hitMask = targetMask;

        // 방향에 맞춰 살짝 회전(선택)
        if (dir.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void Update()
    {
        // 1) 앞으로 이동
        Vector3 delta = (Vector3)(dir.normalized * speed * Time.deltaTime);
        transform.position = transform.position + delta;

        // 2) 사거리 체크
        float traveled = Vector3.Distance(_startPos, transform.position);
        if (traveled >= maxDistance)
        {
            Despawn();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) 맞힌 대상이 hitMask에 속하는지 확인(레이어 기반)
        int otherLayerBit = 1 << other.gameObject.layer;
        bool layerOk = (hitMask.value & otherLayerBit) != 0;

        if (layerOk == false)
        {
            // 맞히면 안 되는 대상(벽/아이템/플레이어 등) → 무시
            return;
        }

        // 2) Health를 찾아 데미지 주기(간단 직통 호출)
        Health h = other.GetComponent<Health>();
        if (h != null)
        {
            // 실제 데미지 적용
            h.TakeDamage(damage, other.transform.position);
        }

        // 3) 관통 수 감소
        pierce = pierce - 1;

        if (pierce <= 0)
        {
            Despawn();
        }
    }

    void Despawn()
    {
        // 풀 매니저가 있으면 풀 반환, 없으면 Destroy
        if (PoolManager.Instance != null)
        {
            gameObject.SetActive(false); // 단순 버전(키 지정 Release는 이후에)
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
