using UnityEngine;

/// <summary>
/// Hệ thống bắn đạn:
///   - Bomb: VÔ HẠN, là vũ khí mặc định luôn sẵn sàng.
///   - Dart / Boomerang: mua từ Shop, dùng TRƯỚC khi có, hết thì về Bomb.
///   Ưu tiên bắn: Dart → Boomerang → Bomb.
/// </summary>
public class PlayerFire : MonoBehaviour
{
    [Header("Prefab từng loại đạn")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject dartPrefab;
    [SerializeField] private GameObject boomerangPrefab;

    [Header("Điểm xuất đạn & cooldown")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float     fireCooldown = 0.4f;

    [Header("Số đạn ban đầu (điều chỉnh trong Inspector)")]
    [SerializeField] private int startBombAmmo      = 5; // Bomb mặc định
    [SerializeField] private int startDartAmmo      = 0;
    [SerializeField] private int startBoomerangAmmo = 0;

    // ─── Runtime ammo ─────────────────────────────────────────────────────
    private int bombAmmo;
    private int dartAmmo;
    private int boomerangAmmo;

    private float cooldownTimer = 0f;
    private bool  isFacingRight = true;

    // ─── Properties ───────────────────────────────────────────────────────
    public int BombAmmo      => bombAmmo;
    public int DartAmmo      => dartAmmo;
    public int BoomerangAmmo => boomerangAmmo;
    public int TotalAmmo     => bombAmmo + dartAmmo + boomerangAmmo;

    public BulletType? ActiveType => GetActiveType();

    // ─── Unity ────────────────────────────────────────────────────────────

    private void Start()
    {
        bombAmmo      = startBombAmmo      + ShopManager.ConsumePendingBomb();
        dartAmmo      = startDartAmmo      + ShopManager.ConsumePendingDart();
        boomerangAmmo = startBoomerangAmmo + ShopManager.ConsumePendingBoomerang();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
        isFacingRight  = transform.localScale.x > 0f;

        if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.F) && cooldownTimer <= 0f)
        {
            TryFire();
            cooldownTimer = fireCooldown;
        }
    }

    // ─── Chọn đạn tự động ────────────────────────────────────────────────

    /// <summary>Dart → Boomerang → Bomb (mặc định). Null nếu hết tất cả.</summary>
    private BulletType? GetActiveType()
    {
        if (dartAmmo      > 0) return BulletType.Dart;
        if (boomerangAmmo > 0) return BulletType.Boomerang;
        if (bombAmmo      > 0) return BulletType.Bomb;
        return null; // hết tất cả đạn
    }

    // ─── Bắn ──────────────────────────────────────────────────────────────

    private void TryFire()
    {
        if (firePoint == null) return;

        BulletType? active = GetActiveType();
        if (active == null) return; // hết đạn

        BulletType type   = active.Value;
        GameObject prefab = GetPrefab(type);
        if (prefab == null) return;

        Vector2    dir = isFacingRight ? Vector2.right : Vector2.left;
        GameObject go  = Instantiate(prefab, firePoint.position, Quaternion.identity);

        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet != null) bullet.Init(dir, transform);

        // Trừ ammo đúng loại
        switch (type)
        {
            case BulletType.Bomb:      bombAmmo      = Mathf.Max(0, bombAmmo      - 1); break;
            case BulletType.Dart:      dartAmmo      = Mathf.Max(0, dartAmmo      - 1); break;
            case BulletType.Boomerang: boomerangAmmo = Mathf.Max(0, boomerangAmmo - 1); break;
        }
    }

    // ─── Public API ───────────────────────────────────────────────────────

    /// <summary>Nhặt đạn trong scene (BulletPickup gọi hàm này).</summary>
    public void AddAmmo(BulletType type, int amount)
    {
        switch (type)
        {
            case BulletType.Bomb:      bombAmmo      += amount; break;
            case BulletType.Dart:      dartAmmo      += amount; break;
            case BulletType.Boomerang: boomerangAmmo += amount; break;
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────────────

    private GameObject GetPrefab(BulletType type) => type switch
    {
        BulletType.Bomb      => bombPrefab,
        BulletType.Dart      => dartPrefab,
        BulletType.Boomerang => boomerangPrefab,
        _                    => null
    };

    // ─── Gizmos ───────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(firePoint.position, 0.1f);

        float dir = transform.localScale.x >= 0f ? 1f : -1f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + new Vector3(dir * 0.5f, 0f, 0f));
    }
}
