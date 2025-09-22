using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Game/BulletData")]
public class BulletDataSO : ScriptableObject
{
    public float fireIntervalSeconds = 0.5f;
    public float detectRadius = 5.0f;
    public float damage = 20.0f;
    public float projectileSpeed = 10.0f;
    public float projectileLifeSeconds = 3.0f;
    public int pierceCount = 0;
    public float muzzleOffset = 0.6f;
    public int multishotCount = 1;
    public float totalSpreadAngleDegrees = 0.0f;
    public bool fireEvenIfNoTarget = false;
}
