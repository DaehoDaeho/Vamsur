using UnityEngine;

/// <summary>
/// 총알이 적에 닿았을 때 Burn/Slow 상태를 '부여'하는 전달자.
/// - ProjectileBullet과 같은 오브젝트에 함께 붙인다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class OnHitStatusApplier : MonoBehaviour
{
    [Header("Target Filter")]
    public LayerMask targetMask; // Enemy 레이어만 체크

    [Header("Burn Settings")]
    public bool applyBurn = true;
    public int burnTickDamage = 2;
    public float burnDuration = 3f;
    public float burnTickInterval = 0.5f;

    [Header("Slow Settings")]
    public bool applySlow = true;
    public float slowPercent = 30f;   // 30% 감속
    public float slowDuration = 2f;

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) 타깃 레이어인지 확인
        int otherLayerBit = 1 << other.gameObject.layer;
        bool layerOk = (targetMask.value & otherLayerBit) != 0;
        if (layerOk == false)
        {
            return;
        }

        // 2) Burn 부여
        if (applyBurn == true)
        {
            BurnStatus burn = other.GetComponent<BurnStatus>();
            if (burn == null)
            {
                burn = other.gameObject.AddComponent<BurnStatus>();
            }
            burn.Apply(burnTickDamage, burnDuration, burnTickInterval);
        }

        // 3) Slow 부여
        if (applySlow == true)
        {
            SlowStatus slow = other.GetComponent<SlowStatus>();
            if (slow == null)
            {
                slow = other.gameObject.AddComponent<SlowStatus>();
            }
            slow.Apply(slowPercent, slowDuration);
        }
    }
}
