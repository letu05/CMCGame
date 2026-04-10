using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    [Header("4 Model nhân vật")]
    [SerializeField] private GameObject modelSmall;         
    [SerializeField] private GameObject modelBig;           
    [SerializeField] private GameObject modelSmallShield;   
    [SerializeField] private GameObject modelBigShield;     

    [Header("Cài đặt")]
    [SerializeField] private float jumpForceAdditional = 5f;
    [SerializeField] private float shieldDuration      = 3f;

    // Trạng thái
    public bool IsBig      { get; private set; } = false;
    public bool IsShielded { get; private set; } = false;

    private float shieldTimer = 0f;

    private PlayerController playerController;
    private PlayerHealth     playerHealth;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerHealth     = GetComponentInChildren<PlayerHealth>(true);
        UpdateModel();
    }

    private void Update()
    {
        if (!IsShielded) return;

        shieldTimer -= Time.deltaTime;
        if (shieldTimer <= 0f)
        {
            IsShielded = false;
            UpdateModel();
        }
    }

    

    
    public void ActivateShield()
    {
        IsShielded  = true;
        shieldTimer = shieldDuration;
        UpdateModel();
    }

    // ─── PowerUp ──────────────────────────────────────────────────────────────

    // Gọi khi ăn nấm (to lên)
    public void GrowBig()
    {
        if (IsBig) return;
        IsBig = true;
        playerController.JumpForce += jumpForceAdditional;
        UpdateModel();
    }

    // Gọi khi bị enemy chạm lúc đang to
    public void ShrinkSmall()
    {
        if (!IsBig) return;
        IsBig = false;
        playerController.JumpForce -= jumpForceAdditional;
        UpdateModel();
    }

    
    // Chỉ bật đúng 1 trong 4 model dựa vào (IsBig, IsShielded)
    private void UpdateModel()
    {
        if (modelSmall)        modelSmall.SetActive       (!IsBig && !IsShielded);
        if (modelBig)          modelBig.SetActive         ( IsBig && !IsShielded);
        if (modelSmallShield)  modelSmallShield.SetActive (!IsBig &&  IsShielded);
        if (modelBigShield)    modelBigShield.SetActive   ( IsBig &&  IsShielded);

        // Đồng bộ Animator theo model đang active
        GameObject activeModel = IsBig
            ? (IsShielded ? modelBigShield   : modelBig)
            : (IsShielded ? modelSmallShield : modelSmall);

        if (activeModel != null)
        {
            Animator anim = activeModel.GetComponentInChildren<Animator>();
            playerController.SetAnimator(anim);
            playerHealth?.SetAnimator(anim);
        }
    }

    /// <summary>
    /// Gọi từ CheckpointManager sau khi respawn.
    /// Reset animator về trạng thái idle, xoá Die state.
    /// </summary>
    public void ResetAfterDeath()
    {
        // Xác định model đang dùng
        GameObject activeModel = IsBig
            ? (IsShielded ? modelBigShield   : modelBig)
            : (IsShielded ? modelSmallShield : modelSmall);

        if (activeModel != null)
        {
            Animator anim = activeModel.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.Rebind();   // xoá toàn bộ trigger còn dư, trở về Entry state
                anim.Update(0f); // force cập nhật ngay lập tức
            }
        }

        // Sync lại model (đảm bảo đúng model hiện)
        UpdateModel();
    }

    // ─── Va chạm ──────────────────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ăn nấm
        if (collision.CompareTag("PointItem"))
        {
            GrowBig();
            Destroy(collision.gameObject);
            return;
        }

        // Chạm địch
        if (collision.CompareTag("Enemy"))
        {
            // Shield đang bật → miễn sát thương
            if (IsShielded)
                return;

            // Stomp: player rơi xuống + chạm đỉnh enemy
            Rigidbody2D rb        = GetComponent<Rigidbody2D>();
            Collider2D  playerCol = GetComponent<Collider2D>();

            if (rb != null && playerCol != null
                && rb.linearVelocity.y < 0f
                && playerCol.bounds.min.y >= collision.bounds.center.y)
            {
                EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.Stomp();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, playerController.JumpForce * 0.7f);
                }
                return;
            }

            
            if (IsBig)
                ShrinkSmall();
            else
                playerHealth?.Die();
        }
    }
}
