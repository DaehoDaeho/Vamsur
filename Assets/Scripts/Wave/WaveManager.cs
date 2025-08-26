using TMPro;
using UnityEngine;

/// <summary>
/// 일정 주기마다 정해진 스폰 포인트 중 랜덤한 위치에 몬스터를 생성시킨다.
/// 생성 후 변경된 정보를 UI에 갱신.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;          // 적이 나올 위치들

    [Header("Enemies")]
    public GameObject[] enemyPrefabs;        // 등장시킬 적 종류(1개만 있어도 됨)
    public int[] enemyWeights;               // 가중치(적이 2종 이상일 때만 사용, 길이는 enemyPrefabs와 동일)

    [Header("Spawn Rules")]
    public float spawnInterval = 3f;         // 처음 스폰 간격(초)
    public float minSpawnInterval = 0.7f;    // 아무리 빨라도 이 값 아래로는 안 내려감
    public int spawnCountPerBatch = 1;       // 한 번에 뽑는 마리 수(처음 1)
    public int maxAlive = 30;                // 동시에 살아있는 적의 상한

    [Header("Difficulty")]
    public float difficultyStepSeconds = 20f;// 이 시간마다 난이도 상승
    public float intervalStepDelta = 0.2f;   // 난이도 상승 시 간격이 줄어드는 양
    public int batchStepEveryN = 2;        // 난이도 N번 올릴 때마다 한 번에 뽑는 수 +1

    [Header("Difficulty / Boss")]
    public DifficultyDirector difficulty;
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    [Header("References (Optional)")]
    public Transform enemiesParent;          // 적들을 이 밑으로 정리(없어도 동작)

    [Header("HUD (Optional)")]
    public TextMeshProUGUI waveText;     // "Wave 3" 같은 표시
    public TextMeshProUGUI timerText;    // "Next: 1.2s" 같은 표시
    public TextMeshProUGUI aliveText;    // "Alive: 12/30" 같은 표시

    [Header("Public Read")]
    public int publicWaveNumber = 1; // 외부에서 읽도록 공개

    // 내부 상태(쉬운 필드만 씀)
    private float nextSpawnTimer = 0f;
    private float elapsed = 0f;
    private int difficultyStepCount = 0;
    private int alive = 0;
    private int waveNumber = 1;            // 스폰이 한번 발생할 때마다 +1 하려고 사용

    private bool bossSpawnedThisWave = false;

    void Start()
    {
        // 타이머를 처음 간격으로 채운다
        nextSpawnTimer = spawnInterval;

        // 가중치 배열이 비었거나 길이가 맞지 않으면, 균등 확률로 취급
        if (enemyWeights == null || enemyWeights.Length != enemyPrefabs.Length)
        {
            enemyWeights = new int[enemyPrefabs.Length];
            int i = 0;
            while (i < enemyWeights.Length)
            {
                enemyWeights[i] = 1; // 전부 1로(균등)
                i++;
            }
        }
    }

    void Update()
    {
        // 시간 흘리기
        float dt = Time.deltaTime;
        elapsed += dt;
        nextSpawnTimer -= dt;

        // HUD 갱신(옵션) — 복잡한 이벤트 안 쓰고 그냥 매 프레임 값 갱신
        UpdateHud();

        // 난이도 상승 체크
        if (elapsed >= difficultyStepSeconds * (difficultyStepCount + 1))
        {
            difficultyStepCount = difficultyStepCount + 1;

            // 간격 줄이기(바닥값 유지)
            spawnInterval = spawnInterval - intervalStepDelta;
            if (spawnInterval < minSpawnInterval) spawnInterval = minSpawnInterval;

            // 몇 번에 한 번씩 번들 크기 +1
            if (batchStepEveryN > 0)
            {
                if (difficultyStepCount % batchStepEveryN == 0)
                {
                    spawnCountPerBatch = spawnCountPerBatch + 1;
                }
            }
        }

        //==========================================================

        if (difficulty != null)
        {
            if (difficulty.IsBossWave(publicWaveNumber) == true)
            {
                bool spawned = SpawnBossOnceThisWave();
                if (spawned == true)
                {
                    // 보스를 스폰한 프레임에는 일반 스폰은 건너뛴다.
                    nextSpawnTimer = spawnInterval;
                    return;
                }
            }
        }

        //==========================================================

        // 스폰 타이밍 도착
        if (nextSpawnTimer <= 0f)
        {
            TrySpawnBatch();
            nextSpawnTimer = spawnInterval; // 타이머 리셋
            waveNumber = waveNumber + 1;    // "웨이브" 느낌용 카운터
            //publicWaveNumber = waveNumber; // 외부 공개값도 동기화
            publicWaveNumber = waveNumber;   // 외부/보스판정에서 쓰는 웨이브 값
            bossSpawnedThisWave = false;     // 이번 웨이브엔 아직 보스를 안 뽑았음
        }
    }

    void TrySpawnBatch()
    {
        // 여유가 없으면 아무것도 안 함
        int room = maxAlive - alive;
        if (room <= 0) return;

        // 이번에 몇 마리 스폰할지 결정(상한 고려)
        int toSpawn = spawnCountPerBatch;
        if (toSpawn > room) toSpawn = room;

        int i = 0;
        while (i < toSpawn)
        {
            SpawnOne();
            i = i + 1;
        }
    }

    void SpawnOne()
    {
        // 스폰 포인트가 없으면 취소
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        // 랜덤 포인트 뽑기
        int spIndex = Random.Range(0, spawnPoints.Length);
        Transform sp = spawnPoints[spIndex];
        Vector3 pos = sp.position;

        // 적 종류 가중치 뽑기
        int eIndex = WeightedPickIndex();
        GameObject prefab = enemyPrefabs[eIndex];

        // 인스턴스 생성(풀 있으면 풀 사용, 없으면 Instantiate)
        GameObject go;
        if (PoolManager.Instance != null)
        {
            go = PoolManager.Instance.Spawn(prefab, pos, Quaternion.identity, enemiesParent);
        }
        else
        {
            if (enemiesParent != null)
                go = Instantiate(prefab, pos, Quaternion.identity, enemiesParent);
            else
                go = Instantiate(prefab, pos, Quaternion.identity);
        }

        //==========================================================

        if (difficulty != null)
        {
            bool makeElite = difficulty.RollIsElite();
            if (makeElite == true)
            {
                EnemyEliteMarker mk = go.GetComponent<EnemyEliteMarker>();
                if (mk == null)
                {
                    mk = go.AddComponent<EnemyEliteMarker>();
                }
                mk.ApplyElite(difficulty);
            }
        }

        //==========================================================

        // 적 수 세기 위해 EnemyTracker 달기(없으면 붙여주기)
        EnemyTracker tracker = go.GetComponent<EnemyTracker>();
        if (tracker == null) tracker = go.AddComponent<EnemyTracker>();
        tracker.manager = this;

        // 올라간 적 수 반영
        alive = alive + 1;
    }

    int WeightedPickIndex()
    {
        // 간단 가중치 합 + 루프
        int total = 0;
        int i = 0;
        while (i < enemyWeights.Length)
        {
            int w = enemyWeights[i];
            if (w < 0) w = 0;
            total = total + w;
            i = i + 1;
        }
        if (total <= 0) return 0;

        int pick = Random.Range(0, total);
        int acc = 0;
        int j = 0;
        while (j < enemyWeights.Length)
        {
            int w = enemyWeights[j];
            if (w < 0) w = 0;
            acc = acc + w;
            if (pick < acc) return j;
            j = j + 1;
        }
        return 0;
    }

    public void NotifyEnemyDestroyed()
    {
        // 적이 파괴될 때 EnemyTracker가 이 함수를 불러줌
        alive = alive - 1;
        if (alive < 0) alive = 0;
    }

    void UpdateHud()
    {
        if (waveText != null)
        {
            waveText.text = "Wave " + waveNumber.ToString();
        }
        if (timerText != null)
        {
            // 남은 시간 표시용(소수 1자리)
            float t = nextSpawnTimer;
            if (t < 0f) t = 0f;
            timerText.text = "Next: " + t.ToString("F1") + "s";
        }
        if (aliveText != null)
        {
            aliveText.text = "Alive: " + alive.ToString() + " / " + maxAlive.ToString();
        }
    }

    bool SpawnBossOnceThisWave()
    {
        if (bossSpawnedThisWave == true)
        {
            return false;
        }
        if (bossPrefab == null)
        {
            return false;
        }

        Transform sp = bossSpawnPoint;
        if (sp == null)
        {
            // 보스 스폰 포인트가 없으면 일반 스폰 포인트 중 하나 사용
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int idx = Random.Range(0, spawnPoints.Length);
                sp = spawnPoints[idx];
            }
        }

        if (sp == null)
        {
            return false;
        }

        GameObject boss;
        if (PoolManager.Instance != null)
        {
            boss = PoolManager.Instance.Spawn(bossPrefab, sp.position, Quaternion.identity, enemiesParent);
        }
        else
        {
            if (enemiesParent != null)
            {
                boss = Instantiate(bossPrefab, sp.position, Quaternion.identity, enemiesParent);
            }
            else
            {
                boss = Instantiate(bossPrefab, sp.position, Quaternion.identity);
            }
        }

        bossSpawnedThisWave = true;
        // 웨이브가 증가하면 자동으로 false로 초기화되게 해주기(아래 한 줄을 waveNumber 증가 직후에 넣자)
        // bossSpawnedThisWave = false;  ← 이 줄은 waveNumber가 증가한 직후에 넣어주세요.

        return true;
    }
}
