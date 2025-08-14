using UnityEngine;

/// <summary>
/// 타격 피드백 총괄: Health.OnDamaged를 받아서
/// Flash/VFX/SFX/Knockback/CameraShake를 실행.
/// 피격 대상(플레이어/적) 프리팹에 부착.
/// </summary>
[RequireComponent(typeof(Health))]
public class ImpactOrchestrator : MonoBehaviour
{
    public DamageFlash flash;              // 없으면 생략
    public KnockbackReceiver knockback;    // 없으면 생략

    public CameraShake2D cameraShake;      // 메인 카메라에 부착된 것 참조
        
    public float magnitudeByHpShare = 0.5f; // 피해%에 대한 셰이크/넉백 스케일 기여
    public float baseKnockback = 4f;              // 기본 넉백 속도
    public bool shakeOnHit = true;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (flash == null)
        {
            flash = GetComponentInChildren<DamageFlash>();
        }
            
        if (knockback == null)
        {
            knockback = GetComponent<KnockbackReceiver>();
        }

        if (cameraShake == null)
        {
            cameraShake = FindAnyObjectByType<CameraShake2D>();
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDamaged += HandleDamaged;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDamaged -= HandleDamaged;
        }   
    }

    private void HandleDamaged(int amount, Vector3 hitPoint)
    {
        // === 1) 컨텍스트 만들기 ===
        var ctx = new ImpactContext
        {
            target = transform,
            hitPoint = hitPoint,
            damage = amount,
            magnitude = ComputeMagnitude(amount),
            instigator = null // 필요하면 AutoAttack 쪽에서 채워넣도록 확장 가능
        };

        // === 2) 피격 점멸 ===
        //if(flash != null)
        //{
        //    flash.PlayFlash();
        //}

        flash?.PlayFlash();

        // === 5) 넉백 ===
        if (knockback != null)
        {
            Vector2 dir = ctx.KnockbackDir2D();
            float strength = baseKnockback * ctx.magnitude;
            knockback.ApplyKnockback(dir * strength);
        }

        // === 6) 카메라 셰이크 ===
        if (shakeOnHit && cameraShake != null)
        {
            // 데미지 비율이 높을수록 조금 더 큰 셰이크
            float amp = 0.05f + 0.1f * ctx.magnitude;
            float dur = 0.07f + 0.05f * ctx.magnitude;
            cameraShake.Shake(amp, dur);
        }
    }

    /// <summary>피해량이 클수록 연출 세기를 키우는 간단 규칙.</summary>
    private float ComputeMagnitude(int damage)
    {
        // 최대체력 대비 피해 비율(0~1) * 가중치
        // Health의 max값을 알 수 없으니, OnHPChanged로 캐시하거나
        // 간단히 피해량 10당 0.1 스케일 같은 규칙도 가능. 여기서는 단순화:
        float byDamage = Mathf.Clamp01(damage / 20f); // 20데미지 = 1.0
        return Mathf.Clamp01(byDamage * magnitudeByHpShare + 0.5f * (1f - magnitudeByHpShare));
    }
}
