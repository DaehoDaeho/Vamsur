using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade/Catalog", fileName = "UpgradeCatalog")]
public class UpgradeCatalogSO : ScriptableObject
{
    public UpgradeDefinitionSO[] all;

    /// 가중치 합을 구함.
    /// 
    public int TotalWeight()
    {
        int sum = 0;
        if(all != null)
        {
            foreach(var u in all)
            {
                if(u != null && u.weight > 0)
                {
                    sum += u.weight;
                }
            }
        }

        return Mathf.Max(1, sum);
    }

    /// <summary>
    /// 가중치 기반으로 1장을 뽑느다.
    /// </summary>
    /// <param name="rng"></param>
    /// <returns></returns>
    public UpgradeDefinitionSO RollOne(System.Random rng)
    {
        if (all == null || all.Length == 0)
        {
            return null;
        }

        int total = TotalWeight();
        int pick = rng.Next(0, total);
        int acc = 0;
        foreach(var u in all)
        {
            if (u == null || u.weight <= 0)
            {
                continue;
            }

            acc += u.weight;
            if(pick < acc)
            {
                return u;
            }
        }

        return all[0];
    }
}
