using System.Collections;
using UnityEngine;

/// <summary>
/// Gạch coin kiểu Mario:
///  - Player đập từ dưới → gạch nảy lên/xuống
///  - questionMarkObject (child) bị tắt khi hết coin
///  - Coin pop lên theo cung rồi rơi xuống
///  - Coin biến mất khi player chạm vào
/// </summary>
public class BrickCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    [Tooltip("Số coin mỗi lần đập (hiện tại spawn 1 coin mỗi lần đập cho đến hết)")]
    public int coinAmount = 3;
    [Tooltip("Số điểm score mỗi lần đập")]      
    public int coinScore = 10;
    [Tooltip("Prefab của coin sẽ pop ra")]
    public GameObject coinPrefab;
    [Tooltip("Âm thanh khi coin bật ra")]
    public AudioClip coinSound;

    [Header("Brick Visual")]
    [Tooltip("GameObject con chứa biểu tượng dấu ?")]
    public GameObject questionMarkObject;


    [Header("Bump Animation")]
    [Tooltip("Chiều cao gạch nảy lên (đơn vị Unity)")]
    public float bumpHeight = 0.3f;
    [Tooltip("Thời gian nảy lên")]
    public float bumpUpDuration = 0.08f;
    [Tooltip("Thời gian rơi xuống")]
    public float bumpDownDuration = 0.12f;

    [Header("Coin Pop Animation")]
    [Tooltip("Độ cao coin bay lên")]
    public float coinPopHeight = 1.5f;
    [Tooltip("Thời gian coin bay lên")]
    public float coinRiseDuration = 0.3f;
    [Tooltip("Thời gian coin rơi xuống sau khi đạt đỉnh")]
    public float coinFallDuration = 0.25f;
    [Tooltip("Coin tự biến mất sau bao giây nếu không được nhặt")]
    public float coinLifetime = 3f;

    // ─── Private state ───────────────────────────────────────────────
    private int remainingCoins;
    private bool isUsed = false;
    private bool isBumping = false;

    private Vector3 originalPosition;

    // ─────────────────────────────────────────────────────────────────

    private void Awake()
    {
        remainingCoins = coinAmount;
        originalPosition = transform.position;

        if (questionMarkObject != null)
            questionMarkObject.SetActive(true);
    }

    // ─── Detect va chạm từ phía dưới ─────────────────────────────────

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        bool hitFromBelow = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                hitFromBelow = true;
                break;
            }
        }

        
        if (!hitFromBelow) return;
        if (isUsed) return;
        if (isBumping) return;

        TriggerBrick();
    }


    private void TriggerBrick()
    {
        remainingCoins--;

        if (remainingCoins <= 0)
        {
            isUsed = true;
            if (questionMarkObject != null)
                questionMarkObject.SetActive(false);
        }

        // Chạy bump animation + spawn coin cùng lúc
        StartCoroutine(BumpAnimation());
        StartCoroutine(SpawnCoinEffect());

        // Cộng điểm
        GameManager.Instance?.AddScore(coinScore);

        // Phát âm thanh
        if (coinSound != null)
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
    }

    // ─── Bump: gạch nảy lên rồi về vị trí cũ ────────────────────────

    private IEnumerator BumpAnimation()
    {
        isBumping = true;

        Vector3 upPosition = originalPosition + Vector3.up * bumpHeight;

        // Đi lên
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / bumpUpDuration;
            transform.position = Vector3.Lerp(originalPosition, upPosition, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // (questionMarkObject đã tắt trong TriggerBrick rồi)

        // Rơi xuống
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / bumpDownDuration;
            transform.position = Vector3.Lerp(upPosition, originalPosition, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transform.position = originalPosition;
        isBumping = false;
    }

    // ─── Coin pop: spawn coin và cho bay theo cung ───────────────────

    private IEnumerator SpawnCoinEffect()
    {
        if (coinPrefab == null) yield break;

        // Spawn coin ngay phía trên gạch (tắt collider & physics để chỉ làm hiệu ứng)
        Vector3 spawnPos = originalPosition + Vector3.up * 0.6f;
        GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

        // Tắt hoàn toàn vật lý, trigger và script Coin
        // (BrickCoin tự xử lý AddCoin — không để Coin.cs gọi AddCoin(1))
        Coin coinScript = coin.GetComponent<Coin>();
        if (coinScript != null) coinScript.enabled = false;

        Collider2D coinCollider = coin.GetComponent<Collider2D>();
        Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
        if (coinRb != null) coinRb.simulated = false;
        if (coinCollider != null) coinCollider.enabled = false;

        Vector3 topPos = spawnPos + Vector3.up * coinPopHeight;

        // Bay lên mượt
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / coinRiseDuration;
            if (coin == null) yield break;
            coin.transform.position = Vector3.Lerp(spawnPos, topPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // Rơi xuống
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / coinFallDuration;
            if (coin == null) yield break;
            coin.transform.position = Vector3.Lerp(topPos, spawnPos, t * t);
            yield return null;
        }

        if (coin == null) yield break;

        int coinValue = Random.Range(10, 16); // 10 đến 15 (inclusive)
        GameManager.Instance?.AddCoin(coinValue);
        Destroy(coin);
    }
}
