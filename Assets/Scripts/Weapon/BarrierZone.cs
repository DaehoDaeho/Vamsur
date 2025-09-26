using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierZone : MonoBehaviour
{
    public int damage = 5;
    public float hitCooldownSec = 0.2f;
    public GameObject barrierCollisionPrefab;

    Dictionary<Transform, float> nextTickTime = new Dictionary<Transform, float>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") == true)
        {
            TryHit(collision.transform);
        }
        else if(collision.CompareTag("BulletEnemy") == true)
        {
            GameObject go = Instantiate(barrierCollisionPrefab, collision.transform.position, Quaternion.identity);
            if (go != null)
            {
                Destroy(go, 1.0f);
            }

            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") == true)
        {
            TryHit(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") == true)
        {
            if(nextTickTime.ContainsKey(collision.transform) == true)
            {
                nextTickTime.Remove(collision.transform);
            }
        }
    }

    void TryHit(Transform enemy)
    {
        float now = Time.time;

        if (nextTickTime.ContainsKey(enemy) == false)
        {
            nextTickTime.Add(enemy, 0.0f);
        }

        if (now >= nextTickTime[enemy])
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
