using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [SerializeField]
    private BulletDataSO weaponData;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private LayerMask enemyLayerMask;

    private float lastFireTime = -9999.0f;

    private float attackSpeedMultiplier = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastFireTime >= weaponData.fireIntervalSeconds)
        {
            TryFire();
        }
    }

    public void TryFire()
    {
        Transform nearest = FindNearestEnemy();

        Vector2 baseDir;

        if(nearest != null)
        {
            Vector3 dir3 = nearest.position - transform.position;
            baseDir = new Vector2(dir3.x, dir3.y).normalized;
        }
        else
        {
            if (weaponData.fireEvenIfNoTarget == true)
            {
                baseDir = Vector2.right;
            }
            else
            {
                return;
            }
        }

        int n = weaponData.multishotCount;
        if(n <= 1)
        {
            FireOne(baseDir);
        }
        else
        {
            float step = weaponData.totalSpreadAngleDegrees / (float)(n - 1);

            float start = -weaponData.totalSpreadAngleDegrees * 0.5f;

            for(int i=0; i<n; ++i)
            {
                float angle = start + step * i;

                Vector2 dir = Rotate2D(baseDir, angle);
                FireOne(dir);
            }
        }

        lastFireTime = Time.time;
    }

    void FireOne(Vector2 directionUnit)
    {
        Vector3 spawnPos = transform.position + new Vector3(directionUnit.x, directionUnit.y, 0.0f) * weaponData.muzzleOffset;

        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        ProjectileStandard p = go.GetComponent<ProjectileStandard>();
        if(p != null)
        {
            p.Setup(directionUnit, weaponData.damage, weaponData.projectileSpeed, weaponData.projectileLifeSeconds, weaponData.pierceCount);
        }
    }

    Vector2 Rotate2D(Vector2 v, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        float x = v.x * cos - v.y * sin;
        float y = v.x * sin + v.y * cos;

        return new Vector2(x, y).normalized;
    }

    Transform FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weaponData.detectRadius, enemyLayerMask);

        Transform best = null;
        float bestSqr = 0.0f;

        for(int i=0; i<hits.Length; ++i)
        {
            Health enemy = hits[i].GetComponent<Health>();
            if(enemy != null)
            {
                Vector3 diff = hits[i].transform.position - transform.position;
                float d2 = diff.sqrMagnitude;

                if(best == null)
                {
                    best = hits[i].transform;
                    bestSqr = d2;
                }
                else
                {
                    if(d2 < bestSqr)
                    {
                        best = hits[i].transform;
                        bestSqr = d2;
                    }
                }
            }
        }

        return best;
    }
}
