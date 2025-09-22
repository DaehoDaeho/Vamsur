using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 연쇄 레이저(체인 라이트닝처럼 동작)
/// - 시작 대상에서 반경(chainRadius) 내 "가장 가까운" 다음 대상으로 점프
/// - 최대 hops 번 링크를 이어가며 각 링크마다 대미지 적용
/// - LineRenderer로 전체 링크 경로를 짧게 표시
/// 규칙:
///  - 모든 if는 블록 사용
///  - 불리언 비교는 == true/false
///  - 변수명 앞 언더스코어 금지
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ChainLaserController : MonoBehaviour
{
    [Header("Chain Settings")]
    public LayerMask damageMask;       // 맞아야 할 대상 레이어
    public float chainRadius = 4.0f;   // 다음 대상을 찾을 탐색 반경
    public int hops = 4;               // 최대 연쇄 횟수(링크 수)
    public int damagePerHop = 20;      // 링크 하나가 줄 대미지
    public float linkLife = 0.1f;      // 라인이 보이는 시간(초)

    LineRenderer lr;
    float timer = 0f;
    bool active = false;
    List<Vector3> linkPoints = new List<Vector3>(); // 캐스터→첫 대상→두 번째 대상→...

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        lr.positionCount = 0;

        // 얇고 선명한 라인
        lr.startWidth = 0.08f;
        lr.endWidth = 0.04f;
    }

    /// <summary>
    /// 시작 "점"만 있는 경우: 해당 점에서 가장 가까운 첫 대상 찾기
    /// </summary>
    public void FireChainFromPoint(Vector2 startPos)
    {
        Collider2D first = FindNearestTarget(startPos, chainRadius, null);
        if (first != null)
        {
            FireChain(first.transform);
        }
    }

    /// <summary>
    /// 시작 "대상"이 정해져 있을 때: 그 대상에서 연쇄 시작
    /// </summary>
    public void FireChain(Transform firstTarget)
    {
        if (firstTarget == null)
        {
            return;
        }

        // 연쇄 경로 리셋
        linkPoints.Clear();

        // 캐스터 → 첫 대상 선 추가
        linkPoints.Add(transform.position);
        linkPoints.Add(firstTarget.position);

        // 첫 대상 즉시 대미지
        DealDamageIfPossible(firstTarget);

        // 이미 맞은 대상 집합(중복 방지)
        HashSet<Transform> visited = new HashSet<Transform>();
        visited.Add(firstTarget);

        int i = 0;
        Transform current = firstTarget;

        // 최대 hops 번 반복
        while (i < hops)
        {
            // 현재 대상 기준 반경 내 "가장 가까운" 다음 대상 검색
            Collider2D next = FindNearestTarget(current.position, chainRadius, visited);
            if (next == null)
            {
                // 더 이상 연쇄 불가(반경 내 없음)
                break;
            }

            Transform nextTf = next.transform;

            // 경로에 점 추가(현재 → 다음)
            linkPoints.Add(nextTf.position);

            // 대미지 1회
            DealDamageIfPossible(nextTf);

            // 방문 기록 후 현재 갱신
            visited.Add(nextTf);
            current = nextTf;

            i = i + 1;
        }

        // 라인 표시(전체 경로 한 번에)
        lr.enabled = true;
        lr.positionCount = linkPoints.Count;

        int p = 0;
        while (p < linkPoints.Count)
        {
            lr.SetPosition(p, linkPoints[p]);
            p = p + 1;
        }

        // 아주 짧게 표시하고 숨김(화면 잡음 방지)
        timer = linkLife;
        active = true;
    }

    void Update()
    {
        if (active == true)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                lr.enabled = false;
                lr.positionCount = 0;
                active = false;
            }
        }
    }

    /// <summary>
    /// 반경 내 후보 중 "가장 가까운" 대상 찾기
    /// exclude에 이미 맞은 대상들이 있으면 제외
    /// 거리 비교는 제곱거리(루트 비용 절감)
    /// </summary>
    Collider2D FindNearestTarget(Vector2 from, float radius, HashSet<Transform> exclude)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(from, radius, damageMask);

        float best = float.MaxValue;
        Collider2D bestCol = null;

        int i = 0;
        while (i < hits.Length)
        {
            Collider2D c = hits[i];
            if (c != null)
            {
                if (exclude != null)
                {
                    if (exclude.Contains(c.transform) == true)
                    {
                        i = i + 1;
                        continue;
                    }
                }

                // 제곱거리 사용: d2 = |p - q|^2
                float d2 = Vector2.SqrMagnitude((Vector2)c.transform.position - from);
                if (d2 < best)
                {
                    best = d2;
                    bestCol = c;
                }
            }
            i = i + 1;
        }

        return bestCol;
    }

    void DealDamageIfPossible(Transform tf)
    {
        Health hp = tf.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damagePerHop, hp.transform.position);
        }
    }
}
