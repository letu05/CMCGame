using UnityEngine;
using UnityEngine.SceneManagement;

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
    private int levelUnlocked; 

    [SerializeField] private int startLives = 3;

    //tu tim den game object co ten tuong tu
    private const string SCORE_TEXT_NAME = "ScoreText";
    private const string COIN_TEXT_NAME  = "CoinText";
    private const string LEVEL_TEXT_NAME = "LevelText";   // tên GameObject chứa text "Level X"

    private TMPro.TextMeshProUGUI scoreText;
    private TMPro.TextMeshProUGUI coinText;
    private TMPro.TextMeshProUGUI levelText;

    // ─── Singleton ────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Tự tìm lại UI mỗi khi scene mới load xong
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scoreText = GameObject.Find(SCORE_TEXT_NAME)?.GetComponent<TMPro.TextMeshProUGUI>();
        coinText  = GameObject.Find(COIN_TEXT_NAME) ?.GetComponent<TMPro.TextMeshProUGUI>();
        levelText = GameObject.Find(LEVEL_TEXT_NAME)?.GetComponent<TMPro.TextMeshProUGUI>();

        UpdateCoinUI();
        UpdateScoreUI();
        // Level UI được cập nhật bởi LevelManager.Start() sau khi nó khởi tạo xong
    }

    private void Start()
    {
        UpdateCoinUI();
        UpdateScoreUI();
    }

    // ─── Coin ─────────────────────────────────────────────────────────

    public void AddCoin(int amount = 1)
    {
        coin += amount;
        UpdateCoinUI();
        SaveData();
    }

    public int GetCoin() => coin;

    /// <summary>Trừ coin khi mua đồ trong Shop.</summary>
    public void SpendCoin(int amount)
    {
        coin = Mathf.Max(0, coin - amount);
        UpdateCoinUI();
        SaveData();
    }


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
            scoreText.text = score.ToString();
    }

    /// <summary>Gọi từ LevelManager.Start() sau khi levelIndex đã sẵn sàng.</summary>
    public void UpdateLevelUI(int levelIndex)
    {
        if (levelText != null)
            levelText.text = levelIndex.ToString();
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
            SaveData();
        }
    }

    /// <summary>Kiểm tra level có được phép chơi không (dùng ở màn chọn level).</summary>
    public bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= levelUnlocked + 1; 
    }

    public int GetLevelUnlocked() => levelUnlocked;

    /// <summary>Lưu số sao (0-3) của 1 level cụ thể. Chỉ lưu nếu cao hơn lần trước.</summary>
    public void SaveLevelStars(int levelIndex, int stars)
    {
        string key    = DataKey.LevelStar(levelIndex);
        int    current = PlayerPrefs.GetInt(key, 0);
        if (stars > current)
        {
            PlayerPrefs.SetInt(key, stars);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Lấy số sao đã đạt của 1 level (0 nếu chưa chơi).</summary>
    public static int GetLevelStars(int levelIndex)
        => PlayerPrefs.GetInt(DataKey.LevelStar(levelIndex), 0);

    // ─── Lives ────────────────────────────────────────────────────────

    public int  GetLives()           => lives;
    public bool HasLives()           => lives > 0;
    public void LoseLife()           { lives = Mathf.Max(0, lives - 1); SaveData(); }
    public void AddLife(int amount = 1) { lives += amount; SaveData(); }

    /// <summary>
    /// Gọi khi player chết. Trừ mạng và hiển thị màn Defeat qua LevelManager.
    /// </summary>
    public void PlayerDied()
    {
        LoseLife();
        LevelManager.Instance?.LevelFail();
    }

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
    }
}

