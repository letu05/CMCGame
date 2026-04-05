using UnityEngine;

/// <summary>
/// Xử lý logic mua hàng và lưu vật phẩm "chờ dùng" vào PlayerPrefs.
/// Ammo / Shield được tiêu thụ khi màn chơi kế bắt đầu (PlayerFire.Start, PlayerPowerUp).
/// </summary>
public static class ShopManager
{
    // ─── Kiểm tra ────────────────────────────────────────────────────────────

    public static bool CanAfford(ShopItemData item)
    {
        int coins = GameManager.Instance != null
            ? GameManager.Instance.GetCoin()
            : PlayerPrefs.GetInt(DataKey.COIN, 0);
        return coins >= item.coinPrice;
    }


    // ─── Mua ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Thực hiện giao dịch mua. Trả về true nếu thành công.
    /// </summary>
    public static bool TryBuy(ShopItemData item)
    {
        if (!CanAfford(item)) return false;

        GameManager.Instance.SpendCoin(item.coinPrice);

        switch (item.itemType)
        {
            case ShopItemType.ExtraLife:
                GameManager.Instance.AddLife(item.amount);
                break;

            case ShopItemType.BombAmmo:
                AddPending(DataKey.PENDING_BOMB, item.amount);
                break;

            case ShopItemType.DartAmmo:
                AddPending(DataKey.PENDING_DART, item.amount);
                break;

            case ShopItemType.BoomerangAmmo:
                AddPending(DataKey.PENDING_BOOMERANG, item.amount);
                break;

            case ShopItemType.Shield:
                PlayerPrefs.SetInt(DataKey.PENDING_SHIELD, 1);
                PlayerPrefs.Save();
                break;
        }

        return true;
    }

    // ─── Tiêu thụ (gọi từ PlayerFire.Start / PlayerPowerUp.Start) ───────────

    /// <summary>Lấy và xoá Bomb đang chờ. Gọi trong PlayerFire.Start().</summary>
    public static int ConsumePendingBomb()      => ConsumeKey(DataKey.PENDING_BOMB);

    /// <summary>Lấy và xoá Dart đang chờ.</summary>
    public static int ConsumePendingDart()      => ConsumeKey(DataKey.PENDING_DART);

    /// <summary>Lấy và xoá Boomerang đang chờ.</summary>
    public static int ConsumePendingBoomerang() => ConsumeKey(DataKey.PENDING_BOOMERANG);

    /// <summary>Kiểm tra và xoá Shield đang chờ.</summary>
    public static bool ConsumePendingShield()
    {
        bool v = PlayerPrefs.GetInt(DataKey.PENDING_SHIELD, 0) > 0;
        if (v)
        {
            PlayerPrefs.SetInt(DataKey.PENDING_SHIELD, 0);
            PlayerPrefs.Save();
        }
        return v;
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static void AddPending(string key, int amount)
    {
        int current = PlayerPrefs.GetInt(key, 0);
        PlayerPrefs.SetInt(key, current + amount);
        PlayerPrefs.Save();
    }

    private static int ConsumeKey(string key)
    {
        int v = PlayerPrefs.GetInt(key, 0);
        if (v > 0)
        {
            PlayerPrefs.SetInt(key, 0);
            PlayerPrefs.Save();
        }
        return v;
    }
}
