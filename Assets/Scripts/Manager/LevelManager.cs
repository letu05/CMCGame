using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
    [SerializeField] private Sprite starEmpty;      
    [SerializeField] private Sprite starCollected;  

    // ─── Level Complete / Fail ────────────────────────────────────────
    [Header("Panels")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject levelFailPanel;
    [SerializeField] private GiftPanel  giftPanel;

    // ─── Private state ────────────────────────────────────────────────
    private float timeRemaining;
    private bool  isLevelOver = false;
    private bool[] starCollectedArr = new bool[4]; 

    // ─────────────────────────────────────────────────────────────────
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f; // reset sau khi qua màn (tránh đứng coroutine)
        timeRemaining = timeLimit;
        UpdateTimerUI();
        RefreshStarUI(); // hiển thị 3 sao trạng thái ban đầu (chưa nhặt)

        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (levelFailPanel     != null) levelFailPanel.SetActive(false);
        if (giftPanel          != null) giftPanel.gameObject.SetActive(false);

        // Cập nhật text "Level X" trên HUD
        GameManager.Instance?.UpdateLevelUI(levelIndex);

        // Reset bộ đếm quảng cáo cho màn mới
        AdManager.Instance?.ResetAdCountForNewLevel();
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

    /// <summary>
    /// Gọi từ Star.cs khi player nhặt sao.
    /// starIndex chỉ dùng để tránh nhặt lại cùng 1 sao.
    /// UI luôn fill từ TRÁI → PHẢI theo số sao đã nhặt.
    /// </summary>
    public void CollectStar(int index)
    {
        if (index < 1 || index > 3) return;
        if (starCollectedArr[index]) return; // đã nhặt rồi, bỏ qua

        starCollectedArr[index] = true;
        RefreshStarUI();
    }

    private void RefreshStarUI()
    {
        // Đếm số sao đã nhặt → fill slot từ trái sang phải
        int count = GetStarCount();
        SetStarImage(starImage1, count >= 1); // slot trái  : sáng nếu có ≥ 1 sao
        SetStarImage(starImage2, count >= 2); // slot giữa  : sáng nếu có ≥ 2 sao
        SetStarImage(starImage3, count >= 3); // slot phải  : sáng nếu có đủ 3 sao
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

        GameManager.Instance?.UnlockLevel(levelIndex);
        GameManager.Instance?.SaveLevelStars(levelIndex, GetStarCount()); // ← lưu sao per-level

        if (giftPanel != null)
            giftPanel.Show(runner: this, onDone: ShowVictoryPanel);
        else
            ShowVictoryPanel();
    }

    /// <summary>Hiện UI Victory (LevelCompletePanel). Gọi trực tiếp hoặc qua callback từ GiftPanel.</summary>
    public void ShowVictoryPanel()
    {
        Time.timeScale = 0f;
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
    }

    public void LevelFail()
    {
        if (isLevelOver) return;
        isLevelOver = true;

        Time.timeScale = 0f;
        if (levelFailPanel != null) levelFailPanel.SetActive(true);
    }

    // ─── Scene Navigation ────────────────────────────────────────────

    public void LoadNextLevel()
    {
        int next = nextSceneIndex >= 0
            ? nextSceneIndex
            : SceneManager.GetActiveScene().buildIndex + 1;

        // Đã hết tất cả level → về Main Menu
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1); // GameOptionLevel
    }

    
    public float GetTimeRemaining() => timeRemaining;
    public int   GetLevelIndex()    => levelIndex;
    public bool  IsLevelOver()      => isLevelOver;
}
