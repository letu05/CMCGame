using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
[Header("Player Settings")]
[SerializeField] private float moveSpeed = 5f;
[SerializeField] private float stopThreshold = 0.1f;
[SerializeField] public float jumpForce = 10f;
[SerializeField] private float doubleJumpForce = 8f;
[SerializeField] private LayerMask groundLayer;
[SerializeField] private float groundRayLength = 0.15f;
[SerializeField] private float groundRayInset  = 0.05f;

[Header("Stomp (giẫm lên đầu quái)")]
[SerializeField] private LayerMask enemyLayer;
[SerializeField] private float stompBounce = 8f;  // Lực nảy lên sau khi giẫm


private Rigidbody2D rb;
private BoxCollider2D boxCollider;
private Animator animator;

private bool isFacingRight = true;
private bool isGrounded;
private bool canDoubleJump;
private bool isSliding;

    public float JumpForce { get; set; }

    // Gọi từ PlayerPowerUp mỗi khi đổi model (PlayerSmall <-> PlayerBig)
    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
    }

    private void Start()
{
    rb = GetComponent<Rigidbody2D>();
    boxCollider = GetComponent<BoxCollider2D>();
    animator = GetComponentInChildren<Animator>();
    JumpForce = jumpForce; // Khởi tạo property từ serialized field
}

private void Update()
    {
        Moved();
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if(ControlFreak2.CF2Input.GetButtonDown("Jump"))
        {
            
            Jump();
        }

        UpdateAnimations();
    }

private void Moved()
    {
        
        float moveInput = ControlFreak2.CF2Input.GetAxisRaw("Horizontal");
        
        // 2. Xác định xem có đang "đảo chiều ngược lại" khi đang có vận tốc hay không
        bool isChangingDirection = (moveInput > 0 && rb.linearVelocity.x < -0.1f) || (moveInput < 0 && rb.linearVelocity.x > 0.1f);

        // Bật trạng thái trượt nếu đang ở trên đất và bấm hướng ngược lại với đà di chuyển
        if (isGrounded && isChangingDirection)
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        // 3. Gia tốc và Lực Phanh (Cảm giác đầm của Mario)
        if (isSliding)
        {
            // Lực phanh gấp (Skid Deceleration). Dùng MoveTowards để phanh lết với khoảng cách đều, tạo cảm giác nặng
            float skidDeceleration = 20f; 
            float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, moveInput * moveSpeed, skidDeceleration * Time.deltaTime);
            rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        }
        else
        {
            // Gia tốc chạy bình thường. Có thể nâng/hạ số 35f để nhân vật tăng tốc khởi hành nhanh hay chậm
            float acceleration = 25f;
            float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, moveInput * moveSpeed, acceleration * Time.deltaTime);
            rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        }

        
        if(moveInput > 0 && !isFacingRight || moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }  

private void UpdateAnimations()
    {
        if (animator != null)
        {
            
            animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        }
    }
private void Jump()
    {
        if (isGrounded)//kiem tra groun
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse); 
            if (animator != null)
            {
                animator.SetTrigger("jumpTrigger");
            }
        }
        else if (canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(new Vector2(0f, doubleJumpForce), ForceMode2D.Impulse);
            canDoubleJump = false;
            if (animator != null)
            {
                animator.SetTrigger("jumpTrigger"); 
            }
        }
    }
private bool IsGrounded()
    {
        if (boxCollider == null)
        {
            return false;
        }

        Bounds bounds = boxCollider.bounds;
        float rayY = bounds.min.y + 0.02f;

        Vector2 leftOrigin = new Vector2(bounds.min.x + groundRayInset, rayY);
        Vector2 centerOrigin = new Vector2(bounds.center.x, rayY);
        Vector2 rightOrigin = new Vector2(bounds.max.x - groundRayInset, rayY);

        return RayHitsGround(leftOrigin) || RayHitsGround(centerOrigin) || RayHitsGround(rightOrigin);
    }

private bool RayHitsGround(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundRayLength, groundLayer);
        return hit.collider != null;
    }
private void OnDrawGizmosSelected()
    {
        BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
        if (collider2D == null)
        {
            return;
        }

        Bounds bounds = collider2D.bounds;
        float rayY = bounds.min.y + 0.02f;

        Vector3 leftOrigin = new Vector3(bounds.min.x + groundRayInset, rayY, 0f);
        Vector3 centerOrigin = new Vector3(bounds.center.x, rayY, 0f);
        Vector3 rightOrigin = new Vector3(bounds.max.x - groundRayInset, rayY, 0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftOrigin, leftOrigin + Vector3.down * groundRayLength);
        Gizmos.DrawLine(centerOrigin, centerOrigin + Vector3.down * groundRayLength);
        Gizmos.DrawLine(rightOrigin, rightOrigin + Vector3.down * groundRayLength);
    }
private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
