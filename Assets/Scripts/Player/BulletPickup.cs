using UnityEngine;

/// <summary>
/// Gắn vào prefab đạn nhặt được trên sàn hoặc do BrickBullet spawn ra.
/// - Khi player chạm vào (Trigger) → cộng đạn rồi hủy item.
/// - Nếu spawn từ BrickBullet: gọi SetupFromBrick() để set loại đạn/số lượng.
/// </summary>
public class BulletPickup : MonoBehaviour
{
    [Header("Loại đạn và số lượng cộng thêm")]
    [SerializeField] private BulletType bulletType = BulletType.Dart;
    [SerializeField] private int        ammoAmount = 5;

    [Header("Layer của Player")]
    [SerializeField] private string playerTag = "Player";

   

    
    public void SetupFromBrick(BulletType type, int amount)
    {
        bulletType = type;
        ammoAmount = amount;
    }

    
    // nhặt đạn
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Tìm PlayerFire trên player
        PlayerFire playerFire = other.GetComponentInParent<PlayerFire>();
        if (playerFire == null)
            playerFire = other.GetComponent<PlayerFire>();

        if (playerFire != null)
            playerFire.AddAmmo(bulletType, ammoAmount);

        Destroy(gameObject);
    }

    // ─── Gizmos ──────────────────────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        Gizmos.color = bulletType switch
        {
            BulletType.Bomb      => new Color(1f, 0.4f, 0f),
            BulletType.Dart      => Color.cyan,
            BulletType.Boomerang => Color.green,
            _                    => Color.white
        };
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
