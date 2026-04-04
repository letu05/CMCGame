using UnityEngine;

/// <summary>
/// Gắn vào GameObject cánh cửa lâu đài.
/// Khi được gọi OpenGate(), script sẽ trigger animation "hit"
/// (animation mà bạn đã tạo sẵn) để mở cửa.
/// </summary>
public class CastleGate : MonoBehaviour
{
    [Header("Animator")]
    [Tooltip("Animator của cánh cửa. Nếu để trống sẽ tự lấy trên GameObject này.")]
    [SerializeField] private Animator gateAnimator;

    [Tooltip("Tên trigger trong Animator để chạy animation mở cửa")]
    [SerializeField] private string openTriggerName = "hit";

    private bool isOpen = false;

    private void Awake()
    {
        if (gateAnimator == null)
            gateAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Gọi hàm này để mở cửa (play animation "hit").
    /// Được gọi từ CastleColumn sau khi cột trượt xuống xong.
    /// </summary>
    public void OpenGate()
    {
        if (isOpen) return;
        isOpen = true;

        if (gateAnimator != null)
        {
            gateAnimator.SetTrigger(openTriggerName);
            Debug.Log("[CastleGate] Đã trigger animation mở cửa: " + openTriggerName);
        }
        else
        {
            Debug.LogWarning("[CastleGate] Không tìm thấy Animator!");
        }
    }
}
