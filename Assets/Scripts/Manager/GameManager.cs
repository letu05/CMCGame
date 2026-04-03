using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score;
    private int coin;
    private int lives;


    [SerializeField] private float timeRemaining = 360f; //
    [SerializeField] private int startLives = 3;

    [Header("Text UI")]
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI coinText;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

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
    }

    private void Update()
    {
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // ─── Coin ────────────────────────────────────────────────────────

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

    // ─── Score ───────────────────────────────────────────────────────

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
            scoreText.text = score.ToString("000");
    }


    // ─── Timer ───────────────────────────────────────────────────────

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }

    // ─── Lưu / Tải dữ liệu (PlayerPrefs) ────────────────────────────

    /// <summary>Lưu coin, score và high score xuống PlayerPrefs.</summary>
    public void SaveData()
    {
        PlayerPrefs.SetInt(DataKey.COIN,  coin);
        PlayerPrefs.SetInt(DataKey.SCORE, score);
        PlayerPrefs.SetInt(DataKey.LIVES, lives);
        PlayerPrefs.Save();
    }

    /// <summary>Tải coin, score và high score từ PlayerPrefs khi khởi động.</summary>
    public void LoadData()
    {
        coin  = PlayerPrefs.GetInt(DataKey.COIN,  0);
        score = PlayerPrefs.GetInt(DataKey.SCORE, 0);
        lives = PlayerPrefs.GetInt(DataKey.LIVES, startLives);
    }

    /// <summary>Xoá toàn bộ dữ liệu đã lưu (dùng khi New Game).</summary>
    public void ResetData()
    {
        coin  = 0;
        score = 0;
        lives = startLives;
        SaveData();
        UpdateCoinUI();
        UpdateScoreUI();
    }
}
