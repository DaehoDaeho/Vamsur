using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerMagnet : MonoBehaviour
{
    public float radius = 2.5f;

    void Reset()
    {
        CircleCollider2D cc = GetComponent<CircleCollider2D>();
        cc.isTrigger = true;
        cc.radius = radius;
        // �� ������Ʈ�� Tag�� �ݵ�� "PlayerMagnet"�̾�� �ؿ�.
        gameObject.tag = "PlayerMagnet";
    }

    void OnValidate()
    {
        CircleCollider2D cc = GetComponent<CircleCollider2D>();
        if (cc != null)
        {
            cc.radius = radius;
        }
    }
}
