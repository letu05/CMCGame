using System.Collections;
using UnityEngine;

/// <summary>
/// Gạch đạn kiểu Mario:
///  - Player đập từ dưới → gạch nảy lên/xuống
///  - Spawn BulletPickup prefab bay lên rồi đứng yên trên đỉnh gạch
///  - Hết lượt → ẩn questionMarkObject
/// </summary>
public class BrickBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [Tooltip("Số lần có thể đập")]
    public int        useCount     = 1;
    [Tooltip("Prefab BulletPickup sẽ pop ra (phải có BulletPickup script)")]
    public GameObject bulletPrefab;
    [Tooltip("Loại đạn spawn ra")]
    public BulletType bulletType   = BulletType.Dart;
    [Tooltip("Số đạn cộng thêm khi nhặt")]
    public int        ammoAmount   = 5;
    [Tooltip("Âm thanh khi đạn bật ra")]
    public AudioClip  bulletSound;

    [Header("Brick Visual")]
    public GameObject questionMarkObject;

    [Header("Bump Animation")]
    public float bumpHeight      = 0.3f;
    public float bumpUpDuration  = 0.08f;
    public float bumpDownDuration= 0.12f;

    [Header("Bullet Pop Animation")]
    public float popHeight      = 1.5f;    // Độ cao item bay lên
    public float riseDuration   = 0.35f;   // Thời gian bay lên
    public float fallDuration   = 0.28f;   // Thời gian rơi xuống
    [Tooltip("Bullet tồn tại bao lâu nếu không nhặt (0 = không tự hủy)")]
    public float bulletLifetime = 8f;

    // ─── Private state ────────────────────────────────────────────────────────
    private int  remaining;
    private bool isUsed    = false;
    private bool isBumping = false;
    private Vector3 originalPosition;

    private void Awake()
    {
        remaining        = useCount;
        originalPosition = transform.position;
        if (questionMarkObject != null) questionMarkObject.SetActive(true);
    }

    // ─── Va chạm từ phía dưới ─────────────────────────────────────────────────

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (isUsed || isBumping) return;

        bool hitFromBelow = false;
        foreach (ContactPoint2D contact in collision.contacts)
            if (contact.normal.y > 0.5f) { hitFromBelow = true; break; }

        if (!hitFromBelow) return;
        TriggerBrick();
    }

    // ─── Trigger ──────────────────────────────────────────────────────────────

    private void TriggerBrick()
    {
        remaining--;
        bool lastUse = remaining <= 0;
        if (lastUse) isUsed = true;

        StartCoroutine(BumpAnimation(lastUse));
        StartCoroutine(SpawnBulletEffect());

        if (bulletSound != null)
            AudioSource.PlayClipAtPoint(bulletSound, transform.position);
    }

    // ─── Bump ─────────────────────────────────────────────────────────────────

    private IEnumerator BumpAnimation(bool deactivateAfter = false)
    {
        isBumping = true;
        Vector3 upPos = originalPosition + Vector3.up * bumpHeight;

        float t = 0f;
        while (t < 1f) { t += Time.deltaTime / bumpUpDuration;   transform.position = Vector3.Lerp(originalPosition, upPos,   Mathf.SmoothStep(0, 1, t)); yield return null; }
        t = 0f;
        while (t < 1f) { t += Time.deltaTime / bumpDownDuration; transform.position = Vector3.Lerp(upPos, originalPosition,   Mathf.SmoothStep(0, 1, t)); yield return null; }

        transform.position = originalPosition;
        isBumping = false;

        // Ẩn questionMark SAU KHI animation hoàn tất
        if (deactivateAfter && questionMarkObject != null)
            questionMarkObject.SetActive(false);
    }

    // ─── Bullet Pop ───────────────────────────────────────────────────────────

    private IEnumerator SpawnBulletEffect()
    {
        if (bulletPrefab == null) yield break;

        // Spawn tại đỉnh gạch
        Vector3 spawnPos = originalPosition + Vector3.up * 1f;
        Vector3 topPos   = spawnPos + Vector3.up * popHeight;
        Vector3 landPos  = spawnPos; // Đứng tại đỉnh gạch

        GameObject item = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        // ── Truyền loại đạn và số lượng vào BulletPickup ──────────────────────
        BulletPickup pickup = item.GetComponent<BulletPickup>();
        if (pickup != null)
        {
            pickup.SetupFromBrick(bulletType, ammoAmount);
        }

        // ── Tắt collider trong lúc bay để không trigger ngay ──────────────────
        Collider2D col = item.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // ── Tắt gravity để item không rơi xuống ───────────────────────────────
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity     = Vector2.zero;
        }

        // ── Fix Sorting Layer nếu cần ──────────────────────────────────────────
        SpriteRenderer sr = item.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Default";
            sr.sortingOrder     = 5;
        }

        // ── Bay lên ───────────────────────────────────────────────────────────
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / riseDuration;
            if (item == null) yield break;
            item.transform.position = Vector3.Lerp(spawnPos, topPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // ── Rơi xuống ─────────────────────────────────────────────────────────
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fallDuration;
            if (item == null) yield break;
            item.transform.position = Vector3.Lerp(topPos, landPos, t * t);
            yield return null;
        }

        if (item == null) yield break;

        // ── Đứng yên tại đỉnh gạch ────────────────────────────────────────────
        item.transform.position = landPos;

        // Đảm bảo Rigidbody frozen hoàn toàn (nếu có)
        if (rb != null)
        {
            rb.gravityScale  = 0f;
            rb.linearVelocity      = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType      = RigidbodyType2D.Kinematic; // Không bị vật lý tác động
        }

        // Bật collider để player nhặt được
        if (col != null) col.enabled = true;

        // Tự hủy nếu không nhặt
        if (bulletLifetime > 0f)
            Destroy(item, bulletLifetime);
    }
}
