using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

/// <summary>
/// Quản lý toàn bộ vòng đời AdMob Rewarded Ads.
/// DontDestroyOnLoad — tồn tại xuyên suốt game.
/// 
/// Cách dùng:
///   AdManager.Instance.ShowRewardedAd(RewardType.Bomb, onRewarded: () => { ... });
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    // ─── App / Ad Unit IDs ────────────────────────────────────────────────────
#if UNITY_ANDROID
    private const string RewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test ID Android
#elif UNITY_IOS
    private const string RewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313"; // Test ID iOS
#else
    private const string RewardedAdUnitId = "unused";
#endif

    // ─── Giới hạn xem quảng cáo mỗi màn ─────────────────────────────────────
    [Header("Giới hạn xem quảng cáo")]
    [SerializeField] private int maxAdsPerLevel = 4;

    private int adsWatchedThisLevel = 0;

    /// <summary>Số lần còn có thể xem quảng cáo trong màn này.</summary>
    public int AdsRemaining => Mathf.Max(0, maxAdsPerLevel - adsWatchedThisLevel);

    /// <summary>Còn quota và ad đã sẵn sàng.</summary>
    public bool CanShowAd => AdsRemaining > 0 && rewardedAd != null && rewardedAd.CanShowAd();

    // ─── Private ──────────────────────────────────────────────────────────────
    private RewardedAd rewardedAd;
    private bool       isInitialized = false;

    // ─── Singleton ────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ─── Init ─────────────────────────────────────────────────────────────────

    private void InitializeAds()
    {
        // Bật chế độ test để hiển thị quảng cáo thử nghiệm trên mọi thiết bị thật
        // Xóa dòng này khi release chính thức lên store
        var requestConfiguration = new RequestConfiguration
        {
            TestDeviceIds = new List<string> { AdRequest.TestDeviceSimulator }
        };
        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(initStatus =>
        {
            isInitialized = true;
            Debug.Log("[AdManager] AdMob initialized.");
            LoadRewardedAd();
        });
    }

    // ─── Load Ad ──────────────────────────────────────────────────────────────

    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        RewardedAd.Load(RewardedAdUnitId, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdManager] Rewarded ad failed to load: {error}");
                return;
            }

            rewardedAd = ad;
            Debug.Log("[AdManager] Rewarded ad loaded.");
            RegisterAdEvents(rewardedAd);
        });
    }

    private void RegisterAdEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdManager] Rewarded ad closed. Loading next...");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (error) =>
        {
            Debug.LogWarning($"[AdManager] Rewarded ad failed to show: {error}");
            LoadRewardedAd();
        };
    }

    // ─── Reset per level ──────────────────────────────────────────────────────

    /// <summary>
    /// Gọi mỗi khi bắt đầu màn mới để reset bộ đếm quảng cáo.
    /// LevelManager.Start() sẽ gọi hàm này.
    /// </summary>
    public void ResetAdCountForNewLevel()
    {
        adsWatchedThisLevel = 0;
        Debug.Log("[AdManager] Ad count reset for new level.");
    }

    // ─── Show Ad ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Hiển thị Rewarded Ad. Gọi onRewarded khi user xem xong.
    /// Gọi onFailed nếu không có ad hoặc đã hết quota.
    /// </summary>
    public void ShowRewardedAd(Action onRewarded, Action onFailed = null)
    {
        if (!CanShowAd)
        {
            Debug.LogWarning(AdsRemaining <= 0
                ? "[AdManager] Đã xem đủ 4 lần quảng cáo trong màn này."
                : "[AdManager] Ad chưa sẵn sàng.");
            onFailed?.Invoke();
            return;
        }

        rewardedAd.Show(reward =>
        {
            adsWatchedThisLevel++;
            Debug.Log($"[AdManager] Reward earned! Lần thứ {adsWatchedThisLevel}/{maxAdsPerLevel} trong màn này.");
            onRewarded?.Invoke();
        });
    }
}
