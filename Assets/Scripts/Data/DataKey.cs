/// <summary>
/// Chứa tất cả key dùng để lưu/tải dữ liệu qua PlayerPrefs.
/// Dùng hằng số tránh lỗi typo khi truy cập ở nhiều script.
/// </summary>
public static class DataKey
{
    // ── Tiền tệ ──────────────────────────────────────────────────────────
    /// <summary>Số coin người chơi đang có.</summary>
    public const string COIN = "player_coin";

    // ── Điểm số ──────────────────────────────────────────────────────────
    /// <summary>Điểm hiện tại của ván chơi.</summary>
    public const string SCORE = "player_score";

    /// <summary>Điểm cao nhất từng đạt được (high score).</summary>
    public const string HIGH_SCORE = "player_high_score";

    // ── Mạng sống ────────────────────────────────────────────────────────
    /// <summary>Số mạng còn lại.</summary>
    public const string LIVES = "player_lives";
}
