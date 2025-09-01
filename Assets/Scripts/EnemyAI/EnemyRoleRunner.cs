using UnityEngine;

/// <summary>
/// 러너 스탯 세팅.
/// </summary>
public class EnemyRoleRunner : MonoBehaviour
{
    public EnemyCore core;
    public Health health;

    public float speed = 3.2f;
    public int hp = 20;

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
            core.externalSpeedMultiplier = 1f; // 기본.
        }
        if (health != null)
        {
            health.currentHP = hp;
            health.maxHP = hp;
        }
    }
}