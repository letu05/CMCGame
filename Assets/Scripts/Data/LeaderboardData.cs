using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject chứa danh sách bảng xếp hạng tĩnh (giả lập).
/// Tạo asset: chuột phải Project → Create → Leaderboard → Leaderboard Data
/// </summary>
[CreateAssetMenu(fileName = "LeaderboardData", menuName = "Leaderboard/Leaderboard Data")]
public class LeaderboardData : ScriptableObject
{
    [Tooltip("Danh sách người chơi tĩnh (không thay đổi theo game)")]
    public List<LeaderboardEntry> staticEntries = new List<LeaderboardEntry>()
    {
        // ──── Thêm/sửa trực tiếp trong Inspector ─────────────────────
        // tham số: (tên, điểm, totalStars)
        new LeaderboardEntry("Rồng Vàng",    98500, 45),
        new LeaderboardEntry("Siêu Nhân",    87200, 39),
        new LeaderboardEntry("Thiên Hà",     75000, 33),
        new LeaderboardEntry("Phượng Hoàng", 66300, 28),
        new LeaderboardEntry("Mãnh Hổ",      58900, 24),
        new LeaderboardEntry("Thần Sấm",     51400, 21),
        new LeaderboardEntry("Hắc Long",     43700, 17),
        new LeaderboardEntry("Bão Táp",      37200, 13),
        new LeaderboardEntry("Cát Vàng",     29800,  9),
        new LeaderboardEntry("Sóng Biển",    21300,  5),
    };
}
