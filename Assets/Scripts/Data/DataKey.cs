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

    // ── Mạng sống ────────────────────────────────────────────────────────
    /// <summary>Số mạng còn lại.</summary>
    public const string LIVES = "player_lives";

    // ── Level ─────────────────────────────────────────────────────────────
    /// <summary>Level cao nhất đã mở khóa (số nguyên).</summary>
    public const string LEVEL_UNLOCKED = "level_unlocked";

    // ── Sao ───────────────────────────────────────────────────────────────
    /// <summary>Tổng số sao đã nhặt.</summary>
    public const string STAR = "player_star";

    /// <summary>Key lưu số sao của từng level. Ví dụ level 1 → "level_star_1"</summary>
    public static string LevelStar(int levelIndex) => $"level_star_{levelIndex}";


    // ── Shop – vật phẩm chờ dùng ở màn tiếp ─────────────────────────────
    public const string PENDING_BOMB      = "pdg_bomb";
    public const string PENDING_DART      = "pdg_dart";
    public const string PENDING_BOOMERANG = "pdg_boomerang";
    public const string PENDING_SHIELD    = "pdg_shield";
}
