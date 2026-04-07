using UnityEngine;

/// <summary>
/// Singleton quản lý toàn bộ hệ thống Achievement.
/// - Theo dõi số quái bị tiêu diệt trong level hiện tại.
/// - Kiểm tra điều kiện hoàn thành từng achievement.
/// - Lưu/tải trạng thái "đã claim" qua PlayerPrefs.
/// 
/// Gắn script này vào 1 GameObject trong scene Menu (hoặc DontDestroyOnLoad).
/// </summary>
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    // ─── Đếm quái trong level hiện tại ──────────────────────────────
    private int enemyKillCount = 0;
    private int bestKillCount  = 0;          // cao nhất từ trước đến giờ

    private const string BEST_KILL_KEY = "ach_best_kill_count";

    // ─── Singleton ──────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            bestKillCount = PlayerPrefs.GetInt(BEST_KILL_KEY, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ════════════════════════════════════════════════════════════════
    //  ENEMY KILL TRACKING
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Gọi mỗi lần 1 enemy bị tiêu diệt (từ EnemyHealth.Die()).
    /// </summary>
    public void RegisterEnemyKill()
    {
        enemyKillCount++;

        // Lưu lại kỷ lục nếu vượt
        if (enemyKillCount > bestKillCount)
        {
            bestKillCount = enemyKillCount;
            PlayerPrefs.SetInt(BEST_KILL_KEY, bestKillCount);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Reset bộ đếm khi bắt đầu level mới.
    /// Gọi từ LevelManager.Start() hoặc khi load scene.
    /// </summary>
    public void ResetKillCount()
    {
        enemyKillCount = 0;
    }

    /// <summary>Số quái đã giết trong level hiện tại.</summary>
    public int GetCurrentKillCount() => enemyKillCount;

    /// <summary>Số quái giết nhiều nhất trong 1 level (toàn bộ lịch sử).</summary>
    public int GetBestKillCount() => bestKillCount;

    // ════════════════════════════════════════════════════════════════
    //  ACHIEVEMENT STATUS
    // ════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kiểm tra xem achievement đã hoàn thành điều kiện hay chưa (chưa tính claim).
    /// </summary>
    public bool IsCompleted(AchievementData data)
    {
        switch (data.type)
        {
            case AchievementType.LevelStars:
                int stars = GameManager.GetLevelStars(data.requiredLevel);
                return stars >= data.requiredValue;

            case AchievementType.KillEnemiesInLevel:
                return bestKillCount >= data.requiredValue;

            default:
                return false;
        }
    }

    /// <summary>Achievement đã được nhận thưởng chưa.</summary>
    public bool IsClaimed(AchievementData data)
    {
        return PlayerPrefs.GetInt(ClaimKey(data), 0) > 0;
    }

    /// <summary>
    /// Nhận thưởng achievement. Trả về true nếu thành công.
    /// </summary>
    public bool TryClaim(AchievementData data)
    {
        if (!IsCompleted(data)) return false;
        if (IsClaimed(data))    return false;

        // Cộng coin
        GameManager.Instance?.AddCoin(data.coinReward);

        // Đánh dấu đã claim
        PlayerPrefs.SetInt(ClaimKey(data), 1);
        PlayerPrefs.Save();

        return true;
    }

    // ─── Helper ─────────────────────────────────────────────────────
    private string ClaimKey(AchievementData data)
        => $"ach_claimed_{data.achievementId}";
}
