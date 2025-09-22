using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    public float fireIntervalSeconds = 0.5f;
    public float detectRadius = 8.0f;
    public float damage = 10.0f;
    public float projectileSpeed = 10.0f;
    public float projectileLifeSeconds = 3.0f;
    public int pierceCount = 0;
    public float muzzleOffset = 0.6f;
    public int multishotCount = 1;
    public float totalSpreadAngleDegrees = 0.0f;
    public bool fireEventIfNoTarget = false;
}
