using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý từng level: timer, 3 sao, thắng/thua, load scene.
/// Gắn vào 1 GameObject trong mỗi scene (KHÔNG DontDestroyOnLoad).
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    // ─── Level Settings ───────────────────────────────────────────────
    [Header("Level Settings")]
    [SerializeField] private int   levelIndex     = 1;
    [SerializeField] private float timeLimit      = 360f;
    [SerializeField] private int   nextSceneIndex = -1;

    // ─── Timer UI ────────────────────────────────────────────────────
    [Header("Timer UI")]
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

    // ─── Star UI (3 sao cố định mỗi màn) ─────────────────────────────
    [Header("Star UI")]
    [SerializeField] private Image starImage1;
    [SerializeField] private Image starImage2;
    [SerializeField] private Image starImage3;
    [SerializeField] private Sprite starEmpty;      // sprite sao chưa nhặt (tối/mờ)
    [SerializeField] private Sprite starCollected;  // sprite sao đã nhặt (sáng)

    // ─── Level Complete / Fail ────────────────────────────────────────
    [Header("Panels")]
    [SerializeField] private GameObject levelCompletePanel;// level hoàn    thành
    [SerializeField] private GameObject levelFailPanel;// level thất bại

    // ─── Private state ────────────────────────────────────────────────
    private float timeRemaining;
    private bool  isLevelOver = false;
    private bool[] starCollectedArr = new bool[4]; // index 1–3 dùng, 0 bỏ

    // ─────────────────────────────────────────────────────────────────
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timeRemaining = timeLimit;
        UpdateTimerUI();
        RefreshStarUI(); // hiển thị 3 sao trạng thái ban đầu (chưa nhặt)

        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (levelFailPanel     != null) levelFailPanel.SetActive(false);
    }

    private void Update()
    {
        if (isLevelOver) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                LevelFail();
            }
        }
    }

    // ─── Timer ───────────────────────────────────────────────────────

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(Mathf.Max(timeRemaining, 0f)).ToString();
    }

    // ─── Star System ─────────────────────────────────────────────────

    /// <summary>Gọi từ Star.cs khi player nhặt sao (index = 1, 2 hoặc 3).</summary>
    public void CollectStar(int index)
    {
        if (index < 1 || index > 3) return;
        if (starCollectedArr[index]) return; // đã nhặt rồi

        starCollectedArr[index] = true;
        RefreshStarUI();
        Debug.Log($"[LevelManager] Sao #{index} đã nhặt!");
    }

    private void RefreshStarUI()
    {
        SetStarImage(starImage1, starCollectedArr[1]);
        SetStarImage(starImage2, starCollectedArr[2]);
        SetStarImage(starImage3, starCollectedArr[3]);
    }

    private void SetStarImage(Image img, bool collected)
    {
        if (img == null) return;
        img.sprite = collected ? starCollected : starEmpty;
    }

    public int GetStarCount()
    {
        int count = 0;
        for (int i = 1; i <= 3; i++)
            if (starCollectedArr[i]) count++;
        return count;
    }

    // ─── Thắng / Thua ────────────────────────────────────────────────

    public void LevelComplete()
    {
        if (isLevelOver) return;
        isLevelOver = true;

        int stars = GetStarCount();
        Debug.Log($"[LevelManager] Level {levelIndex} hoàn thành! ⭐ x{stars}");

        GameManager.Instance?.UnlockLevel(levelIndex);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
    }

    public void LevelFail()
    {
        if (isLevelOver) return;
        isLevelOver = true;

        Debug.Log($"[LevelManager] Level {levelIndex} thất bại!");
        if (levelFailPanel != null) levelFailPanel.SetActive(true);
    }

    // ─── Scene Navigation ────────────────────────────────────────────

    public void LoadNextLevel()
    {
        int next = nextSceneIndex >= 0
            ? nextSceneIndex
            : SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            Debug.Log("[LevelManager] Hết level!");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // ─── Getters ─────────────────────────────────────────────────────
    public float GetTimeRemaining() => timeRemaining;
    public int   GetLevelIndex()    => levelIndex;
    public bool  IsLevelOver()      => isLevelOver;
}
