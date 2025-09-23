using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BulletHoming : MonoBehaviour
{
    public float speed = 8f;
    public float turnSpeedDegPerSec = 240f;
    public float lifeTime = 3f;

    public LayerMask targetMask;
    public float seekRadius = 6f;
    public float retargetInterval = 0.25f;

    public int damage = 10;
    public Rigidbody2D rb;

    float lifeLeft = 0f;
    float retargetLeft = 0f;
    Transform currentTarget = null;

    public void Fire(Vector2 position, Vector2 initialDir)
    {
        transform.position = position;

        Vector2 dir = Vector2.right;

        if (initialDir.sqrMagnitude > 0.0001f)
        {
            dir = initialDir.normalized;
        }

        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        lifeLeft = lifeTime;
        retargetLeft = 0f;
        currentTarget = null;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (lifeLeft > 0f)
        {
            lifeLeft -= dt;
            if (lifeLeft <= 0f)
            {
                lifeLeft = 0f;
                Destroy(gameObject);
                return;
            }
        }

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

        Vector2 forward = transform.up;
        float a = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;

        if (currentTarget != null)
        {
            Vector2 toTarget = (Vector2)currentTarget.position - (Vector2)transform.position;

            if (toTarget.sqrMagnitude > 0.0001f)
            {
                toTarget.Normalize();
                float b = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

                float d = DeltaAngleDeg(a, b);

                float maxTurn = turnSpeedDegPerSec * dt;

                float turn = Mathf.Clamp(d, -maxTurn, +maxTurn);

                float newA = a + turn;
                transform.rotation = Quaternion.Euler(0f, 0f, newA - 90f);
            }
        }

        Vector2 move = (Vector2)transform.up * speed * dt;
        rb.MovePosition(rb.position + move);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetMask.value) != 0)
        {
            Health hp = other.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage, hp.transform.position);
            }
        
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
