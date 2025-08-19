using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 레벨과 경험치를 관리하고 경험치가 차면 레벨업을 처리하는 클래스.
/// </summary>
public class XpSystem : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private XpTableSO xpTable; // 유니티의 Inspector에는 노출이 되지만, 다른 스크립트에서는 접근 불가.

    [Header("State (읽기전용 접근용 프로퍼티 제공")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXp = 0;

    public int CurrentLevel => currentLevel;
    public int CurrentXp => currentXp;
    public int MaxLevel => xpTable ? xpTable.GetMaxLevel() : 1;

    public Action<int, int, int> OnXpChanged;
    public Action<int> OnLevelUp;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NotifyXpChaged();
    }

    public void AddExp(int amount)
    {
        if(amount <= 0)
        {
            return;
        }

        if(currentLevel >= MaxLevel)
        {
            return;
        }

        currentXp += amount;

        // 연속 레벨업 처리.
        while(currentLevel < MaxLevel)
        {
            int required = GetRequiredXp();
            if(currentXp < required)
            {
                break;
            }

            currentXp -= required;  // 잉여 경험치는 다음 레벨로 이월.
            currentLevel++;
            OnLevelUp?.Invoke(currentLevel);

            if(currentLevel >= MaxLevel)
            {
                currentXp = 0;
                break;
            }
        }

        // 경험치 변화를 통지.
        NotifyXpChaged();
    }

    public int GetRequiredXp()
    {
        if(xpTable == null)
        {
            return int.MaxValue;
        }

        return xpTable.GetRequirementForLevel(currentLevel);
    }

    private void NotifyXpChaged()
    {
        OnXpChanged?.Invoke(currentXp, GetRequiredXp(), currentLevel);
    }
}
