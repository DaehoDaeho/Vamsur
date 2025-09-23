using UnityEngine;

/// <summary>
/// ���� �Ѿ� ������ ���� �п�ź ��ũ��Ʈ
/// - �����ϴٰ� '�浹' �Ǵ� '���� ����' �� ���� ���� ������ �߽����� N���� ����.
/// - ���� ��ġ: start~end ������ '�յ� �й�' �� ���� ������ ����.
/// ��Ģ: ��� if ��� ���, �Ҹ��� �񱳴� == true/false
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
    public GameObject childBulletPrefab; // ������ �ڽ� �Ѿ�(���� ������ or ����ź ������)
    public int splitCount = 7;           // ���� ����
    public float spreadDeg = 70f;        // ��ü Ȯ�� ����
    public bool includeCenter = true;    // �߾� ���� ���� ����

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

        // ������ ���� �������� (up ���� ���� -90��)
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

        // ���� �̵�
        Vector2 move = (Vector2)transform.up * speed * dt;
        rb.MovePosition(rb.position + move);

        // ���� üũ
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

        // �� ���̾ ó��
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

        // �߽� ����: ���� ����(up)
        Vector2 forward = transform.up;
        float centerDeg = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;

        if (includeCenter == true)
        {
            // ����~���� count�� ��� �� �߾� ���� ����
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
            // �߾��� ���� �¿�� ���� �й�
            int half = splitCount / 2;
            float halfSpread = spreadDeg * 0.5f;
            float step = 0f;
            if (half > 0)
            {
                step = halfSpread / (float)half;
            }

            // ����
            int i = 1;
            while (i <= half)
            {
                float deg = centerDeg - step * i;
                SpawnChild(deg);
                i = i + 1;
            }
            // ������
            i = 1;
            while (i <= half)
            {
                float deg = centerDeg + step * i;
                SpawnChild(deg);
                i = i + 1;
            }

            // Ȧ���� �߾� �ϳ� �߰�
            if ((splitCount % 2) == 1)
            {
                SpawnChild(centerDeg);
            }
        }
    }

    void SpawnChild(float deg)
    {
        // ���� �� ����
        float rx = Mathf.Cos(deg * Mathf.Deg2Rad);
        float ry = Mathf.Sin(deg * Mathf.Deg2Rad);
        Vector2 dir = new Vector2(rx, ry).normalized;

        GameObject go = GameObject.Instantiate(childBulletPrefab);
        go.transform.position = transform.position;

        // �ڽ� �Ѿ��� Ÿ�Կ� ���� Fire ȣ��
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

        // ������Ʈ�� ���� ����ź ��ũ��Ʈ�� ���� �ִٸ� ���⿡ �б� �߰�
        // ExampleBasicBullet ex = go.GetComponent<ExampleBasicBullet>();
        // if (ex != null) { ex.Fire(transform.position, dir); return; }
    }
}
