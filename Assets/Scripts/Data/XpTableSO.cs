using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Progression/XP Table", fileName = "XpTable")]
public class XpTableSO : ScriptableObject
{
    public int[] requirements;

    /// <summary>
    /// 다음 레벨까지 필요한 경험치를 반환.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetRequirementForLevel(int level)
    {
        int idx = Mathf.Max(0, level - 1);
        if(requirements == null || idx < 0 || idx >= requirements.Length)
        {
            // 테이블 범위를 벗어나면 더 이상 레벨업을 할 수 없음을 처리.
            return int.MaxValue;
        }
        return Mathf.Max(1, requirements[idx]);
    }

    /// <summary>
    /// 최대 레벨을 반환한다.
    /// </summary>
    /// <returns></returns>
    public int GetMaxLevel()
    {
        if(requirements == null)
        {
            return 1;
        }

        return requirements.Length + 1;
    }
}
