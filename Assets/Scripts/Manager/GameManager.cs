using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score;
    private int coin;
    private int lives;

    [SerializeField] private float timeRemaining = 360f;

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
}
