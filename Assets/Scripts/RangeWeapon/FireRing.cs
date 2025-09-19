using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FireRing : MonoBehaviour
{
    public int circlePoints = 64;
    public float startRadius = 0.2f;
    public float endRadius = 4.0f;
    public float duration = 0.5f;
    public float width = 0.15f;
    public Color colorStart = new Color(1.0f, 0.5f, 0.2f, 0.9f);
    public Color colorEnd = new Color(1.0f, 0.5f, 0.2f, 0.0f);

    public int damage = 40;
    public LayerMask targetMask;

    public LineRenderer lr;
    float tElapsed = 0.0f;
    bool running = false;
    Vector2 centerPos;

    private void Awake()
    {
        lr.positionCount = circlePoints;
        lr.widthMultiplier = 1.0f;
        lr.startWidth = width;
        lr.endWidth = width;        

        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] { new GradientColorKey(colorStart, 0.0f), new GradientColorKey(colorStart, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(colorStart.a, 0.0f), new GradientAlphaKey(colorEnd.a, 1.0f) }
            );

        lr.colorGradient = g;
        lr.enabled = false;
    }

    public void Play(Vector2 center)
    {
        centerPos = center;
        tElapsed = 0.0f;
        running = true;
        lr.enabled = true;
        UpdateRing(startRadius);
    }

    void UpdateRing(float radius)
    {
        float theta = 0.0f;
        for(int i=0; i<circlePoints; ++i)
        {
            theta = (Mathf.PI * 2.0f) * ((float)i / (float)circlePoints);

            float cx = Mathf.Cos(theta);
            float cy = Mathf.Sin(theta);

            Vector3 p;
            p.x = centerPos.x + cx * radius;
            p.y = centerPos.y + cy * radius;
            p.z = 0.0f;

            lr.SetPosition(i, p);
        }
    }

    void ApplyDamage(Vector2 center, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, targetMask);
        for(int i=0; i<hits.Length; ++i)
        {
            Health hp = hits[i].GetComponent<Health>();
            if(hp != null)
            {
                hp.TakeDamage(damage, hits[i].transform.position);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(running == true)
        {
            tElapsed += Time.deltaTime;
            float ratio = 0.0f;
            if(duration > 0.0f)
            {
                ratio = Mathf.Clamp01(tElapsed / duration);
            }

            float r = Mathf.Lerp(startRadius, endRadius, ratio);
            UpdateRing(r);

            if(ratio >= 1.0f)
            {
                running = false;
                lr.enabled = false;
                ApplyDamage(centerPos, endRadius);
            }
        }
    }
}
