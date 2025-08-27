using UnityEngine;

/// <summary>
/// - 생존 시간(초) 누적
/// - 처치 수 누적(AddKill 호출로 증가)
/// - 현재 골드 조회(결과창 표기용)
/// </summary>
public class StatsTracker : MonoBehaviour
{
    [Header("State (런타임 누적)")]
    public float survivalTimeSeconds = 0f;  // 지금까지 버틴 시간(초)
    public int killCount = 0;             // 누적 처치 수

    [Header("References")]
    public GoldSystem gold;                 // Player의 GoldSystem

    void Start()
    {
        // gold가 비어 있으면 한 번 자동 탐색(안전을 위해 인스펙터 연결 권장)
        if (gold == null)
        {
            gold = FindAnyObjectByType<GoldSystem>();
        }
    }

    void Update()
    {
        // 의도 명확화: 일시정지 중에는 시간 누적 안 함
        if (Time.timeScale != 0f)
        {
            survivalTimeSeconds = survivalTimeSeconds + Time.deltaTime;
        }
    }

    public void AddKill(int amount)
    {
        if (amount > 0)
        {
            killCount = killCount + amount;
        }
    }

    public string GetFormattedTime()
    {
        int total = Mathf.FloorToInt(survivalTimeSeconds);
        int minutes = total / 60;
        int seconds = total % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public int GetCurrentGold()
    {
        if (gold == null)
        {
            return 0;
        }
        else
        {
            return gold.currentGold;
        }
    }
}
