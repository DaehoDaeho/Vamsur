using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenOrbProjectile : MonoBehaviour
{
    public float speed = 10.0f;
    public float life = 3.0f;
    public int damage = 5;

    Vector2 direction = Vector2.right;

    public void Init(Vector2 dir, float spd, float lifeSec, int dmg)
    {
        direction = dir.normalized;
        speed = spd;
        life = lifeSec;
        damage = dmg;

        transform.right = direction;
    }

    private void Update()
    {
        transform.position = transform.position + (Vector3)(direction * speed * Time.deltaTime);
        life -= Time.deltaTime;
        if(life <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") == true)
        {
            Health hp = collision.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage, hp.transform.position);
            }

            Destroy(gameObject);
        }
    }
}
