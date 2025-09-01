using UnityEngine;

/// <summary>
/// 원거리 스탯 세팅.
/// </summary>
public class EnemyRoleRanged : MonoBehaviour
{
    public EnemyCore core;
    public Health health;

    public float speed = 1.8f;
    public int hp = 35;

    void Awake()
    {
        if (core == null)
        {
            core = GetComponent<EnemyCore>();
        }

        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if (core != null)
        {
            core.moveSpeed = speed;
            core.externalSpeedMultiplier = 1f;
        }
        if (health != null)
        {
            health.currentHP = hp;
            health.maxHP = hp;
        }
    }
}
