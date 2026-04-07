using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gắn vào Panel LeaderboardPanel trong scene.
/// Tự động:
///   1. Load dữ liệu tĩnh từ LeaderboardData (ScriptableObject).
///   2. Chèn điểm của người chơi hiện tại (lấy từ PlayerPrefs "BestScore").
///   3. Sắp xếp giảm dần theo điểm.
///   4. Spawn item prefab vào ScrollView Content.
/// </summary>
public class LeaderboardPanel : MonoBehaviour
{
    // ─── References ──────────────────────────────────────────────────────────

    [Header("Dữ liệu tĩnh (kéo LeaderboardData asset vào đây)")]
    [SerializeField] private LeaderboardData leaderboardData;

    [Header("Prefab cho mỗi hàng (kéo LeaderboardItem prefab vào đây)")]
    [SerializeField] private LeaderboardItemUI itemPrefab;

    [Header("Content của ScrollView (kéo Content transform vào đây)")]
    [SerializeField] private Transform contentParent;

    // ─── Cấu hình người chơi ─────────────────────────────────────────────────

    [Header("Tên người chơi hiện tại")]
    [SerializeField] private string currentPlayerName = "Bạn";

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        BuildLeaderboard();
    }

    // ─── Core ─────────────────────────────────────────────────────────────────

    private void BuildLeaderboard()
    {
        // Xoá item cũ
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Tạo bản sao danh sách để không sửa asset gốc
        var entries = new List<LeaderboardEntry>();

        if (leaderboardData != null)
        {
            foreach (var e in leaderboardData.staticEntries)
                entries.Add(new LeaderboardEntry(e.playerName, e.score, e.totalStars));
        }

        // ── Chèn điểm người chơi ─────────────────────────────────────────
        int playerBestScore = PlayerPrefs.GetInt("BestScore", 0);
        int playerStars     = GetPlayerTotalStars();

        var playerEntry = new LeaderboardEntry(currentPlayerName, playerBestScore, playerStars);
        entries.Add(playerEntry);

        // ── Sắp xếp giảm dần ─────────────────────────────────────────────
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        // ── Spawn UI ──────────────────────────────────────────────────────
        for (int i = 0; i < entries.Count; i++)
        {
            LeaderboardItemUI item = Instantiate(itemPrefab, contentParent);
            bool isPlayer = entries[i].playerName == currentPlayerName
                         && entries[i].score      == playerBestScore;
            item.Bind(i + 1, entries[i], isPlayer);
        }
    }

    /// <summary>
    /// Tính tổng số sao từ tất cả level đã chơi (không giới hạn).
    /// </summary>
    private int GetPlayerTotalStars()
    {
        int total = 0;
        for (int i = 1; i <= 200; i++)
        {
            string key = DataKey.LevelStar(i);
            if (!PlayerPrefs.HasKey(key)) break; // Level chưa chơi → dừng
            total += PlayerPrefs.GetInt(key, 0);
        }
        return total;
    }
}
