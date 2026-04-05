using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Gạch điểm kiểu Mario:
///  - Player đập từ dưới → gạch nảy lên/xuống
///  - Kiểm tra PlayerPowerUp: có IsBig hoặc IsShielded → điểm thưởng thêm
///  - Hiện text "+score" nổi lên rồi mờ dần (không cần nhặt)
///  - Hết lượt → ẩn questionMarkObject
/// </summary>
public class BrickPoint : MonoBehaviour
{
    [Header("Point Settings")]
    [Tooltip("Số lần có thể đập")]
    public int       useCount   = 1;
    [Tooltip("Điểm cơ bản mỗi lần đập")]
    public int       scoreValue = 100;
    [Tooltip("Điểm thưởng thêm nếu player có PowerUp (IsBig hoặc IsShielded)")]
    public int       scoreBonus = 200;
    [Tooltip("Âm thanh khi đập")]
    public AudioClip hitSound;

    [Header("Brick Visual")]
    public GameObject questionMarkObject;

    [Header("Bump Animation")]
    public float bumpHeight       = 0.3f;
    public float bumpUpDuration   = 0.08f;
    public float bumpDownDuration = 0.12f;

    [Header("Floating Text Effect")]
    [Tooltip("Prefab chứa TextMeshPro hiển thị +score")]
    public GameObject floatTextPrefab;
    public float      floatHeight    = 1.2f;
    public float      floatDuration  = 0.7f;
    [Tooltip("Màu text khi không có powerup")]
    public Color      textColorNormal = new Color(1f, 0.9f, 0.1f);  // vàng
    [Tooltip("Màu text khi có powerup (bonus)")]
    public Color      textColorBonus  = new Color(1f, 0.4f, 0.1f);  // cam đỏ

    // ─── Private state ────────────────────────────────────────────────────────
    private int     remaining;
    private bool    isUsed    = false;
    private bool    isBumping = false;
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
        TriggerBrick(collision.gameObject);
    }

    // ─── Trigger ──────────────────────────────────────────────────────────────

    private void TriggerBrick(GameObject player)
    {
        // ── Kiểm tra PowerUp ──────────────────────────────────────────────────
        PlayerPowerUp powerUp    = player.GetComponent<PlayerPowerUp>();
        bool          hasPowerUp = powerUp != null && (powerUp.IsBig || powerUp.IsShielded);

        if (hasPowerUp)
        {
            // Có PowerUp → trừ lượt
            remaining--;
            bool shouldDeactivate = remaining <= 0;
            if (shouldDeactivate) isUsed = true;

            int   totalScore = scoreValue + scoreBonus;
            Color color      = textColorBonus;

            GameManager.Instance?.AddScore(totalScore);
            // Bắt đầu coroutine TRƯỚC khi deactivate — truyền cờ để ẩn sau animation
            StartCoroutine(BumpAnimation(shouldDeactivate));
            StartCoroutine(FloatTextEffect(totalScore, color));
        }
        else
        {
            // Đập thường → chỉ nảy lên xuống, KHÔNG cộng điểm, KHÔNG float text
            StartCoroutine(BumpAnimation(false));
        }

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
    }

    // ─── Bump ─────────────────────────────────────────────────────────────────

    private IEnumerator BumpAnimation(bool deactivateAfter = false)
    {
        isBumping = true;
        Vector3 upPos = originalPosition + Vector3.up * bumpHeight;

        float t = 0f;
        while (t < 1f) { t += Time.deltaTime / bumpUpDuration;   transform.position = Vector3.Lerp(originalPosition, upPos, Mathf.SmoothStep(0,1,t)); yield return null; }
        t = 0f;
        while (t < 1f) { t += Time.deltaTime / bumpDownDuration; transform.position = Vector3.Lerp(upPos, originalPosition, Mathf.SmoothStep(0,1,t)); yield return null; }

        transform.position = originalPosition;
        isBumping = false;

        // Ẩn questionMark SAU KHI animation bump hoàn tất
        if (deactivateAfter && questionMarkObject != null)
            questionMarkObject.SetActive(false);
    }

    // ─── Floating Text ────────────────────────────────────────────────────────

    private IEnumerator FloatTextEffect(int score, Color color)
    {
        if (floatTextPrefab == null) yield break;

        Vector3    startPos = originalPosition + Vector3.up * 0.8f;
        GameObject obj      = Instantiate(floatTextPrefab, startPos, Quaternion.identity);

        TMP_Text tmp = obj.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            tmp.text  = $"+{score}";
            tmp.color = color;
        }

        Vector3 endPos = startPos + Vector3.up * floatHeight;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / floatDuration;
            if (obj == null) yield break;

            obj.transform.position = Vector3.Lerp(startPos, endPos, t);

            // Mờ dần ở nửa sau
            if (tmp != null)
            {
                float alpha = t < 0.5f ? 1f : 1f - (t - 0.5f) * 2f;
                tmp.color = new Color(color.r, color.g, color.b, alpha);
            }

            yield return null;
        }

        if (obj != null) Destroy(obj);
    }
}
