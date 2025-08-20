using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 레벨 업 시 업그레이드 카탈로그에서 3장을 뽑아서 팝업 UI에 전달
/// </summary>
public class LevelupPicker : MonoBehaviour
{
    public XpSystem xpSystem;
    public UpgradeCatalogSO catalog;
    public UpgradePopupUI popupUI;

    private System.Random rng = new System.Random();

    private void Awake()
    {
        if(xpSystem == null)
        {
            xpSystem = FindAnyObjectByType<XpSystem>();
        }

        if(popupUI == null)
        {
            popupUI = FindAnyObjectByType<UpgradePopupUI>();
        }
    }

    private void OnEnable()
    {
        if(xpSystem != null)
        {
            xpSystem.OnLevelUp += HandleLevelUp;
        }
    }

    private void OnDisable()
    {
        if (xpSystem != null)
        {
            xpSystem.OnLevelUp -= HandleLevelUp;
        }
    }

    void HandleLevelUp(int newLevel)
    {
        var picks = PickUnique(3);
        popupUI?.ShowChoices(picks);
    }

    List<UpgradeDefinitionSO> PickUnique(int count)
    {
        var result = new List<UpgradeDefinitionSO>(count);
        int guard = 50;

        while(result.Count < count && guard-- > 0)
        {
            var u = catalog.RollOne(rng);
            if(u == null)
            {
                break;
            }
            if(result.Contains(u) == false)
            {
                result.Add(u);
            }
        }

        return result;
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
