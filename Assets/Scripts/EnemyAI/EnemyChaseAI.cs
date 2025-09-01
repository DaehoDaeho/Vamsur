using UnityEngine;

/// <summary>
/// - 플레이어를 향해 이동.
/// - 근접 반경 안이면 주기적으로 접촉 피해.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaseAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.0f;           // 실제 이동은 externalSpeedMultiplier와 곱해 쓰는 게 이상적.
    public float stopDistance = 0.2f;        // 너무 붙으면 더 전진하지 않음.

    [Header("Contact Damage")]
    public int contactDamage = 5;
    public float contactRadius = 0.4f;
    public float damageCooldown = 0.6f;

    [Header("Refs")]
    public Transform player;                 // Player.
    public EnemyCore core;                   // externalSpeedMultiplier 사용.

    private Rigidbody2D _rb;
    private float _damageTimer = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (core == null)
        {
            core = GetComponent<EnemyCore>();
        }
    }

    void Start()
    {
        if (player == null)
        {
            PlayerCore pc = FindAnyObjectByType<PlayerCore>();
            if (pc != null)
            {
                player = pc.transform;
            }
        }
    }

    void Update()
    {
        if (_damageTimer > 0f)
        {
            _damageTimer = _damageTimer - Time.deltaTime;
        }

        if (player == null)
        {
            return;
        }

        // 1) 이동.
        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        float speed = moveSpeed;
        if (core != null)
        {
            speed = moveSpeed * core.externalSpeedMultiplier;
        }

        if (dist > stopDistance)
        {
            Vector2 dir = toPlayer.normalized;
            Vector2 v = dir * speed;
            _rb.velocity = v;
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }

        // 2) 접촉 피해 (원 안에 들어오면)
        if (dist <= contactRadius)
        {
            if (_damageTimer <= 0f)
            {
                // 플레이어 Health에 피해.
                Health h = player.GetComponent<Health>();
                if (h != null)
                {
                    h.TakeDamage(contactDamage, player.transform.position);
                }
                _damageTimer = damageCooldown;
            }
        }
    }
}
