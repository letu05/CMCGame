using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gắn vào GameObject GiftPanel (ẩn ban đầu).
/// Gọi Show() từ LevelManager khi player qua màn.
/// 
/// FIX: StartCoroutine được chạy bởi LevelManager (luôn active)
/// thay vì GiftPanel tự chạy — tránh lỗi "Coroutine couldn't be started
/// because the game object is inactive".
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

    // Runner ngoài để chạy coroutine (LevelManager hoặc MonoBehaviour luôn active)
    private MonoBehaviour coroutineRunner;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        // KHÔNG gọi SetActive(false) ở đây!
        // Nếu object chưa từng active, SetActive(true) sẽ trigger Awake()
        // và lệnh SetActive(false) bên trong sẽ lập tức tắt lại object → panel không hiện.
        // Thay vào đó: đánh dấu inactive trong Unity Editor (bỏ tick checkbox)
        // và LevelManager.Start() sẽ đảm bảo panel ẩn qua code.
        if (claimButton != null)
            claimButton.onClick.AddListener(OnClaim);
    }

    /// <summary>
    /// Gọi từ LevelManager.LevelComplete().
    /// runner  : MonoBehaviour luôn active để chạy coroutine (truyền LevelManager.Instance).
    /// onDone  : callback chạy sau khi player nhấn CLAIM (thường là hiện Victory panel).
    /// </summary>
    public void Show(MonoBehaviour runner, Action onDone = null)
    {
        coroutineRunner = runner;
        onClaimDone     = onDone;

        gameObject.SetActive(true);  // an toàn vì Awake() không còn gọi SetActive(false)
        spinDone = false;
        if (claimButton != null) claimButton.gameObject.SetActive(false);

        selectedIndex = UnityEngine.Random.Range(0, cards.Length);
        HighlightCard(0);

        coroutineRunner.StartCoroutine(SpinRoutine());
    }



    

    private IEnumerator SpinRoutine()
    {
        int current = 0;

        for (int step = 0; step < totalSteps; step++)
        {
            HighlightCard(current);
            current = (current + 1) % cards.Length;

            // Interval tăng dần (nhanh → chậm)
            float t        = (float)step / totalSteps;
            float interval = Mathf.Lerp(spinFastInterval, spinSlowInterval, t);
            yield return new WaitForSecondsRealtime(interval); // Realtime → không bị đứng bởi timeScale
        }

        // Dừng đúng card đã chọn ngẫu nhiên
        while (current % cards.Length != selectedIndex)
        {
            HighlightCard(current % cards.Length);
            current++;
            yield return new WaitForSecondsRealtime(spinSlowInterval);
        }

        HighlightCard(selectedIndex);
        spinDone = true;

        if (claimButton != null) claimButton.gameObject.SetActive(true);
    }

    // ─── Claim ────────────────────────────────────────────────────────────────

    private void OnClaim()
    {
        if (!spinDone) return;

        int reward = coinValues[selectedIndex];
        GameManager.Instance?.AddCoin(reward);

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
