using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int count = 10;
    public float radius = 2.0f;
    public float angularSpeedDeg = 120.0f;

    List<Transform> orbiters = new List<Transform>();
    float baseAngle = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        RebuildOrbiters();
    }

    // Update is called once per frame
    void Update()
    {
        baseAngle = baseAngle + angularSpeedDeg * Time.deltaTime;

        for(int i=0; i<orbiters.Count; ++i)
        {
            float step = 360.0f / Mathf.Max(1, orbiters.Count);
            float ang = baseAngle + step * i;
            float rad = ang * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0.0f) * radius;

            orbiters[i].position = transform.position + offset;
            orbiters[i].right = offset.normalized;
        }
    }

    public void RebuildOrbiters()
    {
        for(int i=0; i<orbiters.Count; ++i)
        {
            if (orbiters[i] != null)
            {
                Destroy(orbiters[i].gameObject);
            }
        }

        orbiters.Clear();

        for(int i=0; i<count; ++i)
        {
            GameObject go = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            orbiters.Add(go.transform);
        }
    }
}
