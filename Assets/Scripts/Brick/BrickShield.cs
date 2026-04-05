using System.Collections;
using UnityEngine;

/// <summary>
/// Gạch khiên kiểu Mario:
///  - Player đập từ dưới → gạch nảy lên/xuống
///  - Spawn prefab Shield pickup bay lên rồi rơi xuống để player nhặt
///  - Hết lượt → ẩn questionMarkObject
/// </summary>
public class BrickShield : MonoBehaviour
{
    [Header("Shield Settings")]
    [Tooltip("Số lần có thể đập")]
    public int      useCount    = 1;
    [Tooltip("Prefab ShieldPickup sẽ pop ra (phải có ShieldPickup script)")]
    public GameObject shieldPrefab;
    [Tooltip("Âm thanh khi shield bật ra")]
    public AudioClip  shieldSound;

    [Header("Brick Visual")]
    public GameObject questionMarkObject;

    [Header("Bump Animation")]
    public float bumpHeight      = 0.3f;
    public float bumpUpDuration  = 0.08f;
    public float bumpDownDuration= 0.12f;

    [Header("Shield Pop Animation")]
    public float popHeight      = 1.5f;
    public float riseDuration   = 0.35f;
    public float fallDuration   = 0.28f;
    [Tooltip("Shield tồn tại bao lâu nếu không nhặt")]
    public float shieldLifetime = 5f;

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
        if (remaining <= 0)
        {
            isUsed = true;
            if (questionMarkObject != null) questionMarkObject.SetActive(false);
        }

        StartCoroutine(BumpAnimation());
        StartCoroutine(SpawnShieldEffect());

        if (shieldSound != null)
            AudioSource.PlayClipAtPoint(shieldSound, transform.position);
    }

    // ─── Bump ─────────────────────────────────────────────────────────────────

    private IEnumerator BumpAnimation()
    {
        isBumping = true;
        Vector3 upPos = originalPosition + Vector3.up * bumpHeight;

        float t = 0f;
        while (t < 1f) { t += Time.deltaTime / bumpUpDuration;   transform.position = Vector3.Lerp(originalPosition, upPos, Mathf.SmoothStep(0,1,t)); yield return null; }
        t = 0f;
        while (t < 1f) { t += Time.deltaTime / bumpDownDuration; transform.position = Vector3.Lerp(upPos, originalPosition, Mathf.SmoothStep(0,1,t)); yield return null; }

        transform.position = originalPosition;
        isBumping = false;
    }

    // ─── Shield Pop ───────────────────────────────────────────────────────────

    private IEnumerator SpawnShieldEffect()
    {
        if (shieldPrefab == null) yield break;

        Vector3 spawnPos = originalPosition + Vector3.up * 0.6f;
        Vector3 topPos   = spawnPos + Vector3.up * popHeight;

        // Spawn pickup — tắt collider tạm thời để làm hiệu ứng bay trước
        GameObject shield = Instantiate(shieldPrefab, spawnPos, Quaternion.identity);
        Collider2D col    = shield.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Bay lên
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / riseDuration;
            if (shield == null) yield break;
            shield.transform.position = Vector3.Lerp(spawnPos, topPos, Mathf.SmoothStep(0,1,t));
            yield return null;
        }

        // Rơi xuống về vị trí land
        Vector3 landPos = spawnPos;
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fallDuration;
            if (shield == null) yield break;
            shield.transform.position = Vector3.Lerp(topPos, landPos, t * t);
            yield return null;
        }

        if (shield == null) yield break;

        // Bật collider để player nhặt được
        if (col != null) col.enabled = true;

        // Tự hủy nếu không nhặt
        Destroy(shield, shieldLifetime);
    }
}
