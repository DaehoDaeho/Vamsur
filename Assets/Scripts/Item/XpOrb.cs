using UnityEngine;

public class XpOrb : MonoBehaviour
{
    public int xpValue = 3;             // 이 오브가 주는 경험치
    public float moveSpeed = 6f;        // 끌릴 때 속도
    public float catchDistance = 0.25f; // 이 거리 안으로 오면 먹힌 걸로 처리

    // 내부 상태
    private bool attracted = false;     // 자석에 끌리는 중인가
    private Transform target;           // 자석 중심(보통 Player의 Magnet)
    private XpSystem xpSystem;          // 경험치를 올릴 대상
    private GameObject keyPrefab;

    private void Start()
    {
        Invoke("DestroyOrb", 60.0f);
    }

    void Update()
    {
        if (attracted && target != null)
        {
            // 타겟 쪽으로 조금씩 이동
            Vector3 p = transform.position;
            Vector3 t = target.position;
            Vector3 next = Vector3.MoveTowards(p, t, moveSpeed * Time.deltaTime);
            transform.position = next;

            // 가까워지면 먹힌 걸로 처리
            float dist = Vector3.Distance(transform.position, t);
            if (dist <= catchDistance)
            {
                GiveXpAndDisappear();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // PlayerMagnet 태그를 가진 Trigger에 닿으면 끌림 시작
        if (other.CompareTag("PlayerMagnet"))
        {
            attracted = true;
            target = other.transform;

            // 자석의 부모(플레이어) 쪽에서 XpSystem 찾기
            if (xpSystem == null)
            {
                xpSystem = other.GetComponentInParent<XpSystem>();
                if (xpSystem == null)
                {
                    // 혹시 못 찾으면 씬에서 한 번 검색(비추천이지만 안전장치)
                    xpSystem = FindAnyObjectByType<XpSystem>();
                }
            }
        }
    }

    void GiveXpAndDisappear()
    {
        CancelInvoke();

        if (xpSystem != null)
        {
            xpSystem.AddExp(xpValue);
        }

        // 사라지기(풀 있으면 풀에 반납 / 없으면 Destroy)
        DestroyOrb();
    }

    public void SetKeyPrefab(GameObject key)
    {
        keyPrefab = key;
    }

    void DestroyOrb()
    {
        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.Release(keyPrefab, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }    
}
