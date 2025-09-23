using UnityEngine;

/// <summary>
/// ���� �Ѿ� ������ ���� ����ź ��ũ��Ʈ(����/Ʈ���� �̻��)
/// ��Ģ: ��� if ��� ���, �Ҹ��� �񱳴� == true/false, ������ھ� �̻��
/// �ٽ� ���̵��:
///  - retargetInterval���� �ݰ� seekRadius �ȿ��� ���� ����� ���� ã�´�.
///  - ���� �� a �� ��ǥ �� b�� '�����Ӵ� ȸ�� �ѵ�'��ŭ�� ȸ��(�ε巯�� ����).
///  - transform.up�� �������� ����ϹǷ�, ��������Ʈ�� '����'�� �Ѿ��� ���� �ǵ��� ����.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class HomingBullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;                   // �ʴ� �̵� �ӵ�
    public float turnSpeedDegPerSec = 240f;    // �ʴ� ȸ�� ������ �ִ� ����(��). �ʹ� ũ�� ��ȸ��.
    public float lifeTime = 3f;                // �Ѿ� ����(��)

    [Header("Targeting")]
    public LayerMask targetMask;               // �� ���̾�
    public float seekRadius = 6f;              // ��ǥ Ž�� �ݰ�(���� ����)
    public float retargetInterval = 0.25f;     // ��ǥ ��˻� �ֱ�(��) - ������ ���� ����Ʋ��

    [Header("Damage")]
    public int damage = 10;                    // ���� ����� �� �� �����

    Rigidbody2D rb;
    float lifeLeft = 0f;
    float retargetLeft = 0f;
    Transform currentTarget = null;

    /// <summary>
    /// �Ѿ� �߻�(�ܺο��� �� �ٷ� ȣ��)
    /// </summary>
    public void Fire(Vector2 position, Vector2 initialDir)
    {
        transform.position = position;

        // �ʱ� ������ 0�� �����ٸ� �������� �⺻������ ���
        Vector2 dir = Vector2.right;

        if (initialDir.sqrMagnitude > 0.0001f)
        {
            dir = initialDir.normalized;
        }

        // ��������Ʈ�� 'up(����)'�� ���� �������� �����.
        // 2D���� ������ transform.up���� ����� �����Ƿ� -90�� ����.
        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        // ����/Ÿ�� Ÿ�̸� �ʱ�ȭ
        lifeLeft = lifeTime;
        retargetLeft = 0f;
        currentTarget = null;

        //gameObject.SetActive(true);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // �����Ϳ��� Rigidbody2D�� GravityScale=0, Interpolate, Continuous ���� ����
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // (1) ���� ����
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

        // (2) ��ǥ ��˻�(�ʹ� ���� ���� �ʵ��� �ֱ� ���)
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

        // (3) ����: ���� ������ǥ �������� '������' ȸ��
        Vector2 forward = transform.up; // ���� ����(���� ����)
        float a = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg; // ���� ����(��)

        if (currentTarget != null)
        {
            Vector2 toTarget = (Vector2)currentTarget.position - (Vector2)transform.position;

            // ��ǥ�� �ʹ� ������� ������ �ָ��ϸ� ȸ������ ����
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                toTarget.Normalize();
                float b = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg; // ��ǥ ����(��)

                // ���� ���̸� -180~+180���� ����ȭ(ª�� �������� ����)
                float d = DeltaAngleDeg(a, b);

                // �̹� ������ ��� ȸ���� = turnSpeedDegPerSec * dt
                float maxTurn = turnSpeedDegPerSec * dt;

                // ���� ȸ������ -maxTurn ~ +maxTurn���� ����
                float turn = Mathf.Clamp(d, -maxTurn, +maxTurn);

                // �� ���� ���� (up ���� ���� -90��)
                float newA = a + turn;
                transform.rotation = Quaternion.Euler(0f, 0f, newA - 90f);
            }
        }

        // (4) �̵�: �������� speed*dt��ŭ �̵� (���� ������ ���� MovePosition ���)
        Vector2 move = (Vector2)transform.up * speed * dt;
        rb.MovePosition(rb.position + move);
    }

    // �浹(Trigger ����)
    void OnTriggerEnter2D(Collider2D other)
    {
        // ��� ���̾�� ����
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
                // �����Ÿ�: sqrt ���� ���� �Ÿ� �� ���� �� ����
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
        // ���� ���̸� -180~+180 ������ ���ߴ� �Լ�(ª�� ȸ�� ���� ����)
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
