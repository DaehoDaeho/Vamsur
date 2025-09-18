using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostDestroyer : MonoBehaviour
{
    private float lifetime = 0.3f;
    private float timer = 0.0f;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float t = timer / lifetime;
        if(t > 1.0f)
        {
            t = 1.0f;
        }

        Color color = sr.color;
        color.a = 1.0f - t;
        sr.color = color;

        if(timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetLifetime(float seconds)
    {
        lifetime = seconds;
    }
}
