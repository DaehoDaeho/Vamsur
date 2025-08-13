using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDmageable
{
    bool IsAlive { get; }

    void TakeDamage(int amount, Vector3 hitPoint);
}
