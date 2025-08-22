using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public WaveManager manager; // WaveManager를 인스펙터로 연결하거나 스폰 시 붙여줌

    void OnDestroy()
    {
        if (manager != null)
        {
            manager.NotifyEnemyDestroyed();
        }
    }
}
