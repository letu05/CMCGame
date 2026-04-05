using UnityEngine;

/// <summary>
/// Gắn vào scene gameplay. Thực thi phần thưởng sau khi user xem quảng cáo xong.
/// Yêu cầu: gán pointItemPrefab trong Inspector.
/// </summary>
public class AdRewardHandler : MonoBehaviour
{
    public static AdRewardHandler Instance { get; private set; }

    [Header("Thưởng Bomb")]
    [SerializeField] private int bombRewardAmount = 5;

    [Header("Thưởng PointItem — Spawn gần player")]
    [SerializeField] private GameObject pointItemPrefab;       // ← GÁN PREFAB POINTITEM VÀO ĐÂY
    [SerializeField] private Vector2    pointItemSpawnOffset = new Vector2(2f, 1f);

    [Header("Thưởng Coin / Vàng")]
    [SerializeField] private int coinRewardAmount = 50;

    // ─── Singleton ────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ─── 4 loại phần thưởng ──────────────────────────────────────────────────

    /// <summary>Thưởng 1: +5 Bomb ammo cho player.</summary>
    public void RewardBomb()
    {
        PlayerFire pf = FindFirstObjectByType<PlayerFire>();
        if (pf != null)
        {
            pf.AddAmmo(BulletType.Bomb, bombRewardAmount);
            Debug.Log($"[AdReward] +{bombRewardAmount} Bomb!");
        }
        else
        {
            Debug.LogWarning("[AdReward] Không tìm thấy PlayerFire trong scene.");
        }
    }

    /// <summary>Thưởng 2: kích hoạt Shield cho player.</summary>
    public void RewardShield()
    {
        PlayerPowerUp ppu = FindFirstObjectByType<PlayerPowerUp>();
        if (ppu != null)
        {
            ppu.ActivateShield();
            Debug.Log("[AdReward] +1 Shield!");
        }
        else
        {
            Debug.LogWarning("[AdReward] Không tìm thấy PlayerPowerUp trong scene.");
        }
    }

    /// <summary>Thưởng 3: spawn 1 PointItem gần player.</summary>
    public void RewardPointItem()
    {
        if (pointItemPrefab == null)
        {
            Debug.LogWarning("[AdReward] pointItemPrefab chưa được gán!");
            return;
        }

        // Tìm vị trí player
        PlayerPowerUp ppu = FindFirstObjectByType<PlayerPowerUp>();
        Vector3 spawnPos = ppu != null
            ? ppu.transform.position + (Vector3)pointItemSpawnOffset
            : Vector3.zero + (Vector3)pointItemSpawnOffset;

        Instantiate(pointItemPrefab, spawnPos, Quaternion.identity);
        Debug.Log("[AdReward] +1 PointItem spawned!");
    }

    /// <summary>Thưởng 4: cộng coin / vàng.</summary>
    public void RewardCoin()
    {
        GameManager.Instance?.AddCoin(coinRewardAmount);
        Debug.Log($"[AdReward] +{coinRewardAmount} Coin!");
    }
}
