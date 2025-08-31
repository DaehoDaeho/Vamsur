using UnityEngine;

/// <summary>
/// Slow(감속) 상태
/// - 적용 시: externalSpeedMultiplier *= (1 - slowPercent/100)
/// - 종료 시: externalSpeedMultiplier /= 적용했던 배율   (정확 원복)
/// - 재적용: 더 센 퍼센트면 배율 갈아끼우고, 아니면 시간만 리프레시
/// </summary>
public class SlowStatus : MonoBehaviour
{
    [Header("Runtime")]
    public float currentPercent = 0f;  // 현재 적용 중인 퍼센트(0~100)
    public float duration = 0f;

    private float appliedFactor = 1f; // 곱해둔 배율(예: 0.7)
    private bool applied = false;

    private EnemyCore core;           // 이동 속도를 실제로 사용하는 곳

    void Awake()
    {
        core = GetComponent<EnemyCore>();
        // EnemyCore가 없다면 감속을 적용할 수 없다(적용은 그냥 무시).
        // 필요하면 Player에도 비슷하게 적용할 수 있다.
    }

    public void Apply(float newPercent, float newDuration)
    {
        if (core == null)
        {
            // 적용 대상에 EnemyCore가 없으면 아무 것도 하지 않음
            return;
        }

        float clamped = Mathf.Clamp(newPercent, 0f, 95f); // 100%는 멈춤이라 과도, 95% 정도로 제한
        float newFactor = 1f - (clamped / 100f);

        if (applied == false)
        {
            // 처음 적용
            MultiplySpeed(newFactor);   // 곱하기
            appliedFactor = newFactor; // 기억
            currentPercent = clamped;
            duration = newDuration;
            applied = true;
        }
        else
        {
            // 이미 어떤 슬로우가 적용 중
            if (clamped > currentPercent)
            {
                // 더 센 슬로우로 교체: 먼저 이전 배율을 되돌리고, 새 배율을 곱한다
                DivideSpeed(appliedFactor);   // 되돌리기(나누기)
                MultiplySpeed(newFactor);      // 새로 곱하기
                appliedFactor = newFactor;
                currentPercent = clamped;
                duration = newDuration;        // 시간 리프레시
            }
            else
            {
                // 더 약하거나 같으면 시간만 리프레시
                duration = newDuration;
            }
        }
    }

    void Update()
    {
        if (applied == true)
        {
            if (duration > 0f)
            {
                duration = duration - Time.deltaTime;
            }
            else
            {
                // 종료: 정확히 원복
                DivideSpeed(appliedFactor);
                applied = false;
                Destroy(this);
            }
        }
    }

    void OnDisable()
    {
        // 비활성화될 때도 원복을 보장(씬 언로드/프리팹 파괴 대비)
        if (applied == true)
        {
            DivideSpeed(appliedFactor);
            applied = false;
        }
    }

    private void MultiplySpeed(float factor)
    {
        // EnemyCore 안의 '실제 이동 속도' 계산 시 아래처럼 곱해 쓰고 있어야 한다:
        // float speed = baseMoveSpeed * externalSpeedMultiplier;
        core.externalSpeedMultiplier = core.externalSpeedMultiplier * factor;
    }

    private void DivideSpeed(float factor)
    {
        if (factor != 0f)
        {
            core.externalSpeedMultiplier = core.externalSpeedMultiplier / factor;
        }
    }
}
