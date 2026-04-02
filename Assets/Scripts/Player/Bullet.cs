using UnityEngine;

// BulletType enum nằm trong BulletType.cs (cùng thư mục)

/// <summary>
/// Đặc tính đạn được SET SẴN trong từng prefab qua Inspector.
/// PlayerFire chỉ gọi Init() sau khi spawn – không override bulletType.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    // ────────────────────────────── Inspector ──────────────────────────────
    [Header("Loại đạn (set trong prefab, không đổi runtime)")]
    public BulletType bulletType = BulletType.Dart;

    [Header("Chung")]
    [SerializeField] private float speed    = 12f;
    [SerializeField] private float lifetime = 4f;       // Tự huỷ sau n giây
    [SerializeField] private int   damage   = 1;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Bomb – Vòng cung")]
    [SerializeField] private float arcHeight        = 3f;   // Độ cao vòng cung
    [SerializeField] private float bombDetectRadius = 2.5f; // Bán kính phát hiện quái

    [Header("Boomerang – Quay lại")]
    [SerializeField] private float returnDelay  = 0.35f;    // Giây bay ra trước khi quay
    [SerializeField] private float rotateSpeed  = 540f;     // Độ xoay/giây

    // ────────────────────────────── Runtime ────────────────────────────────
    private Rigidbody2D rb;
    private Transform   owner;          // Player (Boomerang cần để quay về)

    // Bomb
    private bool    isBombArcing;
    private Vector2 bombStart;
    private Vector2 bombTarget;
    private float   arcT;

    // Boomerang
    private bool  isReturning;
    private float returnTimer;

    // ───────────────────────────── Public Init API ──────────────────────────
    /// <summary>
    /// Gọi ngay sau Instantiate.
    /// bulletType đã được baked trong prefab – Init chỉ khởi tạo chuyển động.
    /// </summary>
    public void Init(Vector2 direction, Transform ownerTransform)
    {
        rb              = GetComponent<Rigidbody2D>();
        owner           = ownerTransform;
        rb.gravityScale = 0f;

        switch (bulletType)
        {
            case BulletType.Dart:      InitDart(direction);      break;
            case BulletType.Bomb:      InitBomb(direction);      break;
            case BulletType.Boomerang: InitBoomerang(direction); break;
        }

        Destroy(gameObject, lifetime);
    }

    // ───────────────────────────── Update ──────────────────────────────────
    private void Update()
    {
        switch (bulletType)
        {
            case BulletType.Bomb:      UpdateBomb();      break;
            case BulletType.Boomerang: UpdateBoomerang(); break;
            // Dart: velocity set 1 lần, physics tự lo
        }
    }

    // ═══════════════════════════════ DART ══════════════════════════════════
    private void InitDart(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    // ════════════════════════════════ BOMB ═════════════════════════════════
    private void InitBomb(Vector2 direction)
    {
        Collider2D nearEnemy = Physics2D.OverlapCircle(
            transform.position, bombDetectRadius, enemyLayer);

        if (nearEnemy != null)
        {
            // Quái ở gần: đâm thẳng như Dart
            Vector2 toEnemy = ((Vector2)nearEnemy.transform.position
                               - (Vector2)transform.position).normalized;
            rb.linearVelocity = toEnemy * speed;
            isBombArcing = false;
        }
        else
        {
            // Không có quái: ném vòng cung
            isBombArcing = true;
            bombStart    = transform.position;
            bombTarget   = bombStart + direction.normalized * (speed * 1.5f);
            arcT         = 0f;
        }
    }

    private void UpdateBomb()
    {
        if (!isBombArcing) return;

        arcT += Time.deltaTime * (speed / Vector2.Distance(bombStart, bombTarget));
        arcT  = Mathf.Clamp01(arcT);

        // Parabola bằng Lerp + sin
        Vector2 flatPos      = Vector2.Lerp(bombStart, bombTarget, arcT);
        float   heightOffset = arcHeight * Mathf.Sin(Mathf.PI * arcT);
        transform.position   = new Vector3(flatPos.x, flatPos.y + heightOffset, 0f);

        // Sprite xoay theo tiếp tuyến
        if (arcT < 1f)
        {
            float   nextT      = Mathf.Clamp01(arcT + 0.01f);
            Vector2 flatNext   = Vector2.Lerp(bombStart, bombTarget, nextT);
            float   nextH      = arcHeight * Mathf.Sin(Mathf.PI * nextT);
            Vector2 tangent    = new Vector2(flatNext.x - flatPos.x,
                                             (flatNext.y + nextH) - (flatPos.y + heightOffset));
            transform.rotation = Quaternion.Euler(0f, 0f,
                                     Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg);
        }

        if (arcT >= 1f) Destroy(gameObject);
    }

    // ══════════════════════════════ BOOMERANG ══════════════════════════════
    private void InitBoomerang(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        isReturning       = false;
        returnTimer       = 0f;
    }

    private void UpdateBoomerang()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime); // Xoay sprite

        if (!isReturning)
        {
            returnTimer += Time.deltaTime;
            if (returnTimer >= returnDelay) isReturning = true;
        }
        else
        {
            if (owner == null) { Destroy(gameObject); return; }

            Vector2 toOwner   = ((Vector2)owner.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = toOwner * speed;

            if (Vector2.Distance(transform.position, owner.position) < 0.4f)
                Destroy(gameObject);
        }
    }

    // ══════════════════════════════ HIT ════════════════════════════════════
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        IcanTakeDamage damageable = other.GetComponent<IcanTakeDamage>();
        damageable?.TakeDamage(damage);

        

        Destroy(gameObject);
    }

    // ─── Gizmo ─────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (bulletType == BulletType.Bomb)
        {
            Gizmos.color = new Color(1f, 0.3f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, bombDetectRadius);
        }
    }
}
