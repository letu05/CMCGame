using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Gắn vào GameObject LevelCompletePanel (Victory).
/// Tự cập nhật UI khi được SetActive(true) qua OnEnable().
/// 
/// Nút trong Inspector:
///   - Nút Chơi Lại → OnClick gọi OnPlayAgainClicked()
///   - Nút Về Menu  → OnClick gọi OnMainMenuClicked()
/// </summary>
public class LevelCompletePanel : MonoBehaviour
{
    [Header("Sao (3 Image star)")]
    [SerializeField] private Image   starImage1;
    [SerializeField] private Image   starImage2;
    [SerializeField] private Image   starImage3;
    [SerializeField] private Sprite  starFull;   // sprite sao sáng
    [SerializeField] private Sprite  starEmpty;  // sprite sao tối

    [Header("Text điểm")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText; // (tuỳ chọn)

    // ─── Tự động cập nhật khi panel hiện ra ──────────────────────────────────
    private void OnEnable()
    {
        RefreshStars();
        RefreshScore();
    }

    // ─── Stars ────────────────────────────────────────────────────────────────

    private void RefreshStars()
    {
        int collected = LevelManager.Instance != null
            ? LevelManager.Instance.GetStarCount()
            : 0;

        SetStar(starImage1, collected >= 1);
        SetStar(starImage2, collected >= 2);
        SetStar(starImage3, collected >= 3);
    }

    private void SetStar(Image img, bool filled)
    {
        if (img == null) return;
        img.sprite = filled ? starFull : starEmpty;
    }

    // ─── Score ────────────────────────────────────────────────────────────────

    private void RefreshScore()
    {
        if (GameManager.Instance == null) return;

        int score = GameManager.Instance.GetScore();

        if (scoreText     != null) scoreText.text     = score.ToString("N0");

        // Highscore: lưu nếu score hiện tại cao hơn
        if (highscoreText != null)
        {
            int best = PlayerPrefs.GetInt("BestScore", 0);
            if (score > best)
            {
                best = score;
                PlayerPrefs.SetInt("BestScore", best);
                PlayerPrefs.Save();
            }
            highscoreText.text = best.ToString("N0");
        }
    }

    // ─── Button Callbacks ─────────────────────────────────────────────────────

    /// <summary>Nút LEVEL TIẾP THEO — load scene kế tiếp.</summary>
    public void OnNextLevelClicked()
    {
        Time.timeScale = 1f;
        LevelManager.Instance?.LoadNextLevel();
    }

    /// <summary>Nút CHƠI LẠI — restart đúng scene đang chơi.</summary>
    public void OnPlayAgainClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Nút VỀ MENU — load scene 0 (Main Menu).</summary>
    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1); // GameOptionLevel
    }
}

