using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [SerializeField] private GameObject playerSmall;
    [SerializeField] private GameObject playerMedium;
    [SerializeField] private float shieldDuration = 3f;

    private float shieldTimer = 0f;

    // Property để các script khác kiểm tra
    public bool IsShielded { get; private set; } = false;

    private void Start()
    {
        // Tự tìm nếu chưa gắn trong Inspector
        if (playerSmall  == null) playerSmall  = transform.Find("PlayerSmall")?.gameObject;
        if (playerMedium == null) playerMedium = transform.Find("PlayerMedium")?.gameObject;

        SetShieldObjects(false); // Ẩn shield model lúc đầu
    }

    void Update()
    {
        if (IsShielded)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                DeactivateShield();
            }
        }
    }

    // Gọi từ bên ngoài (vd: ăn item shield) để bật shield
    public void ActivateShield()
    {
        IsShielded = true;
        shieldTimer = shieldDuration;
        SetShieldObjects(true);
        Debug.Log($"[PlayerShield] Shield bật! Thời gian: {shieldDuration}s");
    }

    private void DeactivateShield()
    {
        IsShielded = false;
        shieldTimer = 0f;
        SetShieldObjects(false);
        Debug.Log("[PlayerShield] Shield hết hiệu lực.");
    }

    // Bật/tắt các GameObject shield giống PlayerPowerUp.SetPlayerState()
    private void SetShieldObjects(bool active)
    {
        if (playerSmall  != null) playerSmall.SetActive(active);
        if (playerMedium != null) playerMedium.SetActive(active);
    }
}
