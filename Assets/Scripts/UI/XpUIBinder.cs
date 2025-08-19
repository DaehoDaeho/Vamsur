using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class XpUIBinder : MonoBehaviour
{
    [Header("Reference")]
    public XpSystem xpSystem;
    public Slider xpSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    private void OnEnable()
    {
        if(xpSystem != null)
        {
            xpSystem.OnXpChanged += OnXpChanged;
            xpSystem.OnLevelUp += OnLevelUp;
        }
    }

    private void OnDisable()
    {
        if (xpSystem != null)
        {
            xpSystem.OnXpChanged -= OnXpChanged;
            xpSystem.OnLevelUp -= OnLevelUp;
        }
    }

    private void OnXpChanged(int current, int required, int level)
    {
        float t = (required > 0 && required < int.MaxValue) ? Mathf.Clamp01((float)current / required) : 1.0f;
        xpSlider.minValue = 0.0f;
        xpSlider.maxValue = 1.0f;
        xpSlider.value = t;

        levelText.text = "Lv. " + level.ToString();
        xpText.text = current.ToString() + " / " + required.ToString();
    }

    private void OnLevelUp(int newLevel)
    {
        levelText.text = "Lv. " + newLevel.ToString();
    }    
}
