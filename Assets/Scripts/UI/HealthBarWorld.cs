using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarWorld : MonoBehaviour
{
    [Header("ÂüÁ¶")]
    public Health targetHealth;
    public Image fillImage;
    public Slider slider;

    private void OnEnable()
    {
        if(targetHealth == null)
        {
            enabled = false;
            return;
        }

        targetHealth.OnHPChanged += OnHPChanged;
    }

    private void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHPChanged -= OnHPChanged;
        }
    }

    void OnHPChanged(int current, int max)
    {
        float t = max > 0 ? Mathf.Clamp01((float)current / max) : 0.0f;

        if(fillImage != null)
        {
            fillImage.fillAmount = t;
        }

        if(slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = Mathf.Max(max, 1);
            slider.value = Mathf.Clamp(current, 0, slider.maxValue);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
