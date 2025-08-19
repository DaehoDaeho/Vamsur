using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIBinderFilled : MonoBehaviour
{
    [Header("ÂüÁ¶")]
    public Health targetHealth;
    public Image fillImage;

    private void Reset()
    {
        if (targetHealth == null)
        {
            targetHealth = GetComponent<Health>();
        }

        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHPChanged += OnHPChanged;
        }
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
        fillImage.fillAmount = t;
    }
}
