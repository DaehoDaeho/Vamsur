using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OribtProjectile : MonoBehaviour
{
    public int damage = 5;
    public float hitCooldownSec = 0.2f;

    Dictionary<Transform, float> nextTickTime = new Dictionary<Transform, float>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") == true)
        {
            TryHit(collision.transform);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") == true)
        {
            TryHit(collision.transform);
        }
    }

    void TryHit(Transform enemy)
    {
        float now = Time.time;

        if(nextTickTime.ContainsKey(enemy) == false)
        {
            nextTickTime.Add(enemy, 0.0f);
        }

        if(now >= nextTickTime[enemy])
        {
            Health hp = enemy.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage, hp.transform.position);
            }

            nextTickTime[enemy] = now + hitCooldownSec;
        }
    }
}
