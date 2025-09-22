using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ���� ������(ü�� ����Ʈ��ó�� ����)
/// - ���� ��󿡼� �ݰ�(chainRadius) �� "���� �����" ���� ������� ����
/// - �ִ� hops �� ��ũ�� �̾�� �� ��ũ���� ����� ����
/// - LineRenderer�� ��ü ��ũ ��θ� ª�� ǥ��
/// ��Ģ:
///  - ��� if�� ��� ���
///  - �Ҹ��� �񱳴� == true/false
///  - ������ �� ������ھ� ����
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ChainLaserController : MonoBehaviour
{
    [Header("Chain Settings")]
    public LayerMask damageMask;       // �¾ƾ� �� ��� ���̾�
    public float chainRadius = 4.0f;   // ���� ����� ã�� Ž�� �ݰ�
    public int hops = 4;               // �ִ� ���� Ƚ��(��ũ ��)
    public int damagePerHop = 20;      // ��ũ �ϳ��� �� �����
    public float linkLife = 0.1f;      // ������ ���̴� �ð�(��)

    LineRenderer lr;
    float timer = 0f;
    bool active = false;
    List<Vector3> linkPoints = new List<Vector3>(); // ĳ���͡�ù ����� ��° ����...

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        lr.positionCount = 0;

        // ��� ������ ����
        lr.startWidth = 0.08f;
        lr.endWidth = 0.04f;
    }

    /// <summary>
    /// ���� "��"�� �ִ� ���: �ش� ������ ���� ����� ù ��� ã��
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
    /// ���� "���"�� ������ ���� ��: �� ��󿡼� ���� ����
    /// </summary>
    public void FireChain(Transform firstTarget)
    {
        if (firstTarget == null)
        {
            return;
        }

        // ���� ��� ����
        linkPoints.Clear();

        // ĳ���� �� ù ��� �� �߰�
        linkPoints.Add(transform.position);
        linkPoints.Add(firstTarget.position);

        // ù ��� ��� �����
        DealDamageIfPossible(firstTarget);

        // �̹� ���� ��� ����(�ߺ� ����)
        HashSet<Transform> visited = new HashSet<Transform>();
        visited.Add(firstTarget);

        int i = 0;
        Transform current = firstTarget;

        // �ִ� hops �� �ݺ�
        while (i < hops)
        {
            // ���� ��� ���� �ݰ� �� "���� �����" ���� ��� �˻�
            Collider2D next = FindNearestTarget(current.position, chainRadius, visited);
            if (next == null)
            {
                // �� �̻� ���� �Ұ�(�ݰ� �� ����)
                break;
            }

            Transform nextTf = next.transform;

            // ��ο� �� �߰�(���� �� ����)
            linkPoints.Add(nextTf.position);

            // ����� 1ȸ
            DealDamageIfPossible(nextTf);

            // �湮 ��� �� ���� ����
            visited.Add(nextTf);
            current = nextTf;

            i = i + 1;
        }

        // ���� ǥ��(��ü ��� �� ����)
        lr.enabled = true;
        lr.positionCount = linkPoints.Count;

        int p = 0;
        while (p < linkPoints.Count)
        {
            lr.SetPosition(p, linkPoints[p]);
            p = p + 1;
        }

        // ���� ª�� ǥ���ϰ� ����(ȭ�� ���� ����)
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
    /// �ݰ� �� �ĺ� �� "���� �����" ��� ã��
    /// exclude�� �̹� ���� ������ ������ ����
    /// �Ÿ� �񱳴� �����Ÿ�(��Ʈ ��� ����)
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

                // �����Ÿ� ���: d2 = |p - q|^2
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
