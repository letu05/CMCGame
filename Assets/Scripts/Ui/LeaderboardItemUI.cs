using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gắn vào mỗi item prefab trong bảng xếp hạng.
/// Prefab cần có:
///   - rankText      : TMP_Text — hiển thị "🥇"/"🥈"/"🥉"/số
///   - playerNameText: TMP_Text — tên người chơi
///   - scoreText     : TMP_Text — điểm số (N0)
///   - starsText     : TMP_Text — tổng sao, VD "★ 24"
///   - rowBackground : Image   — (tuỳ chọn) nền đổi màu top 3 / player
/// </summary>
public class LeaderboardItemUI : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Tổng số sao (TMP_Text, VD: ★ 24)")]
    [SerializeField] private TMP_Text starsText;

    [Header("Màu nền hàng (tuỳ chọn)")]
    [SerializeField] private Image rowBackground;

    [Header("Màu top 3")]
    [SerializeField] private Color colorRank1  = new Color(1f,    0.84f, 0f,    1f); // Vàng
    [SerializeField] private Color colorRank2  = new Color(0.75f, 0.75f, 0.75f, 1f); // Bạc
    [SerializeField] private Color colorRank3  = new Color(0.80f, 0.50f, 0.20f, 1f); // Đồng
    [SerializeField] private Color colorNormal = new Color(1f,    1f,    1f,    0.05f);
    [SerializeField] private Color colorPlayer = new Color(0.2f,  0.8f,  1f,    0.25f); // Xanh

    /// <summary>
    /// Cập nhật toàn bộ UI của 1 hàng.
    /// </summary>
    /// <param name="rank">Thứ hạng bắt đầu từ 1</param>
    /// <param name="entry">Dữ liệu entry</param>
    /// <param name="isCurrentPlayer">True nếu đây là hàng của người chơi hiện tại</param>
    public void Bind(int rank, LeaderboardEntry entry, bool isCurrentPlayer = false)
    {
        // ── Rank ──────────────────────────────────────────────────────
        if (rankText != null)
        {
            rankText.text = rank switch
            {
                1 => "1",
                2 => "2",
                3 => "3",
                _ => rank.ToString()
            };
        }

        // ── Tên & Điểm ────────────────────────────────────────────────
        if (playerNameText != null) playerNameText.text = entry.playerName;
        if (scoreText      != null) scoreText.text      = entry.score.ToString("N0");

        // ── Tổng sao (số) ─────────────────────────────────────────────
        if (starsText != null)
            starsText.text = $" {entry.totalStars}";

        // ── Màu nền ───────────────────────────────────────────────────
        if (rowBackground != null)
        {
            rowBackground.color = isCurrentPlayer ? colorPlayer :
                                  rank == 1       ? colorRank1  :
                                  rank == 2       ? colorRank2  :
                                  rank == 3       ? colorRank3  : colorNormal;
        }
    }
}
