using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI cho 1 dòng achievement trong panel.
/// Gắn vào prefab AchievementItem.
/// </summary>
public class AchievementItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text   descriptionText;   // Mô tả nhiệm vụ
    [SerializeField] private TMP_Text   rewardText;        // Hiển thị "+50 🪙" trên nút
    [SerializeField] private TMP_Text   buttonLabel;       // Text chính trên button (Locked / Claim / Claimed)
    [SerializeField] private Button     claimButton;       // Button claim

    [Header("Button Colors (optional)")]
    [SerializeField] private Color lockedColor  = new Color(0.5f, 0.5f, 0.5f, 1f);   // xám
    [SerializeField] private Color claimColor   = new Color(0.2f, 0.8f, 0.2f, 1f);   // xanh lá
    [SerializeField] private Color claimedColor = new Color(0.9f, 0.75f, 0.2f, 1f);  // vàng

    private AchievementData data;
    private System.Action   onClaimedCallback;

    // ─── Setup ────────────────────────────────────────────────────────
    /// <summary>
    /// Gọi 1 lần khi tạo card.
    /// </summary>
    public void Setup(AchievementData achievementData, System.Action onClaimed = null)
    {
        data              = achievementData;
        onClaimedCallback = onClaimed;

        descriptionText.text = data.description;
        claimButton.onClick.AddListener(OnClaimClicked);

        RefreshState();
    }

    // ─── Refresh ──────────────────────────────────────────────────────
    /// <summary>Cập nhật giao diện theo trạng thái hiện tại.</summary>
    public void RefreshState()
    {
        if (data == null) return;

        bool completed = false;
        bool claimed   = false;

        if (AchievementManager.Instance != null)
        {
            completed = AchievementManager.Instance.IsCompleted(data);
            claimed   = AchievementManager.Instance.IsClaimed(data);
        }
        else
        {
            // Fallback: đọc trực tiếp từ PlayerPrefs nếu chưa có AchievementManager
            claimed = PlayerPrefs.GetInt($"ach_claimed_{data.achievementId}", 0) > 0;

            switch (data.type)
            {
                case AchievementType.LevelStars:
                    int stars = GameManager.GetLevelStars(data.requiredLevel);
                    completed = stars >= data.requiredValue;
                    break;
                case AchievementType.KillEnemiesInLevel:
                    int bestKill = PlayerPrefs.GetInt("ach_best_kill_count", 0);
                    completed = bestKill >= data.requiredValue;
                    break;
            }
        }

        if (claimed)
        {
            // ── Đã nhận ──
            if (buttonLabel != null) buttonLabel.text = "Claimed";
            if (rewardText  != null) rewardText.text  = "";
            claimButton.interactable = false;
            SetButtonColor(claimedColor);
        }
        else if (completed)
        {
            // ── Hoàn thành, chưa nhận ──
            if (buttonLabel != null) buttonLabel.text = "Claim";
            if (rewardText  != null) rewardText.text  = $"+{data.coinReward}";
            claimButton.interactable = true;
            SetButtonColor(claimColor);
        }
        else
        {
            // ── Chưa hoàn thành ──
            if (buttonLabel != null) buttonLabel.text = "Locked";
            if (rewardText  != null) rewardText.text  = "";
            claimButton.interactable = false;
            SetButtonColor(lockedColor);
        }
    }

    // ─── Button Handler ────────────────────────────────────────────────
    private void OnClaimClicked()
    {
        if (AchievementManager.Instance != null && AchievementManager.Instance.TryClaim(data))
        {
            RefreshState();
            onClaimedCallback?.Invoke();
        }
    }

    // ─── Helper ────────────────────────────────────────────────────────
    private void SetButtonColor(Color color)
    {
        var colors = claimButton.colors;
        colors.normalColor      = color;
        colors.highlightedColor = color * 1.1f;
        colors.pressedColor     = color * 0.9f;
        colors.disabledColor    = color * 0.7f;
        claimButton.colors      = colors;
    }
}
