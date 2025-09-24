using UnityEngine;
using System.Collections.Generic;

public class WorldTintController : MonoBehaviour
{
    public DayNightController dayNight;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.6f, 0.65f, 0.8f, 1f);

    List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    void Awake()
    {
        renderers.Clear();
        SpriteRenderer[] arr = GetComponentsInChildren<SpriteRenderer>(true);
        int i = 0;
        while (i < arr.Length)
        {
            if (arr[i] != null)
            {
                renderers.Add(arr[i]);
            }
            i = i + 1;
        }
    }

    void Update()
    {
        if (dayNight == null)
        {
            return;
        }
        float nightFactor = dayNight.GetNightFactor();
        Color c = Color.Lerp(dayColor, nightColor, nightFactor);

        int i = 0;
        while (i < renderers.Count)
        {
            SpriteRenderer sr = renderers[i];
            if (sr != null)
            {
                Color baseC = sr.color;
                Color newC = new Color(c.r, c.g, c.b, baseC.a);
                sr.color = newC;
            }
            i = i + 1;
        }
    }
}
