using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [SerializeField]
    private WeaponDataSO[] weaponData;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private LayerMask enemyLayerMask;

    private float lastFireTime = -9999.0f;

    private int currentWeaponIndex = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) == true)
        {
            currentWeaponIndex = currentWeaponIndex == 0 ? 1 : 0;
        }

        if(Time.time - lastFireTime >= weaponData[currentWeaponIndex].fireIntervalSeconds)
        {
            TryFire();
        }
    }

    void TryFire()
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
            if (weaponData[currentWeaponIndex].fireEventIfNoTarget == true)
            {
                baseDir = Vector2.right;
            }
            else
            {
                return;
            }
        }

        int n = weaponData[currentWeaponIndex].multishotCount;
        if(n <= 1)
        {
            FireOne(baseDir);
        }
        else
        {
            float step = weaponData[currentWeaponIndex].totalSpreadAngleDegrees / (float)(n - 1);

            float start = -weaponData[currentWeaponIndex].totalSpreadAngleDegrees * 0.5f;

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
        Vector3 spawnPos = transform.position + new Vector3(directionUnit.x, directionUnit.y, 0.0f) * weaponData[currentWeaponIndex].muzzleOffset;

        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        ProjectileStandard p = go.GetComponent<ProjectileStandard>();
        if(p != null)
        {
            p.Setup(directionUnit, weaponData[currentWeaponIndex].damage, weaponData[currentWeaponIndex].projectileSpeed, weaponData[currentWeaponIndex].projectileLifeSeconds, weaponData[currentWeaponIndex].pierceCount);
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weaponData[currentWeaponIndex].detectRadius, enemyLayerMask);

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
