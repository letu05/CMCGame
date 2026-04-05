using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Gắn vào mỗi nút level trong màn GameOptionLevel.
/// Tự đổi sprite theo trạng thái và hiển thị sao đã đạt được.
/// </summary>
public class LevelButton : MonoBehaviour
{
    // ─── Cấu hình ─────────────────────────────────────────────────────────────

    [Header("Cài đặt màn")]
    [Tooltip("Số thứ tự hiển thị (1, 2, 3...)")]
    [SerializeField] private int levelIndex = 1;

    [Tooltip("Build Index của scene level này (File → Build Settings)")]
    [SerializeField] private int sceneIndex = 2;

    // ─── UI References ────────────────────────────────────────────────────────

    [Header("UI (kéo từ Hierarchy)")]
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private Image    buttonImage;
    [SerializeField] private Button   button;

    [Header("3 Star Images (kéo Star1, Star2, Star3 vào)")]
    [SerializeField] private Image starImage1;
    [SerializeField] private Image starImage2;
    [SerializeField] private Image starImage3;

    [Header("Sprite sao")]
    [SerializeField] private Sprite starFilled;  // sao sáng
    [SerializeField] private Sprite starEmpty;   // sao mờ/rỗng

    // ─── Sprites trạng thái ───────────────────────────────────────────────────

    [Header("Sprite 3 trạng thái (kéo từ sprite sheet)")]
    [SerializeField] private Sprite spriteCompleted; // Vàng — đã hoàn thành
    [SerializeField] private Sprite spriteAvailable; // Đỏ   — có thể chơi tiếp
    [SerializeField] private Sprite spriteLocked;    // Xám   — chưa mở

    // ─── Runtime state ────────────────────────────────────────────────────────
    private bool isLocked = true;

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    private void OnEnable() => RefreshState();

    // ─── State ────────────────────────────────────────────────────────────────

    private void RefreshState()
    {
        int unlocked = GameManager.Instance != null
            ? GameManager.Instance.GetLevelUnlocked()
            : PlayerPrefs.GetInt(DataKey.LEVEL_UNLOCKED, 0);

        Sprite sprite;

        if (levelIndex <= unlocked)
        {
            sprite   = spriteCompleted;
            isLocked = false;
        }
        else if (levelIndex == unlocked + 1)
        {
            sprite   = spriteAvailable;
            isLocked = false;
        }
        else
        {
            sprite   = spriteLocked;
            isLocked = true;
        }

        if (levelNumberText != null) levelNumberText.text = levelIndex.ToString();
        if (buttonImage     != null) buttonImage.sprite   = sprite;
        if (button          != null) button.interactable  = !isLocked;

        RefreshStars();
    }

    private void RefreshStars()
    {
        int stars = GameManager.GetLevelStars(levelIndex);
        SetStar(starImage1, stars >= 1);
        SetStar(starImage2, stars >= 2);
        SetStar(starImage3, stars >= 3);
    }

    private void SetStar(Image img, bool filled)
    {
        if (img == null) return;
        if (starFilled != null && starEmpty != null)
            img.sprite = filled ? starFilled : starEmpty;
        else
            img.color = filled ? Color.yellow : new Color(0.4f, 0.4f, 0.4f);
    }

    // ─── Click ────────────────────────────────────────────────────────────────

    public void OnLevelClicked()
    {
        if (isLocked) return; // chặn cứng dù button có bị gọi cách nào đi nữa

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }
}

