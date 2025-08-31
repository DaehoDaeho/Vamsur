using UnityEngine;

/// <summary>
/// Burn(지속 피해) 상태
/// - tickInterval마다 tickDamage를 준다.
/// - duration이 끝나면 상태를 제거한다.
/// - 재적용 시: duration 새로 설정, tickDamage는 더 큰 값 유지.
/// </summary>
public class BurnStatus : MonoBehaviour
{
    [Header("Runtime")]
    public int tickDamage = 1;
    public float duration = 0f;
    public float tickInterval = 0.5f;

    private float _tickTimer = 0f;

    public void Apply(int newTickDamage, float newDuration, float newTickInterval)
    {
        // 틱 대미지는 더 큰 값 유지
        if (newTickDamage > tickDamage)
        {
            tickDamage = newTickDamage;
        }

        // 틱 간격은 새 값 사용(수업 단순화를 위해)
        tickInterval = newTickInterval;

        // 남은 시간은 새로 설정(리프레시)
        duration = newDuration;

        // 바로 틱이 들어가지 않게, 타이머를 초기화
        _tickTimer = tickInterval;
    }

    void Update()
    {
        if (duration > 0f)
        {
            duration = duration - Time.deltaTime;

            _tickTimer = _tickTimer - Time.deltaTime;
            if (_tickTimer <= 0f)
            {
                // 데미지 1틱
                Health h = GetComponent<Health>();
                if (h != null)
                {
                    h.TakeDamage(tickDamage, h.transform.position);
                }
                _tickTimer = tickInterval;
            }
        }
        else
        {
            // 시간 종료 → 상태 제거
            Destroy(this);
        }
    }
}
