using UnityEngine;

public class GoldOrb : MonoBehaviour
{
    [Header("Stats")]
    public int value = 1;              // 이 코인이 주는 골드
    public float moveSpeed = 7f;       // 끌릴 때 속도
    public float catchDistance = 0.25f;

    private bool attracted = false;
    private Transform target;          // 자석 중심
    private GoldSystem goldSystem;     // 돈 받을 대상

    void Update()
    {
        if (attracted && target != null)
        {
            Vector3 p = transform.position;
            Vector3 t = target.position;
            Vector3 next = Vector3.MoveTowards(p, t, moveSpeed * Time.deltaTime);
            transform.position = next;

            float dist = Vector3.Distance(transform.position, t);
            if (dist <= catchDistance)
            {
                GiveGoldAndDisappear();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerMagnet") == true)
        {
            attracted = true;
            target = other.transform;

            if (goldSystem == null)
            {
                goldSystem = other.GetComponentInParent<GoldSystem>();
                if (goldSystem == null)
                {
                    goldSystem = FindAnyObjectByType<GoldSystem>();
                }
            }
        }
    }

    void GiveGoldAndDisappear()
    {
        if (goldSystem != null)
        {
            goldSystem.AddGold(value);
        }

        if (PoolManager.Instance != null)
        {
            gameObject.SetActive(false); // 단순 버전
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
