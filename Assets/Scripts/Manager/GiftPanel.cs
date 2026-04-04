using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gắn vào GameObject GiftPanel (ẩn ban đầu).
/// Gọi Show() từ LevelManager khi player qua màn.
/// </summary>
public class GiftPanel : MonoBehaviour
{
    [Header("Cards (theo thứ tự hiển thị)")]
    [SerializeField] private GameObject[] cards;          // 5 card GameObject
    [SerializeField] private int[]        coinValues = { 100, 200, 300, 400, 500 };

    [Header("Màu viền card")]
    [SerializeField] private Color normalColor   = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    [Header("Nút CLAIM")]
    [SerializeField] private Button claimButton;

    // Callback do LevelManager truyền vào — gọi sau khi player nhấn CLAIM
    private Action onClaimDone;

    // ─── Spin settings ────────────────────────────────────────────────────────
    [Header("Cài đặt quay")]
    [SerializeField] private float spinFastInterval = 0.05f;  // 50ms/card (nhanh)
    [SerializeField] private float spinSlowInterval = 0.30f;  // 300ms/card (chậm)
    [SerializeField] private int   totalSteps       = 30;     // tổng số bước chạy

    private int   selectedIndex  = 0;
    private bool  spinDone       = false;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        gameObject.SetActive(false); // ẩn ban đầu
        if (claimButton != null)
            claimButton.onClick.AddListener(OnClaim);
    }

    /// <summary>
    /// Gọi từ LevelManager.LevelComplete().
    /// onDone: callback chạy sau khi player nhấn CLAIM (thường là hiện Victory panel).
    /// </summary>
    public void Show(Action onDone = null)
    {
        onClaimDone = onDone;
        gameObject.SetActive(true);
        spinDone = false;
        if (claimButton != null) claimButton.gameObject.SetActive(false);

        selectedIndex = UnityEngine.Random.Range(0, cards.Length);
        HighlightCard(0);
        StartCoroutine(SpinRoutine());
    }

    // ─── Coroutine quay ───────────────────────────────────────────────────────

    private IEnumerator SpinRoutine()
    {
        int current = 0;

        for (int step = 0; step < totalSteps; step++)
        {
            HighlightCard(current);
            current = (current + 1) % cards.Length;

            // Interval tăng dần (nhanh → chậm) trong 70% cuối
            float t        = (float)step / totalSteps;
            float interval = Mathf.Lerp(spinFastInterval, spinSlowInterval, t);
            yield return new WaitForSeconds(interval);
        }

        // Dừng đúng card đã chọn ngẫu nhiên
        // Di chuyển tiếp cho đến khi tới selectedIndex
        while (current % cards.Length != selectedIndex)
        {
            HighlightCard(current % cards.Length);
            current++;
            yield return new WaitForSeconds(spinSlowInterval);
        }

        HighlightCard(selectedIndex);
        spinDone = true;

        // Hiện nút CLAIM
        if (claimButton != null) claimButton.gameObject.SetActive(true);

        Debug.Log($"[GiftPanel] Dừng tại card {selectedIndex} → +{coinValues[selectedIndex]} coin");
    }

    // ─── Claim ────────────────────────────────────────────────────────────────

    private void OnClaim()
    {
        if (!spinDone) return;

        int reward = coinValues[selectedIndex];
        GameManager.Instance?.AddCoin(reward);
        Debug.Log($"[GiftPanel] Cộng {reward} coin. Đóng GiftPanel → chạy callback.");

        gameObject.SetActive(false);
        onClaimDone?.Invoke(); // → LevelManager sẽ hiện Victory panel
    }

    // ─── Highlight ────────────────────────────────────────────────────────────

    private void HighlightCard(int index)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;
            Image outline = cards[i].GetComponent<Image>();
            if (outline != null)
                outline.color = (i == index) ? selectedColor : normalColor;
        }
    }
}
