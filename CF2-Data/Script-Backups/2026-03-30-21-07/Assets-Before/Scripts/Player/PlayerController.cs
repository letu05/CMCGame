using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
[Header("Player Settings")]
[SerializeField] private float moveSpeed = 5f;
[SerializeField] private float stopThreshold = 0.1f; // ngưỡng dừng để tránh việc player bị trôi khi không có input
[SerializeField] private float jumpForce = 10f;
[SerializeField] private float doubleJumpForce = 8f;
[SerializeField] private LayerMask groundLayer;
[SerializeField] private float groundRayLength = 0.15f;// ban tiam chieu dai cua raycast de kiem tra xem player co dang o tren mat dat hay khong
[SerializeField] private float groundRayInset = 0.05f; // khoang cach tu canh cua box collider den diem bat dau cua raycast de tranh truong hop raycast bi cham vao tuong hoac vat the khac


private Rigidbody2D rb;
private BoxCollider2D boxCollider;
private Animator animator;

private bool isFacingRight = true;
private bool isGrounded;
private bool canDoubleJump;
private bool isSliding;

private void Start()
{
    rb = GetComponent<Rigidbody2D>();
    boxCollider = GetComponent<BoxCollider2D>();
    animator = GetComponentInChildren<Animator>();
}

private void Update()
    {
        Moved();
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        UpdateAnimations();
    }

private void Moved()
    {
        // 1. Dùng GetAxisRaw để input nhận ngay -1, 0, 1 (Giống bấm nút D-Pad cứng của Mario, giúp không bị trôi nổi/delay)
        float moveInput = Input.GetAxisRaw("Horizontal");
        
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
            float acceleration = 35f;
            float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, moveInput * moveSpeed, acceleration * Time.deltaTime);
            rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        }

        // 4. Đảo chiều hiển thị của nhân vật (Luôn quay mặt NGAY LẬP TỨC theo nút bấm, dù đang trượt lùi)
        if(moveInput > 0 && !isFacingRight || moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }  

private void UpdateAnimations()
    {
        if (animator != null)
        {
            // Trả lại Animator Parameter Bool để DUY TRÌ đúng frame dáng trượt Skid đến khi phanh xong giống y hệt Mario
            animator.SetBool("isSliding", isSliding);
            animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        }
    }
private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
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
                animator.SetTrigger("jumpTrigger"); // Gọi trigger cả khi Double Jump
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
