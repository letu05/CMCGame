using UnityEngine;

/// <summary>
/// Chỉ xử lý việc BẮN:
///  - Quản lý 3 prefab (Bomb, Dart, Boomerang) và số lượng đạn từng loại
///  - Spawn đúng prefab khi bắn, trừ ammo, gọi bullet.Init()
///  - Cho phép nhặt đạn từ ngoài qua AddAmmo()
/// </summary>
public class PlayerFire : MonoBehaviour
{
    
    [Header("Prefab từng loại đạn (kéo 3 prefab vào đây)")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject dartPrefab;
    [SerializeField] private GameObject boomerangPrefab;

    [Header("Điểm xuất đạn & cooldown")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float     fireCooldown = 0.4f;

    [Header("Số đạn ban đầu mỗi loại")]
    [SerializeField] private int startBombAmmo      = 5;
    [SerializeField] private int startDartAmmo      = 10;
    [SerializeField] private int startBoomerangAmmo = 5;

    // ─────────────────────────── Runtime ──────────────────────────────────
    private int   bombAmmo;
    private int   dartAmmo;
    private int   boomerangAmmo;

    [SerializeField] private BulletType currentType   = BulletType.Dart; // Loại đang chọn
    private float      cooldownTimer = 0f;
    private bool       isFacingRight = true;

    // ─────────────────────────── Properties ───────────────────────────────
    public int BombAmmo      => bombAmmo;
    public int DartAmmo      => dartAmmo;
    public int BoomerangAmmo => boomerangAmmo;
    public BulletType CurrentType => currentType;

    // ─────────────────────────── Unity ────────────────────────────────────
    private void Start()
    {
        bombAmmo      = startBombAmmo;
        dartAmmo      = startDartAmmo;
        boomerangAmmo = startBoomerangAmmo;
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        // Đồng bộ hướng mặt theo scale (PlayerController flip scale.x)
        isFacingRight = transform.localScale.x > 0f;

        if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.F) && cooldownTimer <= 0f)
        {
            TryFire();
            cooldownTimer = fireCooldown;
        }
    }


    private void TryFire()
    {
        if (firePoint == null)
        {
            Debug.LogWarning("[PlayerFire] Chưa gán firePoint!");
            return;
        }

        // Lấy prefab & kiểm tra ammo theo loại đang chọn
        GameObject prefab = GetPrefab(currentType);
        if (prefab == null)
        {
            Debug.LogWarning($"[PlayerFire] Chưa gán prefab cho {currentType}!");
            return;
        }

        if (GetAmmo(currentType) <= 0)
        {
            Debug.Log($"[PlayerFire] Hết đạn {currentType}!");
            return;
        }

        // Spawn & khởi tạo đạn
        Vector2    direction = isFacingRight ? Vector2.right : Vector2.left;
        GameObject go        = Instantiate(prefab, firePoint.position, Quaternion.identity);

        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Init(direction, transform);
        else
            Debug.LogWarning("[PlayerFire] Prefab không có component Bullet!");

        // Trừ ammo
        ConsumeAmmo(currentType);
        Debug.Log($"[PlayerFire] Bắn {currentType}. Còn lại: {GetAmmo(currentType)}");
    }


    /// <summary>Nhặt đạn → cộng thêm amount viên cho loại đó.</summary>
    public void AddAmmo(BulletType type, int amount)
    {
        switch (type)
        {
            case BulletType.Bomb:      bombAmmo      += amount; break;
            case BulletType.Dart:      dartAmmo      += amount; break;
            case BulletType.Boomerang: boomerangAmmo += amount; break;
        }
        Debug.Log($"[PlayerFire] Nhặt +{amount} đạn {type}. Tổng: {GetAmmo(type)}");
    }

    /// <summary>Đổi loại đạn đang dùng (gắn vào UI button / PowerUp).</summary>
    public void SwitchType(BulletType type) => currentType = type;

    
    private GameObject GetPrefab(BulletType type) => type switch
    {
        BulletType.Bomb      => bombPrefab,
        BulletType.Dart      => dartPrefab,
        BulletType.Boomerang => boomerangPrefab,
        _                    => null
    };

    private int GetAmmo(BulletType type) => type switch
    {
        BulletType.Bomb      => bombAmmo,
        BulletType.Dart      => dartAmmo,
        BulletType.Boomerang => boomerangAmmo,
        _                    => 0
    };

    private void ConsumeAmmo(BulletType type)
    {
        switch (type)
        {
            case BulletType.Bomb:      bombAmmo      = Mathf.Max(0, bombAmmo      - 1); break;
            case BulletType.Dart:      dartAmmo      = Mathf.Max(0, dartAmmo      - 1); break;
            case BulletType.Boomerang: boomerangAmmo = Mathf.Max(0, boomerangAmmo - 1); break;
        }
    }

    // ─────────────────────────── Gizmos ───────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        // Vẽ hình cầu vàng tại vị trí firePoint
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(firePoint.position, 0.1f);

        // Vẽ đường thẳng theo hướng bắn (dựa theo scale.x)
        float dir = transform.localScale.x >= 0f ? 1f : -1f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + new Vector3(dir * 0.5f, 0f, 0f));
    }
}
