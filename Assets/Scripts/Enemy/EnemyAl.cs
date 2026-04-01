using UnityEngine;

public class EnemyAl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    
    private Transform targetPoint;

    private bool movingRight = true;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPoint = rightPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
        Move();
    }
    private void Move()
    {
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            if (transform.position.x >= targetPoint.position.x)
            {
                movingRight = false;
                targetPoint = leftPoint;
                Flip();
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            if (transform.position.x <= targetPoint.position.x)
            {
                movingRight = true;
                targetPoint = rightPoint;
                Flip();
            }
        }
    }
    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Đảo ngược chiều của sprite
        transform.localScale = scale;
    }
}
