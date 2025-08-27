using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// [11일차] 게임 진행 총괄 매니저
/// - ESC 토글: 일시정지/재개
/// - 승리: 목표 생존 시간 달성
/// - 패배: 플레이어 Health가 사망
/// - 결과창 UI 업데이트 + 재시작 버튼
/// 규칙: 모든 if는 블록 사용, 조건은 명확하게 작성.
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    [Header("Targets (외부 참조)")]
    public Health playerHealth;   // Player의 Health
    public StatsTracker stats;    // 생존 시간/처치 수/골드 취합

    [Header("Win/Lose Rule")]
    public float targetSurvivalSeconds = 120f; // 이 시간을 넘기면 승리

    [Header("UI - Result Panel")]
    public GameObject resultPanel; // 비활성 시작 권장
    public TMP_Text txtTime;
    public TMP_Text txtKills;
    public TMP_Text txtGold;
    public TMP_Text txtTitle;          // "Victory!" / "Game Over" / "Paused"

    // 내부 상태
    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isVictory = false;

    void Start()
    {
        // 게임 시작: 시간 흐름 정상화
        Time.timeScale = 1f;

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        // 인스펙터 누락 대비 자동 탐색
        if (playerHealth == null)
        {
            playerHealth = FindAnyObjectByType<Health>();
        }
        if (stats == null)
        {
            stats = FindAnyObjectByType<StatsTracker>();
        }
    }

    void Update()
    {
        // 1) ESC 일시정지 토글(이미 게임이 끝났다면 무시)
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (isGameOver == true)
            {
                // 게임 종료 상태에서는 토글 무시
            }
            else
            {
                TogglePause();
            }
        }

        // 2) 승리/패배 판정(진행 중에서만 체크)
        if (isGameOver == false && isVictory == false)
        {
            // 패배: 플레이어 사망?
            if (playerHealth != null)
            {
                bool alive = playerHealth.IsAliveNow();
                if (alive == false)
                {
                    ShowResult(false); // 패배
                    return;
                }
            }

            // 승리: 목표 생존 시간 달성?
            if (stats != null)
            {
                if (stats.survivalTimeSeconds >= targetSurvivalSeconds)
                {
                    ShowResult(true); // 승리
                    return;
                }
            }
        }
    }

    void TogglePause()
    {
        if (isPaused == false)
        {
            // 일시정지 켜기
            isPaused = true;
            Time.timeScale = 0f;

            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
                if (txtTitle != null)
                {
                    txtTitle.text = "Paused";
                }
                UpdateResultTextsPreview(); // 현재 통계 미리보기
            }
        }
        else
        {
            // 일시정지 해제
            isPaused = false;
            Time.timeScale = 1f;

            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
        }
    }

    void ShowResult(bool victory)
    {
        isGameOver = true;
        isVictory = victory;

        Time.timeScale = 0f; // 결과창에서 정지

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        if (txtTitle != null)
        {
            if (victory == true)
            {
                txtTitle.text = "Victory!";
            }
            else
            {
                txtTitle.text = "Game Over";
            }
        }

        UpdateResultTextsPreview();
    }

    void UpdateResultTextsPreview()
    {
        if (stats != null)
        {
            if (txtTime != null)
            {
                txtTime.text = "Time  : " + stats.GetFormattedTime();
            }
            if (txtKills != null)
            {
                txtKills.text = "Kills : " + stats.killCount.ToString();
            }
            if (txtGold != null)
            {
                txtGold.text = "Gold  : " + stats.GetCurrentGold().ToString();
            }
        }
    }

    // 버튼: 재시작
    public void Btn_Restart()
    {
        // 정지 상태로 로드되는 버그 방지
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    // 버튼: 재개(일시정지 해제)
    public void Btn_Resume()
    {
        if (isGameOver == true)
        {
            return; // 종료 상태에선 재개 불가
        }

        isPaused = false;
        Time.timeScale = 1f;

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }
}
