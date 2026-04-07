using System;
using UnityEngine;

/// <summary>
/// Dữ liệu cho 1 hàng trong bảng xếp hạng.
/// </summary>
[Serializable]
public class LeaderboardEntry
{
    [Tooltip("Tên người chơi hiển thị")]
    public string playerName;

    [Tooltip("Điểm số")]
    public int score;

    [Tooltip("Tổng số sao đã thu thập (không giới hạn)")]
    public int totalStars;

    public LeaderboardEntry(string name, int score, int totalStars)
    {
        this.playerName  = name;
        this.score       = score;
        this.totalStars  = totalStars;
    }
}
