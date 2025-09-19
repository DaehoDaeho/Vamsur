using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineBullet : MonoBehaviour
{
    public float speed = 12.0f;
    public float life = 1.5f;
    public float length = 0.3f;
    public int damage = 10;
    public LayerMask hitMask;
    public LineRenderer lr;

    Vector2 dir = Vector2.right;
    float timer = 0.0f;

    public void Fire(Vector2 position, Vector2 direction)
    {
        transform.position = position;
        dir = direction.normalized;
        lr.enabled = true;
        timer = life;
        UpdateLineRendererImmediate();
    }

    void UpdateLineRendererImmediate()
    {
        lr.positionCount = 2;

        Vector3 start = transform.position;
        Vector3 end = start - (Vector3)(dir * length);

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    private void OnDisable()
    {
        lr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0.0f)
        {
            float dt = Time.deltaTime;
            timer -= dt;
            if(timer <= 0.0f)
            {
                lr.enabled = false;
                gameObject.SetActive(false);
                return;
            }

            Vector3 p = transform.position;
            p.x = p.x + dir.x * speed * dt;
            p.y = p.y + dir.y * speed * dt;
            transform.position = p;

            float radius = 0.15f;
            Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, hitMask);
            if(hit != null)
            {
                Health hp = hit.GetComponent<Health>();
                if (hp != null)
                {
                    hp.TakeDamage(damage, hit.transform.position);
                }

                lr.enabled = false;
                gameObject.SetActive(false);
                return;
            }

            UpdateLineRendererImmediate();
        }
    }
}
