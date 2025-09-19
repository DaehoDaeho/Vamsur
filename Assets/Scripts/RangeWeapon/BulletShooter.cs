using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    public LineBullet bulletPrefab;
    public int poolSize = 30;
    public float shotSpeed = 10.0f;

    List<LineBullet> pool = new List<LineBullet>();

    private void Awake()
    {
        for(int i=0; i<poolSize; ++i)
        {
            LineBullet lb = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
            lb.gameObject.SetActive(false);
            lb.speed = shotSpeed;
            pool.Add(lb);
        }
    }

    LineBullet PopBullet()
    {
        for(int i=0; i<pool.Count; ++i)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                return pool[i];
            }
        }

        LineBullet b2 = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
        b2.gameObject.SetActive(false);
        b2.speed = shotSpeed;
        pool.Add(b2);

        return b2;
    }

    public void Fire(int count)
    {
        float dTheta = (Mathf.PI * 2.0f) / (float)count;

        for(int i=0; i<count; ++i)
        {
            float theta = dTheta * i;
            Vector2 dir;
            dir.x = Mathf.Cos(theta);
            dir.y = Mathf.Sin(theta);

            LineBullet b = PopBullet();
            b.gameObject.SetActive(true);
            b.Fire(transform.position, dir);
        }
    }
}
