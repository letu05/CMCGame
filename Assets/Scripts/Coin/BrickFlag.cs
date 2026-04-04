using System.Collections;
using UnityEngine;

/// <summary>
/// Gắn vào GameObject cha "Brick_Flag".
/// Khi Player chạm vào Collider (Trigger) của cot_co2:
///   1. Cột (cot_co2) và cờ (endFlag) trượt xuống.
///   2. Cổng (openGate) play animation "hit" để mở.
///   3. Gọi LevelManager.LevelComplete() sau một khoảng trễ.
/// </summary>
public class BrickFlag : MonoBehaviour
{
    [Header("Tham chiếu con (kéo từ Hierarchy vào)")]
    [Tooltip("GameObject cot_co2 – cột sẽ đi xuống")]
    [SerializeField] private Transform cot_co2;

    [Tooltip("GameObject endFlag – lá cờ trên đỉnh cột")]
    [SerializeField] private Transform endFlag;

    [Tooltip("GameObject openGate – cánh cửa lâu đài có Animator")]
    [SerializeField] private Animator openGateAnimator;

    [Header("Cài đặt cột đi xuống")]
    [Tooltip("Cột di chuyển xuống bao nhiêu đơn vị")]
    [SerializeField] private float dropDistance = 3f;

    [Tooltip("Thời gian (giây) để cột trượt hết")]
    [SerializeField] private float dropDuration = 0.8f;

    [Header("Cài đặt cửa & hoàn thành")]
    [Tooltip("Tên Trigger trong Animator của openGate (animation mở cửa)")]
    [SerializeField] private string gateTrigger = "hit";

    [Tooltip("Chờ bao lâu sau khi cửa mở thì gọi LevelComplete")]
    [SerializeField] private float completionDelay = 1.5f;

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;

    // ────────────────────────────────────────────────────────────────────
    //  Phát hiện Player chạm vào Trigger của cot_co2
    //  (Đặt Trigger Collider 2D lên cot_co2, hoặc lên Brick_Flag cha)
    // ────────────────────────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        hasTriggered = true;
        StartCoroutine(FlagSequence());
    }

    // ────────────────────────────────────────────────────────────────────
    //  Trình tự animation
    // ────────────────────────────────────────────────────────────────────
    private IEnumerator FlagSequence()
    {
        // 1. Cột + cờ cùng đi xuống
        yield return StartCoroutine(DropObjects());

        // 2. Mở cửa lâu đài
        OpenGate();

        // 3. Chờ rồi báo hoàn thành màn
        yield return new WaitForSeconds(completionDelay);
        LevelManager.Instance?.LevelComplete();
    }

    private IEnumerator DropObjects()
    {
        // Lưu vị trí ban đầu
        Vector3 cotStart  = cot_co2  != null ? cot_co2.position  : Vector3.zero;
        Vector3 flagStart = endFlag  != null ? endFlag.position   : Vector3.zero;
        Vector3 drop      = Vector3.down * dropDistance;

        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / dropDuration);

            if (cot_co2  != null) cot_co2.position  = Vector3.Lerp(cotStart,  cotStart  + drop, t);
            if (endFlag  != null) endFlag.position   = Vector3.Lerp(flagStart, flagStart + drop, t);

            yield return null;
        }

        // Đảm bảo chính xác vị trí cuối
        if (cot_co2  != null) cot_co2.position  = cotStart  + drop;
        if (endFlag  != null) endFlag.position   = flagStart + drop;
    }

    private void OpenGate()
    {
        if (openGateAnimator != null)
        {
            openGateAnimator.SetTrigger(gateTrigger);
            Debug.Log("[BrickFlag] Đã trigger mở cửa: " + gateTrigger);
        }
        else
        {
            Debug.LogWarning("[BrickFlag] Chưa gán openGateAnimator!");
        }
    }
}
