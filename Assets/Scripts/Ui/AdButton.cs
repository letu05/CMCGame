using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gắn vào Button "Xem Quảng Cáo" trong UI.
/// Chọn RewardType trong Inspector để quyết định phần thưởng.
///
/// Tự động disable button khi:
///   - Đã xem đủ 4 lần trong màn
///   - Ad chưa load xong
/// </summary>
public class AdButton : MonoBehaviour
{
    public enum RewardType
    {
        Bomb,       // +5 bomb
        Shield,     // +1 shield
        PointItem,  // +1 point item
        Coin        // +50 coin/vàng
    }

    [Header("Loại phần thưởng")]
    [SerializeField] private RewardType rewardType = RewardType.Coin;

    [Header("UI Components")]
    [SerializeField] private Button      adButton;
    [SerializeField] private TMP_Text    remainingText; // (tuỳ chọn) hiển thị "Còn X lần"
    [SerializeField] private GameObject  noAdsOverlay;  // (tuỳ chọn) che button khi hết lần

    // ─── Unity ────────────────────────────────────────────────────────────────

    private void Awake()
    {
        // Tự tìm Button nếu chưa gán
        if (adButton == null)
            adButton = GetComponent<Button>();

        if (adButton != null)
            adButton.onClick.AddListener(OnAdButtonClicked);
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    private void Update()
    {
        // Cập nhật trạng thái button mỗi frame (ad có thể load xong bất kỳ lúc nào)
        RefreshUI();
    }

    // ─── Click Handler ────────────────────────────────────────────────────────

    public void OnAdButtonClicked()
    {
        if (AdManager.Instance == null)
        {
            Debug.LogWarning("[AdButton] AdManager chưa tồn tại trong scene!");
            return;
        }

        if (!AdManager.Instance.CanShowAd)
        {
            Debug.LogWarning("[AdButton] Không thể hiển thị ad ngay lúc này.");
            return;
        }

        AdManager.Instance.ShowRewardedAd(
            onRewarded: GiveReward,
            onFailed:   () => Debug.Log("[AdButton] Ad bị skip hoặc thất bại.")
        );
    }

    // ─── Reward Dispatch ──────────────────────────────────────────────────────

    private void GiveReward()
    {
        AdRewardHandler handler = AdRewardHandler.Instance;
        if (handler == null)
        {
            // Fallback: tìm lại trong scene
            handler = FindFirstObjectByType<AdRewardHandler>();
        }

        if (handler == null)
        {
            Debug.LogWarning("[AdButton] AdRewardHandler không tìm thấy trong scene!");
            return;
        }

        switch (rewardType)
        {
            case RewardType.Bomb:      handler.RewardBomb();      break;
            case RewardType.Shield:    handler.RewardShield();    break;
            case RewardType.PointItem: handler.RewardPointItem(); break;
            case RewardType.Coin:      handler.RewardCoin();      break;
        }

        RefreshUI();
    }

    // ─── UI Refresh ───────────────────────────────────────────────────────────

    private void RefreshUI()
    {
        bool canShow = AdManager.Instance != null && AdManager.Instance.CanShowAd;

        // Enable/disable button
        if (adButton != null)
            adButton.interactable = canShow;

        // Hiển thị số lần còn lại
        if (remainingText != null && AdManager.Instance != null)
        {
            int remaining = AdManager.Instance.AdsRemaining;
            remainingText.text = remaining > 0
                ? $"Còn {remaining} lần"
                : "Hết lượt hôm nay";
        }

        // Overlay che button khi hết lần
        if (noAdsOverlay != null && AdManager.Instance != null)
            noAdsOverlay.SetActive(AdManager.Instance.AdsRemaining <= 0);
    }
}
