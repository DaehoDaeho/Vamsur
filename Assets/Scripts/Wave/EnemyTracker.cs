using UnityEngine;

/// <summary>
/// [7일차~] 적 생명주기 보고자
/// - 파괴 시 WaveManager에 동시 적 수 감소 보고
/// - [11일차] StatsTracker에 처치 수 +1 보고
/// 안전: 에디터 Stop 시에도 OnDestroy가 호출되므로, 실제 플레이 중인지 확인
/// </summary>
public class EnemyTracker : MonoBehaviour
{
    public WaveManager manager;

    public void ProcessDestroy()
    {
        // 플레이 중이 아니면(에디터 정지 등) 통계 조작 금지
        if (Application.isPlaying == false)
        {
            return;
        }

        // 동시 적 수 감소
        if (manager != null)
        {
            manager.NotifyEnemyDestroyed();
        }

        // 처치 수 +1
        StatsTracker stats = FindAnyObjectByType<StatsTracker>();
        if (stats != null)
        {
            stats.AddKill(1);
        }
    }
}
