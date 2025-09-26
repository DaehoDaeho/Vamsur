using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenOrb : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float moveSpeed = 5.0f;
    public float lifeTime = 3.0f;
    public float ringCooldown = 0.15f;
    public int countPerRing = 10;
    public float ringSpeed = 8.0f;
    public float ringLife = 2.0f;
    public int ringDamage = 5;

    private float life;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        life = lifeTime;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + transform.right * moveSpeed * Time.deltaTime;

        life -= Time.deltaTime;
        if(life <= 0.0f)
        {
            Destroy(gameObject);
            return;
        }

        timer -= Time.deltaTime;
        if(timer <= 0.0f)
        {
            FireRing();
            timer = ringCooldown;
        }
    }

    void FireRing()
    {
        float step = 360.0f / Mathf.Max(1, countPerRing);
        for(int i=0; i<countPerRing; ++i)
        {
            float ang = step * i;
            float rad = ang * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            GameObject go = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            if(go != null)
            {
                FrozenOrbProjectile projectile = go.GetComponent<FrozenOrbProjectile>();
                if(projectile != null)
                {
                    projectile.Init(dir, ringSpeed, ringLife, ringDamage);
                }
            }
        }
    }
}
