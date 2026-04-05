using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ControlFreak2;

/// <summary>
/// Gắn script này vào button bắn đạn.
/// Tự động cập nhật sprite của CF2 TouchButtonSpriteAnimator theo loại đạn đang active.
/// Hiển thị số lượng đạn hiện tại qua TMP Text.
/// Ưu tiên: Dart → Boomerang → Bomb.
/// </summary>
public class BulletTypeDisplay : MonoBehaviour
{
    [Header("Icon từng loại đạn")]
    [SerializeField] private Sprite bombIcon;
    [SerializeField] private Sprite dartIcon;
    [SerializeField] private Sprite boomerangIcon;

    [Header("Text hiển thị số đạn")]
    [Tooltip("TMP Text hiển thị số lượng đạn (kéo Text con vào đây)")]
    [SerializeField] private TMP_Text ammoText;

    // ─── Cache ────────────────────────────────────────────────────────────────
    private PlayerFire                playerFire;
    private TouchButtonSpriteAnimator cf2Animator;
    private BulletType?               lastType;
    private int                       lastAmmo = -1;

    private void Awake()
    {
        cf2Animator = GetComponent<TouchButtonSpriteAnimator>();

        // Tự tìm TMP_Text trong children nếu chưa assign
        if (ammoText == null)
            ammoText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        playerFire = FindObjectOfType<PlayerFire>();
        if (playerFire == null)
            Debug.LogWarning("[BulletTypeDisplay] Không tìm thấy PlayerFire trong scene!");

        if (cf2Animator == null)
            Debug.LogWarning("[BulletTypeDisplay] Không tìm thấy TouchButtonSpriteAnimator trên GameObject này!");

        RefreshAll();
    }

    private void Update()
    {
        BulletType? currentType = playerFire?.ActiveType;
        int         currentAmmo = GetCurrentAmmo(currentType);

        // Chỉ refresh khi có thay đổi
        if (currentType != lastType || currentAmmo != lastAmmo)
        {
            lastType = currentType;
            lastAmmo = currentAmmo;
            RefreshAll();
        }
    }

    // ─── Refresh ──────────────────────────────────────────────────────────────

    private void RefreshAll()
    {
        // Cập nhật icon
        Sprite icon = GetIconForType(playerFire?.ActiveType);
        if (icon != null && cf2Animator != null)
            cf2Animator.SetSprite(icon);

        // Cập nhật số đạn
        if (ammoText != null)
            ammoText.text = lastAmmo >= 0 ? lastAmmo.ToString() : "0";
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private int GetCurrentAmmo(BulletType? type)
    {
        if (playerFire == null || type == null) return 0;

        return type.Value switch
        {
            BulletType.Bomb      => playerFire.BombAmmo,
            BulletType.Dart      => playerFire.DartAmmo,
            BulletType.Boomerang => playerFire.BoomerangAmmo,
            _                    => 0
        };
    }

    private Sprite GetIconForType(BulletType? type)
    {
        if (type == null) return null;

        return type.Value switch
        {
            BulletType.Bomb      => bombIcon,
            BulletType.Dart      => dartIcon,
            BulletType.Boomerang => boomerangIcon,
            _                    => null
        };
    }
}
