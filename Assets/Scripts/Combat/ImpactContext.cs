using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class ImpactContext// : MonoBehaviour
{
    public Transform target;    // 맞는 대상의 트랜스폼.
    public Vector3 hitPoint;    // 맞는 위치.
    public int damage;  // 피해량.
    public float magnitude; // 연출의 세기. 1 = 기본, 1.5 = 크리티컬, 2 = 보스.
    public Transform instigator;    // 가해자의 트랜스폼.

    public Vector2 KnockbackDir2D()
    {
        var dir = (target.position - hitPoint);
        return dir.sqrMagnitude > 0.0001f ? (Vector2)dir.normalized : Vector2.zero;
    }
}
