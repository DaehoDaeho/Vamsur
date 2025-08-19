using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyXpReward : MonoBehaviour
{
    public int rewardxp = 5;
    public XpSystem playerxp;
    public Health health;

    private void OnEnable()
    {
        health.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        health.OnDied -= HandleDied;
    }

    private void HandleDied()
    {
        playerxp?.AddExp(rewardxp);
    }
}
