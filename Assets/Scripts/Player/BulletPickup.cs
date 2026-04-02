using UnityEngine;

/// <summary>
/// Gắn vào GameObject nhặt đạn trên sàn.
/// Khi player chạm vào → cộng +5 đạn cho loại được chỉ định rồi huỷ item.
/// </summary>
public class BulletPickup : MonoBehaviour
{
    [Header("Loại đạn và số lượng cộng thêm")]
    [SerializeField] private BulletType bulletType  = BulletType.Dart;
    [SerializeField] private int        ammoAmount  = 5;

    [Header("Layer của Player")]
    [SerializeField] private string playerTag = "Player"; // Dùng tag thay vì layer cho đơn giản

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Tìm PlayerFire trên player
        PlayerFire playerFire = other.GetComponentInParent<PlayerFire>();
        if (playerFire == null)
            playerFire = other.GetComponent<PlayerFire>();

        if (playerFire != null)
        {
            playerFire.AddAmmo(bulletType, ammoAmount);
            Debug.Log($"[BulletPickup] Nhặt +{ammoAmount} đạn {bulletType}");
        }
        else
        {
            Debug.LogWarning("[BulletPickup] Không tìm thấy PlayerFire trên player!");
        }

        Destroy(gameObject); // Xoá item sau khi nhặt
    }

    // Hiển thị icon loại đạn trong Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = bulletType switch
        {
            BulletType.Bomb      => new Color(1f, 0.4f, 0f),   // Cam
            BulletType.Dart      => Color.cyan,                  // Xanh lam
            BulletType.Boomerang => Color.green,                 // Xanh lá
            _                   => Color.white
        };
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
