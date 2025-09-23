using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BulletSplit : MonoBehaviour
{
    public float speed = 9f;
    public float lifeTime = 2.0f;

    public LayerMask targetMask;
    public int damage = 8;
    public bool destroyOnHit = true;

    public GameObject childBulletPrefab;
    public int splitCount = 7;
    public float spreadDeg = 70f;
    public bool includeCenter = true;

    public Rigidbody2D rb;

    float lifeLeft = 0f;

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
    }

    void Update()
    {
        float dt = Time.deltaTime;

        Vector2 move = (Vector2)transform.up * speed * dt;
        rb.MovePosition(rb.position + move);

        if (lifeLeft > 0f)
        {
            lifeLeft -= dt;
            if (lifeLeft <= 0f)
            {
                lifeLeft = 0f;
                //DoSplit();
                Destroy(gameObject);
                return;
            }
        }
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

            DoSplit();
            if (destroyOnHit == true)
            {
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

        Vector2 forward = transform.up;
        float centerDeg = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;

        if (includeCenter == true)
        {
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
            int half = splitCount / 2;
            float halfSpread = spreadDeg * 0.5f;
            float step = 0f;
            if (half > 0)
            {
                step = halfSpread / (float)half;
            }

            int i = 1;
            while (i <= half)
            {
                float deg = centerDeg - step * i;
                SpawnChild(deg);
                i = i + 1;
            }
            
            i = 1;
            while (i <= half)
            {
                float deg = centerDeg + step * i;
                SpawnChild(deg);
                i = i + 1;
            }

            if ((splitCount % 2) == 1)
            {
                SpawnChild(centerDeg);
            }
        }
    }

    void SpawnChild(float deg)
    {
        float rx = Mathf.Cos(deg * Mathf.Deg2Rad);
        float ry = Mathf.Sin(deg * Mathf.Deg2Rad);
        Vector2 dir = new Vector2(rx, ry).normalized;

        GameObject go = GameObject.Instantiate(childBulletPrefab);
        go.transform.position = transform.position;

        BulletSplit bs = go.GetComponent<BulletSplit>();
        if (bs != null)
        {
            bs.Fire(transform.position, dir);
            return;
        }
    }
}
