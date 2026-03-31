using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PointItem : MonoBehaviour
{
    [SerializeField]
    private int TimeToLive = 10;   // Thời gian tồn tại (giây)
    [SerializeField]
    private float moveSpeedX = 3f; // Tốc độ trượt ngang

    private float timer = 0f;
    private float directionX = 1f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(directionX * moveSpeedX, rb.linearVelocity.y);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= TimeToLive)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Chạm Player → pickup
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPowerUp playerPowerUp = collision.gameObject.GetComponent<PlayerPowerUp>();
            if (playerPowerUp != null)
            {
                playerPowerUp.GrowBig();
            }
            Destroy(gameObject);
            return;
        }

        // Chạm tường từ bên ngang → đảo chiều
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                directionX *= -1f;
                break;
            }
        }
    }
}
