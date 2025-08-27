using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// [11����] ���� ���� �Ѱ� �Ŵ���
/// - ESC ���: �Ͻ�����/�簳
/// - �¸�: ��ǥ ���� �ð� �޼�
/// - �й�: �÷��̾� Health�� ���
/// - ���â UI ������Ʈ + ����� ��ư
/// ��Ģ: ��� if�� ��� ���, ������ ��Ȯ�ϰ� �ۼ�.
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    [Header("Targets (�ܺ� ����)")]
    public Health playerHealth;   // Player�� Health
    public StatsTracker stats;    // ���� �ð�/óġ ��/��� ����

    [Header("Win/Lose Rule")]
    public float targetSurvivalSeconds = 120f; // �� �ð��� �ѱ�� �¸�

    [Header("UI - Result Panel")]
    public GameObject resultPanel; // ��Ȱ�� ���� ����
    public TMP_Text txtTime;
    public TMP_Text txtKills;
    public TMP_Text txtGold;
    public TMP_Text txtTitle;          // "Victory!" / "Game Over" / "Paused"

    // ���� ����
    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isVictory = false;

    void Start()
    {
        // ���� ����: �ð� �帧 ����ȭ
        Time.timeScale = 1f;

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        // �ν����� ���� ��� �ڵ� Ž��
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
        // 1) ESC �Ͻ����� ���(�̹� ������ �����ٸ� ����)
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (isGameOver == true)
            {
                // ���� ���� ���¿����� ��� ����
            }
            else
            {
                TogglePause();
            }
        }

        // 2) �¸�/�й� ����(���� �߿����� üũ)
        if (isGameOver == false && isVictory == false)
        {
            // �й�: �÷��̾� ���?
            if (playerHealth != null)
            {
                bool alive = playerHealth.IsAliveNow();
                if (alive == false)
                {
                    ShowResult(false); // �й�
                    return;
                }
            }

            // �¸�: ��ǥ ���� �ð� �޼�?
            if (stats != null)
            {
                if (stats.survivalTimeSeconds >= targetSurvivalSeconds)
                {
                    ShowResult(true); // �¸�
                    return;
                }
            }
        }
    }

    void TogglePause()
    {
        if (isPaused == false)
        {
            // �Ͻ����� �ѱ�
            isPaused = true;
            Time.timeScale = 0f;

            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
                if (txtTitle != null)
                {
                    txtTitle.text = "Paused";
                }
                UpdateResultTextsPreview(); // ���� ��� �̸�����
            }
        }
        else
        {
            // �Ͻ����� ����
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

        Time.timeScale = 0f; // ���â���� ����

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

    // ��ư: �����
    public void Btn_Restart()
    {
        // ���� ���·� �ε�Ǵ� ���� ����
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    // ��ư: �簳(�Ͻ����� ����)
    public void Btn_Resume()
    {
        if (isGameOver == true)
        {
            return; // ���� ���¿��� �簳 �Ұ�
        }

        isPaused = false;
        Time.timeScale = 1f;

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }
}
