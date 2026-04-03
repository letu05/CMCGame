using UnityEngine;

/// <summary>
/// Bộ não toàn game — tồn tại xuyên suốt mọi scene (DontDestroyOnLoad).
/// Quản lý: score, coin, lives, level đã mở khóa, save/load.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ─── Data ─────────────────────────────────────────────────────────
    private int score;
    private int coin;
    private int lives;
    private int star;
    private int levelUnlocked; // Level cao nhất đã hoàn thành (mở khóa)

    [SerializeField] private int startLives = 3;

    [Header("Text UI")]
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI coinText;
    [SerializeField] private TMPro.TextMeshProUGUI starText;

    // ─── Singleton ────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCoinUI();
        UpdateScoreUI();
        UpdateStarUI();
    }

    // ─── Coin ─────────────────────────────────────────────────────────

    public void AddCoin(int amount = 1)
    {
        coin += amount;
        UpdateCoinUI();
        SaveData();
    }

    public int GetCoin() => coin;

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = $"{coin:00}";
    }

    // ─── Score ────────────────────────────────────────────────────────

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
        SaveData();
    }

    public int GetScore() => score;

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString("000000");
    }

    // ─── Star ───────────────────────────────────────────────

    public void AddStar(int amount = 1)
    {
        star += amount;
        UpdateStarUI();
        SaveData();
    }

    public int GetStar() => star;

    private void UpdateStarUI()
    {
        if (starText != null)
            starText.text = star.ToString();
    }

    // ─── Level Unlock ─────────────────────────────────────────────────

    /// <summary>
    /// Gọi từ LevelManager.LevelComplete() khi hoàn thành 1 màn.
    /// Tự động mở khóa level tiếp theo nếu chưa mở.
    /// </summary>
    public void UnlockLevel(int levelIndex)
    {
        if (levelIndex > levelUnlocked)
        {
            levelUnlocked = levelIndex;
            Debug.Log($"[GameManager] Mở khóa level {levelUnlocked}!");
            SaveData();
        }
    }

    /// <summary>Kiểm tra level có được phép chơi không (dùng ở màn chọn level).</summary>
    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= levelUnlocked + 1; 
    }

    public int GetLevelUnlocked() => levelUnlocked;

    // ─── Lives ────────────────────────────────────────────────────────

    public int  GetLives()           => lives;
    public bool HasLives()           => lives > 0;
    public void LoseLife()           { lives = Mathf.Max(0, lives - 1); SaveData(); }
    public void AddLife(int amount = 1) { lives += amount; SaveData(); }

    // ─── Save / Load ──────────────────────────────────────────────────

    public void SaveData()
    {
        PlayerPrefs.SetInt(DataKey.COIN,           coin);
        PlayerPrefs.SetInt(DataKey.SCORE,          score);
        PlayerPrefs.SetInt(DataKey.LIVES,          lives);
        PlayerPrefs.SetInt(DataKey.STAR,           star);
        PlayerPrefs.SetInt(DataKey.LEVEL_UNLOCKED, levelUnlocked);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        coin          = PlayerPrefs.GetInt(DataKey.COIN,           0);
        score         = PlayerPrefs.GetInt(DataKey.SCORE,          0);
        lives         = PlayerPrefs.GetInt(DataKey.LIVES,          startLives);
        star          = PlayerPrefs.GetInt(DataKey.STAR,           0);
        levelUnlocked = PlayerPrefs.GetInt(DataKey.LEVEL_UNLOCKED, 0);
    }

    /// <summary>New Game — xóa toàn bộ tiến trình.</summary>
    public void ResetData()
    {
        coin          = 0;
        score         = 0;
        lives         = startLives;
        star          = 0;
        levelUnlocked = 0;
        SaveData();
        UpdateCoinUI();
        UpdateScoreUI();
        UpdateStarUI();
    }
}
