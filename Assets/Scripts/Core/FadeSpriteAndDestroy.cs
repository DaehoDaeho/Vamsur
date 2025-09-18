using UnityEngine;

public class FadeSpriteAndDestroy : MonoBehaviour
{
    private float lifetime = 0.25f;
    private float timer = 0.0f;
    private SpriteRenderer r;

    private void Awake()
    {
        r = GetComponent<SpriteRenderer>();
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

        Color color = r.color;
        color.a = 1.0f - t;
        r.color = color;

        if(timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetLifeTime(float seconds)
    {
        lifetime = seconds;
    }    
}
